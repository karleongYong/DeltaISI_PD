using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class ProductionCounterData
    {
        private const double WRBridgeLowwerLimitSpect = 0.5;
        private const double WRBridgeDecreaseStep = 0.025;
        public const int WRBridgeMinimumPartCounter = 100;
        private int processedHGACount = 0;
        private int _writerBridgePartRunCounter = 0;
        public int WriterFailCounter = 0;
        private int uph = 0;
        private int uph2 = 0;
        private double cycleTime = 0.0;
        private DateTime startDateTime = DateTime.Now;
        private int _samplingCounter = 0;
        private double _currentSamplingNumber = 0;
        private string fileName = "";
        private SettingsXml xml;
        private DateTime _lastSamplingCounterReset;
        private int _lastActiveSamplingPartCount;
        private int _wrBridgeRunningPercentage;
        private double _lastWRBridgePercentage;
        private double _currentR2RiskSamplingNumber;
        private int _r2RiskSamplingCounter = 0;
        private ProductionTriggerData _carrierTriggerData = new ProductionTriggerData();

        public int ProcessedHGACount { get { return processedHGACount; } set { processedHGACount = value; } }
        public int WriterBridgePartRunCounter { get { return _writerBridgePartRunCounter; } set { _writerBridgePartRunCounter = value; } }
        public int SamplingPerDayCounter { get { return _samplingCounter; } set { _samplingCounter = value; } }
        public int R2RiskSamplingPerDayCounter { get { return _r2RiskSamplingCounter; } set { _r2RiskSamplingCounter = value; } }
        public int WRBridgeFailureCounter
        {
            get { return _wrBridgeRunningPercentage; }
            set
            {
                _wrBridgeRunningPercentage = value;
            }
        }
        public double LastWRBridgePercentage
        {
            get { return _lastWRBridgePercentage; }
            set
            {
                _lastWRBridgePercentage = value;
            }
        }
        public int LastActiveSamplingPartCount { get { return _lastActiveSamplingPartCount; } set { _lastActiveSamplingPartCount = value; } }
        public double GetWRBridgeMaxSpect { get { return CommonFunctions.Instance.MeasurementTestRecipe.SamplingETOnDisk > 0 ? CommonFunctions.Instance.MeasurementTestRecipe.SamplingETOnDisk - 1 : 0; } }
        public double GetWRBridgeLowwerLimitSpec { get { return WRBridgeLowwerLimitSpect; } }
        public double GetWRBridgeIncreaseStep { get { return WRBridgeDecreaseStep; } }
        public double CycleTime
        {
            get { return cycleTime; }
            set { cycleTime = value; }
        }

        public int UPH
        {
            get { return uph; }
            set { uph = value; }
        }

        public int UPH2
        {
            get { return uph2; }
            set { uph2 = value; }
        }

        public ProductionTriggerData CarrierTriggeringData
        {
            get { return _carrierTriggerData; }
            set { _carrierTriggerData = value; }
        }
        public DateTime StartDateTime { get { return startDateTime; } }

        public string ProcessedHGACountString { get { return processedHGACount.ToString("#,##0"); } }
        public string CycleTimeString { get { return cycleTime.ToString("0.000"); } }
        public string UPHString { get { return uph.ToString("#,##0"); } }
        public string UPH2String { get { return uph2.ToString("#,##0"); } }
        public string StartDateTimeString { get { return startDateTime.ToString(); } }

        public DateTime LastSamplingResetDate { get { return _lastSamplingCounterReset; } }
        public double GetCurrentSamplingNumber
        {
            get { return _currentSamplingNumber; }
        }

        public ProductionCounterData(string fileName)
        {
            this.fileName = fileName;

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            xml = new SettingsXml(fileName);
            Load();
        }

        public void ResetWriterBridgCounter()
        {
            _wrBridgeRunningPercentage = 0;
            _lastWRBridgePercentage = 0.0;
            _writerBridgePartRunCounter = 0;
        }
        public void Reset()
        {
            HSTMachine.Workcell.HSTSettings.getConfigPerformance().UPH = 0;
            SamplingPerDayCounter = 0;
            processedHGACount = 0;
            cycleTime = 0.0;
            uph = 0;
            uph2 = 0;
            startDateTime = DateTime.Now;
            _lastSamplingCounterReset = DateTime.Now;
            _lastActiveSamplingPartCount = 0;
            _r2RiskSamplingCounter = 0;
            WriterFailCounter = 0;
            Save();
        }

        public void SetCurrentSamplingNumber()
        {
            _currentSamplingNumber = (((double)HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter /
                    (double)HSTMachine.Workcell.LoadCounter.ProcessedHGACount) * 100);
            if (double.IsNaN(_currentSamplingNumber) || double.IsInfinity(_currentSamplingNumber)) _currentSamplingNumber = 0;
        }

        public void GetCurrentR2RiskSamplingNumber()
        {
            _currentR2RiskSamplingNumber = (((double)R2RiskSamplingPerDayCounter /
                    (double)HSTMachine.Workcell.LoadCounter.ProcessedHGACount) * 100);
            if (double.IsNaN(_currentR2RiskSamplingNumber) || double.IsInfinity(_currentR2RiskSamplingNumber)) _currentR2RiskSamplingNumber = 0;
        }

        public void Load()
        {
            xml.OpenSection("Counter");
            if (!int.TryParse(xml.Read("ProcessedHGACount", "0"), out processedHGACount))
                processedHGACount = 0;
            if (!double.TryParse(xml.Read("CycleTime", "0.0"), out cycleTime))
                cycleTime = 0;
            if (!int.TryParse(xml.Read("UPH", "0"), out uph))
                uph = 0;
            if (!int.TryParse(xml.Read("UPH2", "0"), out uph2))
                uph2 = 0;
            if (!DateTime.TryParse(xml.Read("StartDateTime", DateTime.Now.ToString()), out startDateTime))
                startDateTime = DateTime.Now;
            if (!int.TryParse(xml.Read("SamplingCounter", "0"), out _samplingCounter))
                _samplingCounter = 0;
            if (!DateTime.TryParse(xml.Read("LastSamplingReset", DateTime.Now.ToString()), out _lastSamplingCounterReset))
                _lastSamplingCounterReset = DateTime.Now;
            CarrierTriggeringData.Load("CarrierTriggeringData", xml);

            if (!int.TryParse(xml.Read("LastActiveSamplingPartCount", "0"), out _lastActiveSamplingPartCount))
                _lastActiveSamplingPartCount = processedHGACount;
            if (!int.TryParse(xml.Read("WRBridgeRunningPercentage", "0.0"), out _wrBridgeRunningPercentage))
                _wrBridgeRunningPercentage = 0;
            if (!double.TryParse(xml.Read("LastWRBridgePercentage", "0.0"), out _lastWRBridgePercentage))
                _lastWRBridgePercentage = 0.0;
            if (!int.TryParse(xml.Read("WriterBridgePartRunCounter", "0"), out _writerBridgePartRunCounter))
                _writerBridgePartRunCounter = 0;
            xml.CloseSection();
        }

        public void Save()
        {
            HSTMachine.Workcell.HSTSettings.getConfigPerformance().UPH = uph;
            xml.OpenSection("Counter");
            xml.Write("ProcessedHGACount", processedHGACount.ToString());
            xml.Write("CycleTime", cycleTime.ToString());
            xml.Write("UPH", uph.ToString());
            xml.Write("UPH2", uph2.ToString());
            xml.Write("StartDateTime", startDateTime.ToString());
            xml.Write("SamplingCounter", _samplingCounter.ToString());
            xml.Write("LastSamplingReset", _lastSamplingCounterReset.ToString());
            CarrierTriggeringData.Save("CarrierTriggeringData", xml);
            xml.Write("LastActiveSamplingPartCount", _lastActiveSamplingPartCount.ToString());
            xml.Write("WRBridgeRunningPercentage", _wrBridgeRunningPercentage.ToString());
            xml.Write("LastWRBridgePercentage", _lastWRBridgePercentage);
            xml.Write("WriterBridgePartRunCounter", _writerBridgePartRunCounter);
            xml.CloseSection();
            xml.Save();
        }
    }

    public class ProductionTriggerData
    {
        private string _lastActiveDate = string.Empty;
        private int _counterBeforeTrig = 0;
        private int _counterAfterTrig = 0;
        private int _testPassCounter = 0;
        private int _testFailCounter = 0;
        private double _errorCumulative = 0.00;
        private int _totalPartRunCounter = 0;
        private double _errorPerPeriod = 0;
        private bool _buyOffProcessStarted = false;
        private bool _buyOffProcessFinished = true;
        private bool _criticalTriggeringActivated = true;
        private bool _samplingOverPercentageTriggeringActivated = true;
        public const int _buyOffCarrierCounterAfterTriggering = 10;

        public string LastActiveDate
        {
            get { return _lastActiveDate; }
            set { _lastActiveDate = value; }
        }

        public int CounterBeforeTriggering
        {
            get { return _counterBeforeTrig; }
            set { _counterBeforeTrig = value; }
        }

        public int CounterAfterTriggering
        {
            get { return _counterAfterTrig; }
            set { _counterAfterTrig = value; }
        }

        public int TestPassCounter
        {
            get { return _testPassCounter; }
            set { _testPassCounter = value; }
        }

        public int TestFailCounter
        {
            get { return _testFailCounter; }
            set { _testFailCounter = value; }
        }

        public double ErrorPerPeriod
        {
            get { return _errorPerPeriod; }
            set { _errorPerPeriod = value; }
        }

        public double ErrorCumulative
        {
            get { return _errorCumulative; }
            set { _errorCumulative = value; }
        }

        public int TotalPartRunCounter
        {
            get { return _totalPartRunCounter; }
            set { _totalPartRunCounter = value; }
        }

        public bool BuyoffProcessFinished
        {
            get { return _buyOffProcessFinished; }
            set { _buyOffProcessFinished = value; }
        }
        public bool BuyoffProcessStarted
        {
            get { return _buyOffProcessStarted; }
            set { _buyOffProcessStarted = value; }
        }

        public int BuyOffCarrierCounterAfterTriggering { get { return _buyOffCarrierCounterAfterTriggering; } }
        public bool CriticalTriggeringActivated { get { return _criticalTriggeringActivated; } set { _criticalTriggeringActivated = value; } }

        public bool InprocessReTest { get; set; }
        public int BuyOffPartLoadedCounter { get; set; }
        public int BuyOffPartFailStatusCounter { get; set; }
        public int BuyOffCarrierCounter { get; set; }
        public bool SamplingTriggeringOverPercentageActivate
        {
            get { return _samplingOverPercentageTriggeringActivated; }
            set { _samplingOverPercentageTriggeringActivated = value; }
        }
        public void Reset()
        {
            CounterBeforeTriggering = 0;
            CounterAfterTriggering = 0;
            TestPassCounter = 0;
            TestFailCounter = 0;
            TotalPartRunCounter = 0;
            _buyOffProcessStarted = false;
            _buyOffProcessFinished = true;
        }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("LastActiveDate", _lastActiveDate);
            xml.Write("CounterBeforeTrig", _counterBeforeTrig);
            xml.Write("CounterAfterTriggering", _counterAfterTrig);
            xml.Write("TestPassCounter", _testPassCounter);
            xml.Write("TestFailCounter", _testFailCounter);
            xml.Write("ErrorCumulative", _errorCumulative);
            xml.Write("TotalPartRunCounter", _totalPartRunCounter);
            xml.Write("BuyOffProcessFinished", _buyOffProcessFinished);
            xml.Write("BuyOffProcessStarted", _buyOffProcessStarted);
            xml.Write("CriticalTriggeringActivated", _criticalTriggeringActivated);
            xml.Write("SamplingTriggeringOverPercentage", _samplingOverPercentageTriggeringActivated);
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            _lastActiveDate = xml.Read("LastActiveDate", string.Empty);
            _counterBeforeTrig = xml.Read("CounterBeforeTrig", 0);
            _counterAfterTrig = xml.Read("CounterAfterTriggering", 0);
            _testPassCounter = xml.Read("TestPassCounter", 0);
            _testFailCounter = xml.Read("TestFailCounter", 0);
            _errorCumulative = xml.Read("ErrorCumulative", 0.00);
            _totalPartRunCounter = xml.Read("TotalPartRunCounter", 0);
            _buyOffProcessFinished = xml.Read("BuyOffProcessFinished", true);
            _buyOffProcessStarted = xml.Read("BuyOffProcessStarted", false);
            _criticalTriggeringActivated = xml.Read("CriticalTriggeringActivated", false);
            _samplingOverPercentageTriggeringActivated = xml.Read("SamplingTriggeringOverPercentage", false);
            xml.CloseSection();
        }
    }

    public class TriggeringRunResult
    {
        private Queue<HGAStatus[]> _resultPerRunTime = new Queue<HGAStatus[]>();

        public Queue<HGAStatus[]> ResultPerRunTime { get { return _resultPerRunTime; } set { _resultPerRunTime = value; } }
    }

}
