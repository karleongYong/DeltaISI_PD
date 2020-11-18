using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class ProcessRunEventArgs : EventArgs
    {
        public string strArg { get; internal set; }
        public string sLotID { get; internal set; }
        public string sLotRecipe { get; internal set; }
        public string sCarrierType { get; internal set; }
        public string sSliderType { get; internal set; }
        public string sOutputDirectory { get; internal set; }
        public string sFailIf { get; internal set; }
        public string sSelectedSliders { get; internal set; }
        public string sRows { get; internal set; }
        public string sColumns { get; internal set; }
        public string sFlowSequenceFile { get; internal set; }
    }
}
