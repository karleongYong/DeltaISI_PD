namespace Seagate.AAS.HGA.HST.Utils
{
    partial class SingleAxisForm
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
            this.buttonTeach = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.arrowButtonForward = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonBackward = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonLeft = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonRight = new Seagate.AAS.UI.ArrowButton();
            this.groupBoxMove = new System.Windows.Forms.GroupBox();
            this.buttonDIO = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.arrowButtonCW = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonCCW = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonDown = new Seagate.AAS.UI.ArrowButton();
            this.arrowButtonUp = new Seagate.AAS.UI.ArrowButton();
            this.checkBoxIsRelativeMove = new System.Windows.Forms.CheckBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.touchscreenNumBoxRelativeMoveDistance = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxSpeedPercentage = new Seagate.AAS.UI.TouchscreenNumBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxMoveToPosition = new System.Windows.Forms.TextBox();
            this.touchscreenNumBoxMoveToPosition = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxActualPosition = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxTeachPosition = new Seagate.AAS.UI.TouchscreenNumBox();
            this.buttonMoveTo = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.ledToggleEnableX = new Seagate.AAS.UI.Led();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelName = new System.Windows.Forms.Label();
            this.labelAxesName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.groupBoxMove.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonTeach
            // 
            this.buttonTeach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTeach.Location = new System.Drawing.Point(532, 9);
            this.buttonTeach.Name = "buttonTeach";
            this.buttonTeach.Size = new System.Drawing.Size(100, 40);
            this.buttonTeach.TabIndex = 0;
            this.buttonTeach.Text = "&Teach";
            this.buttonTeach.UseVisualStyleBackColor = true;
            this.buttonTeach.Click += new System.EventHandler(this.buttonTeach_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(652, 9);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 40);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Controls.Add(this.buttonTeach);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 319);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(776, 55);
            this.panel1.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Teach Position";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Actual Position";
            // 
            // arrowButtonForward
            // 
            this.arrowButtonForward.ArrowEnabled = true;
            this.arrowButtonForward.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonForward.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonForward.Location = new System.Drawing.Point(67, 23);
            this.arrowButtonForward.Name = "arrowButtonForward";
            this.arrowButtonForward.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonForward.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonForward.Rotation = 0;
            this.arrowButtonForward.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonForward.TabIndex = 11;
            this.arrowButtonForward.Text = "Forward";
            // 
            // arrowButtonBackward
            // 
            this.arrowButtonBackward.ArrowEnabled = true;
            this.arrowButtonBackward.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonBackward.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonBackward.Location = new System.Drawing.Point(66, 117);
            this.arrowButtonBackward.Name = "arrowButtonBackward";
            this.arrowButtonBackward.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonBackward.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonBackward.Rotation = 180;
            this.arrowButtonBackward.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonBackward.TabIndex = 12;
            this.arrowButtonBackward.Text = "Backward";
            // 
            // arrowButtonLeft
            // 
            this.arrowButtonLeft.ArrowEnabled = true;
            this.arrowButtonLeft.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonLeft.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonLeft.Location = new System.Drawing.Point(12, 68);
            this.arrowButtonLeft.Name = "arrowButtonLeft";
            this.arrowButtonLeft.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonLeft.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonLeft.Rotation = 270;
            this.arrowButtonLeft.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonLeft.TabIndex = 13;
            this.arrowButtonLeft.Text = "Left";
            // 
            // arrowButtonRight
            // 
            this.arrowButtonRight.ArrowEnabled = true;
            this.arrowButtonRight.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonRight.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonRight.Location = new System.Drawing.Point(117, 69);
            this.arrowButtonRight.Name = "arrowButtonRight";
            this.arrowButtonRight.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonRight.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonRight.Rotation = 90;
            this.arrowButtonRight.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonRight.TabIndex = 14;
            this.arrowButtonRight.Text = "Right";
            // 
            // groupBoxMove
            // 
            this.groupBoxMove.Controls.Add(this.buttonDIO);
            this.groupBoxMove.Controls.Add(this.groupBox1);
            this.groupBoxMove.Controls.Add(this.checkBoxIsRelativeMove);
            this.groupBoxMove.Controls.Add(this.buttonStop);
            this.groupBoxMove.Controls.Add(this.touchscreenNumBoxRelativeMoveDistance);
            this.groupBoxMove.Controls.Add(this.touchscreenNumBoxSpeedPercentage);
            this.groupBoxMove.Controls.Add(this.labelStatus);
            this.groupBoxMove.Controls.Add(this.label9);
            this.groupBoxMove.Controls.Add(this.label8);
            this.groupBoxMove.Location = new System.Drawing.Point(374, 58);
            this.groupBoxMove.Name = "groupBoxMove";
            this.groupBoxMove.Size = new System.Drawing.Size(385, 253);
            this.groupBoxMove.TabIndex = 15;
            this.groupBoxMove.TabStop = false;
            this.groupBoxMove.Text = "Move";
            // 
            // buttonDIO
            // 
            this.buttonDIO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDIO.Location = new System.Drawing.Point(269, 172);
            this.buttonDIO.Name = "buttonDIO";
            this.buttonDIO.Size = new System.Drawing.Size(100, 40);
            this.buttonDIO.TabIndex = 24;
            this.buttonDIO.Text = "Digital IO";
            this.buttonDIO.UseVisualStyleBackColor = true;
            this.buttonDIO.Visible = false;
            this.buttonDIO.Click += new System.EventHandler(this.buttonDIO_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.arrowButtonCW);
            this.groupBox1.Controls.Add(this.arrowButtonCCW);
            this.groupBox1.Controls.Add(this.arrowButtonRight);
            this.groupBox1.Controls.Add(this.arrowButtonLeft);
            this.groupBox1.Controls.Add(this.arrowButtonBackward);
            this.groupBox1.Controls.Add(this.arrowButtonDown);
            this.groupBox1.Controls.Add(this.arrowButtonUp);
            this.groupBox1.Controls.Add(this.arrowButtonForward);
            this.groupBox1.Location = new System.Drawing.Point(12, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 208);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            // 
            // arrowButtonCW
            // 
            this.arrowButtonCW.ArrowEnabled = true;
            this.arrowButtonCW.HoverEndColor = System.Drawing.Color.DarkRed;
            this.arrowButtonCW.HoverStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCW.Location = new System.Drawing.Point(71, 129);
            this.arrowButtonCW.Name = "arrowButtonCW";
            this.arrowButtonCW.NormalEndColor = System.Drawing.Color.DarkGray;
            this.arrowButtonCW.NormalStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCW.Rotation = 0;
            this.arrowButtonCW.Size = new System.Drawing.Size(52, 52);
            this.arrowButtonCW.TabIndex = 22;
            this.arrowButtonCW.Text = "CW";
            // 
            // arrowButtonCCW
            // 
            this.arrowButtonCCW.ArrowEnabled = true;
            this.arrowButtonCCW.HoverEndColor = System.Drawing.Color.DarkRed;
            this.arrowButtonCCW.HoverStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCCW.Location = new System.Drawing.Point(71, 25);
            this.arrowButtonCCW.Name = "arrowButtonCCW";
            this.arrowButtonCCW.NormalEndColor = System.Drawing.Color.DarkGray;
            this.arrowButtonCCW.NormalStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCCW.Rotation = 0;
            this.arrowButtonCCW.Size = new System.Drawing.Size(52, 52);
            this.arrowButtonCCW.TabIndex = 21;
            this.arrowButtonCCW.Text = "CCW";
            // 
            // arrowButtonDown
            // 
            this.arrowButtonDown.ArrowEnabled = true;
            this.arrowButtonDown.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonDown.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonDown.Location = new System.Drawing.Point(66, 117);
            this.arrowButtonDown.Name = "arrowButtonDown";
            this.arrowButtonDown.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonDown.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonDown.Rotation = 180;
            this.arrowButtonDown.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonDown.TabIndex = 11;
            this.arrowButtonDown.Text = "Down";
            // 
            // arrowButtonUp
            // 
            this.arrowButtonUp.ArrowEnabled = true;
            this.arrowButtonUp.HoverEndColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonUp.HoverStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonUp.Location = new System.Drawing.Point(67, 23);
            this.arrowButtonUp.Name = "arrowButtonUp";
            this.arrowButtonUp.NormalEndColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonUp.NormalStartColor = System.Drawing.Color.Gainsboro;
            this.arrowButtonUp.Rotation = 0;
            this.arrowButtonUp.Size = new System.Drawing.Size(64, 64);
            this.arrowButtonUp.TabIndex = 11;
            this.arrowButtonUp.Text = "Up";
            // 
            // checkBoxIsRelativeMove
            // 
            this.checkBoxIsRelativeMove.AutoSize = true;
            this.checkBoxIsRelativeMove.Checked = true;
            this.checkBoxIsRelativeMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsRelativeMove.Enabled = false;
            this.checkBoxIsRelativeMove.Location = new System.Drawing.Point(226, 27);
            this.checkBoxIsRelativeMove.Name = "checkBoxIsRelativeMove";
            this.checkBoxIsRelativeMove.Size = new System.Drawing.Size(95, 17);
            this.checkBoxIsRelativeMove.TabIndex = 16;
            this.checkBoxIsRelativeMove.Text = "Relative Move";
            this.checkBoxIsRelativeMove.UseVisualStyleBackColor = true;
            this.checkBoxIsRelativeMove.Visible = false;
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(269, 113);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(100, 40);
            this.buttonStop.TabIndex = 19;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // touchscreenNumBoxRelativeMoveDistance
            // 
            this.touchscreenNumBoxRelativeMoveDistance.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBoxRelativeMoveDistance.IntegerOnly = false;
            this.touchscreenNumBoxRelativeMoveDistance.Location = new System.Drawing.Point(306, 44);
            this.touchscreenNumBoxRelativeMoveDistance.Max = 99999999D;
            this.touchscreenNumBoxRelativeMoveDistance.Min = 0D;
            this.touchscreenNumBoxRelativeMoveDistance.Name = "touchscreenNumBoxRelativeMoveDistance";
            this.touchscreenNumBoxRelativeMoveDistance.Size = new System.Drawing.Size(68, 20);
            this.touchscreenNumBoxRelativeMoveDistance.TabIndex = 15;
            this.touchscreenNumBoxRelativeMoveDistance.Text = "0.1";
            // 
            // touchscreenNumBoxSpeedPercentage
            // 
            this.touchscreenNumBoxSpeedPercentage.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBoxSpeedPercentage.IntegerOnly = false;
            this.touchscreenNumBoxSpeedPercentage.Location = new System.Drawing.Point(227, 89);
            this.touchscreenNumBoxSpeedPercentage.Max = 100D;
            this.touchscreenNumBoxSpeedPercentage.Min = 0.01D;
            this.touchscreenNumBoxSpeedPercentage.Name = "touchscreenNumBoxSpeedPercentage";
            this.touchscreenNumBoxSpeedPercentage.Size = new System.Drawing.Size(63, 20);
            this.touchscreenNumBoxSpeedPercentage.TabIndex = 15;
            this.touchscreenNumBoxSpeedPercentage.Text = "100";
            this.touchscreenNumBoxSpeedPercentage.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(9, 230);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(37, 13);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Status";
            this.labelStatus.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(224, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Move Distance:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(223, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Jog Speed Reduction(%):";
            this.label8.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxMoveToPosition);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxMoveToPosition);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxActualPosition);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxTeachPosition);
            this.groupBox3.Controls.Add(this.buttonMoveTo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.ledToggleEnableX);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(356, 253);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status & Control";
            // 
            // textBoxMoveToPosition
            // 
            this.textBoxMoveToPosition.Location = new System.Drawing.Point(121, 91);
            this.textBoxMoveToPosition.Name = "textBoxMoveToPosition";
            this.textBoxMoveToPosition.Size = new System.Drawing.Size(93, 20);
            this.textBoxMoveToPosition.TabIndex = 27;
            // 
            // touchscreenNumBoxMoveToPosition
            // 
            this.touchscreenNumBoxMoveToPosition.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBoxMoveToPosition.IntegerOnly = false;
            this.touchscreenNumBoxMoveToPosition.Location = new System.Drawing.Point(120, 120);
            this.touchscreenNumBoxMoveToPosition.Max = 999999999D;
            this.touchscreenNumBoxMoveToPosition.Min = -999999999D;
            this.touchscreenNumBoxMoveToPosition.Name = "touchscreenNumBoxMoveToPosition";
            this.touchscreenNumBoxMoveToPosition.Size = new System.Drawing.Size(93, 20);
            this.touchscreenNumBoxMoveToPosition.TabIndex = 26;
            this.touchscreenNumBoxMoveToPosition.Text = "0";
            this.touchscreenNumBoxMoveToPosition.Visible = false;
            // 
            // touchscreenNumBoxActualPosition
            // 
            this.touchscreenNumBoxActualPosition.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBoxActualPosition.Enabled = false;
            this.touchscreenNumBoxActualPosition.IntegerOnly = false;
            this.touchscreenNumBoxActualPosition.Location = new System.Drawing.Point(120, 62);
            this.touchscreenNumBoxActualPosition.Max = 999999999D;
            this.touchscreenNumBoxActualPosition.Min = -999999999D;
            this.touchscreenNumBoxActualPosition.Name = "touchscreenNumBoxActualPosition";
            this.touchscreenNumBoxActualPosition.Size = new System.Drawing.Size(93, 20);
            this.touchscreenNumBoxActualPosition.TabIndex = 25;
            this.touchscreenNumBoxActualPosition.Text = "0";
            // 
            // touchscreenNumBoxTeachPosition
            // 
            this.touchscreenNumBoxTeachPosition.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBoxTeachPosition.Enabled = false;
            this.touchscreenNumBoxTeachPosition.IntegerOnly = false;
            this.touchscreenNumBoxTeachPosition.Location = new System.Drawing.Point(120, 34);
            this.touchscreenNumBoxTeachPosition.Max = 999999999D;
            this.touchscreenNumBoxTeachPosition.Min = -999999999D;
            this.touchscreenNumBoxTeachPosition.Name = "touchscreenNumBoxTeachPosition";
            this.touchscreenNumBoxTeachPosition.Size = new System.Drawing.Size(93, 20);
            this.touchscreenNumBoxTeachPosition.TabIndex = 24;
            this.touchscreenNumBoxTeachPosition.Text = "0";
            // 
            // buttonMoveTo
            // 
            this.buttonMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveTo.Location = new System.Drawing.Point(238, 79);
            this.buttonMoveTo.Name = "buttonMoveTo";
            this.buttonMoveTo.Size = new System.Drawing.Size(100, 40);
            this.buttonMoveTo.TabIndex = 18;
            this.buttonMoveTo.Text = "Move To";
            this.buttonMoveTo.UseVisualStyleBackColor = true;
            this.buttonMoveTo.Click += new System.EventHandler(this.buttonMoveTo_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Move To Position";
            // 
            // ledToggleEnableX
            // 
            this.ledToggleEnableX.DisplayAsButton = true;
            this.ledToggleEnableX.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledToggleEnableX.Location = new System.Drawing.Point(120, 148);
            this.ledToggleEnableX.Name = "ledToggleEnableX";
            this.ledToggleEnableX.Size = new System.Drawing.Size(93, 40);
            this.ledToggleEnableX.State = false;
            this.ledToggleEnableX.TabIndex = 20;
            this.ledToggleEnableX.Text = "Toggle Enable";
            this.ledToggleEnableX.Visible = false;
            this.ledToggleEnableX.Click += new System.EventHandler(this.ledToggleEnableX_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(9, 7);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(53, 16);
            this.labelName.TabIndex = 20;
            this.labelName.Text = "Name:";
            // 
            // labelAxesName
            // 
            this.labelAxesName.AutoSize = true;
            this.labelAxesName.Location = new System.Drawing.Point(13, 30);
            this.labelAxesName.Name = "labelAxesName";
            this.labelAxesName.Size = new System.Drawing.Size(67, 13);
            this.labelAxesName.TabIndex = 21;
            this.labelAxesName.Text = "Axes Name: ";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel2.Controls.Add(this.labelName);
            this.panel2.Controls.Add(this.labelAxesName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(776, 52);
            this.panel2.TabIndex = 22;
            // 
            // SingleAxisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 374);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxMove);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SingleAxisForm";
            this.Text = "Gantry Teach";
            this.Load += new System.EventHandler(this.GantryTeachForm_Load);
            this.VisibleChanged += new System.EventHandler(this.SingleAxisTeachForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.groupBoxMove.ResumeLayout(false);
            this.groupBoxMove.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonTeach;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private Seagate.AAS.UI.ArrowButton arrowButtonForward;
        private Seagate.AAS.UI.ArrowButton arrowButtonBackward;
        private Seagate.AAS.UI.ArrowButton arrowButtonLeft;
        private Seagate.AAS.UI.ArrowButton arrowButtonRight;
        private System.Windows.Forms.GroupBox groupBoxMove;
        private Seagate.AAS.UI.ArrowButton arrowButtonDown;
        private Seagate.AAS.UI.ArrowButton arrowButtonUp;
        private System.Windows.Forms.GroupBox groupBox3;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxSpeedPercentage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox checkBoxIsRelativeMove;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxRelativeMoveDistance;
        private System.Windows.Forms.Button buttonMoveTo;
        private System.Windows.Forms.Button buttonStop;
        private Seagate.AAS.UI.Led ledToggleEnableX;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelAxesName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxMoveToPosition;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxActualPosition;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxTeachPosition;
        private Seagate.AAS.UI.ArrowButton arrowButtonCCW;
        private Seagate.AAS.UI.ArrowButton arrowButtonCW;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonDIO;
        private System.Windows.Forms.TextBox textBoxMoveToPosition;
    }
}