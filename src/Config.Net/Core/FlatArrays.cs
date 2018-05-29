using System;
using System.Collections.Generic;
using System.Text;
using Config.Net.Core;
using Config.Net.TypeParsers;

namespace Config.Net.Core
{
   /// <summary>
   /// Helper class to implement flat arrays
   /// </summary>
   public static class FlatArrays
   {
      public static bool IsArrayLength(string key, Func<string, string> getValue, out int length)
      {
         if (!OptionPath.TryStripLength(key, out key))
         {
            length = 0;
            return false;
         }

         string value = getValue(key);
         if (value == null)
         {
            length = 0;
            return false;
         }

         if (!StringArrayParser.TryParse(value, out string[] ar))
         {
            length = 0;
            return false;
         }

         length = ar.Length;
         return true;
      }

      public static bool IsArrayElement(string key, Func<string, string> getValue, out string value)
      {
         if(!OptionPath.TryStripIndex(key, out key, out int index))
         {
            value = null;
            return false;
         }

         string arrayString = getValue(key);
         if (!StringArrayParser.TryParse(arrayString, out string[] array) || index >= array.Length)
         {
            value = null;
            return false;
         }

         value = array[index];
         return true;
      }
   }
}
