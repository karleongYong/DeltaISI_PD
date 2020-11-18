using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.HGA.HST.Settings;

namespace Seagate.AAS.HGA.HST.Models
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
