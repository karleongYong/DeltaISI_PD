//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2006/11/29] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for TouchScreenListBox.
	/// </summary>
	public class TouchScreenListBox : System.Windows.Forms.UserControl
	{
        private System.Windows.Forms.Button _btnUp;
        private System.Windows.Forms.Button _btnDown;
        private System.Windows.Forms.ListBox _lbContent;
        private SplitContainer splitContainer1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TouchScreenListBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TouchScreenListBox));
            this._btnUp = new System.Windows.Forms.Button();
            this._btnDown = new System.Windows.Forms.Button();
            this._lbContent = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnUp
            // 
            this._btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnUp.Image = ((System.Drawing.Image)(resources.GetObject("_btnUp.Image")));
            this._btnUp.Location = new System.Drawing.Point(-1, 0);
            this._btnUp.Margin = new System.Windows.Forms.Padding(0);
            this._btnUp.Name = "_btnUp";
            this._btnUp.Size = new System.Drawing.Size(48, 48);
            this._btnUp.TabIndex = 0;
            this._btnUp.Click += new System.EventHandler(this._btnUp_Click);
            // 
            // _btnDown
            // 
            this._btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnDown.Image = ((System.Drawing.Image)(resources.GetObject("_btnDown.Image")));
            this._btnDown.Location = new System.Drawing.Point(-1, 47);
            this._btnDown.Margin = new System.Windows.Forms.Padding(0);
            this._btnDown.Name = "_btnDown";
            this._btnDown.Size = new System.Drawing.Size(48, 48);
            this._btnDown.TabIndex = 1;
            this._btnDown.Click += new System.EventHandler(this._btnDown_Click);
            // 
            // _lbContent
            // 
            this._lbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lbContent.Location = new System.Drawing.Point(0, 0);
            this._lbContent.Name = "_lbContent";
            this._lbContent.Size = new System.Drawing.Size(97, 95);
            this._lbContent.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._lbContent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._btnUp);
            this.splitContainer1.Panel2.Controls.Add(this._btnDown);
            this.splitContainer1.Size = new System.Drawing.Size(145, 95);
            this.splitContainer1.SplitterDistance = 97;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 4;
            // 
            // TouchScreenListBox
            // 
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(145, 95);
            this.Name = "TouchScreenListBox";
            this.Size = new System.Drawing.Size(145, 95);
            this.Resize += new System.EventHandler(this.TouchScreenListBox_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void _btnUp_Click(object sender, System.EventArgs e)
        {
            int sel = _lbContent.SelectedIndex;

            if(sel< 0) return;

            if(--sel < 0)
                sel = _lbContent.Items.Count -1;

            _lbContent.SetSelected(sel, true);
        }
       
        private void _btnDown_Click(object sender, System.EventArgs e)
        {
            int sel = _lbContent.SelectedIndex;

            if(sel< 0) return;

            if(++sel >= _lbContent.Items.Count)
                sel = 0;

            _lbContent.SetSelected(sel, true);            
        }              

        private void TouchScreenListBox_Resize(object sender, System.EventArgs e)
        {        
            if(this.DesignMode) return;

            System.Drawing.Size lbSize = this.Size;
            lbSize.Width = lbSize.Width - _btnUp.Width + 19;                 // 10 is size of scroll bar to cover up.  up and down button should be same width
            _lbContent.Size = lbSize;
            _lbContent.Location = new System.Drawing.Point(0, 0);
        }

        public ListBox ListBox
        {
            get { return _lbContent; }
        }
	}
}
