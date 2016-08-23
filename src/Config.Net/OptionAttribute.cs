using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
   }
}
