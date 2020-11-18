using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming
{

    public class CCCFinalRunResult : EventArgs
    {
        public CCCFinalRunResult(CCCRunResult partRunData, CCCDataLogger runResult, CCCOutput.Trigger_Type type, TIC_BIN_DATA uticData)
        {
            PartUnitData = partRunData;
            RunResult = runResult;
            TriggerType = type;
            UticData = uticData;
        }

        public CCCOutput.Trigger_Type TriggerType { get; set; }

        public CCCRunResult PartUnitData { get; set; }
        public CCCDataLogger RunResult { get; set; }
        public TIC_BIN_DATA UticData { get; set; }
    }

    public class CCCRunResult
    {
        public CCCRunResult()
        {

        }

        public double StdLcl { get; set; }

        public double AdaptiveNumber { get; set; }
        public int AdaptiveDefectCounter { get; set; }

        public int MCDownTriggering { get; set; }

        public int TICTriggeringCounter { get; set; }
        public int CrdlTriggeringCounter { get; set; }
        public int HighTICPercentTriggeringCounter { get; set; }

        public int HighCrdlPercentTriggeringCounter { get; set; }

        public double DefactCounter { get; set; }

        public int InternalDefectCounter { get; set; }
        public int GoodCounter { get; set; }

        public double YieldTest { get; set; }

        public string CurrentTriggerMc { get; set; }
        public DateTime CurrentUTICTime { get; set; }
    }

    public class CCCAlertActiveStatus
    {
        public CCCAlertActiveStatus()
        {
            IsAlertActivated = false;
            LastAlertActiveTime = DateTime.Now;
            LastAlertClearedTime = DateTime.Now;
        }

        public bool IsAlertActivated { get; set; }

        public DateTime LastAlertActiveTime { get; set; }

        public DateTime LastAlertClearedTime { get; set; }

        public void ChangeActiveStatus(bool isActive)
        {
            IsAlertActivated = isActive;
            LastAlertActiveTime = DateTime.Now;
        }

        public void ClearActiveStatus()
        {
            LastAlertClearedTime = DateTime.Now;
        }
    }

    public class CCCYieldCalculator
    {
        int _goodpart = 0;
        public CCCYieldCalculator()
        {
            GoodPartCounter = 0;
            FailPartCounter = 0;
        }

        public int GoodPartCounter { get { return _goodpart; } set { _goodpart = value; } }
        public int FailPartCounter { get; set; }

    }

    public class CCCYieldCounterCalculator
    {
        public CCCYieldCounterCalculator()
        {
            PartCounter = new Dictionary<int, bool>();
        }
        public Dictionary<int, bool> PartCounter { get; set; }
    }
}
