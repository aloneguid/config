using System;

namespace Config.Net
{
   /// <summary>
   /// Describes a configuration setting
   /// </summary>
   public abstract class Setting
   {
      /// <summary>
      /// Gets configuration setting name
      /// </summary>
      public string Name { get; protected set; }

      /// <summary>
      /// Gets type of configuration value
      /// </summary>
      public Type ValueType { get; protected set; }

      /// <summary>
      /// Gets default value used when nothing can be fetches from any configuration stores or they are not configured
      /// </summary>
      public object DefaultValue { get; private set; }

      /// <summary>
      /// Aliases for this property. Useful when renaming property to something else to support
      /// backward compatibility.
      /// </summary>
      public string[] AlsoKnownAs { get; set; }

      /// <summary>
      /// Returns setting name
      /// </summary>
      public override string ToString()
      {
         return Name;
      }
   }


   /// <summary>
   /// Describes a configuration setting
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public sealed class Setting<T> : Setting
   {
      /// <summary>
      /// Creates a new instance of configuraton setting by name and default value
      /// </summary>
      public Setting(string name, T defaultValue)
      {
         if(name == null) throw new ArgumentNullException(nameof(name));
         //if(!GlobalConfiguration.Instance.CanParse(typeof(T))) throw new TypeLoadException($"type {typeof(T).FullName} not supported");

         Name = name;
         DefaultValue = defaultValue;
         ValueType = typeof(T);
      }

      /// <summary>
      /// Creates a new instance of configuration setting by referencing an existing setting definition
      /// </summary>
      /// <param name="referenceSetting"></param>
      public Setting(Setting<T> referenceSetting) : this(referenceSetting.Name, referenceSetting.DefaultValue)
      {

      }

      /// <summary>
      /// Gets default value used when nothing can be fetches from any configuration stores or they are not configured
      /// </summary>
      public new T DefaultValue { get; private set; }

      /// <summary>
      /// Cast operator which reads the setting
      /// </summary>
      /// <param name="property"></param>
      public static implicit operator T(Setting<T> property)
      {
         return Cfg.Read(property);
      }
   }
}
