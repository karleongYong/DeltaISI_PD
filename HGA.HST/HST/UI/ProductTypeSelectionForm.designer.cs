namespace Seagate.AAS.HGA.HST.UI
{
    partial class ProductTypeSelectionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductTypeSelectionForm));
            this.lblLotType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lstProductType = new System.Windows.Forms.ListView();
            this.headerRecipeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnConversionBoardID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerProductName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxProductID = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLotType
            // 
            this.lblLotType.AutoSize = true;
            this.lblLotType.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLotType.Location = new System.Drawing.Point(67, 15);
            this.lblLotType.Name = "lblLotType";
            this.lblLotType.Size = new System.Drawing.Size(87, 25);
            this.lblLotType.TabIndex = 2;
            this.lblLotType.Text = "Lot Type:";
            this.lblLotType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(337, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a HGA product recipe from the list below";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(322, 325);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(151, 48);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(127, 325);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(151, 48);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click_1);
            // 
            // lstProductType
            // 
            this.lstProductType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerRecipeName,
            this.columnConversionBoardID,
            this.headerProductName});
            this.lstProductType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProductType.FullRowSelect = true;
            this.lstProductType.GridLines = true;
            this.lstProductType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstProductType.HideSelection = false;
            this.lstProductType.Location = new System.Drawing.Point(12, 42);
            this.lstProductType.MultiSelect = false;
            this.lstProductType.Name = "lstProductType";
            this.lstProductType.OwnerDraw = true;
            this.lstProductType.Size = new System.Drawing.Size(461, 202);
            this.lstProductType.TabIndex = 1;
            this.lstProductType.UseCompatibleStateImageBehavior = false;
            this.lstProductType.View = System.Windows.Forms.View.Details;
            this.lstProductType.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lstProductType_DrawColumnHeader);
            this.lstProductType.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lstProductType_DrawItem);
            this.lstProductType.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lstProductType_DrawSubItem);
            this.lstProductType.SelectedIndexChanged += new System.EventHandler(this.lstProductType_SelectedIndexChanged);
            this.lstProductType.DoubleClick += new System.EventHandler(this.lstProductType_DoubleClick);
            // 
            // headerRecipeName
            // 
            this.headerRecipeName.Text = "Recipe Name";
            this.headerRecipeName.Width = 120;
            // 
            // columnConversionBoardID
            // 
            this.columnConversionBoardID.Text = "Board ID";
            this.columnConversionBoardID.Width = 140;
            // 
            // headerProductName
            // 
            this.headerProductName.Text = "Procut Name";
            this.headerProductName.Width = 200;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(73, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Selected Product Recipe Name:";
            // 
            // txtBoxProductID
            // 
            this.txtBoxProductID.BackColor = System.Drawing.SystemColors.Info;
            this.txtBoxProductID.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductID.ForeColor = System.Drawing.Color.Green;
            this.txtBoxProductID.Location = new System.Drawing.Point(304, 7);
            this.txtBoxProductID.Name = "txtBoxProductID";
            this.txtBoxProductID.Size = new System.Drawing.Size(169, 35);
            this.txtBoxProductID.TabIndex = 5;
            this.txtBoxProductID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.txtBoxProductID);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(-4, 253);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 50);
            this.panel1.TabIndex = 6;
            // 
            // ProductTypeSelectionForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(485, 399);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lstProductType);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductTypeSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HGA Product Type Selection";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLotType;
        private System.Windows.Forms.ListView lstProductType;
        private System.Windows.Forms.ColumnHeader headerRecipeName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader headerProductName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxProductID;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ColumnHeader columnConversionBoardID;
        private System.Windows.Forms.Panel panel1;
    }
}