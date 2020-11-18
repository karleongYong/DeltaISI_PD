//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/09/28] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
	/// <summary>
	/// Summary description for FrmHgaRfidDialog.
	/// </summary>
	public partial class BolaTagDataForm : System.Windows.Forms.Form
	{
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private BolaTagData tag = new BolaTagData();
        private BolaReader _reader;
        private bool _readOnly = true;
        // Constructors & Finalizers -------------------------------------------

		public BolaTagDataForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            if (!DesignMode)
            {
                _reader = new BolaReader();
                try
                {
                    if (!_reader.Initialized)
                    {
                        _reader.Initialize(false);
                        lblStatus.Text = "OK";
                        lPort.Text = string.Format("Port: {0}", _reader.PortSettings.PortName);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = string.Format("Error: {0}", ex.Message);
                }
            }
		}

        // Properties ----------------------------------------------------------

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                bolaTagDataPanel1.ReadOnly = _readOnly;
                btnWritetag.Enabled = !_readOnly;
                btnWritetag.Visible = !_readOnly;
            }
        }

        // Methods -------------------------------------------------------------

        // Event handlers ------------------------------------------------------

        private void btnReadtag_Click(object sender, EventArgs e)
        {
            EnableButtons(false);
            try
            {
                _reader.ReadRFIDTag(RFHead.Head1, ref tag);
                bolaTagDataPanel1.DisplayTagData(tag);
                lblStatus.Text = string.Format("OK. Read time: {0} msec", _reader.ReadTime.ToString());
            }
            catch (ExceptionRFID rfidEx)
            {                
                if (rfidEx.ErrorCodeRFID == ErrorCodeRFID.CHECKSUM_ERR)
                {
                    bolaTagDataPanel1.DisplayTagData(tag);
                    lblStatus.Text = string.Format("Error: 0x{0:X2} - {1}{2}Read time: {3} msec", rfidEx.ErrorCode, rfidEx.Message, Environment.NewLine, _reader.ReadTime.ToString());
                }
                else lblStatus.Text = string.Format("Error: 0x{0:X2} - {1}", rfidEx.ErrorCode, rfidEx.Message);
            }
            catch (Exception ex)
            {
                lblStatus.Text = string.Format("Error: {0}", ex.Message);
            }
            EnableButtons(true);
        }

        private void btnWritetag_Click(object sender, EventArgs e)
        {
            EnableButtons(false);
            try
            {
                bolaTagDataPanel1.UpdateTag(tag);
                _reader.WriteRFIDTag(RFHead.Head1, tag, true);
                lblStatus.Text = string.Format("OK. Write time: {0} msec", _reader.WriteTime.ToString());
            }
            catch (ExceptionRFID rfidEx)
            {
                lblStatus.Text = string.Format("Error: 0x{0:X2} - {1}", rfidEx.ErrorCode, rfidEx.Message);
            }
            catch (Exception ex)
            {
                lblStatus.Text = string.Format("Error: {0}", ex.Message);
            }
            EnableButtons(true);
        }

        private void btnRepair_Click(object sender, EventArgs e)
        {

        }

        private void btnCommParam_Click(object sender, EventArgs e)
        {
            CommParamForm portSetup = new CommParamForm();
            portSetup.PortSettings = _reader.PortSettings;
            portSetup.ShowDialog(this);
            if (portSetup.DialogResult == DialogResult.OK)
            {
                try
                {
                    _reader.Initialize(_reader.Simulation, portSetup.PortSettings);
                    lblStatus.Text = "OK";
                    lPort.Text = string.Format("Port: {0}", _reader.PortSettings.PortName);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = string.Format("Error: {0}", ex.Message);
                }
            }
        }

        private void btnIdcancel_Click(object sender, EventArgs e)
        {

        }

        private void chkVerify_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoAsciimode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoHexmode_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------

        private void EnableButtons(bool enable)
        {
            btnCommParam.Enabled = enable;
            btnIdcancel.Enabled = enable;
            btnRepair.Enabled = enable;
            btnWritetag.Enabled = enable;
            btnReadtag.Enabled = enable;
        }

        private void BolaTagDataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           _reader.ShutDown();
        }
	}
}
