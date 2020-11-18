using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagate.AAS.HGA.HST.Models
{
    public class CSVEventArgs : EventArgs
    {
        public string CSVRecord
        {
            get;
            set;
        }

        public CSVEventArgs(string cSVRecord)
        {
            CSVRecord = cSVRecord;
        }
    }

    public class LoadRecipeEventArgs : EventArgs
    {
        public bool LoadStatus
        {
            get;
            set;
        }

        public LoadRecipeEventArgs(bool loadStatus)
        {
            LoadStatus = loadStatus;
        }
    }
}
