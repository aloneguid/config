using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Config.Net.Core.Box
{
   static class BoxFactory
   {
      public static Dictionary<string, ResultBox> Discover(Type t)
      {
         var result = new Dictionary<string, ResultBox>();

         DiscoverProperties(t, result);

         DiscoverMethods(t, result);

         return result;
      }

      private static void DiscoverProperties(Type t, Dictionary<string, ResultBox> result)
      {
         IEnumerable<PropertyInfo> properties = GetHierarchyPublicProperties(t);

         foreach (PropertyInfo pi in properties)
         {
            ResultBox pbox = pi.PropertyType.GetTypeInfo().IsInterface
               ? (ResultBox)new ProxyResultBox(pi.Name, pi.PropertyType)
               : (ResultBox)new PropertyResultBox(pi.Name, pi.PropertyType);

            AddAttributes(pbox, pi);

            result[pi.Name] = pbox;
         }
      }

      private static void DiscoverMethods(Type t, Dictionary<string, ResultBox> result)
      {
         TypeInfo ti = t.GetTypeInfo();

         IEnumerable<MethodInfo> methods = ti.DeclaredMethods.Where(m => !m.IsSpecialName);

         foreach (MethodInfo method in methods)
         {
            var mbox = new MethodResultBox(method);

            AddAttributes(mbox, method);

            result[mbox.Name] = mbox;
         }
      }

      private static object GetDefaultValue(Type t)
      {
         if (t.GetTypeInfo().IsValueType) return Activator.CreateInstance(t);

         return null;
      }

      private static void AddAttributes(ResultBox box, PropertyInfo pi)
      {
         AddAttributes(box, pi.GetCustomAttribute<OptionAttribute>());
      }

      private static void AddAttributes(ResultBox box, MethodInfo mi)
      {
         AddAttributes(box, mi.GetCustomAttribute<OptionAttribute>());
      }


      private static void AddAttributes(ResultBox box, OptionAttribute optionAttribute)
      {
         object defaultValue = null;

         if (optionAttribute != null)
         {
            if (optionAttribute.Alias != null) box.StoreByName = optionAttribute.Alias;

            //validate that types for default value match
            Type dvt = optionAttribute.DefaultValue?.GetType();
            if (optionAttribute.DefaultValue != null)
            {
               if (dvt != box.ResultType && dvt != typeof(string))
               {
                  throw new InvalidCastException($"Default value for option {box.Name} is of type {dvt.FullName} whereas the property has type {box.Name}. To fix this, either set default value to type {box.ResultType.FullName} or a string parseable to the target type.");
               }

               if (box.ResultType != typeof(string) && dvt == typeof(string))
               {
                  ValueHandler.Default.TryParse(box.ResultType, (string)optionAttribute.DefaultValue, out defaultValue);
               }
            }

            if (defaultValue == null)
            {
               defaultValue = optionAttribute.DefaultValue;
            }
         }

         if (defaultValue == null) defaultValue = GetDefaultValue(box.ResultType);

         box.DefaultResult = defaultValue;
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
