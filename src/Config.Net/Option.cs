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

      internal Type NonNullableType;

      internal bool IsNullable;

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
      /// Creates a new instance of configuration where all parameters are default
      /// </summary>
      public Option() : this(null, default(T))
      {

      }

      /// <summary>
      /// Creates a new instance of configuration option default value only
      /// </summary>
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
         return option._parent == null ? default(T) : option._parent.Read(option);
      }

      /// <summary>
      /// Read the option value
      /// </summary>
      public T Value
      {
         get
         {
            return _parent == null ? default(T) : _parent.Read(this);
         }
      }

      /// <summary>
      /// Writes a new value to this option. The value is written to the first available store which is writeable.
      /// If none of the stores are writeable no errors are thrown.
      /// </summary>
      /// <param name="value"></param>
      public void Write(T value)
      {
         _parent.Write(this, value);
      }

      /// <summary>
      /// This method reads the option value and calls .ToString() on it. This is due to the fact that it's easy to forget to
      /// cast to a required type in string format operations etc. as it used to return just an option name.
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return _parent?.Read(this)?.ToString();
      }
   }
}
