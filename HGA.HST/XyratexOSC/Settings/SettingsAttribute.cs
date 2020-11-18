using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// Attribute for defining the access level of a settings property
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsAttribute : System.Attribute
    {
        /// <summary>
        /// The default access level
        /// </summary>
        public const int DefaultAccessLevel = 2;

        /// <summary>
        /// Gets the access level.
        /// </summary>
        /// <value>
        /// The access level.
        /// </value>
        public int AccessLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAttribute"/> class.
        /// </summary>
        /// <param name="requiredAccessLevel">The required access level.</param>
        public SettingsAttribute(int requiredAccessLevel = DefaultAccessLevel)
        {
            AccessLevel = requiredAccessLevel;
        }
    }
}
