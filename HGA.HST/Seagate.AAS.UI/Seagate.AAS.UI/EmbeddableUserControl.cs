/////////////////////////////////////////////////////////////////////////////
//
//  (c) Copyright 2008 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2010/03/03] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////
			
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    public partial class EmbeddableUserControl : UserControl
    {
        // Member Variables ------------------------------------
        private const string _visualStudioDesigner = "devenv";

        // Constructors -----------------------------------------
        public EmbeddableUserControl()
        {
            InitializeComponent();
        }

        // Methods -----------------------------------------------------

        /// <summary>
        /// This is a workaround for a bug in Visual Studio. DesignMode property does not behave properly
        /// when user control are embedded in another user control. This is a known bug and is closed as "Won't Fix"
        /// as of 2010/03/03.
        /// 
        /// Response from Visual Studio Team on this bug:
        /// ----------------------------------------------------
        /// Thank you for your bug report. Customer feedback is a critical part of a successful, 
        /// impact full release. Unfortunately, we will not be able to address this particular issue for the following reason:
        /// While this is a known issue, it is an issue that we cannot fix. If we change the behavior here we run the risk of 
        /// introducing breaking changes to fielded applications. Thank you for contributing to Windows Forms.
        /// </summary>
        public new bool DesignMode
        {
            get { return System.Diagnostics.Process.GetCurrentProcess().ProcessName == _visualStudioDesigner; }
        }
    }
}
