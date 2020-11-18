// Setup.cs

using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Seagate.AAS.Utils
{
    public class SetupValue
    {
        private String      _name;
        private double      _value;
        private double      _lastValue;
        private int         _precision;
        private String      _lastChanged;
        private String      _user;

        private bool        _changed;
        private bool        _default;
       
        public SetupValue(String name, double val, int precision)
        {
            _name = name;
            _value = _lastValue = val;
            _precision = precision;           
            _user = "Unknown";
            _changed = false;
            _default = true;

            DateTime dt = new DateTime(2006, 1, 1, 0, 0, 0);
            _lastChanged = dt.ToString("G");
        }
         
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        } 

        public double Value
        {
            get { return _value; }
            set 
            { 
                if((value != _value) || _default)
                {
                    _default = false;
                    _changed = true;
                    _lastValue = _value;
                    _value = value; 
                }
            }
        } 

        public double LastValue
        {
            get { return _lastValue; }
            set { _lastValue = value; }
        } 

        public bool Default
        {
            get { return _default; }
        } 

        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        public String LastChanged
        {
            get { return _lastChanged; }
            set { _lastChanged = value; }
        }

        public String User
        {
            get { return _user; }
            set { _user = value; }
        }

        public void Load(Ini file)
        {
            bool notFound;
            
            _value   = file.GetValue(_name, "Value", _value, out notFound);
            _default = notFound ? true : file.GetValue(_name, "Default", _default, out notFound);
            _lastValue   = file.GetValue(_name, "LastValue", _lastValue, out notFound);
            _lastChanged = file.GetValue(_name, "Modified", _lastChanged, out notFound);
            _user        = file.GetValue(_name, "User", _user, out notFound);
        }

        public void Save(Ini file)
        {
            if(_changed)
            {
                String fmt = "F" + _precision.ToString();
                file.SetValue(_name, "Value", _value.ToString(fmt));
                file.SetValue(_name, "LastValue", _lastValue.ToString(fmt));
                file.SetValue(_name, "Default", _default.ToString());
                _lastChanged = DateTime.Now.ToString("G");
                file.SetValue(_name, "Modified", _lastChanged);
                file.SetValue(_name, "User", _user);
            }
        }
    }
}
