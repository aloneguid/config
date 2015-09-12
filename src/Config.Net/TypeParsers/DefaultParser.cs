using System;

namespace Config.Net.TypeParsers
{
   class DefaultParser
   {
      public bool TryParse(string value, Type t, out object result)
      {
         if(IsEnum(t))
         {
            try
            {
               result = Enum.Parse(t, value, true);
               return true;
            }
            catch(ArgumentException)
            {

            }
            catch(OverflowException)
            {

            }

            result = null;
            return false;
         }

         throw new NotSupportedException();
      }

      public bool IsSupported(Type t)
      {
         return IsEnum(t);
      }

      public string ToRawString(object value)
      {
         if(value == null) return null;

         Type t = value.GetType();

         if(IsEnum(t))
         {
            return value.ToString();
         }

         throw new NotSupportedException();
      }

      static bool IsEnum(Type t)
      {
         if (t == null) return false;

         try
         {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Enum.GetUnderlyingType(t);
            return true;
         }
         catch (ArgumentException)
         {
            return false;
         }
      }

   }
}
