using Config.Net.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Config.Net.Tests.Stores.EntityFrameworkCore
{
   public class TestContext : DbContext
   {
      private readonly string _file;

      public TestContext(string file)
      {
         _file = file;
      }

      public DbSet<Setting> Settings { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         optionsBuilder.UseSqlite("Data Source=" + _file);
      }
   }
}
