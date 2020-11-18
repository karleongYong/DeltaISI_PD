using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.UI;
using System.Threading;
using System.Windows.Forms;
using XyratexOSC.Logging;
//using Seagate.AAS.HGA.HST.Machine.Vision;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class Utils
    {
        public void ShowSingleAxisTeachForm(HSTIOManifest.Axes axis, /*TouchscreenNumBox*/TextBox tsnb, IMoveProfile mp, SingleAxisForm.AxisOrientation or, string positionName)
        {
            new Thread(delegate()
            {
                IAxis ax = HSTMachine.Workcell.IOManifest.GetAxis((int)axis);
                mp.Velocity = 100;
                mp.Acceleration = 500;
                mp.Deceleration = 500;
                SingleAxisForm f = new SingleAxisForm();
                f.AssignData(ax.Name, Convert.ToDouble(tsnb.Text), ax, mp, or, positionName);
                //f.TopMost = true;
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tsnb.BeginInvoke((MethodInvoker)delegate
                    {
                        tsnb.Text = f.TeachPosition.ToString();
                    });
                }
            }).Start();
        }

        public void ShowMultipleAxisTeachForm(HSTIOManifest.Axes axisX, HSTIOManifest.Axes axisY, HSTIOManifest.Axes axisTheta, /*TouchscreenNumBox*/TextBox tsnbX, /*TouchscreenNumBox*/TextBox tsnbY, /*TouchscreenNumBox*/TextBox tsnbTheta, IMoveProfile mpX, IMoveProfile mpY, IMoveProfile mpTheta, string positionName)
        {
            Log.Info(this, "Text Box X:{0}, Text Box Y:{1}, Text Box Theta:{2}", tsnbX, tsnbY, tsnbTheta);

            new Thread(delegate()
            {
                IAxis axX = HSTMachine.Workcell.IOManifest.GetAxis((int)axisX);
                IAxis axY = HSTMachine.Workcell.IOManifest.GetAxis((int)axisY);
                IAxis axTheta = HSTMachine.Workcell.IOManifest.GetAxis((int)axisTheta);

                //temp do separate move profile, can join together and use same move profile if wannted later
                mpX.Velocity = 100;
                mpX.Acceleration = 500;
                mpX.Deceleration = 500;

                mpY.Velocity = 20;
                mpY.Acceleration = 100;
                mpY.Deceleration = 100;

                mpTheta.Velocity = 20;
                mpTheta.Acceleration = 100;
                mpTheta.Deceleration = 100;

                MultipleAxisForm f = new MultipleAxisForm();
                f.AssignData(axX, axY, axTheta, Convert.ToDouble(tsnbX.Text), Convert.ToDouble(tsnbY.Text), Convert.ToDouble(tsnbTheta.Text), mpX, mpY, mpTheta, positionName);
                //f.TopMost = true;
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tsnbX.BeginInvoke((MethodInvoker)delegate
                    {
                        tsnbX.Text = f.TeachPositionX.ToString();
                    });

                    tsnbY.BeginInvoke((MethodInvoker)delegate
                    {
                        tsnbY.Text = f.TeachPositionY.ToString();
                    });

                    tsnbX.BeginInvoke((MethodInvoker)delegate
                    {
                        tsnbTheta.Text = f.TeachPositionTheta.ToString();
                    });
                }
            }).Start();
        }
    }
}
