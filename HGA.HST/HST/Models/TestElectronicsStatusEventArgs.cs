using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Models
{
    public class TestElectronicsStatusEventArgs : EventArgs
    {
        public string TestElectronicsStatus
        {
            get;
            set;
        }

        public TestElectronicsStatusEventArgs(string testElectronicsStatus)
        {
            TestElectronicsStatus = testElectronicsStatus;
        }
    }
}
