//
//  Copyright MMI System Pte Ltd 2009
//  All Rights Reserved.
//
//  20091211, Developer, Design Engineering Deparment
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.UI;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using System.IO;
using Seagate.AAS.HGA.HST.SeagateDataBaseLogIn;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace Seagate.AAS.HGA.HST.Utils
{
    public partial class GIDEntryForm : Form
    {
        public string EmployeeGID
        {
            get;
            set;
        }

        public GIDEntryForm()
        {
            InitializeComponent();
        }

        private void GIDEntryForm_Load(object sender, EventArgs e)
        {            
            txtGID.Text = HSTMachine.Workcell.HSTSettings.OperatorGID;
        }

        public static GIDLoginDialogResults ShowLoadForm()
        {
            GIDLoginDialogResults results = new GIDLoginDialogResults();

            UIUtility.Invoke(Application.OpenForms[0], () =>
            {
                using (GIDEntryForm form = new GIDEntryForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        results.EmployeeGID = form.txtGID.Text;
                    }
                }
            });

            return results;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtGID.Text) == true)
            {
                Notify.PopUpError("Missing Employee GID", "Cannot authenticate user credential due to missing GID");
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            else if (String.IsNullOrEmpty(txtPassword.Text) == true)
            {
                Notify.PopUpError("Missing Password", "Cannot authenticate user credential due to missing password");
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            Service LoginService = new Service();
            LoginInfo _userInfo = LoginService.Authenticate(LdapServer.OKC, txtGID.Text, txtPassword.Text);

            if(_userInfo.LoginOK)
            {                
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                Notify.PopUpError("User Login Failed", _userInfo.ErrDesc);
                this.DialogResult = DialogResult.Cancel;
            }
        }        

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            EmployeeGID = "";
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }        
    }

    public class GIDLoginDialogResults
    {
        public string EmployeeGID
        {
            get;
            set;
        }
    }
}