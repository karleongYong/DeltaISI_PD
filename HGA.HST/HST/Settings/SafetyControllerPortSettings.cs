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
using Seagate.AAS.HGA.HST.Utils;
using System.IO.Ports;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class SafetyControllerPortSettings
    {                    
        [Category("Safety Controller")]
        [DisplayName("COM Port")] 
        [TypeConverter(typeof(COMPortList))]
        public COMPortList COMPort
        {
            get;
            set;
        }

        [Category("Safety Controller")]
        [DisplayName("Baud Rate")]         
        public int BaudRate
        {
            get;
            set;
        }

        [Category("Safety Controller")]
        [DisplayName("Parity")] 
        [TypeConverter(typeof(Parity))]
        public Parity Parity
        {
            get;
            set;
        }

        [Category("Safety Controller")]
        [DisplayName("Data Bits")]        
        public int DataBits
        {
            get;
            set;
        }

        [Category("Safety Controller")]
        [DisplayName("Stop Bits")]
        [TypeConverter(typeof(StopBits))]
        public StopBits StopBits
        {
            get;
            set;
        }       
    }  
}
