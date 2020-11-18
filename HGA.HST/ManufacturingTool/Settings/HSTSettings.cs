using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Seagate.AAS.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;

namespace BenchTestsTool.Settings
{
    public class HSTSettings
    {
        string _usersPath = "C:\\Seagate\\HGA.HST\\Setup\\Users.config";
        public SettingsEditor _settingsEditor;
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

        public UserAccessSettings getUserAccessSettings()
        {
            return UserAccess;
        }                  

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<HSTSettings> _instance
             = new Lazy<HSTSettings>(() => new HSTSettings());

        // accessor for instance
        public static HSTSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        
        public HSTSettings()
        {                       
            UserAccess = new UserAccessSettings();
                        
            Load();            
        }        

        #endregion
        

        public string UsersSettingsFilePath
        {
            get
            {
                return _filePathUsers;
            }

            set
            {
                _filePathUsers = value;
            }
        }
       
        public string UsersPath { get { return _usersPath; } }
        
        private SettingsXml _xml;                
       
        public void LoadUsers()
        {            
            _settingsDoc = new SettingsDocument();            
            _settingsDoc.Load(_usersPath, SettingsFileOption.Encrypted);            
            SettingsConverter.UpdateObjectFromNode(this.getUserAccessSettings(), _settingsDoc);            
        }

        public void Load()
        {            
            LoadUsers();                      

            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
        }

        public void Save()
        {            
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);            
        }

        public void SaveSettingsToFile(object sender, EventArgs e)
        {            
            Save();
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
