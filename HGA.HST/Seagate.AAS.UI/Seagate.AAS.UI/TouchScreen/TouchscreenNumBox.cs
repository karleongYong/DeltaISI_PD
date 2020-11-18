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
    /// Summary description for TouchscreenNumBox.
    /// </summary>
    public class TouchscreenNumBox : System.Windows.Forms.TextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        String previousString = "";
        private double min;
        public double Min
        {
            get { return min; }
            set { min = (double)value; }
        }
        private double max;
        public double Max
        {
            get { return max; }
            set { max = (double)value; }
        }
        private bool intOnly;
        public bool IntegerOnly
        {
            get { return intOnly; }
            set { intOnly = (bool)value; }
        }

        public TouchscreenNumBox()
        {
            InitializeComponent();
            min = -5;
            max = 100;
            intOnly = false;
            base.Text = "0";
        }
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

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnClick(EventArgs e)
        {
            //if(this.ReadOnly)
            //    return;

            double defaultValue;
            bool validNum = true;
            bool hasDecimal = false;
            string numString = base.Text.Trim();

            if (!IsValidNumber(numString))
            {
                validNum = false;
            }

            //for(int i=0; i<numString.Length; i++)
            //{
            //    if( (numString[i] <= '9' && numString[i] >= '0') || 
            //        (numString[i] == '-' && i == 0) )
            //    {
            //        ;
            //    }
            //    else if( numString[i] == '.' && !hasDecimal && !intOnly)
            //    {
            //        hasDecimal = true;
            //    }
            //    else
            //    {
            //        validNum = false;
            //        break;
            //    }
            //}
            if (validNum && numString.Length > 0)
                defaultValue = Convert.ToDouble(numString);
            else
                defaultValue = 0;

            //			try
            //			{
            //				defaultValue = Convert.ToDouble(base.Text);
            //			}
            //			catch (System.ArgumentException a_ex)
            //			{ 
            //				defaultValue = 0; 
            //			}
            //			catch (System.FormatException f_ex)
            //			{ 
            //				defaultValue = 0; 
            //			}
            NumEntryForm numForm = new NumEntryForm(min, max, defaultValue, intOnly);
            numForm.ShowDialog();
            if (numForm.DialogResult == DialogResult.OK)
                this.Text = numForm.NumberEntered.ToString();
            numForm.Dispose();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TouchscreenNumBox
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ReadOnly = true;
            this.TextChanged += new System.EventHandler(this.TouchscreenNumBox_TextChanged);
            this.Enter += new System.EventHandler(this.TouchscreenNumBox_Enter);
            this.Leave += new System.EventHandler(this.TouchscreenNumBox_Leave);
            this.ResumeLayout(false);

        }
        #endregion

        private void TouchscreenNumBox_TextChanged(object sender, System.EventArgs e)
        {
            CheckNumber();
        }

        private bool IsValidNumber(string s)
        {
            bool result;

            if (this.intOnly)
            {
                int number;
                result = int.TryParse(s, out number);
            }
            else
            {
                double number;
                result = double.TryParse(s, out number);
            }

            //bool hasDecimal = false;
            //s = s.Trim();

            //for(int i=0; i<s.Length; i++)
            //{
            //    if( (s[i] >= '0' && s[i] <= '9' ) ||
            //        (s[i] == '-' && i == 0) )
            //    {
            //        ;
            //    }
            //    else if( s[i] == '.' && !hasDecimal )
            //    {
            //        if(intOnly)
            //            return false;

            //        hasDecimal = true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            return result;
        }

        private bool IsInRange(double val)
        {
            if (val >= min && val <= max)
                return true;

            return false;
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

            if (success)
            {
                this.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.BackColor = System.Drawing.Color.Pink;
            }

            return success;
        }

        private void TouchscreenNumBox_Enter(object sender, System.EventArgs e)
        {
            previousString = this.Text;
        }

        private void TouchscreenNumBox_Leave(object sender, System.EventArgs e)
        {
            if (!CheckNumber())
                this.Text = previousString;
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
    }
}
