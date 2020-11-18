using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A progress bar that allows for two overlayed progress bars.
    /// </summary>
    public class ProgressBarEnhanced : ProgressBar
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the blend shadow is drawn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [draw blend shadow]; otherwise, <c>false</c>.
        /// </value>
        public bool DrawBlendShadow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the back color2.
        /// </summary>
        /// <value>
        /// The back color2.
        /// </value>
        public Color BackColor2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value2.
        /// </summary>
        /// <value>
        /// The value2.
        /// </value>
        public int Value2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum2.
        /// </summary>
        /// <value>
        /// The minimum2.
        /// </value>
        public int Minimum2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum2.
        /// </summary>
        /// <value>
        /// The maximum2.
        /// </value>
        public int Maximum2
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarEnhanced"/> class.
        /// </summary>
        public ProgressBarEnhanced()
            : base()
        {
            DrawBlendShadow = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = Rectangle.Empty;
            Rectangle rec2 = Rectangle.Empty;

            if (Maximum != 0)
            {
                rec = e.ClipRectangle;
                rec.Width = Convert.ToInt32(Math.Truncate(rec.Width * (Convert.ToDouble(Value) / Maximum))) - 4;
                rec.Height = rec.Height - 4;
            }

            if (Maximum2 != 0)
            {
                rec2 = e.ClipRectangle;
                rec2.Width = Convert.ToInt32(Math.Truncate(rec2.Width * (Convert.ToDouble(Value2) / Maximum2))) - 4;
                rec2.Height = rec2.Height - 4;
            }
            
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);

            Color backColor = Color.GreenYellow;
            Color foreColor = Color.GreenYellow;

            Rectangle longRec = rec;
            Rectangle shortRec = rec2;

            if (rec2.Width > rec.Width)
            {
                longRec = rec2;
                shortRec = rec;

                if (BackColor != null)
                    foreColor = BackColor;
                if (BackColor2 != null)
                    backColor = BackColor2;
            }
            else
            {
                if (BackColor != null)
                    backColor = BackColor;
                if (BackColor2 != null)
                    foreColor = BackColor2;
            }

            using (Brush backColorBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backColorBrush, 2, 2, longRec.Width, longRec.Height);
            }

            using (Brush foreColorBrush = new SolidBrush(Color.FromArgb(127, foreColor)))
            {
                e.Graphics.FillRectangle(foreColorBrush, 2, 2, shortRec.Width, shortRec.Height);
            }
        
            if (!String.IsNullOrEmpty(Label))
            {
                Rectangle fullRec = this.ClientRectangle;
                SizeF strLen = e.Graphics.MeasureString(Label, this.Font);
                Point location = new Point((int)((fullRec.Width / 2) - (strLen.Width / 2)) + 3, (int)((fullRec.Height / 2) - (strLen.Height / 2)) + 1);

                using (Brush textBrush = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(Label, this.Font, textBrush, location);
                }
            }

            if (DrawBlendShadow && longRec.Width > 0 && longRec.Height > 0)
            {
                using (LinearGradientBrush gradBrush = new LinearGradientBrush(longRec, Color.FromArgb(147, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical))
                {
                    ColorBlend cb = new ColorBlend();
                    cb.Colors = new Color[] { Color.FromArgb(40, 255, 255, 255), Color.FromArgb(147, 255, 255, 255), Color.FromArgb(40, 255, 255, 255), Color.FromArgb(0, 255, 255, 255)};
                    cb.Positions = new float[] {0, 0.12F, 0.39F, 1.0F};

                    gradBrush.InterpolationColors = cb;
                    gradBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;

                    e.Graphics.FillRectangle(gradBrush, 2, 2, longRec.Width, longRec.Height);
                }
            }
        }
    }

    /// <summary>
    /// A toolstrip control for displaying a <see cref="ProgressBarEnhanced"/> control.
    /// </summary>
    public class ToolStripProgressBarEnhanced : ToolStripControlHost
    {
        /// <summary>
        /// Gets the progress bar.
        /// </summary>
        /// <value>
        /// The progress bar.
        /// </value>
        public ProgressBarEnhanced ProgressBar
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                return ProgressBar.Label;
            }
            set
            {
                ProgressBar.Label = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get
            {
                return ProgressBar.Value;
            }
            set
            {
                ProgressBar.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public int Minimum
        {
            get
            {
                return ProgressBar.Minimum;
            }
            set
            {
                ProgressBar.Minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public int Maximum
        {
            get
            {
                return ProgressBar.Maximum;
            }
            set
            {
                ProgressBar.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the back color2.
        /// </summary>
        /// <value>
        /// The back color2.
        /// </value>
        public Color BackColor2
        {
            get
            {
                return ProgressBar.BackColor2;
            }
            set
            {
                ProgressBar.BackColor2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the value2.
        /// </summary>
        /// <value>
        /// The value2.
        /// </value>
        public int Value2
        {
            get
            {
                return ProgressBar.Value2;
            }
            set
            {
                ProgressBar.Value2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum2.
        /// </summary>
        /// <value>
        /// The minimum2.
        /// </value>
        public int Minimum2
        {
            get
            {
                return ProgressBar.Minimum2;
            }
            set
            {
                ProgressBar.Minimum2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum2.
        /// </summary>
        /// <value>
        /// The maximum2.
        /// </value>
        public int Maximum2
        {
            get
            {
                return ProgressBar.Maximum2;
            }
            set
            {
                ProgressBar.Maximum2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the hosted control.
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> representing the foreground color of the hosted control.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public override Color ForeColor
        {
            get
            {
                return ProgressBar.ForeColor;
            }
            set
            {
                ProgressBar.ForeColor = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>A <see cref="T:System.Drawing.Color" /> that represents the background color of the item. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor" /> property.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public override Color BackColor
        {
            get
            {
                return ProgressBar.BackColor;
            }
            set
            {
                ProgressBar.BackColor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripProgressBarEnhanced"/> class.
        /// </summary>
        public ToolStripProgressBarEnhanced()
            : base(new ProgressBarEnhanced())
        {
            ProgressBar = this.Control as ProgressBarEnhanced;
        }
    }
}
