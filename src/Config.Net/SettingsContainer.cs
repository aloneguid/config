﻿using Config.Net.TypeParsers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Config.Net
{
   /// <summary>
   /// Generic container for test settings
   /// </summary>
   public abstract class SettingsContainer
   {
      private readonly IConfigConfiguration _config = new ContainerConfiguration();

      private readonly ConcurrentDictionary<string, Option> _nameToOption =
         new ConcurrentDictionary<string, Option>();

      private readonly ConcurrentDictionary<string, OptionValue> _nameToOptionValue =
         new ConcurrentDictionary<string, OptionValue>();

      private static readonly DefaultParser DefaultParser = new DefaultParser();

      private readonly string _namespace;
      private bool _isConfigured;

      /// <summary>
      /// Constructs the container in default namespace
      /// </summary>
      protected SettingsContainer() : this(null)
      {

      }

      /// <summary>
      /// Constructs the container allowing to specify a custom namespace
      /// </summary>
      /// <param name="namespaceName"></param>
      protected SettingsContainer(string namespaceName)
      {
         _namespace = namespaceName;

         DiscoverProperties();
      }

      /// <summary>
      /// Reads the option value
      /// </summary>
      /// <typeparam name="T">Option type</typeparam>
      /// <param name="option">Option reference</param>
      /// <returns>Option value</returns>
      public T Read<T>(Option<T> option)
      {
         CheckConfigured();

         CheckCanParse(option.NonNullableType);

         OptionValue optionValue;
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
            if (typeParser.TryParse(value, option.NonNullableType, out result))
            {
               optionValue.Update<T>((T)result);
            }
         }

         OnReadOption(option, optionValue.RawValue);

         return (T)optionValue.RawValue;
      }

      /// <summary>
      /// Writes a new value to the option
      /// </summary>
      /// <typeparam name="T">Option type</typeparam>
      /// <param name="option">Option reference</param>
      /// <param name="value">New value</param>
      public void Write<T>(Option<T> option, T value)
      {
         CheckConfigured();

         CheckCanParse(option.NonNullableType);

         OptionValue optionValue;
         _nameToOptionValue.TryGetValue(option.Name, out optionValue);

         foreach (IConfigStore store in _config.Stores)
         {
            if (store.CanWrite)
            {
               string rawValue = AreEqual(value, option.DefaultValue) ? null : GetRawStringValue(option, value);
               store.Write(option.Name, rawValue);
               break;
            }
         }

         optionValue.Update(value);
         OnWriteOption(option, value);
      }

      /// <summary>
      /// This method is called internally before containers is ready for use. You can specify
      /// configuration stores or any other options here.
      /// </summary>
      /// <param name="configuration"></param>
      protected abstract void OnConfigure(IConfigConfiguration configuration);

      /// <summary>
      /// Called after any value is read
      /// </summary>
      /// <param name="option">Optiond that is read</param>
      /// <param name="value">Option value read from a store</param>
      protected virtual void OnReadOption(Option option, object value)
      {
      }

      /// <summary>
      /// Called before any value is written
      /// </summary>
      /// <param name="option">Option that is written</param>
      /// <param name="value">Option value to write</param>
      protected virtual void OnWriteOption(Option option, object value)
      {
      }

      private void CheckConfigured()
      {
         if (_isConfigured) return;

         OnConfigure(_config);

         _isConfigured = true;
      }

      private void DiscoverProperties()
      {
         Type t = this.GetType();
         Type optionType = typeof(Option);

         IEnumerable<FieldInfo> properties = t.GetRuntimeFields();
         foreach (FieldInfo pi in properties)
         {
            TypeInfo propInfo = pi.FieldType.GetTypeInfo();

            if (propInfo.IsSubclassOf(optionType))
            {
               if ((pi.Attributes & FieldAttributes.InitOnly) == 0)
               {
                  throw new ArgumentException($"field '{pi.Name}' must be declared as read-only");
               }

               if ((pi.Attributes & FieldAttributes.Static) != 0)
               {
                  throw new ArgumentException($"field '{pi.Name}' cannot be static");
               }

               //check if it has the value
               object objValue = pi.GetValue(this);

               //if (objValue == null) throw new ApplicationException($"option '{pi.Name}' must be initialised.");
               if (objValue == null)
               {
                  //create default instance if it doesn't exist
                  var nt = typeof(Option<>);
                  Type[] ntArgs = propInfo.GetGenericArguments();
                  Type ntGen = nt.MakeGenericType(ntArgs);
                  objValue = Activator.CreateInstance(ntGen);

                  //set the instance value back to the container
                  pi.SetValue(this, objValue);
               }

               Option value = (Option)objValue;
               if (string.IsNullOrEmpty(value.Name)) value.Name = pi.Name;
               value.Name = GetFullKeyName(value.Name);
               value._parent = this;
               value.NonNullableType = Nullable.GetUnderlyingType(value.ValueType);
               value.IsNullable = value.NonNullableType != null;
               if (value.NonNullableType == null) value.NonNullableType = value.ValueType;

               _nameToOption[value.Name] = value;
               _nameToOptionValue[value.Name] = new OptionValue();
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
         if (value1 == null && value2 == null) return true;

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

      private string GetRawStringValue<T>(Option<T> option, T value)
      {
         string stringValue = null;
         ITypeParser typeParser = _config.GetParser(option.NonNullableType);
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