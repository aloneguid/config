namespace Config.Net.TypeParsers
{
   class StringParser : ITypeParser
   {
      public bool TryParse(string value, out string result)
      {
         result = value;
         return value != null;
      }

      public string ToRawString(string value)
      {
         return value;
      }
   }
}
