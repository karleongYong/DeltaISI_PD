using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs II Localized String. A series of Unicode characters.
    /// </summary>
    public sealed class SecsString
    {
        private readonly String _value;

        public SecsString(String value)
        {
            _value = value;
        }

        public static implicit operator SecsString(String value)
        {
            if (value == null)
                return null;

            return new SecsString(value);
        }

        public int CompareTo(object obj)
        {
            return _value.CompareTo(obj);
        }
    }
}
