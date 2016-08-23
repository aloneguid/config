using System;
using System.Collections.Generic;
using Config.Net.TypeParsers;

namespace Config.Net
{
   internal class ConfigManager
   {
      private readonly IConfigConfiguration _cfg;
      private readonly DefaultParser _defaultParser;
      private readonly object _storeLock = new object();
      private readonly Dictionary<string, SettingTag> _keyToTag = new Dictionary<string, SettingTag>();

      private class SettingTag
      {
         private DateTime _readOn;
         private object _value;

         public SettingTag(object value)
         {
            _readOn = DateTime.UtcNow;
            _value = value;
         }

         public object Value
         {
            get { return _value; }
         }

         public void Update()
         {
            _readOn = DateTime.UtcNow;
         }

         public bool IsExpired(TimeSpan timeout)
         {
            if (timeout == TimeSpan.Zero) return true;

            return (DateTime.UtcNow - _readOn) > timeout;
         }
      }

      public ConfigManager()
      {
         _cfg = GlobalConfiguration.Instance;
         _defaultParser = GlobalConfiguration.Instance.DefaultParser;
      }

      public OptionValue<T> Read<T>(Option<T> key)
      {
         lock (_storeLock)
         {
            OptionValue<T> result = GetCached(key);
            if (result != null) return result;

            T value;
            if (!ReadValue(key.Name, key.AlsoKnownAs, key.ValueType, out value))
            {
               value = key.DefaultValue;
            }

            return AsProperty(key, value);
         }
      }

      public OptionValue<T?> Read<T>(Option<T?> key) where T : struct
      {
         lock(_storeLock)
         {
            OptionValue<T?> result = GetCached(key);
            if (result != null) return result;

            T? nullableValue;
            T value;
            if(!ReadValue(key.Name, key.AlsoKnownAs, typeof(T), out value))
            {
               nullableValue = null;
            }
            else
            {
               nullableValue = value;
            }

            return AsProperty(key, nullableValue);
         }
      }

      private OptionValue<T> GetCached<T>(Option<T> key)
      {
         SettingTag tag;
         if (!_keyToTag.TryGetValue(key.Name, out tag)) return null;

         if (tag.IsExpired(_cfg.CacheTimeout)) return null;

         return (OptionValue<T>)tag.Value;
      }

      /// <summary>
      /// Responsible for returning the Property for raw value.
      /// If values is changed it simply returns the cached Property and calls ChangeValue on it.
      /// </summary>
      private OptionValue<T> AsProperty<T>(Option<T> key, T value)
      {
         if (!_keyToTag.ContainsKey(key.Name))
         {
            var result = new OptionValue<T>(value, GetRawStringValue(value), AreEqual(value, key.DefaultValue));
            _keyToTag[key.Name] = new SettingTag(result);
            return result;
         }
         else
         {
            SettingTag tag = _keyToTag[key.Name];
            var result = (OptionValue<T>)tag.Value;
            if (!AreEqual(value, result.Value))
            {
               result.ChangeValue(value, GetRawStringValue(value), AreEqual(value, key.DefaultValue));
            }
            return result;
         }
      }

      private bool AreEqual(object value1, object value2)
      {
         if(value1 != null && value2 != null)
         {
            Type t1 = value1.GetType();
            Type t2 = value2.GetType();

            if(t1.IsArray && t2.IsArray)
            {
               return AreEqual((Array)value1, (Array)value2);
            }
         }

         return value1 != null && value1.Equals(value2);
      }

      private bool AreEqual(Array a, Array b)
      {
         if(a == null && b == null) return true;

         if(a == null || b == null) return false;

         if(a.Length != b.Length) return false;

         for(int i = 0; i < a.Length; i++)
         {
            object obj1 = a.GetValue(i);
            object obj2 = b.GetValue(i);

            if(!AreEqual(obj1, obj2)) return false;
         }

         return true;
      }

      private bool ReadValue<T>(string keyName, string[] alsoKnownAs, Type valueType, out T result)
      {
         if(!GlobalConfiguration.Instance.CanParse(valueType))
         {
            throw new ArgumentException("value parser for " + valueType.FullName +
                                        " is not registered and not supported by default parser");
         }

         string value = ReadFirst(keyName, alsoKnownAs);
         if(value == null)
         {
            result = default(T);
            return false;
         }

         if(_defaultParser.IsSupported(typeof(T)))
         {
            object resultObject;
            if(_defaultParser.TryParse(value, typeof(T), out resultObject))
            {
               result = (T)resultObject;
               return true;
            }

            result = default(T);
            return false;
         }

         ITypeParser typeParser = _cfg.GetParser(typeof(T));
         object objResult;
         bool parsed = typeParser.TryParse(value, typeof(T), out objResult);
         result = (T)objResult;
         return parsed;
      }

      public void Write<T>(Option<T> key, T value)
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         if(!GlobalConfiguration.Instance.CanParse(key.ValueType))
         {
            throw new ArgumentException("value parser for " + key.ValueType.FullName + " is not registered and not supported by default parser");
         }
         lock (_storeLock)
         {
            string stringValue = AreEqual(value, key.DefaultValue) ? null : GetRawStringValue(value);

            WriteValue(key, stringValue);
         }
      }

      public void Write<T>(Option<T?> key, T? value) where T : struct
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         if(!GlobalConfiguration.Instance.CanParse(key.ValueType))
         {
            throw new ArgumentException("value parser for " + key.ValueType.FullName +
                                        " is not registered and not supported by default parser");
         }
         lock (_storeLock)
         {
            var nonNullableKey = new Option<T>(
               key.Name,
               default(T));

            string stringValue = null;

            if(value != null)
            {
               T notNullableValue = (T)value;

               stringValue = AreEqual(value, key.DefaultValue) ? null : GetRawStringValue(notNullableValue);
            }
            WriteValue(nonNullableKey, stringValue);
         }
      }

      private string GetRawStringValue<T>(T value)
      {
         string stringValue = null;
         ITypeParser typeParser = _cfg.GetParser(typeof(T));
         if (typeParser != null)
         {
            stringValue = typeParser.ToRawString(value);
         }
         else
         {
            if (_defaultParser.IsSupported(typeof(T)))
            {
               stringValue = _defaultParser.ToRawString(value);
            }
         }
         return stringValue;
      }

      private void WriteValue<T>(Option<T> key, string value)
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         foreach(IConfigStore store in _cfg.Stores)
         {
            if(store.CanWrite)
            {
               try
               {
                  store.Write(key.Name, value);
               }
               catch(Exception e)
               {
                  throw new InvalidOperationException("could not write value", e);
               }
            }
         }
      }

      private string ReadFirst(string key, string[] alsoKnownAs)
      {
         bool hasAlternatives = alsoKnownAs != null && alsoKnownAs.Length > 0;

         foreach(IConfigStore store in _cfg.Stores)
         {
            if(store.CanRead)
            {
               string value = store.Read(key);

               //try to read by alternative key
               if(value == null && hasAlternatives)
               {
                  foreach(string altName in alsoKnownAs)
                  {
                     value = store.Read(altName);
                     if(value != null) break;
                  }
               }

               if(value != null) return value;
            }
         }
         return null;
      }
   }
}
