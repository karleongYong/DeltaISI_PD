using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    /// <summary>
    /// Summary description for NumEntryForm.
    /// </summary>
    public class NumEntryForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btnBS;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btnDot;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.TextBox textBoxEntry;
        private double numberEntered = -99999.999;
        private double minVal;
        private double maxVal;
        private Button btnClearAll;
        private System.Windows.Forms.Button btnNeg;

        public NumEntryForm(double min, double max, double defaultValue)
        {
            InitializeComponent();
            this.Text = "Enter a number between " + min + " and " + max;
            textBoxEntry.Text = defaultValue.ToString();
            minVal = min;
            maxVal = max;
            if (IsInRange(defaultValue))
                btnEnter.Enabled = true;
        }

        public NumEntryForm(double min, double max, double defaultValue, bool OnlyInt)
        {
            InitializeComponent();
            this.Text = "Enter a number between " + min + " and " + max;
            textBoxEntry.Text = defaultValue.ToString();
            minVal = min;
            maxVal = max;
            if (IsInRange(defaultValue))
                btnEnter.Enabled = true;
            if (OnlyInt)
                btnDot.Enabled = false;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btnBS = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btnDot = new System.Windows.Forms.Button();
            this.btn0 = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btnNeg = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEntry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textBoxEntry.Location = new System.Drawing.Point(8, 8);
            this.textBoxEntry.MaxLength = 10;
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.Size = new System.Drawing.Size(424, 44);
            this.textBoxEntry.TabIndex = 13;
            this.textBoxEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxEntry_KeyDown);
            this.textBoxEntry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEntry_KeyPress);
            // 
            // btn4
            // 
            this.btn4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn4.Location = new System.Drawing.Point(8, 144);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(80, 56);
            this.btn4.TabIndex = 4;
            this.btn4.Text = "4";
            this.btn4.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn5
            // 
            this.btn5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn5.Location = new System.Drawing.Point(104, 144);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(80, 56);
            this.btn5.TabIndex = 5;
            this.btn5.Text = "5";
            this.btn5.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btnBS
            // 
            this.btnBS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBS.Location = new System.Drawing.Point(304, 72);
            this.btnBS.Name = "btnBS";
            this.btnBS.Size = new System.Drawing.Size(128, 56);
            this.btnBS.TabIndex = 12;
            this.btnBS.Text = "Backspace";
            this.btnBS.Click += new System.EventHandler(this.btnBS_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(304, 144);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 56);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.Enabled = false;
            this.btnEnter.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnEnter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnter.Location = new System.Drawing.Point(304, 216);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(128, 128);
            this.btnEnter.TabIndex = 0;
            this.btnEnter.Text = "Enter";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btn3
            // 
            this.btn3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn3.Location = new System.Drawing.Point(200, 216);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(80, 56);
            this.btn3.TabIndex = 9;
            this.btn3.Text = "3";
            this.btn3.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn8
            // 
            this.btn8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn8.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn8.Location = new System.Drawing.Point(104, 72);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(80, 56);
            this.btn8.TabIndex = 2;
            this.btn8.Text = "8";
            this.btn8.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn2
            // 
            this.btn2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn2.Location = new System.Drawing.Point(104, 216);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(80, 56);
            this.btn2.TabIndex = 8;
            this.btn2.Text = "2";
            this.btn2.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn6
            // 
            this.btn6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn6.Location = new System.Drawing.Point(200, 144);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(80, 56);
            this.btn6.TabIndex = 6;
            this.btn6.Text = "6";
            this.btn6.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn9
            // 
            this.btn9.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn9.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn9.Location = new System.Drawing.Point(200, 72);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(80, 56);
            this.btn9.TabIndex = 3;
            this.btn9.Text = "9";
            this.btn9.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn7
            // 
            this.btn7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn7.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn7.Location = new System.Drawing.Point(8, 72);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(80, 56);
            this.btn7.TabIndex = 1;
            this.btn7.Text = "7";
            this.btn7.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btnDot
            // 
            this.btnDot.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDot.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDot.Location = new System.Drawing.Point(104, 288);
            this.btnDot.Name = "btnDot";
            this.btnDot.Size = new System.Drawing.Size(80, 56);
            this.btnDot.TabIndex = 11;
            this.btnDot.Text = ".";
            this.btnDot.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn0
            // 
            this.btn0.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn0.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn0.Location = new System.Drawing.Point(8, 288);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(80, 56);
            this.btn0.TabIndex = 10;
            this.btn0.Text = "0";
            this.btn0.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btn1
            // 
            this.btn1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn1.Location = new System.Drawing.Point(8, 216);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(80, 56);
            this.btn1.TabIndex = 7;
            this.btn1.Text = "1";
            this.btn1.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btnNeg
            // 
            this.btnNeg.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNeg.Location = new System.Drawing.Point(200, 288);
            this.btnNeg.Name = "btnNeg";
            this.btnNeg.Size = new System.Drawing.Size(80, 56);
            this.btnNeg.TabIndex = 15;
            this.btnNeg.Text = "+/-";
            this.btnNeg.Click += new System.EventHandler(this.btnDigit_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.BackColor = System.Drawing.Color.White;
            this.btnClearAll.BackgroundImage = global::Seagate.AAS.UI.Properties.Resources.remove;
            this.btnClearAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClearAll.FlatAppearance.BorderSize = 0;
            this.btnClearAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightPink;
            this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAll.Location = new System.Drawing.Point(398, 15);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(30, 30);
            this.btnClearAll.TabIndex = 16;
            this.btnClearAll.UseVisualStyleBackColor = false;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // NumEntryForm
            // 
            this.AcceptButton = this.btnEnter;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(440, 354);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnNeg);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBS);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.textBoxEntry);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn8);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn6);
            this.Controls.Add(this.btn9);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btnDot);
            this.Controls.Add(this.btn0);
            this.Controls.Add(this.btn1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NumEntryForm";
            this.ShowInTaskbar = false;
            this.Text = "NumEntryForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NumEntryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        public double NumberEntered
        {
            get
            {
                return numberEntered;
            }
        }
        private void btnDigit_Click(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            textBoxEntry.BackColor = System.Drawing.Color.White;
            btnClearAll.BackColor = System.Drawing.Color.White;

            if (btn.Text == "+/-" && IsValidNumber(textBoxEntry.Text) && Convert.ToDouble(textBoxEntry.Text) != 0)
            {
                if (textBoxEntry.Text.Substring(0, 1) == "-")
                    textBoxEntry.Text = textBoxEntry.Text.Substring(1, textBoxEntry.Text.Length - 1);
                else
                    textBoxEntry.Text = textBoxEntry.Text.Insert(0, "-");
            }
            else if (btn.Text == "." && IsValidNumber(textBoxEntry.Text) && textBoxEntry.Text.IndexOf(".") < 0)
            {
                if (Convert.ToDouble(textBoxEntry.Text) == 0)
                    textBoxEntry.Text = "0.";
                else
                    textBoxEntry.Text += btn.Text;
            }
            else if (btn.Text == "0" && IsValidNumber(textBoxEntry.Text))
            {
                if (Convert.ToDouble(textBoxEntry.Text) == 0 && textBoxEntry.Text.IndexOf(".") < 0)
                    textBoxEntry.Text = "0";
                else
                    textBoxEntry.Text += btn.Text;
            }
            else
            {
                if (IsValidNumber(textBoxEntry.Text + btn.Text))
                {
                    if (textBoxEntry.Text == "0")
                        textBoxEntry.Text = btn.Text;
                    else
                        textBoxEntry.Text += btn.Text;
                }
            }
            if (IsValidNumber(textBoxEntry.Text) && IsInRange(Convert.ToDouble(textBoxEntry.Text)))
            {
                btnEnter.Enabled = true;
            }
            else
            {
                btnEnter.Enabled = false;
                textBoxEntry.BackColor = System.Drawing.Color.Pink;
                btnClearAll.BackColor = System.Drawing.Color.Pink;
                //				btnClearAll.BackColor =MessageBox.Show("Entry must be between " + minVal + " and " + maxVal);
            }
            textBoxEntry.Focus();
            textBoxEntry.Select(textBoxEntry.Text.Length, 0);
        }

        private void textBoxEntry_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = true;
            switch (e.KeyChar)
            {
                case '0':
                    btnDigit_Click((Button)btn0, e);
                    break;
                case '1':
                    btnDigit_Click((Button)btn1, e);
                    break;
                case '2':
                    btnDigit_Click((Button)btn2, e);
                    break;
                case '3':
                    btnDigit_Click((Button)btn3, e);
                    break;
                case '4':
                    btnDigit_Click((Button)btn4, e);
                    break;
                case '5':
                    btnDigit_Click((Button)btn5, e);
                    break;
                case '6':
                    btnDigit_Click((Button)btn6, e);
                    break;
                case '7':
                    btnDigit_Click((Button)btn7, e);
                    break;
                case '8':
                    btnDigit_Click((Button)btn8, e);
                    break;
                case '9':
                    btnDigit_Click((Button)btn9, e);
                    break;
                case '-':
                    btnDigit_Click((Button)btnNeg, e);
                    break;
                case '.':
                    btnDigit_Click((Button)btnDot, e);
                    break;
            }

        }

        private void btnBS_Click(object sender, System.EventArgs e)
        {
            if (textBoxEntry.Text.Length > 0)
                textBoxEntry.Text = textBoxEntry.Text.Substring(0, textBoxEntry.Text.Length - 1);
            if (textBoxEntry.Text == "-" || textBoxEntry.Text.Length == 0)
                textBoxEntry.Text = "0";
            if (IsValidNumber(textBoxEntry.Text) && IsInRange(Convert.ToDouble(textBoxEntry.Text)))
            {
                textBoxEntry.BackColor = System.Drawing.Color.White;
                btnClearAll.BackColor = System.Drawing.Color.White;
                btnEnter.Enabled = true;
            }
            else
            {
                textBoxEntry.BackColor = System.Drawing.Color.Pink;
                btnClearAll.BackColor = System.Drawing.Color.Pink;
                btnEnter.Enabled = false;
            }
            textBoxEntry.Focus();
            textBoxEntry.Select(textBoxEntry.Text.Length, 0);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnEnter_Click(object sender, System.EventArgs e)
        {
            if (IsValidNumber(textBoxEntry.Text))
            {
                if (IsInRange(Convert.ToDouble(textBoxEntry.Text)))
                {
                    numberEntered = Convert.ToDouble(textBoxEntry.Text);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void textBoxEntry_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.KeyValue)
            {
                case (int)Keys.Back:
                case (int)Keys.Delete:
                    btnBS_Click((Button)btnBS, e);
                    break;
                case (int)Keys.Enter:
                    btnEnter_Click((Button)btnEnter, e);
                    break;
                case (int)Keys.Escape:
                    btnCancel_Click((Button)btnCancel, e);
                    break;
            }
        }

        private bool IsValidNumber(string s)
        {
            bool result;

            if (!this.btnDot.Enabled)
            {
                int number;
                result = int.TryParse(s, out number);
            }
            else
            {
                double number;
                result = double.TryParse(s, out number);
            }

            return result;

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
            //        hasDecimal = true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        private bool IsInRange(double val)
        {
            if (val >= minVal && val <= maxVal)
                return true;
            return false;
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            textBoxEntry.Text = "0";
            if (IsValidNumber(textBoxEntry.Text) && IsInRange(Convert.ToDouble(textBoxEntry.Text)))
            {
                textBoxEntry.BackColor = System.Drawing.Color.White;
                btnClearAll.BackColor = System.Drawing.Color.White;
                btnEnter.Enabled = true;
            }
            else
            {
                textBoxEntry.BackColor = System.Drawing.Color.Pink;
                btnClearAll.BackColor = System.Drawing.Color.Pink;
                btnEnter.Enabled = false;
            }
            textBoxEntry.Focus();
            textBoxEntry.Select(textBoxEntry.Text.Length, 0);
        }

        private void NumEntryForm_Load(object sender, EventArgs e)
        {
            textBoxEntry.Select();
            textBoxEntry.Select(textBoxEntry.Text.Length, 0);
        }
    }
}
