using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.FGGigE;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.PMAlign;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using XyratexOSC.Logging;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Vision
{    
    // class for Hardware Object Camera
    public class Camera
    {
        public ICogAcqFifo cogAcqFifo;
        public CogImage8Grey grabImage;
        private ICogAcqTrigger cogAcqTrigger;
        private ICogAcqROI cogROIParams;
        private ICogAcqContrast cogContrast;
        private ICogAcqExposure cogExposure;
        private ICogAcqBrightness cogBrightness;

        private String IPAddress;

        private int acqTicket = 0;
        private int numGCAcqs = 0;

        public DateTime GrabTime { get; private set; }
        public string BackupImageFileName { get; private set; }

        /// <summary>
        /// Camera Constructor
        /// </summary>
        public Camera()
        {
            cogAcqFifo = null;
        }

        public Camera(ICogAcqFifo acqFifo)
        {
            if (acqFifo == null)
                return;
            cogAcqFifo = acqFifo;

            cogROIParams = cogAcqFifo.OwnedROIParams;

            cogContrast = cogAcqFifo.OwnedContrastParams;
            cogExposure = cogAcqFifo.OwnedExposureParams;
            cogBrightness = cogAcqFifo.OwnedBrightnessParams;
            cogAcqTrigger = cogAcqFifo.OwnedTriggerParams;

            GrabTime = DateTime.MinValue;
        }

        public bool GrabManualWithFile(string fileName)
        {
            bool status = GrabManual(true);

            if (status)
            {
                try
                {
                    GrabTime = DateTime.Now;
                    Bitmap bitmap = new Bitmap(grabImage.ToBitmap());

                    int retryCnt = 0;
                    bool done = false;
                    while (!done)
                    {
                        try
                        {
                            bitmap.Save(fileName);
                            done = true;
                        }
                        catch
                        {
                            Thread.Sleep(1);
                            retryCnt++;
                            if (retryCnt > 5)
                                throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not save image to disk. Original error: " + ex.Message);
                    status = false;
                }
            }

            return status;
        }

        public void AddInfoToImageFile(string fileName, string info, bool[] result, bool checkEmpty)
        {
            AddInfoToImageFile(fileName, info, result, checkEmpty, false);
        }

        public void AddInfoToImageFile(string fileName, string info, bool[] result, bool checkEmpty, bool isSecondSetBola) // isSecondSetBola is used for BOLA only
        {
            int[] xPos = new int[3] { 7, 40, 20 };
            int[] yPos = new int[3] { 780, 880, 950 };
            int xInc = 129;

            try
            {
                if (!File.Exists(fileName)) return;

                string tmpName = fileName + ".tmp";
                Bitmap bitmap = (Bitmap)Image.FromFile(fileName);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    // Line 1
                    using (Font font = new Font("Wingdings", 70, FontStyle.Bold))
                    {
                        int xx = xPos[0];
                        for (int i = 0; i < result.Length; i++)
                        {
                            if (checkEmpty)
                                graphics.DrawString(result[i] ? "l" : "¡", font, result[i] ? Brushes.Red : Brushes.LimeGreen, new Point(xx, yPos[0]));
                            else graphics.DrawString(result[i] ? "l" : "¡", font, result[i] ? Brushes.LimeGreen : Brushes.Red, new Point(xx, yPos[0]));
                            xx += xInc;
                        }
                    }

                    using (Font font = new Font("Tahoma", 25, FontStyle.Bold))
                    {
                        // Line 2
                        int xx = xPos[1];
                        string slotNo = "";
                        for (int i = 0; i < result.Length; i++)
                        {
                            if (isSecondSetBola)
                                slotNo = (i + 1 + 10).ToString();
                            else slotNo = (i + 1).ToString();

                            if (checkEmpty)
                                graphics.DrawString(slotNo, font, result[i] ? Brushes.Red : Brushes.LimeGreen, new Point(xx, yPos[1]));
                            else graphics.DrawString(slotNo, font, result[i] ? Brushes.LimeGreen : Brushes.Red, new Point(xx, yPos[1]));
                            xx += xInc;
                        }

                        // Line 3
                        graphics.DrawString(info, font, Brushes.LimeGreen, new Point(xPos[2], yPos[2]));
                    }
                }
                bitmap.Save(tmpName, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmap.Dispose();
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                int retryCnt = 0;
                bool done = false;
                while (!done)
                {
                    try
                    {
                        System.IO.File.Move(tmpName, fileName);
                        done = true;
                    }
                    catch
                    {
                        Thread.Sleep(1);
                        retryCnt++;
                        if (retryCnt > 5)
                            throw;
                    }
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show("Error: Could not write information to image file. Original error: " + ex.Message);
            }
        }

        public void BackupImageFile(string srcFileName, string id, string state)
        {
            string path = string.Format("{0}\\{1}\\{2}\\{3}", Path.GetDirectoryName(srcFileName), GrabTime.ToString("yyyy"), GrabTime.ToString("yyyyMM"), GrabTime.ToString("yyyyMMdd"));
            string fn = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(srcFileName), GrabTime.ToString("yyyyMMdd_HHmmss_fff"));
            if (id != "")
                fn += string.Format("_{0}", id);
            if (state != "")
                fn += string.Format("_{0}", state.Replace('/', '_'));
            fn += Path.GetExtension(srcFileName);
            BackupImageFileName = string.Format("{0}\\{1}", path, fn);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.Copy(srcFileName, BackupImageFileName, true);
        }

        /// <summary>
        /// Manual Grab Image
        /// </summary>
        public bool GrabManual(bool waitComplete)
        {
            bool Status = false;
            int trigNum, completeTicket;
            int retry = 0;

            while (retry < 3)// if something wrong during image capturing retry 3 times;
            {
                if (cogAcqFifo != null)
                {
                    if (this.cogAcqTrigger != null)
                    {
                        if (this.cogAcqTrigger.TriggerModel != CogAcqTriggerModelConstants.Manual)
                        {
                            // Select manual trigger mode
                            this.cogAcqTrigger.TriggerEnabled = false;
                            this.cogAcqTrigger.TriggerModel = CogAcqTriggerModelConstants.Manual;
                            this.cogAcqTrigger.TriggerEnabled = true;
                        }

                        // Acquire an image
                        try
                        {
                            // Acquire an image
                            acqTicket = cogAcqFifo.StartAcquire();

                            if (waitComplete)
                            {
                                grabImage = this.cogAcqFifo.CompleteAcquire(this.acqTicket,
                                                                            out completeTicket,
                                                                            out trigNum) as CogImage8Grey;
                                numGCAcqs++;
                                // do GC every COGFRAMENUM
                                if (this.numGCAcqs >= 4)
                                {
                                    GC.Collect();
                                    numGCAcqs = 0;
                                }
                            }
                            Status = true;
                            

                        }
                        catch (Exception ex)
                        {
                            retry++;
                            Log.Warn(this, "cogAcqFifo Acquire error: {0}, Retry:{1}", ex.Message, retry);
                            Thread.Sleep(2000);
                        }
                    }
                }
                else
                {
                    break;
                }
                if (Status)
                    break;
                

            }//while loop

            return Status;
        }

        /// <summary>
        /// Manual Grab Image
        /// </summary>
        public bool GrabWait()
        {
            bool Status = true;
            int trigNum, completeTicket;

            if (cogAcqFifo != null)
            {
                if (this.cogAcqTrigger != null)
                {
                    // Acquire an image
                    try
                    {
                        grabImage = this.cogAcqFifo.CompleteAcquire(this.acqTicket, out completeTicket, out trigNum) as CogImage8Grey;
                        this.numGCAcqs++;
                        // do GC every COGFRAMENUM
                        if (this.numGCAcqs >= 4)
                        {
                            GC.Collect();
                            this.numGCAcqs = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("cogAcqFifo Acquire error: " + ex.Message);
                        Status = false;
                    }
                }
            }
            else
                Status = false;

            return Status;
        }
    }
    // class for vision app with image processing and results
    public class VisionApp
    {
        
        private const int MAX_RESULT = 10;              // Max Results for one APP

        private String name;
        private CogToolBlock cogToolRun;
        public CogImage8Grey grabImage;
        private String ToolBlockFileName;
        private Utils.HGAStatus[] inspectResult = new Utils.HGAStatus[MAX_RESULT];    // order of front/rear
        private string imagedirectorypath;
        private bool savedefectimages;
        private bool saveallimages;
        private string _imagefilename;
        private bool _isboatreverse;
        private bool _isDycemBoat;
        private string _errorMessage;
        /// <summary>
        /// VisionApp Constructor
        /// </summary>
        public VisionApp()
        {
            _imagefilename = CommonFunctions.UNKNOWN;
            _isboatreverse = false;
            _isDycemBoat = false;
            _errorMessage = "NoError";
        }

        /// <summary>
        /// VisionApp Constructor
        /// </summary>
        public VisionApp(string _imagedirectorypath, bool _savedefectimages, bool _saveallimages ) : this()
        {
            LoadSettings(_imagedirectorypath, _savedefectimages, _saveallimages);
        }        

        /// <summary>
        /// Load the list of Project Files for Recipe
        /// </summary>

        public void LoadRecipe(string fileName, CameraLocation cameraLocation)
        {
            ToolBlockFileName = fileName;

            if (System.IO.File.Exists(ToolBlockFileName))
            {
                // upper as index 0
                this.cogToolRun = CogSerializer.LoadObjectFromFile(fileName) as CogToolBlock;
                this.cogToolRun.Name = Path.GetFileNameWithoutExtension(fileName);
            }
            else
            {
                switch(cameraLocation)
                {
                    case CameraLocation.InputStation:
                        Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(string.Format("Failed to load vision recipe file for input station camera: [{0}]", fileName));                        
                        break;
                    case CameraLocation.PrecisorStation:
                        Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(string.Format("Failed to load vision recipe file for precisor station camera: [{0}]", fileName));                        
                        break;
                    case CameraLocation.OutputStation:
                        Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(string.Format("Failed to load vision recipe file for output station camera: [{0}]", fileName));                        
                        break;
                }
            }
        }

        /// <summary>
        /// Do a run on current displayed Image
        /// </summary>
        public bool RunToolBlock(CogImage8Grey Image, String CarrierID)
        {
            bool runStatus = true;
            String colResult;
            int i;

            if (this.cogToolRun != null)
            {
                this.cogToolRun.Inputs["InputImage"].Value = Image as CogImage8Grey;
                this.cogToolRun.Inputs["ImageDirectoryPath"].Value = imagedirectorypath;
                this.cogToolRun.Inputs["SaveDefectImages"].Value = savedefectimages;
                this.cogToolRun.Inputs["SaveAllImages"].Value = saveallimages;
                this.cogToolRun.Inputs["CarrierID"].Value = CarrierID;


                this.cogToolRun.Run();

                ICogRunStatus cogStatus = this.cogToolRun.RunStatus;

                if (cogStatus.Result == CogToolResultConstants.Error)
                {
                    runStatus = false;
                    // error default all to invalid
                    for (i = 0; i < MAX_RESULT; i++)
                    {
                        inspectResult[i] = HGAStatus.Unknown;
                    }
                    
                }
                else
                {
                    for (i = 0; i < MAX_RESULT; i++)
                    {
                        colResult = String.Format("hga{0}", i + 1);
                        try
                        {
                            if(this.cogToolRun.Outputs[colResult].Value.ToString().Equals("1"))
                            {
                                inspectResult[i] = Utils.HGAStatus.HGAPresent;
                            }
                            else
                            {
                                inspectResult[i] = Utils.HGAStatus.NoHGAPresent;
                            }
                           
                        }
                        catch
                        {

                            inspectResult[i] = Utils.HGAStatus.Unknown;
                        }
                    }
                }
                // Need to save images
                if( savedefectimages || saveallimages)
                {
                    _imagefilename = this.cogToolRun.Outputs["ImageFileName"].Value.ToString();
                }
            }
            else
            {
                runStatus = false;
            }
            try
            {
                _isboatreverse = (bool)this.cogToolRun.Outputs["BoatReverse"].Value;
                _isDycemBoat = (bool)this.cogToolRun.Outputs["DyCemBoat"].Value;
                Log.Info(this, "Boat is reverse = {0}", _isboatreverse);
                Log.Info(this, "Boat is dycem boat = {0}", _isDycemBoat);
            }
            catch
            {
                _isboatreverse = false;
                _isDycemBoat = false;
            }

            try
            {
                if (!this.cogToolRun.Outputs["ErrorMessage"].Value.Equals("NoError"))
                {
                    runStatus = false;
                    _errorMessage = (string)this.cogToolRun.Outputs["ErrorMessage"].Value;
                    Log.Info(this, "Cognex Vision Error: {0}", _errorMessage);
                }
            }
            catch
            {

            }

            return runStatus;
        }

        public Utils.HGAStatus GetResult(int index)
        {
            return inspectResult[index];
        }

        public string ImageFileName()
        {
            return _imagefilename;
        }

        public bool IsBoatReverse()
        {
            return _isboatreverse;
        }

        public bool IsDycemBoat()
        {
            return _isDycemBoat;
        }

        public string ErrorMessage()
        {
            return _errorMessage;
        }

        public override string ToString()
        {
            if (cogToolRun == null)
                return base.ToString();
            else
                return cogToolRun.Name;
        }

        public void LoadSettings(string _imagedirectorypath, bool _savedefectimages, bool _saveallimages)
        {
            imagedirectorypath = _imagedirectorypath;
            savedefectimages = _savedefectimages;
            saveallimages = _saveallimages;
        }

        public bool FiducialToolBlock(CogImage8Grey Image, out double offset_x, out double offset_y, out double offset_theta)
        {
            offset_x = 0;
            offset_y = 0;
            offset_theta = 0;
            bool runStatus = true;
            String colResult;
            int i;

            if (this.cogToolRun != null)
            {
                this.cogToolRun.Inputs["InputImage"].Value = Image as CogImage8Grey;
                this.cogToolRun.Inputs["ImageDirectoryPath"].Value = imagedirectorypath;
                this.cogToolRun.Inputs["SaveDefectImages"].Value = savedefectimages;
                this.cogToolRun.Inputs["SaveAllImages"].Value = saveallimages;

                this.cogToolRun.Run();

                ICogRunStatus cogStatus = this.cogToolRun.RunStatus;

                if (cogStatus.Result == CogToolResultConstants.Error)
                {
                    runStatus = false;
                }
                else
                {
                    try
                    {
                        offset_x = (double)this.cogToolRun.Outputs["Tool1XOffset"].Value;
                        offset_y = (double)this.cogToolRun.Outputs["Tool1YOffset"].Value;
                        offset_theta = (double)this.cogToolRun.Outputs["Tool1XDegree"].Value;

                        Log.Info(this, "Fiducial offset values,{0},{1},{2}", offset_x, offset_y, offset_theta);
                    }
                    catch
                    {

                        offset_x = 0;
                        offset_y = 0;
                        offset_theta = 0;
                    }
                    
                }
                // Need to save images
                if (savedefectimages || saveallimages)
                {
                    _imagefilename = this.cogToolRun.Outputs["ImageFileName"].Value.ToString();
                }
            }
            else
            {
                runStatus = false;
            }


            return runStatus;

        }

    }
    // class for vision hardware componment, frameGrabber, etc.
    public class VisionHardware : IHardwareComponent
    {
        private const int MAX_FRAME_GCNUM = 10;          // Cog Acquisition Garbage Collection Limit
        private const int MAX_CAMERA = 2;               // Max Cameras
        private const int MAX_APP = 4;               // Max App
        private const int MAX_RESULT = 10;              // Max Results for one APP


        private CogDisplay[] cogDisplayMain = new CogDisplay[MAX_CAMERA];
        public ICogAcqFifo[] cogAcqFifo = new ICogAcqFifo[MAX_CAMERA];
        private ICogAcqTrigger[] cogAcqTrigger = new ICogAcqTrigger[MAX_CAMERA];
        private ICogFrameGrabber[] cogFrameGrabber = new ICogFrameGrabber[MAX_CAMERA];
        public CogImage8Grey[] grabImage = new CogImage8Grey[MAX_CAMERA];
        private CogToolBlock[] cogToolRun = new CogToolBlock[MAX_APP];
        private HGAStatus[,] inspectResult = new HGAStatus[MAX_APP, MAX_RESULT];    // order of front/rear, row no, col no.

        private int numGCAcqs = 0;
        private int[] imageSizeX = null;
        private int[] imageSizeY = null;
        private int[] displaySizeX = null;
        private int[] displaySizeY = null;
        private int[] acqTicket = null;
        private double[] cameraExposure = null;         // Default Exposure in ms

        private string recipeProjectDir = "c:\\Seagate\\vision";
        private bool _simulation = false;
        private string _name = "Vision System";
        private static VisionHardware _instance;


        public CogBlobTool CommonCogToolBlock;
        public CogPMAlignTool cogpmaligntool;

        public static VisionHardware Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VisionHardware();
                }

                return _instance;
            }
        }

        public string RecipeProjectDir
        {
            set { this.recipeProjectDir = value; }
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool Simulation
        { get { return _simulation; } }

        public void EMOReset()
        {
            ShutDown();
        }

        public void ShutDown()
        {
            CloseModule();
        }

        /// <summary>
        /// Vision Hardware Constructor
        /// </summary>
        public VisionHardware()
        {
            // initialize exposure settings and default sizes
            this.imageSizeX = new int[] { 1920, 1920 };
            this.imageSizeY = new int[] { 1200, 1200};
            this.displaySizeX = new int[] { 1920, 1920 };
            this.displaySizeY = new int[] { 1200, 1200};
            // set all exposure to 5ms for strobe to fire within window
            this.cameraExposure = new double[] { 5.0, 5.0};
            this.acqTicket = new int[] { 0, 0 };
        }

        /// <summary>
        /// Initialize Vision Framegrabber, Camera and Acq Buffer
        /// </summary>
        public void Initialize(bool simulation)
        {
            bool runStatus = true;

            int i;

            _simulation = simulation;
            // Get a reference to a collection of all the GigE Vision cameras
            // found by this system.
            CogFrameGrabberGigEs frameGrabbers = new CogFrameGrabberGigEs();

            // look for cameras only if enabled
            if (!_simulation)
            {
                // look for MAXCAMERA cameras
                if (frameGrabbers.Count < MAX_CAMERA)
                {
                    runStatus = false;
                    if (HSTMachine.Workcell.HSTSettings.Install.HGADetectionUsingVision)
                    {                        
                        Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError(String.Format("Not all 2 cameras found in this system! {0}", new Exception()));
                        XyratexOSC.Logging.Log.Info(this, "Error: Not all 2 cameras found in this system!");
                    }
                    else
                    {
                        Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError("HGA Detection using vision is disabled");                        
                        XyratexOSC.Logging.Log.Info(this, "HGA Detection using vision is disable!");
                    }
                }
                //else
                //{
                
                    try
                    {
                        for (i = 0; i < frameGrabbers.Count; i++)
                        {
                            this.cogFrameGrabber[i] = frameGrabbers[i];
                            
                            this.cogAcqFifo[i] = this.cogFrameGrabber[i].CreateAcqFifo("Generic GigEVision (Mono)",
                                                                                        CogAcqFifoPixelFormatConstants.Format8Grey, 0, false);

                            ICogAcqROI cogROIParams;
                            ICogAcqContrast cogContrast;
                            int x, y, width, height;

                            // Get the CogAcqContrastParams
                            cogContrast = this.cogAcqFifo[i].OwnedContrastParams;

                            // set default contrast params
                            if (cogContrast != null)
                            {
                                cogContrast.Contrast = 0.5;
                            }

                            this.cogAcqTrigger[i] = this.cogAcqFifo[i].OwnedTriggerParams;
                        }
                    }
                    catch (Exception ex)
                    {
                        runStatus = false;
                        Log.Error(this, "Error: Could not Initialize Camera(s). Original error: " + ex.Message);
                    }
            }
        }

        /// <summary>
        /// Sets Camera Exposure in ms
        /// </summary>
        public bool SetCameraExpoure(double millisecExposure, string serialnumber)
        {
            bool Status = true;
            int cameraModule = this.GetCameraModuleNumber(serialnumber);
            
            if (cameraModule == -1)
                return false;

            if (this.cogAcqFifo[cameraModule] != null)
            {
                ICogAcqExposure cogAcqExposure = this.cogAcqFifo[cameraModule].OwnedExposureParams;

                if (cogAcqExposure != null)
                {
                    // set exposure to 1ms
                    cogAcqExposure.Exposure = millisecExposure;
                }
            }
            else
                Status = false;

            return Status;
        }

        /// <summary>
        /// Displays a alignment reticle in middle of image
        /// </summary>
        public void DisplayReticle(int cameraModule)
        {
            if (this.cogDisplayMain != null)
            {
                // remove any old group
                foreach (string groupName in this.cogDisplayMain[cameraModule].StaticGraphics.ZOrderGroups)
                {
                    if (groupName == "Reticle")
                        this.cogDisplayMain[cameraModule].StaticGraphics.Remove("Reticle");
                }

                // image must be visible to see
                CogLineSegment cogLineSegment;

                cogLineSegment = new CogLineSegment();

                // vertical line
                cogLineSegment.SelectedSpaceName = "#";
                cogLineSegment.StartX = this.displaySizeX[cameraModule] / 2;
                cogLineSegment.StartY = 0;
                cogLineSegment.EndX = this.displaySizeX[cameraModule] / 2;
                cogLineSegment.EndY = this.displaySizeY[cameraModule];
                cogLineSegment.Color = CogColorConstants.Blue;
                cogLineSegment.LineWidthInScreenPixels = 1;

                this.cogDisplayMain[cameraModule].StaticGraphics.Add(cogLineSegment, "Reticle");

                // horizontal line
                cogLineSegment.StartX = 0;
                cogLineSegment.StartY = this.displaySizeY[cameraModule] / 2;
                cogLineSegment.EndX = this.displaySizeX[cameraModule];
                cogLineSegment.EndY = this.displaySizeY[cameraModule] / 2;

                this.cogDisplayMain[cameraModule].StaticGraphics.Add(cogLineSegment, "Reticle");
            }
        }


        /// <summary>
        /// Displays a alignment reticle in middle of image
        /// </summary>
        public void DisplayReticle2(int cameraModule, CogDisplay cogDisplay)
        {
            if (cogDisplay != null)
            {
                // remove any old group

                foreach (string groupName in cogDisplay.StaticGraphics.ZOrderGroups)
                {
                    if (groupName == "Reticle")
                        cogDisplay.StaticGraphics.Remove("Reticle");
                }
                // image must be visible to see
                CogLineSegment cogLineSegment;

                cogLineSegment = new CogLineSegment();

                // vertical line
                cogLineSegment.SelectedSpaceName = "#";
                cogLineSegment.StartX = this.displaySizeX[cameraModule] / 2;
                cogLineSegment.StartY = 0;
                cogLineSegment.EndX = this.displaySizeX[cameraModule] / 2;
                cogLineSegment.EndY = this.displaySizeY[cameraModule];
                cogLineSegment.Color = CogColorConstants.Blue;
                cogLineSegment.LineWidthInScreenPixels = 1;

                cogDisplay.StaticGraphics.Add(cogLineSegment, "Reticle");

                // horizontal line
                cogLineSegment.StartX = 0;
                cogLineSegment.StartY = this.displaySizeY[cameraModule] / 2;
                cogLineSegment.EndX = this.displaySizeX[cameraModule];
                cogLineSegment.EndY = this.displaySizeY[cameraModule] / 2;

                cogDisplay.StaticGraphics.Add(cogLineSegment, "Reticle");
            }
        }
        /// <summary>
        /// Clears any static overlay graphics
        /// </summary>
        public void ClearDisplay(int cameraModule)
        {
            if (this.cogDisplayMain[cameraModule] != null)
            {
                this.cogDisplayMain[cameraModule].StaticGraphics.Clear();

                while (this.cogDisplayMain[cameraModule].InteractiveGraphics.Count > 0)
                    this.cogDisplayMain[cameraModule].InteractiveGraphics.Remove(0);
            }
        }

        /// <summary>
        /// Fit Image into Display
        /// </summary>
        public void DisplayFitImage(int cameraModule)
        {
            if (this.cogDisplayMain[cameraModule] != null)
            {
                this.cogDisplayMain[cameraModule].Fit(true);
            }
        }

        /// <summary>
        /// Load Image into Main Display buffer
        /// </summary>
        public bool LoadImage(string imageFilename, int cameraModule)
        {
            bool Status = true;

            try
            {
                Bitmap bitmap = new Bitmap(imageFilename);
                CogImage8Grey img = new CogImage8Grey(bitmap);
                grabImage[cameraModule] = img;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);                        
                Status = false;
            }
            return Status;
        }

        /// <summary>
        /// Save Image in Main Display buffer
        /// </summary>
        public bool SaveImage(string imageFilename, int cameraModule)
        {
            bool Status = true;

            try
            {
                Bitmap bitmap = new Bitmap(this.grabImage[cameraModule].ToBitmap());
                bitmap.Save(imageFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not save image to disk. Original error: " + ex.Message);
                Status = false;
            }
            return Status;
        }


        /// <summary>
        /// Manual Grab Image into Main Display buffer
        /// </summary>
        public bool CameraGrabManual(int cameraModule, bool waitComplete)
        {
            bool Status = true;
            int trigNum, completeTicket;

            if (cameraModule < MAX_CAMERA)
            {
                if (this.cogAcqTrigger[cameraModule] != null)
                {
                    if (this.cogAcqTrigger[cameraModule].TriggerModel != CogAcqTriggerModelConstants.Manual)
                    {
                        // Select manual trigger mode
                        this.cogAcqTrigger[cameraModule].TriggerEnabled = false;
                        this.cogAcqTrigger[cameraModule].TriggerModel = CogAcqTriggerModelConstants.Manual;
                        this.cogAcqTrigger[cameraModule].TriggerEnabled = true;
                    }

                    // Acquire an image
                    try
                    {
                        // Acquire an image
                        this.acqTicket[cameraModule] = this.cogAcqFifo[cameraModule].StartAcquire();

                        if (waitComplete)
                        {
                            grabImage[cameraModule] = this.cogAcqFifo[cameraModule].CompleteAcquire(this.acqTicket[cameraModule],
                                                                                                                    out completeTicket,
                                                                                                                    out trigNum) as CogImage8Grey;
                            this.numGCAcqs++;
                            // do GC every COGFRAMENUM
                            if (this.numGCAcqs >= MAX_FRAME_GCNUM)
                            {
                                GC.Collect();
                                this.numGCAcqs = 0;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("cogAcqFifo Acquire error: " + ex.Message);
                        Status = false;
                    }
                }
            }
            else
                Status = false;

            return Status;
        }

        /// <summary>
        /// Manual Grab Image into Main Display buffer
        /// </summary>
        public bool CameraGrabWait(int cameraModule)
        {
            bool Status = true;
            int trigNum, completeTicket;

            if (cameraModule < MAX_CAMERA)
            {
                if (this.cogAcqTrigger[cameraModule] != null)
                {
                    // Acquire an image
                    try
                    {
                        grabImage[cameraModule] = this.cogAcqFifo[cameraModule].CompleteAcquire(this.acqTicket[cameraModule],
                                                                                                                out completeTicket,
                                                                                                                out trigNum) as CogImage8Grey;
                        this.numGCAcqs++;
                        // do GC every COGFRAMENUM
                        if (this.numGCAcqs >= MAX_FRAME_GCNUM)
                        {
                            GC.Collect();
                            this.numGCAcqs = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("cogAcqFifo Acquire error: " + ex.Message);
                        Status = false;
                    }
                }
            }
            else
                Status = false;

            return Status;
        }


        /// <summary>
        /// Load the list of Project Files for Recipe
        /// </summary>
        public bool LoadRecipe()
        {
            bool runStatus = true;
            int i;
            string fullProjectPath;

            for (i = 0; i < MAX_APP; i++)
            {
                fullProjectPath = String.Format("{0}\\{1}_{2}.vpp", this.recipeProjectDir, "Camera", i + 1);

                if (System.IO.File.Exists(fullProjectPath))
                {
                    // upper as index 0
                    this.cogToolRun[i] = CogSerializer.LoadObjectFromFile(fullProjectPath) as CogToolBlock;
                }
                else
                    runStatus = false;
            }

            return runStatus;
        }

        /// <summary>
        /// Do a run on current displayed Image
        /// </summary>
        public bool RunToolBlock(int cameraModule)
        {
            bool runStatus = true;
            String colResult;
            CogToolResultConstants toolColResult;
            int i;

            if (this.cogToolRun[cameraModule] != null)
            {
                this.cogToolRun[cameraModule].Inputs["InputImage"].Value = grabImage[cameraModule] as CogImage8Grey;

                this.cogToolRun[cameraModule].Run();

                ICogRunStatus cogStatus = this.cogToolRun[cameraModule].RunStatus;

                if (cogStatus.Result == CogToolResultConstants.Error)
                {
                    runStatus = false;
                    // error default all to invalid
                    for (i = 0; i < MAX_RESULT; i++)
                        this.inspectResult[cameraModule, i] = HGAStatus.Unknown;
                }
                else
                {
                    double[] blob = { 0, 0, 0, 0, 0, 0 };
                    double[] centerofmass_X = { 0, 0, 0, 0, 0, 0 };
                    CommonCogToolBlock = (CogBlobTool)this.cogToolRun[cameraModule].Tools["blobtool"];

                    CogBlobResultCollection resultcollection = CommonCogToolBlock.Results.GetBlobs();
                    for (int x = 0; x < resultcollection.Count; x++)
                    {
                        blob[x] = resultcollection[x].Area;
                        centerofmass_X[x] = resultcollection[x].CenterOfMassX;

                    }
                }
            }
            else
            {
                runStatus = false;
            }

            return runStatus;
        }

        /// <summary>
        /// Close the Vision Framegrabber, Camera and Acq Buffer
        /// </summary>
        public void CloseModule()
        {
            int i;

            for (i = 0; i < MAX_CAMERA; i++)
            {
                if (this.cogAcqFifo[i] != null)
                {
                    this.cogFrameGrabber[i].Disconnect(false);
                    this.cogAcqFifo[i].Flush();
                }
            }
        }


        /// <summary>
        /// Get Current Results
        /// </summary>
        public void GetLastResults(out HGAStatus[,] lastResults) // no need
        {
            // 0 is fail or unprocessed, 1 is pass
            lastResults = this.inspectResult;
        }

        /// <summary>
        /// Get Current Results
        /// </summary>
        public HGAStatus GetLastResults(int CameraModule, int ResultCol) //no need
        {
            HGAStatus result;
            result = inspectResult[CameraModule, ResultCol];
            return result;
        }

        /// <summary>
        /// Get Camera
        /// </summary>
        public ICogAcqFifo GetCamera(string serialnumber)
        {
            for (int i = 0; i < cogAcqFifo.Length; i++ )
            {
                try
                {
                    if (this.cogFrameGrabber[i].SerialNumber.Equals(serialnumber))
                        return cogAcqFifo[i];
                }
                catch (Exception ex)
                { }
            }
            return null;
        }

        public int GetCameraModuleNumber(string serialnumber)
        {
            for (int i = 0; i < cogAcqFifo.Length; i++)
            {
                try
                {
                    if (this.cogFrameGrabber[i].SerialNumber.Equals(serialnumber))
                        return i;
                }
                catch (Exception ex)
                { }
            }
            return -1;
        }

        public bool RunTool_Pattern(int cameraModule) //no need
        {
            bool runStatus = true;
            String colResult;
            CogToolResultConstants toolColResult;
            int i;

            if (this.cogToolRun[cameraModule] != null)
            {
                //configure the vision tool
                this.cogToolRun[cameraModule].Inputs["InputImage"].Value = grabImage[cameraModule] as CogImage8Grey;
                this.cogToolRun[cameraModule].Inputs["SaveImage"].Value = true;
                this.cogToolRun[cameraModule].Inputs["ImageDirectoryPath"].Value = "c:\\Seagate\\vision\\";
                
                this.cogToolRun[cameraModule].Run();

                ICogRunStatus cogStatus = this.cogToolRun[cameraModule].RunStatus;

                if (cogStatus.Result == CogToolResultConstants.Error)
                {
                    runStatus = false;
                    // error default all to invalid
                    for (i = 0; i < MAX_RESULT; i++)
                        this.inspectResult[cameraModule, i] = HGAStatus.Unknown;
                }
                else
                {
                    double[] score = { 0, 0, 0, 0, 0, 0 };
                    double[] centerofmass_X = { 0, 0, 0, 0, 0, 0 };

                    cogpmaligntool = (CogPMAlignTool)this.cogToolRun[cameraModule].Tools["PatternTool"];
                    CogPMAlignResults resultcollection = cogpmaligntool.Results;
                    for (int x = 0; x < resultcollection.Count; x++)
                    {
                        score[x] = resultcollection[x].Score;
                        centerofmass_X[x] = resultcollection[x].GetPose().TranslationX;
                    }
                }
            }
            else
            {
                runStatus = false;
            }

            return runStatus;
        }
    }
}

