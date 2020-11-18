using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelDiagnostics : UserControl
    {
        public PanelDiagnostics(HSTWorkcell workcell)
        {
            InitializeComponent();
            _workcell = workcell;            
        }

        public Diagnostics.MotionControlPanel getMotionControllerPanel()
        {
            return MCPanel;
        }

        public Diagnostics.SelfCheckPanel getSelfCheckPanel()
        {
            return SCPanel;
        }

        HSTWorkcell _workcell;
    }    
}
