using System;
using System.Collections.Generic;
using System.Text;

namespace Config.Net
{
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
   public class OptionAttribute : Attribute
   {
      /// <summary>
      /// Alias is used to override option name if it's stored by a different name in external stores
      /// </summary>
      public string Alias { get; set; }

      /// <summary>
      /// Set to override the default value if option is not found in any stores
      /// </summary>
      public object DefaultValue { get; set; }

   }
}
