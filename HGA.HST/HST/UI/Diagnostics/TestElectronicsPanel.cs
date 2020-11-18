using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Models;

namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    public partial class TestElectronicsPanel : UserControl
    {
        public TestElectronicsPanel()
        {
            InitializeComponent();
        }

        private void buttonGetTemperatures_Click(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.TemperaturesEvent -= new EventHandler(DisplayTemperaturesData);
            HSTMachine.Instance.MainForm.TemperaturesEvent += new EventHandler(DisplayTemperaturesData);

            this.textBoxMeasurementBoardTemperature.Text = "";
            this.textBoxWorkingZoneTemperature.Text = "";
            this.textBoxExhaustTemperature.Text = "";

            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            // HST_start_meas
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_temperature_Message_ID, TestProbeAPICommand.HST_get_temperature_Message_Name, TestProbeAPICommand.HST_get_temperature_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);    
        }

        private void DisplayTemperaturesData(object sender, EventArgs e)
        {
            TemperaturesEventArgs temperaturesEventArgs = e as TemperaturesEventArgs;

            HSTMachine.Instance.MainForm.TemperaturesEvent -= new EventHandler(DisplayTemperaturesData);

            this.textBoxMeasurementBoardTemperature.Text = temperaturesEventArgs.MeasurementBoardTemperature;
            this.textBoxWorkingZoneTemperature.Text = temperaturesEventArgs.WorkingZoneTemperature;
            this.textBoxExhaustTemperature.Text = temperaturesEventArgs.ExhaustTemperature;
        }

        private void TestElectronicsPanel_Enter(object sender, EventArgs e)
        {
            this.textBoxMeasurementBoardTemperature.Text = "";
            this.textBoxWorkingZoneTemperature.Text = "";
            this.textBoxExhaustTemperature.Text = "";
        }
    }
}
