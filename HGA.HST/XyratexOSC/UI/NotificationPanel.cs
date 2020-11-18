using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A panel that catches banner notifications raised in the application, and docks them to the top of the panel.
    /// </summary>
    public partial class NotificationPanel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPanel"/> class.
        /// </summary>
        public NotificationPanel()
        {
            InitializeComponent();
            Notify.Instance.BannerNotified += new EventHandler<NotifyBannerEventArgs>(BannerNotified);
        }

        /// <summary>
        /// Closes this notification panel instance.
        /// </summary>
        public void Close()
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                NotifyBanner banner = control as NotifyBanner;
                if (banner == null)
                    continue;

                banner.Close();
            }
        }

        private void BannerNotified(object sender, NotifyBannerEventArgs e)
        {
            if (e.Banner == null)
                return;

            if (e.Banner is WizardBanner)
            {
                var wizards = tableLayoutPanel.Controls.OfType<WizardBanner>();
                if (wizards.Count() > 0)
                {
                    e.Banner.Dispose();
                    return;
                }
            }

            UIUtility.Invoke(this, () =>
            {
                e.Banner.Dock = DockStyle.Fill;
                e.Banner.Margin = new System.Windows.Forms.Padding(0);
                e.Banner.Padding = new System.Windows.Forms.Padding(0);
                e.Banner.Closed += new EventHandler(CloseBanner);

                this.SuspendLayout();
                tableLayoutPanel.SuspendLayout();

                this.Height += e.Banner.Height;

                tableLayoutPanel.RowCount++;
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 0));
                tableLayoutPanel.Controls.Add(e.Banner, 0, tableLayoutPanel.RowCount - 1);

                tableLayoutPanel.ResumeLayout();
                this.ResumeLayout();
                this.Show();
            });
        }

        private void CloseBanner(object sender, EventArgs e)
        {
            var banner = sender as NotifyBanner;
            if (banner == null)
                return;

            if (!tableLayoutPanel.Controls.Contains(banner))
                return;

            this.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            
            int row = tableLayoutPanel.GetRow(banner);
            tableLayoutPanel.Controls.Remove(banner);
            tableLayoutPanel.RowStyles.RemoveAt(row);
            tableLayoutPanel.RowCount--;
            // TableLayoutPanel does not update the row number of a control automatically (a bug!)
            // So if we don't update the Row #, we get invalid index if there multiple rows  
            foreach (var item in tableLayoutPanel.Controls)
            {
                Control ctrl = (Control)item;
                int oldRow = tableLayoutPanel.GetRow(ctrl);
                if (oldRow > row) // decrement the row number by 1
                {
                    tableLayoutPanel.SetRow(ctrl, oldRow - 1);
                }
            }

            this.Height -= banner.Height;

            if (tableLayoutPanel.RowCount < 1)
                this.Hide();

            tableLayoutPanel.ResumeLayout();
            this.ResumeLayout();
        }
    }
}