using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Globalization;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using DesktopTester.Data;
using DesktopTester.Data.IncomingTestProbeData;
using DesktopTester.Data.OutgoingTestProbeData;
using DesktopTester.Utils;

namespace DesktopTester.UI
{
    public partial class frmMain
    {
        private void tabPageServoCalibration_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;            
            if (IsServoCalibrationTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }
            System.Drawing.Graphics tabPageServoCalibrationGraphics = tabPageServoCalibration.CreateGraphics();
            tabPageServoCalibrationGraphics.FillEllipse(myBrush, new Rectangle(930, 2, 40, 40));
            myBrush.Dispose();
            tabPageServoCalibrationGraphics.Dispose();

            System.Drawing.SolidBrush myDesktopTesterConnectionStatusBrush;
            if (IsConnectedToDesktopTester == true)
            {
                myDesktopTesterConnectionStatusBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
                enableAllButtonsOnServoCalibrationPage();
            }
            else
            {
                myDesktopTesterConnectionStatusBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                disableAllButtonsOnServoCalibrationPage();                
            }
            System.Drawing.Graphics tabPageServoCalibrationConnectionStatusGraphics = tabPageServoCalibration.CreateGraphics();
            tabPageServoCalibrationConnectionStatusGraphics.FillEllipse(myDesktopTesterConnectionStatusBrush, new Rectangle(250, 2, 40, 40));
            myDesktopTesterConnectionStatusBrush.Dispose();
            tabPageServoCalibrationConnectionStatusGraphics.Dispose();            
        }

        private void disableAllButtonsOnServoCalibrationPage()
        {
            btnServoCalibrationAllMoveToStandbyPosition.Enabled = false;
            btnServoCalibrationDesktopTesterConnect.Enabled = true;
            btnServoCalibrationDesktopTesterDisconnect.Enabled = false;
            btnServoCalibrationDownTabOff.Enabled = false;
            btnServoCalibrationDownTabOn.Enabled = false;
            btnServoCalibrationFlattenerExtend1.Enabled = false;
            btnServoCalibrationFlattenerRetract1.Enabled = false;
            btnServoCalibrationFlattenerExtend2.Enabled = false;
            btnServoCalibrationFlattenerRetract2.Enabled = false;
            btnServoCalibrationGetAllCurrentPositions.Enabled = false;
            btnServoCalibrationGetAllInputs.Enabled = false;
            btnServoCalibrationHomeAll.Enabled = false;
            btnServoCalibrationHomePrecisor.Enabled = false;
            btnServoCalibrationHomeTestProbe.Enabled = false;
            btnServoCalibrationJogCCW.Enabled = false;
            btnServoCalibrationJogCW.Enabled = false;
            btnServoCalibrationJogDown.Enabled = false;
            btnServoCalibrationJogToInside.Enabled = false;
            btnServoCalibrationJogToLeft.Enabled = false;
            btnServoCalibrationJogToOutside.Enabled = false;
            btnServoCalibrationJogToRight.Enabled = false;
            btnServoCalibrationJogUp.Enabled = false;
            btnServoCalibrationLoadHGAs.Enabled = false;
            btnServoCalibrationMovePrecisorToFlatDownTabPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToFlatUpTabPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToLoadUnloadPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToStandbyPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToTargetPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToTestDownTabPosition.Enabled = false;
            btnServoCalibrationMovePrecisorToTestUpTabPosition.Enabled = false;
            btnServoCalibrationMoveProbeToDownTabPosition.Enabled = false;
            btnServoCalibrationMoveProbeToStandbyPosition.Enabled = false;
            btnServoCalibrationMoveProbeToTargetPosition.Enabled = false;
            btnServoCalibrationMoveProbeToUpTabPosition.Enabled = false;
            btnServoCalibrationEjectHGAs.Enabled = false;
            btnServoCalibrationSaveAndDownloadCalibrationData.Enabled = false;
            btnServoCalibrationServoOff.Enabled = false;
            btnServoCalibrationTeachPrecisorFlatDownTabPosition.Enabled = false;
            btnServoCalibrationTeachPrecisorFlatUpTabPosition.Enabled = false;
            btnServoCalibrationTeachPrecisorLoadUnloadPosition.Enabled = false;
            btnServoCalibrationTeachPrecisorStandbyPosition.Enabled = false;
            btnServoCalibrationTeachPrecisorTestDownTabPosition.Enabled = false;
            btnServoCalibrationTeachPrecisorTestUpTabPosition.Enabled = false;
            btnServoCalibrationTeachProbeDownTabPosition.Enabled = false;
            btnServoCalibrationTeachProbeStandbyPosition.Enabled = false;
            btnServoCalibrationTeachProbeUpTabPosition.Enabled = false;
            btnServoCalibrationUpTabOff.Enabled = false;
            btnServoCalibrationUpTabOn.Enabled = false;
        }

