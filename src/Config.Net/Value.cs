using System;

namespace Config.Net
{
   class Value
   {
      public object RawValue;

      public DateTime Updated;

      public bool IsExpired(TimeSpan ttl)
      {
         if (ttl == TimeSpan.Zero) return true;

         return (DateTime.UtcNow - Updated) > ttl;
      }

      public void Update<T>(Type valueType, object value, bool isNullable)
      {

      }

      public void Update<T>(Type valueType, object value, bool isNullable)
      {
         if(isNullable)
         {
            RawValue = new Nullable<T>((T)value);
         }
         else
         {
            RawValue = value;
         }

         Updated = DateTime.UtcNow;
      }
   }
}
