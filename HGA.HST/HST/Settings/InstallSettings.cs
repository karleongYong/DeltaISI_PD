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


namespace Seagate.AAS.HGA.HST.Settings
{
    public class InstallSettings
    {    
        public bool BypassInputAndOutputEEsPickAndPlace;
        public bool BypassMeasurementTestAtTestProbe;        
        public bool BypassRFIDReadAtInput;
        public bool BypassRFIDAndSeatrackWriteAtOutput;
        public bool BypassVisionAtInputTurnStation;
        public bool BypassVisionAtOutput;
        public bool DryRunWithoutBoat;
        public string WorkOrderFilePath;

        
        [Category("Machine Information")]
        [DisplayName("Equipment ID")]
        [ReadOnly(true)]
        public string EquipmentID
        {
            get;
            set;
        }

        [Category("Machine Information")]
        [DisplayName("Location ID")]        
        public string LocationID
        {
            get;
            set;
        }

        [Category("Machine Information")]
        [DisplayName("Cell ID")]        
        public string CellID
        {
            get;
            set;
        }

        [Category("Machine Information")]
        [DisplayName("Factory")]
        [TypeConverter(typeof(FactoryList))]
        public FactoryList Factory
        {
            get;
            set;
        }
         

        [Category("Machine Operation")]
        [DisplayName("Operation Mode")]
        [TypeConverter(typeof(OperationMode))]
        [Description("Configure the machine operation mode.")]
        public OperationMode OperationMode
        {
            get;
            set;
        }
          [ReadOnly(true)]
        [Category("Machine Operation")]
        [DisplayName("Enable Audible Alarm")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Enable or disable the audible alarm system.")]
        public bool AudibleAlarmEnabled
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [Category("Machine Operation")]
        [DisplayName("Enable Data Log File Saving")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Enable or disable saving of data log into a data file.")]
        public bool DataLoggingFileSavingEnabled
        {
            get;
            set;
        }

        [Category("Machine Operation")]
        [DisplayName("Enable Process Step Checking")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Enable or disable checking on process step (Can be used for loopback Seaveyor test when disabled).")]
        [Browsable(false)]
        public bool ProcessStepCheckingEnabled
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [Category("Machine Operation")]
        [DisplayName("Enable Logging For RFID and Seatrack Record Update")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Enable or disable data logging for the update of record data stored in RFID and Seatrack.")]
        public bool DataLoggingForRFIDAndSeatrackRecordUpdateEnabled
        {
            get;
            set;
        }
          [ReadOnly(true)]
        [Category("Machine Operation")]
        [DisplayName("Enable Seatrack Record Update")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Enable or disable update of record data stored in Seatrack.")]
        public bool SeatrackRecordUpdateEnabled
        {
            get;
            set;
        }        

        [Category("Machine Operation")]
        [DisplayName("Clear Improper Shutdown message.")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Prevent the improper shutdown message from being triggered again when start the production mode.")]
        public bool ClearImproperShutDownMessage
        {
            get;
            set;
        }


        [Category("Machine Operation")]
        [DisplayName("Enable HGA Detection using Vision")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("When set to true use input and output camera to check the present of HGAs")]
        [Browsable(false)]
        public bool HGADetectionUsingVision
        {
            get;
            set;
        }


        [Category("Machine Operation")]
        [DisplayName("Enable Vision")]
        [TypeConverter(typeof(YesNoConverter))]        
        [Description("When set to true HST will use all cameras to performe inspection")]
        [Browsable(false)]
        public bool EnableVision
        {
            get;
            set;
        }

        [Category("Machine Operation")]
        [DisplayName("Conveyor Congestion Tolerance Time Limit")]
        [Description("Pop up 'Conveyor Congestion' warning message if exceeded the specified time limit in second. (use 0 to ignore conveyor congestion)")]
        public int ConveyorCongestionToleranceTimeLimit
        {
            get;
            set;
        }

        [DisplayName("Measurement Test Timeout Limit")]
        [Description("Pop up 'Measurement Test Timeout' message if exceeded the specified time limit in second. (use 0 to disable timeout check)")]
        public int MeasurementTestTimeOutLimit
        {
            get;
            set;
        }

        // controll hga purging duration
        [DisplayName("HGA Purging Duration")]
        [Description("Duration to purging HGA to ensure it is release from vacuum in milliseconds.\nIf set to 0, it will use the default duration of 200ms as a purging duration")]
        public int HGAPurgingDurationInms
        {
            get;
            set;
        }

        [Category("Machine Operation")]
        [DisplayName("Enable Maintenance Speed For Manual Move")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("When set to true HST will use Maintenance Speed for Manual Move")]
        public bool EnableMaintenanceSpeedForManualMove
        {
            get;
            set;
        }

        [Category("Machine Operation")]
        [DisplayName("RFID Update Option")]
        [TypeConverter(typeof(OperationMode))]
        [Description("Configure how the output carrier RFID should be updated.")]
        public RFIDUpdateOption RFIDUpdateOption
        {
            get;
            set;
        }

        [Category("Debug Options")]
        [DisplayName("Enable Debug Log")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Specify whether to log debug messages.")]
        public bool EnableDebugLog
        {
            get;
            set;
        }

        [Category("Debug Options")]
        [DisplayName("Enable Run Test Script Button")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Specify whether to enable 'Run Test Script' button.")]
        public bool EnableRunTestScriptButton
        {
            get;
            set;
        }

        [Category("Debug Options")]
        [DisplayName("Test Script")]
        [Description("Select Test script for internal testing.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TestScript
        {
            get;

            set;

        }

        [Category("Debug Options")]
        [DisplayName("Enable IO State Logging")]
        [TypeConverter(typeof(YesNoConverter))]
        [Description("Log IO change state.")]
        public bool LogIOChangeState
        {
            get;
            set;
        }


        [Category("TIC Error Detection Feature 1")]
        [DisplayName("Number of fail HGAs in a boat")]
        [Description("Set the number of fail HGAs to consider a boat fail with TIC errors")]
        public int TICFailHGAsInBoatLimit
        {
            get;

            set;
        }

        [Category("TIC Error Detection Feature 1")]
        [DisplayName("Number of consecutive fail boats")]
        [Description("Set the number of consecutive fail boats to trigger TIC errors message")]
        public int TICConsecutiveFailBoatsLimit
        {
            get;

            set;
        }

        [Category("TIC Error Detection Feature 2")]
        [DisplayName("Number of fail HGAs to Trigger Error")]
        [Description("Set the total number of fail HGAs for TIC errors Triggering")]
        public int TICFailHGAsTotalLimit
        {
            get;

            set;
        }

        [Category("TIC Error Detection Feature 2")]
        [DisplayName("Interval Time for TIC Error counting(Minute)")]
        [Description("Set the number of Minute to count the TIC Error. \nIf the past number of minute has TIC error count more or equal to the limit set for total number of TIC error,\nthe TIC error will be triggered.")]
        public int TICErrorCountingTimeInterval
        {
            get;

            set;
        }

        [Category("Input EE Fail Pick UP All HGAs Detection")]
        [DisplayName("Number of consecutive boats where Input EE Fail Pick UP All HGAs")]
        [Description("Set the number of consecutive fail boats to trigger Input EE fail pickup all errors message")]
        public int ConsecutiveFailBoatsFailPickupByInputEE
        {
            get;

            set;
        }

        [Category("Flattener Deployment Options")]
        [DisplayName("Deploy flattener BEFORE Precisor Nest vacuum is activated")]
        [Description("After HGAs are placed into the Precisor Nest, select 'Yes' to deploy flattener BEFORE Precisor Nest vacuum is activated.\nSelect 'No' to deploy AFTER Precisor Nest vacuum is activated")]
        [Browsable(false)]
        public bool FlattenerDeployBeforePrecisorVaccumON
        {
            get;

            set;
        }

        [Category("Flattener Deployment Options")]
        [DisplayName("Flattener Deploy Duration (ms)")]
        [Description("Specify the delay after Flattener deployment in ms before retract the Flattener")]
        public int FlattenerDeployDuration
        {
            get;

            set;
        }

        [Category("Flattener Deployment Options")]
        [DisplayName("Enable Flattener during the RUN process")]
        [Description("Specify whether to enable or disable the Flattener during the RUN process. (Manual mode not effected by this option)")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool EnableFlattenerAsPrecisor
        {
            get;

            set;
        }

        [Category("Flattener Deployment Options")]
        [DisplayName("Enable Flattener during the RUN iput process")]
        [Description("Specify whether to enable or disable the Flattener during the RUN as input process. (Manual mode not effected by this option)")]
        [TypeConverter(typeof(YesNoConverter))]
        public bool EnableFlattenerAsInput
        {
            get;

            set;
        }

        [Category("Dycem Cleaning Options")]
        [DisplayName("Number of touching on Dycem for Input EE ")]
        [Description("Specify the number of touching on Dycem for Input EE")]
        public int TotalNumberOfInputEETouchingOnDycem
        {
            get;

            set;
        }

        [Category("Dycem Cleaning Options")]
        [DisplayName("Number of touching on Dycem for Output EE ")]
        [Description("Specify the number of touching on Dycem for Output EE")]
        public int TotalNumberOfOutputEETouchingOnDycem
        {
            get;

            set;
        }

        [Category("Dycem Cleaning Options")]
        [DisplayName("Input EE touching on Dycem Duration (s)")]
        [Description("Specify the Duration of Input EE touching on Dycem in Second.")]
        public int InputEETouchingOnDycemDuration
        {
            get;

            set;
        }
        
        [Category("Dycem Cleaning Options")]
        [DisplayName("Output EE touching on Dycem Duration (s)")]
        [Description("Specify the Duration of Output EE touching on Dycem in Second.")]
        public int OutputEETouchingOnDycemDuration
        {
            get;
            set;
        }

        [Category("Capacitance Sampling Test")]
        [DisplayName("Sampling carrier count")]
        [Description("The size of the sampling. Set less than 1 will disable the sampling. ")]
        public int CapacitanceTestSamplingSize
        {
            get;
            set;
        }
         [ReadOnly(true)]
        [Category("Enable/Disable to save TDF file")]
        [DisplayName("Save and Send TDF File")]
        [Description("Save TDF file and send them to server. ")]
        public bool EnabledTDFFileSystem
        {
            get;
            set;
        }
          [ReadOnly(true)]
        [Category("Enable/Disable to save TDF file")]
        [DisplayName("Only Save TDF File")]
        [Description("Only Save TDF Backup file to local but not send to server")]
        public bool EnabledSaveTDFFileOnly
        {
            get;
            set;
        }
          [ReadOnly(false)]
        [Category("Enable/Disable to save TDF file")]
        [DisplayName("Save Backup TDF File")]
        [Description("Save TDF Backup file to local")]
        public bool EnabledSaveTDFBackupFile
        {
            get;
            set;
        }
          [ReadOnly(true)]
        [Category("Disable to home axis before run")]
        [DisplayName("Home Axis")]
        [Description("Home Axis")]
        public bool DisabledHomeAxis
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [Category("Enable/Disable to check user access control")]
        [DisplayName("User access Control System")]
        [Description("Check user access control")]
        public bool EnabledUserAccessControl
        {
            get;
            set;
        }

        [Category("Total data record display")]
        [DisplayName("Record display counter")]
        [Description("Record display counter")]
        public int DataRecordDisplayCounter
        {
            get;
            set;
        }

        [Category("ANC graph record counter")]
        [DisplayName("ANC Record display counter")]
        [Description("ANC Record display counter")]
        public int ANCGraphCounterMaximum
        {
            get;
            set;
        }
    }  
}
