using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XyratexOSC.Settings;

namespace Seagate.AAS.HGA.HST.Settings
{
    public static class SettingsHelper
    {
        /// <summary>
        /// Return the value from the node that matches nodelabel or return the defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="nodeLabel"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T UpdateFromNode<T>(SettingsNode node, string nodeLabel, T defaultValue)
        {
            if (node.ExistsAndHasAValue<T>(nodeLabel))
                return node[nodeLabel].GetValueAs<T>();
            else
                return defaultValue;
        }
    }
}
