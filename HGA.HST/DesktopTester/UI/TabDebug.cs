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
using System.Threading.Tasks;

namespace DesktopTester.UI
{
    public partial class frmMain
    {
        private void btnDebugEEPROMWrite_Click(object sender, EventArgs e)
        {                        
            //TestProbe22EEPROMWrite     
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugEEPROMWriteReg.Text) ? 0 : Convert.ToUInt32(txtDebugEEPROMWriteReg.Text, 16));
            TestProbe22EEPROMWrite.EEPROMStartAddressLSB = ByteArray[0];
            TestProbe22EEPROMWrite.EEPROMStartAddressMSB = ByteArray[1];

            TestProbe22EEPROMWrite.NumberOfDataBytesToWrite = 1;

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugEEPROMWriteData.Text) ? 0 : Convert.ToUInt32(txtDebugEEPROMWriteData.Text, 16));
            TestProbe22EEPROMWrite.DataByte1 = ByteArray[0];
             
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe22EEPROMWrite);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_eeprom_write_Message_ID, TestProbeAPICommand.HST_eeprom_write_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();      
        }

        private void btnDebugDACWrite_Click(object sender, EventArgs e)
        {           
            //TestProbe24DACWrite     
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugDACWriteReg.Text) ? 0 : Convert.ToUInt32(txtDebugDACWriteReg.Text, 16));
            TestProbe24DACWrite.RegisterAddress = ByteArray[0];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugDACWriteData.Text) ? 0 : Convert.ToUInt32(txtDebugDACWriteData.Text, 16));
            TestProbe24DACWrite.DataLSB = ByteArray[0];
            TestProbe24DACWrite.DataMSB = ByteArray[1];

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe24DACWrite);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_dac_write_Message_ID, TestProbeAPICommand.HST_dac_write_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDebugADCWrite_Click(object sender, EventArgs e)
        {           
            //TestProbe27ADCWrite     
            if (rdxDebugRegisterADC1.Checked)
            {
                TestProbe27ADCWrite.ADCNumber = 1;
            }
            if (rdxDebugRegisterADC2.Checked)
            {
                TestProbe27ADCWrite.ADCNumber = 2;
            }

            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugADCWriteReg.Text) ? 0 : Convert.ToUInt32(txtDebugADCWriteReg.Text, 16));
            TestProbe27ADCWrite.RegisterAddress = ByteArray[0];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugADCWriteData.Text) ? 0 : Convert.ToUInt32(txtDebugADCWriteData.Text, 16));
            TestProbe27ADCWrite.DataLSB = ByteArray[0];
            TestProbe27ADCWrite.DataMid = ByteArray[1];
            TestProbe27ADCWrite.DataMSB = ByteArray[2];

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe27ADCWrite);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_adc_write_Message_ID, TestProbeAPICommand.HST_adc_write_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDebugEEPROMRead_Click(object sender, EventArgs e)
        {            
            //TestProbe23SetEEPROMRead     
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugEEPROMReadReg.Text) ? 0 : Convert.ToUInt32(txtDebugEEPROMReadReg.Text, 16));
            TestProbe23SetEEPROMRead.EEPROMStartAddressLSB = ByteArray[0];
            TestProbe23SetEEPROMRead.EEPROMStartAddressMSB = ByteArray[1];

            TestProbe23SetEEPROMRead.NumberOfDataBytesToRead = 1;

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe23SetEEPROMRead);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_eeprom_read_Message_ID, TestProbeAPICommand.HST_eeprom_read_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();       
        }
        
        private void btnDebugDACRead_Click(object sender, EventArgs e)
        {
            //TestProbe25SetDACRead     
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugDACReadReg.Text) ? 0 : Convert.ToUInt32(txtDebugDACReadReg.Text, 16));
            TestProbe25SetDACRead.RegisterAddress = ByteArray[0];

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe25SetDACRead);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_dac_read_Message_ID, TestProbeAPICommand.HST_dac_read_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();                
        }
        
        private void btnDebugADCRead_Click(object sender, EventArgs e)
        {            
            byte ADCNumber = 1;
            
            if (rdxDebugRegisterADC1.Checked)
            {
                ADCNumber = 1;
            }
            else
            {
                ADCNumber = 2;
            }

            //TestProbe28SetADCRead   
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtDebugADCReadReg.Text) ? 0 : Convert.ToUInt32(txtDebugADCReadReg.Text, 16));            
            TestProbe28SetADCRead.RegisterAddress = ByteArray[0];
            TestProbe28SetADCRead.ADCNumber = ADCNumber;

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe28SetADCRead);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_adc_read_Message_ID, TestProbeAPICommand.HST_adc_read_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDebugMUXSetMUXSwitch_Click(object sender, EventArgs e)
        {            
            int FunctionalBlock = 0;
            if (rdxDebugMUXDAC.Checked)
            {
                FunctionalBlock = 1;
            }
            else if (rdxDebugMUXADC.Checked)
            {
                FunctionalBlock = 2;
            }
            else if (rdxDebugMUXCAP.Checked)
            {
                FunctionalBlock = 3;
            }            

            //TestProbe30SetMUX
            TestProbe30SetMUX.FunctionalBlock = Convert.ToByte(FunctionalBlock);
            TestProbe30SetMUX.ChannelNumber = Convert.ToByte(int.Parse(cboDebugMUXCh.Text));
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe30SetMUX);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_set_mux_Message_ID, TestProbeAPICommand.HST_set_mux_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDebugMUXGetCapacitanceReading_Click(object sender, EventArgs e)
        {            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_reading_Message_ID, TestProbeAPICommand.HST_get_cap_reading_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();    
        }
		
        private void btnDebugMUXGetProcessorStatus_Click(object sender, EventArgs e)
        {            
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_status_Message_ID, TestProbeAPICommand.HST_get_status_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();
        }

        private void btnDebugTemperatureGetTemperatures_Click(object sender, EventArgs e)
        {
            if(chkDebugTemperatureContinuous.Checked)
            {
                CommonFunctions.Instance.GetTemperatureContinuous = true;
            }
            else
            {
                CommonFunctions.Instance.GetTemperatureContinuous = false;
            }

            Task t = Task.Factory.StartNew(() =>
            {
                do
                {
                    //TestProbe33SetTemperatureRead                    
                    TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_temperature_Message_ID, TestProbeAPICommand.HST_get_temperature_Message_Size, null);
                    CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);                    

                    bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                    Thread.Sleep(CommonFunctions.Instance.DelayInBetweenCommandBatch);
                } while (CommonFunctions.Instance.GetTemperatureContinuous);                
            });
                        
        }        

        private void btnDebugTemperatureStop_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.GetTemperatureContinuous = false;
        }

        private void btnDebugADCVoltageGetADCVoltages_Click(object sender, EventArgs e)
        {
            if (chkDebugADCVoltageContinuous.Checked)
            {
                CommonFunctions.Instance.GetADCVoltageContinuous = true;
            }
            else
            {
                CommonFunctions.Instance.GetADCVoltageContinuous = false;
            }

            int ADCNumber = 0;
            if (rdxDebugADCVoltageADC1.Checked)
            {
                ADCNumber = 1;
            }
            else if (rdxDebugADCVoltageADC2.Checked)
            {
                ADCNumber = 2;
            }


            if (String.Compare(cboDebugADCVoltageCh.Text, "ALL", true) == 0)
            {
                string ADCVoltageChannel = cboDebugADCVoltageCh.Text;

                Task t = Task.Factory.StartNew(() =>
                {
                    do
                    {
                        for (int i = 1; i < 17; i++)
                        {
                            //TestProbe29SetADCVoltagesRead
                            TestProbe29SetADCVoltagesRead.ADCNumber = Convert.ToByte(ADCNumber);
                            TestProbe29SetADCVoltagesRead.ChannelNumber = (byte)i;
                            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe29SetADCVoltagesRead);
                            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_adc_voltages_Message_ID, TestProbeAPICommand.HST_get_adc_voltages_Message_Size, ByteArrayFromStructure);
                            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                            DataReceived = false;
                            while (!DataReceived)
                            {
                                Thread.Sleep(100);
                            }  
                        }
                    } while (CommonFunctions.Instance.GetADCVoltageContinuous);
                });
            }
            else
            {
                string ADCVoltageChannel = cboDebugADCVoltageCh.Text;

                Task t = Task.Factory.StartNew(() =>
                {
                    do
                    {
                        //TestProbe29SetADCVoltagesRead
                        TestProbe29SetADCVoltagesRead.ADCNumber = Convert.ToByte(ADCNumber);
                        TestProbe29SetADCVoltagesRead.ChannelNumber = Convert.ToByte(ADCVoltageChannel);
                        byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe29SetADCVoltagesRead);
                        TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_adc_voltages_Message_ID, TestProbeAPICommand.HST_get_adc_voltages_Message_Size, ByteArrayFromStructure);
                        CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                        bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

                        Thread.Sleep(CommonFunctions.Instance.DelayInBetweenCommandBatch);
                    } while (CommonFunctions.Instance.GetADCVoltageContinuous);
                });
            }
        }

        private void btnDebugADCVoltageStop_Click(object sender, EventArgs e)
        {
            CommonFunctions.Instance.GetADCVoltageContinuous = false;
        }

        private void btnDebugBiasSensingGetBiasVoltages_Click(object sender, EventArgs e)
        {           
            //TestProbe15SetGetBiasByHGA
            TestProbe15SetGetBiasByHGA.HGAIndex = Convert.ToByte(int.Parse(cboDebugBiasSensingGetBiasVoltagesHGA.Text));
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe15SetGetBiasByHGA);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_bias_by_hga_Message_ID, TestProbeAPICommand.HST_get_bias_by_hga_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();     
        }

        private void btnDebugBiasSensingGetSensingVoltages_Click(object sender, EventArgs e)
        {           
            //TestProbe16SetGetSensingByHGA
            TestProbe16SetGetSensingByHGA.HGAIndex = Convert.ToByte(int.Parse(cboDebugBiasSensingGetBiasVoltagesHGA.Text));
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe16SetGetSensingByHGA);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_sensing_by_hga_Message_ID, TestProbeAPICommand.HST_get_sensing_by_hga_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();       
        }

        private void btnDebugResistanceCapacitanceGetResults_Click(object sender, EventArgs e)
        {
            bool CorrectionEnable = false;
            if(chkDebugResistanceCapacitanceCorrectionEnable.Checked)
            {
                CorrectionEnable = true;
            }

            //TestProbe14SetGetAllResultsByHGA
            TestProbe14SetGetAllResultsByHGA.HGAIndex = Convert.ToByte(int.Parse(cboDebugResistanceCapacitanceHGA.Text));
            TestProbe14SetGetAllResultsByHGA.Correction = Convert.ToByte(CorrectionEnable);
            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe14SetGetAllResultsByHGA);
            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_results_by_hga_Message_ID, TestProbeAPICommand.HST_get_results_by_hga_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_cap_secondary_results_Message_ID, TestProbeAPICommand.HST_get_cap_secondary_results_Message_Size, null);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer(); 
        }        

        private void rdxDebugMUXDAC_CheckedChanged(object sender, EventArgs e)
        {            
            if(rdxDebugMUXDAC.Checked)
            {
                cboDebugMUXCh.Items.Clear();
                cboDebugMUXCh.Items.AddRange(new object[] {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16"});
                cboDebugMUXCh.Text = "1";
            }
        }

        private void rdxDebugMUXADC_CheckedChanged(object sender, EventArgs e)
        {
            if (rdxDebugMUXADC.Checked)
            {
                cboDebugMUXCh.Items.Clear();
                cboDebugMUXCh.Items.AddRange(new object[] {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16"});
                cboDebugMUXCh.Text = "1";
            }
        }

        private void rdxDebugMUXCAP_CheckedChanged(object sender, EventArgs e)
        {
            if (rdxDebugMUXCAP.Checked)
            {
                cboDebugMUXCh.Items.Clear();
                cboDebugMUXCh.Items.AddRange(new object[] {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
                "17",
                "18",
                "19",
                "20",
                "21",
                "22",
                "23",
                "24",
                "25",
                "26",
                "27",
                "28",
                "29",
                "30",
                "31",
                "32"});
                cboDebugMUXCh.Text = "1";
            }
        }         
	}
}