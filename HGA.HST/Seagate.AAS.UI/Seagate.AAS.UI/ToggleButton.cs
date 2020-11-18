using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Seagate.AAS.UI
{
    public enum ToggleState
    {
        OFF,
        ON
    }

    [DefaultEvent("StateChanged")]
    [DefaultProperty("State")]
    public class ToggleButton : Control
    {
        public event EventHandler StateChanged;

        protected Image _toggleOff = Seagate.AAS.UI.Properties.Resources.ToggleButtonOff;
        protected Image _toggleOn = Seagate.AAS.UI.Properties.Resources.ToggleButtonOn;
        protected ToggleState _state = ToggleState.OFF;

        [DefaultValue(ToggleState.OFF)]
        public ToggleState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    this.OnStateChanged(EventArgs.Empty);
                    base.Invalidate();
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (_state == ToggleState.OFF)
                State = ToggleState.ON;
            else
                State = ToggleState.OFF;

            base.OnClick(e);
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._state == ToggleState.OFF)
                e.Graphics.DrawImage(_toggleOff, 0, 0, this.Size.Width, this.Size.Height);
            else
                e.Graphics.DrawImage(_toggleOn, 0, 0, this.Size.Width, this.Size.Height);

            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Height = (int)(this.Width / 2.851);

            base.OnSizeChanged(e);
        }

        // Invoke the Changed event; called whenever state changes:
        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }
    }
}
