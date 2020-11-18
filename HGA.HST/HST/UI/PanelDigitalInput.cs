using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelDigitalInput : UserControl
    {
        private A3200HC _a3200;
        private int _card;
        private int _bit;
        private IDigitalInput _di = null;
        public const int NumberOfIOForXAxis = 24;

        public PanelDigitalInput()
        {
            InitializeComponent();
        }

        public void AssignAxis(A3200HC a3200, int card, int bit)
        {
            if (DesignMode) return;

            _a3200 = a3200;
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
                    _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(_card == 0 ? _bit : (_card-1)*8+NumberOfIOForXAxis+_bit);
                    ledInput.State = HSTMachine.Workcell._a3200HC.GetState(_di) == DigitalIOState.On ? true : false;
                }
                catch (Exception ex)
                {
                    //DisplayError("Status update error: " + ex.Message);
                }
            }
        }

        private void ledInput_VisibleChanged(object sender, EventArgs e)
        {
            if ((_a3200 != null) && !DesignMode)
                tmrUpdate.Enabled = Visible;
        }
    }
}
