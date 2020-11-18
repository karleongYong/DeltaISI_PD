using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.Utils;
using Seagate.AAS.EFI.Log;
using System.IO;
using System.Threading;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Settings;

namespace Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming
{
    public class CCCMcDownTriggerEvent : EventArgs
    {
        public CCCMcDownTriggerEvent(CCCDataLogger data)
        {
            PartUnitData = data;
        }

        public CCCDataLogger PartUnitData { get; set; }
    }

    public class CCCOutput
    {

        public event EventHandler<CCCMcDownTriggerEvent> OnMcDownTrigger;
        public DateTime lastResetDate;
        public enum CCC_MC_Type
        {
            TIC,
            HST
        }

        public enum Trigger_Type
        {
            TIC,
            CRDL
        }


        public enum MC_Trigger_Failure_Type
        {
            PART_FAIL,
            FAIL_ANC,
            FAIL_CCC
        }

        private string _outputName = String.Empty;
        private CCCResult _cccDefect;
        private bool _isCalculateTicDefect = false;
        private bool _isCalculateCrdlDefect = false;
        private string _TicMachineName = "";
        public double current_yield = 0;

        public int ALL_MC_Trigger = 0;
        public int ALL_TIC_TRIGGERING = 0;
        public string TICMachineName { get; set; }
        public CCCOutput(string outputName)
        {
            _outputName = Name;
            _cccDefect = new CCCResult("TIC_Defect");
        }

        public CCCOutput(CCCParameter setting)
        {
            _outputName = Name;
            _cccDefect = new CCCResult("TIC_Defect", setting.TestRunGroup, setting.Alpha, setting.YeildTarget, setting.YeildLimit, setting.DefectCounterLimit);
        }

        public CCCOutput(CCCParameter setting, CCCCounterInfo counterInfo, bool TurnOnTicDefect, bool TurnOnCrdlDefect)
        {
            _outputName = Name;
            _isCalculateTicDefect = TurnOnTicDefect;
            _isCalculateCrdlDefect = TurnOnCrdlDefect;
            _cccDefect = new CCCResult("TIC_Defect", setting.TestRunGroup, setting.Alpha, setting.YeildTarget, setting.YeildLimit, setting.DefectCounterLimit);

            _cccDefect.OutputCounter = counterInfo.CCCCounterForHst;
        }

        public CCCOutput(string outputName, int countPerPeriod, double alpha, double yeildTarget, double yeildLimit, int defectLimit, bool TurnOnTicDefect, bool TurnOnCrdlDefect)
        {
            _outputName = Name;
            _isCalculateTicDefect = TurnOnTicDefect;
            _isCalculateCrdlDefect = TurnOnCrdlDefect;
            _cccDefect = new CCCResult("TIC_Defect", countPerPeriod, alpha, yeildTarget, yeildLimit, defectLimit);
        }

        public string Name { get { return _outputName; } }
        public CCCResult CccResult { get { return _cccDefect; } }

        public void UpdateGoodCounter()
        {
            CccResult.OutputCounter.TicGoodPartCounter++;
            CccResult.OutputCounter.HstGoodPartCounter++;
            CccResult.OutputCounter.LastRunGoodCounter = CccResult.OutputCounter.TicGoodPartCounter;
        }

        public void UpdateDefectCounter(CCC_MC_Type mc_type)
        {
            if (mc_type == CCC_MC_Type.HST)
            {
                CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.PART_FAIL;
                CccResult.OutputCounter.HstFailCounter++;
                CccResult.OutputCounter.AdaptiveDefectCounter++;
                HSTDefectCalculate();
            }
            else if (mc_type == CCC_MC_Type.TIC)
            {
                CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.PART_FAIL;
                CccResult.OutputCounter.TicFailCounter++;
                CccResult.OutputCounter.AdaptiveDefectCounter++;
                TICDefectCalculat();
            }
        }

        public void UpdatePartRunCounter()
        {
            CccResult.OutputCounter.AdaptivePartRunCounter++;
            CccResult.SetAdaptiveNumber();
        }

