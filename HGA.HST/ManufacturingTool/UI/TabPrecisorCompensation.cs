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
using BenchTestsTool.Data;
using BenchTestsTool.Data.IncomingTestProbeData;
using BenchTestsTool.Data.OutgoingTestProbeData;
using BenchTestsTool.Utils;

namespace BenchTestsTool.UI
{
    public partial class frmMain
    {
        private void tabPagePrecisorCompensation_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;
            if (IsPrecisorCompensationTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }

            System.Drawing.Graphics tabPagePrecisorCompensationGraphics = tabPagePrecisorCompensation.CreateGraphics();
            tabPagePrecisorCompensationGraphics.FillEllipse(myBrush, new Rectangle(700, 307, 40, 40));
            myBrush.Dispose();
            tabPagePrecisorCompensationGraphics.Dispose();
        }
        
        private void btnPrecisorCompensationSetPrecisorCapacitanceCompensation_Click(object sender, EventArgs e)
        {
            disableAllAPIButtons();

            // TestProbe52SetPrecisorCapacitanceCompensation
            // HGA1 CH1
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA1CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA1CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch1CompensationMSBHigh = ByteArray[3];

            // HGA1 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA1CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA1CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA1Ch2CompensationMSBHigh = ByteArray[3];

            // HGA2 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA2CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA2CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch1CompensationMSBHigh = ByteArray[3];

            // HGA2 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA2CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA2CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA2Ch2CompensationMSBHigh = ByteArray[3];

            // HGA3 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA3CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA3CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch1CompensationMSBHigh = ByteArray[3];

            // HGA3 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA3CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA3CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA3Ch2CompensationMSBHigh = ByteArray[3];

            // HGA4 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA4CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA4CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch1CompensationMSBHigh = ByteArray[3];

            // HGA4 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA4CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA4CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA4Ch2CompensationMSBHigh = ByteArray[3];

            // HGA5 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA5CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA5CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch1CompensationMSBHigh = ByteArray[3];

            // HGA5 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA5CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA5CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA5Ch2CompensationMSBHigh = ByteArray[3];

            // HGA6 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA6CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA6CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch1CompensationMSBHigh = ByteArray[3];

            // HGA6 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA6CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA6CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA6Ch2CompensationMSBHigh = ByteArray[3];

            // HGA7 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA7CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA7CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch1CompensationMSBHigh = ByteArray[3];

            // HGA7 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA7CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA7CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA7Ch2CompensationMSBHigh = ByteArray[3];

            // HGA8 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA8CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA8CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch1CompensationMSBHigh = ByteArray[3];

            // HGA8 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA8CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA8CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA8Ch2CompensationMSBHigh = ByteArray[3];

            // HGA9 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA9CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA9CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch1CompensationMSBHigh = ByteArray[3];

            // HGA9 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA9CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA9CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA9Ch2CompensationMSBHigh = ByteArray[3];

            // HGA10 CH1
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA10CH1.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA10CH1.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch1CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch1CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch1CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch1CompensationMSBHigh = ByteArray[3];

            // HGA10 CH2
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtPrecisorCompensationHGA10CH2.Text) ? 0 : int.Parse(txtPrecisorCompensationHGA10CH2.Text));
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch2CompensationLSBLow = ByteArray[0];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch2CompensationLSBMid = ByteArray[1];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch2CompensationMSBLow = ByteArray[2];
            TestProbe52SetPrecisorCapacitanceCompensation.HGA10Ch2CompensationMSBHigh = ByteArray[3];

            if (String.Compare(cboPrecisorCompensationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe52SetPrecisorCapacitanceCompensation.PrecisorIndex = 1;
            }
            else
            {
                TestProbe52SetPrecisorCapacitanceCompensation.PrecisorIndex = 2;
            }

            if (String.Compare(cboPrecisorCompensationEnableFlag.Text, "TRUE", true) == 0)
            {
                TestProbe52SetPrecisorCapacitanceCompensation.EnableFlag = 1;
            }
            else
            {
                TestProbe52SetPrecisorCapacitanceCompensation.EnableFlag = 0;
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe52SetPrecisorCapacitanceCompensation);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_precisor_cap_compensation_Message_ID, TestProbeAPICommand.HST_set_precisor_cap_compensation_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            enableAllAPIButtons();

            if (commandSentToMicroprocessor)
            {
                IsPrecisorCompensationTempered = true;
                tabPagePrecisorCompensation.Refresh();
            }
        }

        private void btnPrecisorCompensationGetPrecisorCapacitanceCompensation_Click(object sender, EventArgs e)
        {
            disableAllAPIButtons();

            if (String.Compare(cboPrecisorCompensationUpDownTabSelection.Text, "UP", true) == 0)
            {
                TestProbe53SetGetPrecisorCapacitanceCompensation.PrecisorIndex = 1;                
            }
            else
            {
                TestProbe53SetGetPrecisorCapacitanceCompensation.PrecisorIndex = 2;      
            }

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe53SetGetPrecisorCapacitanceCompensation);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_precisor_cap_compensation_Message_ID, TestProbeAPICommand.HST_get_precisor_cap_compensation_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);            

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            enableAllAPIButtons();

            if (commandSentToMicroprocessor)
            {
                IsPrecisorCompensationTempered = false;
                tabPagePrecisorCompensation.Refresh();
            }
        }

        private void btnPrecisorCompensationSavePrecisorCapacitanceCompensation_Click(object sender, EventArgs e)
        {
            disableAllAPIButtons();

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_save_precisor_cap_compensation_Message_ID, TestProbeAPICommand.HST_save_precisor_cap_compensation_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            enableAllAPIButtons();

            if (commandSentToMicroprocessor)
            {
                IsPrecisorCompensationTempered = false;
                tabPagePrecisorCompensation.Refresh();

                Notify.PopUp("Command Sent To uProcessor", "Precisor compensation data saved.", null, "OK");
            }
        }       
	}
}