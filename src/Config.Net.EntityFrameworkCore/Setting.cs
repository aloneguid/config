using System.ComponentModel.DataAnnotations;

namespace Config.Net.EntityFrameworkCore
{
   public class Setting
   {
      [Key]
      public long? SettingId { get; set; }

      [Required]
      public string Key { get; set; }

      [Required]
      public string Value { get; set; }
   }
}