        public void HSTDefectCalculate()
        {
            CccResult.OutputCounter.HstDefactCounter++;
            if (CccResult.OutputCounter.HstGoodPartCounter < CccResult.StdLcl)
            {
                CccResult.OutputCounter.HstHighPercentTriggeringCounter++;
                CccResult.OutputCounter.MCDownTriggering++;
                CccResult.OutputCounter.McTriggerType = Trigger_Type.CRDL;

                if (_isCalculateCrdlDefect)
                    RaiseMcDownEvent(CccResult.CCCDataLogger);

                CccResult.OutputCounter.AdaptiveDefectCounter = 0;
            }

            UpdateData(CccResult);
            CccResult.OutputCounter.HstGoodPartCounter = 0;
        }

        public void TICDefectCalculat()
        {
            CccResult.SetAdaptiveNumber();
            CccResult.OutputCounter.LastAdaptiveDefectCounter = CccResult.OutputCounter.AdaptiveDefectCounter;
            if (CccResult.OutputCounter.TicGoodPartCounter < CccResult.StdLcl) //if found defect in-case of part counter less than LCL(part counter mininum)
            {
                CccResult.OutputCounter.TicHighPercentTriggeringCounter++;
            }

            if (CccResult.OutputCounter.AdaptiveDefectCounter < CccResult.AdaptiveNumber)
            {
                var goodpart = CccResult.OutputCounter.TicGoodPartCounter;
                var lcl = CccResult.StdLcl;

                if (CccResult.OutputCounter.TicGoodPartCounter <= CccResult.StdLcl)
                {
                    CccResult.OutputCounter.InternalTriggerCounter++;
                    CccResult.OutputCounter.LastDefectCounter = CccResult.OutputCounter.InternalTriggerCounter;

                    if (CccResult.OutputCounter.InternalTriggerCounter >= CccResult.DefectLimit)
                    {
                        CccResult.OutputCounter.TicDefactCounter++;
                        CccResult.OutputCounter.MCDownTriggering++;
                        CccResult.OutputCounter.IsTriggering = true;
                        CccResult.OutputCounter.IsUnderCCCAlert = true;


                        CccResult.OutputCounter.McTriggerType = Trigger_Type.TIC;
                        CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.FAIL_CCC;

                        UpdateData(CccResult);
                        ResetInternalTrigger();

                        if (_isCalculateTicDefect)
                            RaiseMcDownEvent(CccResult.CCCDataLogger);
                    }
                    else
                    {
                        CccResult.OutputCounter.TicGoodPartCounter = 0;
                        CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.PART_FAIL;
                        UpdateData(CccResult);
                    }
                }
                else
                {
                    CccResult.OutputCounter.InternalTriggerCounter = 0;
                    CccResult.OutputCounter.TicGoodPartCounter = 0;
                    CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.PART_FAIL;
                    UpdateData(CccResult);
                }
            }
            else if (CccResult.OutputCounter.AdaptiveDefectCounter >= CccResult.AdaptiveNumber)
            {

                CccResult.OutputCounter.TicDefactCounter++;
                CccResult.OutputCounter.MCDownTriggering++;
                CccResult.OutputCounter.IsTriggering = true;
                CccResult.OutputCounter.IsUnderCCCAlert = true;

                CccResult.OutputCounter.LastDefectCounter = CccResult.OutputCounter.InternalTriggerCounter;
                CccResult.OutputCounter.McTriggerType = Trigger_Type.TIC;
                CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.FAIL_ANC;

                UpdateData(CccResult);
                ResetInternalTrigger();

                if (_isCalculateTicDefect)
                    RaiseMcDownEvent(CccResult.CCCDataLogger);
            }
            else
            {
                if (CccResult.OutputCounter.TicGoodPartCounter > CccResult.StdLcl)
                {
                    CccResult.OutputCounter.InternalTriggerCounter = 0;
                }
                CccResult.OutputCounter.TriggerFailureType = MC_Trigger_Failure_Type.PART_FAIL;
                UpdateData(CccResult);
            }

        }

