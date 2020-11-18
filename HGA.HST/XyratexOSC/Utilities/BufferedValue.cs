using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using XyratexOSC.Utilities;
using XyratexOSC.UI;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// Public interface for generic BufferedValue class.
    /// </summary>
    public interface IBufferedValue
    {
        /// <summary>
        /// Readonly accessor for the Name of the BufferedValue variable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        object Value { get; set; }
    }

    /// <summary>
    /// Public generic interface for generic BufferedValue class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBufferedValue<T>:IBufferedValue
    {
        /// <summary>
        /// Accessor for generic Value object
        /// </summary>
        new T Value { get; set; }

        /// <summary>
        /// EventHandler for when the value variable is updated.
        /// </summary>
        event EventHandler<ValueChangedArg<T>> ValueChanged;
    }

    /// <summary>
    /// A generic BufferedValue class to be used for settings variables
    /// to create a binding when values are updated.
    /// This is a thread safe class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BufferedValue<T> : IBufferedValue<T>
    {
        /// <summary>
        /// EventHandler for when the value variable is updated.
        /// </summary>
        public event EventHandler<ValueChangedArg<T>> ValueChanged;
        readonly string _name;
        private T _settingVariable;
        private object _lock = new object();

        /// <summary>
        /// A generic BufferedValue class to be used for settings variables
        /// to create a binding when values are updated.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settingVariable"></param>
        public BufferedValue(string name, T settingVariable)
        {
            _name = name;
            _settingVariable = settingVariable;
        }

        /// <summary>
        /// Readonly accessor for the Name of the BufferedValue variable.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Read/Write accessor for generic value object T.
        /// </summary>
        public T Value
        {
            get
            {
                return _settingVariable;
            }

            set
            {
                lock (_lock)
                {
                    SetValue(value);
                }
            }
        }

        object IBufferedValue.Value
        {
            get { return Value; }
            set
            {
                value = ChangeType(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ChangeType(object obj)
        {
            if(obj == null || typeof(T).IsAssignableFrom(obj.GetType()))
            {
                return (T)obj;
            }

            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return (T)Activator.CreateInstance(typeof(T), obj);
            }
        }

        private void SetValue(T newValue)
        {
            T oldValue = _settingVariable;
            _settingVariable = newValue;
            ValueChanged.SafeInvoke(this, new ValueChangedArg<T>(_name, oldValue, _settingVariable));
        }

        private T GetValue()
        {
            return _settingVariable;
        }

        /// <summary>
        /// Returns an string with variable name and current value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}, {1})", Name, Value);
        }
    }

    /// <summary>
    /// Event arguments used with the ValueChanged EventHandler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueChangedArg<T> : EventArgs
    {
        readonly private string _name;
        readonly private T _oldValue;
        readonly private T _newValue;

        /// <summary>
        /// Readonly accessor for the variable that represents Name of the object.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Readonly accessor for the variable that represents value before it was updated.
        /// </summary>
        public T OldValue
        {
            get
            {
                return _oldValue;
            }
        }

        /// <summary>
        /// Readonly accessor for the variable that represent the new value.
        /// </summary>
        public T NewValue
        {
            get
            {
                return _newValue;
            }
        }

        /// <summary>
        /// Constructor for the ValueChangedArg.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public ValueChangedArg(string name, T oldValue, T newValue)
        {
            _name = name;
            _oldValue = oldValue;
            _newValue = newValue;
        }
    }
}
