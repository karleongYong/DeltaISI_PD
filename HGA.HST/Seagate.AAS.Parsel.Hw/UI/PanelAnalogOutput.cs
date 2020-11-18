using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace Seagate.AAS.Parsel.Hw
{
    public partial class PanelAnalogOutput : UserControl
    {
        protected IAnalogOutput analogOutput;
        protected string numberFormat = "0.00";
        protected string name = "Analog Output Name";

        public PanelAnalogOutput()
        {
            InitializeComponent();

            timer1.Enabled = false;
            numberAnalog.NumberFormat = numberFormat;
        }

        public void AssignAnalogOutput(IAnalogOutput analogOutput)
        {
            if (analogOutput == null)
            {
                timer1.Enabled = false;
                return;
            }
            this.analogOutput = analogOutput;
            labelUnit.Text = analogOutput.Unit;
            labelName.Text = analogOutput.Name;

            numberLimitMax.Number = analogOutput.LimitMax;
            numberLimitMin.Number = analogOutput.LimitMin;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            int value = trackBar.Value;
            double min = analogOutput.LimitMin;
            double max = analogOutput.LimitMax;

            double outputValue = (1.0 * value / trackBar.Maximum) * (max - min) + min;
            analogOutput.Set(outputValue);

        }

        public string NumberFormat
        {
            get { return numberFormat; }
            set 
            {
                numberFormat = value;
                numberAnalog.NumberFormat = numberFormat;
                numberLimitMax.NumberFormat = numberFormat;
                numberLimitMin.NumberFormat = numberFormat;
            }
        }

        public string AnalogOutputName
        {
            get { return name; }
            set
            {
                name = value;
                labelName.Text = name;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.Visible || this.DesignMode || analogOutput==null)
            {
                timer1.Enabled = false;
                return;
            }

            try
            {
                numberAnalog.Number = analogOutput.Value;

                numberLimitMax.Number = analogOutput.LimitMax;
                numberLimitMin.Number = analogOutput.LimitMin;

            }
            catch
            {

            }
        }

        private void PanelAnalogOutput_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.DesignMode)
                timer1.Enabled = this.Visible;
        }
    }
}
