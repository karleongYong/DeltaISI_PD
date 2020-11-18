using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Data
{
    public class ImageManaging
    {
        public DateTime LastDeletedDate;
        public bool AllFilesAccepted;
        public List<string> ImageDeletedList = new List<string>();
    }
}
