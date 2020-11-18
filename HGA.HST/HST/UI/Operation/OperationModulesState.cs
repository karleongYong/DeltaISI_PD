using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.UI.Operation.WorkOrder;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using System.Threading;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.Parsel.Equipment;

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class OperationModulesState : UserControl
    {

        public HSTWorkcell _workcell;

        public OperationModulesState()
        {
            InitializeComponent();
            if (HSTMachine.Workcell == null) return;

            _workcell = HSTMachine.Workcell;
            HSTMachine.Workcell.Process.OnInitStart += new EventHandler(Process_OnInitStart);
            HSTMachine.Workcell.Process.OnInit += new EventHandler(Process_OnInit);
                   
        }
        

        private void OperationModulesState_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            timer1.Enabled = this.Visible;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = this.Visible;
            if (!this.Visible)
            {
                return;
            }

           UpdateProcessTimer(HSTMachine.Workcell.HSTSettings.getConfigPerformance().TotalTime,
           HSTMachine.Workcell.HSTSettings.getConfigPerformance().RunTime,
           HSTMachine.Workcell.HSTSettings.getConfigPerformance().IdleTime,
           HSTMachine.Workcell.HSTSettings.getConfigPerformance().DownTime);                     
        }

        private void buttonQuitApp_Click(object sender, EventArgs e)
        {            
            HSTMachine.Instance.MainForm.Close();
            HSTHwSystem.Instance.GetHwComponent((int)Seagate.AAS.HGA.HST.Machine.HSTHwSystem.HardwareComponent.VisionSystem).ShutDown();
        }
        private void labelInputTurnStation_Click(object sender, EventArgs e)
        {

        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            try
            {
                HSTMachine.Workcell.Process.InitializeMachine(false);
            }
            catch (Exception ex)
            {
                Notify.PopUpError("Failed to initializa state machine", ex);                
            }
        }

        void Process_OnInit(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.buttonInit.BackColor = Color.FromKnownColor(KnownColor.Control);
            });

        }

        void Process_OnInitStart(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.buttonInit.BackColor = Color.LightYellow;
            });
        }

        private void OperationModulesState_Load(object sender, EventArgs e)
        {

        }

        public void UpdateProcessTimer(TimeSpan unloadTotalTime, TimeSpan unloadRunTime, TimeSpan unloadStandbyTime, TimeSpan unloadDownTime)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                textUnloadTotalTime.Text = string.Format("{0:00}:{1:00}:{2:00}",
              (int)unloadTotalTime.TotalHours,
                   unloadTotalTime.Minutes,
                   unloadTotalTime.Seconds);
                textUnloadRunTime.Text = string.Format("{0:00}:{1:00}:{2:00}",
               (int)unloadRunTime.TotalHours,
                    unloadRunTime.Minutes,
                    unloadRunTime.Seconds);
                textUnloadStandbyTime.Text = string.Format("{0:00}:{1:00}:{2:00}",
                (int)unloadStandbyTime.TotalHours,
                     unloadStandbyTime.Minutes,
                     unloadStandbyTime.Seconds);
                textUnloadDownTime.Text = string.Format("{0:00}:{1:00}:{2:00}",
                 (int)unloadDownTime.TotalHours,
                      unloadDownTime.Minutes,
                      unloadDownTime.Seconds);
            });
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (ParselMessageBox.Show("Counter", "Do you want to reset the counter?", MessageBoxIcon.Question, ErrorButton.NoButton, ErrorButton.Yes, ErrorButton.No) == ErrorButton.Yes)
            {
                HSTMachine.Workcell.HSTSettings.getConfigPerformance().StopTracking();
                HSTMachine.Workcell.HSTSettings.getConfigPerformance().StartTracking();
            }
        }

        private void groupMotionModule_Enter(object sender, EventArgs e)
        {

        }

        public DycemCleaningCounter getDycemCleaningCounterUserControl()
        {
            return dycemCleaningCounter1;
        }
    }
}







//
//int tmrCtr = 0;
//Timer mTimer;

//private void ResetTimer()
//
// tmrCtr = 0;
//}

//private void Timer1_Tick()
//
//  tmrCtr++;
//perform task
//}  