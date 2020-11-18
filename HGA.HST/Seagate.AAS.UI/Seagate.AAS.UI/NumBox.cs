//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2006/12/01] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    /// <summary>
    /// Summary description for NumBox.
    /// </summary>
    public partial class NumBox : System.Windows.Forms.TextBox
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private String previousString = "";
        private double min;
        private double max;
        private bool intOnly;

        // Constructors & Finalizers -------------------------------------------

        public NumBox()
        {
            InitializeComponent();
            min = -5;
            max = 100;
            intOnly = false;
            base.Text = "0";
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NumBox
            // 
            this.Enter += new System.EventHandler(this.NumBox_Enter);
            this.Leave += new System.EventHandler(this.NumBox_Leave);
            this.TextChanged += new System.EventHandler(this.NumBox_TextChanged);
            this.ResumeLayout(false);

        }

        // Properties ----------------------------------------------------------

        [Browsable(true)]
        [Category("Range")]
        [Description("Range minimum .")]
        public double Min
        {
            get { return min; }
            set { min = (double)value; }
        }

        [Browsable(true)]
        [Category("Range")]
        [Description("Range maximum.")]
        public double Max
        {
            get { return max; }
            set { max = (double)value; }
        }

        [Browsable(true)]
        [Category("Type")]
        [Description("This event occurs when the form options must be saved (click on Apply or OK form buttons)")]
        public bool IntegerOnly
        {
            get { return intOnly; }
            set { intOnly = (bool)value; }
        }

        // Methods -------------------------------------------------------------

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                CheckNumber();
            }
        }

        public int RangeLimit(ref double val)
        {
            if (val < min)
            {
                val = min;
                return -1;
            }

            if (val > max)
            {
                val = max;
                return 1;
            }

            return 0;
        }

        public int SetInRange(int val)
        {
            return SetInRange(ref val);
        }

        public int SetInRange(ref int val)
        {
            double myVal = val;
            int ret = RangeLimit(ref myVal);
            val = (int)myVal;
            this.Text = val.ToString();
            return ret;
        }

        public int SetInRange(double val, int precision)
        {
            return SetInRange(ref val, precision);
        }

        public int SetInRange(ref double val, int precision)
        {
            int ret = RangeLimit(ref val);
            this.Text = val.ToString("F" + precision.ToString());
            return ret;
        }

        // Event handlers ------------------------------------------------------

        private void NumBox_TextChanged(object sender, System.EventArgs e)
        {
            CheckNumber();
        }

        private void NumBox_Enter(object sender, System.EventArgs e)
        {
            previousString = this.Text;
        }

        private void NumBox_Leave(object sender, System.EventArgs e)
        {
            if (!CheckNumber())
                this.Text = previousString;
        }

        // Internal methods ----------------------------------------------------

        private bool IsValidNumber(string s)
        {
            bool hasDecimal = false;
            s = s.Trim();

            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i] >= '0' && s[i] <= '9') ||
                    (s[i] == '-' && i == 0))
                {
                    ;
                }
                else if (s[i] == '.' && !hasDecimal)
                {
                    if (intOnly)
                        return false;

                    hasDecimal = true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsInRange(double val)
        {
            if (val >= min && val <= max)
                return true;

            return false;
        }

        private bool CheckNumber()
        {
            bool success = false;

            if (IsValidNumber(this.Text))
            {

                try
                {
                    double num = Convert.ToDouble(this.Text);
                    if (IsInRange(num))
                        success = true;
                }
                catch { success = false; } // assume fail for all exceptions in convert
            }

            if (this.ReadOnly)
            {
                this.BackColor = System.Drawing.Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                if (success)
                {
                    this.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    this.BackColor = System.Drawing.Color.Pink;
                }
            }

            return success;
        }
    }
}
