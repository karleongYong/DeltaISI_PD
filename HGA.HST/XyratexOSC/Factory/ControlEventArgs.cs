using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.Factory
{
    public class HostControlEventArgs : EventArgs
    {
        public HostState State
        {
            get;
            set;
        }

        public HostControlEventArgs(HostState state)
        {
            State = state;
        }
    }

    public class ToolControlEventArgs : EventArgs
    {
        public ToolState State
        {
            get;
            set;
        }

        public ToolControlEventArgs(ToolState state)
        {
            State = state;
        }
    }
}
