using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{

    public enum PhotoBoxSizeMode
    {
        Normal = PictureBoxSizeMode.Normal,
        StretchImage = PictureBoxSizeMode.StretchImage,
        AutoSize = PictureBoxSizeMode.AutoSize,
        CenterImage = PictureBoxSizeMode.CenterImage,
        ScaleImage
    }

    /// <summary>
    ///Photo Box taken from http://www2.sys-con.com/ITSG/virtualcd/Dotnet/archives/0101/brown/index.html#s2
    /// </summary>
    public class PhotoBox : System.Windows.Forms.PictureBox
    {
        private PhotoBoxSizeMode _sizeMode = PhotoBoxSizeMode.ScaleImage;

        [Category("Behavior")]
        [Description("Controls how the image is drawn within the control.")]
        [DefaultValue(PhotoBoxSizeMode.ScaleImage)]
        public new PhotoBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                this.Invalidate();
            }
        }

        private Rectangle ScaleToFit(Rectangle targetArea, System.Drawing.Image img)
        {
            Rectangle r = new Rectangle(
                targetArea.Location, targetArea.Size);

            // Determine best fit: width or height
            if (r.Height * img.Width > r.Width * img.Height)
            {
                // Use width, determine height
                r.Height = r.Width * img.Height / img.Width;
                r.Y += (targetArea.Height - r.Height) / 2;
            }
            else
            {
                // Use height, determine width
                r.Width = r.Height * img.Width / img.Height;
                r.X += (targetArea.Width - r.Width) / 2;
            }
            return r;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (SizeMode == PhotoBoxSizeMode.ScaleImage)
                base.SizeMode = PictureBoxSizeMode.Normal;
            else
                base.SizeMode = (PictureBoxSizeMode)_sizeMode;

            // Call base class, invoke Paint handlers
            base.OnPaint(e);

            if (SizeMode == PhotoBoxSizeMode.ScaleImage
                && Image != null)
            {
                // Clear background
                e.Graphics.Clear(SystemColors.Control);

                // Implement ScaleImage drawing
                Rectangle paintRec
                    = ScaleToFit(ClientRectangle, Image);
                e.Graphics.DrawImage(this.Image, paintRec);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (SizeMode == PhotoBoxSizeMode.ScaleImage)
                this.Invalidate();  // redraw image

            base.OnResize(e);
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
