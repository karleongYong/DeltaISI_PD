using Seagate.AAS.HGA.HST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using XyratexOSC.UI;
using XyratexOSC.XMath;
using Seagate.AAS.HGA.HST.Utils;


namespace Seagate.AAS.HGA.HST.Settings
{
    public class SimulatedPartSettings
    {    
        [DisplayName("Carriers")]
        [Category("Simulated Carriers (for Dry Run and Simulation mode)")]
        [Editor(typeof(NamedCollectionEditor<CarrierSettings>), typeof(UITypeEditor))]
        public NamedCollection<CarrierSettings> Carriers
        {
            get;
            set;
        }

        public SimulatedPartSettings()
        {
            Carriers = new NamedCollection<CarrierSettings>();
        }
        
    }  
}
