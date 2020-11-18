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
    public class DirectorySettings
    {
        [Category("TSR Directories")]
        [DisplayName("Local TSR Recipe Directory")]
        [Description("Directory where the product TSR recipe files reside.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string TSRRecipLocalPath
        {
            get;
            set;
        }
        
        [Category("TSR Directories")]
        [DisplayName("Server TSR Recipe Directory")]
        [Description("Mapped Network Directory where the server stores the TSR recipe files.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string TSRRecipeGlobalPath
        {
            get;
            set;
        }
        [Category("Directories")]
        [DisplayName("LDU Directory")]
        [Description("Directory to keep LDU data file located.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string LDUDirectoryPath
        {
            get;
            set;
        }
        [Category("Directories")]
        [DisplayName("Log File Directory")]
        [Description("Directory where the log files reside.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string LogFilePath
        {
            get;
            set;
        }
        
        [Category("Directories")]
        [DisplayName("Error Handling Directory")]
        [Description("Directory where the error files reside.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ErrorHandlingPath
        {
            get;
            set;
        }

        [Category("Directories")]
        [DisplayName("Data Directory")]
        [Description("Directory where the data log and results files reside.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string DataPath
        {
            get;
            set;
        }

        [Category("TDF Directories")]
        [DisplayName("TDF Global Data Directory")]
        [Description("Directory where the TDF data log to global drive and results files reside.")]
        [ReadOnly(false)]
        public string TDFGlobalDataPath
        {
            get;
            set;
        }

        [Category("TDF Directories")]
        [DisplayName("TDF Local Data Directory")]
        [Description("Directory where the TDF data log to local drive and results files reside.")]
        [ReadOnly(false)]
        public string TDFLocalDataPath
        {
            get;
            set;
        }

        [Category("TDF Directories")]
        [DisplayName("TDF Local Data Back Directory")]
        [Description("Directory where the TDF data log to local drive and results files reside.")]
        [ReadOnly(false)]
        public string TDFDataBackPath
        {
            get;
            set;
        }

        [Category("Directories")]
        [DisplayName("IO State Change Directory")]
        [Description("Directory where the IO state change log file located.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string IOChangeStatePath
        {
            get;
            set;
        }

        [Category("Directories")]
        [DisplayName("ANC Output Data Directory")]
        [Description("Directory where CCC output data log file located.")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ANCDataLogPath
        {
            get;
            set;
        }

        [Category("Workorder Directory")]
        [DisplayName("Workorder Local Directory")]
        [Description("Please spacific local path that contain product Workorder")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string WorkorderLocalPath
        {
            get;
            set;
        }

        [Category("Workorder Directory")]
        [DisplayName("Workorder Global Directory")]
        [Description("Please spacific global path that contain product Workorder")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string WorkorderGlobalPath
        {
            get;
            set;
        }

        [Category("Machine Config Directory")]
        [DisplayName("Machine Config Global Directory")]
        [Description("Please spacific global path that contain spacial machine config file")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string McConfigGlobalPath
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [Browsable(true)]
        [Category("Spacial User Access")]
        [DisplayName("Spacial User Access")]
        [Description("Please spacific global path that contain spacial user file")]
        public string SpacialUserGlobalPath
        {
            get;
            set;
        }

        [Category("Machine Robot Point Directory")]
        [DisplayName("Machine Robot Point Directory")]
        [Description("Please spacific machine path that contain machine robot point file")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string MachineRobotPointPath
        {
            get;
            set;
        }
    }  
}
