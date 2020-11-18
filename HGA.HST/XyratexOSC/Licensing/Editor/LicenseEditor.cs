using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XyratexOSC.Licensing;
using XyratexOSC.UI;
using XyratexOSC.Utilities;


namespace XyratexOSC.Licensing.Editor
{
    public partial class LicenseEditor : Form
    {
        private Authentication _auth;

        public LicenseEditor()
        {
            InitializeComponent();
        }

        private void LicenseEditor_Load(object sender, EventArgs e)
        {
            pnlConnect.Dock = DockStyle.Fill;

            string[] featureNames = Enum.GetNames(typeof(LicenseFeatures));

            clbFeatures.Items.Clear();

            for (int i = 1; i < featureNames.Length; i++)
                clbFeatures.Items.Add(featureNames[i]);
        }

        private void btnProgram_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            LicenseFeatures features = LicenseFeatures.None;

            Array values = Enum.GetValues(typeof(LicenseFeatures));

            for (int i = 0; i < clbFeatures.Items.Count; i++)
            {
                CheckState state = clbFeatures.GetItemCheckState(i);
                if (state != CheckState.Checked)
                    continue;

                features |= (LicenseFeatures)values.GetValue(i + 1);
            }

            ushort writePass = 0xB838;
            ushort owp1 = 0xE39A;
            ushort owp2 = 0x03FB;

            LicenseLock ll = new LicenseLock();
            ll.XyratexKeyNeeded = true;

            LicenseKeyCode code = ll.SetFeatures(features, writePass, owp1, owp2);

            this.Cursor = this.DefaultCursor;

            if (code == LicenseKeyCode.SUCCESS)
            {
                Notify.PopUp(
                    "Successfully Programmed Features", 
                    "The following features were added: " + features.ToString(), 
                    "", 
                    "OK");
            }
            else
            {
                Notify.PopUpError(
                    "Failed Feature Programming",
                    "Failed to program the current key. " + code.GetDescription(),
                    "");
            }
        }

        private Authentication UpdateKeyStatus()
        {
            this.Cursor = Cursors.WaitCursor;

            LicenseLock ll = new LicenseLock();
            ll.XyratexKeyNeeded = true;

            _auth = ll.Verify();

            if (_auth.Status == LicenseStatus.Key)
            {
                lblKeyStatus.Text = "Key Connected";
                lblKeyStatus.BackColor = Color.Honeydew;
                lblKeyStatus.ForeColor = Color.Green;

                lblKeyDetails.Text = _auth.Details;
                lblKeyDetails.BackColor = Color.Honeydew;
                lblKeyDetails.ForeColor = Color.Green;

                pnlConnect.Visible = false;
                pnlFeatures.Visible = true;
            }
            else
            {
                lblKeyStatus.Text = "No Valid Key";
                lblKeyStatus.BackColor = Color.MistyRose;
                lblKeyStatus.ForeColor = Color.IndianRed;

                lblKeyDetails.Text = "(" + _auth.KeyCode.GetDescription() + ")";
                lblKeyDetails.BackColor = Color.MistyRose;
                lblKeyDetails.ForeColor = Color.IndianRed;

                pnlConnect.Visible = true;
                pnlFeatures.Visible = false;
            }

            this.Cursor = this.DefaultCursor;

            return _auth;
        }

        private void btnReadFeatures_Click(object sender, EventArgs e)
        {
            ReadFeatures();
        }

        private void ReadFeatures()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                LicenseLock ll = new LicenseLock();
                ll.XyratexKeyNeeded = true;

                _auth = ll.Verify();

                if (_auth.Status == LicenseStatus.Key)
                {
                    lblKeyStatus.Text = "Key Connected";
                    lblKeyStatus.BackColor = Color.Honeydew;
                    lblKeyStatus.ForeColor = Color.Green;

                    lblKeyDetails.Text = _auth.Details;
                    lblKeyDetails.BackColor = Color.Honeydew;
                    lblKeyDetails.ForeColor = Color.Green;

                    pnlConnect.Visible = false;
                    pnlFeatures.Visible = true;
                }
                else
                {
                    lblKeyStatus.Text = "No Valid Key";
                    lblKeyStatus.BackColor = Color.MistyRose;
                    lblKeyStatus.ForeColor = Color.IndianRed;

                    lblKeyDetails.Text = "(" + _auth.KeyCode.GetDescription() + ")";
                    lblKeyDetails.BackColor = Color.MistyRose;
                    lblKeyDetails.ForeColor = Color.IndianRed;

                    pnlConnect.Visible = true;
                    pnlFeatures.Visible = false;
                }

                if (_auth.Status != LicenseStatus.Key)
                {
                    for (int i = 0; i < clbFeatures.Items.Count; i++)
                        clbFeatures.SetItemCheckState(i, CheckState.Unchecked);

                    cbAll.Checked = false;
                }
                else
                {
                    Array values = Enum.GetValues(typeof(LicenseFeatures));
                    bool anyUnchecked = false;

                    for (int i = 0; i < clbFeatures.Items.Count; i++)
                    {
                        bool state = _auth.Features.HasFlag((LicenseFeatures)values.GetValue(i + 1));
                        clbFeatures.SetItemCheckState(i, state ? CheckState.Checked : CheckState.Unchecked);

                        if (!state)
                            anyUnchecked = true;
                    }

                    cbAll.Checked = !anyUnchecked;
                }

                lblValue.ForeColor = SystemColors.ControlText;
            }
            finally
            {
                this.Cursor = this.DefaultCursor;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ReadFeatures();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            lblKeyStatus.Text = "Key Disconnected";
            lblKeyStatus.BackColor = Color.MistyRose;
            lblKeyStatus.ForeColor = Color.IndianRed;

            lblKeyDetails.Text = "(click to connect)";
            lblKeyDetails.BackColor = Color.MistyRose;
            lblKeyDetails.ForeColor = Color.IndianRed;

            pnlConnect.Visible = true;
            pnlFeatures.Visible = false;
        }

        private void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < clbFeatures.Items.Count; i++)
                clbFeatures.SetItemCheckState(i, cbAll.Checked ? CheckState.Checked : CheckState.Unchecked);
        }

        private void clbFeatures_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Int16 value = 0;
            Array values = Enum.GetValues(typeof(LicenseFeatures));

            for (int i = 0; i < clbFeatures.Items.Count; i++)
            {
                CheckState state;

                if (i == e.Index)
                    state = e.NewValue;
                else
                    state = clbFeatures.GetItemCheckState(i);

                if (state == CheckState.Checked)
                {
                    LicenseFeatures enumValue = (LicenseFeatures)values.GetValue(i + 1);

                    if ((int)enumValue > 0)
                        value |= (Int16)enumValue;
                }
            }

            lblValue.Text = String.Format("0x{0:X4}", value);

            if ((Int16)_auth.Features == value)
                lblValue.ForeColor = SystemColors.ControlText;
            else
                lblValue.ForeColor = Color.DodgerBlue;
        }
    }
}
