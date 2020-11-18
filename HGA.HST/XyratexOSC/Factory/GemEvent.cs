using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class GemEvent : IGemItem
    {
        private int _id;
        private string _name;
        private string _description;
        private IList<GemReport> _linkedReports = new List<GemReport>();

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

        public IList<GemReport> LinkedReports
        {
            get
            {
                return _linkedReports;
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

        public bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public GemEvent(GemEventId ceid, string name, string description, params GemReport[] linkedReports)
            : this((int)ceid, name, description, linkedReports)
        {
        }

        public GemEvent(int ceid, string name, string description, params GemReport[] linkedReports)
        {
            ID = ceid;
            Name = name;
            Description = description;

            foreach (GemReport report in linkedReports)
                LinkedReports.Add(report);
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }
    }
}
