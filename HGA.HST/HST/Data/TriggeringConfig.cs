using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.Utils;
using System.ComponentModel;

namespace Seagate.AAS.HGA.HST.Data
{
    public class TriggeringConfig
    {
        int _triggerByCarrierCount;
        int _triggerByCarrierHour;
        int _perCarrierCounter;
        int _perHourCounter;
        double _failurePhase1Min;
        double _failurePhase2Min;
        bool _triggeringByCarrierEnable;
        private int _totalBuyoffCarrier;
        private bool _triggeringByErrorCodeEnable;
        private double _errorCodeFailurePercent;
        private int _errorCodeFailureCounter;
        private int _errorCodePartRunCounter;
        private double _errorCodePartPerPeriod;
        private bool _errorCodeTriggeringActivate;
        private bool _errorCodeHeightWriterFailure;
        private int _doubletestdelayTime;

        public TriggeringConfig()
        {
        }

        [Category("Triggering")]
        [DisplayName("Trigger by carrier enabled")]
        [Description("Enabled/Disabled trigger by carrier")]
        [ReadOnly(true)]
        public bool TriggerByCarrierEnabled
        {
            get { return _triggeringByCarrierEnable; }
            set { _triggeringByCarrierEnable = value; }
        }

        [Category("Triggering")]
        [DisplayName("Trigger by carrier count number")]
        [Description("Total carrier running that trigger will be activated")]
        [ReadOnly(true)]
        public int TrigerByCarrierCount
        {
            get { return _triggerByCarrierCount; }
            set { _triggerByCarrierCount = value; }
        }

        [Category("Triggering")]
        [DisplayName("Trigger by time count number")]
        [Description("Total time that trigger will be activated")]
        [ReadOnly(true)]
        public int TriggerByCarrierHour
        {
            get { return _triggerByCarrierHour; }
            set { _triggerByCarrierHour = value; }
        }

        [Category("Triggering")]
        [DisplayName("Minimum failure for phase-1")]
        [Description("If testing result more than this minimum number, fail alarm will be activated")]
        [ReadOnly(true)]
        public double FailurePhase1Min
        {
            get { return _failurePhase1Min; }
            set { _failurePhase1Min = value; }
        }

        [Category("Triggering")]
        [DisplayName("Minimum failure for phase-2")]
        [Description("If testing cumulate result more than this minimum number, fail alarm will be activated")]
        [ReadOnly(true)]
        public double FailurePhase2Min
        {
            get { return _failurePhase2Min; }
            set { _failurePhase2Min = value; }
        }

        [Category("Triggering")]
        [DisplayName("Carrier counter after triggering activated")]
        [Description("Carrier counter after triggering activated")]
        [ReadOnly(true)]
        public int PerCarrierCounter
        {
            get { return _perCarrierCounter; }
            set { _perCarrierCounter = value; }
        }

        [Category("Triggering")]
        [DisplayName("Time counter after triggering activated")]
        [Description("Time counter after triggering activated")]
        [ReadOnly(true)]
        public int PerHourCounter
        {
            get { return _perHourCounter; }
            set { _perHourCounter = value; }
        }

        [Category("Triggering")]
        [DisplayName("Total carrier number for byoff process")]
        [Description("Total carrier number for byoff process after triggering activated")]
        [ReadOnly(true)]
        public int TotalCarrierForBuyOff
        {
            get { return _totalBuyoffCarrier; }
            set { _totalBuyoffCarrier = value; }
        }

        [Category("Triggering")]
        [DisplayName("Enabled to triggering by error code")]
        [Description("Enabled or disabled to triggering by error code")]
        [ReadOnly(true)]
        public bool TriggerByErrorCodeEnabled
        {
            get { return _triggeringByErrorCodeEnable; }
            set { _triggeringByErrorCodeEnable = value; }
        }

        [Category("Triggering")]
        [DisplayName("Total percentage that will be activated triggering")]
        [Description("Total percentage that will be activated triggering")]
        [ReadOnly(true)]
        public double TriggerByErrorCodePercent
        {
            get { return _errorCodeFailurePercent; }
            set { _errorCodeFailurePercent = value; }
        }

        [Category("Triggering")]
        [DisplayName("Total failure that will be use to calculate percentage")]
        [Description("Total failure that will be use to calculate percentage")]
        [ReadOnly(true)]
        public int TriggerByErrorCodeFailureCounter
        {
            get { return _errorCodeFailureCounter; }
            set { _errorCodeFailureCounter = value; }
        }

        

