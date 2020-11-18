using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI.Operation.WorkOrder
{
    public partial class UnloadInformation : UserControl
    {
        public UnloadInformation()
        {
            InitializeComponent();
        }

        private void UnloadInformation_Load(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell == null)
                return;

            HSTMachine.Workcell.Process.InputStationProcess.Controller.OnUploadUnloadWO += new Seagate.AAS.HGA.HST.Controllers.InputStationController.OnUpdateUnloadWOHandler(Controller_OnUploadUnloadWO);
        
        }

        void Controller_OnUploadUnloadWO(string carrierID)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {                
                txtCurrentWorkOrderNo.Text = HSTMachine.Workcell.WorkOrder.Loading.WorkOrderNo;
                txtCurrentVersion.Text = HSTMachine.Workcell.WorkOrder.Loading.Version;
                txtCurrentProductName.Text = HSTMachine.Workcell.WorkOrder.Loading.ProductName;
                txtCurrentCarrierID.Text = carrierID;
                txtCurrentTray.Text = HSTMachine.Workcell.WorkOrder.Loading.TrayType;
            });
        }
    }
}
