using System;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Hw;
namespace Seagate.AAS.Parsel.Device.TurnSection
{
	/// <summary>
	/// Summary description for PanelTurnTable.
	/// </summary>
	public class PanelTurnSection : System.Windows.Forms.UserControl
	{
		private Seagate.AAS.UI.Led ledInPosition;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer timer1;
		private ITurnSection myTurnSection = null;
		private Seagate.AAS.UI.Led ledZoneSensor;
		private Seagate.AAS.UI.Led ledExitSensor;
		private Seagate.AAS.UI.Led ledEntranceSensor;
		private Seagate.AAS.UI.Led ledEntranceClear;
		private System.Windows.Forms.Button buttonEntranceRelayOn;
		private System.Windows.Forms.Button buttonExitRelayOn;
		private Seagate.AAS.Parsel.Device.PneumaticControl.UI.PneumaticControlUI pneumaticControlUIRotaryCylinder;
		private System.Windows.Forms.Label labelTurnSectionName;
        private UI.IndicatorButton indicatorButtonInhibit;
		private Seagate.AAS.UI.Led ledExitClear;


		public PanelTurnSection()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
						
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelTurnSection));
            this.ledZoneSensor = new Seagate.AAS.UI.Led();
            this.ledInPosition = new Seagate.AAS.UI.Led();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ledExitSensor = new Seagate.AAS.UI.Led();
            this.ledEntranceSensor = new Seagate.AAS.UI.Led();
            this.ledEntranceClear = new Seagate.AAS.UI.Led();
            this.ledExitClear = new Seagate.AAS.UI.Led();
            this.buttonEntranceRelayOn = new System.Windows.Forms.Button();
            this.buttonExitRelayOn = new System.Windows.Forms.Button();
            this.pneumaticControlUIRotaryCylinder = new Seagate.AAS.Parsel.Device.PneumaticControl.UI.PneumaticControlUI();
            this.labelTurnSectionName = new System.Windows.Forms.Label();
            this.indicatorButtonInhibit = new Seagate.AAS.UI.IndicatorButton();
            this.SuspendLayout();
            // 
            // ledZoneSensor
            // 
            this.ledZoneSensor.DisplayAsButton = false;
            this.ledZoneSensor.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledZoneSensor.Location = new System.Drawing.Point(16, 51);
            this.ledZoneSensor.Name = "ledZoneSensor";
            this.ledZoneSensor.Size = new System.Drawing.Size(120, 24);
            this.ledZoneSensor.State = false;
            this.ledZoneSensor.TabIndex = 3;
            this.ledZoneSensor.Text = "Zone Sensor";
            // 
            // ledInPosition
            // 
            this.ledInPosition.DisplayAsButton = false;
            this.ledInPosition.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledInPosition.Location = new System.Drawing.Point(16, 22);
            this.ledInPosition.Name = "ledInPosition";
            this.ledInPosition.Size = new System.Drawing.Size(120, 24);
            this.ledInPosition.State = false;
            this.ledInPosition.TabIndex = 2;
            this.ledInPosition.Text = "In Position";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ledExitSensor
            // 
            this.ledExitSensor.DisplayAsButton = false;
            this.ledExitSensor.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledExitSensor.Location = new System.Drawing.Point(11, 330);
            this.ledExitSensor.Name = "ledExitSensor";
            this.ledExitSensor.Size = new System.Drawing.Size(120, 24);
            this.ledExitSensor.State = false;
            this.ledExitSensor.TabIndex = 7;
            this.ledExitSensor.Text = "Exit";
            this.ledExitSensor.Visible = false;
            // 
            // ledEntranceSensor
            // 
            this.ledEntranceSensor.DisplayAsButton = false;
            this.ledEntranceSensor.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledEntranceSensor.Location = new System.Drawing.Point(34, 345);
            this.ledEntranceSensor.Name = "ledEntranceSensor";
            this.ledEntranceSensor.Size = new System.Drawing.Size(88, 24);
            this.ledEntranceSensor.State = false;
            this.ledEntranceSensor.TabIndex = 6;
            this.ledEntranceSensor.Text = "Entrance";
            this.ledEntranceSensor.Visible = false;
            // 
            // ledEntranceClear
            // 
            this.ledEntranceClear.DisplayAsButton = false;
            this.ledEntranceClear.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledEntranceClear.Location = new System.Drawing.Point(16, 81);
            this.ledEntranceClear.Name = "ledEntranceClear";
            this.ledEntranceClear.Size = new System.Drawing.Size(120, 24);
            this.ledEntranceClear.State = false;
            this.ledEntranceClear.TabIndex = 9;
            this.ledEntranceClear.Text = "Entrance Clear";
            // 
            // ledExitClear
            // 
            this.ledExitClear.DisplayAsButton = false;
            this.ledExitClear.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.ledExitClear.Location = new System.Drawing.Point(16, 110);
            this.ledExitClear.Name = "ledExitClear";
            this.ledExitClear.Size = new System.Drawing.Size(120, 24);
            this.ledExitClear.State = false;
            this.ledExitClear.TabIndex = 8;
            this.ledExitClear.Text = "Exit Clear";
            // 
            // buttonEntranceRelayOn
            // 
            this.buttonEntranceRelayOn.Location = new System.Drawing.Point(11, 136);
            this.buttonEntranceRelayOn.Name = "buttonEntranceRelayOn";
            this.buttonEntranceRelayOn.Size = new System.Drawing.Size(120, 32);
            this.buttonEntranceRelayOn.TabIndex = 12;
            this.buttonEntranceRelayOn.Text = "Entrance Relay ON";
            this.buttonEntranceRelayOn.Click += new System.EventHandler(this.buttonEntranceRelayOn_Click);
            // 
            // buttonExitRelayOn
            // 
            this.buttonExitRelayOn.Location = new System.Drawing.Point(11, 172);
            this.buttonExitRelayOn.Name = "buttonExitRelayOn";
            this.buttonExitRelayOn.Size = new System.Drawing.Size(120, 32);
            this.buttonExitRelayOn.TabIndex = 13;
            this.buttonExitRelayOn.Text = "Exit Relay ON";
            this.buttonExitRelayOn.Click += new System.EventHandler(this.buttonExitRelayOn_Click);
            // 
            // pneumaticControlUIRotaryCylinder
            // 
            this.pneumaticControlUIRotaryCylinder.ActuatorName = "Unassigned";
            this.pneumaticControlUIRotaryCylinder.Enabled = false;
            this.pneumaticControlUIRotaryCylinder.Location = new System.Drawing.Point(8, 210);
            this.pneumaticControlUIRotaryCylinder.Name = "pneumaticControlUIRotaryCylinder";
            this.pneumaticControlUIRotaryCylinder.RefreshIntervalMilliseconds = 100;
            this.pneumaticControlUIRotaryCylinder.Size = new System.Drawing.Size(128, 120);
            this.pneumaticControlUIRotaryCylinder.TabIndex = 14;
            this.pneumaticControlUIRotaryCylinder.Load += new System.EventHandler(this.pneumaticControlUIRotaryCylinder_Load);
            // 
            // labelTurnSectionName
            // 
            this.labelTurnSectionName.Location = new System.Drawing.Point(8, -8);
            this.labelTurnSectionName.Name = "labelTurnSectionName";
            this.labelTurnSectionName.Size = new System.Drawing.Size(128, 23);
            this.labelTurnSectionName.TabIndex = 15;
            this.labelTurnSectionName.Text = "Name";
            this.labelTurnSectionName.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // indicatorButtonInhibit
            // 
            this.indicatorButtonInhibit.Image = ((System.Drawing.Image)(resources.GetObject("indicatorButtonInhibit.Image")));
            this.indicatorButtonInhibit.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.indicatorButtonInhibit.Location = new System.Drawing.Point(16, 330);
            this.indicatorButtonInhibit.Name = "indicatorButtonInhibit";
            this.indicatorButtonInhibit.Size = new System.Drawing.Size(95, 38);
            this.indicatorButtonInhibit.State = false;
            this.indicatorButtonInhibit.TabIndex = 16;
            this.indicatorButtonInhibit.Text = "Inhibit";
            this.indicatorButtonInhibit.UseVisualStyleBackColor = true;
            this.indicatorButtonInhibit.Click += new System.EventHandler(this.indicatorButtonInhibit_Click);
            // 
            // PanelTurnSection
            // 
            this.Controls.Add(this.indicatorButtonInhibit);
            this.Controls.Add(this.labelTurnSectionName);
            this.Controls.Add(this.pneumaticControlUIRotaryCylinder);
            this.Controls.Add(this.buttonExitRelayOn);
            this.Controls.Add(this.buttonEntranceRelayOn);
            this.Controls.Add(this.ledEntranceClear);
            this.Controls.Add(this.ledExitClear);
            this.Controls.Add(this.ledExitSensor);
            this.Controls.Add(this.ledEntranceSensor);
            this.Controls.Add(this.ledZoneSensor);
            this.Controls.Add(this.ledInPosition);
            this.Name = "PanelTurnSection";
            this.Size = new System.Drawing.Size(159, 388);
            this.VisibleChanged += new System.EventHandler(this.Panel_VisibleChanged);
            this.ResumeLayout(false);

		}
		#endregion

		//Assign the TurnTable object to panel UI
		public void AssignTurnSection(ITurnSection newTurnSection)
		{
			myTurnSection = newTurnSection;
			this.labelTurnSectionName.Text = myTurnSection.Name;
			this.pneumaticControlUIRotaryCylinder.AssignActuator(myTurnSection.RotaryCylinder);
		}
		
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(myTurnSection == null || !this.Visible)
			{
				timer1.Enabled = false;
				return;
			}
            ledInPosition.State=myTurnSection.IsInPosition(DigitalIOState.On);
            ledEntranceClear.State = myTurnSection.IsEntranceClear(DigitalIOState.Off);
            ledExitClear.State = myTurnSection.IsExitClear(DigitalIOState.Off);
            //ledEntranceSensor.State = myTurnSection.IsEntrance(DigitalIOState.Off);
            //ledExitSensor.State = myTurnSection.IsExit(DigitalIOState.Off);
            ledZoneSensor.State = myTurnSection.IsZone(DigitalIOState.On);
		}

		private void Panel_VisibleChanged(object sender, System.EventArgs e)
		{
			if(!this.DesignMode)
				timer1.Enabled = this.Visible;
            
		}

        private void pneumaticControlUIRotaryCylinder_Load(object sender, EventArgs e)
        {

        }

        private void buttonEntranceRelayOn_Click(object sender, EventArgs e)
        {
            if (myTurnSection == null)
            {
                MessageBox.Show("Object not assign!");
                return;
            }
            if (!myTurnSection.IsHome()) return;
            if (buttonEntranceRelayOn.Text == "Entrance Relay ON")
            {
                buttonExitRelayOn.Text = "Exit Relay ON";
                myTurnSection.ExitRelay = 0;
                buttonEntranceRelayOn.Text = "Entrance Relay OFF";
                myTurnSection.EntranceRelay = 1;
            }
            else
            {
                buttonEntranceRelayOn.Text = "Entrance Relay ON";
                myTurnSection.EntranceRelay = 0;
            }
        }
        
        private void buttonExitRelayOn_Click(object sender, EventArgs e)
        {
           if (myTurnSection.IsHome()) return;
           if (buttonExitRelayOn.Text == "Exit Relay ON")
            {
                buttonExitRelayOn.Text = "Exit Relay OFF";
                myTurnSection.ExitRelay = 1;
                buttonEntranceRelayOn.Text = "Entrance Relay ON";
                myTurnSection.EntranceRelay = 0;
            }
            else
            {
                buttonExitRelayOn.Text = "Exit Relay ON";
                myTurnSection.ExitRelay = 0;
            }
            
        }

        private void indicatorButtonInhibit_Click(object sender, EventArgs e)
        {
            if (myTurnSection.IsInhibit())
                myTurnSection.InhibitRelay = 1;
            else
                myTurnSection.InhibitRelay = 0;
        }	
    }
}
