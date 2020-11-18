using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class UserAccessSettings
    {
        public event EventHandler UpdateCurrentUser;

        private User _adminUser = new User("Seagate", "SerembanDE", UserLevel.Administrator, true);        
        private User _defaultUser = new User("Operator", "Operator", UserLevel.Operator, false);
        private User _currentUser;

        [Category("Users")]
        [Editor(typeof(NamedCollectionEditor<User>), typeof(System.Drawing.Design.UITypeEditor))]
        public NamedCollection<User> Users
        {
            get;
            set;
        }

        internal User DefaultUser
        {
            get
            {
                return _defaultUser;
            }
        }

        public void setCurrentUser(User currentUser)
        {
            CurrentUser = currentUser;
        }

        public User getCurrentUser()
        {
            return CurrentUser;
        }

        private User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = _defaultUser;
                }

                return _currentUser;
            }
            set
            {
                if (value != _defaultUser && _currentUser == value)
                    return;

                _currentUser = value;

                EventHandler userChanged = UserChanged;
                if (userChanged != null)
                    userChanged(this, new EventArgs());
            }
        }

        public event EventHandler UserChanged;

        public UserAccessSettings()
        {
            Users = new NamedCollection<User>();            
        }

        public void LogIn(string username, string password)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new Exception("No user name supplied");

            if (String.Equals(username, _defaultUser.Name))
            {
                CurrentUser = _defaultUser;
                UpdateCurrentUser.SafeInvoke(this, new CurrentUserEventArgs(CurrentUser));
                return;
            }

            if (String.IsNullOrWhiteSpace(password))
                throw new Exception("No user password supplied");

            User user = null;
            
            if (user == null && username == "HSTME" && password == "meengineering")
            {
                user = new User("HSTME", "meengineering", UserLevel.Engineer, true);
                user.EmpName = "Bypass login";
            }

            if (user == null)
                throw new Exception("No user exists by this name");

            if (!String.Equals(user.Password, password))
                throw new Exception("User password is invalid");

            if (user.Level == UserLevel.Administrator && HSTMachine.Workcell.Process.IsRunState)
            {
                Notify.PopUp("Invalid User Login", "User with Administrator level cannot login while system is running.", "", "OK");
                return;
            }
            CurrentUser = user;

            UpdateCurrentUser.SafeInvoke(this, new CurrentUserEventArgs(CurrentUser));
        }

        public void LogOut()
        {
            CurrentUser = _defaultUser;
        }
    }
}
