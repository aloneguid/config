namespace Config.Net.Stores.Formats.Ini
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
