using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for FormattedNumber.
	/// </summary>
	public class FormattedNumber : System.Windows.Forms.Label
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private string numberFormat = "0.000";
        private double numberValue;
        private Size ledSize = new Size(15, 15);

		public FormattedNumber()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// 
            this.TextChanged += new EventHandler(FormattedNumber_TextChanged);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.TextAlign = ContentAlignment.MiddleRight;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
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
			components = new System.ComponentModel.Container();
		}
		#endregion

        #region FormattedNumber Members
        
        protected override void OnPaint(PaintEventArgs pe)
        {
//            // 
//            Graphics g = pe.Graphics;
//            using ( Pen boundaryBrush = new Pen(Color.Black, 2) )
//            using ( Brush foreBrush  = new SolidBrush(this.ForeColor) )
//            using ( Brush backBrush  = new SolidBrush(this.BackColor) ) 
//            {
//                // display value
//                StringFormat fmt = new StringFormat();
//                fmt.Alignment = StringAlignment.Near;
//                fmt.LineAlignment = StringAlignment.Center;
//                g.DrawString(numberValue.ToString(format), this.Font, foreBrush, this.ClientRectangle, fmt);
//            }

            this.Text = numberValue.ToString(numberFormat);

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("Number Format, e.g. 0.000"),
        ]
        public string NumberFormat
        {
            get
            {
                return numberFormat;
            }
            set
            {
                if (this.numberFormat != value)
                {
                    this.numberFormat = value; 
                    this.Invalidate();
                }
            }
        }

        [
        CategoryAttribute("Appearance"),
        DescriptionAttribute("Number to display.")
        ]
        public double Number
        {
            get
            {
                return numberValue;
            }
            set
            {
                if (this.numberValue != value)
                {
                    this.numberValue = value; 
                    this.Invalidate();
                }
            }
        }

        //[BrowsableAttribute(false)]
        public override string Text
        {
            get { return base.Text;    }
        }

        private void FormattedNumber_TextChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        #endregion


    }
}
