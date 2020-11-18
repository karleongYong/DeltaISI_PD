using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchTestsTool.Settings;

namespace BenchTestsTool.Models
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