        private void ResetInternalTrigger()
        {
            CccResult.OutputCounter.AdaptiveDefectCounter = 0;
            CccResult.OutputCounter.InternalTriggerCounter = 0;
            CccResult.OutputCounter.AdaptivePartRunCounter = 0;
            CccResult.OutputCounter.LastRunGoodCounter = 0;
            CccResult.OutputCounter.TicGoodPartCounter = 0;
        }


        public void UpdateData(CCCResult result)
        {


            //Mar-2020 change triggering count
            var TotalMCDownTrigger = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering + HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering;
            var TotalTICTrigger = HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicDefactCounter + HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicDefactCounter;

            string triggerType = string.Empty;
            triggerType = result.OutputCounter.McTriggerType == Trigger_Type.CRDL ? "HIGH_PERCENT_CRDL" : "HIGH_PERCENT_TIC";
            decimal mc1FailPartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
            decimal totalParts = decimal.Parse(result.AdaptivePartCounter.ToString());
            decimal mc2FailPartCounter = decimal.Parse(HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());

            decimal currentYield = 0;
            var totalcount = (totalParts - mc1FailPartCounter) * 100;
            try
            {
                if (totalParts != 0)
                    currentYield = decimal.Divide(totalcount, totalParts);
            }
            catch (Exception)
            {
            }

            result.SetAdaptiveNumber();
            if (result.OutputCounter.LastCCCOutputUTICtime == null)
            {
                result.OutputCounter.LastCCCOutputUTICtime = "";
            }
            CCCDataLogger dataUpdate = new CCCDataLogger
            {
                DateTime = DateTime.Now.ToString(),
                TEST_RUN_COUNT = HSTMachine.Workcell.HSTSettings.CccParameterSetting.TestRunGroup.ToString(),
                YIELD_TARGET = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildTarget.ToString(),
                YIELD_LIMIT = HSTMachine.Workcell.HSTSettings.CccParameterSetting.YeildLimit.ToString(),
                DEFECT_LIMIT = HSTMachine.Workcell.HSTSettings.CccParameterSetting.DefectCounterLimit.ToString(),
                LCL = HSTMachine.Workcell.HSTSettings.CccParameterSetting.Alpha.ToString(),
                PART_COUNT = ((result.OutputCounter.LastRunGoodCounter == 0) ? 0 : result.OutputCounter.LastRunGoodCounter - 1).ToString(),
                PART_LIMIT_YIELD = result.YieldTest.ToString(), // not define yet
                PART_TRIGGER = result.AdaptiveNumber.ToString(), // not define yet
                DEFECT_COUNTER = result.OutputCounter.AdaptiveDefectCounter.ToString(), //not define yet
                CCC_TRIGGERING = result.OutputCounter.InternalTriggerCounter.ToString(), // not define yet
                TIC_TRIGGERING_COUNT = result.OutputCounter.TicDefactCounter.ToString(),
                HIGH_CRDL_PERCENT_TRIGGERING_COUNT = result.OutputCounter.HstHighPercentTriggeringCounter.ToString(),
                CRDL_TRIGGERING_COUNT = result.OutputCounter.HstDefactCounter.ToString(),
                MC_TRIGGERING_COUNT = result.OutputCounter.MCDownTriggering.ToString(),
                MC_TRIGGER_TYPE = result.OutputCounter.McTriggerType.ToString(),
                TRIGERING_TYPE = triggerType,
                FAILURE_TYPE = result.OutputCounter.TriggerFailureType.ToString(),
                ALL_MC_TRIGGER = TotalMCDownTrigger.ToString(),
                PART_RUNNING = result.AdaptivePartCounter.ToString(),
                TotalParts = totalParts.ToString(),
                FailM1PartCounter = mc1FailPartCounter.ToString(),
                FailM2PartCounter = mc2FailPartCounter.ToString(),
                CurrentYield = currentYield.ToString(),
                ALL_TIC_TRIGGERING = TotalTICTrigger.ToString(),
                uTICdate = result.OutputCounter.LastCCCOutputUTICtime.ToString()
            };

            result.CCCDataLogger = dataUpdate;
            CccResult.CCCDataLogger = dataUpdate;
        }

