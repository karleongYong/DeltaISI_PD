using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.Logging;

namespace XyratexOSC.UI
{
    /// <summary>
    /// An enhanced MessageBox style notification form.
    /// </summary>
    public partial class NotifyForm : Form
    {
        /// <summary>
        /// Gets the form buttons.
        /// </summary>
        /// <value>
        /// The form buttons.
        /// </value>
        public IEnumerable<Button> Buttons
        {
            get
            {
                return buttonLayout.Controls.OfType<Button>();
            }
        }

        /// <summary>
        /// Gets the clicked button that caused the form to close.
        /// </summary>
        /// <value>
        /// The clicked button.
        /// </value>
        public NotifyButton ClickedButton 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the name of the clicked custom button.
        /// </summary>
        /// <value>
        /// The name of the clicked custom button.
        /// </value>
        public string ClickedCustomName 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the title of the notify form.
        /// </summary>
        /// <value>
        /// The form title.
        /// </value>
        public string Title
        {
            get 
            { 
                return lblTitle.Text; 
            }
            set 
            { 
                lblTitle.Text = value;

                SizeForm();
            }
        }

        /// <summary>
        /// Gets or sets the message of the notify form.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get 
            { 
                return lblMessage.Text; 
            }
            set 
            { 
                lblMessage.Text = value;

                SizeForm();
            }
        }

        /// <summary>
        /// Gets or sets the request prompt located above the form buttons. Typically in the form of a question.
        /// </summary>
        /// <value>
        /// The request prompt.
        /// </value>
        public string Request
        {
            get 
            { 
                return lblRequest.Text; 
            }
            set 
            { 
                lblRequest.Text = value;
                lblRequest.Visible = !String.IsNullOrEmpty(lblRequest.Text);

                SizeForm();
            }
        }

        /// <summary>
        /// Gets or sets the custom button text.
        /// </summary>
        /// <value>
        /// The custom button text.
        /// </value>
        public string CustomButtonText
        {
            get 
            { 
                return btnCustom.Text; 
            }
            set 
            { 
                btnCustom.Text = value;

                SizeForm();
            }
        }

