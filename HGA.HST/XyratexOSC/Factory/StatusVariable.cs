using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Represents a Status Variable (read-only).
    /// </summary>
    public class StatusVariable<T> : GemVariable<T>
        where T : ISecsValue
    {
        private static readonly string _class = "SV";

        public new bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        public StatusVariable(int sid, string name, string format, int length, string description)
        {
            ID = sid;
            Name = name;
            Format = format;
            Length = length;
            Description = description;
        }

        public override string GetClass()
        {
            return _class;
        }
    }
}
