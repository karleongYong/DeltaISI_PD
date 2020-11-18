using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class GemAlarm : IGemItem
    {
        private bool _set;
        private bool _enabled;
        private int _id;
        private AlarmCode _code;
        private string _name;
        private string _description;
        private GemEvent _setEvent;
        private GemEvent _clearEvent;

        public GemEvent ClearEvent
        {
            get
            {
                return _clearEvent;
            }
            private set
            {
                if (_clearEvent == value)
                    return;

                _clearEvent = value;

                OnPropertyChanged("ClearEvent");
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            private set
            {
                if (_description == value)
                    return;

                _description = value;

                OnPropertyChanged("Description");
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            private set
            {
                if (_enabled == value)
                    return;

                _enabled = value;

                OnPropertyChanged("Enabled");
            }
        }

        public int ID
        {
            get
            {
                return _id;
            }
            private set
            {
                if (_id == value)
                    return;

                _id = value;

                OnPropertyChanged("ID");
            }
        }

        public AlarmCode Code
        {
            get
            {
                return _code;
            }
            private set
            {
                if (_code == value)
                    return;

                _code = value;

                OnPropertyChanged("Code");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                if (_name == value)
                    return;

                _name = value;

                OnPropertyChanged("Name");
            }
        }

        public bool Set
        {
            get
            {
                return _set;
            }
            private set
            {
                if (_set == value)
                    return;

                _set = value;

                OnPropertyChanged("Set");
            }
        }

        public GemEvent SetEvent
        {
            get
            {
                return _setEvent;
            }
            private set
            {
                if (_setEvent == value)
                    return;

                _setEvent = value;

                OnPropertyChanged("SetEvent");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public GemAlarm(int id, string name, GemEvent setEvent, GemEvent clearEvent)
        {
            ID = id;
            Name = name;
            SetEvent = setEvent;
            ClearEvent = clearEvent;
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }
    }
}
