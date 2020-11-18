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
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelDigitalOutput : UserControl
    {
        private A3200HC _a3200;
        private IAxis _axis;
        private int _card;
        private int _bit;
        private IDigitalOutput _do = null;
        public const int NumberOfIOForXAxis = 24;
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
                    _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(_card == 0 ? _bit : (_card - 1) * 8 + NumberOfIOForXAxis + _bit);
                    ledOutput.State = HSTMachine.Workcell._a3200HC.GetState(_do) == DigitalIOState.On ? true : false;
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
                        HSTMachine.Workcell._a3200HC.SetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput(_card == 0 ? _bit : (_card - 1) * 8 + NumberOfIOForXAxis + _bit), DigitalIOState.Off);
                    }
                    else
                    {
                        HSTMachine.Workcell._a3200HC.SetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput(_card == 0 ? _bit : (_card - 1) * 8 + NumberOfIOForXAxis + _bit), DigitalIOState.On);
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
