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

namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    public partial class IOControlPanel : UserControl
    {
        private IDigitalInput _di;
        private IDigitalOutput _do;
        private IAnalogInput _ai;

        PanelDigitalInput panelInput;

        public IOControlPanel()
        {
            InitializeComponent();

            ///////////////////////////////////////////////////////////////////////////////////////
            //Digital Input Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////
            //X axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(0);
            panelDigitalInputElectronicsOutput1.ledInput.Text = "X-I1 - "+ _di.ToString().Replace("_", " ");
            panelDigitalInputElectronicsOutput1.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(1);
            panelDigitalInputElectronicsOutput2.ledInput.Text = "X-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputElectronicsOutput2.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(2);
            panelDigitalInputElectronicsOutput3.ledInput.Text = "X-I3 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputElectronicsOutput3.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 2);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(3);
            panelDigitalInputElectronicsOutput4.ledInput.Text = "X-I4 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputElectronicsOutput4.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 3);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(4);
            panelDigitalInputOutputTurnStationExitClear.ledInput.Text = "X-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputTurnStationExitClear.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(5);
            panelDigitalInputOutputTurnStationAt90Deg.ledInput.Text = "X-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputTurnStationAt90Deg.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(6);
            panelDigitalInputOutputTurnStationAt0Deg.ledInput.Text = "X-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputTurnStationAt0Deg.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(7);
            panelDigitalInputInputConveyorPositionOn.ledInput.Text = "X-I8 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputConveyorPositionOn.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 7);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(8);
            panelDigitalInputGroundMaster.ledInput.Text = "X-I9 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputGroundMaster.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 8);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(9);
            panelDigitalInputVentilationFan1.ledInput.Text = "X-I10 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputVentilationFan1.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 9);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(10);
            panelDigitalInputVentilationFan2.ledInput.Text = "X-I11 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputVentilationFan2.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 10);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(11);
            panelDigitalInputVentilationFan3.ledInput.Text = "X-I12 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputVentilationFan3.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 11);

            //X-I12 to X-I24 is spare (not used)
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(20);
            panelDigitalInputCarrierClampInput.ledInput.Text = "X-I20 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputCarrierClampInput.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 20);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(21);
            panelDigitalInputCarrierClampOutput.ledInput.Text = "X-I21 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputCarrierClampOutput.AssignAxis(HSTMachine.Workcell._a3200HC, 0, 21);


            //Y axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(24);
            panelDigitalInputBISPositionOn.ledInput.Text = "Y-I1 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputBISPositionOn.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(25);
            panelDigitalInputBOSPositionOn.ledInput.Text = "Y-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputBOSPositionOn.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(26);
            panelDigitalInputBBZPositionOn.ledInput.Text = "Y-I3 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputBBZPositionOn.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 2);

            //Y-I4 is spare

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(28);
            panelDigitalInputInputTurnStationAt90Deg.ledInput.Text = "Y-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputTurnStationAt90Deg.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(29);
            panelDigitalInputInputTurnStationAt0Deg.ledInput.Text = "Y-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputTurnStationAt0Deg.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(30);
            panelDigitalInputInputTurnStationInPosition.ledInput.Text = "Y-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputTurnStationInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(31);
            panelDigitalInputOutputTurnStationInPosition.ledInput.Text = "Y-I8 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputTurnStationInPosition.AssignAxis(HSTMachine.Workcell._a3200HC, 1, 7);

            //Theta axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(32);
            panelDigitalInputInputStopperUp.ledInput.Text = "Theta-I1 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputStopperUp.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(33);
            panelDigitalInputInputStopperDown.ledInput.Text = "Theta-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputStopperDown.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(34);
            panelDigitalInputInputLifterUp.ledInput.Text = "Theta-I3 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputLifterUp.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 2);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(35);
            panelDigitalInputInputLifterDown.ledInput.Text = "Theta-I4 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputLifterDown.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 3);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(36);
            panelDigitalInputOutputStopperUp.ledInput.Text = "Theta-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputStopperUp.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(37);
            panelDigitalInputOutputStopperDown.ledInput.Text = "Theta-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputStopperDown.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(38);
            panelDigitalInputOutputLifterUp.ledInput.Text = "Theta-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputLifterUp.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(39);
            panelDigitalInputOutputLifterDown.ledInput.Text = "Theta-I8 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputLifterDown.AssignAxis(HSTMachine.Workcell._a3200HC, 2, 7);

            //Z1 axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(40);
            panelDigitalInputInputCSDeploy.ledInput.Text = "Z1-I1 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputCSDeploy.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(41);
            panelDigitalInputInputCSRetract.ledInput.Text = "Z1-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputCSRetract.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(42);
            panelDigitalInputInputCSLock.ledInput.Text = "Z1-I3 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputCSLock.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 2);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(43);
            panelDigitalInputInputCSUnlock.ledInput.Text = "Z1-I4 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputCSUnlock.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 3);


            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(44);
            panelDigitalInputOutputCSDeploy.ledInput.Text = "Z1-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputCSDeploy.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(45);
            panelDigitalInputOutputCSRetract.ledInput.Text = "Z1-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputCSRetract.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(46);
            panelDigitalInputOutputCSLock.ledInput.Text = "Z1-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputCSLock.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(47);
            panelDigitalInputOutputCSUnlock.ledInput.Text = "Z1-I8 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputCSUnlock.AssignAxis(HSTMachine.Workcell._a3200HC, 3, 7);

            //Z2 axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(48);
            panelDigitalInputPNVS1.ledInput.Text = "Z2-I1 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS1.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(49);
            panelDigitalInputPNVS2.ledInput.Text = "Z2-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS2.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(50);
            panelDigitalInputPNVS3.ledInput.Text = "Z2-I3 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS3.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 2);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(51);
            panelDigitalInputPNVS4.ledInput.Text = "Z2-I4 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS4.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 3);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(52);
            panelDigitalInputPNVS5.ledInput.Text = "Z2-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS5.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(53);
            panelDigitalInputPNVS6.ledInput.Text = "Z2-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS6.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(54);
            panelDigitalInputPNVS7.ledInput.Text = "Z2-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputPNVS7.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(55);
            panelDigitalInputEMOSenseInput.ledInput.Text = "Z2-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputEMOSenseInput.AssignAxis(HSTMachine.Workcell._a3200HC, 4, 7);

            //Z3 axis-input bank
            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(56);
            panelDigitalInputInputEEVS1.ledInput.Text = "Z3-I1 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputEEVS1.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 0);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(57);
            panelDigitalInputInputEEVS2.ledInput.Text = "Z3-I2 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputEEVS2.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 1);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(60);
            panelDigitalInputOutputEEVS1.ledInput.Text = "Z3-I5 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputEEVS1.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 4);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(61);
            panelDigitalInputOutputEEVS2.ledInput.Text = "Z3-I6 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputEEVS2.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 5);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(62);
            panelDigitalInputInputLifterCarrierSense.ledInput.Text = "Z3-I7 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputInputLifterCarrierSense.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 6);

            _di = HSTMachine.Workcell._ioManifest.GetDigitalInput(63);
            panelDigitalInputOutputLifterCarrierSense.ledInput.Text = "Z3-I8 - " + _di.ToString().Replace("_", " ");
            panelDigitalInputOutputLifterCarrierSense.AssignAxis(HSTMachine.Workcell._a3200HC, 5, 7);

            ///////////////////////////////////////////////////////////////////////////////////////
            //Digital Input Session : End
            ///////////////////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////////////////
            //Analog Input Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////
            _ai = HSTMachine.Workcell._ioManifest.GetAnalogInput(1);
            panelAnalogInputFRLSwitch.labelAnalogInputName.Text = _ai.ToString().Replace("_", " ");
            panelAnalogInputFRLSwitch.AssignAxis(HSTMachine.Workcell._a3200HC, _ai);
            
            //panelAnalogInput2.AssignAxis(HSTMachine.Workcell._a3200HC, _ai);
            ///////////////////////////////////////////////////////////////////////////////////////
            //Analog Input Session : End
            ///////////////////////////////////////////////////////////////////////////////////////

            
            ///////////////////////////////////////////////////////////////////////////////////////
            //Output Session : Start
            ///////////////////////////////////////////////////////////////////////////////////////
            //X axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(0);
            panelDigitalOutputInputEEVCH.ledOutput.Text = "X-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputEEVCH.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 0);
            
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(1);
            panelDigitalOutputInputEEPCH.ledOutput.Text = "X-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputEEPCH.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(2);
            panelDigitalOutputOutputEEVCH.ledOutput.Text = "X-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputEEVCH.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(3);
            panelDigitalOutputOutputEEPCH.ledOutput.Text = "X-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputEEPCH.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 3);

            //X-O5 to X-O8 is spare (not used)

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(8);
            panelDigitalOutputOutputCSDeploy.ledOutput.Text = "X-O9 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputCSDeploy.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 8);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(9);
            panelDigitalOutputOutputCSRotate.ledOutput.Text = "X-O10 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputCSRotate.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 9);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(10);
            panelDigitalOutputInputEEFlattener.ledOutput.Text = "X-O11 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputEEFlattener.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 10);

            //X-O11 not used anymore (was for Input EE flattener)

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(11);
            panelDigitalOutputInputConveyorInhibit.ledOutput.Text = "X-O12 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputConveyorInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 11);

            //X-O13 to X-O16 is spare (not used)
            
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(16);
            panelDigitalOutputDCServicingLight.ledOutput.Text = "X-O17 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputDCServicingLight.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 16);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(17);
            panelDigitalOutputCamera1.ledOutput.Text = "X-O18 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputCamera1.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 17);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(18);
            panelDigitalOutputCamera3.ledOutput.Text = "X-O19 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputCamera3.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 18);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(19);
            panelDigitalOutputCamera2.ledOutput.Text = "X-O20 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputCamera2.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 19);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(20);
            panelDigitalOutputElectronicInput1.ledOutput.Text = "X-O21 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputElectronicInput1.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 20);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(21);
            panelDigitalOutputElectronicInput2.ledOutput.Text = "X-O22 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputElectronicInput2.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 21);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(22);
            panelDigitalOutputElectronicInput3.ledOutput.Text = "X-O23 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputElectronicInput3.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 22);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(23);
            panelDigitalOutputElectronicInput4.ledOutput.Text = "X-O24 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputElectronicInput4.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0), 0, 23);

            //Y axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(24);
            panelDigitalOutputCISInhibit.ledOutput.Text = "Y-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputCISInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 0);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(25);
            panelDigitalOutputCOSInhibit.ledOutput.Text = "Y-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputCOSInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(26);
            panelDigitalOutputInputTurnStationInhibit.ledOutput.Text = "Y-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputTurnStationInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(27);
            panelDigitalOutputInputTurnStationTurnTo90Deg.ledOutput.Text = "Y-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputTurnStationTurnTo90Deg.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 3);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(28);
            panelDigitalOutputInputTurnStationTurnTo0Deg.ledOutput.Text = "Y-O5 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputTurnStationTurnTo0Deg.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 4);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(29);
            panelDigitalOutputOutputTurnStationInhibit.ledOutput.Text = "Y-O6 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputTurnStationInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 5);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(30);
            panelDigitalOutputOutputTurnStationTurnTo90Deg.ledOutput.Text = "Y-O7 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputTurnStationTurnTo90Deg.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 6);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(31);
            panelDigitalOutputOutputTurnStationTurnTo0Deg.ledOutput.Text = "Y-O8 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputTurnStationTurnTo0Deg.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1), 1, 7);

            //Theta axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(32);
            panelDigitalOutputInputStopper.ledOutput.Text = "Theta-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputStopper.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 0);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(33);
            panelDigitalOutputInputLifterUp.ledOutput.Text = "Theta-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputLifterUp.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(34);
            panelDigitalOutputInputLifterDown.ledOutput.Text = "Theta-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputLifterDown.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(35);
            panelDigitalOutputOutputStopper.ledOutput.Text = "Theta-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputStopper.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 3);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(36);
            panelDigitalOutputOutputLifterUp.ledOutput.Text = "Theta-O5 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputLifterUp.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 4);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(37);
            panelDigitalOutputOutputLifterDown.ledOutput.Text = "Theta-O6 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputOutputLifterDown.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 5);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(38);
            panelDigitalOutputInputCSDeploy.ledOutput.Text = "Theta-O7 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputCSDeploy.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 6);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(39);
            panelDigitalOutputInputCSRotate.ledOutput.Text = "Theta-O8 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputInputCSRotate.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2), 2, 7);

            //Z1 axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(40);
            panelDigitalOutputPNVTS1.ledOutput.Text = "Z1-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS1.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 0);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(41);
            panelDigitalOutputPNVTS2.ledOutput.Text = "Z1-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS2.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(42);
            panelDigitalOutputPNVTS3.ledOutput.Text = "Z1-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS3.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(43);
            panelDigitalOutputPNVTS4.ledOutput.Text = "Z1-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS4.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 3);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(44);
            panelDigitalOutputPNVTS5.ledOutput.Text = "Z1-O5 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS5.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 4);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(45);
            panelDigitalOutputPNVTS6.ledOutput.Text = "Z1-O6 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS6.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 5);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(46);
            panelDigitalOutputPNVTS7.ledOutput.Text = "Z1-O7 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVTS7.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 6);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(47);
            panelDigitalOutputSoftStartUp.ledOutput.Text = "Z1-O8 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputSoftStartUp.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3), 3, 7);

            //Z2 axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(48);
            panelDigitalOutputPNVCH1.ledOutput.Text = "Z2-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH1.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 0);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(49);
            panelDigitalOutputPNPCH1.ledOutput.Text = "Z2-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH1.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(50);
            panelDigitalOutputPNVCH2.ledOutput.Text = "Z2-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH2.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(51);
            panelDigitalOutputPNPCH2.ledOutput.Text = "Z2-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH2.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 3);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(52);
            panelDigitalOutputPNVCH3.ledOutput.Text = "Z2-O5 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH3.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 4);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(53);
            panelDigitalOutputPNPCH3.ledOutput.Text = "Z2-O6 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH3.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 5);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(54);
            panelDigitalOutputPNVCH4.ledOutput.Text = "Z2-O7 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH4.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 6);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(55);
            panelDigitalOutputPNPCH4.ledOutput.Text = "Z2-O8 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH4.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4), 4, 7);

            //Z3 axis-output bank
            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(56);
            panelDigitalOutputPNVCH5.ledOutput.Text = "Z3-O1 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH5.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 0);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(57);
            panelDigitalOutputPNPCH5.ledOutput.Text = "Z3-O2 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH5.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 1);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(58);
            panelDigitalOutputPNVCH6.ledOutput.Text = "Z3-O3 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH6.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 2);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(59);
            panelDigitalOutputPNPCH6.ledOutput.Text = "Z3-O4 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH6.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 3);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(60);
            panelDigitalOutputPNVCH7.ledOutput.Text = "Z3-O5 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNVCH7.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 4);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(61);
            panelDigitalOutputPNPCH7.ledOutput.Text = "Z3-O6 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputPNPCH7.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 5);

            _do = HSTMachine.Workcell._ioManifest.GetDigitalOutput(62);
            panelDigitalOutputBBZInhibit.ledOutput.Text = "Z3-O7 - " + _do.ToString().Replace("_", " ");
            panelDigitalOutputBBZInhibit.AssignAxis(HSTMachine.Workcell._a3200HC, HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5), 5, 6);

        }

        private void tabControlIOControl_VisibleChanged(object sender, EventArgs e)
        {
            if (HSTMachine.Instance.MainForm != null)
            {
                if (HSTMachine.Instance.MainForm.getPanelTitle().getHeaderUserAccess().getUserAccessSettings().getCurrentUser().Level != Seagate.AAS.HGA.HST.Settings.UserLevel.Operator)
                {
                    enablePanelDigitalOutput(true);
                }
                else
                {
                    enablePanelDigitalOutput(false);
                }
            }
        }

        private void enablePanelDigitalOutput(bool enable)
        {
            Control outputTabpage = this.tabControlIOControl.GetControl(this.tabControlIOControl.TabPages.IndexOf(this.tabPage2));

            foreach (Control panel in outputTabpage.Controls)
            {
                foreach (Control groupbox in panel.Controls)
                {
                    foreach (Control panelDigitalOutput in groupbox.Controls)
                    {
                        if (panelDigitalOutput is HGA.HST.UI.PanelDigitalOutput)
                        {
                            ((HGA.HST.UI.PanelDigitalOutput)panelDigitalOutput).Enabled = enable;
                        }

                    }
                }
            }
        }
    }
}
