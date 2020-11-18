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
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using System.Threading;
using XyratexOSC.Utilities;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Recipe;
using System.Xml;
using Seagate.AAS.HGA.HST.Settings;

namespace Seagate.AAS.HGA.HST.UI.Operation
{
    public partial class OperationStatus : UserControl
    {
        public HSTWorkcell _workcell;
        public const int AdditionalSetOfIOForXAxis = 2;
        TeachPointRecipe _teachPointRecipe;        
        XmlDocument _doc;
        string _fullPathName = string.Empty;
        string _fileNameWithExt = string.Empty;
        bool _recipeLoaded = false;

        public OperationStatus()
        {
            InitializeComponent();
            if (HSTMachine.Workcell == null) return;

            _workcell = HSTMachine.Workcell;
            
            //Seaveyor
            //input
            //in position
            panelDigitalInputInputTurnTableInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_In_Position < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 : //id below 24 is card 0
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_In_Position / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_In_Position % 8);

            panelDigitalInputInputStationInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 
                (int)HSTIOManifest.DigitalInputs.BIS_Position_On < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 : 
                (int)HSTIOManifest.DigitalInputs.BIS_Position_On / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.BIS_Position_On % 8);
            
            panelDigitalInputBufferStationInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 
                (int)HSTIOManifest.DigitalInputs.BBZ_Position_On < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 : 
                (int)HSTIOManifest.DigitalInputs.BBZ_Position_On / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.BBZ_Position_On % 8);
            
            panelDigitalInputOutputStationInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 
                (int)HSTIOManifest.DigitalInputs.BOS_Position_On < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 : 
                (int)HSTIOManifest.DigitalInputs.BOS_Position_On / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.BOS_Position_On % 8);
            
            panelDigitalInputOutputTurnTableInPosition.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_In_Position < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 : 
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_In_Position / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_In_Position % 8);
            
            //CarrierLockDeploy
            panelDigitalInputInputStationCarrierLockDeploy.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_CS_Deploy < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_CS_Deploy / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_CS_Deploy % 8);
            
            panelDigitalInputInputStationCarrierUnLockDeploy.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_CS_Retract < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_CS_Retract / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_CS_Retract % 8);
            
            panelDigitalInputOutputStationCarrierLockDeploy.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_CS_Deploy < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_CS_Deploy / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_CS_Deploy % 8);
            
            panelDigitalInputOutputStationCarrierUnLockDeploy.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_CS_Retract < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_CS_Retract / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_CS_Retract % 8);

            panelDigitalInputInputStationCarrierLock.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_CS_Lock < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_CS_Lock / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_CS_Lock % 8);
            
            panelDigitalInputInputStationCarrierUnLock.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_CS_Unlock < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_CS_Unlock / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_CS_Unlock % 8);
            
            panelDigitalInputOutputStationCarrierLock.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_CS_Lock < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_CS_Lock / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_CS_Lock % 8);
            
            panelDigitalInputOutputStationCarrierUnLock.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_CS_Unlock < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_CS_Unlock / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_CS_Unlock % 8);

            panelDigitalInputInputTurnTableRotateCW.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_0_Deg < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_0_Deg / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_0_Deg % 8);
            
            panelDigitalInputInputTurnTableRotateCCW.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_90_Deg < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_90_Deg / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_90_Deg % 8);
            
            panelDigitalInputOutputTurnTableRotateCW.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg % 8);
            
            panelDigitalInputOutputTurnTableRotateCCW.AssignAxis(HSTMachine.Workcell._a3200HC,
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_90_Deg < ((AdditionalSetOfIOForXAxis + 1) * 8) ? 0 :
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_90_Deg / 8 - AdditionalSetOfIOForXAxis, 
                (int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_90_Deg % 8);

            //Automation
            if (_workcell.TeachPointRecipe != null)
            {
                setAxesState();
            }
            ledInputEEParkPosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked ? true : false;
            ledInputEEPickPosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Pick ? true : false;
            ledInputEEPlacePosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Place ? true : false;
            ledInputEEDycemPosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.DycemCleaning ? true : false;
            ledOutputEEParkPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked ? true : false;
            ledOutputEEPickPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Pick ? true : false;
            ledOutputEEPlacePosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Place ? true : false;
            ledOutputEEDycemPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.DycemCleaning ? true : false;
            ledTestProbeParkPosition.State = HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked ? true : false;
            ledTestProbeTestPosition.State = HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Test ? true : false;
            ledPrecisorNestAtInputStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtInputStation ? true : false;
            ledPrecisorNestAtParkPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.Parked ? true : false;
            ledPrecisorNestAtTestStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtPrecisorStation ? true : false;
            ledPrecisorNestAtOutputStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtOutputStation ? true : false;
            //Conveyor Power
            ledEnableConveyorPower.State = _workcell.Process.MonitorProcess.Controller.isConveyorEnabled();
            ledDisableConveyorPower.State = !ledEnableConveyorPower.State;

            //Output Handler
            radioButtonLongTail.Checked = true;
            radioButtonUpTab.Checked = true;
            rdxPrecisorNestUpTab.Checked = true;
        }

        private void buttonInputLifterFree_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputStationProcess.Controller.FreeInputLifter();
        }

        private void buttonOutputLifterFree_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputStationProcess.Controller.FreeOutputLifter();
        }

        private void buttonInputTurnTableFree_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnTableFree();
        }

        private void buttonOutputTurnTableFree_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnTableFree();
        }

        private void buttonAllPrecisorVacuumOn_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.SetValvePosition(radioButtonUpTab.Checked);

            if(radioButtonLongTail.Checked)
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOnVacuumChannelsOneByOne(HST.Utils.HGAProductTailType.LongTail);

            if (radioButtonShortTail.Checked)
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOnVacuumChannelsOneByOne(HST.Utils.HGAProductTailType.ShortTail);
        }

        private void buttonAllPrecisorVacuumOff_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.SetValvePosition(radioButtonUpTab.Checked);
            HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOffVaccuumChannels();
        }

        private void buttonAllOutputEEVacuumOn_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobVacuum(true);
        }

        private void buttonAllOutputEEVacuumOff_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobVacuum(false);
        }

        private void TurnInputTurnTable()
        {
            uint timeUsed;

            if (!ledInputTurnTableAt0.State)
                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnSectionTurnTo0Deg(out timeUsed);
            else
                HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InputTurnSectionTurnTo90Deg(out timeUsed);

            labelInputTurnTableTimeUsed.Text = timeUsed.ToString();
        }

        private void RaiseOrLowerInputStopper()
        {
            uint timeUsed = 0;

            if (!ledInputStopperUp.State)
            {
                HSTMachine.Workcell.Process.InputStationProcess.Controller.RaiseInputStationStopper(true, out timeUsed);
            }
            else
            {
                HSTMachine.Workcell.Process.InputStationProcess.Controller.RaiseInputStationStopper(false, out timeUsed);
            }

            labelInputStopperTimeUsed.Text = timeUsed.ToString();
        }

        private void RaiseOrLowerInputLifter()
        {
            uint timeUsed;
            if (!ledInputLifterUp.State)
            {
                HSTMachine.Workcell.Process.InputStationProcess.Controller.RaiseInputLifter(true, out timeUsed);
            }
            else
            {
                HSTMachine.Workcell.Process.InputStationProcess.Controller.RaiseInputLifter(false, out timeUsed);
            }

            labelInputLifterTimeUsed.Text = timeUsed.ToString();
        }

        private void TurnOutputTurnTable()
        {
            uint timeUsed;

            if (!ledOutputTurnTableAt0.State)
                HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo0Deg(out timeUsed);
            else
                HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.OutputTurnSectionTurnTo90Deg(out timeUsed);

            labelOutputTurnTableTimeUsed.Text = timeUsed.ToString();
        }

        private void RaiseOrLowerOutputStopper()
        {
            uint timeUsed = 0;

            if (!ledOutputStopperUp.State)
            {
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.RaiseOutputStationStopper(true, out timeUsed);
            }
            else
            {
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.RaiseOutputStationStopper(false, out timeUsed);
            }

            labelOutputStopperTimeUsed.Text = timeUsed.ToString();
        }

        private void RaiseOrLowerOutputLifter()
        {
            uint timeUsed;
            if (!ledOutputLifterUp.State)
            {
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.RaiseOutputLifter(true, out timeUsed);
            }
            else
            {
                HSTMachine.Workcell.Process.OutputStationProcess.Controller.RaiseOutputLifter(false, out timeUsed);
            }

            labelOutputLifterTimeUsed.Text = timeUsed.ToString();
        }

        private void ledInputTurnTableAt0_Click(object sender, EventArgs e)
        {
            if (ledInputTurnTableAt0.State)
                return;

            TurnInputTurnTable();
        }

        private void timerInputTurnTableModule_Tick(object sender, EventArgs e)
        {
            try
            {
                ledInputTurnTableAt0.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_0_Deg)) == DigitalIOState.On ? true : false;

                ledInputTurnTableAt90.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Turn_Station_At_90_Deg)) == DigitalIOState.On ? true : false;

                ////indicate the output status
                ledSfotStartValve.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up)) == DigitalIOState.On ? true : false;
            }
            catch (Exception ex)
            {
                // do nothing

            }

            //update enableness of vacuum related buttton based on soft start valve
            ledInputTurnTableAt0.Enabled = ledSfotStartValve.State;
            ledInputTurnTableAt90.Enabled = ledSfotStartValve.State;
            ledInputStopperUp.Enabled = ledSfotStartValve.State;
            ledInputStopperDown.Enabled = ledSfotStartValve.State;
            ledInputScrewDriverExtend.Enabled = ledSfotStartValve.State;
            ledInputScrewDriverRetract.Enabled = ledSfotStartValve.State;
            ledInputScrewDriverLock.Enabled = ledSfotStartValve.State;
            ledInputScrewDriverUnLock.Enabled = ledSfotStartValve.State;
            ledInputLifterUp.Enabled = ledSfotStartValve.State;
            ledInputLifterDown.Enabled = ledSfotStartValve.State;
            ledFlattenerExtend.Enabled = ledSfotStartValve.State;
            ledFlattenerRetract.Enabled = ledSfotStartValve.State;
        }

        private void ledInputTurnTableAt90_Click(object sender, EventArgs e)
        {
            if (ledInputTurnTableAt90.State)
                return;

            TurnInputTurnTable();
        }

        private void tabControlOperationStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlOperationStatus.SelectedIndex == 1)//automation page
            {
                timerInputTurnTableModule.Enabled = true;
                timerInputHandlerModule.Enabled = true;
                timerOutputHandlerModule.Enabled = true;
                timerTestProbeHandlerModule.Enabled = true;
                timerPrecisorNestModule.Enabled = true;
                timerOutputTurnTableModule.Enabled = true;
                timerConveyorInhibit.Enabled = true;
            }
            else
            {
                timerInputTurnTableModule.Enabled = false;
                timerInputHandlerModule.Enabled = false;
                timerOutputHandlerModule.Enabled = false;
                timerTestProbeHandlerModule.Enabled = false;
                timerPrecisorNestModule.Enabled = false;
                timerOutputTurnTableModule.Enabled = false;
                timerConveyorInhibit.Enabled = false;
            }

            if (tabControlOperationStatus.SelectedIndex == 0)//Seaveyor page
            {
                timerConveyorPower.Enabled = true;
            }
            else
            {
                timerConveyorPower.Enabled = false;
            }
        }
        
        private void ledInputEEPickPosition_Click(object sender, EventArgs e)
        {
            if (ledInputEEPickPosition.State)
                return;

            if (HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtInputStation)
                HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobMoveZToPick(true);
            else
                MessageBox.Show("Move cancelled.\n\nPrecisor Nest is at Input Station Position");
        }

        private void ledInputEEPlacePosition_Click(object sender, EventArgs e)
        {
            if (ledInputEEPlacePosition.State)
                return;

            HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobMoveZToPlace(rdxPrecisorNestUpTab.Checked, true);
        }

        private void ledInputEEDycemPosition_Click(object sender, EventArgs e)
        {
            if (ledInputEEDycemPosition.State)
                return;

            if (HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtInputStation)
                HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobMoveZToDycem(true);
            else
                MessageBox.Show("Move cancelled.\n\nPrecisor Nest is at Input Station Position");
        }

        private void timerInputHandlerModule_Tick(object sender, EventArgs e)
        {
            ledInputEEParkPosition.State =  HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked ? true: false;
            ledInputEEPickPosition.State =  HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Pick ? true: false;
            ledInputEEPlacePosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Place ? true : false;
            ledInputEEDycemPosition.State = HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.DycemCleaning ? true : false;

            try
            {
                ledInputScrewDriverExtend.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Deploy)) == DigitalIOState.On ? true : false;
                
                ledInputScrewDriverRetract.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Retract)) == DigitalIOState.On ? true : false;

                ledInputScrewDriverLock.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Lock)) == DigitalIOState.On ? true : false;

                ledInputScrewDriverUnLock.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Unlock)) == DigitalIOState.On ? true : false;

                ledInputStopperUp.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Stopper_Up)) == DigitalIOState.On ? true : false;

                ledInputStopperDown.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Stopper_Down)) == DigitalIOState.On ? true : false;

                ledInputLifterUp.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Up)) == DigitalIOState.On ? true : false;

                ledInputLifterDown.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Down)) == DigitalIOState.On ? true : false;

                ledFlattenerExtend.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_Flattener)) == DigitalIOState.On ? true : false;

                ledFlattenerRetract.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_Flattener)) == DigitalIOState.Off ? true : false;

                //led indicate the vacuum output status  
                ledInputEEVacuum.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_VCH)) == DigitalIOState.On ? true : false;

                //led indicate the purge output status  
                ledInputEEPurge.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_PCH)) == DigitalIOState.On ? true : false;

                ledInputEEParkPosition.Enabled = _recipeLoaded;
                ledInputEEPickPosition.Enabled = _recipeLoaded;
                ledInputEEPlacePosition.Enabled = _recipeLoaded;
                ledInputEEDycemPosition.Enabled = _recipeLoaded;
            }
            catch (Exception ex)
            {
                //Do nothing
            }

        }

        private void ledInputEEParkPosition_Click(object sender, EventArgs e)
        {
            if (ledInputEEParkPosition.State)
                return;

            HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobMoveZToPark(true);
        }

        private void ledInputScrewDriverLock_Click(object sender, EventArgs e)
        {
            if (ledInputScrewDriverLock.State)
                return;

            HSTMachine.Workcell.Process.InputStationProcess.Controller.InputStationClampRotaryOpenCover(false);
        }

        private void ledInputScrewDriverUnLock_Click(object sender, EventArgs e)
        {
            if (ledInputScrewDriverUnLock.State)
                return;

            HSTMachine.Workcell.Process.InputStationProcess.Controller.InputStationClampRotaryOpenCover(true);            
        }

        private void timerOutputHandlerModule_Tick(object sender, EventArgs e)
        {
            ledOutputEEParkPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked ? true : false;
            ledOutputEEPickPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Pick ? true : false;
            ledOutputEEPlacePosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Place ? true : false;
            ledOutputEEDycemPosition.State = HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.DycemCleaning ? true : false;

            try
            {
                ledOutputScrewDriverExtend.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Deploy)) == DigitalIOState.On ? true : false;

                ledOutputScrewDriverRetract.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Retract)) == DigitalIOState.On ? true : false;

                ledOutputScrewDriverLock.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Lock)) == DigitalIOState.On ? true : false;

                ledOutputScrewDriverUnLock.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_CS_Unlock)) == DigitalIOState.On ? true : false;

                ledOutputStopperUp.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Stopper_Up)) == DigitalIOState.On ? true : false;

                ledOutputStopperDown.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Stopper_Down)) == DigitalIOState.On ? true : false;

                ledOutputLifterUp.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Up)) == DigitalIOState.On ? true : false;

                ledOutputLifterDown.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Down)) == DigitalIOState.On ? true : false;

                //led indicate the vacuum output status  
                ledOutputEEVacuum.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_VCH)) == DigitalIOState.On ? true : false;

                //led indicate the purge output status  
                ledOutputEEPurge.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_PCH)) == DigitalIOState.On ? true : false;

                ledOutputEEParkPosition.Enabled = _recipeLoaded;
                ledOutputEEPickPosition.Enabled = _recipeLoaded;
                ledOutputEEPlacePosition.Enabled = _recipeLoaded;
                ledOutputEEDycemPosition.Enabled = _recipeLoaded;
            }
            catch(Exception ex)
            {
                // do nothing
            }
        }

        private void ledInputStopperUp_Click(object sender, EventArgs e)
        {
            if (ledInputStopperUp.State)
                return;

            RaiseOrLowerInputStopper();
        }

        private void ledInputStopperDown_Click(object sender, EventArgs e)
        {
            if (ledInputStopperDown.State)
                return;

            RaiseOrLowerInputStopper();
        }

        private void ledInputLifterUp_Click(object sender, EventArgs e)
        {
            if (ledInputLifterUp.State)
                return;

            //if both up and down sensor is off, turn on down output first to avoid sudden lifter up
            if (HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Down).State == DigitalIOState.Off &&
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Up).State == DigitalIOState.Off)
            {
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Down).Set(DigitalIOState.On);
                Thread.Sleep(500);
            }

            RaiseOrLowerInputLifter();
        }

        private void ledInputLifterDown_Click(object sender, EventArgs e)
        {
            if (ledInputLifterDown.State)
                return;

            RaiseOrLowerInputLifter();
        }

        private void buttonInputPickVacuumProcess_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobVacuum(true);
        }

        private void buttonInputPlacePurgeProcess_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputEEProcess.Controller.DoJobVacuum(false);

            HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.SetValvePosition(radioButtonUpTab.Checked);

            if (radioButtonLongTail.Checked)
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOnVacuumChannelsOneByOne(HST.Utils.HGAProductTailType.LongTail);

            if (radioButtonShortTail.Checked)
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOnVacuumChannelsOneByOne(HST.Utils.HGAProductTailType.ShortTail);
        }

        private void ledInputEEVacuum_Click(object sender, EventArgs e)
        {
            if (ledInputEEVacuum.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_VCH).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_VCH).Set(DigitalIOState.On);
        }

        private void ledInputEEPurge_Click(object sender, EventArgs e)
        {
            if (ledInputEEPurge.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_PCH).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_EE_PCH).Set(DigitalIOState.On);
        }

        private void timerTestProbeHandlerModule_Tick(object sender, EventArgs e)
        {
            ledTestProbeParkPosition.State = HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked ? true : false;
            ledTestProbeTestPosition.State = HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Test ? true : false;

            ledTestProbeParkPosition.Enabled = _recipeLoaded;
            ledTestProbeTestPosition.Enabled = _recipeLoaded;
        }

        private void ledOutputTurnTableAt0_Click(object sender, EventArgs e)
        {
            if (ledOutputTurnTableAt0.State)
                return;

            TurnOutputTurnTable();
        }

        private void ledOutputTurnTableAt90_Click(object sender, EventArgs e)
        {
            if (ledOutputTurnTableAt90.State)
                return;

            TurnOutputTurnTable();
        }

        private void ledTestProbeParkPosition_Click(object sender, EventArgs e)
        {
            if (ledTestProbeParkPosition.State)
                return;

            HSTMachine.Workcell.Process.TestProbeProcess.Controller.GoToParkPosition(true);
        }

        private void ledTestProbeTestPosition_Click(object sender, EventArgs e)
        {
            if (ledTestProbeTestPosition.State)
                return;

            HSTMachine.Workcell.Process.TestProbeProcess.Controller.GoToProbePosition(rdxPrecisorNestUpTab.Checked, true);
        }

        private void ledPrecisorNestAtInputStationPosition_Click(object sender, EventArgs e)
        {
            if (ledPrecisorNestAtInputStationPosition.State)
                return;

            try
            {
                if (HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked &&
                    HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked &&
                    HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked)
                    HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.MoveToInputStation(rdxPrecisorNestUpTab.Checked, true);
                else
                    MessageBox.Show("Move cancelled.\n\nOne or more of Z Axis is not at Park Position");
            }
            catch(Exception ex)
            {
                CommonFunctions.Instance.PromptExceptionErrorMessage("Move Precisor Nest To Input Station", ex);
            }
        }

        private void ledPrecisorNestAtParkPosition_Click(object sender, EventArgs e)
        {
            if (ledPrecisorNestAtParkPosition.State)
                return;

            try
            {
                if (HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked &&
                    HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked &&
                    HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked)
                    HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.ParkPrecisorNestBetweenInputStationAndPrecisorStation(true);
                else
                    MessageBox.Show("Move cancelled.\n\nOne or more of Z Axis is not at Park Position");
            }
            catch (Exception ex)
            {
                CommonFunctions.Instance.PromptExceptionErrorMessage("Move Precisor Nest To Park Position", ex);
            }
        }

        private void ledPrecisorNestAtTestStationPosition_Click(object sender, EventArgs e)
        {
            if (ledPrecisorNestAtTestStationPosition.State)
                return;

            try
            {
                if (HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked &&
                    HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked &&
                    HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked)
                    HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.MoveToPrecisorStation(rdxPrecisorNestUpTab.Checked, true);
                else
                    MessageBox.Show("Move cancelled.\n\nOne or more of Z Axis is not at Park Position");
            }
            catch (Exception ex)
            {
                CommonFunctions.Instance.PromptExceptionErrorMessage("Move Precisor Nest To Test Station", ex);
            }
        }

        private void ledPrecisorNestAtOutputStationPosition_Click(object sender, EventArgs e)
        {
            if (ledPrecisorNestAtOutputStationPosition.State)
                return;

            try
            {
                if (HSTMachine.Workcell.InputEEZAxisPosition == InputEEZAxis.Parked &&
                    HSTMachine.Workcell.TestProbeZAxisPosition == TestProbeZAxis.Parked &&
                    HSTMachine.Workcell.OutputEEZAxisPosition == OutputEEZAxis.Parked)
                    HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.MoveToOutputStation(rdxPrecisorNestUpTab.Checked, true);
                else
                    MessageBox.Show("Move cancelled.\n\nOne or more of Z Axis is not at Park Position");
            }
            catch (Exception ex)
            {
                CommonFunctions.Instance.PromptExceptionErrorMessage("Move Precisor Nest To Output Station", ex);
            }
        }

        private void ledPrecisorVacuum1_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum1.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_1).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_1).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum2_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum2.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_2).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_2).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum3_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum3.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_3).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_3).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum4_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum4.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_4).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_4).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum5_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum5.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_5).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_5).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum6_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum6.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_6).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_6).Set(DigitalIOState.On);
        }

        private void ledPrecisorVacuum7_Click(object sender, EventArgs e)
        {
            if (ledPrecisorVacuum7.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_7).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_7).Set(DigitalIOState.On);
        }

        private void timerPrecisorNestModule_Tick(object sender, EventArgs e)
        {
            ledPrecisorNestAtInputStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition ==  PrecisorNestXAxis.AtInputStation? true : false;
            ledPrecisorNestAtParkPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.Parked ? true : false;
            ledPrecisorNestAtTestStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtPrecisorStation ? true : false;
            ledPrecisorNestAtOutputStationPosition.State = HSTMachine.Workcell.PrecisorNestXAxisPosition == PrecisorNestXAxis.AtOutputStation ? true : false;

            try
            {
                ledPrecisorVacuum1.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_1)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum2.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_2)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum3.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_3)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum4.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_4)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum5.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_5)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum6.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_6)) == DigitalIOState.On ? true : false;

                ledPrecisorVacuum7.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.PN_VCH_7)) == DigitalIOState.On ? true : false;

                ledPrecisorNestAtInputStationPosition.Enabled = _recipeLoaded;
                ledPrecisorNestAtParkPosition.Enabled = _recipeLoaded;
                ledPrecisorNestAtTestStationPosition.Enabled = _recipeLoaded;
                ledPrecisorNestAtOutputStationPosition.Enabled = _recipeLoaded;

            }
            catch(Exception ex)
            {
                // do nothing
            }
        }

        private void ledOutputEEParkPosition_Click(object sender, EventArgs e)
        {
            if (ledOutputEEParkPosition.State)
                return;

            HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobMoveZToPark(true, false);
        }

        private void ledOutputEEPickPosition_Click(object sender, EventArgs e)
        {
            if (ledOutputEEPickPosition.State)
                return;

            HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobMoveZToPick(rdxPrecisorNestUpTab.Checked, true);
        }

        private void ledOutputEEPlacePosition_Click(object sender, EventArgs e)
        {
            if (ledOutputEEPlacePosition.State)
                return;

            if (HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtOutputStation)
                HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobMoveZToPlace(true);
            else
                MessageBox.Show("Move cancelled.\n\nPrecisor Nest is at Output Station Position");
        }

        private void ledOutputEEDycemPosition_Click(object sender, EventArgs e)
        {
            if (ledOutputEEDycemPosition.State)
                return;

            if (HSTMachine.Workcell.PrecisorNestXAxisPosition != PrecisorNestXAxis.AtOutputStation)
                HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobMoveZToDycem(true);
            else
                MessageBox.Show("Move cancelled.\n\nPrecisor Nest is at Output Station Position");
        }

        private void ledOutputStopperUp_Click(object sender, EventArgs e)
        {
            if (ledOutputStopperUp.State)
                return;

            RaiseOrLowerOutputStopper();
        }

        private void ledOutputStopperDown_Click(object sender, EventArgs e)
        {
            if (ledOutputStopperDown.State)
                return;

            RaiseOrLowerOutputStopper();
        }

        private void ledOutputScrewDriverLock_Click(object sender, EventArgs e)
        {
            if (ledOutputScrewDriverLock.State)
                return;

            HSTMachine.Workcell.Process.OutputStationProcess.Controller.OutputStationClampRotaryOpenCover(false);
        }

        private void ledOutputScrewDriverUnLock_Click(object sender, EventArgs e)
        {
            if (ledOutputScrewDriverUnLock.State)
                return;

            HSTMachine.Workcell.Process.OutputStationProcess.Controller.OutputStationClampRotaryOpenCover(true);   
        }

        private void ledOutputLifterUp_Click(object sender, EventArgs e)
        {
            if (ledOutputLifterUp.State)
                return;

            //if both up and down sensor is off, turn on down output first to avoid sudden lifter up
            if (HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Down).State == DigitalIOState.Off &&
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Up).State == DigitalIOState.Off)
            {
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Down).Set(DigitalIOState.On);
                Thread.Sleep(500);
            }

            RaiseOrLowerOutputLifter();
        }

        private void ledOutputLifterDown_Click(object sender, EventArgs e)
        {
            if (ledOutputLifterDown.State)
                return;

            RaiseOrLowerOutputLifter();
        }

        private void buttonOutputLifterFree_Click_1(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputStationProcess.Controller.FreeOutputLifter();
        }

        private void ledOutputEEVacuum_Click(object sender, EventArgs e)
        {
            if (ledOutputEEVacuum.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_VCH).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_VCH).Set(DigitalIOState.On);
        }

        private void ledOutputEEPurge_Click(object sender, EventArgs e)
        {
            if (ledOutputEEPurge.State)
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_PCH).Set(DigitalIOState.Off);
            else
                HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_EE_PCH).Set(DigitalIOState.On);
        }

        private void buttonOutputPickVacuumProcess_Click(object sender, EventArgs e)
        {
            if (buttonOutputPickVacuumProcess.BackColor != Color.Green)
            {
                HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobVacuum(true);
                buttonOutputPickVacuumProcess.BackColor = Color.LightGreen;
                buttonOutputPlacePurgeProcess.BackColor = Color.Transparent;
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.SetValvePosition(radioButtonUpTab.Checked);
                HSTMachine.Workcell.Process.PrecisorStationProcess.Controller.TurnOffVaccuumChannels();
            }
        }

        private void buttonOutputPlacePurgeProcess_Click(object sender, EventArgs e)
        {
            if (buttonOutputPlacePurgeProcess.BackColor != Color.Green)
            {
                HSTMachine.Workcell.Process.OutputEEProcess.Controller.DoJobVacuum(false);
                buttonOutputPickVacuumProcess.BackColor = Color.Transparent;
                buttonOutputPlacePurgeProcess.BackColor = Color.LightGreen;
            }
           
        }

        private void timerOutputTurnTableModule_Tick(object sender, EventArgs e)
        {
            try
            {
                ledOutputTurnTableAt0.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_0_Deg)) == DigitalIOState.On ? true : false;

                ledOutputTurnTableAt90.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Turn_Station_At_90_Deg)) == DigitalIOState.On ? true : false;
            }
            catch (Exception ex)
            {
                // do nothing
            }
            //update enableness of vacuum related buttton based on soft start valve
            ledOutputTurnTableAt0.Enabled = ledSfotStartValve.State;
            ledOutputTurnTableAt90.Enabled = ledSfotStartValve.State;
            ledOutputStopperUp.Enabled = ledSfotStartValve.State;
            ledOutputStopperDown.Enabled = ledSfotStartValve.State;
            ledOutputScrewDriverExtend.Enabled = ledSfotStartValve.State;
            ledOutputScrewDriverRetract.Enabled = ledSfotStartValve.State;
            ledOutputScrewDriverLock.Enabled = ledSfotStartValve.State;
            ledOutputScrewDriverUnLock.Enabled = ledSfotStartValve.State;
            ledOutputLifterUp.Enabled = ledSfotStartValve.State;
            ledOutputLifterDown.Enabled = ledSfotStartValve.State;
        }

        private void ledInputScrewDriverExtend_Click(object sender, EventArgs e)
        {
            if (ledInputScrewDriverExtend.State)
                return;

            HSTMachine.Workcell.Process.InputStationProcess.Controller.InputStationForwardClamp(true);
        }

        private void ledInputScrewDriverRetract_Click(object sender, EventArgs e)
        {
            if (ledInputScrewDriverRetract.State)
                return;

            HSTMachine.Workcell.Process.InputStationProcess.Controller.InputStationForwardClamp(false);
        }

        private void ledOutputScrewDriverExtend_Click(object sender, EventArgs e)
        {
            if (ledOutputScrewDriverExtend.State)
                return;

            HSTMachine.Workcell.Process.OutputStationProcess.Controller.OutputStationForwardClamp(true);
        }

        private void ledOutputScrewDriverRetract_Click(object sender, EventArgs e)
        {
            if (ledOutputScrewDriverRetract.State)
                return;

            HSTMachine.Workcell.Process.OutputStationProcess.Controller.OutputStationForwardClamp(false);
        }

        private void ledSfotStartValve_Click(object sender, EventArgs e)
        {
            if (ledSfotStartValve.State)
                HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up).Set(DigitalIOState.Off);
            else
            {
                if (!(HSTMachine.Workcell.IOManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Down).State == DigitalIOState.On &&
                    HSTMachine.Workcell.IOManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Up).State == DigitalIOState.Off))
                {
                    HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Up).Set(DigitalIOState.Off);
                    HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Down).Set(DigitalIOState.On);
                }

                if (!(HSTMachine.Workcell.IOManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Down).State == DigitalIOState.On &&
                    HSTMachine.Workcell.IOManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Lifter_Up).State == DigitalIOState.Off))
                {
                    HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Up).Set(DigitalIOState.Off);
                    HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Lifter_Down).Set(DigitalIOState.On);
                }

                HSTMachine.Workcell.IOManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Soft_Start_Up).Set(DigitalIOState.On);
            }
        }

        private void btnOpenRecipe_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = HSTSettings.Instance.Directory.MachineRobotPointPath;
            openFileDialog1.Filter = "TeachPointRcp files (*.TeachPointRcp)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fullPath = openFileDialog1.FileName;
                    string fileName = openFileDialog1.SafeFileName;
                    string path = fullPath.Replace(fileName, "");

                    LoadRecipeFromXmlFile(path, fileName, ".TeachPointRcp");

                    textBoxRecipeNameDisplay.Text = _teachPointRecipe.Name;
                    textBoxVersionDisplay.Text = _teachPointRecipe.Version;
                    textBoxDescriptionDisplay.Text = _teachPointRecipe.Description;

                    // Re-enable the X, Y and Theta axes of the Precisor Nest
                    if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel() != null)
                    {
                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestXAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.X), true);
                        }

                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestYAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.Y), true);
                        }

                        if (HSTMachine.Workcell.getPanelDiagnostics().getMotionControllerPanel().getPrecisorNestThetaAxis() != null)
                        {
                            HSTMachine.Workcell._a3200HC.Enable(HSTMachine.Workcell._ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta), true);
                        }
                    }

                    _recipeLoaded = true;
                    if (_workcell.TeachPointRecipe != null)
                    {
                        setAxesState();
                    }
                }
                catch (Exception ex)
                {
                    _teachPointRecipe = null;
                    ParselMessageBox.Show("Load error:", ex, MessageBoxIcon.Error,
                         Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                         Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                         Seagate.AAS.Parsel.Services.ErrorButton.OK);
                }
            }
        }

        private void LoadRecipeFromXmlFile(string searchFolder, string selectedFileName, string _ext)
        {
            try
            {
                _teachPointRecipe = new TeachPointRecipe();
                _doc = new XmlDocument();

                if (selectedFileName.Contains(_ext))
                    _doc.Load(searchFolder + "\\" + selectedFileName);
                else
                    _doc.Load(searchFolder + "\\" + selectedFileName + _ext);

                _teachPointRecipe.Load(_doc);
                HSTMachine.Workcell.TeachPointRecipe.Load(_doc);

                _teachPointRecipe.Name = selectedFileName;
                _teachPointRecipe.FullPath = searchFolder + selectedFileName;
            }
            catch (Exception ex)
            {
                _teachPointRecipe = null;
                ParselMessageBox.Show("Load error:", ex, MessageBoxIcon.Error,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.NoButton,
                     Seagate.AAS.Parsel.Services.ErrorButton.OK);
            }
        }

        public void EnableAllButtons()
        {
            arrowButton1.Enabled = true;
            arrowButton2.Enabled = true;
            arrowButton3.Enabled = true;
            arrowButton4.Enabled = true;
            arrowButton5.Enabled = true;
            arrowButton6.Enabled = true;
            arrowButton7.Enabled = true;
            arrowButton8.Enabled = true;
            arrowButton9.Enabled = true;
            arrowButton10.Enabled = true;
            arrowButton11.Enabled = true;
            arrowButton12.Enabled = true;
            arrowButton13.Enabled = true;
            buttonAllPrecisorVacuumOn.Enabled = true;
            buttonAllPrecisorVacuumOff.Enabled = true;
            buttonInputPlacePurgeProcess.Enabled = true;
            buttonInputPickVacuumProcess.Enabled = true;
            buttonOutputTurnTableFree.Enabled = true;
            buttonInputTurnTableFree.Enabled = true;
            buttonInputLifterFree.Enabled = true;
            buttonOutputPlacePurgeProcess.Enabled = true;
            buttonOutputPickVacuumProcess.Enabled = true;
            buttonOutputLifterFree.Enabled = true;
            btnOpenRecipe.Enabled = true;

            panelDigitalInputOutputTurnTableInPosition.Enabled = true;
            panelDigitalInputOutputStationInPosition.Enabled = true;
            panelDigitalInputBufferStationInPosition.Enabled = true;
            panelDigitalInputInputStationInPosition.Enabled = true;
            panelDigitalInputOutputTurnTableRotateCW.Enabled = true;
            panelDigitalInputOutputTurnTableRotateCCW.Enabled = true;
            panelDigitalInputInputTurnTableRotateCW.Enabled = true;
            panelDigitalInputInputTurnTableRotateCCW.Enabled = true;
            panelDigitalInputOutputStationCarrierLock.Enabled = true;
            panelDigitalInputInputStationCarrierLock.Enabled = true;
            panelDigitalInputOutputStationCarrierUnLockDeploy.Enabled = true;
            panelDigitalInputInputStationCarrierUnLockDeploy.Enabled = true;
            panelDigitalInputOutputStationCarrierLockDeploy.Enabled = true;
            panelDigitalInputInputStationCarrierLockDeploy.Enabled = true;
            panelDigitalInputInputTurnTableInPosition.Enabled = true;
            panelDigitalInputOutputStationCarrierUnLock.Enabled = true;
            panelDigitalInputInputStationCarrierUnLock.Enabled = true;
            arrowButtonDownToPrecisorStation.Enabled = true;
            arrowButtonUpFromPrecisorStation.Enabled = true;
            ledPrecisorVacuum1.Enabled = true;
            ledPrecisorVacuum2.Enabled = true;
            ledPrecisorVacuum3.Enabled = true;
            ledPrecisorVacuum4.Enabled = true;
            ledPrecisorVacuum5.Enabled = true;
            ledPrecisorVacuum6.Enabled = true;
            ledPrecisorVacuum7.Enabled = true;
            ledPrecisorNestAtOutputStationPosition.Enabled = true;
            ledPrecisorNestAtTestStationPosition.Enabled = true;
            ledPrecisorNestAtParkPosition.Enabled = true;
            ledPrecisorNestAtInputStationPosition.Enabled = true;
            ledTestProbeTestPosition.Enabled = true;
            ledTestProbeParkPosition.Enabled = true;
            ledOutputTurnTableAt90.Enabled = true;
            ledOutputTurnTableAt0.Enabled = true;
            ledInputEEPurge.Enabled = true;
            ledInputEEVacuum.Enabled = true;
            ledInputLifterDown.Enabled = true;
            ledInputLifterUp.Enabled = true;
            ledInputStopperDown.Enabled = true;
            ledInputStopperUp.Enabled = true;
            ledInputScrewDriverUnLock.Enabled = true;
            ledInputScrewDriverLock.Enabled = true;
            ledInputEEPlacePosition.Enabled = true;
            ledInputEEPickPosition.Enabled = true;
            ledInputEEParkPosition.Enabled = true;
            ledInputEEDycemPosition.Enabled = true;
            ledInputTurnTableAt90.Enabled = true;
            ledInputTurnTableAt0.Enabled = true;
            groupBoxPrecisorVacuum.Enabled = true;
            groupBoxTailType.Enabled = true;
            groupBoxTabType.Enabled = true;
            ledPrecisorOutputStationPosition.Enabled = true;
            ledPrecisorInputStationPosition.Enabled = true;
            ledTestProbeTestPositionOnPic.Enabled = true;
            ledPrecisorTestStationPosition.Enabled = true;
            ledTestProbeSafePosition.Enabled = true;
            ledOutputEEPurge.Enabled = true;
            ledOutputEEVacuum.Enabled = true;
            ledOutputLifterDown.Enabled = true;
            ledOutputLifterUp.Enabled = true;
            ledOutputStopperDown.Enabled = true;
            ledOutputStopperUp.Enabled = true;
            ledOutputScrewDriverUnLock.Enabled = true;
            ledOutputScrewDriverLock.Enabled = true;
            ledOutputEEPlacePosition.Enabled = true;
            ledOutputEEPickPosition.Enabled = true;
            ledOutputEEParkPosition.Enabled = true;
            ledOutputEEDycemPosition.Enabled = true;
            ledInputScrewDriverRetract.Enabled = true;
            ledInputScrewDriverExtend.Enabled = true;
            ledOutputScrewDriverRetract.Enabled = true;
            ledOutputScrewDriverExtend.Enabled = true;
            ledFlattenerExtend.Enabled = true;
            ledFlattenerRetract.Enabled = true;
            ledSfotStartValve.Enabled = true;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
        }

        public void DisableAllButtons()
        {
            arrowButton1.Enabled = false;
            arrowButton2.Enabled = false;
            arrowButton3.Enabled = false;
            arrowButton4.Enabled = false;
            arrowButton5.Enabled = false;
            arrowButton6.Enabled = false;
            arrowButton7.Enabled = false;
            arrowButton8.Enabled = false;
            arrowButton9.Enabled = false;
            arrowButton10.Enabled = false;
            arrowButton11.Enabled = false;
            arrowButton12.Enabled = false;
            arrowButton13.Enabled = false;
            buttonAllPrecisorVacuumOn.Enabled = false;
            buttonAllPrecisorVacuumOff.Enabled = false;
            buttonInputPlacePurgeProcess.Enabled = false;
            buttonInputPickVacuumProcess.Enabled = false;
            buttonOutputTurnTableFree.Enabled = false;
            buttonInputTurnTableFree.Enabled = false;
            buttonInputLifterFree.Enabled = false;
            buttonOutputPlacePurgeProcess.Enabled = false;
            buttonOutputPickVacuumProcess.Enabled = false;
            buttonOutputLifterFree.Enabled = false;
            btnOpenRecipe.Enabled = false;

            panelDigitalInputOutputTurnTableInPosition.Enabled = false;
            panelDigitalInputOutputStationInPosition.Enabled = false;
            panelDigitalInputBufferStationInPosition.Enabled = false;
            panelDigitalInputInputStationInPosition.Enabled = false;
            panelDigitalInputOutputTurnTableRotateCW.Enabled = false;
            panelDigitalInputOutputTurnTableRotateCCW.Enabled = false;
            panelDigitalInputInputTurnTableRotateCW.Enabled = false;
            panelDigitalInputInputTurnTableRotateCCW.Enabled = false;
            panelDigitalInputOutputStationCarrierLock.Enabled = false;
            panelDigitalInputInputStationCarrierLock.Enabled = false;
            panelDigitalInputOutputStationCarrierUnLockDeploy.Enabled = false;
            panelDigitalInputInputStationCarrierUnLockDeploy.Enabled = false;
            panelDigitalInputOutputStationCarrierLockDeploy.Enabled = false;
            panelDigitalInputInputStationCarrierLockDeploy.Enabled = false;
            panelDigitalInputInputTurnTableInPosition.Enabled = false;
            panelDigitalInputOutputStationCarrierUnLock.Enabled = false;
            panelDigitalInputInputStationCarrierUnLock.Enabled = false;
            arrowButtonDownToPrecisorStation.Enabled = false;
            arrowButtonUpFromPrecisorStation.Enabled = false;
            ledPrecisorVacuum1.Enabled = false;
            ledPrecisorVacuum2.Enabled = false;
            ledPrecisorVacuum3.Enabled = false;
            ledPrecisorVacuum4.Enabled = false;
            ledPrecisorVacuum5.Enabled = false;
            ledPrecisorVacuum6.Enabled = false;
            ledPrecisorVacuum7.Enabled = false;
            ledPrecisorNestAtOutputStationPosition.Enabled = false;
            ledPrecisorNestAtTestStationPosition.Enabled = false;
            ledPrecisorNestAtParkPosition.Enabled = false;
            ledPrecisorNestAtInputStationPosition.Enabled = false;
            ledTestProbeTestPosition.Enabled = false;
            ledTestProbeParkPosition.Enabled = false;
            ledOutputTurnTableAt90.Enabled = false;
            ledOutputTurnTableAt0.Enabled = false;
            ledInputEEPurge.Enabled = false;
            ledInputEEVacuum.Enabled = false;
            ledInputLifterDown.Enabled = false;
            ledInputLifterUp.Enabled = false;
            ledInputStopperDown.Enabled = false;
            ledInputStopperUp.Enabled = false;
            ledInputScrewDriverUnLock.Enabled = false;
            ledInputScrewDriverLock.Enabled = false;
            ledInputEEPlacePosition.Enabled = false;
            ledInputEEDycemPosition.Enabled = false;
            ledInputEEPickPosition.Enabled = false;
            ledInputEEParkPosition.Enabled = false;
            ledInputTurnTableAt90.Enabled = false;
            ledInputTurnTableAt0.Enabled = false;
            groupBoxPrecisorVacuum.Enabled = false;
            groupBoxTailType.Enabled = false;
            groupBoxTabType.Enabled = false;
            ledPrecisorOutputStationPosition.Enabled = false;
            ledPrecisorInputStationPosition.Enabled = false;
            ledTestProbeTestPositionOnPic.Enabled = false;
            ledPrecisorTestStationPosition.Enabled = false;
            ledTestProbeSafePosition.Enabled = false;
            ledOutputEEPurge.Enabled = false;
            ledOutputEEVacuum.Enabled = false;
            ledOutputLifterDown.Enabled = false;
            ledOutputLifterUp.Enabled = false;
            ledOutputStopperDown.Enabled = false;
            ledOutputStopperUp.Enabled = false;
            ledOutputScrewDriverUnLock.Enabled = false;
            ledOutputScrewDriverLock.Enabled = false;
            ledOutputEEPlacePosition.Enabled = false;
            ledOutputEEPickPosition.Enabled = false;
            ledOutputEEParkPosition.Enabled = false;
            ledOutputEEDycemPosition.Enabled = false;
            ledInputScrewDriverRetract.Enabled = false;
            ledInputScrewDriverExtend.Enabled = false;
            ledOutputScrewDriverRetract.Enabled = false;
            ledOutputScrewDriverExtend.Enabled = false;
            ledFlattenerExtend.Enabled = false;
            ledFlattenerRetract.Enabled = false;
            ledSfotStartValve.Enabled = false;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }

        public void SetAllTimerOnOff(bool bOn)
        {
            timerInputTurnTableModule.Enabled = bOn;
            timerInputHandlerModule.Enabled = bOn;
            timerOutputHandlerModule.Enabled = bOn;
            timerTestProbeHandlerModule.Enabled = bOn;
            timerPrecisorNestModule.Enabled = bOn;
            timerOutputTurnTableModule.Enabled = bOn;
            timerConveyorPower.Enabled = bOn;
            timerConveyorInhibit.Enabled = bOn;
        }

        private void tabControlOperationStatus_Leave(object sender, EventArgs e)
        {
            //disable all timer when leave 
            timerInputTurnTableModule.Enabled = false;
            timerInputHandlerModule.Enabled = false;
            timerOutputHandlerModule.Enabled = false;
            timerTestProbeHandlerModule.Enabled = false;
            timerPrecisorNestModule.Enabled = false;
            timerOutputTurnTableModule.Enabled = false;
            timerConveyorPower.Enabled = false;
            timerConveyorInhibit.Enabled = false;
        }

        private void tabControlOperationStatus_Enter(object sender, EventArgs e)
        {
            if (tabControlOperationStatus.SelectedIndex == 1)//automation page
            {
                timerInputTurnTableModule.Enabled = true;
                timerInputHandlerModule.Enabled = true;
                timerOutputHandlerModule.Enabled = true;
                timerTestProbeHandlerModule.Enabled = true;
                timerPrecisorNestModule.Enabled = true;
                timerOutputTurnTableModule.Enabled = true;
                timerConveyorInhibit.Enabled = true;
            }
            else
            {
                timerInputTurnTableModule.Enabled = false;
                timerInputHandlerModule.Enabled = false;
                timerOutputHandlerModule.Enabled = false;
                timerTestProbeHandlerModule.Enabled = false;
                timerPrecisorNestModule.Enabled = false;
                timerOutputTurnTableModule.Enabled = false;
                timerConveyorInhibit.Enabled = false;
            }

            if (tabControlOperationStatus.SelectedIndex == 0)//Seaveyor page
            {
                timerConveyorPower.Enabled = true;
            }
            else
            {
                timerConveyorPower.Enabled = false;
            }
            tabControlOperationStatus.Focus();
        }

        private void tabControlOperationStatus_VisibleChanged(object sender, EventArgs e)
        {
            if (tabControlOperationStatus.Visible)
            {
                if (tabControlOperationStatus.SelectedIndex == 1)//automation page
                {
                    timerInputTurnTableModule.Enabled = true;
                    timerInputHandlerModule.Enabled = true;
                    timerOutputHandlerModule.Enabled = true;
                    timerTestProbeHandlerModule.Enabled = true;
                    timerPrecisorNestModule.Enabled = true;
                    timerOutputTurnTableModule.Enabled = true;
                    timerConveyorInhibit.Enabled = true;
                }
                else
                {
                    timerInputTurnTableModule.Enabled = false;
                    timerInputHandlerModule.Enabled = false;
                    timerOutputHandlerModule.Enabled = false;
                    timerTestProbeHandlerModule.Enabled = false;
                    timerPrecisorNestModule.Enabled = false;
                    timerOutputTurnTableModule.Enabled = false;
                    timerConveyorInhibit.Enabled = false;
                }

                if (tabControlOperationStatus.SelectedIndex == 0)//Seaveyor page
                {
                    timerConveyorPower.Enabled = true;
                }
                else
                {
                    timerConveyorPower.Enabled = false;
                }
            }
            tabControlOperationStatus.Focus();
        }

        private void timerConveyorPower_Tick(object sender, EventArgs e)
        {
            ledEnableConveyorPower.State = _workcell.Process.MonitorProcess.Controller.isConveyorEnabled();
            ledEnableConveyorPower.Enabled = !ledEnableConveyorPower.State;

            if (ledEnableConveyorPower.State)
               CommonFunctions.Instance.powerOnConveyor = false;

            ledDisableConveyorPower.State = !_workcell.Process.MonitorProcess.Controller.isConveyorEnabled();
            ledDisableConveyorPower.Enabled = !ledDisableConveyorPower.State;

            if (ledDisableConveyorPower.State)
                CommonFunctions.Instance.powerOffConveyor = false;

            ledDigitalInputCarrierClampSensor.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Carrier_Clamp_Sensor)) == DigitalIOState.On ? true : false;
            ledDigitalOutputCarrierClampSensor.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Output_Carrier_Clamp_Sensor)) == DigitalIOState.On ? true : false;
        }

        private void ledEnableConveyorPower_Click(object sender, EventArgs e)
        {
            if(!ledEnableConveyorPower.State)
            {
                CommonFunctions.Instance.powerOnConveyor = true;
                CommonFunctions.Instance.powerOffConveyor = false;
            }
        }

        private void ledDisableConveyorPower_Click(object sender, EventArgs e)
        {
            if (!ledDisableConveyorPower.State)
            {
                if ("Yes" == Notify.PopUp("Power OFF the conveyor?",
                                        "Are you sure you want to power OFF the conveyor?", "Confirm Power OFF conveyor", "Yes", "Cancel"))
                {
                    CommonFunctions.Instance.powerOffConveyor = true;
                    CommonFunctions.Instance.powerOnConveyor = false;
                }
            }
        }

        private void ledInputConveyorInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputConveyor(!ledInputConveyorInhibit.State);
        }

        private void ledInputTurnStationInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputTurnStationProcess.Controller.InhibitInputTurnStation(!ledInputTurnStationInhibit.State);
        }

        private void ledInputStationInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputStationProcess.Controller.InhibitInputStation(!ledInputStationInhibit.State);
        }

        private void ledBufferStationInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.InputStationProcess.Controller.InhibitBufferStation(!ledBufferStationInhibit.State);
        }

        private void ledOutputStationInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputStationProcess.Controller.InhibitOutputStation(!ledOutputStationInhibit.State);
        }

        private void ledOutputTurnStationInhibit_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.Process.OutputTurnStationProcess.Controller.InhibitOutputTurnStation(!ledOutputTurnStationInhibit.State);
        }

        private void timerConveyorInhibit_Tick(object sender, EventArgs e)
        {
            try
            {
                ledInputConveyorInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Conveyor_Inhibit)) == DigitalIOState.On ? true : false;
                ledInputTurnStationInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Turn_Station_Inhibit)) == DigitalIOState.On ? true : false;
                ledInputStationInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.CIS_Inhibit)) == DigitalIOState.On ? true : false;
                ledBufferStationInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.BBZ_Inhibit)) == DigitalIOState.On ? true : false;
                ledOutputStationInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.COS_Inhibit)) == DigitalIOState.On ? true : false;
                ledOutputTurnStationInhibit.State = HSTMachine.Workcell._a3200HC.GetState(HSTMachine.Workcell._ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Output_Turn_Station_Inhibit)) == DigitalIOState.On ? true : false;
            }
            catch (Exception ex)
            {
                // do nothing
            }
        }

        private void setAxesState()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
            {
                try
                {
                    if (_workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() < (_workcell.TeachPointRecipe.InputEESafeHeight + 5))
                        HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Parked;
                    else if (_workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() > (_workcell.TeachPointRecipe.InputEEPickHeight - 5) &&
                        _workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() < (_workcell.TeachPointRecipe.InputEEPickHeight + 5))
                        HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Pick;
                    else if (_workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() > (_workcell.TeachPointRecipe.InputEEPlaceHeight_UpTab - 5) &&
                        _workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() < (_workcell.TeachPointRecipe.InputEEPlaceHeight_UpTab + 5))
                        HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Place;
                    else if (_workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() > (_workcell.TeachPointRecipe.InputEEPlaceHeight_DownTab - 5) &&
                        _workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() < (_workcell.TeachPointRecipe.InputEEPlaceHeight_DownTab + 5))
                        HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.Place;
                    else if (_workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() > (_workcell.TeachPointRecipe.InputEEDycemHeight - 5) &&
                        _workcell.Process.InputEEProcess.Controller.GetInputEEPositionZ() < (_workcell.TeachPointRecipe.InputEEDycemHeight + 5))
                        HSTMachine.Workcell.InputEEZAxisPosition = InputEEZAxis.DycemCleaning;

                    if (_workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() < (_workcell.TeachPointRecipe.OutputEESafeHeight + 5))
                        HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Parked;
                    else if (_workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() > (_workcell.TeachPointRecipe.OutputEEPickHeight_UpTab - 5) &&
                        _workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() < (_workcell.TeachPointRecipe.OutputEEPickHeight_UpTab + 5))
                        HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
                    else if (_workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() > (_workcell.TeachPointRecipe.OutputEEPickHeight_DownTab - 5) &&
                        _workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() < (_workcell.TeachPointRecipe.OutputEEPickHeight_DownTab + 5))
                        HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Pick;
                    else if (_workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() > (_workcell.TeachPointRecipe.OutputEEPlaceHeight - 5) &&
                        _workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() < (_workcell.TeachPointRecipe.OutputEEPlaceHeight + 5))
                        HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.Place;
                    else if (_workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() > (_workcell.TeachPointRecipe.OutputEEDycemHeight - 5) &&
                        _workcell.Process.OutputEEProcess.Controller.GetOutputEEPositionZ() < (_workcell.TeachPointRecipe.OutputEEDycemHeight + 5))
                        HSTMachine.Workcell.OutputEEZAxisPosition = OutputEEZAxis.DycemCleaning;

                    if (_workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ() < (_workcell.TeachPointRecipe.TestProbeSafeHeight + 5))
                        HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Parked;
                    else if (_workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ() > (_workcell.TeachPointRecipe.TestProbeTestHeight_UpTab - 5) &&
                        _workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ() < (_workcell.TeachPointRecipe.TestProbeTestHeight_UpTab + 5))
                        HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Test;
                    else if (_workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ() > (_workcell.TeachPointRecipe.TestProbeTestHeight_DownTab - 5) &&
                        _workcell.Process.TestProbeProcess.Controller.GetTestProbePositionZ() < (_workcell.TeachPointRecipe.TestProbeTestHeight_DownTab + 5))
                        HSTMachine.Workcell.TestProbeZAxisPosition = TestProbeZAxis.Test;

                    if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorInputStationPositionX_UpTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorInputStationPositionX_UpTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtInputStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorInputStationPositionX_DownTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorInputStationPositionX_DownTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtInputStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorTestStationPositionX_UpTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorTestStationPositionX_UpTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtPrecisorStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorTestStationPositionX_DownTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorTestStationPositionX_DownTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtPrecisorStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorOutputStationPositionX_UpTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorOutputStationPositionX_UpTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtOutputStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorOutputStationPositionX_DownTab - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorOutputStationPositionX_DownTab + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.AtOutputStation;
                    else if (_workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() > (_workcell.TeachPointRecipe.PrecisorSafePositionX - 5) &&
                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX() < (_workcell.TeachPointRecipe.PrecisorSafePositionX + 5))
                        HSTMachine.Workcell.PrecisorNestXAxisPosition = PrecisorNestXAxis.Parked;
                }
                catch(Exception ex)
                {
                    // do nothing
                }
            }
        }

        private void ledFlattenerExtend_Click(object sender, EventArgs e)
        {
            if (ledFlattenerExtend.State)
                return;

            HSTMachine.Workcell.Process.InputEEProcess.Controller.ExtendInputEEFlattener();

        }

        private void ledFlattenerRetract_Click(object sender, EventArgs e)
        {
            if (ledFlattenerRetract.State)
                return;

            HSTMachine.Workcell.Process.InputEEProcess.Controller.RetractInputEEFlattener();
        }

        private void OperationStatus_Load(object sender, EventArgs e)
        {

        }
    }
}