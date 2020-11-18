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
    public class RFIDPortSettings
    {                    
        [Category("RFID Serial Port Settings")]
        [DisplayName("COM Port")] 
        [TypeConverter(typeof(COMPortList))]
        public COMPortList RFIDCOMPort
        {
            get;
            set;
        }

        [Category("RFID Serial Port Settings")]
        [DisplayName("Baud Rate")]         
        public int RFIDBaudRate
        {
            get;
            set;
        }

        [Category("RFID Serial Port Settings")]
        [DisplayName("Parity")] 
        [TypeConverter(typeof(Parity))]
        public Parity RFIDParity
        {
            get;
            set;
        }

        [Category("RFID Serial Port Settings")]
        [DisplayName("Data Bits")]        
        public int RFIDDataBits
        {
            get;
            set;
        }

        [Category("RFID Serial Port Settings")]
        [DisplayName("Stop Bits")]
        [TypeConverter(typeof(StopBits))]
        public StopBits RFIDStopBits
        {
            get;
            set;
        }


        [Category("RFID Settings")]
        [DisplayName("RFID Write Counter Limit")]    
        public int RfidWriteCounterLimit
        {
            get;
            set;
        }        
    }  
}
