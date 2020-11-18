using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using Seagate.AAS.Security.AccessControl;
using System.Threading;
namespace Seagate.AAS.HGA.HST.UI.Utils
{
    public partial class CCCAlertForm : Form
    {
        public enum AlertType
        {
            Unknown,
            Inspect,
            Acknowledge
        }

        int _flashcounter = 0;
        AlertType _currentAlertType = AlertType.Unknown;

        public CCCAlertForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _flashcounter++;

            if (_flashcounter > 10 && _flashcounter <= 20)
            {
                if (_currentAlertType == AlertType.Inspect)
                    this.BackColor = Color.FromArgb(192, 64, 0);
                else if (_currentAlertType == AlertType.Acknowledge)
                    this.BackColor = Color.Lime;

            }
            else if (_flashcounter > 20)
            {
                this.BackColor = SystemColors.Control;
                _flashcounter = 0;
            }
        }

        private void CCCAlertForm_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                timer1.Enabled = Visible;
                this.TopMost = true;
                UserAccessControl.UserLogin -= UserAccessControl_UserLogin;
                UserAccessControl.UserLogout -= UserAccessControl_UserLogout;
                UserAccessControl.UserLogin += UserAccessControl_UserLogin;
                UserAccessControl.UserLogout += UserAccessControl_UserLogout;
            }
        }

        void UserAccessControl_UserLogout()
        {
            //throw new NotImplementedException();
        }

        void UserAccessControl_UserLogin(UserInformation userInfo)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UserAccessControl_UserLogin(userInfo)));
            }
            else
            {
                if (userInfo != null)
                {
                    if (userInfo.JobFunction == "operator")
                        MessageBox.Show("This logging in user was not allow to acknowledge alert, please try again!");
                    else
                    {
                        FormCollection fa = Application.OpenForms;
                        foreach (Form item in fa)
                        {
                            if (item.Text.Trim().ToLower() == "ancalertform")
                            {
                                HSTMachine.Workcell.CurretCCCActiveStatus.ChangeActiveStatus(false);
                                //HSTMachine.Workcell.TicMcFailureCounter.Reset();
                                HSTMachine.Workcell.TicMcFailureCounter.ResetByMC(HSTMachine.Workcell.CCCFailureInfo.FailedMc);
                                if(HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.IsTriggering)
                                {
                                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.Default();
                                }
                                if (HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.IsTriggering)
                                {
                                    HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.Default();
                                }

                                HSTMachine.Workcell.CCCMachineTriggeringDown = true;
                                HSTMachine.Workcell.HSTSettings.Save();
                                item.Close();
                                break;
                            }
                        }

                    }

                }
            }
        }


        public void AssignAlert(AlertType alerttype, string alertMsg,string topic,string uticErrType, string dockNo)
        {
            labelMessage.Text = alertMsg;
            labelTopic.Text = topic;
            labelDockNumber.Text = dockNo;

            if(!string.IsNullOrEmpty(uticErrType))
            {
                labelUticFailType.Text = uticErrType;
                labelUticFailType.Visible = true;
            }
            else
                labelUticFailType.Visible = false;

            if(!string.IsNullOrEmpty(dockNo))
            {
                labelDockNumber.Text = dockNo;
                labelDockNumber.Visible = true;
            }
            else
                labelDockNumber.Visible = false;

            _currentAlertType = alerttype;
            if(alerttype == AlertType.Inspect)
            {
                buttonHSTDefect.Visible = false;
                buttonTICDefect.Visible = false;
                buttonAcknowledge.Visible = false;
                buttonReleaseCarrier.Visible = true;
            }else if(alerttype == AlertType.Acknowledge)
            {
                buttonReleaseCarrier.Visible = false;
                buttonHSTDefect.Visible = false;
                buttonTICDefect.Visible = false;
                buttonAcknowledge.Visible = true;
            }
        }

        private void buttonTICDefect_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.RaiseCCCDefectUpdated(new CCCDefectSelection(CCCAlertInformation.CCCMcDefect.TICDefect));
            Close();
        }

        private void buttonHSTDefect_Click(object sender, EventArgs e)
        {
            HSTMachine.Workcell.RaiseCCCDefectUpdated(new CCCDefectSelection(CCCAlertInformation.CCCMcDefect.HSTDefect));
            Close();
        }

        private void buttonAcknowledge_Click(object sender, EventArgs e)
        {
            UserAccessControl.SetSessionTimeOut(100);
            UserAccessControl.Login();
        }

        private void buttonReleaseCarrier_Click(object sender, EventArgs e)
        {
            int delay = Convert.ToInt32(HSTMachine.Workcell.HSTSettings.CccParameterSetting.DelayForReleaseCarrier * 1000);
            Thread.Sleep(delay);
            HSTMachine.Workcell.CCCMachineTriggeringActivated = false;
            buttonHSTDefect.Visible = true;
            buttonTICDefect.Visible = true;
            buttonReleaseCarrier.Visible = false;
            buttonReleaseCarrier.Refresh();
            labelMessage.Text = "Please inspect on Tail pad and TIC joint.Then select bellow button in the cause of defect that you see.";
            HSTMachine.Instance.MainForm.getPanelCommand().stopSystemProcess();
        }
    }
}
