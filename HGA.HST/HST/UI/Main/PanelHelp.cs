using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelHelp : UserControl
    {
        public PanelHelp(HSTWorkcell workcell)
        {
            InitializeComponent();
        }

        private void PanelHelp_Layout(object sender, LayoutEventArgs e)
        {

            if (System.IO.File.Exists(@"C:\Seagate\\HGA.HST\\Documents\\help files\\HSTHelp.chm"))
                System.Windows.Forms.Help.ShowHelp(this, "file://C://Seagate//HGA.HST//Documents//help files//HSTHelp.chm");
              //  System.Windows.Forms.Help.ShowHelp(this, "file://d:\\help files\\HSTHelp.chm");
        }
    }
}
