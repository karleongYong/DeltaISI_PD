using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A red background notification banner.
    /// </summary>
    public class ErrorBanner : NotifyBanner
    {
        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        /// <value>
        /// The error details.
        /// </value>
        public string Details
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorBanner"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        public ErrorBanner(string message, string details = "")
        {
            InitializeComponent();

            this.CloseText = "OK";
            this.Message = message;
            this.LightStackColor = IO.LightStackColor.Red;

            if (!String.IsNullOrEmpty(details))
            {
                Details = details;
                AddAction("Details", ShowDetails);
            }
        }

        private void ShowDetails()
        {
            Notify.PopUpError(Message, Details);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // ErrorBanner
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.Tomato;
            this.Name = "ErrorBanner";
            this.ResumeLayout(false);
        }
    }
}