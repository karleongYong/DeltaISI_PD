using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Models
{
    public class TestFirmwareEventArgs : EventArgs
    {
        public string TestStatus
        {
            get;
            set;
        }

        public TestFirmwareEventArgs(string testStatus)
        {
            TestStatus = testStatus;
        }
    }
}
