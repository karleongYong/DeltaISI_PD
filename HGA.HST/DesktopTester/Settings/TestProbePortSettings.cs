using DesktopTester.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using XyratexOSC.UI;
using XyratexOSC.XMath;
using DesktopTester.Utils;
using System.IO.Ports;

namespace DesktopTester.Settings
{
    public class TestProbePortSettings
    {                    
        [Category("Test Electronics")]
        [DisplayName("COM Port")] 
        [TypeConverter(typeof(COMPortList))]
        public COMPortList COMPort
        {
            get;
            set;
        }

        [Category("Test Electronics")]
        [DisplayName("Baud Rate")]         
        public int BaudRate
        {
            get;
            set;
        }

        [Category("Test Electronics")]
        [DisplayName("Parity")] 
        [TypeConverter(typeof(Parity))]
        public Parity Parity
        {
            get;
            set;
        }

        [Category("Test Electronics")]
        [DisplayName("Data Bits")]        
        public int DataBits
        {
            get;
            set;
        }

        [Category("Test Electronics")]
        [DisplayName("Stop Bits")]
        [TypeConverter(typeof(StopBits))]
        public StopBits StopBits
        {
            get;
            set;
        }       
    }  
}
