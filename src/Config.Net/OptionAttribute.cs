using System;

namespace Config.Net
{
   [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
   public class OptionAttribute : Attribute
   {
      public OptionAttribute()
      {

      }

      public string Name { get; set; }

      public object DefaultValue { get; set; }

      internal DateTime ReadOn;

      internal object Value;
   }
}
