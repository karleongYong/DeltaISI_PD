using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// This class provides a semi-numeric IComparer implemention for strings. 
    /// It will sort by numerics if possible to convert otherwise alphabetically.
    /// </summary>
    public class AlphaNumericComparer : IComparer<string>
    {
        /// <summary>
        /// Compares the specified string 1 to string 2.
        /// </summary>
        /// <param name="s1">String 1.</param>
        /// <param name="s2">String 2.</param>
        /// <returns></returns>
        public int Compare(string s1, string s2)
        {
            if (IsNumeric(s1) && IsNumeric(s2))
            {
                if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
            }

            if (IsNumeric(s1) && !IsNumeric(s2))
                return -1;

            if (!IsNumeric(s1) && IsNumeric(s2))
                return 1;

            return string.Compare(s1, s2, true);
        }

        /// <summary>
        /// Determines whether the specified value can be converted to a numeric.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(string value)
        {
            int i;
            return Int32.TryParse(value, out i);
        }
    }
}
