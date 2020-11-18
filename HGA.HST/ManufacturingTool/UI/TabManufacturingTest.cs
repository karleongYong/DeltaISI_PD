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
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using BenchTestsTool.Data;
using BenchTestsTool.Data.IncomingTestProbeData;
using BenchTestsTool.Data.OutgoingTestProbeData;
using BenchTestsTool.Utils;
using System.Timers;

namespace BenchTestsTool.UI
{
    public partial class frmMain
    {
        private bool boolPass;

        private void btnStartManufacturingTest_Click(object sender, EventArgs e)
        {

            boolPass = true;
            picTestStatus1.Visible = false;
            picTestStatus2.Visible = false;
            picTestStatus3.Visible = false;
            picTestStatus4.Visible = false;
            picTestStatus5.Visible = false;
            picTestStatus6.Visible = false;
            picTestStatus7.Visible = false;

            PCBACommunicationStatusFlag = false;
            PCBAParameterSettingsStatusFlag = false;
            PCBAAutoCalibrationStatusFlag = false;
            PCBADisablePrecisorCompensationStatusFlag = false;
            PCBAShortCircuitSettingsStatusFlag = false;
            PCBATemperatureCalibrationStatusFlag = false;
            PCBAFunctionalTestStatusFlag = true;

           // test1();
            
            Thread t1 = new Thread(test1);
            t1.Start();
            
           

        }


