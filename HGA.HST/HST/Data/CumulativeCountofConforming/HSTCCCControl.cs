using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.Utils;

namespace Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming
{
    public class HSTCCCControl
    {
        private CCCOutput _cccControlTicMc1;
        private CCCOutput _cccControlTicMc2;
        private CCCOutput _cccControlAllMc;

        public HSTCCCControl(CCCParameter setting)
        {
            _cccControlTicMc1 = new CCCOutput(setting);
            _cccControlTicMc2 = new CCCOutput(setting);
            _cccControlAllMc = new CCCOutput(setting);
        }

        public HSTCCCControl(CCCParameter setting, HSTCCCCounter counterInfo, bool TurnOnTic, bool TurnOnCrdl)
        {
            _cccControlTicMc1 = new CCCOutput(setting, counterInfo.CCCCounterTicMc1, TurnOnTic, TurnOnCrdl);
            _cccControlTicMc2 = new CCCOutput(setting, counterInfo.CCCCounterTicMc2, TurnOnTic, TurnOnCrdl);
            _cccControlAllMc = new CCCOutput(setting, counterInfo.CCCCounterAllMc, TurnOnTic, TurnOnCrdl);


        }

        public CCCOutput CCCControlTicMc1 { get { return _cccControlTicMc1; } set { _cccControlTicMc1 = value; } }
        public CCCOutput CCCControlTicMc2 { get { return _cccControlTicMc2; } set { _cccControlTicMc2 = value; } }
        public CCCOutput CCCControlAllMc { get { return _cccControlAllMc; } set { _cccControlAllMc = value; } }

    }

    public class HSTCCCCounter
    {
        private CCCCounterInfo _cccCounterTicMc1;
        private CCCCounterInfo _cccCounterTicMc2;
        private CCCCounterInfo _cccCounterAllMc;

        public HSTCCCCounter()
        {
            _cccCounterAllMc = new CCCCounterInfo("CounterForAllMc");
            _cccCounterTicMc1 = new CCCCounterInfo("CounterForTicMc1");
            _cccCounterTicMc2 = new CCCCounterInfo("CounterForTicMc2");
        }

        public CCCCounterInfo CCCCounterAllMc { get { return _cccCounterAllMc; } set { _cccCounterAllMc = value; } }
        public CCCCounterInfo CCCCounterTicMc1 { get { return _cccCounterTicMc1; } set { _cccCounterTicMc1 = value; } }
        public CCCCounterInfo CCCCounterTicMc2 { get { return _cccCounterTicMc2; } set { _cccCounterTicMc2 = value; } }

        public void Save(string section, SettingsXml xml)
        {
            _cccCounterAllMc.Save(section, xml);
            _cccCounterTicMc1.Save(section, xml);
            _cccCounterTicMc2.Save(section, xml);
        }

        public void Load(string section, SettingsXml xml)
        {
            _cccCounterAllMc.Load(section, xml);
            _cccCounterTicMc1.Load(section, xml);
            _cccCounterTicMc2.Load(section, xml);
        }
    }

    public class CCCCounterInfo
    {
        private CCCCounter _cccCounterForHst;
        private CCCCounter _cccCounterForTic;
        private string _counterName;
        private const string HSTCounterName = "CounterForHst";

        public CCCCounterInfo(string counterName)
        {
            _counterName = counterName;
            _cccCounterForHst = new CCCCounter(HSTCounterName);
        }

        public CCCCounter CCCCounterForHst { get { return _cccCounterForHst; } set { _cccCounterForHst = value; } }

        public void Reset()
        {
            //25-mar-2020 have update Default at CCCinfo to reset TIC_defect and tot mac defect by 6am everyday
            _cccCounterForHst.Default();
        }

        public void Save(string section, SettingsXml xml)
        {
            _cccCounterForHst.Save(section + "/" + _counterName + "/" + HSTCounterName + "/", xml);
        }

        public void Load(string section, SettingsXml xml)
        {
            _cccCounterForHst.Load(section + "/" + _counterName + "/" + HSTCounterName + "/", xml);
        }
    }

}