        private void enableAllButtonsOnServoCalibrationPage()
        {
            btnServoCalibrationAllMoveToStandbyPosition.Enabled = true;
            btnServoCalibrationDesktopTesterConnect.Enabled = true;
            btnServoCalibrationDesktopTesterDisconnect.Enabled = true;
            btnServoCalibrationDownTabOff.Enabled = true;
            btnServoCalibrationDownTabOn.Enabled = true;
            btnServoCalibrationFlattenerExtend1.Enabled = true;
            btnServoCalibrationFlattenerRetract1.Enabled = true;
            btnServoCalibrationFlattenerExtend2.Enabled = true;
            btnServoCalibrationFlattenerRetract2.Enabled = true;
            btnServoCalibrationGetAllCurrentPositions.Enabled = true;
            btnServoCalibrationGetAllInputs.Enabled = true;
            btnServoCalibrationHomeAll.Enabled = true;
            btnServoCalibrationHomePrecisor.Enabled = true;
            btnServoCalibrationHomeTestProbe.Enabled = true;
            btnServoCalibrationJogCCW.Enabled = true;
            btnServoCalibrationJogCW.Enabled = true;
            btnServoCalibrationJogDown.Enabled = true;
            btnServoCalibrationJogToInside.Enabled = true;
            btnServoCalibrationJogToLeft.Enabled = true;
            btnServoCalibrationJogToOutside.Enabled = true;
            btnServoCalibrationJogToRight.Enabled = true;
            btnServoCalibrationJogUp.Enabled = true;
            btnServoCalibrationLoadHGAs.Enabled = true;
            btnServoCalibrationMovePrecisorToFlatDownTabPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToFlatUpTabPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToLoadUnloadPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToStandbyPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToTargetPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToTestDownTabPosition.Enabled = true;
            btnServoCalibrationMovePrecisorToTestUpTabPosition.Enabled = true;
            btnServoCalibrationMoveProbeToDownTabPosition.Enabled = true;
            btnServoCalibrationMoveProbeToStandbyPosition.Enabled = true;
            btnServoCalibrationMoveProbeToTargetPosition.Enabled = true;
            btnServoCalibrationMoveProbeToUpTabPosition.Enabled = true;
            btnServoCalibrationEjectHGAs.Enabled = true;
            btnServoCalibrationSaveAndDownloadCalibrationData.Enabled = true;
            btnServoCalibrationServoOff.Enabled = true;
            btnServoCalibrationTeachPrecisorFlatDownTabPosition.Enabled = true;
            btnServoCalibrationTeachPrecisorFlatUpTabPosition.Enabled = true;
            btnServoCalibrationTeachPrecisorLoadUnloadPosition.Enabled = true;
            btnServoCalibrationTeachPrecisorStandbyPosition.Enabled = true;
            btnServoCalibrationTeachPrecisorTestDownTabPosition.Enabled = true;
            btnServoCalibrationTeachPrecisorTestUpTabPosition.Enabled = true;
            btnServoCalibrationTeachProbeDownTabPosition.Enabled = true;
            btnServoCalibrationTeachProbeStandbyPosition.Enabled = true;
            btnServoCalibrationTeachProbeUpTabPosition.Enabled = true;
            btnServoCalibrationUpTabOff.Enabled = true;
            btnServoCalibrationUpTabOn.Enabled = true;
        }

