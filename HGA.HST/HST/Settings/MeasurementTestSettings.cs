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
    public class MeasurementTestSettings
    {
        public const string MachineConfigFileName = "McConfig.CON";
        public const string SpacialUserFileName = "Access.Log";

        [Category("Test Electronics Serial Port")]
        [DisplayName("COM Port")] 
        [TypeConverter(typeof(COMPortList))]
        public COMPortList COMPort
        {
            get;
            set;
        }

        [Category("Test Electronics Serial Port")]
        [DisplayName("Baud Rate")]         
        public int BaudRate
        {
            get;
            set;
        }

        [Category("Test Electronics Serial Port")]
        [DisplayName("Parity")] 
        [TypeConverter(typeof(Parity))]
        public Parity Parity
        {
            get;
            set;
        }

        [Category("Test Electronics Serial Port")]
        [DisplayName("Data Bits")]        
        public int DataBits
        {
            get;
            set;
        }

        [Category("Test Electronics Serial Port")]
        [DisplayName("Stop Bits")]
        [TypeConverter(typeof(StopBits))]
        public StopBits StopBits
        {
            get;
            set;
        }

        [DisplayName("Test Probe Types")]
        [Category("Test Probe Types")]
        [Editor(typeof(NamedCollectionEditor<TestProbeType>), typeof(UITypeEditor))]
        public NamedCollection<TestProbeType> TestProbeTypes
        {
            get;
            set;
        }

        [Category("Test Probe Types")]
        [DisplayName("Current Installed Test Probe Type")]
        [Editor(typeof(TestProbeTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CurrentInstalledTestProbeType
        {
            get;
            set;
        }

        public MeasurementTestSettings()
        {            
            TestProbeTypes = new NamedCollection<TestProbeType>();
        }
    }  
}