        public void RaiseMcDownEvent(CCCDataLogger data)
        {
            if (OnMcDownTrigger != null)
            {
                OnMcDownTrigger(this, new CCCMcDownTriggerEvent(data));
            }
        }
    }

    public class CCCResult
    {
        public event EventHandler OnSetMaximumReached;

        private CCCCounter _cccCounter;
        private double _adNumber;
        private string _uticMachinename;
        private DateTime _LastUTICDate;
        public CCCResult(string counterName)
        {
            _cccCounter = new CCCCounter(counterName);
        }

        public CCCResult(string counterName, int countPerPeriod, double alpha, double yeildTarget, double yeildLimit, double defectLimit)
        {
            _cccCounter = new CCCCounter(counterName);
            CountPerPeriod = countPerPeriod;
            Alpha = alpha;
            YeildTarget = yeildTarget;
            DefectLimit = defectLimit;
            YeildLimit = yeildLimit;
            _adNumber = 0;
        }

        public int CountPerPeriod { get; set; }
        public double YeildTarget { get; set; }
        public double YeildLimit { get; set; }
        public double Alpha { get; set; }
        public double AdaptivePartCounter { get; set; }
        public double DefectLimit { get; set; }

        public double P_Value { get { return ((100 - YeildTarget) / 100); } }

        public double StdCl { get { return (1 / P_Value); } }

        public double StdUcl { get { return (Math.Log(Alpha / 2)) / (Math.Log(1 - P_Value)); } }

        public double StdLcl { get { return Alpha; } }

        public double YieldTest { get { return ((100 - YeildLimit) / 100) * (CountPerPeriod); } }

        public string uTICMachineName { get { return _uticMachinename; } }

        public DateTime LastUTICDate { get; set; }
        public decimal CurrentRunningYield
        {
            get
            {
                var totalpartCounter = OutputCounter.LastRunGoodCounter + OutputCounter.LastDefectCounter;
                decimal totalyield = 0;
                if (totalpartCounter > 0)
                    totalyield = decimal.Divide((OutputCounter.LastRunGoodCounter * 100), totalpartCounter);
                return totalyield;
            }
        }

        public double AdaptiveNumber
        {
            get { return _adNumber; }
        }

        public void SetAdaptiveNumber()
        {

            double returnVal = 0.0;

            if (OutputCounter.AdaptivePartRunCounter > 0)
            {

                returnVal = (P_Value * (OutputCounter.AdaptivePartRunCounter));
            }
            else
            {
                returnVal = 1;

            }
            AdaptivePartCounter = OutputCounter.AdaptivePartRunCounter;
            if (returnVal <= 1)
            {
                returnVal = 1;
            }
            if (OutputCounter.AdaptivePartRunCounter < CountPerPeriod || returnVal <= YieldTest)
            {
                returnVal = YieldTest;
            }
            _adNumber = returnVal;
        }

        public CCCCounter OutputCounter { get { return _cccCounter; } set { _cccCounter = value; } }

        public CCCDataLogger CCCDataLogger { get; set; }

        public void Default()
        {
            OutputCounter.Reset();
        }

    }

    public class CCCCounter
    {
        private string _counterName = String.Empty;
        public CCCCounter(string Name)
        {
            _counterName = Name;
        }
        public DateTime LastSaveLogTime { get; set; }
        public int TicFailCounter { get; set; }
        public int HstFailCounter { get; set; }

        public string uTICMachineName { get; set; }
        public int TicDefactCounter { get; set; }
        public int HstDefactCounter { get; set; }

        public int TicGoodPartCounter { get; set; }
        public int HstGoodPartCounter { get; set; }

        public int AdaptivePartRunCounter { get; set; }

        public int AdaptiveDefectCounter { get; set; }

        public int LastRunGoodCounter { get; set; }
        public int LastAdaptiveDefectCounter { get; set; }
        public int LastDefectCounter { get; set; }
        public CCCOutput.Trigger_Type McTriggerType { get; set; }
        public CCCOutput.MC_Trigger_Failure_Type TriggerFailureType { get; set; }
        public bool IsTriggering { get; set; }
        public bool IsUnderCCCAlert { get; set; }
        public DateTime LastTICAlertActiveTime { get; set; }

