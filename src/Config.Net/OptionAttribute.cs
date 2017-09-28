using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net
{
   [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
   public class OptionAttribute : Attribute
   {
      public string Name { get; set; }

      public object DefaultValue { get; set; }
   }
}
