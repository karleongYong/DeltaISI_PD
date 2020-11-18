using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public abstract class GemVariable : IGemItem
    {
        protected object _data;
        private int _id;
        private string _name;
        private string _description;
        private string _units;

        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;

                OnPropertyChanged("Data");
            }
        }

        public string Format
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return _description;
            }
            protected set
            {
                if (_description == value)
                    return;

                _description = value;

                OnPropertyChanged("Description");
            }
        }

        public string Units
        {
            get
            {
                return _units;
            }
            protected set
            {
                if (_units == value)
                    return;

                _units = value;

                OnPropertyChanged("Units");
            }
        }

        public int ID
        {
            get
            {
                return _id;
            }
            protected set
            {
                if (_id == value)
                    return;

                _id = value;

                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                if (_name == value)
                    return;

                _name = value;

                OnPropertyChanged("Name");
            }
        }

        public bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }

        public abstract string GetClass();
    }

    public abstract class GemVariable<T> : GemVariable 
        where T : ISecsValue
    {
        public new T Data
        {
            get
            {
                return (T)_data;
            }
            set
            {
                _data = value;

                OnPropertyChanged("Data");
            }
        }
    }
}
