using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// The opetions used for saving and loading a <see cref="SettingsDocument"/>.
    /// </summary>
    public enum SettingsFileOption
    {
        /// <summary>
        /// Uses the default file options [tab delimited]
        /// </summary>
        Default,
        /// <summary>
        /// A simple tab delimited file
        /// </summary>
        TabDelimited,
        /// <summary>
        /// An encrypted file
        /// </summary>
        Encrypted,
        /// <summary>
        /// A compressed tab-delimited file
        /// </summary>
        Compressed
    }
}
