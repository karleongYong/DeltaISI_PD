using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Globalization;
using XyratexOSC.UI;
using System.IO.Ports;
using Seagate.AAS.Utils;
using DesktopTester.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;

namespace DesktopTester.Settings
{
    public class ApplicationSettings
    {        
        public SettingsEditor _settingsCOMPortSettingsEditor;
        protected SettingsDocument _settingsDoc;
        protected string _filePathUsers;
        
        public EventHandler OnSettingsChanged;

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("User Access")]
        private UserAccessSettings UserAccess
        {
            get;
            set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Test Electronics")]
        public TestProbePortSettings TestProbe
        {
            get;
            private set;
        }

        public UserAccessSettings getUserAccessSettings()
        {
            return UserAccess;
        }                  

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<ApplicationSettings> _instance
             = new Lazy<ApplicationSettings>(() => new ApplicationSettings());

        // accessor for instance
        public static ApplicationSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        
        public ApplicationSettings()
        {                       
            UserAccess = new UserAccessSettings();
            TestProbe = new TestProbePortSettings();
                        
            Load();            
        }        

        #endregion
        

        public string UsersSettingsFilePath
        {
            get
            {
                return CommonFunctions.Instance._usersPath;
            }

            set
            {
                _filePathUsers = value;
            }
        }
       
        public string UsersPath { get { return CommonFunctions.Instance._usersPath; } }
        
        private SettingsXml _xml;                
       
        public void LoadUsers()
        {            
            _settingsDoc = new SettingsDocument();            
            _settingsDoc.Load(CommonFunctions.Instance._usersPath, SettingsFileOption.Encrypted);            
            SettingsConverter.UpdateObjectFromNode(this.getUserAccessSettings(), _settingsDoc);            
        }

        public void Load()
        {            
            LoadUsers();

            string COMPortSettingsFilePath = string.Format("{0}DesktopTester.config", CommonFunctions.Instance.COMPortSettingsFileDirectory);
            _xml = new SettingsXml(COMPortSettingsFilePath);
            _xml.OpenSection("TestProbe");
            TestProbe.COMPort = (COMPortList)Enum.Parse(typeof(COMPortList), _xml.Read("COMPort", COMPortList.Unknown.ToString()));
            TestProbe.BaudRate = int.Parse(_xml.Read("BaudRate", "19200"));
            TestProbe.Parity = (Parity)Enum.Parse(typeof(Parity), _xml.Read("Parity", Parity.None.ToString()));
            TestProbe.DataBits = int.Parse(_xml.Read("DataBits", "8"));
            TestProbe.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _xml.Read("StopBits", StopBits.One.ToString()));   
            _xml.CloseSection();

            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
        }

        public void Save()
        {            
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);

            _xml.OpenSection("TestProbe");
            _xml.Write("COMPort", TestProbe.COMPort.ToString());
            _xml.Write("BaudRate", TestProbe.BaudRate.ToString());
            _xml.Write("StopBits", TestProbe.StopBits.ToString());
            _xml.Write("Parity", TestProbe.Parity.ToString());
            _xml.Write("DataBits", TestProbe.DataBits.ToString());
            _xml.CloseSection();
            
            _xml.Save();
        }        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Config]");
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(String) ||
                    pi.PropertyType == typeof(Boolean) ||
                    pi.PropertyType == typeof(Double) ||
                    pi.PropertyType == typeof(Int32))
                {
                    sb.AppendLine(pi.Name + "," + pi.GetValue(this, null));
                }
            }
            return sb.ToString();
        }        
    }

}
