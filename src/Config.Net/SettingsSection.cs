using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Net
{
   public abstract class SettingsSection : SettingsContainer
   {
      protected SettingsSection(string sectionName) : base(sectionName)
      {

      }

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
      }
   }
}
