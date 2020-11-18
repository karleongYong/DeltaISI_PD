using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;
using System.Runtime.InteropServices;
using Seagate.AAS.Parsel.Device.RFID.Hga;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class RFIDInfo
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string filePath;
        public FolaTagData RFIDTag;
        
        public string RFIDFileName
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }

        public FolaTagData RFIDTagData
        {
            get
            {
                return RFIDTag;
            }
            set
            {
                RFIDTag = value;
            }
        }

        public void LoadRFIDFile(string LocalRFIDFileName)
        {
            if (LocalRFIDFileName == "")
                throw new Exception("Local RFID file name is not set");
            if (!File.Exists(LocalRFIDFileName))
                throw new Exception(string.Format("Not found RFID File - {0}", LocalRFIDFileName));

            RFIDTool rfidTool = new RFIDTool(LocalRFIDFileName);

            this.RFIDTag.CarrierID = rfidTool.FileReadStringValue("Header", "CarrierID");
            this.RFIDTag.WorkOrder = rfidTool.FileReadStringValue("Header", "WorkOrder");
            this.RFIDTag.WorkOrderVersion = Convert.ToChar(rfidTool.FileReadStringValue("Header", "WorkOrderVersion"));
            this.RFIDTag.LastStep = rfidTool.FileReadIntegerValue("Header", "LastStep");
            this.RFIDTag.WriteCount = rfidTool.FileReadIntegerValue("Header", "WriteCount");
            this.RFIDTag.CheckSum = (byte)rfidTool.FileReadIntegerValue("Header", "CheckSum");

            this.RFIDTag.SetHGAData(0, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA1SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA1Status")));
            this.RFIDTag.SetHGAData(1, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA2SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA2Status")));
            this.RFIDTag.SetHGAData(2, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA3SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA3Status")));
            this.RFIDTag.SetHGAData(3, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA4SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA4Status")));
            this.RFIDTag.SetHGAData(4, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA5SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA5Status")));
            this.RFIDTag.SetHGAData(5, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA6SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA6Status")));
            this.RFIDTag.SetHGAData(6, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA7SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA7Status")));
            this.RFIDTag.SetHGAData(7, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA8SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA8Status")));
            this.RFIDTag.SetHGAData(8, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA9SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA9Status")));
            this.RFIDTag.SetHGAData(9, new StringBuilder(rfidTool.FileReadStringValue("HGA", "HGA10SerialNumber")), Convert.ToChar(rfidTool.FileReadStringValue("HGA", "HGA10Status")));

            this.RFIDTag.SetProcessStepData(0, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep1StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep1ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(1, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep2StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep2ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(2, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep3StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep3ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(3, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep4StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep4ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(4, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep5StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep5ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(5, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep6StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep6ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(6, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep7StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep7ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(7, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep8StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep8ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(8, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep9StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep9ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(9, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep10StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep10ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(10, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep11StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep11ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(11, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep12StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep12ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(12, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep13StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep13ProcessRecipe")));
            this.RFIDTag.SetProcessStepData(13, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep14StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep14ProcessRecipe")));

        }

        public RFIDInfo()
        {
            RFIDTag = new FolaTagData();
            this.RFIDTag.CarrierID = FileReadStringValue("Header", "CarrierID");
            this.RFIDTag.WorkOrder = FileReadStringValue("Header", "WorkOrder");
            if (FileReadStringValue("Header", "WorkOrderVersion") == "")
            {
                this.RFIDTag.WorkOrderVersion = '0';
            }
            else
            {
                this.RFIDTag.WorkOrderVersion = Convert.ToChar(FileReadStringValue("Header", "WorkOrderVersion"));
            }
            this.RFIDTag.LastStep = FileReadIntegerValue("Header", "LastStep");
            this.RFIDTag.WriteCount = FileReadIntegerValue("Header", "WriteCount");
            this.RFIDTag.CheckSum = (byte)FileReadIntegerValue("Header", "CheckSum");

            if (FileReadStringValue("HGA", "HGA1Status") == "")
            {
                this.RFIDTag.SetHGAData(0, new StringBuilder(FileReadStringValue("HGA", "HGA1SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(0, new StringBuilder(FileReadStringValue("HGA", "HGA1SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA1Status")));
            }

            if (FileReadStringValue("HGA", "HGA2Status") == "")
            {
                this.RFIDTag.SetHGAData(1, new StringBuilder(FileReadStringValue("HGA", "HGA2SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(1, new StringBuilder(FileReadStringValue("HGA", "HGA2SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA2Status")));
            }

            if (FileReadStringValue("HGA", "HGA3Status") == "")
            {
                this.RFIDTag.SetHGAData(2, new StringBuilder(FileReadStringValue("HGA", "HGA3SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(2, new StringBuilder(FileReadStringValue("HGA", "HGA3SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA3Status")));
            }

            if (FileReadStringValue("HGA", "HGA4Status") == "")
            {
                this.RFIDTag.SetHGAData(3, new StringBuilder(FileReadStringValue("HGA", "HGA4SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(3, new StringBuilder(FileReadStringValue("HGA", "HGA4SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA4Status")));
            }

            if (FileReadStringValue("HGA", "HGA5Status") == "")
            {
                this.RFIDTag.SetHGAData(4, new StringBuilder(FileReadStringValue("HGA", "HGA5SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(4, new StringBuilder(FileReadStringValue("HGA", "HGA5SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA5Status")));
            }

            if (FileReadStringValue("HGA", "HGA6Status") == "")
            {
                this.RFIDTag.SetHGAData(5, new StringBuilder(FileReadStringValue("HGA", "HGA6SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(5, new StringBuilder(FileReadStringValue("HGA", "HGA6SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA6Status")));
            }

            if (FileReadStringValue("HGA", "HGA7Status") == "")
            {
                this.RFIDTag.SetHGAData(6, new StringBuilder(FileReadStringValue("HGA", "HGA7SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(6, new StringBuilder(FileReadStringValue("HGA", "HGA7SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA7Status")));
            }

            if (FileReadStringValue("HGA", "HGA8Status") == "")
            {
                this.RFIDTag.SetHGAData(7, new StringBuilder(FileReadStringValue("HGA", "HGA8SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(7, new StringBuilder(FileReadStringValue("HGA", "HGA8SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA8Status")));
            }

            if (FileReadStringValue("HGA", "HGA9Status") == "")
            {
                this.RFIDTag.SetHGAData(8, new StringBuilder(FileReadStringValue("HGA", "HGA9SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(8, new StringBuilder(FileReadStringValue("HGA", "HGA9SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA9Status")));
            }

            if (FileReadStringValue("HGA", "HGA10Status") == "")
            {
                this.RFIDTag.SetHGAData(9, new StringBuilder(FileReadStringValue("HGA", "HGA10SerialNumber")), CommonFunctions.TEST_PASS_CODE);
            }
            else
            {
                this.RFIDTag.SetHGAData(9, new StringBuilder(FileReadStringValue("HGA", "HGA10SerialNumber")), Convert.ToChar(FileReadStringValue("HGA", "HGA10Status")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep1StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(0, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep1ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(0, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep1StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep1ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep2StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(1, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep2ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(1, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep2StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep2ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep3StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(2, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep3ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(2, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep3StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep3ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep4StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(3, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep4ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(3, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep4StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep4ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep5StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(4, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep5ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(4, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep5StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep5ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep6StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(5, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep6ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(5, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep6StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep6ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep7StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(6, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep7ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(6, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep7StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep7ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep8StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(7, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep8ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(7, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep8StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep8ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep9StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(8, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep9ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(8, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep9StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep9ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep10StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(9, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep10ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(9, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep10StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep10ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep11StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(10, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep11ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(10, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep11StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep11ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep12StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(11, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep12ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(11, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep12StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep12ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep13StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(12, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep13ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(12, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep13StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep13ProcessRecipe")));
            }

            if (FileReadStringValue("Process Step", "ProcessStep14StationCode") == "")
            {
                this.RFIDTag.SetProcessStepData(13, CommonFunctions.HST_STATION_CODE, new StringBuilder(FileReadStringValue("Process Step", "ProcessStep14ProcessRecipe")));
            }
            else
            {
                this.RFIDTag.SetProcessStepData(13, Convert.ToChar(FileReadStringValue("Process Step", "ProcessStep14StationCode")), new StringBuilder(FileReadStringValue("Process Step", "ProcessStep14ProcessRecipe")));
            }          
        }
        
        public string FileReadStringValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            return temp.ToString();
        }

        public int FileReadIntegerValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            string TempString = temp.ToString();
            if (TempString == "")
            {
                return 0;
            }
            else
            {
                return Int32.Parse(TempString);
            }
        }
        
    }
}