        public DateTime LastTICAlertClearTime { get; set; }

        public string LastCCCOutputUTICtime { get; set; }

        public int MCDownTriggering { get; set; }

        public int TicHighPercentTriggeringCounter
        {
            get; set;
        }

        public int HstHighPercentTriggeringCounter
        {
            get;
            set;
        }
        public int InternalTriggerCounter { get; set; }

        public void Default()
        {
            TicFailCounter = 0;
            HstFailCounter = 0;

            HstDefactCounter = 0;
            TicGoodPartCounter = 0;
            AdaptivePartRunCounter = 0;
            LastRunGoodCounter = 0;
            AdaptiveDefectCounter = 0;
            TicHighPercentTriggeringCounter = 0;
            HstHighPercentTriggeringCounter = 0;
            InternalTriggerCounter = 0;
            IsTriggering = false;
            uTICMachineName = "";
        }


        public void Reset()
        {
            IsTriggering = false;
            TicFailCounter = 0;
            HstFailCounter = 0;
            HstDefactCounter = 0;
            TicGoodPartCounter = 0;
            AdaptivePartRunCounter = 0;
            LastRunGoodCounter = 0;
            AdaptiveDefectCounter = 0;
            TicHighPercentTriggeringCounter = 0;
            HstHighPercentTriggeringCounter = 0;
            InternalTriggerCounter = 0;
            LastSaveLogTime = System.DateTime.Now;

        }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("TicFailCounter", TicFailCounter);
            xml.Write("HstFailCounter", HstFailCounter);
            xml.Write("TicDefactCounter", TicDefactCounter);
            xml.Write("HstDefactCounter", HstDefactCounter);
            xml.Write("TicGoodPartCounter", TicGoodPartCounter);
            xml.Write("HstGoodPartCounter", HstGoodPartCounter);
            xml.Write("AdaptivePartRunCounter", AdaptivePartRunCounter);
            xml.Write("AdaptiveDefectCounter", AdaptiveDefectCounter);
            xml.Write("LastSaveLogTime", LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            xml.Write("MCDownTriggering", MCDownTriggering);
            xml.Write("TicHighPercentTriggeringCounter", TicHighPercentTriggeringCounter);
            xml.Write("HstHighPercentTriggeringCounter", HstHighPercentTriggeringCounter);
            xml.Write("InternalTriggerCounter", InternalTriggerCounter);
            xml.Write("uTICMachineName", uTICMachineName);

            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            TicFailCounter = xml.Read("TicFailCounter", 0);
            HstFailCounter = xml.Read("HstFailCounter", 0);
            TicDefactCounter = xml.Read("TicDefactCounter", 0);
            HstDefactCounter = xml.Read("HstDefactCounter", 0);
            TicGoodPartCounter = xml.Read("TicGoodPartCounter", 0);
            HstGoodPartCounter = xml.Read("HstGoodPartCounter", 0);
            AdaptivePartRunCounter = xml.Read("AdaptivePartRunCounter", 0);
            AdaptiveDefectCounter = xml.Read("AdaptiveDefectCounter", 0);
            LastSaveLogTime = DateTime.ParseExact(xml.Read("LastSaveLogTime", (System.DateTime.Today.AddDays(-1)).ToString("dd-MMM-yy:HH:mm:ss")), "dd-MMM-yy:HH:mm:ss", null);
            MCDownTriggering = xml.Read("MCDownTriggering", 0);
            TicHighPercentTriggeringCounter = 0;
            HstHighPercentTriggeringCounter = 0;
            InternalTriggerCounter = xml.Read("InternalTriggerCounter", 0);
            uTICMachineName = xml.Read("uTICMachineName", "");
            xml.CloseSection();
        }
    }

    public class CCCDataLogger
    {
        public CCCDataLogger()
        {

        }

