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

      public string EscapedValue
      {
         get { return Value.Replace("\r", @"\r").Replace("\n", @"\n"); }
      }

      public override string ToString() => Value;
   }
}
