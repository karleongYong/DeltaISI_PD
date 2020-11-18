using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.UI;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.UI;
using XyratexOSC.Utilities;


namespace Seagate.AAS.HGA.HST.UI.Operation.WorkOrder
{
    public partial class MeasurementTest : UserControl
    {
        private int _cap_measurement_sampling_count;
        public bool _manualTest;
        public MeasurementTest()
        {
            InitializeComponent();
            _cap_measurement_sampling_count = HSTSettings.Instance.Install.CapacitanceTestSamplingSize;
            _manualTest = false;
        }

        public void btnGetConversionBoardD_Click(object sender, EventArgs e)
        {          
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_conversion_board_id_Message_ID, TestProbeAPICommand.HST_get_conversion_board_id_Message_Name, TestProbeAPICommand.HST_get_conversion_board_id_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(false); 
        }

        public void btnStartMeasurementTest_Click(object sender, EventArgs e)
        {
            //_manualTest = btnisclick;     
                   
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
              //  return;
            }

            if (btnStartMeasurementTest.Enabled == true)
            {
                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel(), () =>
                {
                    if (String.IsNullOrEmpty(cboTabType.Text) == true)
                    {
                        Notify.PopUp("Missing Tab Type", "A correct Product Tab Type must be selected first before you can start the manual measurement test.", "", "OK");
                        return;
                    }
                });
                
            }
            
            HSTMachine.Instance.MainForm.ResetMeasurementTestResult();

            if (btnStartMeasurementTest.Enabled == false)
            {
                TestProbeProcess testProbeProcess = sender as TestProbeProcess;

                if (testProbeProcess.Controller.IncomingCarrier.HGATabType == HGAProductTabType.Up)
                {
                    HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement.FlexCableIndex = 1;
                }
                else if (testProbeProcess.Controller.IncomingCarrier.HGATabType == HGAProductTabType.Down)
                {
                    HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement.FlexCableIndex = 2;
                }
            }
            else
            {
                UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel(), () =>
                {
                    if (String.Compare(cboTabType.Text, "Up", true) == 0)
                    {
                        HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement.FlexCableIndex = 1;
                    }
                    else
                    {
                        HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement.FlexCableIndex = 2;
                    }
                });                
            }
            if (HSTMachine.Workcell.Process.IsIdleState)
            {
                btnStartMeasurementTest.Enabled = false;
            }
            ResetMeasurementDataOnGUI();
            
