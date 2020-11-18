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
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.Data;

namespace Seagate.AAS.HGA.HST.Settings
{
    [Serializable]
    [TypeConverter(typeof(NamedConverter<CarrierSettings>))]
    public class CarrierSettings : BaseCarrier, INamed
    {
        private string carrierName = "Unknown";

        [Browsable(false)]
        [Category("Carrier")]               
        [DisplayName("Carrier Name")]
        [Description("Carrier name.")]
        public string Name
        {
            //Lai: Ensure the Carrier Name always set to Carrier ID
            get { return this.CarrierID; }
            set { carrierName = this.CarrierID; } 
 
        }

        [Category("RFID")]
        [DisplayName("RFID File Name")]
        [Description("Full Path File Name of the RFID.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string RFIDFileName
        {
            get;
            set;
        }

        [Category("Work Order")]
        [DisplayName("Work Order File Name")]
        [Description("Full Path File Name of the Work Order.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        [Browsable(false)]
        public string WorkOrderFileName
        {
            get;
            set; 
        }        
    }
}
