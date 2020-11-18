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
    /// Hovering, borderless form for context pop-ups that can hold a custom control. Provides more flexibility than a context menu.
    /// </summary>
    public partial class PopUpContextForm : Form, IMessageFilter
    {
        private Control _control;

        /// <summary>
        /// Gets or sets the custom user control.
        /// </summary>
        /// <value>
        /// The user control.
        /// </value>
        public Control UserControl
        {
            get
            {
                return _control;
            }

            set
            {
                contentPanel.Controls.Clear();

                _control = value;

                if (_control == null)
                    return;

                _control.Location = new Point(0, 0);
                _control.Dock = DockStyle.Fill;

                contentPanel.Controls.Add(_control);

                int width = Math.Max(_control.PreferredSize.Width, _control.Width);
                int height = Math.Max(_control.PreferredSize.Height, _control.Height);

                this.ClientSize = new Size(width, height);
            }
        }

        /// <summary>
        /// [For designer purposes only] Initializes a new instance of the <see cref="PopUpContextForm"/> class.
        /// </summary>
        public PopUpContextForm()
            : this(null)
        {
        }

        private PopUpContextForm(Control control)
        {
            InitializeComponent();
            UserControl = control;

            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (_control != null)
                _control.Visible = this.Visible;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.FormClosed" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.FormClosedEventArgs" /> that contains the event data.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Application.RemoveMessageFilter(this);

            if (_control != null)
                _control.Visible = false;

            contentPanel.Controls.Clear();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Shows the specified popup control.
        /// </summary>
        /// <param name="popupControl">The popup control.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="size">The size.</param>
        /// <param name="pos">The pos.</param>
        public static void Show(Control popupControl, Control parent, Size size, Point pos)
        {
            PopUpContextForm form = new PopUpContextForm(popupControl);

            form.StartPosition = FormStartPosition.Manual;
            form.Location = parent.PointToScreen(pos);
            form.Size = size;

            while (!(parent is Form))
            {
                if (parent.Parent == null)
                    break;

                parent = parent.Parent;
            }

            form.Show(parent);
        }

        /// <summary>
        /// Detects mouse clicks outside of the form in order to close the form when focus is lost.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.
        /// </returns>
        public bool PreFilterMessage(ref Message m)
        {
            // Detect mouse clicks outside of the form 
            if (m.Msg == 0x201 || m.Msg == 0x204 || m.Msg == 0x207 ||
                m.Msg == 0xA1 || m.Msg == 0xA4 || m.Msg == 0xA7)
            {
                Point pos = new Point(m.WParam.ToInt32() & 0xffff, m.WParam.ToInt32() >> 16);
                Control ctl = Control.FromHandle(m.HWnd);

                if (ctl != null)
                {
                    pos = ctl.PointToScreen(pos);
                    pos = this.PointToClient(pos);
                }

                if (pos.X < 0 || pos.Y < 0 || pos.X >= this.Width || pos.Y >= this.Height)
                {
                    this.Close();
                }
            }

            return false;
        }
    }
}
