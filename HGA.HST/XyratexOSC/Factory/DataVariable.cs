using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Represents a Data Variable (read-only and volatile). These variables are associated with events.
    /// </summary>
    public class DataVariable<T> : GemVariable<T>
        where T : ISecsValue
    {
        private static readonly string _class = "DV";

        /// <summary>
        /// Gets a value indicating whether this variable is read only, or writable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read-only; otherwise, <c>false</c>.
        /// </value>
        public new bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataVariable{T}"/> class.
        /// </summary>
        /// <param name="dvid">The data variable id.</param>
        /// <param name="name">The variable name.</param>
        /// <param name="format">The variable format.</param>
        /// <param name="length">The length of the variable data.</param>
        /// <param name="description">The user-friendly description.</param>
        public DataVariable(int dvid, string name, string format, int length, string description)
        {
            ID = dvid;
            Name = name;
            Format = format;
            Length = length;
            Description = description;
        }

        /// <summary>
        /// Gets the variable class metadata.
        /// </summary>
        /// <returns>The class type.</returns>
        public override string GetClass()
        {
            return _class;
        }
    }
}
