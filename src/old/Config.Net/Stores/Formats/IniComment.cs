namespace Config.Net.Stores.Formats
{
   class IniComment : IniEntity
   {
      public const string CommentSeparator = ";";

      public IniComment(string value)
      {
         Value = value;
      }

      public string Value { get; set; }
   }
}
