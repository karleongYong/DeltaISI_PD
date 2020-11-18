using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class HGAProductSetting
    {
        [DisplayName("HGA Product")]
        [Category("HGA Product")]
        [Editor(typeof(NamedCollectionEditor<HGAProductType>), typeof(UITypeEditor))]
        public NamedCollection<HGAProductType> HGAProduct
        {
            get;
            set;
        }

        public HGAProductSetting()
        {
            HGAProduct = new NamedCollection<HGAProductType>();
        }

        public void Load(SettingsXml xml, string section)
        {
            HGAProduct.Clear();
            int pid = 0;
            bool end = false;

            while (!end)
            {
                pid++;
                var pidname = string.Format("PID{0}", pid.ToString());
                xml.OpenSection(section);
                var name = xml.Read(pidname + "/ProductName", string.Empty);

                if (name != String.Empty)
                {
                    HGAProductType GetPid = new HGAProductType(name);
                    GetPid.ConversionBoardID = xml.Read(pidname + "/ConversionBoardID", 0);
                    GetPid.HGATailType = (HGAProductTailType)Enum.Parse(typeof(HGAProductTailType), 
                        xml.Read(pidname + "/HGAProductTailType", HGAProductTailType.Unknown.ToString()));
                    GetPid.TestProbeType = xml.Read(pidname + "/TestProbeType", String.Empty);
                    GetPid.RecipeGlobalPath = xml.Read(pidname + "/GlobalPath", String.Empty);
                    HGAProduct.Add(GetPid);
                }
                else
                {
                    end = true;
                }
            }

            xml.CloseSection();
        }

        public void Save(SettingsXml xml, string section)
        {
            int pid = 0;
            foreach (HGAProductType item in HGAProduct)
            {
                pid++;
                var pidname = string.Format("PID{0}", pid.ToString());
                xml.OpenSection(section);
                xml.Write(pidname + "/ProductName", item.Name);
                xml.Write(pidname + "/ConversionBoardID", item.ConversionBoardID.ToString());
                xml.Write(pidname + "/HGAProductTailType", item.HGATailType.ToString());
                xml.Write(pidname + "/TestProbeType", item.TestProbeType);
                xml.Write(pidname + "/GlobalPath", item.RecipeGlobalPath);
                xml.CloseSection();
            }
            xml.Save();
        }
    }
}
