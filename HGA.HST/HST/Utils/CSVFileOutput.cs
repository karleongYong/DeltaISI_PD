using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Seagate.AAS.HGA.HST.Models;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class CSVFileOutput
    {
        public event EventHandler<CSVEventArgs> NewCSVRecordAdded;
        public event EventHandler NewCSVRecordUpdated;

        private string _fileName = "";
        private string _workingPath = "";
        private string[] _fieldNames;
        public CSVFileOutput(string Path)
        {
            _workingPath = Path;
        }

        public string CreateFileHeader(params string[] fieldNames)
        {
            string fileHeader = "";
            foreach(string fieldName in fieldNames)
            {
                fileHeader += fieldName + ",";
            }
            
            return fileHeader.Substring(0, fileHeader.Length-1);
        }        

        public bool GenerateNewCSVFile(/*string FileName, */params string[] fieldNames)
        {
            if (HSTSettings.Instance.Install.DataLoggingFileSavingEnabled)
            {
                _fileName = "DataLog_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
                _fieldNames = fieldNames;

                if (!Directory.Exists(HSTSettings.Instance.Directory.DataPath))
                {
                    Directory.CreateDirectory(HSTSettings.Instance.Directory.DataPath);
                }
                string SummaryFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath, _fileName);

                if (!File.Exists(SummaryFilePath))
                {
                    try
                    {
                        using (StreamWriter SummaryFile = new StreamWriter(SummaryFilePath, false))
                        {
                            SummaryFile.WriteLine(CreateFileHeader(fieldNames));
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.PopUpError("Error in creating a new '" + _fileName + "'", ex);
                    }
                }
            }
            return true;
        }

        public bool AppendNewCSVRecord(DataLog dataLog)
        {
            string HGAStatusString = "";
            switch (dataLog.HGAStatus)
            {
                case HGAStatus.TestedFail:
                    HGAStatusString = "Present and Tested Fail";
                    break;
                case HGAStatus.TestedPass:
                    HGAStatusString = "Present and Tested Pass";
                    break;
                case HGAStatus.HGAPresent:
                    HGAStatusString = "Present";
                    break;
                case HGAStatus.NoHGAPresent:
                    HGAStatusString = "No HGA Present";
                    break;
                case HGAStatus.Untested:
                    HGAStatusString = "Present but Untested";
                    break;
                default:
                    HGAStatusString = "Unknown";
                    break;
            }

            string shortTestString = "";
            switch (dataLog.ShortTest)
            {
                case ShortTest.NoShort:
                    shortTestString = "No short";
                    break;
                case ShortTest.Short:
                    shortTestString = "Short";
                    break;
                case ShortTest.NoTest:
                default:
                    shortTestString = "Empty";
                    break;
            }

            string softwareStatusString = "";
            switch (dataLog.SoftwareStatus)
            {
                case SoftwareStatus.Pause:
                    softwareStatusString = "Pause";
                    break;
                case SoftwareStatus.Start:
                    softwareStatusString = "Start";
                    break;
                case SoftwareStatus.Stop:
                    softwareStatusString = "Stop";
                    break;
                default:
                    softwareStatusString = "Empty";
                    break;
            }

            string operationModeString = "";
            switch (dataLog.OperationMode)
            {
                case OperationMode.Auto:
                    operationModeString = "Auto";
                    break;
                case OperationMode.DryRun:
                    operationModeString = "DryRun";
                    break;
                case OperationMode.Bypass:
                    operationModeString = "Bypass";
                    break;                
                case OperationMode.Simulation:
                    operationModeString = "Simulation";
                    break;
                default:
                    operationModeString = "Empty";
                    break;
            }           

            string CSVRecord = String.Join(",",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                dataLog.CarrierID,
                dataLog.CarrierSlot <= 0 ? "" : dataLog.CarrierSlot.ToString(),
                dataLog.HGASerialNumber,
                HGAStatusString,
                dataLog.WorkOrder,
                dataLog.ErrorMessageCode,
                dataLog.SetupFileName,
                dataLog.ReaderResistance < 0 ? "" : dataLog.ReaderResistance.ToString(),
                dataLog.Reader2Resistance < 0
                    ? ""
                    : !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable
                        ? dataLog.Reader2Resistance.ToString()
                        : "0",
                dataLog.DeltaISIReader1.ToString(),
                dataLog.DeltaISIReader2.ToString(),
                dataLog.WriterResistance < 0 ? "" : dataLog.WriterResistance.ToString(),
                dataLog.rHeaterResistance < 0 ? "" : dataLog.rHeaterResistance.ToString(),
                dataLog.wHeaterResistance < 0 ? "" : dataLog.wHeaterResistance.ToString(),
                dataLog.TAResistance < 0 ? "" : dataLog.TAResistance.ToString(),
                shortTestString,
                dataLog.ShortTestPosition,
                dataLog.Voltage_Delta1.ToString("F4"),          
                dataLog.Voltage_Delta2.ToString("F4"),          
                dataLog.ThermisterTemperature,
                dataLog.Sdet_Reader1.ToString("F4"),            
                dataLog.Sdet_Reader2.ToString("F4"),            
                dataLog.Delta_Polarity_R1.ToString("F4"),       
                dataLog.Delta_Polarity_R2.ToString("F4"),       
                dataLog.Uth_Equip_Id,                       
                dataLog.Uth_Time,
                dataLog.MachineLocation,
                dataLog.EquipmentID,
                dataLog.EquipmentType,
                dataLog.Sdet_Writer.ToString("F4"),             
                dataLog.Hst_Sdet_Delta_Writer.ToString("F4"),   
                dataLog.Wrbridge_Pct.ToString(),            
                dataLog.Wrbridge_Adap_Spec.ToString(),      
                softwareStatusString,
                dataLog.LoginUser,
                operationModeString,
                dataLog.WorkOrderVersion,
                dataLog.DeltaISIReader1FromTable,
                dataLog.DeltaISIReader2FromTable,
                dataLog.Volt_Ratio_Ch1.ToString("F4"),     
                dataLog.Volt_Ratio_Ch2.ToString("F4"),
                dataLog.Volt_Ratio_Ch3.ToString("F4"),
                dataLog.Volt_Ratio_Ch4.ToString("F4"),
                dataLog.Volt_Ratio_Ch5.ToString("F4"),
                dataLog.Volt_Ratio_Ch6.ToString("F4"), 
                dataLog.LDURes,
                dataLog.LDUSpec,
                dataLog.led_intercept <= 0 ? "" : dataLog.led_intercept.ToString("F4"),
                dataLog.i_threshold <= 0 ? "" : dataLog.i_threshold.ToString("F4"),
                dataLog.max_v_pd <= 0 ? "" : dataLog.max_v_pd.ToString("F4"),
                dataLog.ldu_turn_on_voltage <= 0 ? "" : dataLog.ldu_turn_on_voltage.ToString("F4"),
                dataLog.Sdet_i_threshold.ToString("F3"),
                dataLog.Delta_i_threshold.ToString("F3")
                ); //51

            if (HSTSettings.Instance.Install.DataLoggingFileSavingEnabled)
            {
                _fileName = "DataLog_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";

                string SummaryFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath, _fileName);
                if (!File.Exists(SummaryFilePath))
                {
                    try
                    {
                        using (StreamWriter SummaryFile = new StreamWriter(SummaryFilePath, false))
                        {
                            SummaryFile.WriteLine(CreateFileHeader(_fieldNames));
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.PopUpError("Error in creating a new '" + _fileName + "'", ex);
                    }
                }

                try
                {
                    using (StreamWriter SummaryFile = new StreamWriter(SummaryFilePath, true))
                    {
                        SummaryFile.WriteLine(CSVRecord);
                    }
                }
                catch (Exception ex)
                {
                    Notify.PopUpError("Error in appending a new record to '" + _fileName + "'", ex);
                }
            }

            NewCSVRecordAdded.SafeInvoke(this, new CSVEventArgs(CSVRecord));            

            return true;
        }

        public void DataUpdateCompleted()
        {
            NewCSVRecordUpdated.SafeInvoke(this, new EventArgs());
        }

        public void SaveDataCompareBeforeAfter(Models.DataLogBoforeAfter data)
        {
                var fileName = "DataBeforeAfter_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";

                if (!Directory.Exists(HSTSettings.Instance.Directory.DataPath))
                {
                    Directory.CreateDirectory(HSTSettings.Instance.Directory.DataPath);
                }
                string SummaryFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath, fileName);

                if (!File.Exists(SummaryFilePath))
                {
                    try
                    {
                        using (StreamWriter SummaryFile = new StreamWriter(SummaryFilePath, false))
                        {
                            SummaryFile.WriteLine(CreateFileHeader(fileName));
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.PopUpError("Error in creating a new '" + fileName + "'", ex);
                    }
                }
        }

        public void SaveDataReTestProcess(string data)
        {
            var fileName = "DataReTestProcess_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";

            if (!Directory.Exists(HSTSettings.Instance.Directory.DataPath))
            {
                Directory.CreateDirectory(HSTSettings.Instance.Directory.DataPath);
            }
            string SummaryFilePath = Path.Combine(HSTSettings.Instance.Directory.DataPath, fileName);

            if (!File.Exists(SummaryFilePath))
            {
                try
                {
                    using (StreamWriter SummaryFile = new StreamWriter(SummaryFilePath, false))
                    {
                        SummaryFile.WriteLine(CreateFileHeader(fileName));
                    }
                }
                catch (Exception ex)
                {
                    Notify.PopUpError("Error in creating a new '" + fileName + "'", ex);
                }
            }
        }

    }
}
