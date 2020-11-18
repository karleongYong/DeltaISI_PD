using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Settings
{
    public interface INamed
    {
        [DisplayName("(Name)")]
        string Name
        {
            get;
            set;
        }
    }
}
