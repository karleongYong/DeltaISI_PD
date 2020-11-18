using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Utils
{
    public static class Formatting
    {
        public static double RestrictDecimalPlaces(double originalValue, int decimalPlaces)
        {
            return Math.Round(originalValue, decimalPlaces);
        }               

    }
}
