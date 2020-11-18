using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class GemReport : List<GemVariable>, IGemItem
    {
        private int _id;
        private string _name;
        private string _description;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public GemReport(int id, string name)
        {
            ID = id;
        }

        public GemReport(int id, string name, params GemVariable[] vars)
            : this(id, name)
        {
            this.AddRange(vars);
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }
    }
}
