using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public interface IGemItem : INotifyPropertyChanged, IDisposable
    {
        int ID
        {
            get;
        }

        string Name
        {
            get;
        }

        string Description
        {
            get;
        }
    }
}
