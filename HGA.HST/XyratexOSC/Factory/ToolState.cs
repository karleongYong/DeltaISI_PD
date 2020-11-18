using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyratexOSC.Factory
{
    public enum ToolState
    {
        /// <summary>
        /// Disabled. Tool has no connection to Host.
        /// </summary>
        Disabled,

        /// <summary>
        /// Offline. Tool will not attempt to connect to Host.
        /// </summary>
        Offline,

        /// <summary>
        /// Offline. Tool will attempt to connect to Host.
        /// </summary>
        OfflineAttemptOnline,

        /// <summary>
        /// Online. Local Control.
        /// </summary>
        OnlineLocal,

        /// <summary>
        /// Online. Host (Remote) Control.
        /// </summary>
        OnlineRemote
    }
}