        private void btnServoCalibrationGetAllInputs_Click(object sender, EventArgs e)
        {            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_get_all_inputs_Message_ID, TestProbeAPICommand.HST_desktop_tester_get_all_inputs_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationEjectHGAs_Click(object sender, EventArgs e)
        {           
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_ID, TestProbeAPICommand.HST_desktop_tester_eject_hga_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationLoadHGAs_Click(object sender, EventArgs e)
        {
            if (String.Compare(cboServoCalibrationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe91SetDesktopLoadHGA.UpDownTabIndex = 1;
            }
            else
            {
                TestProbe91SetDesktopLoadHGA.UpDownTabIndex = 2;
            }
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe91SetDesktopLoadHGA);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_load_hga_Message_ID, TestProbeAPICommand.HST_desktop_tester_load_hga_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);            

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMoveProbeToDownTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Down;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 9;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMoveProbeToUpTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Up;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 8;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMoveProbeToStandbyPosition_Click(object sender, EventArgs e)
        {
            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToTestDownTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Down;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 7;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToTestUpTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Up;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 6;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToFlatDownTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Down;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 5;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToFlatUpTabPosition_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Up;

            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 4;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToLoadUnloadPosition_Click(object sender, EventArgs e)
        {
            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 3;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToStandbyPosition_Click(object sender, EventArgs e)
        {
            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachProbeDownTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.Z2;
            TabType = UpOrDownTab.Down;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 9;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachProbeUpTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.Z2;
            TabType = UpOrDownTab.Up;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 8;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachProbeStandbyPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.Z2;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorTestDownTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;
            TabType = UpOrDownTab.Down;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 7;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorTestUpTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;
            TabType = UpOrDownTab.Up;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 6;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorFlatDownTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;
            TabType = UpOrDownTab.Down;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 5;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorFlatUpTabPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;
            TabType = UpOrDownTab.Up;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 4;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorLoadUnloadPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 3;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationTeachPrecisorStandbyPosition_Click(object sender, EventArgs e)
        {
            PrecisorXOrTestProbeZCommand = XOrZ2Command.X;

            TestProbe86SetDesktopTesterServoTeachPosition.TaughtPositionIndex = 1;           
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe86SetDesktopTesterServoTeachPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_teach_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationFlattenerRetract1_Click(object sender, EventArgs e)
        {
            TestProbe89SetDesktopTesterOutput.OutputIndex = 3;
            TestProbe89SetDesktopTesterOutput.OnOff = 0;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationFlattenerExtend1_Click(object sender, EventArgs e)
        {
            TestProbe89SetDesktopTesterOutput.OutputIndex = 3;
            TestProbe89SetDesktopTesterOutput.OnOff = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationHomeTestProbe_Click(object sender, EventArgs e)
        {
            TestProbe81SetDesktopTesterHomingAxis.ServoAxis = 4;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe81SetDesktopTesterHomingAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMoveProbeToTargetPosition_Click(object sender, EventArgs e)
        {
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ServoAxis = 4;

            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtServoCalibrationTestProbeZTargetPosition.Text) ? 0 : int.Parse(txtServoCalibrationTestProbeZTargetPosition.Text));
            TestProbe83SetDesktopTesterServoAbsMoveAxis.Z2AbsPositionLSBLow = ByteArray[0];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.Z2AbsPositionLSBMid = ByteArray[1];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.Z2AbsPositionMSBLow = ByteArray[2];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.Z2AbsPositionMSBHigh = ByteArray[3];
            
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe83SetDesktopTesterServoAbsMoveAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_servo_abs_move_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_servo_abs_move_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
        
        private void btnServoCalibrationJogDown_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            btnServoCalibrationJogUp_MouseUp(sender, e);
        }

        private void btnServoCalibrationJogDown_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 4;
            TestProbe94SetDesktopJog.JogDirection = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogUp_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 4;
            TestProbe94SetDesktopJog.JogDirection = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogUp_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe95SetDesktopJogStop.ServoAxisIndex = 4;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe95SetDesktopJogStop);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
        
        private void btnServoCalibrationDownTabOn_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Down;

            TestProbe89SetDesktopTesterOutput.OutputIndex = 2;
            TestProbe89SetDesktopTesterOutput.OnOff = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationDownTabOff_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Down;

            TestProbe89SetDesktopTesterOutput.OutputIndex = 2;
            TestProbe89SetDesktopTesterOutput.OnOff = 0;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationUpTabOn_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Up;

            TestProbe89SetDesktopTesterOutput.OutputIndex = 1;
            TestProbe89SetDesktopTesterOutput.OnOff = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationUpTabOff_Click(object sender, EventArgs e)
        {
            TabType = UpOrDownTab.Up;

            TestProbe89SetDesktopTesterOutput.OutputIndex = 1;
            TestProbe89SetDesktopTesterOutput.OnOff = 0;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationHomePrecisor_Click(object sender, EventArgs e)
        {
            TestProbe81SetDesktopTesterHomingAxis.ServoAxis = 5;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe81SetDesktopTesterHomingAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationMovePrecisorToTargetPosition_Click(object sender, EventArgs e)
        {
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ServoAxis = 5;

            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtServoCalibrationPrecisorActuatorsXTargetPosition.Text) ? 0 : int.Parse(txtServoCalibrationPrecisorActuatorsXTargetPosition.Text));
            TestProbe83SetDesktopTesterServoAbsMoveAxis.XAbsPositionLSBLow = ByteArray[0];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.XAbsPositionLSBMid = ByteArray[1];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.XAbsPositionMSBLow = ByteArray[2];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.XAbsPositionMSBHigh = ByteArray[3];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtServoCalibrationPrecisorActuatorsYTargetPosition.Text) ? 0 : int.Parse(txtServoCalibrationPrecisorActuatorsYTargetPosition.Text));
            TestProbe83SetDesktopTesterServoAbsMoveAxis.YAbsPositionLSBLow = ByteArray[0];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.YAbsPositionLSBMid = ByteArray[1];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.YAbsPositionMSBLow = ByteArray[2];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.YAbsPositionMSBHigh = ByteArray[3];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtServoCalibrationPrecisorActuatorsThetaTargetPosition.Text) ? 0 : int.Parse(txtServoCalibrationPrecisorActuatorsThetaTargetPosition.Text));
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ThetaAbsPositionLSBLow = ByteArray[0];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ThetaAbsPositionLSBMid = ByteArray[1];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ThetaAbsPositionMSBLow = ByteArray[2];
            TestProbe83SetDesktopTesterServoAbsMoveAxis.ThetaAbsPositionMSBHigh = ByteArray[3];

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe83SetDesktopTesterServoAbsMoveAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_servo_abs_move_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_servo_abs_move_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
        
        private void btnServoCalibrationJogCCW_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            btnServoCalibrationJogCW_MouseUp(sender, e);
        }

        private void btnServoCalibrationJogCCW_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 3;
            TestProbe94SetDesktopJog.JogDirection = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogCW_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 3;
            TestProbe94SetDesktopJog.JogDirection = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogCW_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe95SetDesktopJogStop.ServoAxisIndex = 3;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe95SetDesktopJogStop);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }    

