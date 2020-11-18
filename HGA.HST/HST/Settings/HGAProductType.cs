using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Settings
{
    [Serializable]
    [TypeConverter(typeof(NamedConverter<HGAProductType>))]
    public class HGAProductType : INamed
    {
        [Category("HGA Types")]
        [DisplayName("Conversion Board ID")]
        [Description("Conversion Board ID used by the measurement test electronics.")]
        public int ConversionBoardID
        {
            get;
            set;
        }

        [Category("HGA Types")]        
        [DisplayName("Product Name")]
        [Description("HGA product name.")]
        public string Name
        {
            get;
            set;
        }

        [Category("HGA Types")]
        [DisplayName("Product Tail Type")]
        [Description("HGA product Tail Type.")]
        public HGAProductTailType HGATailType
        {
            get;
            set;
        }

        [Category("Test Probe Types")]
        [DisplayName("Test Probe Type")]
        [Description("Test Probe Type required to probe the HGAs.")]
        [Editor(typeof(TestProbeTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TestProbeType
        {
            get;
            set;
        }

        [Category("Recipe Global Path")]
        [DisplayName("Recipe Global Path")]
        [Description("Please spacific global path that contain product recipe")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string RecipeGlobalPath
        {
            get;
            set;
        }

        public HGAProductType()
            : this(0, CommonFunctions.UNKNOWN, HGAProductTailType.LongTail.ToString(), CommonFunctions.UNKNOWN)
        {            
        }

        public HGAProductType(string name)
        {
            Name = name;
            ConversionBoardID = 0;
            HGATailType = HGAProductTailType.Unknown;
            TestProbeType = CommonFunctions.UNKNOWN;
        }
        public HGAProductType(int conversionBoardID, string name, string tailType, string testProbeType)
        {
            Name = name;
            ConversionBoardID = conversionBoardID;
            HGATailType = (HGAProductTailType)Enum.Parse(typeof(HGAProductTailType), tailType);
            TestProbeType = testProbeType;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
