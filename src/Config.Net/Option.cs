using System;

namespace Config.Net
{
   /// <summary>
   /// Describes a configuration option
   /// </summary>
   public abstract class Option
   {
      /// <summary>
      /// Gets configuration option name
      /// </summary>
      public string Name { get; internal set; }

      /// <summary>
      /// Gets type of configuration value
      /// </summary>
      public Type ValueType { get; protected set; }

      /// <summary>
      /// Gets default value used when nothing can be fetches from any configuration stores or they are not configured
      /// </summary>
      public object DefaultValue { get; private set; }

      internal SettingsContainer _parent;

      /// <summary>
      /// Returns option name
      /// </summary>
      public override string ToString()
      {
         return Name;
      }
   }


   /// <summary>
   /// Describes a configuration option
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public sealed class Option<T> : Option
   {
      /// <summary>
      /// Creates a new instance of configuration option default value only
      /// </summary>
      /// <param name="defaultValue"></param>
      public Option(T defaultValue) : this(null, defaultValue)
      {

      }

      /// <summary>
      /// Creates a new instance of configuraton option by name and default value
      /// </summary>
      public Option(string name, T defaultValue)
      {
         //if(name == null) throw new ArgumentNullException(nameof(name));
         //if(!GlobalConfiguration.Instance.CanParse(typeof(T))) throw new TypeLoadException($"type {typeof(T).FullName} not supported");

         Name = name;
         DefaultValue = defaultValue;
         ValueType = typeof(T);
      }

      /// <summary>
      /// Creates a new instance of configuration option by referencing an existing option definition
      /// </summary>
      /// <param name="referenceSetting"></param>
      public Option(Option<T> referenceSetting) : this(referenceSetting.Name, referenceSetting.DefaultValue)
      {

      }

      /// <summary>
      /// Gets default value used when nothing can be fetches from any configuration stores or they are not configured
      /// </summary>
      public new T DefaultValue { get; private set; }

      /// <summary>
      /// Cast operator which reads the option
      /// </summary>
      /// <param name="option"></param>
      public static implicit operator T(Option<T> option)
      {
         object newValue = option._parent.Read(option.ValueType, option.Name);

         return Cfg.Read(option);
      }
   }
}
