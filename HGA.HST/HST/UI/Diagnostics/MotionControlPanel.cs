using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw.Aerotech.A3200;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Settings;
using XyratexOSC.UI;
using Seagate.AAS.Parsel.Hw;//temp test

namespace Seagate.AAS.HGA.HST.UI.Diagnostics
{
    public partial class MotionControlPanel : UserControl
    {
        private UserAccessSettings _userAccess;
        private HSTIOManifest _ioManifest;//temp test
        PanelSingleAxis panelSingleAxis;
        PanelSingleAxis panelSingleAxis1;
        PanelSingleAxis panelSingleAxis2;

        public MotionControlPanel()
        {
            InitializeComponent();

            _userAccess = HSTMachine.Workcell.HSTSettings.getUserAccessSettings();            
            _userAccess.UserChanged += new EventHandler(UserChanged);

            this._ioManifest = (HSTIOManifest)HSTMachine.Workcell.IOManifest;//temp test

            int startLocation = 6;

            panelSingleAxis = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis.Location = new System.Drawing.Point(startLocation, 6);
            panelSingleAxis.Name = "panelSingleAxis0";
            panelSingleAxis.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis.TabIndex = 0;
            this.Controls.Add(panelSingleAxis);
            panelSingleAxis.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)0));

            panelSingleAxis1 = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis1.Location = new System.Drawing.Point(startLocation + 295, 6);
            panelSingleAxis1.Name = "panelSingleAxis1";
            panelSingleAxis1.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis1.TabIndex = 0;
            this.Controls.Add(panelSingleAxis1);
            panelSingleAxis1.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)1));

            panelSingleAxis2 = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis2.Location = new System.Drawing.Point(startLocation + (295 * 2), 6);
            panelSingleAxis2.Name = "panelSingleAxis2";
            panelSingleAxis2.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis2.TabIndex = 0;
            this.Controls.Add(panelSingleAxis2);
            panelSingleAxis2.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)2));

            // second row
            PanelSingleAxis panelSingleAxis3 = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis3.Location = new System.Drawing.Point(startLocation, 6 + 360);
            panelSingleAxis3.Name = "panelSingleAxis3";
            panelSingleAxis3.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis3.TabIndex = 0;
            this.Controls.Add(panelSingleAxis3);
            panelSingleAxis3.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)3));

            PanelSingleAxis panelSingleAxis4 = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis4.Location = new System.Drawing.Point(startLocation + 295 , 6 + 360);
            panelSingleAxis4.Name = "panelSingleAxis3";
            panelSingleAxis4.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis4.TabIndex = 0;
            this.Controls.Add(panelSingleAxis4);
            panelSingleAxis4.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)4));

            PanelSingleAxis panelSingleAxis5 = new Seagate.AAS.Parsel.Hw.Aerotech.A3200.PanelSingleAxis();
            panelSingleAxis5.Location = new System.Drawing.Point(startLocation + (295 * 2), 6 + 360);
            panelSingleAxis5.Name = "panelSingleAxis3";
            panelSingleAxis5.Size = new System.Drawing.Size(290, 360);
            panelSingleAxis5.TabIndex = 0;
            this.Controls.Add(panelSingleAxis5);
            panelSingleAxis5.AssignAxis(HSTMachine.Workcell._a3200HC.IOStore.GetAxis((uint)5));

            labelInstructionandInformation.Text = "The command referring to motion controller command.\nFor Example ENABLE X , DISABLE X, MOVEABS X5 10 and MOVEINC X5 10.\nExample: MOVEABS <Axis> <Position> <Speed>\n<Axis>: Axis Name (X,Y,Z)\n<Position> Absolute postion to move to (-100, 100)\n<Speed> The speed at which to move an axis (50, 100, 200)";
            EnableDisableTernimal();
        }

        public PanelSingleAxis getPrecisorNestXAxis()
        {
            return panelSingleAxis;
        }

        public PanelSingleAxis getPrecisorNestYAxis()
        {
            return panelSingleAxis1;
        }

        public PanelSingleAxis getPrecisorNestThetaAxis()
        {
            return panelSingleAxis2;
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
        }

        private void textBoxCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                buttonExecute.PerformClick();
        }

        private void UserChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {
                    EnableDisableTernimal();
                });
            }
            catch { }
        }

        private void EnableDisableTernimal()
        {
            if (_userAccess != null)
            {
                groupBoxTernimal.Visible = _userAccess.getCurrentUser().Level == UserLevel.Administrator ? true : false;
            }
        }
    }
}
