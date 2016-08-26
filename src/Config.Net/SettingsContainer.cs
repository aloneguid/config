using System;
using System.Reflection;
using System.Collections.Concurrent;
using Config.Net.TypeParsers;

namespace Config.Net
{
   public abstract class SettingsContainer
   {
      private readonly IConfigConfiguration _config = new ContainerConfiguration();

      private readonly ConcurrentDictionary<string, Option> _nameToOption =
         new ConcurrentDictionary<string, Option>();

      private readonly ConcurrentDictionary<string, Value> _nameToOptionValue =
         new ConcurrentDictionary<string, Value>();

      private static readonly DefaultParser DefaultParser = new DefaultParser();

      private readonly string _namespace;
      private bool _isConfigured;

      protected SettingsContainer() : this(null)
      {

      }

      protected SettingsContainer(string namespaceName)
      {
         _namespace = namespaceName;

         DiscoverProperties();
      }

      /*public object Read(Type valueType, string name, object defaultValue)
      {
         CheckConfigured();

         object result = ReadTypedValue(name, valueType);
         return result ?? defaultValue;
      }*/

      public T Read<T>(Option<T> option)
      {
         CheckConfigured();

         CheckCanParse(option.NonNullableType);

         Value optionValue;
         _nameToOptionValue.TryGetValue(option.Name, out optionValue);

         if (!optionValue.IsExpired(_config.CacheTimeout))
         {
            return (T)optionValue.RawValue;
         }

         string value = ReadFirstValue(option.Name);
         if (value == null)
         {
            optionValue.RawValue = option.DefaultValue;
         }
         else if (DefaultParser.IsSupported(option.NonNullableType))
         {
            object resultObject;
            if (DefaultParser.TryParse(value, option.NonNullableType, out resultObject))
            {
               optionValue.Update<T>((T)resultObject);
            }
            else
            {
               optionValue.Update(option.DefaultValue);
            }
         }
         else
         {
            ITypeParser typeParser = _config.GetParser(option.NonNullableType);
            object result;
            typeParser.TryParse(value, option.NonNullableType, out result);
            optionValue.Update<T>((T)result);
         }

         return (T)optionValue.RawValue;
      }

      public T? Read<T>(Option<T?> option) where T : struct
      {
         CheckConfigured();

         throw new NotImplementedException();
      }

      public void Write<T>(Option<T> option, T value)
      {
         CheckConfigured();

         Value optionValue;
         _nameToOptionValue.TryGetValue(option.Name, out optionValue);

         foreach(IConfigStore store in _config.Stores)
         {
            if(store.CanWrite)
            {
               string rawValue = AreEqual(value, option.DefaultValue) ? null : GetRawStringValue(value);
               store.Write(option.Name, rawValue);
               break;
            }
         }

         optionValue.Update(value);
      }

      public void Write<T>(Option<T?> option, T? value) where T : struct
      {
         CheckConfigured();

         throw new NotImplementedException();
      }

      protected abstract void OnConfigure(IConfigConfiguration configuration);

      private void CheckConfigured()
      {
         if (_isConfigured) return;

         OnConfigure(_config);

         _isConfigured = true;
      }

      private void DiscoverProperties()
      {
         Type t = this.GetType();

         FieldInfo[] properties = t.GetFields();
         foreach (FieldInfo pi in properties)
         {
            if (pi.FieldType.IsSubclassOf(typeof(Option)))
            {
               //check if it has the value
               object objValue = pi.GetValue(this);

               //if (objValue == null) throw new ApplicationException($"option '{pi.Name}' must be initialised.");
               if (objValue == null)
               {
                  //create default instance if it doesn't exist
                  var nt = typeof(Option<>);
                  Type[] ntArgs = pi.FieldType.GetGenericArguments();
                  Type ntGen = nt.MakeGenericType(ntArgs);
                  objValue = Activator.CreateInstance(ntGen);
               }

               Option value = (Option)objValue;
               if (string.IsNullOrEmpty(value.Name)) value.Name = pi.Name;
               value.Name = GetFullKeyName(value.Name);
               value._parent = this;
               value.NonNullableType = Nullable.GetUnderlyingType(value.ValueType);
               value.IsNullable = value.NonNullableType != null;
               if (value.NonNullableType == null) value.NonNullableType = value.ValueType;

               _nameToOption[value.Name] = value;
               _nameToOptionValue[value.Name] = new Value();
            }
         }
      }

      private string GetFullKeyName(string name)
      {
         if (string.IsNullOrEmpty(_namespace)) return name;

         return _namespace + "." + name;
      }

      private bool CanParse(Type t)
      {
         return _config.HasParser(t) || DefaultParser.IsSupported(t);
      }

      private string ReadFirstValue(string key)
      {
         foreach (IConfigStore store in _config.Stores)
         {
            if (store.CanRead)
            {
               string value = store.Read(key);

               if (value != null) return value;
            }
         }
         return null;
      }

      private void CheckCanParse(Type t)
      {
         if (!CanParse(t))
         {
            throw new ArgumentException("value parser for " + t.FullName +
                                        " is not registered and not supported by default parser");
         }
      }

      private bool AreEqual(object value1, object value2)
      {
         if (value1 != null && value2 != null)
         {
            Type t1 = value1.GetType();
            Type t2 = value2.GetType();

            if (t1.IsArray && t2.IsArray)
            {
               return AreEqual((Array)value1, (Array)value2);
            }
         }

         return value1 != null && value1.Equals(value2);
      }

      private bool AreEqual(Array a, Array b)
      {
         if (a == null && b == null) return true;

         if (a == null || b == null) return false;

         if (a.Length != b.Length) return false;

         for (int i = 0; i < a.Length; i++)
         {
            object obj1 = a.GetValue(i);
            object obj2 = b.GetValue(i);

            if (!AreEqual(obj1, obj2)) return false;
         }

         return true;
      }

      private string GetRawStringValue<T>(T value)
      {
         string stringValue = null;
         ITypeParser typeParser = _config.GetParser(typeof(T));
         if (typeParser != null)
         {
            stringValue = typeParser.ToRawString(value);
         }
         else
         {
            if (DefaultParser.IsSupported(typeof(T)))
            {
               stringValue = DefaultParser.ToRawString(value);
            }
         }
         return stringValue;
      }

   }
}