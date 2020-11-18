using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming
{
    public class CCCParameter
    {
        public CCCParameter()
        {
            Enabled = false;
            EnableAlertMsg = false;
            YeildTarget = 0.0;
            Alpha = 0.0;
            YeildLimit = 0;
            DefectCounterLimit = 0;
            TestRunGroup = 0;
            TimeToIgnoreAfterRepair = 0;

        }
        public string uTICMachineName1 { get; set; }
        public string uTICMachineName2 { get; set; }
        public bool Enabled { get; set; }
        public double YeildTarget { get; set; }

        public double Alpha { get; set; }

        public double YeildLimit { get; set; }
        public int DefectCounterLimit { get; set; }

        public int TestRunGroup { get; set; }

        public bool EnableAlertMsg { get; set; }

        public int TimeToIgnoreAfterRepair { get; set; }

        public int PartCounterYieldPerShortPeriod { get; set; }
        public int PartCounterYieldPerLongPeriod { get; set; }
        public double DelayForReleaseCarrier { get; set; }
        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("Enabled", Enabled);
            xml.Write("EnableAlertMag", EnableAlertMsg);
            xml.Write("YeildTarget", YeildTarget);
            xml.Write("YeildLimit", YeildLimit);
            xml.Write("Alpha", Alpha);
            xml.Write("DefectCounterLimit", DefectCounterLimit);
            xml.Write("TestRunGroup", TestRunGroup);
            xml.Write("TimeToIgnoreAfterRepair", TimeToIgnoreAfterRepair);
            xml.Write("PartCounterYieldPerShortPeriod", PartCounterYieldPerShortPeriod);
            xml.Write("PartCounterYieldPerLongPeriod", PartCounterYieldPerLongPeriod);
            xml.Write("DelayForReleaseCarrier", DelayForReleaseCarrier);
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            Enabled = xml.Read("Enabled", Enabled);
            EnableAlertMsg = xml.Read("EnableAlertMag", EnableAlertMsg);
            YeildTarget = xml.Read("YeildTarget", 0.00);
            YeildLimit = xml.Read("YeildLimit", 0.00);
            Alpha = xml.Read("Alpha", 0.00);
            DefectCounterLimit = xml.Read("DefectCounterLimit", 0);
            TestRunGroup = xml.Read("TestRunGroup", 0);
            TimeToIgnoreAfterRepair = xml.Read("TimeToIgnoreAfterRepair", 0);
            PartCounterYieldPerShortPeriod = xml.Read("PartCounterYieldPerShortPeriod", 100);
            PartCounterYieldPerLongPeriod = xml.Read("PartCounterYieldPerLongPeriod", 100);
            DelayForReleaseCarrier = xml.Read("DelayForReleaseCarrier", 0.0);
            xml.CloseSection();
        }
    }
}
