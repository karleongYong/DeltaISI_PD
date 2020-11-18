using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DesktopTester.Settings;

namespace DesktopTester.UserControls
{
    public partial class UserPasswordChangeForm : Form
    {
        private User _user;

        public string NewPassword
        {
            get;
            private set;
        }

        public UserPasswordChangeForm(User user)
        {
            InitializeComponent();

            _user = user;
        }

        private void UserPasswordChangeForm_Load(object sender, EventArgs e)
        {
            txtUser.Text = _user.Name;
        }

        private void txtNewPass2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                SubmitNewPassword(sender as Control);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SubmitNewPassword(sender as Control);
        }

        private void SubmitNewPassword(Control control)
        {
            try
            {
                if (!String.Equals(_user.Password, txtOldPass.Text))
                    throw new Exception("Old password was not correct.");

                if (!String.Equals(txtNewPass1.Text, txtNewPass2.Text))
                    throw new Exception("New passwords did not match.");

                NewPassword = txtNewPass1.Text;

                this.Close();
            }
            catch (Exception ex)
            {
                txtOldPass.Text = "";
                txtNewPass1.Text = "";
                txtNewPass2.Text = "";
                errorToolTip.SetToolTip(control, ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
