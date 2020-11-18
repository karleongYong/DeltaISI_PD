using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;
using System.Runtime.InteropServices;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class CompensationInfo
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string filePath;

        public string ProductName
        {
            get;
            set;
        }

        // Up Tab ch 1
        public double HGA1UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA2UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA3UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA4UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA5UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA6UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA7UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA8UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA9UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA10UpTabCapacitance1Compensation
        {
            get;
            set;
        }

        // Down Tab ch 1
        public double HGA1DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA2DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA3DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA4DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA5DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA6DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA7DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA8DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA9DownTabCapacitance1Compensation
        {
            get;
            set;
        }

        public double HGA10DownTabCapacitance1Compensation
        {
            get;
            set;
        }


        // Up Tab ch 2
        public double HGA1UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA2UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA3UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA4UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA5UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA6UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA7UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA8UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA9UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA10UpTabCapacitance2Compensation
        {
            get;
            set;
        }

        // Down Tab ch 2
        public double HGA1DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA2DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA3DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA4DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA5DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA6DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA7DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA8DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA9DownTabCapacitance2Compensation
        {
            get;
            set;
        }

        public double HGA10DownTabCapacitance2Compensation
        {
            get;
            set;
        }


        public string CompensationFileName
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
                
        public CompensationInfo(string LocalCompensationFileName, bool isCapaFunctional)
        {
            if (isCapaFunctional)
            {
                if (LocalCompensationFileName == "")
                    throw new Exception("Local Compensation file name is not set");
                if (!File.Exists(LocalCompensationFileName))
                    throw new Exception(string.Format("Not found Compensation File - {0}", LocalCompensationFileName));

                CompensationFileName = LocalCompensationFileName;

                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "Loaded Compensation File = {0}.", CompensationFileName);
                }
                this.ProductName = FileReadStringValue("Header", "ProductName");

                // Up Tab ch 1
                this.HGA1UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA1Capacitance1Compensation");
                this.HGA2UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA2Capacitance1Compensation");
                this.HGA3UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA3Capacitance1Compensation");
                this.HGA4UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA4Capacitance1Compensation");
                this.HGA5UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA5Capacitance1Compensation");
                this.HGA6UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA6Capacitance1Compensation");
                this.HGA7UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA7Capacitance1Compensation");
                this.HGA8UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA8Capacitance1Compensation");
                this.HGA9UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA9Capacitance1Compensation");
                this.HGA10UpTabCapacitance1Compensation = FileReadDoubleValue("UpTab", "HGA10Capacitance1Compensation");

                // Down Tab ch 1
                this.HGA1DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA1Capacitance1Compensation");
                this.HGA2DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA2Capacitance1Compensation");
                this.HGA3DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA3Capacitance1Compensation");
                this.HGA4DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA4Capacitance1Compensation");
                this.HGA5DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA5Capacitance1Compensation");
                this.HGA6DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA6Capacitance1Compensation");
                this.HGA7DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA7Capacitance1Compensation");
                this.HGA8DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA8Capacitance1Compensation");
                this.HGA9DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab", "HGA9Capacitance1Compensation");
                this.HGA10DownTabCapacitance1Compensation = FileReadDoubleValue("DownTab",
                    "HGA10Capacitance1Compensation");

                // Up Tab ch 2
                this.HGA1UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA1Capacitance2Compensation");
                this.HGA2UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA2Capacitance2Compensation");
                this.HGA3UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA3Capacitance2Compensation");
                this.HGA4UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA4Capacitance2Compensation");
                this.HGA5UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA5Capacitance2Compensation");
                this.HGA6UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA6Capacitance2Compensation");
                this.HGA7UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA7Capacitance2Compensation");
                this.HGA8UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA8Capacitance2Compensation");
                this.HGA9UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA9Capacitance2Compensation");
                this.HGA10UpTabCapacitance2Compensation = FileReadDoubleValue("UpTab", "HGA10Capacitance2Compensation");

                // Down Tab ch 2
                this.HGA1DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA1Capacitance2Compensation");
                this.HGA2DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA2Capacitance2Compensation");
                this.HGA3DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA3Capacitance2Compensation");
                this.HGA4DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA4Capacitance2Compensation");
                this.HGA5DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA5Capacitance2Compensation");
                this.HGA6DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA6Capacitance2Compensation");
                this.HGA7DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA7Capacitance2Compensation");
                this.HGA8DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA8Capacitance2Compensation");
                this.HGA9DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab", "HGA9Capacitance2Compensation");
                this.HGA10DownTabCapacitance2Compensation = FileReadDoubleValue("DownTab",
                    "HGA10Capacitance2Compensation");
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

        public double FileReadDoubleValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            double i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            string TempString = temp.ToString();
            if (TempString == "")
            {
                return 0.0;
            }
            else
            {
                return Double.Parse(TempString);
            }
        }
        
    }
}
