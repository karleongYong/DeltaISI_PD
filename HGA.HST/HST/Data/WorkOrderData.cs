using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class WorkOrderData
    {
        private const int _maxProcStep = 14;
        public string WorkOrderNo { get; private set; }
        public string Version { get; private set; }
        public string ProductName { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string CreateDate { get; private set; }
        public string Released { get; private set; }
        public string TrayType { get; private set; }
        public string HgaPartNo { get; private set; }
        public string FsaPartNo { get; private set; }
        public string FsaVendor { get; private set; }
        public string FsaVendorName { get; private set; }
        public List<string> FsaVendorCode { get; private set; }

        public string FsaType { get; private set; }
        public string SlaChksumFailed { get; private set; }
        public string Wafer3Digit { get; private set; }
        public string TMWIP { get; private set; }
        public string TMWIR { get; private set; }
        public bool IBSCheck { get; private set; }
        public int NumberOfFolaSteps { get; private set; }
        public List<string> FolaProgramNameList { get; private set; }
        public List<string> FolaProcessStepList { get; private set; }       

        public string LocalWorkOrderDir { get; private set; }
        public string LogWorkOrderDir { get; private set; }
        public string LoadingStationCode { get; private set; }
        public string UnloadingStationCode { get; private set; }
       
        public bool IsLoaded { get; private set; }

        public string LocalFileName { get; set; }
        public string ServerFileName { get; set; }
        public string WoFileName { get; set; }
        public string RecipeFileName { get; set; }
        
        private int loadingFolaStepIndex = -1;        

        public WorkOrderData(string localWorkOrderDir, string logDir, string loadingStationCode)
        {
            Clear();
            LocalWorkOrderDir = localWorkOrderDir;
            LogWorkOrderDir = Path.Combine(logDir, "WorkOrder");
            if (!Directory.Exists(LogWorkOrderDir))
                Directory.CreateDirectory(LogWorkOrderDir);
            LoadingStationCode = loadingStationCode;                  
            LocalFileName = "";
            WoFileName = "";             
        }

        public string Tab
        {
            get
            {
                if (WorkOrderNo != null && WorkOrderNo.Length > 0)
                    return WorkOrderNo.Substring(WorkOrderNo.Length - 1);
                else return "";
            }
        }

        public string LoadingProgramName
        {
            get
            {
                if (loadingFolaStepIndex >= 0 && FolaProgramNameList.Count > loadingFolaStepIndex)
                    return FolaProgramNameList[loadingFolaStepIndex];
                else return "";
            }
        }        

        public int LoadingFolaStep
        {
            get
            {
                if (loadingFolaStepIndex >= 0)
                    return loadingFolaStepIndex + 1; // Start from 1 not 0
                else return -1;
            }
        }        

        public void LoadNewWO(string serverFileName)
        {
            CopyWOFileToLocal(serverFileName);
            LoadLocalFile();
        }

        public void ReloadWO()
        {
            CopyWOFileToLocal();
            LoadLocalFile();
        }

        private void CopyWOFileToLocal(string serverFileName)
        {
            WoFileName = Path.GetFileName(serverFileName);
            CopyWOFileToLocal();
        }

        private void CopyWOFileToLocal()
        {
            LocalFileName = Path.Combine(Seagate.AAS.HGA.HST.Machine.HSTMachine.Workcell.HSTSettings.Directory.WorkorderLocalPath, Path.GetFileName(WoFileName));
            ServerFileName = Path.Combine(Seagate.AAS.HGA.HST.Machine.HSTMachine.Workcell.HSTSettings.Directory.WorkorderGlobalPath, Path.GetFileName(WoFileName));

            FileInfo serverInfo = new FileInfo(ServerFileName);
            FileInfo localInfo = new FileInfo(LocalFileName);
            string action = "";
            string actionStatus = "DONE";

            try
            {
                if (!localInfo.Exists && serverInfo.Exists)
                {
                    File.Copy(ServerFileName, LocalFileName, true);
                    action = "NEW COPY";
                }
                else if (localInfo.Exists && serverInfo.Exists)
                {
                    if (localInfo.LastWriteTime.CompareTo(serverInfo.LastWriteTime) != 0)
                    {
                        File.Copy(ServerFileName, LocalFileName, true);
                        action = "OVERWRITTEN BY SERVER FILE";
                    }
                    else action = "NO CHANGES";
                }
                else
                {
                    action = "NO SERVER & LOCAL FILE";
                    actionStatus = "ERROR";
                }
            }
            catch (Exception ex)
            {
                actionStatus = ex.Message;
            }
            finally
            {
            }
        }

        public void LoadLocalFile()
        {
            if (LocalFileName == "")
                throw new Exception("Local Work Order file name is not set");
            if (!File.Exists(LocalFileName))
                throw new Exception(string.Format("Not found Local Work Order File - {0}", LocalFileName));

            WorkOrderTool woTool = new WorkOrderTool(LocalFileName);
            WorkOrderNo = woTool.FileReadStringValue("Header", "WorkOrder");
            Version = woTool.FileReadStringValue("Header", "WorkOrderVersion");
            ProductName = woTool.FileReadStringValue("Header", "ProductName");
            Description = woTool.FileReadStringValue("Header", "Description");
            Author = woTool.FileReadStringValue("Header", "Author");
            CreateDate = woTool.FileReadStringValue("Header", "CreateDate");
            Released = woTool.FileReadStringValue("Header", "Released");
            TrayType = woTool.FileReadStringValue("Header", "TrayType");
            FsaVendor = woTool.FileReadStringValue("Header", "FSA_Vendor");
            int firstComma = FsaVendor.IndexOf(',');
            FsaVendorName = FsaVendor.Substring(0, firstComma);
            FsaVendorCode = new List<string>(FsaVendor.Substring(firstComma + 1).Split(new char[] { ',' }));

            FsaType = woTool.FileReadStringValue("Header", "FSA_Type");
            SlaChksumFailed = woTool.FileReadStringValue("Header", "SLA_CHKSUM_FAIL");
            Wafer3Digit = woTool.FileReadStringValue("Header", "Wafer3Digit");
            TMWIP = woTool.FileReadStringValue("Header", "TMWI_P");
            TMWIR = woTool.FileReadStringValue("Header", "TMWI_R");
            var getIBSChk = woTool.FileReadStringValue("Header", "IBS_Check");
            if (string.IsNullOrEmpty(getIBSChk))
                IBSCheck = false;
            else
                IBSCheck = true;
            HgaPartNo = woTool.FileReadStringValue("BOM", "HGAPartNumber");
            FsaPartNo = woTool.FileReadStringValue("BOM", "FSAPartNumber");

            NumberOfFolaSteps = 0;            
            FolaProcessStepList.Clear();
            FolaProgramNameList.Clear();            

            int stepCnt;
            if (int.TryParse(woTool.FileReadStringValue("Process", "NumberOfSteps"), out stepCnt))
            {
                NumberOfFolaSteps = stepCnt;
                for (int i = 0; i < NumberOfFolaSteps; i++)
                {
                    FolaProcessStepList.Add(woTool.FileReadStringValue("Process", string.Format("ProcessStep{0}", i + 1)));
                    FolaProgramNameList.Add(woTool.FileReadStringValue("Process", string.Format("ProgramName{0}", i + 1)));

                    if (FolaProcessStepList[i] == CommonFunctions.HST_STATION_CODE.ToString())
                        RecipeFileName = FolaProgramNameList[i];
                }
            }

           
            loadingFolaStepIndex = FolaProcessStepList.IndexOf(LoadingStationCode);
            IsLoaded = true;
        }

        public WorkOrderData DeepCopy()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                return (WorkOrderData)formatter.Deserialize(stream);
            }
        }        

        public void Clear()
        {
            WorkOrderNo = "";
            Version = "1";
            ProductName = "";
            Description = "";
            Author = "";
            CreateDate = "";
            Released = "";
            TrayType = "";
            HgaPartNo = "";
            FsaPartNo = "";
            FsaVendor = "";
            FsaVendorName = "";
            FsaVendorCode = null;

            FsaType = "";
            SlaChksumFailed = "";
            Wafer3Digit = "";
            TMWIP = "";
            TMWIR = "";

            loadingFolaStepIndex = -1;            

            NumberOfFolaSteps = 0;
            FolaProgramNameList = new List<string>();
            FolaProcessStepList = new List<string>();            

            IsLoaded = false;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", WorkOrderNo, Version, ProductName, Description, Author, CreateDate, Released, TrayType, HgaPartNo, FsaPartNo);
        }        
    };
}
