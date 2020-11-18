using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using DesktopTester.Settings;
using DesktopTester.Models;
using DesktopTester.UserControls;


namespace DesktopTester.UI
{
    public partial class HeaderUserAccess : UserControl
    {
        public event EventHandler SwitchToOperationPage;
        frmMain MainForm;

        public HeaderUserAccess()
        {
            InitializeComponent();              
        }

        private UserAccessSettings _userAccess;
        private UserLoginControl _userLoginControl;

        public void UpdateCurrentUser(object sender, EventArgs e)
        {
            CurrentUserEventArgs currentUserEventArgs = e as CurrentUserEventArgs;
            setCurrentUser(currentUserEventArgs.CurrentUser);
        }

        public UserAccessSettings getUserAccessSettings()
        {
            return _userAccess;
        }       

        public void setCurrentUser(User currentUser)
        {
            _userAccess.setCurrentUser(currentUser);
        }

        public void AssignSettings(ApplicationSettings settings, frmMain frmMain)
        {
            MainForm = frmMain;
            _userAccess = settings.getUserAccessSettings();
            
            _userAccess.UserChanged += new EventHandler(UserChanged);
            settings.OnSettingsChanged += new EventHandler(SettingsChanged);
            _userAccess.UpdateCurrentUser += UpdateCurrentUser;

            _userAccess.getCurrentUser().Name = "Monitor";
            _userAccess.getCurrentUser().Password = "Monitor";
            _userAccess.getCurrentUser().Level = 0;
            _userLoginControl = new UserLoginControl(settings);
            _userLoginControl.Size = new Size(307, 105);
        }

        public void ShowLogin()
        {
            PopUpContextForm.Show(
                _userLoginControl,
                btnLogin,
                new Size(307, 105),
                new Point(-300, btnLogin.Height));
        }

        private void UserChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.BeginInvoke(this, () =>
                {
                    btnLogout.Enabled = (_userAccess.getCurrentUser() != _userAccess.DefaultUser);
                    txtUserName.Text = _userAccess.getCurrentUser().Name;

                    UserLevel level = _userAccess.getCurrentUser().Level;

                    XyratexOSC.Logging.Log.Info(this, "User:{0} from user group:{1} has logged in.", _userAccess.getCurrentUser().Name, level.ToString());

                    TabPage tabPageApplicationSettings = MainForm.getTabControl().TabPages[9] as TabPage;         
                    TabControl tabControlApplicationSettings = tabPageApplicationSettings.Controls.Find("tabControlApplicationSettings", true).FirstOrDefault() as TabControl;
                    
                    (MainForm.getTabControl().TabPages[0] as Control).Enabled = true;  // Bench Tests tab
                    (MainForm.getTabControl().TabPages[1] as Control).Enabled = true;  // Functional Tests tab                    
                    (MainForm.getTabControl().TabPages[8] as Control).Enabled = true;  // Desktop Tests tab

                    switch (level)
                    {
                        case UserLevel.Monitor:
                        case UserLevel.Operator:
                            userIcon.Image = Properties.Resources.UserOperator;    
                            (MainForm.getTabControl().TabPages[2] as Control).Enabled = false;  // Configurwtion & Setup tab
                            (MainForm.getTabControl().TabPages[3] as Control).Enabled = false;  // PCBA Calibration tab
                            (MainForm.getTabControl().TabPages[4] as Control).Enabled = false;  // Cable Calibration tab
                            (MainForm.getTabControl().TabPages[5] as Control).Enabled = false;  // Precisor Compensation tab
                            (MainForm.getTabControl().TabPages[6] as Control).Enabled = false;  // Debug tab
                            (MainForm.getTabControl().TabPages[7] as Control).Enabled = false;  // ServoCalibration tab
                            (MainForm.getTabControl().TabPages[9] as Control).Enabled = false;   // Application Settings tab
                            (tabControlApplicationSettings.TabPages[0] as Control).Enabled = false;  // COM Port tab
                            (tabControlApplicationSettings.TabPages[1] as Control).Enabled = false; // User Accounts tab        
                            break;
                        case UserLevel.Engineer:
                            userIcon.Image = Properties.Resources.UserEngineer; 
                            (MainForm.getTabControl().TabPages[2] as Control).Enabled = true;   // Configurwtion & Setup tab
                            (MainForm.getTabControl().TabPages[3] as Control).Enabled = true;  // PCBA Calibration tab
                            (MainForm.getTabControl().TabPages[4] as Control).Enabled = true;   // Cable Calibration tab
                            (MainForm.getTabControl().TabPages[5] as Control).Enabled = true;  // Precisor Compensation tab
                            (MainForm.getTabControl().TabPages[6] as Control).Enabled = true;   // Debug tab
                            (MainForm.getTabControl().TabPages[7] as Control).Enabled = true;   // ServoCalibration tab
                            (MainForm.getTabControl().TabPages[9] as Control).Enabled = true;   // Application Settings tab                                               
                            (tabControlApplicationSettings.TabPages[0] as Control).Enabled = true;  // COM Port tab
                            (tabControlApplicationSettings.TabPages[1] as Control).Enabled = false; // User Accounts tab                            
                            break;
                        case UserLevel.Administrator:                        
                            userIcon.Image = Properties.Resources.UserAdministrator;    
                            (MainForm.getTabControl().TabPages[2] as Control).Enabled = true;   // Configurwtion & Setup tab
                            (MainForm.getTabControl().TabPages[3] as Control).Enabled = true;   // PCBA Calibration tab
                            (MainForm.getTabControl().TabPages[4] as Control).Enabled = true;   // Cable Calibration tab
                            (MainForm.getTabControl().TabPages[5] as Control).Enabled = true;   // Precisor Compensation tab
                            (MainForm.getTabControl().TabPages[6] as Control).Enabled = true;   // Debug tab
                            (MainForm.getTabControl().TabPages[7] as Control).Enabled = true;   // ServoCalibration tab
                            (MainForm.getTabControl().TabPages[9] as Control).Enabled = true;   // Application Settings tab                            
                            (tabControlApplicationSettings.TabPages[0] as Control).Enabled = true;  // COM Port tab
                            (tabControlApplicationSettings.TabPages[1] as Control).Enabled = true; // User Accounts tab
                            break;
                    }
                });
            }
            catch { }
        }

        public void SettingsChanged(object sender, EventArgs e)
        {
            try
            {
                ApplicationSettings settings = sender as ApplicationSettings;

                if (settings != null && settings.getUserAccessSettings() != null)                
                    _userAccess = settings.getUserAccessSettings();                    
            }
            catch
            { }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            XyratexOSC.Logging.Log.Info(this, "User:{0} from user group:{1} has logged out.", _userAccess.getCurrentUser().Name, _userAccess.getCurrentUser().Level.ToString());
            _userAccess.LogOut();            
            SwitchToOperationPage.SafeInvoke(this, e);
        }

        private void btnLogin_MouseClick(object sender, MouseEventArgs e)
        {
            ShowLogin();
        }

        private void HeaderUserAccess_Load(object sender, EventArgs e)
        {
            UserChanged(this, new EventArgs());
        }
    }
}
