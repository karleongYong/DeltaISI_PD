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
        private void tabPagePCBACalibration_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;
            if (IsPCBACalibrationTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }

            System.Drawing.Graphics tabPagePCBACalibrationGraphics = tabPagePCBACalibration.CreateGraphics();
            tabPagePCBACalibrationGraphics.FillEllipse(myBrush, new Rectangle(790, 120, 40, 40));
            myBrush.Dispose();
            tabPagePCBACalibrationGraphics.Dispose();
        }

        private void chkPCBACalibrationCalibrationEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPCBACalibrationCalibrationEnable.Checked == true)
            {
                //TestProbe17CalibrationEnable
                TestProbe17CalibrationEnable.EnableFlag = 1;                
            }
            else
            {
                //TestProbe17CalibrationEnable
                TestProbe17CalibrationEnable.EnableFlag = 0;               
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe17CalibrationEnable);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibration_enable_Message_ID, TestProbeAPICommand.HST_calibration_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            
            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void ManualCalibration(CalibrationType CalibrationType, int Value)
        {            
            if(chkPCBACalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "Command not executed because the 'Calibration Enable' check box is not checked.");
                return;
            }            

            int HGAIndex = 0;
            if(rdxPCBACalibrationManualCalibrationHGA1.Checked)
            {
                HGAIndex = 1;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA2.Checked)
            {
                HGAIndex = 2;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA3.Checked)
            {
                HGAIndex = 3;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA4.Checked)
            {
                HGAIndex = 4;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA5.Checked)
            {
                HGAIndex = 5;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA6.Checked)
            {
                HGAIndex = 6;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA7.Checked)
            {
                HGAIndex = 7;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA8.Checked)
            {
                HGAIndex = 8;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA9.Checked)
            {
                HGAIndex = 9;
            }
            else if (rdxPCBACalibrationManualCalibrationHGA10.Checked)
            {
                HGAIndex = 10;
            }

            int ChannelNumber = 0;
            if(rdxPCBACalibrationManualCalibrationCh1.Checked)
            {
                ChannelNumber = 1;
            }
            else if (rdxPCBACalibrationManualCalibrationCh2.Checked)
            {
                ChannelNumber = 2;
            }
            else if (rdxPCBACalibrationManualCalibrationCh3.Checked)
            {
                ChannelNumber = 3;
            }
            else if (rdxPCBACalibrationManualCalibrationCh4.Checked)
            {
                ChannelNumber = 4;
            }
            else if (rdxPCBACalibrationManualCalibrationCh5.Checked)
            {
                ChannelNumber = 5;
            }
            else if (rdxPCBACalibrationManualCalibrationCh6.Checked)
            {
                ChannelNumber = 6;
            }


            int ResistanceValue = 0;
            int CapacitanceValue = 0;
            byte[] ByteArrayValue;
            if(CalibrationType == CalibrationType.ResistanceCalibration)
            {
                ResistanceValue = Value * 1000;   // Change from Ohm to milliOhm
                ByteArrayValue = BitConverter.GetBytes(ResistanceValue);
            }
            else
            {
                CapacitanceValue = Value;
                ByteArrayValue = BitConverter.GetBytes(CapacitanceValue);
            }
            //TestProbe21SetManualCalibration
            TestProbe21SetManualCalibration.CalibrationType = Convert.ToByte(CalibrationType);
            TestProbe21SetManualCalibration.HGAIndex = Convert.ToByte(HGAIndex);
            TestProbe21SetManualCalibration.ChannelNumber = Convert.ToByte(ChannelNumber);
            TestProbe21SetManualCalibration.ExternalValueLSBLow = ByteArrayValue[0];
            TestProbe21SetManualCalibration.ExternalValueLSBMid = ByteArrayValue[1];
            TestProbe21SetManualCalibration.ExternalValueMSBLow = ByteArrayValue[2];
            TestProbe21SetManualCalibration.ExternalValueMSBHigh = ByteArrayValue[3];
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe21SetManualCalibration);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_manual_set_calibration_Message_ID, TestProbeAPICommand.HST_manual_set_calibration_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsPCBACalibrationTempered = true;
                tabPagePCBACalibration.Refresh();
            }
        }

        
        private void btnPCBACalibrationManualCalibration0Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "0R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 0);
        }
        
        private void btnPCBACalibrationManualCalibration10Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "10R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 10);
        }

        private void btnPCBACalibrationManualCalibration100Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "100R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 100);
        }

        private void btnPCBACalibrationManualCalibration500Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "500R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 500);
        }

        private void btnPCBACalibrationManualCalibration1000Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "1000R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 1000);
        }

        private void btnPCBACalibrationManualCalibration10000Ohm_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "10000R";
            ManualCalibration(CalibrationType.ResistanceCalibration, 10000);
        }
        
        private void btnPCBACalibrationManualCalibration100pF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "100C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 100);
        }

        private void btnPCBACalibrationManualCalibration270pF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "270C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 270);
        }

        private void btnPCBACalibrationManualCalibration470pF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "470C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 470);
        }

        private void btnPCBACalibrationManualCalibration680pF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "680C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 680);
        }

        private void btnPCBACalibrationManualCalibration820pF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "820C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 820);
        }

        private void btnPCBACalibrationManualCalibration10nF_CheckedChanged(object sender, EventArgs e)
        {
            CommonFunctions.Instance.strManualCalibrationClickedButton = "10000C";
            ManualCalibration(CalibrationType.CapacitanceCalibration, 10000);
        }

        private void TemperatureCalibration(int TemperatureValue)
        {
            if (chkPCBACalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "Command not executed because the 'Calibration Enable' check box is not checked.");
                return;
            }            
            
            int TemperatureChannel = 0;
            if (rdxPCBACalibrationTemperatureCalibrationCh1.Checked)
            {
                TemperatureChannel = 1;
            }
            else if (rdxPCBACalibrationTemperatureCalibrationCh2.Checked)
            {
                TemperatureChannel = 2;
            }
            else if (rdxPCBACalibrationTemperatureCalibrationCh3.Checked)
            {
                TemperatureChannel = 3;
            }

            //TestProbe31SetTemperatureCalibration
            TestProbe31SetTemperatureCalibration.ChannelNumber = Convert.ToByte(TemperatureChannel);
            TestProbe31SetTemperatureCalibration.Temperature = Convert.ToByte(TemperatureValue);
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe31SetTemperatureCalibration);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_temp_calibration_Message_ID, TestProbeAPICommand.HST_set_temp_calibration_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsPCBACalibrationTempered = true;
                tabPagePCBACalibration.Refresh();
            }
        }

        private void btnPCBACalibrationTemperatureCalibration5Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(5);
        }

        private void btnPCBACalibrationTemperatureCalibration10Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(10);
        }

        private void btnPCBACalibrationTemperatureCalibration15Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(15);
        }

        private void btnPCBACalibrationTemperatureCalibration20Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(20);
        }

        private void btnPCBACalibrationTemperatureCalibration25Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(25);
        }

        private void btnPCBACalibrationTemperatureCalibration30Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(30);
        }

        private void btnPCBACalibrationTemperatureCalibration35Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(35);
        }

        private void btnPCBACalibrationTemperatureCalibration40Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(40);
        }

        private void btnPCBACalibrationTemperatureCalibration45Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(45);
        }

        private void btnPCBACalibrationTemperatureCalibration50Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(50);
        }

        private void btnPCBACalibrationTemperatureCalibration55Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(55);
        }

        private void btnPCBACalibrationTemperatureCalibration60Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(60);
        }

        private void btnPCBACalibrationTemperatureCalibration65Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(65);
        }

        private void btnPCBACalibrationTemperatureCalibration70Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(70);
        }

        private void btnPCBACalibrationTemperatureCalibration75Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(75);
        }

        private void btnPCBACalibrationTemperatureCalibration80Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(80);
        }

        private void btnPCBACalibrationTemperatureCalibration85Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(85);
        }

        private void btnPCBACalibrationTemperatureCalibration90Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(90);
        }

        private void btnPCBACalibrationTemperatureCalibration95Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(95);
        }

        private void btnPCBACalibrationTemperatureCalibration100Celcius_CheckedChanged(object sender, EventArgs e)
        {
            TemperatureCalibration(100);
        }

        public void btnPCBACalibrationSaveCalibrationData_Click(object sender, EventArgs e)
        {
            if (chkPCBACalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "You need to first enable calibration before attempting to save the calibration data.");
                return;
            }
            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_save_calibration_data_Message_ID, TestProbeAPICommand.HST_save_calibration_data_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if(commandSentToMicroprocessor)
            { 
                IsPCBACalibrationTempered = false;
                tabPagePCBACalibration.Refresh();
                
                Notify.PopUp("Command Sent To uProcessor", "Calibration data saved.", null, "OK");
            }
        }

        private void btnPCBACalibrationGetCalibrationData_Click(object sender, EventArgs e)
        {   
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_calibration_data_Message_ID, TestProbeAPICommand.HST_get_calibration_data_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsPCBACalibrationTempered = false;
                tabPagePCBACalibration.Refresh();
            }        
        }

        private void btnPCBACalibrationClearDisplay_Click(object sender, EventArgs e)
        {
            //0Ohm
            txtPCBACalibration0Ch1.Text = "";
            txtPCBACalibration0Ch2.Text = "";
            txtPCBACalibration0Ch3.Text = "";
            txtPCBACalibration0Ch4.Text = "";
            txtPCBACalibration0Ch5.Text = "";
            txtPCBACalibration0Ch6.Text = "";

            //10Ohm
            txtPCBACalibration10Ch1.Text = "";
            txtPCBACalibration10Ch2.Text = "";
            txtPCBACalibration10Ch3.Text = "";
            txtPCBACalibration10Ch4.Text = "";
            txtPCBACalibration10Ch5.Text = "";
            txtPCBACalibration10Ch6.Text = "";

            //100Ohm
            txtPCBACalibration100Ch1.Text = "";
            txtPCBACalibration100Ch2.Text = "";
            txtPCBACalibration100Ch3.Text = "";
            txtPCBACalibration100Ch4.Text = "";
            txtPCBACalibration100Ch5.Text = "";
            txtPCBACalibration100Ch6.Text = "";

            //500Ohm
            txtPCBACalibration500Ch1.Text = "";
            txtPCBACalibration500Ch2.Text = "";
            txtPCBACalibration500Ch3.Text = "";
            txtPCBACalibration500Ch4.Text = "";
            txtPCBACalibration500Ch5.Text = "";
            txtPCBACalibration500Ch6.Text = "";

            //1000Ohm
            txtPCBACalibration1000Ch1.Text = "";
            txtPCBACalibration1000Ch2.Text = "";
            txtPCBACalibration1000Ch3.Text = "";
            txtPCBACalibration1000Ch4.Text = "";
            txtPCBACalibration1000Ch5.Text = "";
            txtPCBACalibration1000Ch6.Text = "";

            //10000Ohm
            txtPCBACalibration10000Ch1.Text = "";
            txtPCBACalibration10000Ch2.Text = "";
            txtPCBACalibration10000Ch3.Text = "";
            txtPCBACalibration10000Ch4.Text = "";
            txtPCBACalibration10000Ch5.Text = "";
            txtPCBACalibration10000Ch6.Text = "";

            txtPCBACalibration100Capa.Text = "";
            txtPCBACalibration270Capa.Text = "";
            txtPCBACalibration470Capa.Text = "";
            txtPCBACalibration680Capa.Text = "";
            txtPCBACalibration820Capa.Text = "";
            txtPCBACalibration10000Capa.Text = "";

            txtPCBACalibrationTemperatureCalibrationVoltage.Text = "";

            //HGA
            rdxPCBACalibrationManualCalibrationHGA1.Checked = true;
            rdxPCBACalibrationManualCalibrationHGA2.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA3.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA4.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA5.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA6.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA7.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA8.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA9.Checked = false;
            rdxPCBACalibrationManualCalibrationHGA10.Checked = false;

            //HGA channel
            rdxPCBACalibrationManualCalibrationCh1.Checked = true;
            rdxPCBACalibrationManualCalibrationCh2.Checked = false;
            rdxPCBACalibrationManualCalibrationCh3.Checked = false;
            rdxPCBACalibrationManualCalibrationCh4.Checked = false;
            rdxPCBACalibrationManualCalibrationCh5.Checked = false;
            rdxPCBACalibrationManualCalibrationCh6.Checked = false;

            //Temperature channel
            rdxPCBACalibrationTemperatureCalibrationCh1.Checked = true;
            rdxPCBACalibrationTemperatureCalibrationCh2.Checked = false;
            rdxPCBACalibrationTemperatureCalibrationCh3.Checked = false;

            IsPCBACalibrationTempered = false;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch1_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration0Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }   

        private void txtPCBACalibration100Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch1_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration270Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch1_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration100Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration470Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch1_Validated(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration500Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration680Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch1_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration1000Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration820Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch1_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch2_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch3_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch4_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch5_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Ch6_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibration10000Capa_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        private void txtPCBACalibrationTemperatureCalibrationVoltage_TextChanged(object sender, EventArgs e)
        {
            IsPCBACalibrationTempered = true;
            tabPagePCBACalibration.Refresh();
        }

        

        private void btnPCBACalibrationStartAutoCalibration_Click(object sender, EventArgs e)
        {
            if (chkPCBACalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "Command not executed because the 'Calibration Enable' check box is not checked.");
                return;
            }   
            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_auto_calibration_Message_ID, TestProbeAPICommand.HST_start_auto_calibration_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsPCBACalibrationTempered = true;
                tabPagePCBACalibration.Refresh();
            }            
        }

        private void btnPCBACalibrationManualCalibrationCalibrateOffset_CheckedChanged(object sender, EventArgs e)
        {            
            if (chkPCBACalibrationCalibrationEnable.Checked == false)
            {
                Notify.PopUpError("Calibration Disabled", "Command not executed because the 'Calibration Enable' check box is not checked.");
                return;
            }   

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibrate_offset_Message_ID, TestProbeAPICommand.HST_calibrate_offset_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            if (commandSentToMicroprocessor)
            {
                IsPCBACalibrationTempered = true;
                tabPagePCBACalibration.Refresh();
            }
        }

        private void btnPCBACalibrationManualCalibrationGetCalibrationOffset_CheckedChanged(object sender, EventArgs e)
        {                        
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_calibration_offset_Message_ID, TestProbeAPICommand.HST_get_calibration_offset_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnPCBACalibrationSetCh1TempOffset_CheckedChanged(object sender, EventArgs e)
        {
            if(int.Parse(txtPCBACalibrationCh1TemperatureOffset.Text) < -128 || int.Parse(txtPCBACalibrationCh1TemperatureOffset.Text) > 127)
            {
                Notify.PopUpError("Command Execution Aborted", "The value of CH1 Temperature Offset is out of range.");
                return;
            }
            
            TestProbe49SetTemp1Offset.TemperatureOffsetValue = Convert.ToByte(int.Parse(txtPCBACalibrationCh1TemperatureOffset.Text));
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe49SetTemp1Offset);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_temp1_offset_Message_ID, TestProbeAPICommand.HST_set_temp1_offset_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);                 

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();       
        }

        private void btnPCBACalibrationGetCh1TempOffset_CheckedChanged(object sender, EventArgs e)
        {
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_temp1_offset_Message_ID, TestProbeAPICommand.HST_get_temp1_offset_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }    
	}
}