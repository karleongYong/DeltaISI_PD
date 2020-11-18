using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Specifies the on/off states of all the lights in the lightstack.
    /// </summary>
    [FlagsAttribute]
    public enum LightStackColor
    {
        /// <summary>Light Stack: Off</summary>
        Off = 0,
        /// <summary>Light Stack: Green</summary>
        Green = 2,
        /// <summary>Light Stack: Yellow</summary>
        Yellow = 4,
        /// <summary>Light Stack: Red</summary>
        Red = 8,
        /// <summary>Light Stack: Blue</summary>
        Blue = 16,
        /// <summary>Light Stack: All</summary>
        All = Green | Yellow | Red | Blue
    }
}
