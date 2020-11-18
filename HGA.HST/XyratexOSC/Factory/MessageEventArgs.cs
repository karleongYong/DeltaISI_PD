using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class MessageEventArgs : EventArgs
    {
        public string Message
        {
            get;
            set;
        }

        public MessageEventArgs(string msg)
        {
            Message = msg;
        }
    }
}
