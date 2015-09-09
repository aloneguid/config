namespace Aloneguid.Config.TypeParsers
{
   class StringParser : ITypeParser<string>
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
