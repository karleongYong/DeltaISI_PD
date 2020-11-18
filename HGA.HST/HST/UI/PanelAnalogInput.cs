using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelAnalogInput : UserControl
    {
        private A3200HC _a3200;
        private IAnalogInput _iAnalogInput;
        
        public PanelAnalogInput()
        {
            InitializeComponent();
        }

        public void AssignAxis(A3200HC a3200, IAnalogInput iAnalogInput)
        {
            if (DesignMode) return;

            _a3200 = a3200;
            _iAnalogInput = iAnalogInput;

            tmrUpdate.Enabled = true;
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                tmrUpdate.Enabled = false;
                return;
            }

            if (_a3200 != null)
            {
                try
                {
                    labelAnalogInputValue.Text = _a3200.GetRawValue(_iAnalogInput).ToString();
                    
                }
                catch (Exception ex)
                {
                    //DisplayError("Status update error: " + ex.Message);
                }
            }
        }

        private void labelAnalogInputName_VisibleChanged(object sender, EventArgs e)
        {
            if ((_a3200 != null) && !DesignMode)
                tmrUpdate.Enabled = Visible;
        }
    }
}