        [Category("Triggering")]
        [DisplayName("Total part counter that will be use to calculate percentage")]
        [Description("Total part counter that will be use to calculate percentage")]
        [ReadOnly(true)]
        public int TriggerByErrorCodePartRunCounter
        {
            get { return _errorCodePartRunCounter; }
            set { _errorCodePartRunCounter = value; }
        }

        [Category("Triggering")]
        [DisplayName("Total part counter per period that will be use to calculate percentage")]
        [Description("Total part counter per period that will be use to calculate percentage")]
        [ReadOnly(true)]
        public double TriggerByErrorCodePartPerPeriod
        {
            get { return _errorCodePartPerPeriod; }
            set { _errorCodePartPerPeriod = value; }
        }

        [Category("Triggering")]
        [DisplayName("Active to alert if percentage per period out of spec")]
        [Description("Active to alert if percentage per period out of spec")]
        [ReadOnly(true)]
        public bool ErrorCodeTriggeringActivate
        {
            get { return _errorCodeTriggeringActivate; }
            set { _errorCodeTriggeringActivate = value; }
        }

        [Category("Triggering")]
        [DisplayName("Active to alert if writer bridge failure percentage per period out of spec")]
        [Description("Active to alert if writer bridge failure percentage per period out of spec")]
        [ReadOnly(true)]
        public bool ErrorCodeHeightWriterFailureActivate
        {
            get { return _errorCodeHeightWriterFailure; }
            set { _errorCodeHeightWriterFailure = value; }
        }

