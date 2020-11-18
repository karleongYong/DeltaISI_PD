using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using XyratexOSC.IO;
using XyratexOSC.Logging;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A notification banner that will be displayed at the top of the notification panel.
    /// </summary>
    [Designer("ParentControlDesigner, Design", typeof(IDesigner))]
    public partial class NotifyBanner : UserControl
    {
        private IList<Tuple<string, Action>> _actionItems = new List<Tuple<string, Action>>();
        private LightStackColor _prevColor;

        /// <summary>
        /// Gets or sets the color of the light stack associated with this notification.
        /// </summary>
        /// <value>
        /// The color of the light stack.
        /// </value>
        public LightStackColor LightStackColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message.
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
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public Image Icon
        {
            get
            {
                return pbIcon.BackgroundImage;
            }
            set
            {
                pbIcon.BackgroundImage = value;
                pbIcon.Visible = (pbIcon.BackgroundImage != null);
            }
        }

        /// <summary>
        /// Gets or sets the close text.
        /// </summary>
        /// <value>
        /// The close text.
        /// </value>
        public string CloseText
        {
            get
            {
                return btnClose.Text;
            }
            set
            {
                btnClose.Text = value;

                if (!String.IsNullOrEmpty(btnClose.Text))
                    btnClose.BackgroundImage = null;
                else
                    btnClose.BackgroundImage = Properties.Resources.RemoveIcon;
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <returns>The foreground <see cref="T:System.Drawing.Color" /> of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultForeColor" /> property.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public override Color ForeColor
        {
            get
            {
                return (this.BackColor.GetBrightness() < 0.5) ? Color.White : Color.Green;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyBanner"/> class.
        /// </summary>
        public NotifyBanner()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
        }

        /// <summary>
        /// Adds an action.
        /// </summary>
        /// <param name="actionText">The action text.</param>
        /// <param name="action">The action.</param>
        public void AddAction(string actionText, Action action)
        {
            Tuple<string, Action> actionAndName = new Tuple<string, Action>(actionText, action);
            _actionItems.Add(actionAndName);
            UpdateActionButtons();
        }

        /// <summary>
        /// Removes an action with the specified text.
        /// </summary>
        /// <param name="actionText">The action text.</param>
        public void RemoveAction(string actionText)
        {
            Tuple<string, Action> actionAndName = null;

            foreach (Tuple<string, Action> actionItem in _actionItems)
                if (String.Equals(actionText, actionItem.Item1, StringComparison.CurrentCultureIgnoreCase))
                    actionAndName = actionItem;

            if (actionAndName != null)
                _actionItems.Remove(actionAndName);

            UpdateActionButtons();
        }

        /// <summary>
        /// Clears all actions.
        /// </summary>
        public void ClearActions()
        {
            _actionItems.Clear();
            UpdateActionButtons();
        }

        private void UpdateActionButtons()
        {
            UIUtility.Invoke(this, () =>
            {
                this.SuspendLayout();

                try
                {
                    flowLayout.Controls.Clear();

                    foreach (Tuple<string, Action> actionItem in _actionItems)
                    {
                        Button btnAction = new Button();
                        btnAction.AutoSize = true;
                        btnAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                        btnAction.ForeColor = System.Drawing.SystemColors.ControlText;
                        btnAction.Location = new System.Drawing.Point(2, 3);
                        btnAction.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
                        btnAction.MinimumSize = new System.Drawing.Size(29, 28);
                        btnAction.Name = "btn" + actionItem.Item1;
                        btnAction.Padding = new System.Windows.Forms.Padding(5, 0, 5, 1);
                        btnAction.Size = new System.Drawing.Size(64, 28);
                        btnAction.TabIndex = 1;
                        btnAction.Tag = actionItem.Item2;
                        btnAction.Text = actionItem.Item1;
                        btnAction.UseVisualStyleBackColor = true;
                        btnAction.Click += new System.EventHandler(this.btnAction_Click);

                        flowLayout.Controls.Add(btnAction);
                    }
                }
                finally
                {
                    this.ResumeLayout();
                }
            });
        }

        /// <summary>
        /// Performs the action associated with the specified text.
        /// </summary>
        /// <param name="actionText">The action text.</param>
        public void DoAction(string actionText)
        {
            foreach (Control control in flowLayout.Controls)
            {
                Button button = control as Button;
                if (button == null)
                    continue;

                if (String.Equals(actionText, button.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    btnAction_Click(button, EventArgs.Empty);
                    return;
                }
            }
        }

        private void NotifyBanner_Load(object sender, EventArgs e)
        {
            try
            {
                if (Notify.LightTower != null && LightStackColor != LightStackColor.Off)
                {
                    _prevColor = Notify.LightTower.GetBlink();
                    Notify.LightTower.Off(LightStackColor.All);
                    Notify.LightTower.Blink(LightStackColor);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Closes this banner.
        /// </summary>
        public void Close()
        {
            CancelEventHandler closing = Closing;
            CancelEventArgs cancelArgs = new CancelEventArgs();

            if (closing != null)
                closing(this, cancelArgs);

            if (cancelArgs.Cancel)
                return;

            try
            {
                if (Notify.LightTower != null && LightStackColor != LightStackColor.Off)
                {
                    Notify.LightTower.Off(LightStackColor.All);
                    Notify.LightTower.Blink(_prevColor);
                }
            }
            catch (Exception)
            {
            }

            EventHandler closed = Closed;

            if (closed != null)
                closed(this, new EventArgs());

            this.Dispose();
        }

        /// <summary>
        /// Occurs when the banner is closing.
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        /// Occurs when the banner has closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when a banner action is starting.
        /// </summary>
        public event CancelEventHandler ActionStarting;

        /// <summary>
        /// Occurs when a banner action has completed.
        /// </summary>
        public event EventHandler ActionCompleted;

        private void btnAction_Click(object sender, EventArgs e)
        {
            Button btnAction = sender as Button;
            if (btnAction == null)
                return;

            Action action = btnAction.Tag as Action;
            if (action == null)
                return;

            btnAction.Enabled = false;
            CancelEventArgs cancelArgs = new CancelEventArgs();
            CancelEventHandler actionStarting = ActionStarting;
            if (actionStarting != null)
                actionStarting(this, cancelArgs);

            if (cancelArgs.Cancel)
                return;

            Task.Factory.StartNew(action).ContinueWith(t =>
            {
                UIUtility.Invoke(this, () =>
                {
                    btnAction.Enabled = true;

                    if (t.IsFaulted)
                    {
                        if (this != null)
                            lblMessage.Text = t.Exception.InnerException.Message;

                        Log.Error("Error", t.Exception.InnerException);
                        return;
                    }
                    else
                    {
                        EventHandler actionCompleted = ActionCompleted;
                        if (actionCompleted != null)
                            actionCompleted(this, new EventArgs());
                    }
                });
            });
        }

        private void NotifyBanner_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Black, 0, this.Height - 1, this.Width, this.Height - 1);

            Rectangle all = new Rectangle(0, 0, this.Width, this.Height);

            using (LinearGradientBrush gradient =
                new LinearGradientBrush(all, Color.FromArgb(0, Color.DimGray), Color.FromArgb(105, Color.DimGray), 90F))
            {
                e.Graphics.FillRectangle(gradient, all);
            }
        }
    }
}