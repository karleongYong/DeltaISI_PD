using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.UI;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public partial class PanelSingleAxis : EmbeddableUserControl
    {
        private IAxis axis;
        DateTime homeStart;
        bool checkHomedStatus = false;

        public PanelSingleAxis()
        {
            InitializeComponent();
            
            if (DesignMode) return;

            axis = null;
            tmrUpdate.Enabled = false;
        }

        public void AssignAxis(IAxis axis)
        {
            if (DesignMode) return;

            this.axis = axis;

            Axis _axis = (Axis)axis;
            groupBox1.Text = _axis.Name;
            lbAxisNo.Text = _axis.PointId.ToString();

            tmrUpdate.Enabled = true;
        }

        public void AssignAxis(IAxis axis, IMoveProfile moveProfile)
        {
            if (DesignMode) return;

            this.axis = axis;

            Axis _axis = (Axis)axis;
            groupBox1.Text      = _axis.Name;
            lbAxisNo.Text       = _axis.PointId.ToString();
            tNumAcceleration.Text = moveProfile.Acceleration.ToString();
            tNumVelocity.Text     = moveProfile.Velocity.ToString();

            //tmrUpdate.Enabled = true;
        }

        private void DisplayError(string message)
        {
            if (message == "")
            {
                labelStatus.Visible = false;
            }
            else
            {
                labelStatus.Visible = true;
                labelStatus.ForeColor = Color.White;
                labelStatus.BackColor = Color.Red;
                labelStatus.Text = message;
            }
        }

        private void DisplayStatus(string message)
        {
            labelStatus.ForeColor = Color.Black;
            labelStatus.BackColor = System.Drawing.SystemColors.Control;
            labelStatus.Text = message;
        }

        private void PanelSingleAxis_VisibleChanged(object sender, EventArgs e)
        {
            if ((axis != null) && !DesignMode)
                tmrUpdate.Enabled = Visible;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    axis.HomeStart();
                    checkHomedStatus = true;
                    homeStart = DateTime.Now;
                    DisplayStatus("Home started");
                }
                catch (Exception ex)
                {
                    DisplayError("Home error: " + ex.Message);
                }
            }
        }

        public void EnableAxis()
        {
            if (axis != null)
            {
                try
                {
                    axis.Enable(true);
                    DisplayStatus("Enabled");
                }
                catch (Exception ex)
                {
                    DisplayError("Enable error: " + ex.Message);
                }
            }
        }

        public void ledEnable_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    if (ledEnable.State)
                    {
                        axis.Enable(false);
                        DisplayStatus("Disabled");
                    }
                    else
                    {
                        axis.Enable(true);
                        DisplayStatus("Enabled");
                    }
                }
                catch (Exception ex)
                {
                    DisplayError("Enable error: " + ex.Message);
                }
            }
        }

        private void btnJogPositive_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    axis.MoveRelativeStart(double.Parse(tNumAcceleration.Text), double.Parse(tNumVelocity.Text), double.Parse(tbPosition.Text));
                    DisplayStatus("Moved relative " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnJogNegative_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    axis.MoveRelativeStart(double.Parse(tNumAcceleration.Text), double.Parse(tNumVelocity.Text), -double.Parse(tbPosition.Text));
                    DisplayStatus("Moved relative " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnFreeRunPositive_MouseDown(object sender, MouseEventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    _axis.FreeRun(double.Parse(tNumVelocity.Text));
                    DisplayStatus("Moved started to " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnFreeRunPositive_MouseUp(object sender, MouseEventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    _axis.FreeRunStop();
                    DisplayStatus("Moved started to " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnFreeRunNegative_MouseDown(object sender, MouseEventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    _axis.FreeRun(-double.Parse(tNumVelocity.Text));
                    DisplayStatus("Moved started to " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnFreeRunNegative_MouseUp(object sender, MouseEventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    _axis.FreeRunStop();
                    DisplayStatus("Moved started to " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnMoveAbs_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    axis.MoveAbsoluteStart(double.Parse(tNumAcceleration.Text), double.Parse(tNumVelocity.Text), double.Parse(tbPosition.Text));
                    DisplayStatus("Moved started to " + tbPosition.Text);
                }
                catch (Exception ex)
                {
                    DisplayError("Move error: " + ex.Message);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    checkHomedStatus = false;
                    axis.Stop();
                    DisplayStatus("Stopped");

                }
                catch (Exception ex)
                {
                    DisplayError("Stop error: " + ex.Message);
                }
            }
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                tmrUpdate.Enabled = false;
                return;
            }

            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    ledEnable.State     = _axis.IsEnabled;
                    ledhomed.State      = _axis.IsHomed;
                    ledInPosition.State = _axis.IsMoveDone;
                    ledFault.State      = _axis.IsAxisFault;

                    lbPosition.Text     = _axis.GetActualPosition().ToString("0.000");

                    btnHome.Enabled     = _axis.IsEnabled;
                    btnMoveAbs.Enabled  = _axis.IsEnabled;
                    btnStop.Enabled     = _axis.IsEnabled;
                    btnFaultAck.Visible = _axis.IsAxisFault;

                    if (checkHomedStatus)
                    {
                        TimeSpan elapsed = DateTime.Now.Subtract(homeStart);
                        if (elapsed.Seconds > 10)
                        {
                            checkHomedStatus = false;
                            if (!axis.IsHomed)
                            {
                                DisplayError("Failed to home within 10 seconds");
                            }
                        }
                    }

                    //if (_axis.IsAxisFault) DisplayError("Axis fault. Please click \"Ack.\" button to continue.");
                }
                catch (Exception ex)
                {
                    DisplayError("Status update error: " + ex.Message);
                }
            }
        }

        private void btnFaultAck_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                try
                {
                    Axis _axis = (Axis)axis;
                    _axis.FaultAcknowledge();
                    DisplayError("");
                }
                catch (Exception ex)
                {
                    //DisplayError("Fault Acknowledge error: " + ex.Message);
                }
            }
        }
    }
}
