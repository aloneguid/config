using System;

namespace Config.Net
{
   public sealed class Setting<T>
   {
      public Setting(string name, T defaultValue, string docCategory, string docTitle, string docDescription)
      {
         if (name == null) throw new ArgumentNullException(nameof(name));

         Name = name;
         DefaultValue = defaultValue;
         DocCategory = docCategory;
         DocTitle = docTitle;
         DocDescription = docDescription;
      }

      public Setting(string name, T defaultValue)
         : this(name, defaultValue, null, null, null)
      {

      }

      public string Name { get; private set; }

      public Type ValueType { get { return typeof(T); } }

      public T DefaultValue { get; private set; }

      /// <summary>
      /// Aliases for this property. Useful when renaming property to something else to support
      /// backward compatibility.
      /// todo: implement
      /// </summary>
      //public string[] AlsoKnownAs { get; set; }

      /// <summary>
      /// Documentation category
      /// </summary>
      public string DocCategory { get; set; }

      /// <summary>
      /// Documentation title
      /// </summary>
      public string DocTitle { get; set; }

      /// <summary>
      /// Documentation description
      /// </summary>
      public string DocDescription { get; set; }

      public override string ToString()
      {
         return Name;
      }
   }
}
