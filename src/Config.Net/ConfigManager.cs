using System;
using System.Collections.Generic;
using Config.Net.TypeParsers;

namespace Config.Net
{
   internal class ConfigManager : IConfigSource
   {
      private readonly IConfigConfiguration _cfg;
      private readonly DefaultParser _defaultParser;
      private readonly object _storeLock = new object();
      private readonly Dictionary<string, object> _keyToProperty = new Dictionary<string, object>();

      public ConfigManager()
      {
         _cfg = GlobalConfiguration.Instance;
         _defaultParser = GlobalConfiguration.Instance.DefaultParser;
      }

      public Property<T> Read<T>(Setting<T> key)
      {
         lock (_storeLock)
         {
            T value;
            if (!ReadValue(key, out value))
            {
               value = key.DefaultValue;
            }

            return AsProperty(key, value);
         }
      }

      public Property<T?> Read<T>(Setting<T?> key) where T : struct
      {
         T? value;

         string rawValue = Read(key.Name, key.AlsoKnownAs);
         if (rawValue == null)
         {
            value = null;
         }
         else
         {
            var notNullableKey = new Setting<T>(
               key.Name,
               default(T));

            T notNullableValue;
            if (ReadValue(notNullableKey, out notNullableValue))
            {
               value = notNullableValue;
            }
            else
            {
               value = null;
            }
         }

         return AsProperty(key, value);
      }

      /// <summary>
      /// Responsible for returning the Property for raw value.
      /// If values is changed it simply returns the cached Property and calls ChangeValue on it.
      /// </summary>
      private Property<T> AsProperty<T>(Setting<T> key, T value)
      {
         if (!_keyToProperty.ContainsKey(key.Name))
         {
            var result = new Property<T>(value, GetRawStringValue(value), AreEqual(value, key.DefaultValue));
            _keyToProperty[key.Name] = result;
            return result;
         }
         else
         {
            var result = (Property<T>)_keyToProperty[key.Name];
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

      private bool ReadValue<T>(Setting<T> key, out T result)
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         if(!GlobalConfiguration.Instance.CanParse(key.ValueType))
         {
            throw new ArgumentException("value parser for " + key.ValueType.FullName +
                                        " is not registered and not supported by default parser");
         }

         string value = ReadFirst(key.Name, key.AlsoKnownAs);
         if(_defaultParser.IsSupported(typeof(T)))
         {
            object resultObject;
            if(_defaultParser.TryParse(value, typeof(T), out resultObject))
            {
               result = (T)resultObject;
               return true;
            }

            result = key.DefaultValue;
            return false;
         }

         ITypeParser<T> typeParser = _cfg.GetParser<T>();
         return typeParser.TryParse(value, out result);
      }

      public void Write<T>(Setting<T> key, T value)
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

      public void Write<T>(Setting<T?> key, T? value) where T : struct
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         if(!GlobalConfiguration.Instance.CanParse(key.ValueType))
         {
            throw new ArgumentException("value parser for " + key.ValueType.FullName +
                                        " is not registered and not supported by default parser");
         }
         lock (_storeLock)
         {
            var nonNullableKey = new Setting<T>(
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
         ITypeParser<T> typeParser = _cfg.GetParser<T>();
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

      private void WriteValue<T>(Setting<T> key, string value)
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

      private string Read(string key, string[] alsoKnownAs)
      {
         if(key == null) throw new ArgumentNullException(nameof(key));

         lock(_storeLock)
         {
            return ReadFirst(key, alsoKnownAs);
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
