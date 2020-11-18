using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aerotech.A3200.Tasks;
using Seagate.AAS.UI;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public partial class PanelTask : EmbeddableUserControl
    {
        Task task;
        public PanelTask()
        {
            InitializeComponent();
        }

        public void AssignTask(Task task)
        {
            if (DesignMode) return;

            this.task = task;

            groupBox1.Text = task.Name.ToString();
            tmrUpdate.Enabled = true;
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                tmrUpdate.Stop();
                return;
            }

            if (task != null)
            {
                try
                {
                    lbState.Text = task.State.ToString();
                    lbStatus.Text = task.Status.ToString();
                    lbLoadedProgram.Text = task.Program.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnRunProgram_Click(object sender, EventArgs e)
        {
            if (task != null)
                task.Program.Start();
        }

        private void btnStopProgram_Click(object sender, EventArgs e)
        {
            if (task != null)
                task.Program.Stop();
        }

        private void PanelTask_VisibleChanged(object sender, EventArgs e)
        {
            if ((task != null) && !DesignMode)
                tmrUpdate.Enabled = Visible;
        }
    }
}
