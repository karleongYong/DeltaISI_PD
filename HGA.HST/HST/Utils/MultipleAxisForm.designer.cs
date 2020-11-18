namespace Seagate.AAS.HGA.HST.Utils
{
    partial class MultipleAxisForm
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
            this.textBoxMoveToPositionTheta = new System.Windows.Forms.TextBox();
            this.textBoxMoveToPositionY = new System.Windows.Forms.TextBox();
            this.textBoxMoveToPositionX = new System.Windows.Forms.TextBox();
            this.labelTheta = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.touchscreenNumBoxMoveToPositionTheta = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxActualPositionTheta = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxTeachPositionTheta = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxMoveToPositionY = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxActualPositionY = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxTeachPositionY = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxMoveToPositionX = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxActualPositionX = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenNumBoxTeachPositionX = new Seagate.AAS.UI.TouchscreenNumBox();
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
            this.buttonTeach.Location = new System.Drawing.Point(657, 9);
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
            this.buttonCancel.Location = new System.Drawing.Point(777, 9);
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
            this.panel1.Location = new System.Drawing.Point(0, 315);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(901, 55);
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
            this.arrowButtonRight.Location = new System.Drawing.Point(115, 69);
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
            this.groupBoxMove.Location = new System.Drawing.Point(499, 58);
            this.groupBoxMove.Name = "groupBoxMove";
            this.groupBoxMove.Size = new System.Drawing.Size(391, 251);
            this.groupBoxMove.TabIndex = 15;
            this.groupBoxMove.TabStop = false;
            this.groupBoxMove.Text = "Move";
            // 
            // buttonDIO
            // 
            this.buttonDIO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDIO.Location = new System.Drawing.Point(267, 182);
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
            this.groupBox1.Location = new System.Drawing.Point(15, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 204);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            // 
            // arrowButtonCW
            // 
            this.arrowButtonCW.ArrowEnabled = true;
            this.arrowButtonCW.HoverEndColor = System.Drawing.Color.DarkRed;
            this.arrowButtonCW.HoverStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCW.Location = new System.Drawing.Point(12, 139);
            this.arrowButtonCW.Name = "arrowButtonCW";
            this.arrowButtonCW.NormalEndColor = System.Drawing.Color.DarkGray;
            this.arrowButtonCW.NormalStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCW.Rotation = 315;
            this.arrowButtonCW.Size = new System.Drawing.Size(52, 52);
            this.arrowButtonCW.TabIndex = 22;
            this.arrowButtonCW.Text = "CW";
            // 
            // arrowButtonCCW
            // 
            this.arrowButtonCCW.ArrowEnabled = true;
            this.arrowButtonCCW.HoverEndColor = System.Drawing.Color.DarkRed;
            this.arrowButtonCCW.HoverStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCCW.Location = new System.Drawing.Point(136, 139);
            this.arrowButtonCCW.Name = "arrowButtonCCW";
            this.arrowButtonCCW.NormalEndColor = System.Drawing.Color.DarkGray;
            this.arrowButtonCCW.NormalStartColor = System.Drawing.Color.WhiteSmoke;
            this.arrowButtonCCW.Rotation = 45;
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
            this.arrowButtonDown.Text = "Front";
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
            this.arrowButtonUp.Text = "Back";
            // 
            // checkBoxIsRelativeMove
            // 
            this.checkBoxIsRelativeMove.AutoSize = true;
            this.checkBoxIsRelativeMove.Checked = true;
            this.checkBoxIsRelativeMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsRelativeMove.Enabled = false;
            this.checkBoxIsRelativeMove.Location = new System.Drawing.Point(226, 19);
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
            this.buttonStop.Location = new System.Drawing.Point(267, 123);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(100, 40);
            this.buttonStop.TabIndex = 19;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // touchscreenNumBoxRelativeMoveDistance
            // 
            this.touchscreenNumBoxRelativeMoveDistance.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxRelativeMoveDistance.IntegerOnly = false;
            this.touchscreenNumBoxRelativeMoveDistance.Location = new System.Drawing.Point(314, 40);
            this.touchscreenNumBoxRelativeMoveDistance.Max = 99999999D;
            this.touchscreenNumBoxRelativeMoveDistance.Min = 0D;
            this.touchscreenNumBoxRelativeMoveDistance.Name = "touchscreenNumBoxRelativeMoveDistance";
            this.touchscreenNumBoxRelativeMoveDistance.ReadOnly = true;
            this.touchscreenNumBoxRelativeMoveDistance.Size = new System.Drawing.Size(53, 20);
            this.touchscreenNumBoxRelativeMoveDistance.TabIndex = 15;
            this.touchscreenNumBoxRelativeMoveDistance.Text = "0.1";
            // 
            // touchscreenNumBoxSpeedPercentage
            // 
            this.touchscreenNumBoxSpeedPercentage.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxSpeedPercentage.IntegerOnly = false;
            this.touchscreenNumBoxSpeedPercentage.Location = new System.Drawing.Point(230, 94);
            this.touchscreenNumBoxSpeedPercentage.Max = 100D;
            this.touchscreenNumBoxSpeedPercentage.Min = 0.01D;
            this.touchscreenNumBoxSpeedPercentage.Name = "touchscreenNumBoxSpeedPercentage";
            this.touchscreenNumBoxSpeedPercentage.ReadOnly = true;
            this.touchscreenNumBoxSpeedPercentage.Size = new System.Drawing.Size(63, 20);
            this.touchscreenNumBoxSpeedPercentage.TabIndex = 15;
            this.touchscreenNumBoxSpeedPercentage.Text = "100";
            this.touchscreenNumBoxSpeedPercentage.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 227);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(37, 13);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Status";
            this.labelStatus.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(226, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Move Distance:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(226, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Jog Speed Reduction(%):";
            this.label8.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxMoveToPositionTheta);
            this.groupBox3.Controls.Add(this.textBoxMoveToPositionY);
            this.groupBox3.Controls.Add(this.textBoxMoveToPositionX);
            this.groupBox3.Controls.Add(this.labelTheta);
            this.groupBox3.Controls.Add(this.labelY);
            this.groupBox3.Controls.Add(this.labelX);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxMoveToPositionTheta);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxActualPositionTheta);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxTeachPositionTheta);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxMoveToPositionY);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxActualPositionY);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxTeachPositionY);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxMoveToPositionX);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxActualPositionX);
            this.groupBox3.Controls.Add(this.touchscreenNumBoxTeachPositionX);
            this.groupBox3.Controls.Add(this.buttonMoveTo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.ledToggleEnableX);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(16, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(477, 251);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status & Control";
            // 
            // textBoxMoveToPositionTheta
            // 
            this.textBoxMoveToPositionTheta.Location = new System.Drawing.Point(281, 93);
            this.textBoxMoveToPositionTheta.Name = "textBoxMoveToPositionTheta";
            this.textBoxMoveToPositionTheta.Size = new System.Drawing.Size(71, 20);
            this.textBoxMoveToPositionTheta.TabIndex = 38;
            // 
            // textBoxMoveToPositionY
            // 
            this.textBoxMoveToPositionY.Location = new System.Drawing.Point(199, 93);
            this.textBoxMoveToPositionY.Name = "textBoxMoveToPositionY";
            this.textBoxMoveToPositionY.Size = new System.Drawing.Size(71, 20);
            this.textBoxMoveToPositionY.TabIndex = 37;
            // 
            // textBoxMoveToPositionX
            // 
            this.textBoxMoveToPositionX.Location = new System.Drawing.Point(120, 94);
            this.textBoxMoveToPositionX.Name = "textBoxMoveToPositionX";
            this.textBoxMoveToPositionX.Size = new System.Drawing.Size(71, 20);
            this.textBoxMoveToPositionX.TabIndex = 36;
            // 
            // labelTheta
            // 
            this.labelTheta.AutoSize = true;
            this.labelTheta.Location = new System.Drawing.Point(300, 17);
            this.labelTheta.Name = "labelTheta";
            this.labelTheta.Size = new System.Drawing.Size(35, 13);
            this.labelTheta.TabIndex = 35;
            this.labelTheta.Text = "Theta";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(224, 17);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(14, 13);
            this.labelY.TabIndex = 34;
            this.labelY.Text = "Y";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(147, 17);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(14, 13);
            this.labelX.TabIndex = 33;
            this.labelX.Text = "X";
            // 
            // touchscreenNumBoxMoveToPositionTheta
            // 
            this.touchscreenNumBoxMoveToPositionTheta.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxMoveToPositionTheta.IntegerOnly = false;
            this.touchscreenNumBoxMoveToPositionTheta.Location = new System.Drawing.Point(281, 123);
            this.touchscreenNumBoxMoveToPositionTheta.Max = 999999999D;
            this.touchscreenNumBoxMoveToPositionTheta.Min = -999999999D;
            this.touchscreenNumBoxMoveToPositionTheta.Name = "touchscreenNumBoxMoveToPositionTheta";
            this.touchscreenNumBoxMoveToPositionTheta.ReadOnly = true;
            this.touchscreenNumBoxMoveToPositionTheta.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxMoveToPositionTheta.TabIndex = 32;
            this.touchscreenNumBoxMoveToPositionTheta.Text = "0";
            this.touchscreenNumBoxMoveToPositionTheta.Visible = false;
            // 
            // touchscreenNumBoxActualPositionTheta
            // 
            this.touchscreenNumBoxActualPositionTheta.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxActualPositionTheta.Enabled = false;
            this.touchscreenNumBoxActualPositionTheta.IntegerOnly = false;
            this.touchscreenNumBoxActualPositionTheta.Location = new System.Drawing.Point(281, 61);
            this.touchscreenNumBoxActualPositionTheta.Max = 999999999D;
            this.touchscreenNumBoxActualPositionTheta.Min = -999999999D;
            this.touchscreenNumBoxActualPositionTheta.Name = "touchscreenNumBoxActualPositionTheta";
            this.touchscreenNumBoxActualPositionTheta.ReadOnly = true;
            this.touchscreenNumBoxActualPositionTheta.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxActualPositionTheta.TabIndex = 31;
            this.touchscreenNumBoxActualPositionTheta.Text = "0";
            // 
            // touchscreenNumBoxTeachPositionTheta
            // 
            this.touchscreenNumBoxTeachPositionTheta.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxTeachPositionTheta.Enabled = false;
            this.touchscreenNumBoxTeachPositionTheta.IntegerOnly = false;
            this.touchscreenNumBoxTeachPositionTheta.Location = new System.Drawing.Point(281, 33);
            this.touchscreenNumBoxTeachPositionTheta.Max = 999999999D;
            this.touchscreenNumBoxTeachPositionTheta.Min = -999999999D;
            this.touchscreenNumBoxTeachPositionTheta.Name = "touchscreenNumBoxTeachPositionTheta";
            this.touchscreenNumBoxTeachPositionTheta.ReadOnly = true;
            this.touchscreenNumBoxTeachPositionTheta.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxTeachPositionTheta.TabIndex = 30;
            this.touchscreenNumBoxTeachPositionTheta.Text = "0";
            // 
            // touchscreenNumBoxMoveToPositionY
            // 
            this.touchscreenNumBoxMoveToPositionY.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxMoveToPositionY.IntegerOnly = false;
            this.touchscreenNumBoxMoveToPositionY.Location = new System.Drawing.Point(199, 123);
            this.touchscreenNumBoxMoveToPositionY.Max = 999999999D;
            this.touchscreenNumBoxMoveToPositionY.Min = -999999999D;
            this.touchscreenNumBoxMoveToPositionY.Name = "touchscreenNumBoxMoveToPositionY";
            this.touchscreenNumBoxMoveToPositionY.ReadOnly = true;
            this.touchscreenNumBoxMoveToPositionY.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxMoveToPositionY.TabIndex = 29;
            this.touchscreenNumBoxMoveToPositionY.Text = "0";
            this.touchscreenNumBoxMoveToPositionY.Visible = false;
            // 
            // touchscreenNumBoxActualPositionY
            // 
            this.touchscreenNumBoxActualPositionY.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxActualPositionY.Enabled = false;
            this.touchscreenNumBoxActualPositionY.IntegerOnly = false;
            this.touchscreenNumBoxActualPositionY.Location = new System.Drawing.Point(199, 61);
            this.touchscreenNumBoxActualPositionY.Max = 999999999D;
            this.touchscreenNumBoxActualPositionY.Min = -999999999D;
            this.touchscreenNumBoxActualPositionY.Name = "touchscreenNumBoxActualPositionY";
            this.touchscreenNumBoxActualPositionY.ReadOnly = true;
            this.touchscreenNumBoxActualPositionY.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxActualPositionY.TabIndex = 28;
            this.touchscreenNumBoxActualPositionY.Text = "0";
            // 
            // touchscreenNumBoxTeachPositionY
            // 
            this.touchscreenNumBoxTeachPositionY.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxTeachPositionY.Enabled = false;
            this.touchscreenNumBoxTeachPositionY.IntegerOnly = false;
            this.touchscreenNumBoxTeachPositionY.Location = new System.Drawing.Point(199, 33);
            this.touchscreenNumBoxTeachPositionY.Max = 999999999D;
            this.touchscreenNumBoxTeachPositionY.Min = -999999999D;
            this.touchscreenNumBoxTeachPositionY.Name = "touchscreenNumBoxTeachPositionY";
            this.touchscreenNumBoxTeachPositionY.ReadOnly = true;
            this.touchscreenNumBoxTeachPositionY.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxTeachPositionY.TabIndex = 27;
            this.touchscreenNumBoxTeachPositionY.Text = "0";
            // 
            // touchscreenNumBoxMoveToPositionX
            // 
            this.touchscreenNumBoxMoveToPositionX.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxMoveToPositionX.IntegerOnly = false;
            this.touchscreenNumBoxMoveToPositionX.Location = new System.Drawing.Point(120, 124);
            this.touchscreenNumBoxMoveToPositionX.Max = 999999999D;
            this.touchscreenNumBoxMoveToPositionX.Min = -999999999D;
            this.touchscreenNumBoxMoveToPositionX.Name = "touchscreenNumBoxMoveToPositionX";
            this.touchscreenNumBoxMoveToPositionX.ReadOnly = true;
            this.touchscreenNumBoxMoveToPositionX.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxMoveToPositionX.TabIndex = 26;
            this.touchscreenNumBoxMoveToPositionX.Text = "0";
            this.touchscreenNumBoxMoveToPositionX.Visible = false;
            // 
            // touchscreenNumBoxActualPositionX
            // 
            this.touchscreenNumBoxActualPositionX.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxActualPositionX.Enabled = false;
            this.touchscreenNumBoxActualPositionX.IntegerOnly = false;
            this.touchscreenNumBoxActualPositionX.Location = new System.Drawing.Point(120, 62);
            this.touchscreenNumBoxActualPositionX.Max = 999999999D;
            this.touchscreenNumBoxActualPositionX.Min = -999999999D;
            this.touchscreenNumBoxActualPositionX.Name = "touchscreenNumBoxActualPositionX";
            this.touchscreenNumBoxActualPositionX.ReadOnly = true;
            this.touchscreenNumBoxActualPositionX.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxActualPositionX.TabIndex = 25;
            this.touchscreenNumBoxActualPositionX.Text = "0";
            // 
            // touchscreenNumBoxTeachPositionX
            // 
            this.touchscreenNumBoxTeachPositionX.BackColor = System.Drawing.SystemColors.Control;
            this.touchscreenNumBoxTeachPositionX.Enabled = false;
            this.touchscreenNumBoxTeachPositionX.IntegerOnly = false;
            this.touchscreenNumBoxTeachPositionX.Location = new System.Drawing.Point(120, 34);
            this.touchscreenNumBoxTeachPositionX.Max = 999999999D;
            this.touchscreenNumBoxTeachPositionX.Min = -999999999D;
            this.touchscreenNumBoxTeachPositionX.Name = "touchscreenNumBoxTeachPositionX";
            this.touchscreenNumBoxTeachPositionX.ReadOnly = true;
            this.touchscreenNumBoxTeachPositionX.Size = new System.Drawing.Size(71, 20);
            this.touchscreenNumBoxTeachPositionX.TabIndex = 24;
            this.touchscreenNumBoxTeachPositionX.Text = "0";
            // 
            // buttonMoveTo
            // 
            this.buttonMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveTo.Location = new System.Drawing.Point(365, 78);
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
            this.ledToggleEnableX.Location = new System.Drawing.Point(98, 149);
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
            this.panel2.Size = new System.Drawing.Size(901, 52);
            this.panel2.TabIndex = 22;
            // 
            // MultipleAxisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 370);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxMove);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MultipleAxisForm";
            this.Text = "Gantry Teach";
            this.Load += new System.EventHandler(this.GantryTeachForm_Load);
            this.VisibleChanged += new System.EventHandler(this.MultipleAxisTeachForm_VisibleChanged);
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
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxMoveToPositionX;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxActualPositionX;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBoxTeachPositionX;
        private Seagate.AAS.UI.ArrowButton arrowButtonCCW;
        private Seagate.AAS.UI.ArrowButton arrowButtonCW;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonDIO;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxMoveToPositionTheta;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxActualPositionTheta;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxTeachPositionTheta;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxMoveToPositionY;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxActualPositionY;
        private AAS.UI.TouchscreenNumBox touchscreenNumBoxTeachPositionY;
        private System.Windows.Forms.Label labelTheta;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.TextBox textBoxMoveToPositionX;
        private System.Windows.Forms.TextBox textBoxMoveToPositionY;
        private System.Windows.Forms.TextBox textBoxMoveToPositionTheta;
    }
}