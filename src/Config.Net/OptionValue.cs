using System;

namespace Config.Net
{
   /// <summary>
   /// Property definition
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class OptionValue<T>
   {
      private readonly object _sync = new object();
      private T _value;
      private string _rawValue;
      private bool _isDefaultValue;

      /// <summary>
      /// Triggered when value is changed
      /// </summary>
      public event Action<T> ValueChanged;

      /// <summary>
      /// Constructs an instance
      /// </summary>
      /// <param name="value"></param>
      /// <param name="rawValue"></param>
      /// <param name="isDefaultValue"></param>
      public OptionValue(T value, string rawValue, bool isDefaultValue)
      {
         _value = value;
         _rawValue = rawValue;
         _isDefaultValue = isDefaultValue;
      }

      /// <summary>
      /// Strong typed value
      /// </summary>
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

      /// <summary>
      /// Returns true if current value is the same as default value
      /// </summary>
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

      /// <summary>
      /// Converts to string
      /// </summary>
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

      /// <summary>
      /// Implicit conversion to strong typed value
      /// </summary>
      public static implicit operator T(OptionValue<T> property)
      {
         return property.Value;
      }
   }
}