        private void test1()
        {
            disableAllAPIButtons();
          
            CheckPCBACommunication();
            PCBASettingParameter();
            PCBAAutoCalibration();
            PCBASetShortCircuitParameter();
            PCBADisablePricisorCompensation();
            PCBATemperatureCalibrationCH2();
            //PCBATemperatureCalibrationCH3();
            PCBAFunctionalTest();
           
            enableAllAPIButtons();
            if (PCBACommunicationStatusFlag && 
                PCBACommunicationStatusFlag &&
                PCBAParameterSettingsStatusFlag &&
                PCBAAutoCalibrationStatusFlag &&
                PCBADisablePrecisorCompensationStatusFlag &&
                PCBAShortCircuitSettingsStatusFlag &&
                PCBATemperatureCalibrationStatusFlag &&
                PCBAFunctionalTestStatusFlag )
            Log.Info("The Test Finish", "PASS");
            else
                Log.Info("The Test Finish", "FAIL");
          
        }

      

       
        private void CheckPCBACommunication()
        {
            if (boolPass)
            {
                Log.Info("Prompt", "1.) Checking PCBA Communication\n\n");
                TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_firmware_version_Message_ID, TestProbeAPICommand.HST_get_firmware_version_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();


                Delay(1000);
                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBACommunicationStatusFlag)
                    {
                        this.picTestStatus1.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus1.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
                    }

                    this.picTestStatus1.Visible = true;

                });
            }
        }

        private void PCBASettingParameter()
        {
            Delay(1000);
            if (boolPass)
            {
               
                Log.Info("Prompt", "2.) SettingPCBAParameter\n");
                btnConfigurationSaveConfiguration_Click(this, new EventArgs());
                disableAllAPIButtons();


                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBAParameterSettingsStatusFlag)
                    {
                        this.picTestStatus2.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus2.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
                    }

                    this.picTestStatus2.Visible = true;

                });
            }

          
        }

        private void PCBAAutoCalibration()
        {
            Delay(1000);
            if (boolPass)
            {
                Delay(1000);
                Log.Info("Prompt", "3.) Perform PCBA Auto Calibration\n");
                TestProbe17CalibrationEnable.EnableFlag = 1;


                byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe17CalibrationEnable);
                TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibration_enable_Message_ID, TestProbeAPICommand.HST_calibration_enable_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_auto_calibration_Message_ID, TestProbeAPICommand.HST_start_auto_calibration_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_save_calibration_data_Message_ID, TestProbeAPICommand.HST_save_calibration_data_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);



                bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                Delay(12000);
                UIUtility.BeginInvoke(this, () =>
                {

                    if (PCBAAutoCalibrationStatusFlag)
                    {
                        this.picTestStatus3.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus3.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
            
                    }

                    this.picTestStatus3.Visible = true;

                });
            }
            //    chkPCBACalibrationCalibrationEnable.Checked = false;
        }
       
        private void PCBASetShortCircuitParameter()
        {
            Delay(1000);
            if (boolPass)
            {
                Log.Info("Prompt", "4.) Set Short Circuit Parameter\n");

                // TestProbe47SetShortDetectionThreshold
                byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtCableCalibrationShortDetectionThreshold.Text) ? 0 : 40);
                TestProbe47SetShortDetectionThreshold.ThresholdVoltageLSBLow = ByteArray[0];
                TestProbe47SetShortDetectionThreshold.ThresholdVoltageLSBMid = ByteArray[1];
                TestProbe47SetShortDetectionThreshold.ThresholdVoltageMSBLow = ByteArray[2];
                TestProbe47SetShortDetectionThreshold.ThresholdVoltageMSBHigh = ByteArray[3];


                byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe47SetShortDetectionThreshold);
                TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_short_detection_threshold_Message_ID, TestProbeAPICommand.HST_set_short_detection_threshold_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();


                Delay(1000);
                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBAShortCircuitSettingsStatusFlag)
                    {
                        this.picTestStatus4.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus4.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
            
                    }

                    this.picTestStatus4.Visible = true;

                });
            }
        }

        private void PCBADisablePricisorCompensation()
        {
            Delay(1000);
            if (boolPass)
            {
                Log.Info("Prompt", "5.) Disable Precisor Compensation\n");

                for (int index = 1; index < 3; index++)
                {

                    // Read the Precisor Capaciatnce Compensation Value
                    //TestProbe53SetGetPrecisorCapacitanceCompensation.PrecisorIndex = (byte)index;

                    //byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe53SetGetPrecisorCapacitanceCompensation);
                    //TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_precisor_cap_compensation_Message_ID, TestProbeAPICommand.HST_get_precisor_cap_compensation_Message_Size, ByteArrayFromStructure);
                    //CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    //bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                    //Delay(3000);
                    //-------------------------------

                    // Write the Precisor Capaciatnce Compensation Value

                    if(index == 1)
                        TestProbe52SetPrecisorCapacitanceCompensation.PrecisorIndex = 1;
                    else
                        TestProbe52SetPrecisorCapacitanceCompensation.PrecisorIndex = 2;
                   
                    TestProbe52SetPrecisorCapacitanceCompensation.EnableFlag = 0; //Disable the compensation
                    byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe52SetPrecisorCapacitanceCompensation);
                    TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_precisor_cap_compensation_Message_ID, TestProbeAPICommand.HST_set_precisor_cap_compensation_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                    Delay(1000);
                }
                //Delay(1000);
                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBADisablePrecisorCompensationStatusFlag)
                    {
                        this.picTestStatus5.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus5.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
            
                    
                    }

                    this.picTestStatus5.Visible = true;

                });
            }

        }

        private void PCBATemperatureCalibrationCH2()
        {
            string message = "Connect CH 2 to 100 ohm resistor";
            string caption = "Temperature Calibarition for CH 2";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result;
           

            TestProbeAPICommand APICommand;
            bool commandSentToMicroprocessor;
            byte[] ByteArrayFromStructure;
            Delay(1000);
            if (boolPass)
            {
                Log.Info("Prompt", "6.) PCBA Temperature Calibration Test\n");
                Delay(1000);
                

                
                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    // Enable calibration mode     
                    ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe17CalibrationEnable);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibration_enable_Message_ID, TestProbeAPICommand.HST_calibration_enable_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                    Delay(1000);

                    int TemperatureChannel = 2;


                    //TestProbe31SetTemperatureCalibration
                    TestProbe31SetTemperatureCalibration.ChannelNumber = Convert.ToByte(TemperatureChannel);
                    TestProbe31SetTemperatureCalibration.Temperature = Convert.ToByte(5); // the firmware accept 5, 10 , and so on .. up to 100
                    ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe31SetTemperatureCalibration);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_temp_calibration_Message_ID, TestProbeAPICommand.HST_set_temp_calibration_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                }

                PCBATemperatureCalibrationCH3();
                Delay(3000);
                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBATemperatureCalibrationStatusFlag)
                    {
                        this.picTestStatus6.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus6.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;
            
                    }

                    this.picTestStatus6.Visible = true;

                });
            }
        }

        private void PCBATemperatureCalibrationCH3()
        {
            string message = "Connect CH 3 to 100 ohm resistor";
            string caption = "Temperature Calibarition for CH 3";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result;
           

            TestProbeAPICommand APICommand;
            bool commandSentToMicroprocessor;
            byte[] ByteArrayFromStructure;
            Delay(1000);
            if (boolPass)
            {
               // Log.Info("Prompt", "6.) PCBA Temperature Calibration Test\n");
                Delay(1000);



                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    // Enable calibration mode     
                    ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe17CalibrationEnable);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_calibration_enable_Message_ID, TestProbeAPICommand.HST_calibration_enable_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                    

                    int TemperatureChannel = 3;

                    Delay(1000);
                    //TestProbe31SetTemperatureCalibration
                    TestProbe31SetTemperatureCalibration.ChannelNumber = Convert.ToByte(TemperatureChannel);
                    TestProbe31SetTemperatureCalibration.Temperature = Convert.ToByte(5); // the firmware accept 5, 10 , and so on .. up to 100
                    ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe31SetTemperatureCalibration);
                    APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_temp_calibration_Message_ID, TestProbeAPICommand.HST_set_temp_calibration_Message_Size, ByteArrayFromStructure);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                    commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                }

         /*       Delay(3000);
                UIUtility.BeginInvoke(this, () =>
                {
                    if (PCBATemperatureCalibrationStatusFlag)
                    {
                        this.picTestStatus6.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                    }
                    else
                    {
                        this.picTestStatus6.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                        boolPass = false;

                    }

                    this.picTestStatus6.Visible = true;

                });*/
            }
        }
        private void PCBAFunctionalTest()
        {
            Delay(1000);
            if (boolPass)
            {
                Log.Info("Prompt", "7.) PCBA Functional Test\n");
                Delay(1000);
           
                ClearDisplay();
               
                TestProbe9SetStartMeasurement.FlexCableIndex = 1;
               
                byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe9SetStartMeasurement);
                TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_start_meas_Message_ID, TestProbeAPICommand.HST_start_meas_Message_Size, ByteArrayFromStructure);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
               
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_short_detection_Message_ID, TestProbeAPICommand.HST_get_short_detection_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_res_results_Message_ID, TestProbeAPICommand.HST_get_res_results_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
                
                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_results_Message_ID, TestProbeAPICommand.HST_get_cap_results_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_secondary_results_Message_ID, TestProbeAPICommand.HST_get_cap_secondary_results_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
                Delay(12000);

                UIUtility.BeginInvoke(this, () =>
                 {
                     if (PCBAFunctionalTestStatusFlag)
                     {
                         this.picTestStatus7.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.PASS, new System.Drawing.Size(36, 36)));
                     }
                     else
                     {
                         this.picTestStatus7.Image = (System.Drawing.Image)(new System.Drawing.Bitmap(ManufacturingTestTool.Properties.Resources.FAIL, new System.Drawing.Size(36, 36)));
                         boolPass = false;

                     }


                     this.picTestStatus7.Visible = true;

                 });
            }
        }

        private void Delay(int miliseconds)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            var delay = Task.Delay(miliseconds).ContinueWith(_ =>
            {
                sw.Stop();
                return sw.ElapsedMilliseconds;
            });
            Log.Info("", "", delay.Result);
        }

        private void ClearDisplay()
        {
            UIUtility.BeginInvoke(this, () =>
            {
                txtBenchTestsHGA1Short.Text = "";
                txtBenchTestsHGA1Ch1Res.Text = "";
                txtBenchTestsHGA1Ch2Res.Text = "";
                txtBenchTestsHGA1Ch3Res.Text = "";
                txtBenchTestsHGA1Ch4Res.Text = "";
                txtBenchTestsHGA1Ch5Res.Text = "";
                txtBenchTestsHGA1Ch6Res.Text = "";
                txtBenchTestsHGA1Ch1Capa.Text = "";
                txtBenchTestsHGA1Ch1ESR.Text = "";
                txtBenchTestsHGA1Ch2Capa.Text = "";
                txtBenchTestsHGA1Ch2ESR.Text = "";

                txtBenchTestsHGA2Short.Text = "";
                txtBenchTestsHGA2Ch1Res.Text = "";
                txtBenchTestsHGA2Ch2Res.Text = "";
                txtBenchTestsHGA2Ch3Res.Text = "";
                txtBenchTestsHGA2Ch4Res.Text = "";
                txtBenchTestsHGA2Ch5Res.Text = "";
                txtBenchTestsHGA2Ch6Res.Text = "";
                txtBenchTestsHGA2Ch1Capa.Text = "";
                txtBenchTestsHGA2Ch1ESR.Text = "";
                txtBenchTestsHGA2Ch2Capa.Text = "";
                txtBenchTestsHGA2Ch2ESR.Text = "";

                txtBenchTestsHGA3Short.Text = "";
                txtBenchTestsHGA3Ch1Res.Text = "";
                txtBenchTestsHGA3Ch2Res.Text = "";
                txtBenchTestsHGA3Ch3Res.Text = "";
                txtBenchTestsHGA3Ch4Res.Text = "";
                txtBenchTestsHGA3Ch5Res.Text = "";
                txtBenchTestsHGA3Ch6Res.Text = "";
                txtBenchTestsHGA3Ch1Capa.Text = "";
                txtBenchTestsHGA3Ch1ESR.Text = "";
                txtBenchTestsHGA3Ch2Capa.Text = "";
                txtBenchTestsHGA3Ch2ESR.Text = "";

                txtBenchTestsHGA4Short.Text = "";
                txtBenchTestsHGA4Ch1Res.Text = "";
                txtBenchTestsHGA4Ch2Res.Text = "";
                txtBenchTestsHGA4Ch3Res.Text = "";
                txtBenchTestsHGA4Ch4Res.Text = "";
                txtBenchTestsHGA4Ch5Res.Text = "";
                txtBenchTestsHGA4Ch6Res.Text = "";
                txtBenchTestsHGA4Ch1Capa.Text = "";
                txtBenchTestsHGA4Ch1ESR.Text = "";
                txtBenchTestsHGA4Ch2Capa.Text = "";
                txtBenchTestsHGA4Ch2ESR.Text = "";

                txtBenchTestsHGA5Short.Text = "";
                txtBenchTestsHGA5Ch1Res.Text = "";
                txtBenchTestsHGA5Ch2Res.Text = "";
                txtBenchTestsHGA5Ch3Res.Text = "";
                txtBenchTestsHGA5Ch4Res.Text = "";
                txtBenchTestsHGA5Ch5Res.Text = "";
                txtBenchTestsHGA5Ch6Res.Text = "";
                txtBenchTestsHGA5Ch1Capa.Text = "";
                txtBenchTestsHGA5Ch1ESR.Text = "";
                txtBenchTestsHGA5Ch2Capa.Text = "";
                txtBenchTestsHGA5Ch2ESR.Text = "";

                txtBenchTestsHGA6Short.Text = "";
                txtBenchTestsHGA6Ch1Res.Text = "";
                txtBenchTestsHGA6Ch2Res.Text = "";
                txtBenchTestsHGA6Ch3Res.Text = "";
                txtBenchTestsHGA6Ch4Res.Text = "";
                txtBenchTestsHGA6Ch5Res.Text = "";
                txtBenchTestsHGA6Ch6Res.Text = "";
                txtBenchTestsHGA6Ch1Capa.Text = "";
                txtBenchTestsHGA6Ch1ESR.Text = "";
                txtBenchTestsHGA6Ch2Capa.Text = "";
                txtBenchTestsHGA6Ch2ESR.Text = "";

                txtBenchTestsHGA7Short.Text = "";
                txtBenchTestsHGA7Ch1Res.Text = "";
                txtBenchTestsHGA7Ch2Res.Text = "";
                txtBenchTestsHGA7Ch3Res.Text = "";
                txtBenchTestsHGA7Ch4Res.Text = "";
                txtBenchTestsHGA7Ch5Res.Text = "";
                txtBenchTestsHGA7Ch6Res.Text = "";
                txtBenchTestsHGA7Ch1Capa.Text = "";
                txtBenchTestsHGA7Ch1ESR.Text = "";
                txtBenchTestsHGA7Ch2Capa.Text = "";
                txtBenchTestsHGA7Ch2ESR.Text = "";

                txtBenchTestsHGA8Short.Text = "";
                txtBenchTestsHGA8Ch1Res.Text = "";
                txtBenchTestsHGA8Ch2Res.Text = "";
                txtBenchTestsHGA8Ch3Res.Text = "";
                txtBenchTestsHGA8Ch4Res.Text = "";
                txtBenchTestsHGA8Ch5Res.Text = "";
                txtBenchTestsHGA8Ch6Res.Text = "";
                txtBenchTestsHGA8Ch1Capa.Text = "";
                txtBenchTestsHGA8Ch1ESR.Text = "";
                txtBenchTestsHGA8Ch2Capa.Text = "";
                txtBenchTestsHGA8Ch2ESR.Text = "";

                txtBenchTestsHGA9Short.Text = "";
                txtBenchTestsHGA9Ch1Res.Text = "";
                txtBenchTestsHGA9Ch2Res.Text = "";
                txtBenchTestsHGA9Ch3Res.Text = "";
                txtBenchTestsHGA9Ch4Res.Text = "";
                txtBenchTestsHGA9Ch5Res.Text = "";
                txtBenchTestsHGA9Ch6Res.Text = "";
                txtBenchTestsHGA9Ch1Capa.Text = "";
                txtBenchTestsHGA9Ch1ESR.Text = "";
                txtBenchTestsHGA9Ch2Capa.Text = "";
                txtBenchTestsHGA9Ch2ESR.Text = "";

                txtBenchTestsHGA10Short.Text = "";
                txtBenchTestsHGA10Ch1Res.Text = "";
                txtBenchTestsHGA10Ch2Res.Text = "";
                txtBenchTestsHGA10Ch3Res.Text = "";
                txtBenchTestsHGA10Ch4Res.Text = "";
                txtBenchTestsHGA10Ch5Res.Text = "";
                txtBenchTestsHGA10Ch6Res.Text = "";
                txtBenchTestsHGA10Ch1Capa.Text = "";
                txtBenchTestsHGA10Ch1ESR.Text = "";
                txtBenchTestsHGA10Ch2Capa.Text = "";
                txtBenchTestsHGA10Ch2ESR.Text = "";
            });
        }

	}
}