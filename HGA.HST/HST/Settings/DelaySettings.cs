using Seagate.AAS.HGA.HST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using XyratexOSC.UI;
using XyratexOSC.XMath;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class DelaySettings
    {
        //temporary didn't categories into module like input module, precisor module, can do so if required in future.

        [Category("Vacuum Delay Timer")]
        [DisplayName("VacuumOnDelay")]
        [Description("Delay (in miliseconds) after Turn On Vacuum")]
        public int VacuumOnDelay
        {
            get;
            set;
        }

        [Category("Vacuum Delay Timer")]
        [DisplayName("VacuumOffDelay")]
        [Description("Delay (in miliseconds) after Turn Off Vacuum")]
        public int VacuumOffDelay
        {
            get;
            set;
        }

        [Category("Vacuum Delay Timer")]
        [DisplayName("Delay after vacuum Off at Precisor before OutputEE pick part")]
        [Description("Delay (in miliseconds) after Turn Off Vacuum")]
        public int VacuumOffAtPrecisorBeforeOutputEEPick
        {
            get;
            set;
        }

        [Category("Probe Delay Timer")]
        [DisplayName("ProbeMoveUpDelay")]
        [Description("Delay (in miliseconds) before moved probe up after test process done")]
        public int ProbeMoveUpDelay
        {
            get;
            set;
        }

        [Category("Input Turn Table Delay Timer")]
        [DisplayName("Turn Table fully Stop Delay")]
        [Description("Delay (in miliseconds) for waiting the input Turn table to fully stop after turning, before vision process and release boat to input conveyor.")]
        public int InputTurnTableFullyStopDelay
        {
            get;
            set;
        }

        [Category("Output Turn Table Delay Timer")]
        [DisplayName("Turn Table fully Stop Delay")]
        [Description("Delay (in miliseconds) for waiting the output Turn table to fully stop after turning, before we can release boat to output conveyor.")]
        public int OutputTurnTableFullyStopDelay
        {
            get;
            set;
        }

        [Category("Output Turn Table Delay Timer")]
        [DisplayName("Release Boat to Output Conveyor Delay")]
        [Description("Delay (in miliseconds) for releasing boat to output conveyor, before it can turn back to 0 degree.")]
        public int OutputTurnTableReleaseBoatDelay
        {
            get;
            set;
        }

        public DelaySettings()
        {
            VacuumOnDelay = 0;
            VacuumOffDelay = 0;
            ProbeMoveUpDelay = 0;
            OutputTurnTableFullyStopDelay = 1000;
            OutputTurnTableReleaseBoatDelay = 2000;
            InputTurnTableFullyStopDelay = 100;
        }
    }
}
