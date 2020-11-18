using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI.Operation.WorkOrder
{
    public partial class DycemCleaningCounter : UserControl
    {
        private HSTWorkcell _workcell;

        public DycemCleaningCounter()
        {
            InitializeComponent();
            if (HSTMachine.Workcell == null)
                return;
            _workcell = HSTMachine.Workcell;
        }

        public void DisplayDycemCleaningInformation(bool save)
        {
            if (HSTMachine.Workcell == null)
                return;

            this.BeginInvoke((MethodInvoker)delegate
            {
                txtSystemResetDate.Text = HSTMachine.Workcell.DycemCleaningCounter.StartDateTimeString;
                txtInputEECleanCount.Text = HSTMachine.Workcell.DycemCleaningCounter.InputEEDycemCleaningCountString;
                txtOutputEECleanCount.Text = HSTMachine.Workcell.DycemCleaningCounter.OutputEEDycemCleaningCountString;
                if (save)
                    HSTMachine.Workcell.DycemCleaningCounter.Save();
            });
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset the counter?", "Counter", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                HSTMachine.Workcell.DycemCleaningCounter.Reset();

                DisplayDycemCleaningInformation(true);
            }
        }

        private void DycemCleaningCounter_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            if (HSTMachine.Workcell == null)
                return;

            DisplayDycemCleaningInformation(false);
        }
    }
}
