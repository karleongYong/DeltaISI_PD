using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs/Gem Alarm Codes
    /// </summary>
    public enum AlarmCode
    {
        /// <summary>Personal Safety Alarm. Highest Severity.</summary>
        PersonalSafety = 1,

        /// <summary>Equipment Safety Alarm. High importance as the equipment health could be in danger.</summary>
        EquipmentSafety = 2,

        /// <summary>Parameter that has been specified to watch, has fallen within the warning range.</summary>
        ParamControlWarn = 3,

        /// <summary>Parameter that has been specified to watch, has fallen within the error range.</summary>
        ParamControlError = 4,

        /// <summary>Equipment error has occurred that cannot be recovered automatically.</summary>
        IrrecoverableError = 5,

        /// <summary>Generic equipment status warning.</summary>
        EquipmentStatusWarn = 6,

        /// <summary>Equipment requires attention.</summary>
        AttentionFlags = 7,

        /// <summary>Data integrity alarm.</summary>
        DataIntegrity = 8
    }
}
