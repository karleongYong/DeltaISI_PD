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

using Seagate.AAS.IO;
using Seagate.AAS.IO.Serial;

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
	/// <summary>
	/// Summary description for FrmCommParam.
	/// </summary>
	public partial class CommParamForm : System.Windows.Forms.Form
	{
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private SerialPortSettings _portSettings;

        // Constructors & Finalizers -------------------------------------------

        public CommParamForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        // Properties ----------------------------------------------------------
        
        public SerialPortSettings PortSettings
	    {
		    get { return _portSettings; }
		    set { _portSettings = value; serialPortSettingsPanel1.PortSettings = _portSettings; }
	    }

        // Methods -------------------------------------------------------------

        private void btnOK_Click(object sender, EventArgs e)
        {
            _portSettings = serialPortSettingsPanel1.PortSettings;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }

        // Event handlers ------------------------------------------------------
        
        // Interface implementation --------------------------------------------
        
        // Internal methods ----------------------------------------------------
        
	}
}
