using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class CogDisplayPanel : UserControl
    {
        public CogDisplayPanel(Cognex.VisionPro.ICogAcqFifo myFifo)
        {
            InitializeComponent();
            this.cogDisplay1.StartLiveDisplay(myFifo);

        }

        public void StopCogDisplayPanel()
        {
            this.cogDisplay1.StopLiveDisplay();

        }
    }
}
