using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelTitle : UserControl
    {
        private HSTWorkcell workcell = null;

        
        public PanelTitle()
        {
            InitializeComponent();            
            this.getHeaderUserAccess().SwitchToOperationPage += SwitchToOperationPage;
        }

        public HeaderUserAccess getHeaderUserAccess()
        {
            return headerUserAccess1;
        }

        public void AssignWorkcell(HSTWorkcell workcell)
        {
            this.workcell = workcell;            
            headerUserAccess1.AssignSettings(workcell.HSTSettings);
            workcell.Process.MonitorIOState.AssignSettings(workcell.HSTSettings); 
        }

        private void SwitchToOperationPage(object sender, EventArgs e)
        {
            HSTMachine.Instance.MainForm.getPanelNavigation().SetPanel("Operation");
        }

        private void PanelTitle_Load(object sender, System.EventArgs e)
        {
            this.Height = 69;
            if (!this.DesignMode && workcell != null)
            {
                SoftwareRevision = workcell.Version;
                string versionToolTip = "Build Version: " + workcell.Version;
                string[] tokens = workcell.Version.Split('.');
                if (tokens.Length == 4)
                {
                    try
                    {
                        DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0);

                        dt = dt.AddDays(Convert.ToInt32(tokens[2]));

                        dt = dt.AddSeconds(2 * Convert.ToInt32(tokens[3]));

                        if (TimeZone.IsDaylightSavingTime(dt, TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year)))
                            dt = dt.AddHours(1);

                        versionToolTip += Environment.NewLine + dt.ToLongDateString() + " " + dt.ToLongTimeString();
                    }
                    catch
                    {

                    }
                }
                toolTip1.SetToolTip(labelVersion, versionToolTip);


                // notify frame that this has been created (will perform initial update)
                if (ServiceManager.Messaging.GetMessageChannel("TitlePanelCreated") != null)
                    ServiceManager.Messaging.GetMessageChannel("TitlePanelCreated").SendMessage(null, null);

                // create message channel for UI updates
                MessageChannel messageChannel = ServiceManager.Messaging.GetMessageChannel("TitleMessage");
                messageChannel.MessageSentEvent += new MessageChannel.ReceiveMessageHandler(messageChannel_MessageSentEvent);       

            }
            }

        // Display the content of TitleMessage received from state machine
        private void messageChannel_MessageSentEvent(object source, Seagate.AAS.Parsel.Services.Message msg)
        {
            if (this.lblState.InvokeRequired)
                lblState.BeginInvoke(new EventHandler(delegate { lblState.Text = msg.Text; }));
            else
                lblState.Text = msg.Text;
        }

		public void ViewChangedHandler(string viewName)
		{
			lblCurrentView.Text = viewName;
		}

        public string CurrentView
        {
            set { lblCurrentView.Text = value; }
        }

        public string SoftwareRevision
        {
            set { labelVersion.Text = value; }
        }
        
		public string RunModeCaption
		{
			set { RunModeLbl.Text = value; }
		}
        
        public string FirmwareVersion
        {
            set { labelFirmware.Text = value; }
        }

        //copy Tom's code
        void config_OnConfigChanged()
        {
            //this.Invoke(new Config.ConfigChangedHandler(UpdateConfigDisplay));
        }

        public void UpdateConfigDisplay()
        {
        }

        bool flashState = false;
        private void timer1_Tick(object sender, EventArgs e)
        {

            // toggle color
            if (flashState)
            {
                //switch (workcell.config.RunMode)
                //{
                //    case RunMode.StandAlone:
                //        this.BackColor = Color.Tomato;
                //        break;
                //    case RunMode.GageRR:
                //        this.BackColor = Color.CornflowerBlue;
                //        break;
                //    case RunMode.Bypass:
                //        this.BackColor = Color.MediumSeaGreen;
                //        break;
                //    case RunMode.DryRun:
                //        this.BackColor = Color.Plum;
                //        break;
                //}

            }
            else
                this.BackColor = Color.FromArgb(51, 102, 153);


            flashState = !flashState;

        }
       
    }
}
