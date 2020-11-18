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

namespace Seagate.AAS.HGA.HST.Settings
{
    public class VisionSettings
    {
        [Category("Camera")]
        [DisplayName("1. Input Camera")]
        [Description("Input Camera configuration.")]
        public CameraSettings InputCamera
        {
            get;
            set;
        }

        [Category("Camera")]
        [DisplayName("3. Output Camera")]
        [Description("Output Camera configuration.")]
        public CameraSettings OutputCamera
        {
            get;
            set;
        }

        public VisionSettings()
        {

            InputCamera = new CameraSettings();
            OutputCamera = new CameraSettings();
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CameraSettings
        {

            public CameraSettings()
            {
                ImagesOutputPath = "";
                Recipe = "";
                CameraSerialNumber = "";
                SaveImagesLessThanTenHGAs = false;
                SaveAllImages = false;
            }


            [Category("Settings")]
            [DisplayName("Images Output File Path")]
            [Description("The directory path that hold the images capture by the camera.")]
            [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
            public string ImagesOutputPath
            {
                get;

                set;

            }
            [Category("Settings")]
            [DisplayName("Vision Tool Recipe")]
            [Description("Cognex Vision tool vpp file")]
            [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
            public string Recipe
            {
                get;

                set;

            }

            [Category("Settings")]
            [DisplayName("Camera Serial Number")]
            [Description("The serial number of the camera")]
            public string CameraSerialNumber
            {
                get;

                set;

            }

            [Category("Settings")]
            [DisplayName("Save Images if less than 10 HGAs")]
            [Description("Save images if less than 10 HGAs")]
            public bool SaveImagesLessThanTenHGAs
            {
                get;

                set;

            }
            [Category("Settings")]
            [DisplayName("Save All Images")]
            [Description("Save all the images")]
            public bool SaveAllImages
            {
                get;

                set;

            }

            [Category("Settings")]
            [DisplayName("Total day to store the image")]
            [Description("After spacific storing day the image will be automatically removed")]
            public int TotalDayToStoreImage
            {
                get;
                set;
            }
        }
    }
}
