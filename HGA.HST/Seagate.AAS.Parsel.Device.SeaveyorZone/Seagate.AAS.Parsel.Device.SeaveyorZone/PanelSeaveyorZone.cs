using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Device.SeaveyorZone
{
    public partial class PanelSeaveyorZone : UserControl
    {
        private ISeaveyorZone myZone = null;
        public PanelSeaveyorZone()
        {
            InitializeComponent();
        }

        //Assign the TurnTable object to panel UI
        public void AssignSeaveyorZone(ISeaveyorZone newZone)
        {
            myZone = newZone;
            this.labelZoneName.Text = myZone.Name;
        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            //indicatorButtonInhibit.State = myZone.Inhibit;
            //ledInPosition.State = myZone.IsInPosition(DigitalIOState.On);

            ledInPosition.State = myZone.IsInpositionSensorOn();
            ledZoneSensor.State = myZone.IsZoneSensorOn();
            indicatorButtonInhibit.State = myZone.IsInhibit();
            indicatorButtonReverse.State = myZone.IsReverse();
        }
        private void Panel_VisibleChanged(object sender, System.EventArgs e)
        {
            if (!this.DesignMode)
                timer1.Enabled = this.Visible;
        }

        private void indicatorButtonReverse_Click(object sender, EventArgs e)
        {
            myZone.Reverse();
        }

        private void indicatorButtonInhibit_Click_1(object sender, EventArgs e)
        {
            if (myZone.IsInhibit())
                myZone.Inhibit(false);
            else
                myZone.Inhibit(true);

        }
    }


}
