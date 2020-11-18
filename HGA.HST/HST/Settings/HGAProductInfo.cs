using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class HGAProductInfo
    {
        public HGAProductInfo()
        {
        }

        public void Load(SettingsXml xml, string section)
        {
            xml.OpenSection(section);
            //CurrentInstalledTestTailType = (HGAProductTailType)Enum.Parse(typeof(HGAProductTailType),
            //xml.Read("TailType", HGAProductTailType.Unknown.ToString()));
            xml.CloseSection();
        }

        public void Save(SettingsXml xml, string section)
        {
            xml.OpenSection(section);
            //xml.Write("TailType", CurrentInstalledTestTailType.ToString());
            xml.CloseSection();
            xml.Save();
        }

    }
}
