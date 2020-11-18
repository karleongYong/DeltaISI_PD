
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.UI;
using System.Threading;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.Utils
{
    /// <summary>
    /// Represent a teach dialog which will allow the control of single axis
    /// and update the assigned teach point will latest axis current position.
    /// </summary>
    public partial class MultipleAxisForm : Form
    {
        // Nested declarations -------------------------------------------------
        ///// <summary>
        ///// Represent the 4 possible pairs of directional arrows on the panels.
        ///// </summary>
        //public enum AxisOrientation
        //{
        //    LeftRight,
        //    ForwardBackward,
        //    UpDown,
        //    Rotation
        //}

        private class AxisAndMovePackage
        {
            public IAxis Axis;
            public IMoveProfile MoveProfile;
            public bool IsPositiveMove;
        }
        /// <summary>
        /// Occurs after the Move To button is clicked and before the axis performs the absolute move. 
        /// </summary>
        public event CancelEventHandler BeforeMove;
        /// <summary>
        /// Represent the method that will be used to handle the BeforeRelativeMove event.
        /// </summary>
        /// <param name="axisToMove"></param>
        /// <param name="distance"></param>
        /// <param name="ce"></param>
        public delegate void BeforeRelativeMoveEventHandler(IAxis axisToMove, double distance, CancelEventArgs ce);
        /// <summary>
        /// Occurs after the Directional buttons is clicked and before the axis perform the relative move.
        /// </summary>
        public event BeforeRelativeMoveEventHandler BeforeRelativeMove;

        private delegate void SetControlEnableStateDelegate(Control control, bool state);

        // Member variables ----------------------------------------------------
        Thread _moveToThread;
        private bool _simulationX = false;
        private bool _simulationY = false;
        private bool _simulationTheta = false;
        private double _teachPositionX;
        private double _teachPositionY;
        private double _teachPositionTheta;
        private double _actualPositionX;
        private double _actualPositionY;
        private double _actualPositionTheta;
        private double _moveToPositionX;
        private double _moveToPositionY;
        private double _moveToPositionTheta;
        private IAxis _axisX;
        private IAxis _axisY;
        private IAxis _axisTheta;
        private IMoveProfile _axisMoveProfileX;
        private IMoveProfile _axisMoveProfileY;
        private IMoveProfile _axisMoveProfileTheta;
        //private List<IDigitalInput> _digitalInputs;
        //private List<IDigitalOutput> _digitalOutputs;

        // Constructors & Finalizers -------------------------------------------
        /// <summary>
        /// Initialize a new instance of SingleAxisTeachForm.
        /// </summary>
        public MultipleAxisForm()
        {
            InitializeComponent();
            IsEnableTeachButton = true;
            arrowButtonRight.Visible = false;
            arrowButtonLeft.Visible = false;
            arrowButtonForward.Visible = false;
            arrowButtonBackward.Visible = false;
            arrowButtonUp.Visible = false;
            arrowButtonDown.Visible = false;
            arrowButtonCCW.Visible = false;
            arrowButtonCW.Visible = false;

            MaxMoveDistance = 10;
            this.FormClosing += new FormClosingEventHandler(MultipleAxisTeachForm_FormClosing);
        }

        // Properties ----------------------------------------------------------
        /// <summary>
        /// Gets or sets the simulate mode status.
        /// </summary>
        public bool SimulationX { get { return _simulationX; } set { _simulationX = value; } }
        /// <summary>
        /// Gets or sets the simulate mode status.
        /// </summary>
        public bool SimulationY { get { return _simulationY; } set { _simulationY = value; } }
        /// <summary>
        /// Gets or sets the simulate mode status.
        /// </summary>
        public bool SimulationTheta { get { return _simulationTheta; } set { _simulationTheta = value; } }
        /// <summary>
        /// Gets the teach point of this panel. It will contain the update teach point if the Teach button is clicked.
        /// </summary>
        public double TeachPositionX { get { return _teachPositionX; } }
        /// <summary>
        /// Gets the teach point of this panel. It will contain the update teach point if the Teach button is clicked.
        /// </summary>
        public double TeachPositionY { get { return _teachPositionY; } }
        /// <summary>
        /// Gets the teach point of this panel. It will contain the update teach point if the Teach button is clicked.
        /// </summary>
        public double TeachPositionTheta { get { return _teachPositionTheta; } }
        /// <summary>
        /// Gets or sets whether to enable the Teach button.
        /// </summary>
        public bool IsEnableTeachButton { get; set; }
        /// <summary>
        /// Specify the maximum value to be allowed to be entered at the Move Distance box. The default is 10.
        /// </summary>
        public double MaxMoveDistance { get; set; }

        // Methods -------------------------------------------------------------
        public void AssignData(IAxis axisX, IAxis axisY, IAxis axisTheta, double positionX, double positionY, double positionTheta, IMoveProfile MoveProfileX, IMoveProfile MoveProfileY, IMoveProfile MoveProfileTheta, string positionName)
        {
            //if (axis == null)
            //    throw new Exception("Axis cannot be null.");

            labelName.Text = "Precisor Station";
            labelAxesName.Text = positionName;

            _teachPositionX = positionX;
            _teachPositionY = positionY;
            _teachPositionTheta = positionTheta;

            _axisX = axisX;
            _axisY = axisY;
            _axisTheta = axisTheta;

            _axisMoveProfileX = MoveProfileX;
            _axisMoveProfileY = MoveProfileY;
            _axisMoveProfileTheta = MoveProfileTheta;

            textBoxMoveToPositionX.Text = positionX.ToString();
            textBoxMoveToPositionY.Text = positionY.ToString();
            textBoxMoveToPositionTheta.Text = positionTheta.ToString();

            arrowButtonLeft.Visible = true;
            arrowButtonRight.Visible = true;
            arrowButtonRight.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.X), _axisMoveProfileX, true);
            arrowButtonLeft.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.X), _axisMoveProfileX, false);
           
            arrowButtonUp.Visible = true;
            arrowButtonDown.Visible = true;
            arrowButtonUp.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.Y), _axisMoveProfileY, true);
            arrowButtonDown.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.Y), _axisMoveProfileY, false); ;
           
            arrowButtonCCW.Visible = true;
            arrowButtonCW.Visible = true;
            arrowButtonCCW.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.Theta), _axisMoveProfileTheta, false);
            arrowButtonCW.Tag = CreatePackage(HSTMachine.Workcell.IOManifest.GetAxis((int)HSTIOManifest.Axes.Theta), _axisMoveProfileTheta, true);            
        }

        //public void InvertAxisDirection(AxisOrientation axisOrient, bool isInvert)
        //{
        //    switch (axisOrient)
        //    {
        //        case AxisOrientation.LeftRight:
        //            AxisAndMovePackage pkg = (AxisAndMovePackage)arrowButtonRight.Tag;
        //            arrowButtonRight.Tag = CreatePackage(pkg.Axis, pkg.MoveProfile, !isInvert);
        //            arrowButtonLeft.Tag = CreatePackage(pkg.Axis, pkg.MoveProfile, isInvert);
        //            break;
        //        case AxisOrientation.ForwardBackward:
        //            AxisAndMovePackage pkg2 = (AxisAndMovePackage)arrowButtonForward.Tag;
        //            arrowButtonForward.Tag = CreatePackage(pkg2.Axis, pkg2.MoveProfile, !isInvert);
        //            arrowButtonBackward.Tag = CreatePackage(pkg2.Axis, pkg2.MoveProfile, isInvert);
        //            break;
        //        case AxisOrientation.UpDown:
        //            AxisAndMovePackage pkg3 = (AxisAndMovePackage)arrowButtonUp.Tag;
        //            arrowButtonUp.Tag = CreatePackage(pkg3.Axis, pkg3.MoveProfile, !isInvert);
        //            arrowButtonDown.Tag = CreatePackage(pkg3.Axis, pkg3.MoveProfile, isInvert);
        //            break;
        //        case AxisOrientation.Rotation:
        //            AxisAndMovePackage pkg4 = (AxisAndMovePackage)arrowButtonCCW.Tag;
        //            arrowButtonCCW.Tag = CreatePackage(pkg4.Axis, pkg4.MoveProfile, !isInvert);
        //            arrowButtonCW.Tag = CreatePackage(pkg4.Axis, pkg4.MoveProfile, isInvert);
        //            break;
        //    }
        //}

        //public void AssignDigitalInputs(params IDigitalInput[] digitalInputs)
        //{
        //    if (digitalInputs == null)
        //        return;
        //    _digitalInputs = new List<IDigitalInput>();
        //    _digitalInputs.AddRange(digitalInputs);
        //    buttonDIO.Visible = true;
        //}

        //public void AssignDigitalOutputs(params IDigitalOutput[] digitalOutputs)
        //{
        //    if (digitalOutputs == null)
        //        return;
        //    _digitalOutputs = new List<IDigitalOutput>();
        //    _digitalOutputs.AddRange(digitalOutputs);
        //    buttonDIO.Visible = true;
        //}

        // Internal methods ----------------------------------------------------
        private AxisAndMovePackage CreatePackage(IAxis axis, IMoveProfile moveProfile, bool isPositiveMove)
        {
            if (axis != null)
            {
                AxisAndMovePackage pkg = new AxisAndMovePackage();
                pkg.Axis = axis;
                pkg.MoveProfile = moveProfile;
                pkg.IsPositiveMove = isPositiveMove;
                return pkg;
            }
            else
                return null;
        }

        private void AxisJoggingHelper(object sender, MouseEventArgs e, bool isMOuseDown)
        {
            Control con = (Control)sender;
            AxisAndMovePackage pkg = null;
            if (con.Tag != null)
            {
                pkg = (AxisAndMovePackage)con.Tag;
                IAxis axis = pkg.Axis;

                // Reduction of speed
                IMoveProfile mf = new MoveProfileBase();
                mf.Acceleration = pkg.MoveProfile.Acceleration;
                mf.Deceleration = pkg.MoveProfile.Deceleration;
                mf.Velocity = pkg.MoveProfile.Velocity;

                double spdReduction = Convert.ToDouble(touchscreenNumBoxSpeedPercentage.Text);
                mf.Velocity = pkg.MoveProfile.Velocity * spdReduction / 100.0;
                mf.Acceleration = pkg.MoveProfile.Acceleration * spdReduction / 100.0;
                mf.Deceleration = pkg.MoveProfile.Deceleration * spdReduction / 100.0;

                double sign = pkg.IsPositiveMove ? 1 : -1;
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        if (checkBoxIsRelativeMove.Checked)
                        {
                            if (isMOuseDown)
                            {
                                if (BeforeRelativeMove != null)
                                {
                                    CancelEventArgs ce = new CancelEventArgs();
                                    BeforeRelativeMove(axis, sign * Convert.ToDouble(touchscreenNumBoxRelativeMoveDistance.Text), ce);
                                    if (ce.Cancel == true)
                                    {
                                        return;
                                    }
                                }
                                if (axis.IsMoveDone)
                                {
                                    double relDist = Math.Round(Convert.ToDouble(touchscreenNumBoxRelativeMoveDistance.Text), 3);
                                    axis.MoveRelativeStart(mf, relDist * sign);
                                    if (_simulationX)
                                    {
                                        if (sign == 1)
                                            _actualPositionX = _actualPositionX + relDist;
                                        else
                                            _actualPositionX = _actualPositionX - relDist;
                                    }

                                    if (_simulationY)
                                    {
                                        if (sign == 1)
                                            _actualPositionY = _actualPositionY + relDist;
                                        else
                                            _actualPositionY = _actualPositionY - relDist;
                                    }

                                    if (_simulationTheta)
                                    {
                                        if (sign == 1)
                                            _actualPositionTheta = _actualPositionTheta + relDist;
                                        else
                                            _actualPositionTheta = _actualPositionTheta - relDist;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isMOuseDown) // 
                            {
                                if (axis.IsMoveDone)
                                {
                                    axis.MoveRelativeStart(mf, 5000 * sign);
                                    labelStatus.Text = string.Format("{0} is moving...", axis.Name);
                                }
                            }
                            else
                            {
                                axis.Stop();
                                labelStatus.Text = string.Format("{0} is stopped.", axis.Name);
                                labelStatus.Refresh();
                                System.Threading.Thread.Sleep(100);
                                labelStatus.Text = "Status";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        axis.Stop();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private uint GetMoveTimeOutValue(IAxis axis, double targePosition, IMoveProfile moveProfile)
        {
            return 10000;
        }

        private void MoveToHandler()
        {
            try
            {
                if (_simulationX || _simulationY || _simulationTheta)
                {
                    Thread.Sleep(100);
                }

                if (BeforeMove != null)
                {
                    CancelEventArgs ce = new CancelEventArgs();
                    BeforeMove(this, ce);
                    if (ce.Cancel == true)
                    {
                        return;
                    }
                }

                // Move XYT to target

                //temp do not move in vector/parallel
                //temp move individually
                _axisX.MoveAbsoluteStart(_axisMoveProfileX, _moveToPositionX);
                _axisY.MoveAbsoluteStart(_axisMoveProfileY, _moveToPositionY);
                _axisTheta.MoveAbsoluteStart(_axisMoveProfileTheta, _moveToPositionTheta);

                if (_simulationX)
                {
                    _actualPositionX = Convert.ToDouble(textBoxMoveToPositionX.Text);
                }

                if (_simulationY)
                {
                    _actualPositionY = Convert.ToDouble(textBoxMoveToPositionY.Text);
                }

                if (_simulationTheta)
                {
                    _actualPositionTheta = Convert.ToDouble(textBoxMoveToPositionTheta.Text);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (this.IsHandleCreated)
                    this.BeginInvoke(new SetControlEnableStateDelegate(SetControlEnableState), buttonMoveTo, true);
            }
        }

        private void SetControlEnableState(Control control, bool state)
        {
            control.Enabled = state;
        }

        // Event handlers ------------------------------------------------------
        private void GantryTeachForm_Load(object sender, EventArgs e)
        {
            touchscreenNumBoxRelativeMoveDistance.Max = this.MaxMoveDistance;

            // Subscribing arrow buttons events
            foreach (Control item in groupBox1.Controls)
            {
                if (item is ArrowButton || item is Button)
                {
                    Control con = (Control)item;
                    con.MouseDown += new MouseEventHandler(HandleArrowButtonMouseDownEvent);
                    if (con.Tag == null)
                    {
                        // If axis is null
                        con.Enabled = false;
                        if (item is ArrowButton)
                        {
                            Color disabled = Color.Transparent;
                            Color disabled2 = Color.Gainsboro;
                            ((ArrowButton)item).NormalStartColor = disabled;
                            ((ArrowButton)item).NormalEndColor = disabled;
                            ((ArrowButton)item).ForeColor = disabled2;
                        }
                    }
                }
            }
            buttonTeach.Enabled = IsEnableTeachButton;
            timer1.Enabled = true;
            timer1.Interval = 200;
        }

        void MultipleAxisTeachForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void buttonTeach_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirm teach the position?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            _teachPositionX = Convert.ToDouble(touchscreenNumBoxActualPositionX.Text);
            _teachPositionY = Convert.ToDouble(touchscreenNumBoxActualPositionY.Text);
            _teachPositionTheta = Convert.ToDouble(touchscreenNumBoxActualPositionTheta.Text);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //Console.Beep(600, 200);
                timer1.Enabled = false;
                if (!_simulationX)
                {
                    _actualPositionX = Math.Round(_axisX.GetActualPosition(), 3);
                    //Application.DoEvents();
                }

                if (!_simulationY)
                {
                    _actualPositionY = Math.Round(_axisY.GetActualPosition(), 3);
                    //Application.DoEvents();
                }

                if (!_simulationTheta)
                {
                    _actualPositionTheta = Math.Round(_axisTheta.GetActualPosition(), 3);
                    //Application.DoEvents();
                }
                Application.DoEvents();

                touchscreenNumBoxActualPositionX.Text = Math.Round(_actualPositionX, 3).ToString();
                touchscreenNumBoxTeachPositionX.Text = _teachPositionX.ToString();

                touchscreenNumBoxActualPositionY.Text = Math.Round(_actualPositionY, 3).ToString();
                touchscreenNumBoxTeachPositionY.Text = _teachPositionY.ToString();

                touchscreenNumBoxActualPositionTheta.Text = Math.Round(_actualPositionTheta, 3).ToString();
                touchscreenNumBoxTeachPositionTheta.Text = _teachPositionTheta.ToString();

                // Enable status
                ledToggleEnableX.State = _axisX.IsEnabled;
            }
            catch (Exception ex)
            {
                labelStatus.Text = ex.Message;
            }
            timer1.Enabled = true;
        }

        void HandleArrowButtonMouseDownEvent(object sender, MouseEventArgs e)
        {
            AxisJoggingHelper(sender, e, true);
        }

        private void buttonMoveTo_Click(object sender, EventArgs e)
        {
            Thread.Sleep(200); //delay to let all text box value updated.

            if (MessageBox.Show("Move to position?", textBoxMoveToPositionX.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            Thread.Sleep(200); //delay to let all text box value updated.

            _moveToPositionX = Convert.ToDouble(textBoxMoveToPositionX.Text);
            _moveToPositionY = Convert.ToDouble(textBoxMoveToPositionY.Text);
            _moveToPositionTheta = Convert.ToDouble(textBoxMoveToPositionTheta.Text);

            buttonMoveTo.Enabled = false;
            _moveToThread = new Thread(MoveToHandler);
            _moveToThread.IsBackground = true;
            _moveToThread.Start();
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            _axisX.Stop();
            _axisY.Stop();
            _axisTheta.Stop();
        }

        private void ledToggleEnableX_Click(object sender, EventArgs e)
        {
            if (_axisX != null)
            {
                _axisX.Enable(!_axisX.IsEnabled);
            }
        }

        private void MultipleAxisTeachForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            timer1.Enabled = this.Visible;
        }

        private void buttonDIO_Click(object sender, EventArgs e)
        {
            
        }

        void _dioListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            buttonDIO.Enabled = true;
        }

        private void arrowButtonRight_Click(object sender, EventArgs e)
        {

        }
    }
}
