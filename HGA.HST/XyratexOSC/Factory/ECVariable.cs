using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Represents an Equipment Constant Variable properties.
    /// </summary>
    public class ECVariable<T> : GemVariable<T>
        where T : ISecsValue
    {
        private static readonly string _class = "EC";

        private T _min;
        private T _max;

        /// <summary>
        /// Gets or sets the minimum value that will trigger an alarm if facilities are listening.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public T Minimum
        {
            get
            {
                return _min;
            }
            protected set
            {
                _min = value;

                OnPropertyChanged("Min");
            }
        }

        /// <summary>
        /// Gets or sets the maximum value that will trigger an alarm if facilities are listening.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public T Maximum
        {
            get
            {
                return _max;
            }
            protected set
            {
                _max = value;

                OnPropertyChanged("Max");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this variable is read-only, or writable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read-only; otherwise, <c>false</c>.
        /// </value>
        public new bool ReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ECVariable{T}"/> class.
        /// </summary>
        /// <param name="ecid">The equipment constant variable id.</param>
        /// <param name="name">The variable name.</param>
        /// <param name="format">The data format.</param>
        /// <param name="length">The length.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="description">The user-friendly description.</param>
        public ECVariable(int ecid, string name, string format, int length, T defaultValue, T min, T max, string description)
        {
            ID = ecid;
            Name = name;
            Format = format;
            Length = length;
            Minimum = min;
            Maximum = max;
            Data = defaultValue;
            Description = description;
        }

        /// <summary>
        /// Gets the class type.
        /// </summary>
        /// <returns>The class type of this variable</returns>
        public override string GetClass()
        {
            return _class;
        }
    }
}
