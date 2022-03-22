﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Config.Net.Core.Box
{
   static class BoxFactory
   {
      public static Dictionary<string, ResultBox> Discover(Type t, ValueHandler valueHandler, string basePath)
      {
         var result = new Dictionary<string, ResultBox>();

         DiscoverProperties(t, valueHandler, result, basePath);

         DiscoverMethods(t, valueHandler, result);

         return result;
      }

      private static void DiscoverProperties(Type t, ValueHandler valueHandler, Dictionary<string, ResultBox> result, string basePath)
      {
         IEnumerable<PropertyInfo> properties = GetHierarchyPublicProperties(t);

         foreach (PropertyInfo pi in properties)
         {
            Type propertyType = pi.PropertyType;
            ResultBox rBox;
            bool isCollection = false;

            if(ResultBox.TryGetCollection(propertyType, out propertyType))
            {
               if(pi.SetMethod != null)
               {
                  throw new NotSupportedException($"Collection properties cannot have a setter. Detected at '{OptionPath.Combine(basePath, pi.Name)}'");
               }

               isCollection = true;
            }

            if(propertyType.GetTypeInfo().IsInterface)
            {
               rBox = new ProxyResultBox(pi.Name, propertyType);
            }
            else
            {
               rBox = new PropertyResultBox(pi.Name, propertyType);
            }

            ValidateSupportedType(rBox, valueHandler);

            AddAttributes(rBox, pi, valueHandler);

            //adjust to collection
            if(isCollection)
            {
               rBox = new CollectionResultBox(pi.Name, rBox);
            }

            result[pi.Name] = rBox;
         }
      }

      private static void DiscoverMethods(Type t, ValueHandler valueHandler, Dictionary<string, ResultBox> result)
      {
         TypeInfo ti = t.GetTypeInfo();

         IEnumerable<MethodInfo> methods = ti.DeclaredMethods.Where(m => !m.IsSpecialName);

         foreach (MethodInfo method in methods)
         {
            var mBox = new MethodResultBox(method);

            AddAttributes(mBox, method, valueHandler);

            result[mBox.Name] = mBox;
         }
      }

      private static void ValidateSupportedType(ResultBox rb, ValueHandler valueHandler)
      {
         Type t = null;

         if (rb is PropertyResultBox pBox)
            t = rb.ResultBaseType;

         if (t != null && !valueHandler.IsSupported(t))
         {
            throw new NotSupportedException($"type {t} on object '{rb.Name}' is not supported.");
         }
      }

      private static object GetDefaultValue(Type t)
      {
         if (t.GetTypeInfo().IsValueType) return Activator.CreateInstance(t);

         return null;
      }

      private static void AddAttributes(ResultBox box, PropertyInfo pi, ValueHandler valueHandler)
      {
         AddAttributes(box, valueHandler, pi.GetCustomAttribute<OptionAttribute>(), pi.GetCustomAttribute<DefaultValueAttribute>());
      }

      private static void AddAttributes(ResultBox box, MethodInfo mi, ValueHandler valueHandler)
      {
         AddAttributes(box, valueHandler, mi.GetCustomAttribute<OptionAttribute>(), mi.GetCustomAttribute<DefaultValueAttribute>());
      }


      private static void AddAttributes(ResultBox box, ValueHandler valueHandler, params Attribute[] attributes)
      {
         OptionAttribute optionAttribute = attributes.OfType<OptionAttribute>().FirstOrDefault();
         DefaultValueAttribute defaultValueAttribute = attributes.OfType<DefaultValueAttribute>().FirstOrDefault();

         if (optionAttribute?.Alias != null)
         {
            box.StoreByName = optionAttribute.Alias;
         }

         box.DefaultResult = GetDefaultValue(optionAttribute?.DefaultValue, box, valueHandler) ??
                             GetDefaultValue(defaultValueAttribute?.Value, box, valueHandler) ??
                             GetDefaultValue(box.ResultType);
      }

      private static object GetDefaultValue(object defaultValue, ResultBox box, ValueHandler valueHandler)
      {
         object result = null;
         if (defaultValue != null)
         {
            //validate that types for default value match
            Type dvt = defaultValue?.GetType();

            if (dvt != box.ResultType && dvt != typeof(string))
            {
               throw new InvalidCastException($"Default value for option {box.Name} is of type {dvt.FullName} whereas the property has type {box.Name}. To fix this, either set default value to type {box.ResultType.FullName} or a string parseable to the target type.");
            }

            if (box.ResultType != typeof(string) && dvt == typeof(string))
            {
               valueHandler.TryParse(box.ResultType, (string)defaultValue, out result);
            }
         }

         if (result == null)
         {
            result = defaultValue;
         }

         return result;
      }

      private static PropertyInfo[] GetHierarchyPublicProperties(Type type)
      {
         var propertyInfos = new List<PropertyInfo>();

         var considered = new List<TypeInfo>();
         var queue = new Queue<TypeInfo>();
         considered.Add(type.GetTypeInfo());
         queue.Enqueue(type.GetTypeInfo());

         while (queue.Count > 0)
         {
            TypeInfo typeInfo = queue.Dequeue();

            //add base interfaces to the queue
            foreach (Type subInterface in typeInfo.ImplementedInterfaces)
            {
               TypeInfo subInterfaceTypeInfo = subInterface.GetTypeInfo();

               if (considered.Contains(subInterfaceTypeInfo)) continue;

               considered.Add(subInterfaceTypeInfo);
               queue.Enqueue(subInterfaceTypeInfo);
            }

            //add base classes to the queue
            if (typeInfo.BaseType != null)
            {
               TypeInfo baseType = typeInfo.BaseType.GetTypeInfo();

               if (!considered.Contains(baseType))
               {
                  considered.Add(baseType);
                  queue.Enqueue(baseType);
               }
            }


            //get properties from the current type
            IEnumerable<PropertyInfo> newProperties = typeInfo.DeclaredProperties.Where(p => !propertyInfos.Contains(p));
            propertyInfos.InsertRange(0, newProperties);
         }

         return propertyInfos.ToArray();
      }


   }
}
