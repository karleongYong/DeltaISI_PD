using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopTester.Settings;

namespace DesktopTester.Models
{
    public class CurrentUserEventArgs : EventArgs
    {
        public User CurrentUser
        {
            get;
            set;
        }

        public CurrentUserEventArgs(User currentUser)
        {            
            CurrentUser = currentUser;            
        }
    }
}
