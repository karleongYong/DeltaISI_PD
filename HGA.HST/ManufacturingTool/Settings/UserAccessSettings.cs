using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BenchTestsTool.Utils;
using BenchTestsTool.Models;
using XyratexOSC.Utilities;

namespace BenchTestsTool.Settings
{
    public class UserAccessSettings
    {
        public event EventHandler UpdateCurrentUser;

        private User _adminUser = new User("Seagate", "SerembanDE", UserLevel.Administrator);        
        private User _defaultUser = new User("Monitor", "Monitor", UserLevel.Monitor);
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
            
            if (String.Equals(username, _adminUser.Name))
                user = _adminUser;
            else
                user = Users[username];

            if (user == null)
                throw new Exception("No user exists by this name");

            if (!String.Equals(user.Password, password))
                throw new Exception("User password is invalid");

            CurrentUser = user;

            UpdateCurrentUser.SafeInvoke(this, new CurrentUserEventArgs(CurrentUser));
        }

        public void LogOut()
        {
            CurrentUser = _defaultUser;
            //UpdateCurrentUser.SafeInvoke(this, new CurrentUserEventArgs(CurrentUser));
        }
    }
}
