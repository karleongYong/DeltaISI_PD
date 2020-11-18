using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public partial class PanelDigitalOutput : UserControl
    {
        private A3200HC _a3200;
        private IAxis _axis;
        private int _card;
        private int _bit;

        public PanelDigitalOutput()
        {
            InitializeComponent();
        }

        public void AssignAxis(A3200HC a3200, IAxis axis, int card, int bit)
        {
            if (DesignMode) return;

            _a3200 = a3200;
            _axis = axis;
            _card = card;
            _bit = bit;

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
                    ledOutput.State = _a3200.GetDigitalOutputBit(_axis, _bit);
                }
                catch (Exception ex)
                {
                    //DisplayError("Status update error: " + ex.Message);
                }
            }
        }

        private void ledOutput_VisibleChanged(object sender, EventArgs e)
        {
            if ((_a3200 != null) && !DesignMode)
                tmrUpdate.Enabled = Visible;
        }

        private void ledOutput_Click(object sender, EventArgs e)
        {
            if (_a3200 != null)
            {
                try
                {
                    if (ledOutput.State)
                    {
                        _a3200.SetDigitalOutputBit(_card, _bit, false);
                        //DisplayStatus("Disabled");
                    }
                    else
                    {
                        _a3200.SetDigitalOutputBit(_card, _bit, true);
                        //DisplayStatus("Enabled");
                    }
                }
                catch (Exception ex)
                {
                    //DisplayError("Enable error: " + ex.Message);
                }
            }
        }
    }
}
