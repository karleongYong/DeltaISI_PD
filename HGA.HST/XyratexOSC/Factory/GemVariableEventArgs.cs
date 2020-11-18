using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class GemVariableEventArgs : EventArgs
    {
        public GemVariable Variable
        {
            get;
            set;
        }

        public int VID
        {
            get;
            set;
        }

        public ISecsValue Value
        {
            get;
            set;
        }

        public GemVariableEventArgs(GemVariable var)
        {
            Variable = var;
            VID = var.ID;
            Value = var.Data as ISecsValue;
        }

        public GemVariableEventArgs(int vid, ISecsValue value)
        {
            VID = vid;
            Value = value;
        }
    }
}