        [Category("Triggering")]
        [DisplayName("Delay for double test second period measurement done")]
        [Description("Delay for double test second period measurement done")]
        public int DoubleTestDelayMillisec
        {
            get { return _doubletestdelayTime; }
            set { _doubletestdelayTime = value; }
        }

        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("TrigerByCarrierEnable", _triggeringByCarrierEnable);
            xml.Write("TrigerByCarrierCount", _triggerByCarrierCount);
            xml.Write("TriggerByCarrierHour", _triggerByCarrierHour);
            xml.Write("FailurePhase1Min", _failurePhase1Min);
            xml.Write("FailurePhase2Min", _failurePhase2Min);
            xml.Write("PerCarrierCounter", _perCarrierCounter);
            xml.Write("PerHourCounter", _perHourCounter);
            xml.Write("TotalCarrierForBuyOff", _totalBuyoffCarrier);
            xml.Write("TriggerByErrorCodeEnabled", _triggeringByErrorCodeEnable);
            xml.Write("TriggerByErrorCodePercent", _errorCodeFailurePercent);
            xml.Write("TriggerByErrorCodePartRunCounter", _errorCodePartRunCounter);
            xml.Write("TriggerByErrorCodeFailureCounter", _errorCodeFailureCounter);
            xml.Write("ErrorCodeTriggeringActivate", _errorCodeTriggeringActivate);
            xml.Write("TriggerByErrorCodePartPerPeriod", _errorCodePartPerPeriod);
            xml.Write("DoubleTestDelayTime", _doubletestdelayTime);
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            TriggerByCarrierEnabled = xml.Read("TrigerByCarrierEnable", false);
            TrigerByCarrierCount = xml.Read("TrigerByCarrierCount", 1);
            TriggerByCarrierHour = xml.Read("TriggerByCarrierHour", 0);
            FailurePhase1Min = xml.Read("FailurePhase1Min", 0.0);
            FailurePhase2Min = xml.Read("FailurePhase2Min", 0.0);
            PerCarrierCounter = xml.Read("PerCarrierCounter", 0);
            PerHourCounter = xml.Read("PerHourCounter", 0);
            TotalCarrierForBuyOff = xml.Read("TotalCarrierForBuyOff", 1);
            TriggerByErrorCodeEnabled = xml.Read("TriggerByErrorCodeEnabled", false);
            TriggerByErrorCodePercent = xml.Read("TriggerByErrorCodePercent", 0.0);
            TriggerByErrorCodePartRunCounter = xml.Read("TriggerByErrorCodePartRunCounter", 0);
            TriggerByErrorCodeFailureCounter = xml.Read("TriggerByErrorCodeFailureCounter", 0);
            ErrorCodeTriggeringActivate = xml.Read("ErrorCodeTriggeringActivate", false);
            TriggerByErrorCodePartPerPeriod = xml.Read("TriggerByErrorCodePartPerPeriod", 0);
            DoubleTestDelayMillisec = xml.Read("DoubleTestDelayTime", 400);
            xml.CloseSection();
        }

    }

    public class ResistanceCheckConfig
    {
        public enum ResistanceCheckType
        {
            Unknown,
            CheckByHour,
            CheckByPartCounter
        }
        public ResistanceCheckConfig()
        {
        }

        private string _lastCheckByHourActive = string.Empty;
        private int _lastCheckByPartCountActive = 0;
        private bool _enabledCheck = false;
        private bool _overallTestResult = false;
        private double _checkByHourCounter = 0.0;
        private int _checkByPartCounter = 0;
        private bool _resistanceCriticalAtivated = false;
        private ResistanceCheckType _checkType = ResistanceCheckType.Unknown;
        private bool _isRetestProcessRequired = false;

        [Category("ResistanceCheck")]
        [DisplayName("Last checked time stamp")]
        [Description("Last checked time stamp")]
        [ReadOnly(true)]
        public string LastCheckByHourActive { get { return _lastCheckByHourActive; } set { _lastCheckByHourActive = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Last checked carrier counter stamp")]
        [Description("Last checked carrier counter stamp")]
        [ReadOnly(true)]
        public int LastCheckByPartCountActive { get { return _lastCheckByPartCountActive; } set { _lastCheckByPartCountActive = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Enabled/Disabled resistance check")]
        [Description("Enabled/Disabled resistance check")]
        [ReadOnly(true)]
        public bool EnabledResistanceCheck { get { return _enabledCheck; } set { _enabledCheck = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Overall result for resistance check")]
        [Description("Overall result for resistance check")]
        [ReadOnly(true)]
        public bool OverallTestResult { get { return _overallTestResult; } set { _overallTestResult = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Carrier counter")]
        [Description("Carrier counter for hour check")]
        [ReadOnly(true)]
        public double CheckByHourCounter { get { return _checkByHourCounter; }  set { _checkByHourCounter = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Part counter")]
        [Description("Part counter for hour check")]
        [ReadOnly(true)]
        public int CheckByPartCounter { get { return _checkByPartCounter; } set { _checkByPartCounter = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Check Type")]
        [Description("Resistance check type")]
        [ReadOnly(true)]
        public ResistanceCheckType CheckType { get { return _checkType; } set { _checkType = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Is check process required")]
        [Description("Is check process required")]
        [ReadOnly(true)]
        public bool IsRetestProcessRequired { get { return _isRetestProcessRequired; } set { _isRetestProcessRequired = value; } }

        [Category("ResistanceCheck")]
        [DisplayName("Activated checking process")]
        [Description("Activated checking process")]
        [ReadOnly(true)]
        public bool ResistanceCriticalActivated { get { return _resistanceCriticalAtivated; } set { _resistanceCriticalAtivated = value; } }
        public void Save(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            xml.Write("LastCheckByHourActive", _lastCheckByHourActive);
            xml.Write("LastCheckByPartCountActive", _lastCheckByPartCountActive);
            xml.Write("EnabledResistanceCheck", _enabledCheck);
            xml.Write("OverallTestResult", _overallTestResult);
            xml.Write("CheckByHourCounter", _checkByHourCounter);
            xml.Write("CheckByPartCounter", _checkByPartCounter);
            xml.Write("ResistanceCriticalActivated", _resistanceCriticalAtivated);
            xml.Write("IsRetestProcessRequired", _isRetestProcessRequired);
            xml.Write("Type", _checkType.ToString());
            xml.CloseSection();
        }

        public void Load(string section, SettingsXml xml)
        {
            xml.OpenSection(section);
            LastCheckByHourActive = xml.Read("LastCheckByHourActive", DateTime.Now.ToString());
            LastCheckByPartCountActive = xml.Read("LastCheckByPartCountActive", 1);
            EnabledResistanceCheck = xml.Read("EnabledResistanceCheck", false);
            OverallTestResult = xml.Read("OverallTestResult", false);
            CheckByHourCounter = xml.Read("CheckByHourCounter", 0.0);
            CheckByPartCounter = xml.Read("CheckByPartCounter", 0);
            ResistanceCriticalActivated = xml.Read("ResistanceCriticalActivated",false);
            IsRetestProcessRequired = xml.Read("IsRetestProcessRequired", false);
            CheckType = (ResistanceCheckType)Enum.Parse(typeof(ResistanceCheckType), xml.Read("Type", ResistanceCheckType.Unknown.ToString()));
            xml.CloseSection();
        }

    }
}
