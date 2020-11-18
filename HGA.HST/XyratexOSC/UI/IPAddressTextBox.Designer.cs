namespace XyratexOSC.UI
{
    partial class IPAddressTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 1);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(43, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.Tag = this.textBox2;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.Text_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Text_KeyPress);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(51, 1);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(43, 22);
            this.textBox2.TabIndex = 1;
            this.textBox2.Tag = this.textBox3;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.Text_TextChanged);
            this.textBox2.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Text_KeyPress);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(99, 1);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(43, 22);
            this.textBox3.TabIndex = 2;
            this.textBox3.Tag = this.textBox4;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox3.TextChanged += new System.EventHandler(this.Text_TextChanged);
            this.textBox3.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Text_KeyPress);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(148, 1);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(43, 22);
            this.textBox4.TabIndex = 3;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox4.TextChanged += new System.EventHandler(this.Text_TextChanged);
            this.textBox4.Enter += new System.EventHandler(this.TextBox_Enter);
            this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Text_KeyPress);
            // 
            // IPAddressTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "IPAddressTextBox";
            this.Size = new System.Drawing.Size(193, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
    }
}
