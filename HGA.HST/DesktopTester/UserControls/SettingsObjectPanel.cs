using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.Settings;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace DesktopTester.UserControls
{
    public partial class SettingsObjectPanel : UserControl
    {
        public event EventHandler UpdateUsersSettings;

        public SettingsObjectPanel()
        {
            InitializeComponent();
        }

        string _filePathUsers;
        object _settingsObject;

        /// <summary>
        /// Happens just before save
        /// </summary>
        public EventHandler OnSave;

        /// <summary>
        /// Happens just after load
        /// </summary>
        public EventHandler OnLoad;

        public object SettingsObject { get { return _settingsObject; } }

        public void AssignObject(object settingsObject, string UsersSettingsFilePath = null)
        {
            _settingsObject = settingsObject;
            propertyGrid.SelectedObject = settingsObject;

            if (UsersSettingsFilePath != null)
            {
                _filePathUsers = UsersSettingsFilePath;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            
        }        

        private void buttonSave_Click(object sender, EventArgs e)
        {            
            if (_filePathUsers != null)
            {
                SettingsDocument doc = SettingsConverter.ConvertObjectToDocument(_settingsObject, _filePathUsers);                                
                doc.Save(_filePathUsers, SettingsFileOption.Encrypted);                
                UpdateUsersSettings.SafeInvoke(this._settingsObject, e);
            }
        }
    }
}
