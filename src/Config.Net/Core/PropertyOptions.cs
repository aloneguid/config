using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Config.Net.Core
{
   class PropertyOptions
   {
      public const string PathSeparator = ".";

      public PropertyOptions(string name, Type type, object defaultValue, bool? isGetter)
      {
         Name = name;
         StoreName = name;

         Type = type;

         TypeInfo ti = type.GetTypeInfo();
         if(ti.IsClass)
         {
            BaseType = type;
         }
         else
         {
            if(ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
               BaseType = ti.GenericTypeArguments[0];
            }
            else
            {
               BaseType = type;
            }
         }

         DefaultValue = defaultValue;
         IsGetter = isGetter;
      }

      public string Name { get; set; }

      public string StoreName { get; set; }

      public Type Type { get; }

      public Type BaseType { get; }

      public object DefaultValue { get; set;  }

      /// <summary>
      /// When set, this is either a getter or a setter, otherwise it can be either
      /// </summary>
      public bool? IsGetter { get; }

      public static Dictionary<string, PropertyOptions> Discover<TInterface>()
      {
         var result = new Dictionary<string, PropertyOptions>();

         DiscoverProperties<TInterface>(result);

         DiscoverMethods<TInterface>(result);

         return result;
      }

      /// <summary>
      /// Composes a uniqueue method name using method name itself and parameter type names, separated by underscore
      /// </summary>
      public static string GetMethodName(MethodInfo mi)
      {
         ParameterInfo[] parameters = mi.GetParameters();
         var sb = new StringBuilder();
         sb.Append(mi.Name);
         foreach(ParameterInfo pi in parameters)
         {
            sb.Append("-");
            sb.Append(pi.ParameterType.ToString());
         }
         return sb.ToString();
      }

      private static string GetMethodStoreName(MethodInfo mi)
      {
         string name = mi.Name;

         if(name.StartsWith("get", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("set", StringComparison.OrdinalIgnoreCase))
         {
            name = name.Substring(3);
         }

         return name;
      }

      private static void DiscoverProperties<TInterface>(Dictionary<string, PropertyOptions> result)
      {
         IEnumerable<PropertyInfo> properties = GetHierarchyPublicProperties(typeof(TInterface));

         foreach (PropertyInfo pi in properties)
         {
            var options = new PropertyOptions(pi.Name, pi.PropertyType, null, null);

            AddAttributes(options, pi);

            result[pi.Name] = options;
         }

      }

      private static void DiscoverMethods<TInterface>(Dictionary<string, PropertyOptions> result)
      {
         TypeInfo ti = typeof(TInterface).GetTypeInfo();

         IEnumerable<MethodInfo> methods = ti.DeclaredMethods.Where(m => !m.IsSpecialName);

         foreach(MethodInfo method in methods)
         {
            ParameterInfo[] parameters = method.GetParameters();

            if(parameters == null || parameters.Length == 0)
            {
               throw new InvalidOperationException($"method {method.Name} must have at least one parameter");
            }

            bool isGetter = method.ReturnType != typeof(void);

            //for a getter the type is return type, for a setter it's the last parameter in method signature
            Type returnType = isGetter ? method.ReturnType : parameters[parameters.Length - 1].ParameterType;

            string name = GetMethodName(method);

            var options = new PropertyOptions(name,
               returnType,
               null,
               isGetter);
            options.StoreName = GetMethodStoreName(method);

            AddAttributes(options, method);

            result[name] = options;
         }
      }

      private static void AddAttributes(PropertyOptions po, PropertyInfo pi)
      {
         AddAttributes(po, pi.GetCustomAttribute<OptionAttribute>());
      }

      private static void AddAttributes(PropertyOptions po, MethodInfo mi)
      {
         AddAttributes(po, mi.GetCustomAttribute<OptionAttribute>());
      }

      private static void AddAttributes(PropertyOptions po, OptionAttribute optionAttribute)
      {
         object defaultValue = null;

         if (optionAttribute != null)
         {
            if (optionAttribute.Alias != null) po.StoreName = optionAttribute.Alias;

            //validate that types for default value match
            Type dvt = optionAttribute.DefaultValue?.GetType();
            if (optionAttribute.DefaultValue != null)
            {
               if (dvt != po.Type && dvt != typeof(string))
               {
                  throw new InvalidCastException($"Default value for option {po.Name} is of type {dvt.FullName} whereas the property has type {po.Name}. To fix this, either set default value to type {po.Type.FullName} or a string parseable to the target type.");
               }

               if (po.Type != typeof(string) && dvt == typeof(string))
               {
                  ValueHandler.Default.TryParse(po.Type, (string)optionAttribute.DefaultValue, out defaultValue);
               }
            }

            if (defaultValue == null)
            {
               defaultValue = optionAttribute.DefaultValue;
            }
         }

         if (defaultValue == null) defaultValue = GetDefaultValue(po.Type);

         po.DefaultValue = defaultValue;
      }

      private static object GetDefaultValue(Type t)
      {
         if (t.GetTypeInfo().IsValueType) return Activator.CreateInstance(t);

         return null;
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
