namespace Config.Net.Stores.Formats
{
   class IniSection
   {
      /// <summary>
      /// Section name
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="name">Pass null to work with global section</param>
      public IniSection(string name)
      {
         Name = name;
      }
   }
}
