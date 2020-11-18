using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Hw;
using System.Threading;
using System.IO;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.Logging;
using Seagate.AAS.UI;
using Seagate.AAS.HGA.HST.UI;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class FileManageController : ControllerHST
    {

        private const uint HRFileLocked = 0x80070020;
        private const uint HRPortionOfFileLocked = 0x80070021;
        private const int TDFFileOptimalLength = 1300;  //in byte type
        private readonly object _lockObject = new object();
        private bool isDisableSearch = false;
        private int DeletedCounterContinuous = 0;

        public FileManageController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            _workcell = workcell;
            _ioManifest = (HSTIOManifest)workcell.IOManifest;
        }

        //Method
        public override void InitializeController()
        {

        }

        private static bool FileIsLocked(System.IO.IOException ioException)
        {
            var errorCode = (uint)Marshal.GetHRForException(ioException);
            return errorCode == HRFileLocked || errorCode == HRPortionOfFileLocked;
        }

        public static bool TryOpen(string path,
                           FileMode fileMode,
                           FileAccess fileAccess,
                           FileShare fileShare,
                           out Stream stream)
        {
            try
            {
                stream = File.Open(path, fileMode, fileAccess, fileShare);
                return true;
            }
            catch (System.IO.IOException e)
            {
                if (!FileIsLocked(e))
                    throw;

                stream = null;
                return false;
            }
        }

        public bool IsNotFileBusy(string filePath)
        {
            Stream stream = null;
            bool retResult = false;
            try
            {
                var timeOut = TimeSpan.FromSeconds(1);
                retResult = TryOpen(filePath,
                             FileMode.Open,
                             FileAccess.ReadWrite,
                             FileShare.ReadWrite,
                             out stream);

                // Use stream...
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                retResult = true;
            }

            return retResult;
        }

        /// <summary>
        /// Map network drive for saving TDF data
        /// </summary>
        /// <returns></returns>
        public bool MappingNetworkDrive()
        {
            bool retConnect = false;

            //Return 0 = mapped
            //Return 85 = already mapped
            if (Directory.Exists(HSTSettings.Instance.Directory.TDFGlobalDataPath))
            {
                retConnect = true;
            }
            return retConnect;

        }

        public void TDFFilesManaging()
        {
            DirectoryInfo fileInfo = new DirectoryInfo(HSTSettings.Instance.Directory.TDFLocalDataPath);
            FileInfo fileItem = null;
            lock (_lockObject)
            {
                try
                {
                    List<FileInfo> loadedfile = new List<FileInfo>();
                    if (fileInfo.GetFiles().ToList().Count > 5)
                        loadedfile = fileInfo.GetFiles().Take(5).ToList();
                    else
                        loadedfile = fileInfo.GetFiles().Take(fileInfo.GetFiles().ToList().Count).ToList();

                    foreach (var item in loadedfile)
                    {
                        var getfile = System.IO.Path.Combine(@"" + item.FullName);
                        var confirmfile = new FileInfo(getfile);
                        var currentTimeStamp = DateTime.Now;
                        var timeInterval = currentTimeStamp - confirmfile.LastWriteTime;
                        if (confirmfile.Length > TDFFileOptimalLength) //good file : file size need to more than 10k
                        {
                            if (timeInterval.Days > 1 || timeInterval.Hours > 1 || timeInterval.Minutes > 1 || timeInterval.Seconds > 20) // creation time need to more than 1 s.
                            {
                                SendFileToGlobal(item);
                                fileItem = item;
                            }
                        }
                        else
                        {
                            if (timeInterval.Hours >= 1)
                                MoveFileToNotSendDirectory(item.Name);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    if (fileItem.FullName != string.Empty)
                    {
                        MoveFileToNotSendDirectory(fileItem.FullName);
                    }
                }
            }
        }

        public void SendFileToGlobal(FileInfo fileinfo)
        {
            DirectoryInfo fileInfo = new DirectoryInfo(HSTSettings.Instance.Directory.TDFLocalDataPath);
            FileInfo fileItem = null;
            lock (_lockObject)
            {
                try
                {
                    if (fileinfo != null && _workcell.getPanelOperation() != null)
                    {
                        bool copyFileCompleted = false;
                        string backupFilePathName = CheckBackupFilePath();

                        fileItem = fileinfo;

                        if (IsNotFileBusy(fileinfo.FullName))
                        {
                            try
                            {
                                var movefrom = System.IO.Path.Combine(@"" + fileinfo.FullName);
                                var moveTo = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFGlobalDataPath + fileItem.Name);
                                var moveToBackup = System.IO.Path.Combine(@"" + backupFilePathName + "\\" + fileItem.Name);
                                var splitFileName = fileItem.Name.Split('.');
                                FileInfo originalFile = new FileInfo(movefrom);
                                DateTime currentTime = DateTime.Now;

                                if (originalFile.Exists)
                                {
                                    if (File.Exists(movefrom) && !File.Exists(moveTo))
                                    {
                                        File.Copy(movefrom, moveTo);
                                        if (backupFilePathName != null && backupFilePathName != string.Empty && HSTMachine.Workcell.HSTSettings.Install.EnabledSaveTDFBackupFile)
                                            File.Copy(movefrom, moveToBackup);
                                    }
                                    do
                                    {
                                        if (File.Exists(moveTo))
                                        {
                                            var newFileName = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFGlobalDataPath + splitFileName[0] + ".TDF");
                                            FileInfo newFile = new FileInfo(moveTo);
                                            if (originalFile.Length == newFile.Length)
                                            {
                                                copyFileCompleted = true;
                                                newFile.MoveTo(newFileName);
                                                originalFile.Delete();
                                            }
                                            else
                                            {
                                                copyFileCompleted = true;
                                            }
                                        }

                                        Thread.Sleep(10);
                                    } while (!copyFileCompleted);

                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    Log.Info(this, "Save TDF file error, file name ==> {0}", fileinfo.FullName);
                }

            }
        }

        public void MoveFileToNotSendDirectory(string fileName)
        {
            try
            {
                var movefrom = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFLocalDataPath + "\\" + fileName);
                var moveToPath = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFLocalDataPath + "\\NotSendFile");
                var moveToFile = System.IO.Path.Combine(@"" + moveToPath + "\\" + fileName);

                if (!Directory.Exists(moveToPath))
                {
                    Directory.CreateDirectory(moveToPath);
                }
                if (File.Exists(movefrom))
                {
                    FileInfo originalFile = new FileInfo(movefrom);
                    FileInfo newFile = new FileInfo(moveToFile);
                    File.Copy(movefrom, moveToFile, true);
                    originalFile.Delete();
                }
            }
            catch (Exception)
            {
            }
        }

        private string CheckBackupFilePath()
        {
            string totalFilePath = string.Empty;
            try
            {
                var toDatePathName = DateTime.Now.Day.ToString("D2") + "-" + DateTime.Now.Month.ToString("D2") + "-" + DateTime.Now.Year.ToString();
                totalFilePath = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFDataBackPath + "\\" + toDatePathName);

                if (!Directory.Exists(HSTSettings.Instance.Directory.TDFDataBackPath))
                    Directory.CreateDirectory(HSTSettings.Instance.Directory.TDFDataBackPath);

                if (!Directory.Exists(totalFilePath))
                    Directory.CreateDirectory(totalFilePath);
            }
            catch (Exception)
            {
            }
            return totalFilePath;
        }

        //Try to manage file zero byte in global
        public void SearchUselessFileFromGlobal()
        {
            try
            {
                var moveTo = System.IO.Path.Combine(@"" + HSTSettings.Instance.Directory.TDFGlobalDataPath);
                DirectoryInfo dirInfo = new DirectoryInfo(HSTSettings.Instance.Directory.TDFGlobalDataPath);
                var file = dirInfo.GetFiles();
                var files = dirInfo.GetFiles().Where(f => f.LastWriteTime.Day < DateTime.Now.Day).ToList();
                if (files.Count > 0 && !isDisableSearch)
                {

                    DeletedCounterContinuous = 0;
                    FileInfo fileInfo = new FileInfo(files.FirstOrDefault().FullName);
                    if (fileInfo.Length == 0)
                        fileInfo.Delete();
                }
                else
                {
                    if (files.Count > 2)
                        isDisableSearch = false;
                    else
                    {
                        DeletedCounterContinuous++;
                        if (DeletedCounterContinuous > 10)
                        {
                            isDisableSearch = true;
                            DeletedCounterContinuous = 10;
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
        }

    }
}