        private void btnServoCalibrationJogToOutside_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            btnServoCalibrationJogToInside_MouseUp(sender, e);
        }
       
        private void btnServoCalibrationJogToOutside_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 2;
            TestProbe94SetDesktopJog.JogDirection = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogToInside_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 2;
            TestProbe94SetDesktopJog.JogDirection = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogToInside_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe95SetDesktopJogStop.ServoAxisIndex = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe95SetDesktopJogStop);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }      

        private void btnServoCalibrationJogToRight_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            btnServoCalibrationJogToLeft_MouseUp(sender, e);
        }  
       
        private void btnServoCalibrationJogToRight_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 1;
            TestProbe94SetDesktopJog.JogDirection = 2;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }
        
		private void btnServoCalibrationJogToLeft_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe94SetDesktopJog.ServoAxisIndex = 1;
            TestProbe94SetDesktopJog.JogDirection = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe94SetDesktopJog);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            Thread.Sleep(300);
        }

        private void btnServoCalibrationJogToLeft_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbe95SetDesktopJogStop.ServoAxisIndex = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe95SetDesktopJogStop);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_ID, TestProbeAPICommand.HST_desktop_tester_jog_stop_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
        
        private void btnServoCalibrationSaveAndDownloadCalibrationData_Click(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_save_taught_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_save_taught_position_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationAllMoveToStandbyPosition_Click(object sender, EventArgs e)
        {
            TestProbe87SetDesktopTesterMoveToTaughtServoPosition.TaughtPositionIndex = 10;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe87SetDesktopTesterMoveToTaughtServoPosition);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_ID, TestProbeAPICommand.HST_desktop_tester_move_to_taught_servo_position_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationServoOff_Click(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_servo_off_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_servo_off_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationHomeAll_Click(object sender, EventArgs e)
        {
            TestProbe81SetDesktopTesterHomingAxis.ServoAxis = 255;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe81SetDesktopTesterHomingAxis);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_homing_axis_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationGetAllCurrentPositions_Click(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_get_servo_positions_Message_ID, TestProbeAPICommand.HST_desktop_tester_get_servo_positions_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationDesktopTesterDisconnect_Click(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_disconnect_Message_ID, TestProbeAPICommand.HST_desktop_tester_disconnect_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnServoCalibrationDesktopTesterConnect_Click(object sender, EventArgs e)
        {
            TestProbe80SetDesktopTesterConnect.PasswordLSB = 0xA5;
            TestProbe80SetDesktopTesterConnect.PasswordMSB = 0xA5;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe80SetDesktopTesterConnect);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_connect_Message_ID, TestProbeAPICommand.HST_desktop_tester_connect_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);            

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }
        
        private void btnServoCalibrationFlattenerExtend2_Click(object sender, EventArgs e)
        {
            TestProbe89SetDesktopTesterOutput.OutputIndex = 4;
            TestProbe89SetDesktopTesterOutput.OnOff = 1;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }        

        private void btnServoCalibrationFlattenerRetract2_Click(object sender, EventArgs e)
        {
            TestProbe89SetDesktopTesterOutput.OutputIndex = 4;
            TestProbe89SetDesktopTesterOutput.OnOff = 0;
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe89SetDesktopTesterOutput);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_desktop_tester_set_output_Message_ID, TestProbeAPICommand.HST_desktop_tester_set_output_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }        
	}
}