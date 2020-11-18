using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data
{
    public class TempDataFile
    {
        public string FileName { get; private set; }
        public bool Use { get; set; }

        public string WorkOrder { get; set; }
        public string WorkOrderVersion { get; set; }
        public int LastStep { get; set; }
        public int VendorCode { get; set; }
        public string PartNumber { get; set; }
        public string Tab { get; set; }
        public string ProgramName { get; set; }
        public string CarrierID { get; set; }
        public double UnloadUPH { get; set; }

        public string[] HGASN { get; set; }
        public string[] HGAStatus { get; set; }
        public DateTime TimeStamp { get; set;}

        public string FSAType { get; set; }
        public string WaferLot { get; set; }
        public string ProcessStep { get; set; }
        public string ProductName { get; set; }
        public string VendorName { get; set; }
        public string TMWIP { get; set; }
        public string TMWIR { get; set; }
        public string Remark { get; set; }

        public string LastStepStr
        {
            set
            {
                int lastStep;
                if (int.TryParse(value, out lastStep))
                    LastStep = lastStep;
                else LastStep = 0;
            }
        }

        public string VendorCodeStr
        {
            set
            {
                int code;
                if (int.TryParse(value, out code))
                    VendorCode = code;
                else VendorCode = 0;
            }
        }

        public string UnloadUPHStr
        {
            set 
            {
                double uph;
                if (double.TryParse(value, out uph))
                    UnloadUPH = uph;
                else UnloadUPH = 0;
            }
        }

        public string TimeStampStr
        {
            set
            {
                DateTime dtTmp;
                if (DateTime.TryParse(value, out dtTmp))
                    TimeStamp = dtTmp;
                else TimeStamp = DateTime.MinValue;
            }
        }

        private int hgaCount;

        public TempDataFile(string fileName, int hgaCount)
        {
            FileName = fileName;
            Use = false;
            this.hgaCount = hgaCount;
            HGASN = new string[hgaCount];
            HGAStatus = new string[hgaCount];
            Clear();
        }

        public void Clear()
        {
            WorkOrder = "";
            WorkOrderVersion = "";
            LastStep = 0;
            VendorCode = 0;
            PartNumber = "";
            Tab = "";
            ProgramName = "";
            CarrierID = "";
            UnloadUPH = 0;
            TimeStamp = DateTime.MinValue;

            FSAType = "";
            WaferLot = "";
            ProcessStep = "";
            ProductName = "";
            VendorName = "";
            TMWIP = "";
            TMWIR = "";
            Remark = "";

            for (int i = 0; i < hgaCount; i++)
            {
                HGASN[i] = "";
                HGAStatus[i] = "";
            }
        }
    }
}
