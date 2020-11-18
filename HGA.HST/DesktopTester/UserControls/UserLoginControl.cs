using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using DesktopTester.Settings;


namespace DesktopTester.UserControls
{
    public partial class UserLoginControl : UserControl
    {
        public UserLoginControl()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(100);
            _timer.SynchronizingObject = this;
            _timer.Elapsed += new ElapsedEventHandler(timerElapsed);
        }

        public UserLoginControl(ApplicationSettings settings)
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(100);
            _timer.SynchronizingObject = this;
            _timer.Elapsed += new ElapsedEventHandler(timerElapsed);
            
            _userAccess = settings.getUserAccessSettings();                       
        }

        private UserAccessSettings _userAccess;
        private System.Timers.Timer _timer;


        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login(sender as Control);
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Login(sender as Control);
            }
        }

        private void Login(Control control)
        {
            try
            {
                _userAccess.LogIn(txtName.Text, txtPassword.Text);

                txtPassword.Text = "";
                txtName.Text = "";

                Control parent = this;

                while (parent.Parent != null)
                    parent = parent.Parent;

                if (parent is Form)
                    ((Form)parent).Close();
                else if (parent is Control)
                    parent.Hide();
            }
            catch (Exception ex)
            {
                this.txtName.Focus();
                this.txtName.SelectAll();

                errorToolTip.Show(ex.Message, control);
            }
            finally
            {
                txtPassword.Text = "";
                _timer.Enabled = false;
            }
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;

        }

        private void txtName_VisibleChanged(object sender, EventArgs e)
        {
            if (this.txtName.Visible)
                _timer.Enabled = true;
        }
    }
}
