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
    [TypeConverter(typeof(NamedConverter<TestProbeType>))]
    public class TestProbeType : INamed
    {
        [Category("Test Probe Types")]
        [DisplayName("Test Probe Name")]
        [Description("Test Probe Name.")]
        public string Name
        {
            get;
            set;
        }

        public TestProbeType()
            : this(CommonFunctions.UNKNOWN)
        {
        }

        public TestProbeType(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
