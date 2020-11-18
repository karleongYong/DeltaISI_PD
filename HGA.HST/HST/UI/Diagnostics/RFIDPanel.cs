using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Equipment.UI;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;

namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    public partial class RFIDPanel : UserControl
    {
        private HSTWorkcell _workcell;        

        private ReadWriteRFIDController folaRFIDController;
        private FolaTagData folaTagData = new FolaTagData();
        private RFHead folaRFIDHead;

        public RFIDPanel()
        {
            InitializeComponent();
        }
                
        private void RFIDPanel_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            _workcell = HSTMachine.Workcell;
            if (HSTMachine.Workcell == null)
                return;
            folaTagDataPanel1.ReadOnly = !btWriteFola.Visible;
        }        

        private void FolaEnableButtons(bool enable)
        {            
            btReadFola.Enabled = enable;
            btWriteFola.Enabled = enable;
        }
        
        private void btWriteFola_Click(object sender, EventArgs e)
        {
            FolaEnableButtons(false);
            try
            {
                folaRFIDController = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidController;
                if (folaRFIDController == null)
                {
                    folaRFIDController = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidController;
                    folaRFIDController.RFIDScanner.Initialize(false);
                }

                if (rdxInputStation.Checked)
                {                    
                    folaRFIDHead = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfHead;                                        
                }
                else
                {                    
                    folaRFIDHead = HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfHead;                    
                }
                folaTagDataPanel1.UpdateTag(folaTagData);
                folaRFIDController.RFIDScanner.WriteRFIDTag(folaRFIDHead, folaTagData, true);
                lblFolaStatus.Text = string.Format("Status: OK. Write time: {0} msec", folaRFIDController.RFIDScanner.ReadTime.ToString());              
            }
            catch (Exception ex)
            {
                lblFolaStatus.Text = string.Format("Status: Error: {0}", ex.Message);
            }
            FolaEnableButtons(true);
        }

        private void btReadFola_Click(object sender, EventArgs e)
        {
            FolaEnableButtons(false);
            try
            {
                folaRFIDController = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidController;
                if (folaRFIDController == null)
                {
                    folaRFIDController = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfidController;
                    folaRFIDController.RFIDScanner.Initialize(false);
                }

                if (rdxInputStation.Checked)
                {                    
                    folaRFIDHead = HSTMachine.Workcell.Process.InputStationProcess.Controller.RfHead;                    
                }
                else
                {                    
                    folaRFIDHead = HSTMachine.Workcell.Process.OutputStationProcess.Controller.RfHead;                    
                }
                folaTagDataPanel1.ClearTag(folaTagData);
                folaRFIDController.RFIDScanner.ReadRFIDTag(folaRFIDHead, ref folaTagData);
                folaTagDataPanel1.DisplayTagData(folaTagData);
                lblFolaStatus.Text = string.Format("Status: OK. Read time: {0} msec", folaRFIDController.RFIDScanner.ReadTime.ToString());              
            }
            catch (ExceptionRFID rfidEx)
            {
                if (rfidEx.ErrorCodeRFID == ErrorCodeRFID.CHECKSUM_ERR)
                {
                    folaTagDataPanel1.DisplayTagData(folaTagData);
                    lblFolaStatus.Text = string.Format("Status: Error: {0}{1}Read time: {2} msec", rfidEx.Message, Environment.NewLine, folaRFIDController.RFIDScanner.ReadTime.ToString());
                }
                else lblFolaStatus.Text = string.Format("Status: Error: {0}", rfidEx.Message);
            }
            catch (Exception ex)
            {
                lblFolaStatus.Text = string.Format("Status: Error: {0}", ex.Message);
            }
            FolaEnableButtons(true);
        }
    }
}
