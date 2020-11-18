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
    public partial class ProductionCounter : UserControl
    {       
        private HSTWorkcell _workcell;

        public ProductionCounter()
        {
            InitializeComponent();
            if (HSTMachine.Workcell == null)
                return;
            _workcell = HSTMachine.Workcell;           
        }

        public void DisplayLoadInformation(bool save)
        {
            if (HSTMachine.Workcell == null)
                return;

            this.BeginInvoke((MethodInvoker)delegate
            {
                txtSystemResetDate.Text = HSTMachine.Workcell.LoadCounter.StartDateTimeString;
                txtSystemUPH.Text = HSTMachine.Workcell.LoadCounter.UPHString;
                txtSystemUPH2.Text = HSTMachine.Workcell.LoadCounter.UPH2String;
                txtSystemCycleTime.Text = HSTMachine.Workcell.LoadCounter.CycleTimeString;
                txtProcessedHGACount.Text = HSTMachine.Workcell.LoadCounter.ProcessedHGACountString;
                double totalPercentage = (((double)HSTMachine.Workcell.LoadCounter.SamplingPerDayCounter /
                        (double)HSTMachine.Workcell.LoadCounter.ProcessedHGACount) * 100);
                if (double.IsNaN(totalPercentage) || double.IsInfinity(totalPercentage)) totalPercentage = 0;
                txtSamplingCounter.Text = totalPercentage.ToString("F2");
                textBoxWRBrigeFailure.Text = HSTMachine.Workcell.LoadCounter.LastWRBridgePercentage.ToString();
                if (save)
                    HSTMachine.Workcell.LoadCounter.Save();
            });
        }
        
        private void btReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset the counter?", "Counter", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                HSTMachine.Workcell.LoadCounter.Reset();
                HSTMachine.Workcell.LoadCounter.ResetWriterBridgCounter();
                DisplayLoadInformation(true);               
            }
        }

        private void ProductionCounter_Load(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell == null)
                return;
            DisplayLoadInformation(false);
        }
    }
}
