using Seagate.AAS.HGA.HST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using XyratexOSC.UI;
using XyratexOSC.XMath;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class ToleranceSettings
    {
        //temporary didn't categories into module like input module, precisor module, can do so if required in future.

        [Category("Tolerance")]
        [DisplayName("PrecisorNestXTolerance")]
        [Description("Offset Tolerance (in micron) for Precisor Nest X-axis")]
        //[EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public double PrecisorNestXTolerance
        {
            get;
            set;
        }

        [Category("Tolerance")]
        [DisplayName("PrecisorNestYTolerance")]
        [Description("Offset Tolerance (in micron) for Precisor Nest Y-axis")]
        //[EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public double PrecisorNestYTolerance
        {
            get;
            set;
        }

        [Category("Tolerance")]
        [DisplayName("PrecisorNestThetaTolerance")]
        [Description("Offset Tolerance (in micron) for Precisor Nest Theta-axis")]
        //[EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public double PrecisorNestThetaTolerance
        {
            get;
            set;
        }

        public ToleranceSettings()
        {
            PrecisorNestXTolerance = 0;
            PrecisorNestYTolerance = 0;
            PrecisorNestThetaTolerance = 0;
        }
    }
}
