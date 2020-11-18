using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// Defines methods that will be used by the Settings Converter to convert an object-to-settingsnode, and settingsnode-to-object
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Updates an object from a settings node.
        /// </summary>
        /// <param name="node">The settings node.</param>
        void UpdateFromSettingsNode(SettingsNode node);

        /// <summary>
        /// Converts an object to settings node.
        /// </summary>
        /// <returns></returns>
        SettingsNode ConvertToSettingsNode();
    }
}
