using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.UI.Operation;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;

namespace Seagate.AAS.HGA.HST.UI.Main
{
    public partial class PanelOperation : UserControl
    {
        HSTWorkcell _workcell;

        public PanelOperation(HSTWorkcell workcell)
        {
            InitializeComponent();
            _workcell = workcell;
        }        

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:  // tabPageMain

                    if (HSTMachine.Instance.MainForm != null)
                    {
                        Control tabModuleStatePage = tabControl1.TabPages[1] as Control;
                        Control tabStatusPage = tabControl1.TabPages[2] as Control;                                                                                

                        if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Operator ||
                            HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Monitor)
                        {
                            operationMainPanel.getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                            operationMainPanel.getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                            operationMainPanel.getMeasurementTestUserControl().cboTabType.Enabled = false;
                            HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().DisableAllButtons();
                        }
                        else
                        {
                            operationMainPanel.getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = (HSTMachine.Workcell.Process.IsRunState) ? false : true;
                            operationMainPanel.getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = (HSTMachine.Workcell.Process.IsRunState) ? false : true;
                            operationMainPanel.getMeasurementTestUserControl().cboTabType.Enabled = (HSTMachine.Workcell.Process.IsRunState) ? false : true;
                            HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().EnableAllButtons();
                        }
                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().SetAllTimerOnOff(false);
                    }                    
                    break;
                case 1: // tabPageModuleState
                    HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().SetAllTimerOnOff(false);
                    break;
                case 2: // tabPageStatus
                    if (HSTMachine.Instance.MainForm != null)
                    {                        
                        Control tabModuleStatePage = tabControl1.TabPages[1] as Control;
                        Control tabStatusPage = tabControl1.TabPages[2] as Control;
                        if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Operator ||
                            HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level == Seagate.AAS.HGA.HST.Settings.UserLevel.Monitor)
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().DisableAllButtons();
                        }
                        else
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().EnableAllButtons();
                        }

                        HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().SetAllTimerOnOff(true);
                    }
                    break;
            }            
        }

        public OperationMainPanel getOperationMainPanel()
        {
            return operationMainPanel;
        }

        public OperationModulesState getOperationModuleStatePanel()
        {
            return operationModuleState;
        }

        public OperationStatus getOperationStatusPanel()
        {
            return operationStatus;
        }
    }
}
