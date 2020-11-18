namespace Test_RFID
{
    partial class MainTestForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTestHgaBola = new System.Windows.Forms.Button();
            this.btnTestHgaFola = new System.Windows.Forms.Button();
            this.btnHgaBolaPanel = new System.Windows.Forms.Button();
            this.btnHgaFolaPanel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestHgaBola
            // 
            this.btnTestHgaBola.Location = new System.Drawing.Point(180, 12);
            this.btnTestHgaBola.Name = "btnTestHgaBola";
            this.btnTestHgaBola.Size = new System.Drawing.Size(149, 44);
            this.btnTestHgaBola.TabIndex = 0;
            this.btnTestHgaBola.Text = "HGA BOLA Test";
            this.btnTestHgaBola.UseVisualStyleBackColor = true;
            this.btnTestHgaBola.Click += new System.EventHandler(this.btnTestHgaBola_Click);
            // 
            // btnTestHgaFola
            // 
            this.btnTestHgaFola.Location = new System.Drawing.Point(12, 12);
            this.btnTestHgaFola.Name = "btnTestHgaFola";
            this.btnTestHgaFola.Size = new System.Drawing.Size(149, 44);
            this.btnTestHgaFola.TabIndex = 0;
            this.btnTestHgaFola.Text = "HGA FOLA Test";
            this.btnTestHgaFola.UseVisualStyleBackColor = true;
            this.btnTestHgaFola.Click += new System.EventHandler(this.btnTestHgaFola_Click);
            // 
            // btnHgaBolaPanel
            // 
            this.btnHgaBolaPanel.Location = new System.Drawing.Point(181, 69);
            this.btnHgaBolaPanel.Name = "btnHgaBolaPanel";
            this.btnHgaBolaPanel.Size = new System.Drawing.Size(149, 44);
            this.btnHgaBolaPanel.TabIndex = 2;
            this.btnHgaBolaPanel.Text = "HGA BOLA Panel";
            this.btnHgaBolaPanel.UseVisualStyleBackColor = true;
            this.btnHgaBolaPanel.Click += new System.EventHandler(this.btnHgaBolaPanel_Click);
            // 
            // btnHgaFolaPanel
            // 
            this.btnHgaFolaPanel.Location = new System.Drawing.Point(13, 69);
            this.btnHgaFolaPanel.Name = "btnHgaFolaPanel";
            this.btnHgaFolaPanel.Size = new System.Drawing.Size(149, 44);
            this.btnHgaFolaPanel.TabIndex = 1;
            this.btnHgaFolaPanel.Text = "HGA FOLA Panel";
            this.btnHgaFolaPanel.UseVisualStyleBackColor = true;
            this.btnHgaFolaPanel.Click += new System.EventHandler(this.btnHgaFolaPanel_Click);
            // 
            // MainTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 123);
            this.Controls.Add(this.btnHgaBolaPanel);
            this.Controls.Add(this.btnHgaFolaPanel);
            this.Controls.Add(this.btnTestHgaBola);
            this.Controls.Add(this.btnTestHgaFola);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainTestForm";
            this.Text = "RFID Test Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestHgaBola;
        private System.Windows.Forms.Button btnTestHgaFola;
        private System.Windows.Forms.Button btnHgaBolaPanel;
        private System.Windows.Forms.Button btnHgaFolaPanel;
    }
}