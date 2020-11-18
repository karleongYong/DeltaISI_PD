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
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.UserControls;
using Seagate.AAS.Security.AccessControl;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class HeaderUserAccess : UserControl
    {
        public event EventHandler SwitchToOperationPage;

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

        public void AssignSettings(HSTSettings settings)
        {
            _userAccess = settings.getUserAccessSettings();            
            _userLoginControl = new UserLoginControl(settings);
            _userLoginControl.Size = new Size(307, 105);
            _userAccess.UserChanged += new EventHandler(UserChanged);
            settings.OnSettingsChanged += new EventHandler(SettingsChanged);
            _userAccess.UpdateCurrentUser += UpdateCurrentUser;
            UserAccessControl.UserLogin += UserAccessControl_UserLogin;
        }

        public void ShowLogin()
        {
            UserAccessControl.Login();

            //PopUpContextForm.Show(
            //    _userLoginControl,
            //    btnLogin,
            //    new Size(307, 105),
            //    new Point(-300, btnLogin.Height));
        }

        private void UserAccessControl_UserLogin(UserInformation userInfo)
        {
            var loginUser = userInfo;

            User user = new User();
            user.Name = userInfo.UserName;
            user.EmpName = userInfo.EmpName;

            //Load spaical user
            CommonFunctions.Instance.LoadSpacialUser();

            //sr = sr eng and sr tech
            if (IsSpacialUser(user.Name))
                user.Level = UserLevel.Engineer;
            else if (string.Compare(userInfo.JobFunction, "sr", StringComparison.InvariantCultureIgnoreCase) == 0)
                if(user.Name == "62562")
                    user.Level = UserLevel.Administrator;
                else
                    user.Level = UserLevel.Engineer;
            else if (string.Compare(userInfo.JobFunction, "engineer", StringComparison.InvariantCultureIgnoreCase) == 0)
                user.Level = UserLevel.Engineer;
            else if (string.Compare(userInfo.JobFunction, "technician", StringComparison.InvariantCultureIgnoreCase) == 0)
                user.Level = UserLevel.Technician;
            else if (string.Compare(userInfo.JobFunction, "operator", StringComparison.InvariantCultureIgnoreCase) == 0)
                user.Level = UserLevel.Operator;
            else
                user.Level = UserLevel.Monitor;

            _userAccess.setCurrentUser(user);
        }

        private bool IsSpacialUser(string gid)
        {
            bool returnResult = false;

            foreach (var item in CommonFunctions.Instance.SpacialUser)
            {
                if (gid.Trim().Contains(item.Trim()))
                {
                    returnResult = true;
                    break;
                }
            }

            return returnResult;
        }
        private void UserChanged(object sender, EventArgs e)
        {
            try
            {
                    UIUtility.Invoke(this, () =>
                    {
                        btnLogout.Enabled = (_userAccess.getCurrentUser() != _userAccess.DefaultUser);
                        txtUserName.Text = _userAccess.getCurrentUser().Name;
                        labelUserName.Text = _userAccess.getCurrentUser().EmpName;
                        UserLevel level = _userAccess.getCurrentUser().Level;

                        if (!HSTMachine.Workcell.CCCMachineTriggeringActivated && !HSTMachine.Workcell.Process.IsRunState)
                        {
                            HSTMachine.Instance.MainForm.getPanelNavigation().SetPanel("Operation");
                            UserControl activePanel = Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.GetActivePanel();
                            TabControl tabControl = activePanel.Controls[0] as TabControl;
                            tabControl.SelectedIndex = 0;
                        }

                        XyratexOSC.Logging.Log.Info(this, "User:{0} from user group:{1} has logged in.", _userAccess.getCurrentUser().Name, level.ToString());
                        switch (level)
                        {
                            case UserLevel.Monitor:
                                userIcon.Image = Properties.Resources.UserMonitor;
                                labelUserName.BackColor = Color.FromArgb(255, 255, 192);
                                break;
                            case UserLevel.Operator:
                                userIcon.Image = Properties.Resources.UserOperator;
                                labelUserName.BackColor = Color.FromArgb(255, 255, 192);
                                break;
                            case UserLevel.Engineer:
                                userIcon.Image = Properties.Resources.UserEngineer;
                                labelUserName.BackColor = Color.LimeGreen;
                                break;
                            case UserLevel.Administrator:
                                userIcon.Image = Properties.Resources.UserAdministrator;
                                labelUserName.BackColor = Color.LimeGreen;
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
                HSTSettings settings = sender as HSTSettings;

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
            labelUserName.BackColor = Color.FromArgb(255, 255, 192);
        }

        private void btnLogin_MouseClick(object sender, MouseEventArgs e)
        {
            ShowLogin();
        }

        private void HeaderUserAccess_Load(object sender, EventArgs e)
        {
            if (HSTSettings.Instance.Install.EnabledUserAccessControl)
            {
                UserChanged(this, new EventArgs());
            }
            else
            {
                panel1.Enabled = false;
                _userAccess.Users.Clear();

                UserInformation userinfo = new UserInformation();
                userinfo.EmpID = "00000";
                userinfo.EmpName = "Bypass";
                userinfo.UserName = "00000";

                User user = new User();
                user.Name = userinfo.UserName;
                user.EmpName = userinfo.EmpName;
                user.Level = UserLevel.Engineer;
                _userAccess.setCurrentUser(user);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
        }

        private void buttonBypass_Click(object sender, EventArgs e)
        {
            PopUpContextForm.Show(
            _userLoginControl,
            btnLogin,
            new Size(307, 105),
            new Point(-300, btnLogin.Height));
        }

    }
}
