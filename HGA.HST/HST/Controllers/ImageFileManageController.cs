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
    public class ImageFileManageController : ControllerHST
    {
        private readonly object _lockObject = new object();

        public ImageFileManageController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            _workcell = workcell;
            _ioManifest = (HSTIOManifest)workcell.IOManifest;
        }

        //Method
        public override void InitializeController()
        {

        }

        /// <summary>
        /// Remove input image
        /// </summary>
        public void AutoRemoveInputImageFile()
        {
            try
            {
                int counter = 0;
                var dirInputImage = HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.ImagesOutputPath;

                //Input image
                if (Directory.Exists(dirInputImage))
                {
                    HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Clear();
                    DirectoryInfo backupInfo = new DirectoryInfo(dirInputImage);
                    foreach (FileInfo fi in backupInfo.GetFiles())
                    {
                        var trimtxt = fi.Extension.Split('.');
                        if (trimtxt[1] == "bmp")
                        {
                            var fileCmdSplit = fi.Name.Split('_');
                            var splitedate = fileCmdSplit[1].Split('-'); //DD-MM-YYYY
                            var createDate = new DateTime(Convert.ToInt32(splitedate[2]), Convert.ToInt32(splitedate[1]), Convert.ToInt32(splitedate[0]));
                            var diffDate = createDate - DateTime.Now; ;
                            if (Math.Abs(diffDate.Days) > HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.TotalDayToStoreImage)
                            {
                                counter++;
                                HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Add(fi.Name);
                                if (counter >= 2)
                                    break;
                            }
                        }
                    }
                }

                if (HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Count > 0)
                {
                    foreach (var file in HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList)
                    {
                        var getFile = System.IO.Path.Combine(@"" + dirInputImage + "\\" + file);

                        FileInfo originalFile = new FileInfo(getFile);
                        var fileCmdSplit = getFile.Split('_');

                        if (fileCmdSplit.Length >= 3)
                        {
                            if (File.Exists(getFile))
                            {
                                FileInfo deleteFile = new FileInfo(getFile);
                                deleteFile.Delete();
                                HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Remove(file);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var diffDay = DateTime.Now - HSTMachine.Workcell.InputImageDeletedList.LastDeletedDate;
                    if (diffDay.Days > 0)
                    {
                        HSTMachine.Workcell.InputImageDeletedList.AllFilesAccepted = false;
                        if (HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Count > 0)
                            HSTMachine.Workcell.InputImageDeletedList.ImageDeletedList.Clear();
                    }

                    if (!HSTMachine.Workcell.InputImageDeletedList.AllFilesAccepted)
                    {
                        HSTMachine.Workcell.InputImageDeletedList.LastDeletedDate = DateTime.Now;
                        HSTMachine.Workcell.InputImageDeletedList.AllFilesAccepted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

        }


        /// <summary>
        /// Remove output image
        /// </summary>
        public void AutoRemoveOutputImageFile()
        {
            try
            {
                int counter = 0;
                var dirOutputImage = HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.ImagesOutputPath;
                //Output image
                if (Directory.Exists(dirOutputImage))
                {
                    HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Clear();
                    DirectoryInfo backupInfo = new DirectoryInfo(dirOutputImage);
                    foreach (FileInfo fi in backupInfo.GetFiles())
                    {
                        var trimtxt = fi.Extension.Split('.');
                        if (trimtxt[1] == "bmp")
                        {
                            var fileCmdSplit = fi.Name.Split('_');
                            var splitedate = fileCmdSplit[1].Split('-'); //DD-MM-YYYY
                            var createDate = new DateTime(Convert.ToInt32(splitedate[2]), Convert.ToInt32(splitedate[1]), Convert.ToInt32(splitedate[0]));
                            var diffDate = createDate - DateTime.Now; ;
                            if (Math.Abs(diffDate.Days) > HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.TotalDayToStoreImage)
                            {
                                counter++;
                                HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Add(fi.Name);
                                if (counter >= 2)
                                    break;
                            }
                        }
                    }
                }

                if (HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Count > 0)
                {
                    foreach (var file in HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList)
                    {
                        var getFile = System.IO.Path.Combine(@"" + dirOutputImage + "\\" + file);

                        FileInfo originalFile = new FileInfo(getFile);
                        var fileCmdSplit = getFile.Split('_');

                        if (fileCmdSplit.Length >= 3)
                        {
                            if (File.Exists(getFile))
                            {
                                FileInfo deleteFile = new FileInfo(getFile);
                                deleteFile.Delete();
                                HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Remove(file);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var diffDay = DateTime.Now - HSTMachine.Workcell.OutputImageDeletedList.LastDeletedDate;
                    if (diffDay.Days > 0)
                    {
                        HSTMachine.Workcell.OutputImageDeletedList.AllFilesAccepted = false;
                        if (HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Count > 0)
                            HSTMachine.Workcell.OutputImageDeletedList.ImageDeletedList.Clear();
                    }

                    if (!HSTMachine.Workcell.OutputImageDeletedList.AllFilesAccepted)
                    {
                        HSTMachine.Workcell.OutputImageDeletedList.LastDeletedDate = DateTime.Now;
                        HSTMachine.Workcell.OutputImageDeletedList.AllFilesAccepted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

        }

    }
}