#if TestOpt1SoftwareTrigger
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe9SetStartMeasurement);

            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
            // HST_start_meas
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_meas_Message_ID, TestProbeAPICommand.HST_start_meas_Message_Name, TestProbeAPICommand.HST_start_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_short_detection
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Name, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_res_results
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Name, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
          
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);            
#endif                                                
        }

        public void IssueGetMeasurementResults()
        {
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
           
            
            // HST_get_temperature            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_temperature_Message_ID, TestProbeAPICommand.HST_get_temperature_Message_Name, TestProbeAPICommand.HST_get_temperature_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_bias_voltages
            /*     APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_bias_voltages_Message_ID, TestProbeAPICommand.HST_get_bias_voltages_Message_Name, TestProbeAPICommand.HST_get_bias_voltages_Message_Size, null);
                 CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                 APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_all_bias_voltage_Message_ID, TestProbeAPICommand.HST_get_all_bias_voltage_Message_Name, TestProbeAPICommand.HST_get_all_bias_voltage_Message_Size, null);
                 CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                 APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_all_sensing_voltage_Message_ID, TestProbeAPICommand.HST_get_all_sensing_voltage_Message_Name, TestProbeAPICommand.HST_get_all_sensing_voltage_Message_Size, null);
                 CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                 APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_vol_delta_Message_ID, TestProbeAPICommand.HST_get_vol_delta_Message_Name, TestProbeAPICommand.HST_get_vol_delta_Message_Size, null);
                 CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
     */
            //get ldu data

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable)
            {
                for (int x = 1; x < 3; x++)
                {
                    // Get LDU data
                    HSTMachine.Instance.MainForm.TestProbe88Request_5_LDUAndLEDData.first_5_hga = Convert.ToByte(x);
                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe88Request_5_LDUAndLEDData);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_half_LDU_Data_Message_ID, TestProbeAPICommand.HST_get_half_LDU_Data_Message_Name, TestProbeAPICommand.HST_get_half_LDU_Data_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                }

                for (int x = 1; x < 3; x++)
                {
                    HSTMachine.Instance.MainForm.TestProbe89RequestPDVoltage.first_5_hga = Convert.ToByte(x);
                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe89RequestPDVoltage);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_half_photodiode_data_Message_ID, TestProbeAPICommand.HST_get_half_photodiode_data_Message_Name, TestProbeAPICommand.HST_get_half_photodiode_data_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                }
            }

            // HST_get_cap_results
         //   APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_results_Message_ID, TestProbeAPICommand.HST_get_cap_results_Message_Name, TestProbeAPICommand.HST_get_cap_results_Message_Size, null);
         //   CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_short_detection
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Name, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_res_results
           // APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Name, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
           // CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
           
            // HST_get_bias_voltages
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_bias_voltages_Message_ID, TestProbeAPICommand.HST_get_bias_voltages_Message_Name, TestProbeAPICommand.HST_get_bias_voltages_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_all_bias_voltage_Message_ID, TestProbeAPICommand.HST_get_all_bias_voltage_Message_Name, TestProbeAPICommand.HST_get_all_bias_voltage_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_all_sensing_voltage_Message_ID, TestProbeAPICommand.HST_get_all_sensing_voltage_Message_Name, TestProbeAPICommand.HST_get_all_sensing_voltage_Message_Size,null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_vol_delta_Message_ID, TestProbeAPICommand.HST_get_vol_delta_Message_Name, TestProbeAPICommand.HST_get_vol_delta_Message_Size,null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            // HST_get_res_results
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Name, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);



            if (!_manualTest)
            {
                if (HSTSettings.Instance.Install.CapacitanceTestSamplingSize > 0)
                {
                    if (_cap_measurement_sampling_count == 0)
                    {
                        _cap_measurement_sampling_count = HSTSettings.Instance.Install.CapacitanceTestSamplingSize;
                        // TestProbe5MeasurementChannelEnable
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh1Writer = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh2TA = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh3WriteHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh4ReadHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh5Read1 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh6Read2 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh1 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1;
                        HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh2 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2;

                        byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable);

                        APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_meas_channel_enable_Message_ID, TestProbeAPICommand.HST_meas_channel_enable_Message_Name, TestProbeAPICommand.HST_meas_channel_enable_Message_Size, ByteArrayFromStructure);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                    }
                    else
                    {
                        if (_cap_measurement_sampling_count == HSTSettings.Instance.Install.CapacitanceTestSamplingSize)
                        {
                            // TestProbe5MeasurementChannelEnable
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh1Writer = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh2TA = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh3WriteHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh4ReadHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh5Read1 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh6Read2 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh1 = (byte)0;
                            HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh2 = (byte)0; // disable the channel

                            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable);

                            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_meas_channel_enable_Message_ID, TestProbeAPICommand.HST_meas_channel_enable_Message_Name, TestProbeAPICommand.HST_meas_channel_enable_Message_Size, ByteArrayFromStructure);
                            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                        }
                        _cap_measurement_sampling_count--;
                    }
                }
                _manualTest = true;
            }
            else
            {
                if (_cap_measurement_sampling_count != HSTSettings.Instance.Install.CapacitanceTestSamplingSize)
                {
                    // reset the sampling count and restore the count.
                    _cap_measurement_sampling_count = HSTSettings.Instance.Install.CapacitanceTestSamplingSize;
                    // TestProbe5MeasurementChannelEnable
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh1Writer = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh2TA = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh3WriteHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh4ReadHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh5Read1 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.ResistanceCh6Read2 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh1 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1;
                    HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable.CapacitanceCh2 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2;

                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(HSTMachine.Instance.MainForm.TestProbe5MeasurementChannelEnable);

                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_meas_channel_enable_Message_ID, TestProbeAPICommand.HST_meas_channel_enable_Message_Name, TestProbeAPICommand.HST_meas_channel_enable_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                }
            }
           
            HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(true);
        }

        private void ResetMeasurementDataOnGUI()
        {
             UIUtility.Invoke(this, () =>
            {
                this.txtHGA10Ch1WriterResistance.Text = "";
                this.txtHGA10Ch2TAResistance.Text = "";
                this.txtHGA10Ch3WHResistance.Text = "";
                this.txtHGA10Ch4RHResistance.Text = "";
                this.txtHGA10Ch5R1Resistance.Text = "";
                this.txtHGA10Ch6R2Resistance.Text = "";
                this.txtHGA10OverallResult.Text = "";
                this.txtHGA10ShortTest.Text = "";
                this.txtHGA1Ch1WriterResistance.Text = "";
                this.txtHGA1Ch2TAResistance.Text = "";
                this.txtHGA1Ch3WHResistance.Text = "";
                this.txtHGA1Ch4RHResistance.Text = "";
                this.txtHGA1Ch5R1Resistance.Text = "";
                this.txtHGA1Ch6R2Resistance.Text = "";
                this.txtHGA1OverallResult.Text = "";
                this.txtHGA1ShortTest.Text = "";
                this.txtHGA2Ch1WriterResistance.Text = "";
                this.txtHGA2Ch2TAResistance.Text = "";
                this.txtHGA2Ch3WHResistance.Text = "";
                this.txtHGA2Ch4RHResistance.Text = "";
                this.txtHGA2Ch5R1Resistance.Text = "";
                this.txtHGA2Ch6R2Resistance.Text = "";
                this.txtHGA2OverallResult.Text = "";
                this.txtHGA2ShortTest.Text = "";
                this.txtHGA3Ch1WriterResistance.Text = "";
                this.txtHGA3Ch2TAResistance.Text = "";
                this.txtHGA3Ch3WHResistance.Text = "";
                this.txtHGA3Ch4RHResistance.Text = "";
                this.txtHGA3Ch5R1Resistance.Text = "";
                this.txtHGA3Ch6R2Resistance.Text = "";
                this.txtHGA3OverallResult.Text = "";
                this.txtHGA3ShortTest.Text = "";
                this.txtHGA4Ch1WriterResistance.Text = "";
                this.txtHGA4Ch2TAResistance.Text = "";
                this.txtHGA4Ch3WHResistance.Text = "";
                this.txtHGA4Ch3WHResistance.Text = "";
                this.txtHGA4Ch4RHResistance.Text = "";
                this.txtHGA4Ch5R1Resistance.Text = "";
                this.txtHGA4Ch5R1Resistance.Text = "";
                this.txtHGA4Ch6R2Resistance.Text = "";
                this.txtHGA4OverallResult.Text = "";
                this.txtHGA4ShortTest.Text = "";
                this.txtHGA5Ch1WriterResistance.Text = "";
                this.txtHGA5Ch2TAResistance.Text = "";
                this.txtHGA5Ch3WHResistance.Text = "";
                this.txtHGA5Ch4RHResistance.Text = "";
                this.txtHGA5Ch5R1Resistance.Text = "";
                this.txtHGA5Ch6R2Resistance.Text = "";
                this.txtHGA5OverallResult.Text = "";
                this.txtHGA5ShortTest.Text = "";
                this.txtHGA6Ch1WriterResistance.Text = "";
                this.txtHGA6Ch2TAResistance.Text = "";
                this.txtHGA6Ch3WHResistance.Text = "";
                this.txtHGA6Ch4RHResistance.Text = "";
                this.txtHGA6Ch5R1Resistance.Text = "";
                this.txtHGA6Ch6R2Resistance.Text = "";
                this.txtHGA6OverallResult.Text = "";
                this.txtHGA6ShortTest.Text = "";
                this.txtHGA7Ch1WriterResistance.Text = "";
                this.txtHGA7Ch2TAResistance.Text = "";
                this.txtHGA7Ch3WHResistance.Text = "";
                this.txtHGA7Ch4RHResistance.Text = "";
                this.txtHGA7Ch5R1Resistance.Text = "";
                this.txtHGA7Ch6R2Resistance.Text = "";
                this.txtHGA7OverallResult.Text = "";
                this.txtHGA7ShortTest.Text = "";
                this.txtHGA8Ch1WriterResistance.Text = "";
                this.txtHGA8Ch2TAResistance.Text = "";
                this.txtHGA8Ch3WHResistance.Text = "";
                this.txtHGA8Ch4RHResistance.Text = "";
                this.txtHGA8Ch5R1Resistance.Text = "";
                this.txtHGA8Ch6R2Resistance.Text = "";
                this.txtHGA8OverallResult.Text = "";
                this.txtHGA8ShortTest.Text = "";
                this.txtHGA9Ch1WriterResistance.Text = "";
                this.txtHGA9Ch2TAResistance.Text = "";
                this.txtHGA9Ch3WHResistance.Text = "";
                this.txtHGA9Ch4RHResistance.Text = "";
                this.txtHGA9Ch5R1Resistance.Text = "";
                this.txtHGA9Ch6R2Resistance.Text = "";
                this.txtHGA9OverallResult.Text = "";
                this.txtHGA9ShortTest.Text = "";
                //Delta ISI
                this.txtbox_HGA1DeltaISI.Text = "";
                this.txtbox_HGA2DeltaISI.Text = "";
                this.txtbox_HGA3DeltaISI.Text = "";
                this.txtbox_HGA4DeltaISI.Text = "";
                this.txtbox_HGA5DeltaISI.Text = "";
                this.txtbox_HGA6DeltaISI.Text = "";
                this.txtbox_HGA7DeltaISI.Text = "";
                this.txtbox_HGA8DeltaISI.Text = "";
                this.txtbox_HGA9DeltaISI.Text = "";
                this.txtbox_HGA10DeltaISI.Text = "";

                this.txtbox_HGA1DeltaISI_Diff.Text = "";
                this.txtbox_HGA1DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA2DeltaISI_Diff.Text = "";
                this.txtbox_HGA2DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA3DeltaISI_Diff.Text = "";
                this.txtbox_HGA3DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA4DeltaISI_Diff.Text = "";
                this.txtbox_HGA4DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA5DeltaISI_Diff.Text = "";
                this.txtbox_HGA5DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA6DeltaISI_Diff.Text = "";
                this.txtbox_HGA6DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA7DeltaISI_Diff.Text = "";
                this.txtbox_HGA7DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA8DeltaISI_Diff.Text = "";
                this.txtbox_HGA8DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA9DeltaISI_Diff.Text = "";
                this.txtbox_HGA9DeltaISIR2_Diff.Text = "";
                this.txtbox_HGA10DeltaISI_Diff.Text = "";
                this.txtbox_HGA10DeltaISIR2_Diff.Text = "";
            });
        }

        private void MeasurementTest_Load(object sender, EventArgs e)
        {
            if(DesignMode) return;
        }

        private void MeasurementTest_VisibleChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;
        }

        private void txtbox_HGA1DeltaISI_Diff_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
