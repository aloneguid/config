using System;

namespace Config.Net
{
   public class Property<T>
   {
      private readonly object _sync = new object();
      private T _value;
      private string _rawValue;
      private bool _isDefaultValue;

      public event Action<T> ValueChanged;

      public Property(T value, string rawValue, bool isDefaultValue)
      {
         if(!Cfg.Configuration.HasParser(typeof(T))) throw new TypeLoadException($"type {typeof(T).FullName} not supported");

         _value = value;
         _rawValue = rawValue;
         _isDefaultValue = isDefaultValue;
      }

      public T Value
      {
         get
         {
            lock (_sync)
            {
               return _value;
            }
         }
      }

      public bool IsDefaultValue
      {
         get
         {
            lock (_sync)
            {
               return _isDefaultValue;
            }
         }
      }

      public override string ToString()
      {
         return _rawValue;
      }

      internal void ChangeValue(T newValue, string rawValue, bool isDefault)
      {
         lock (_sync)
         {
            _value = newValue;
            _isDefaultValue = isDefault;
            _rawValue = rawValue;
         }

         ValueChanged?.Invoke(Value);
      }

      public static implicit operator T(Property<T> property)
      {
         return property.Value;
      }
   }
}
