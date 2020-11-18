using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.Logging;

namespace BenchTestsTool.UserControls
{
    public partial class PageLog : UserControl
    {
        private RollingListBoxListener rlbListener;

        public PageLog()
        {
            InitializeComponent();

            rlbListener = new RollingListBoxListener(listBox);
            rlbListener.AutoScrollToBottom = true;

            TraceSource trace = XyratexOSC.Logging.Log.Trace();
            trace.Listeners.Add(rlbListener);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            rlbListener.Close();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void CopyToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            foreach (object line in listBox.SelectedItems)
                sb.AppendLine(line.ToString());

            string clipboardText = sb.ToString();

            if (!String.IsNullOrEmpty(clipboardText))
                Clipboard.SetText(clipboardText);
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C) || 
                e.KeyData == (Keys.Control | Keys.Shift | Keys.C))
            {
                CopyToClipboard();
            }
        }

        private void PageLog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                XyratexOSC.Logging.Log.FlushAllTraces();
        }
        
        private void listBox_Resize(object sender, EventArgs e)
        {
            if (rlbListener != null && rlbListener.AutoScrollToBottom)
                rlbListener.ScrollToBottom();
        }
    }
}