        public string DateTime { get; set; }
        public string TEST_RUN_COUNT { get; set; }
        public string YIELD_TARGET { get; set; }
        public string YIELD_LIMIT { get; set; }
        public string DEFECT_LIMIT { get; set; }
        public string LCL { get; set; }
        public string PART_COUNT { get; set; }
        public string PART_LIMIT_YIELD { get; set; }
        public string PART_TRIGGER { get; set; }
        public string DEFECT_COUNTER { get; set; }
        public string CCC_TRIGGERING { get; set; }
        public string TIC_TRIGGERING_COUNT { get; set; }
        public string HIGH_CRDL_PERCENT_TRIGGERING_COUNT { get; set; }
        public string CRDL_TRIGGERING_COUNT { get; set; }
        public string MC_TRIGGERING_COUNT { get; set; }
        public string CURRENT_TRIGGER_MC { get; set; }
        public string MC_TRIGGER_TYPE { get; set; }
        public string TRIGERING_TYPE { get; set; }
        public string TIC_MC_NUMBER { get; set; }
        public string FAILURE_TYPE { get; set; }
        public string ALL_MC_TRIGGER { get; set; }
        public string PART_RUNNING { get; set; }

        public string ALL_TIC_TRIGGERING { get; set; }
        public string TotalParts { get; set; }
        public string FailM1PartCounter { get; set; }
        public string FailM2PartCounter { get; set; }
        public string CurrentYield { get; set; }
        public string uTICdate { get; set; }
    }


    public class CCCFileLoger : EfiLogger
    {
        public const string PosTDFName = ".CSV";
        public const string NoData = "NO_DATA";

        public enum Category
        {
            DATE_TIME,
            TEST_RUN_COUNT,
            YIELD_TARGET,
            YIELD_LIMIT,
            ANC_COUNT_LIMIT,
            LCL,
            PART_COUNT,
            PART_LIMIT_YIELD,
            PART_TRIGGER,
            DEFECT_COUNTER,
            CCC_TRIGER,
            TIC_TRIGGERING_COUNT,
            HIGH_CRDL_PERCENT_TRIGGERING_COUNT,
            CRDL_TRIGGERING_COUNT,
            MC_TRIGGERING_COUNT,
            TRIGERING_MC,
            FAILURE_TYPE,
            ERROR_MSG_CODE,
            PART_RUNNING,
            TotalParts,
            FailM1PartCounter,
            FailM2PartCounter,
            CurrentYield,
            uTICTime
        }

        public enum Log_Triggering_type
        {
            Defect_Triggering,
            MC_CCC_Trigering
        }

        private DirectoryInfo _dirInfo;
        private string _logPath = string.Empty;
        public string _logName = string.Empty;
        public CCCFileLoger(string loggerPath, string fileName)
            : base(loggerPath, fileName)
        {
            _dirInfo = new DirectoryInfo(loggerPath);
            _logName = fileName;
            _logPath = loggerPath;

            SaveHeaderInfo();
            GenerateTDFPath();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // Properties ----------------------------------------------------------
        // Methods -------------------------------------------------------------


        public void LogLine(string data)
        {
            base.Log(data);
        }

        public void LogLine(Category category)
        {
            base.Log((int)category);
        }

        public void LogLine(Category category, string logline)
        {
            string fileName = _logPath + "\\" + _logName;

            base.Log((int)category, logline);

        }

        public string GetStandardTimeStamp()
        {
            return base.GetTimeStamp();
        }

        // Event handlers ------------------------------------------------------

        // Internal methods ----------------------------------------------------


        private void SaveHeaderInfo()
        {
            var header = new StringBuilder();
            int loopIndex = 0;
            foreach (Category category in Enum.GetValues(typeof(Category)))
            {
                if (loopIndex != 0)
                    header.Append(",");
                header.Append("\"" + category.ToString() + "\"");
                loopIndex++;
            }

            string headerFileName = _logPath + "\\" + _logName;

            while (!File.Exists(headerFileName))
            {
                header.Append("\r\n");
                using (Stream fs = new FileStream(headerFileName, FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(header.ToString());
                    }
                }
                Thread.Sleep(200);
            }
        }

        public void GenerateTDFPath()
        {
            if (!Directory.Exists(HSTSettings.Instance.Directory.TDFLocalDataPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.TDFLocalDataPath);
            }
        }
    }
}
