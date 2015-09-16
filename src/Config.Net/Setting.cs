using System;

namespace Config.Net
{
   public sealed class Setting<T>
   {
      public Setting(string name, T defaultValue)
      {
         if (name == null) throw new ArgumentNullException(nameof(name));

         Name = name;
         DefaultValue = defaultValue;
      }

      public string Name { get; }

      public Type ValueType => typeof(T);

      public T DefaultValue { get; private set; }

      /// <summary>
      /// Aliases for this property. Useful when renaming property to something else to support
      /// backward compatibility.
      /// todo: implement
      /// </summary>
      //public string[] AlsoKnownAs { get; set; }

      public override string ToString()
      {
         return Name;
      }
   }
}
