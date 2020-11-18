using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI.Operation.WorkOrder
{
    public partial class WOInformationPanel : UserControl
    {
        private Font TextFont = new Font("Arial", 9);
        public MyEventArgs MyArgs = new MyEventArgs();

        //Events
        public event EventHandler ValueChanged;
        public class MyEventArgs : EventArgs
        {
            private string myControl;
            public string MyControl
            {
                get { return myControl; }
                set { myControl = value; }
            }
        }

        protected virtual void OnValueChanged(object sender, MyEventArgs e)
        {
            EventHandler eh = ValueChanged;
            if (eh != null)
                eh(this, e);
        }

        public void RaiseEvent(object sender, MyEventArgs e)
        {
            OnValueChanged(sender, e);
        }
       
        //Constructor
        public WOInformationPanel()
        {
            InitializeComponent();
            if (HSTMachine.Workcell == null)
                return;
            HSTMachine.Workcell.WorkOrder.OnLoadingWOChanged += new WorkOrderInfo.LoadingWOChangedHandler(WorkOrder_OnLoadingWOChanged);
        }       

        public void WorkOrder_OnLoadingWOChanged()
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                txtWONumber.Text = HSTMachine.Workcell.WorkOrder.Loading.WorkOrderNo;
                txtVersion.Text = HSTMachine.Workcell.WorkOrder.Loading.Version;
                txtProductName.Text = HSTMachine.Workcell.WorkOrder.Loading.ProductName;
                txtRel.Text = HSTMachine.Workcell.WorkOrder.Loading.Released;
                txtTray.Text = HSTMachine.Workcell.WorkOrder.Loading.TrayType;
                txtCreateDate.Text = HSTMachine.Workcell.WorkOrder.Loading.CreateDate;
                txtWorkOrderDescription.Text = HSTMachine.Workcell.WorkOrder.Loading.Description;
                txtAuthor.Text = HSTMachine.Workcell.WorkOrder.Loading.Author;
                txtHGAPartNo.Text = HSTMachine.Workcell.WorkOrder.Loading.HgaPartNo;
                txtFSAPartNo.Text = HSTMachine.Workcell.WorkOrder.Loading.FsaPartNo;
                txtSLSProgramName.Text = HSTMachine.Workcell.WorkOrder.Loading.LoadingProgramName;
                txtFSAVendor.Text = HSTMachine.Workcell.WorkOrder.Loading.FsaVendor;
            });
        }

        private void WOInformationPanel_Load(object sender, EventArgs e)
        {
            if (HSTMachine.Workcell == null)
                return;
        }          
    }
}
