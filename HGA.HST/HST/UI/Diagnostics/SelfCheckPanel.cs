using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Models;
using Seagate.AAS.HGA.HST.Vision;

namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    public partial class SelfCheckPanel : UserControl
    {
        public enum CheckComponents
        {
            TestElectronics,
            LCRMeter
        }    

        private System.Timers.Timer testElectronicsCommunicationTimer;
        private DateTime startTrackingEvent = new DateTime();
        private CheckComponents checkComponent;

        public SelfCheckPanel()
        {
            InitializeComponent();

            testElectronicsCommunicationTimer = new System.Timers.Timer();
            testElectronicsCommunicationTimer.Interval = 1000;
            testElectronicsCommunicationTimer.Elapsed += new System.Timers.ElapsedEventHandler(updateTimer_Elapsed);                       
        }

        private void buttonCheckInputCamera_Click(object sender, EventArgs e)
        {
            textBoxInputCameraStatus.Text = "";

            Camera _inputCamera = HSTMachine.Workcell.Process.InputEEProcess.Controller.InputCamera;

            if (_inputCamera!= null)
            {
                if (_inputCamera.GrabManual(true))
                {
                    textBoxInputCameraStatus.ForeColor = Color.Red;
                    textBoxInputCameraStatus.Text = "OK. Able to grap image.";
                }
                else
                {
                    textBoxInputCameraStatus.ForeColor = Color.Red;
                    textBoxInputCameraStatus.Text = "Fail to grab image.";
                }
            }
            else
            {
                textBoxInputCameraStatus.ForeColor = Color.Red;
                textBoxInputCameraStatus.Text = "Fail to get camera object";
            }
        }

        private void buttonCheckOutputCamera_Click(object sender, EventArgs e)
        {
            textBoxOutputCameraStatus.Text = "";

            Camera _outputCamera = HSTMachine.Workcell.Process.OutputEEProcess.Controller.OutputCamera;

            if (_outputCamera != null)
            {
                if (_outputCamera.GrabManual(true))
                {
                    textBoxOutputCameraStatus.ForeColor = Color.Green;
                    textBoxOutputCameraStatus.Text = "OK. Able to grab image.";
                }
                else
                {
                    textBoxOutputCameraStatus.ForeColor = Color.Red;
                    textBoxOutputCameraStatus.Text = "Fail to grab image.";
                }
            }
            else
            {
                textBoxOutputCameraStatus.ForeColor = Color.Red;
                textBoxOutputCameraStatus.Text = "Fail to get camera object";
            }
        }

        private void buttonCheckElectronicsBoard_Click(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent -= new EventHandler(DisplayElectronicsBoardStatus);
            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent += new EventHandler(DisplayElectronicsBoardStatus);

            textBoxElectronicsBoardStatus.Text = "";
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_status_Message_ID, TestProbeAPICommand.HST_get_status_Message_Name, TestProbeAPICommand.HST_get_status_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
            testElectronicsCommunicationTimer.Start();
            startTrackingEvent = DateTime.Now;
            buttonCheckElectronicsBoard.Enabled = false;
            buttonCheckLCRMeter.Enabled = false;
            checkComponent = CheckComponents.TestElectronics;
        }

        private void buttonCheckLCRMeter_Click(object sender, EventArgs e)
        {
        /* // No LCR in photodiode feature

            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent -= new EventHandler(DisplayLCRMeterStatus);
            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent += new EventHandler(DisplayLCRMeterStatus);

            textBoxLCRMeterStatus.Text = "";
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe3ConfigCapacitanceMeasurement);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_cap_meas_Message_ID, TestProbeAPICommand.HST_config_cap_meas_Message_Name, TestProbeAPICommand.HST_config_cap_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
            testElectronicsCommunicationTimer.Start();
            startTrackingEvent = DateTime.Now;
            buttonCheckElectronicsBoard.Enabled = false;
            buttonCheckLCRMeter.Enabled = false;
            checkComponent = CheckComponents.LCRMeter;*/
        }

        private void DisplayElectronicsBoardStatus(object sender, EventArgs e)
        {
            TestElectronicsStatusEventArgs testElectronicsStatusEventArgs = e as TestElectronicsStatusEventArgs;

            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent -= new EventHandler(DisplayElectronicsBoardStatus);
            
            UIUtility.Invoke(this, () =>
            {       
                this.textBoxElectronicsBoardStatus.ForeColor = (testElectronicsStatusEventArgs.TestElectronicsStatus.Contains("Ready")) ? Color.Green : Color.Red;
                this.textBoxElectronicsBoardStatus.Text = testElectronicsStatusEventArgs.TestElectronicsStatus;
                this.buttonCheckElectronicsBoard.Enabled = true;
                this.buttonCheckLCRMeter.Enabled = true;
                this.testElectronicsCommunicationTimer.Stop();
            });
        }

        private void DisplayLCRMeterStatus(object sender, EventArgs e)
        {
            TestElectronicsStatusEventArgs testElectronicsStatusEventArgs = e as TestElectronicsStatusEventArgs;

            HSTMachine.Instance.MainForm.TestElectronicsStatusEvent -= new EventHandler(DisplayLCRMeterStatus);

            UIUtility.Invoke(this, () =>
            {
                this.textBoxLCRMeterStatus.ForeColor = (testElectronicsStatusEventArgs.TestElectronicsStatus.Contains("Ready")) ? Color.Green : Color.Red;
                this.textBoxLCRMeterStatus.Text = testElectronicsStatusEventArgs.TestElectronicsStatus;
                this.buttonCheckElectronicsBoard.Enabled = true;
                this.buttonCheckLCRMeter.Enabled = true;
                this.testElectronicsCommunicationTimer.Stop();
            });
        }

        private void DisplayFirmwareStatus(object sender, EventArgs e)
        {
            TestFirmwareEventArgs testFirmwareEventArgs = e as TestFirmwareEventArgs;

            HSTMachine.Instance.MainForm.TestFirmwareVersionEvent -= new EventHandler(DisplayFirmwareStatus);

            UIUtility.Invoke(this, () =>
            {
                this.textBoxFirmwareVersion.Text = testFirmwareEventArgs.TestStatus;
            });
        }

        private void SelfCheckPanel_Enter(object sender, EventArgs e)
        {
            textBoxInputCameraStatus.Text = "";
            textBoxOutputCameraStatus.Text = "";
            textBoxElectronicsBoardStatus.Text = "";
            textBoxLCRMeterStatus.Text = "";
        }

        void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan totalTime = DateTime.Now - startTrackingEvent;
            if (totalTime.TotalSeconds > 2)
            {
                testElectronicsCommunicationTimer.Stop();

                UIUtility.Invoke(this, () =>
                {
                    if (checkComponent == CheckComponents.TestElectronics)
                        this.textBoxElectronicsBoardStatus.Text = "Communication Timeout";
                    else if (checkComponent == CheckComponents.LCRMeter)
                        this.textBoxLCRMeterStatus.Text = "Communication Timeout";
                    this.buttonCheckElectronicsBoard.Enabled = true;
                    this.buttonCheckLCRMeter.Enabled = true;
                });
            }
        }

        public void checkFirmwareVersion_Click(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.TestFirmwareVersionEvent += new EventHandler(DisplayFirmwareStatus);
            textBoxFirmwareVersion.Text = "";
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_firmware_version_Message_ID, TestProbeAPICommand.HST_get_firmware_version_Message_Name, TestProbeAPICommand.HST_get_firmware_version_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);

        }

    }
}