        /// <summary>
        /// Gets or sets the custom button action.
        /// </summary>
        /// <value>
        /// The custom button action.
        /// </value>
        public Action CustomButtonAction
        {
            get
            {
                return btnCustom.Tag as Action;
            }
            set
            {
                btnCustom.Tag = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor" /> property.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                lblTitle.ForeColor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyForm"/> class.
        /// </summary>
        public NotifyForm()
        {
            InitializeComponent();

            foreach (Control control in buttonLayout.Controls)
                control.Visible = false;

            this.BackColor = UIUtility.XyratexBlue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyForm"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="request">The request.</param>
        /// <param name="customButtonText">The custom button text if a custom button is needed.</param>
        /// <param name="customAction">The custom action if a custom button is needed.</param>
        public NotifyForm(string title, string message, NotifyButton buttons, string request = "", string customButtonText = "", Action customAction = null)
            : this()
        {
            this.Title = title;
            this.Message = message;
            this.Request = request;

            if ((buttons & NotifyButton.OK) == NotifyButton.OK)
                this.btnOK.Visible = true;
            if ((buttons & NotifyButton.Cancel) == NotifyButton.Cancel)
                this.btnCancel.Visible = true;
            if ((buttons & NotifyButton.Close) == NotifyButton.Close)
                this.btnClose.Visible = true;
            if ((buttons & NotifyButton.Shutdown) == NotifyButton.Shutdown)
                this.btnShutdown.Visible = true;
            if ((buttons & NotifyButton.Custom) == NotifyButton.Custom &&
                !String.IsNullOrEmpty(customButtonText) &&
                customAction != null)
            {
                this.btnCustom.Visible = true;
                this.CustomButtonText = customButtonText;
                this.CustomButtonAction = customAction;
            }

            SizeForm();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyForm"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="buttonNames">The custom button names.</param>
        public NotifyForm(string title, string message, string request, params string[] buttonNames)
            : this()
        {
            this.buttonLayout.Controls.Clear();

            this.Title = title;
            this.Message = message;
            this.Request = request;

            for (int i = buttonNames.Length - 1; i >= 0; i--)
            {
                Button button = new Button();
                button.AutoSize = true;
                button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                button.Name = "btnCustom" + i;
                button.MinimumSize = new System.Drawing.Size(90, 44);
                button.MaximumSize = new System.Drawing.Size(190, 44);
                button.TabIndex = 9;
                button.Text = buttonNames[i];
                button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler(this.btnCustom_Click);                

                this.buttonLayout.Controls.Add(button);
            }

            SizeForm();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyForm"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="defaultButtonIndex">The index of the buttonNames array which is set to be the default button.</param>
        /// <param name="buttonNames">The custom button names.</param>
        public NotifyForm(string title, string message, string request, int defaultButtonIndex, params string[] buttonNames)
            : this()
        {
            this.buttonLayout.Controls.Clear();

            this.Title = title;
            this.Message = message;
            this.Request = request;            

            for (int i = buttonNames.Length - 1; i >= 0; i--)
            {                
                Button button = new Button();
                button.TabStop = true;
                if(i == defaultButtonIndex)
                {
                    button.TabIndex = 0;                    
                }
                else
                {
                    button.TabIndex = 9;
                }
                button.AutoSize = true;
                button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                button.Name = "btnCustom" + i;
                button.MinimumSize = new System.Drawing.Size(90, 44);
                button.MaximumSize = new System.Drawing.Size(190, 44);
                
                button.Text = buttonNames[i];
                button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler(this.btnCustom_Click);

                this.buttonLayout.Controls.Add(button);
            }

            SizeForm();
        }

        private void btnShutdown_Click(object sender, EventArgs e)
        {
            ClickedButton = NotifyButton.Shutdown;
            Application.Exit();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ClickedButton = NotifyButton.Close;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClickedButton = NotifyButton.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ClickedButton = NotifyButton.OK;
            this.Close();
        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            ClickedButton = NotifyButton.Custom;
            ClickedCustomName = button.Text;

            Action customAction = button.Tag as Action;

            if (customAction != null)
                customAction.Invoke();

            this.Close();
        }

        private void SizeForm()
        {
            int newHeight = 0;
            int newWidth = 0;
            int buttonLayoutWidth = buttonLayout.Padding.Horizontal;

            foreach (Control control in buttonLayout.Controls)
                buttonLayoutWidth += control.Width + control.Margin.Horizontal;

            newWidth = Math.Max(lblTitle.PreferredWidth, lblRequest.PreferredWidth);
            newWidth = Math.Max(newWidth, lblMessage.PreferredWidth + lblMessage.Padding.Horizontal);
            newWidth = Math.Max(newWidth, buttonLayoutWidth + buttonLayout.Padding.Horizontal);
            newWidth = Math.Max(newWidth, this.MinimumSize.Width);

            lblTitle.Width = newWidth;
            lblRequest.Width = newWidth;
            lblMessage.Width = newWidth;

            int messageHeight = lblMessage.PreferredHeight + lblMessage.Padding.Vertical;

            newHeight = buttonLayout.Height + lblTitle.Height + messageHeight;

            if (lblRequest.Visible)
                newHeight += lblRequest.Height;

            if (this.ClientSize.Width != newWidth || this.ClientSize.Height != newHeight)
            {
                SetClientSizeCore(newWidth, newHeight);
                CenterToScreen();
            }
        }

        private void Notify_FormClosed(object sender, FormClosedEventArgs e)
        {
            string selection = "";

            if (ClickedButton == NotifyButton.Custom)
                selection = ClickedCustomName;
            else
                selection = ClickedButton.ToString();

            Log.Warn("Prompt", "Operator selected {0}.", selection);
        }

        private void Notify_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            Log.Info("Prompt", "{0}: {1}", Title.TrimEnd('.'), Message);

            if (!String.IsNullOrEmpty(Request))
                Log.Info("Prompt", "{0}", Request);
        }

        #region FormMove

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        /// <summary>
        /// Sends a Windows message
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="Msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Releases capture.
        /// </summary>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void moveForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion
    }

    #region Notify Buttons

    /// <summary>
    /// Standard buttons for notification windows.
    /// </summary>
    [Flags]
    public enum NotifyButton
    {
        /// <summary>
        /// OK
        /// </summary>
        OK = 0,
        /// <summary>
        /// Cancel
        /// </summary>
        Cancel = 1,
        /// <summary>
        /// Close
        /// </summary>
        Close = 8,
        /// <summary>
        /// Shutdown
        /// </summary>
        Shutdown = 16,
        /// <summary>
        /// Custom
        /// </summary>
        Custom = 32
    }

    #endregion
}
