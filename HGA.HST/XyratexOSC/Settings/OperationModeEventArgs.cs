using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.Settings
{
    public class OperationModeEventArgs : EventArgs
    {
        public string strOperationMode
        {
            get;
            set;
        }

        public OperationModeEventArgs(string operationMode)
        {
            strOperationMode = operationMode;
        }
    }
}
