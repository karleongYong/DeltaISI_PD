
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

namespace Seagate.AAS.HGA.HST.Utils
{
    /// <summary>
    /// Represent a teach dialog which will allow the control of single axis
    /// and update the assigned teach point will latest axis current position.
    /// </summary>
    public partial class SingleAxisForm : Form
    {
        // Nested declarations -------------------------------------------------
        /// <summary>
        /// Represent the 4 possible pairs of directional arrows on the panels.
        /// </summary>
        public enum AxisOrientation
        {
            LeftRight,
            ForwardBackward,
            UpDown,
            Rotation
        }

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
        private bool _simulation = false;
        private double _teachPosition;
        private double _actualPosition;
        private double _moveToPosition;
        private IAxis _axis;
        private IMoveProfile _axisMoveProfile;
        private List<IDigitalInput> _digitalInputs;
        private List<IDigitalOutput> _digitalOutputs;

        // Constructors & Finalizers -------------------------------------------
        /// <summary>
        /// Initialize a new instance of SingleAxisTeachForm.
        /// </summary>
        public SingleAxisForm()
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
            this.FormClosing += new FormClosingEventHandler(SingleAxisTeachForm_FormClosing);
        }

        // Properties ----------------------------------------------------------
        /// <summary>
        /// Gets or sets the simulate mode status.
        /// </summary>
        public bool Simulation { get { return _simulation; } set { _simulation = value; } }
        /// <summary>
        /// Gets the teach point of this panel. It will contain the update teach point if the Teach button is clicked.
        /// </summary>
        public double TeachPosition { get { return _teachPosition; } }
        /// <summary>
        /// Gets or sets whether to enable the Teach button.
        /// </summary>
        public bool IsEnableTeachButton { get; set; }
        /// <summary>
        /// Specify the maximum value to be allowed to be entered at the Move Distance box. The default is 10.
        /// </summary>
        public double MaxMoveDistance { get; set; }

        // Methods -------------------------------------------------------------
        public void AssignData(string name, double position, IAxis axis, IMoveProfile axisMoveProfile, AxisOrientation orientation, string positionName)
        {
            if (axis == null)
                throw new Exception("Axis cannot be null.");

            labelName.Text = name;
            labelAxesName.Text = positionName;//axis.Name;

            _teachPosition = position;
            _axis = axis;
            _axisMoveProfile = axisMoveProfile;

            textBoxMoveToPosition.Text = position.ToString();

            //
            if (orientation == AxisOrientation.LeftRight)
            {
                arrowButtonLeft.Visible = true;
                arrowButtonRight.Visible = true;
                arrowButtonRight.Tag = CreatePackage(axis, axisMoveProfile, true);
                arrowButtonLeft.Tag = CreatePackage(axis, axisMoveProfile, false);
            }
            else if (orientation == AxisOrientation.ForwardBackward)
            {
                arrowButtonForward.Visible = true;
                arrowButtonBackward.Visible = true;
                arrowButtonForward.Tag = CreatePackage(axis, axisMoveProfile, true);
                arrowButtonBackward.Tag = CreatePackage(axis, axisMoveProfile, false);
            }
            else if (orientation == AxisOrientation.UpDown)
            {
                arrowButtonUp.Visible = true;
                arrowButtonDown.Visible = true;
                arrowButtonUp.Tag = CreatePackage(axis, axisMoveProfile, false);
                arrowButtonDown.Tag = CreatePackage(axis, axisMoveProfile, true); ;
            }
            else
            {
                arrowButtonCCW.Visible = true;
                arrowButtonCW.Visible = true;
                arrowButtonCCW.Tag = CreatePackage(axis, axisMoveProfile, false);
                arrowButtonCW.Tag = CreatePackage(axis, axisMoveProfile, true);
            }
        }

        public void InvertAxisDirection(AxisOrientation axisOrient, bool isInvert)
        {
            switch (axisOrient)
            {
                case AxisOrientation.LeftRight:
                    AxisAndMovePackage pkg = (AxisAndMovePackage)arrowButtonRight.Tag;
                    arrowButtonRight.Tag = CreatePackage(pkg.Axis, pkg.MoveProfile, !isInvert);
                    arrowButtonLeft.Tag = CreatePackage(pkg.Axis, pkg.MoveProfile, isInvert);
                    break;
                case AxisOrientation.ForwardBackward:
                    AxisAndMovePackage pkg2 = (AxisAndMovePackage)arrowButtonForward.Tag;
                    arrowButtonForward.Tag = CreatePackage(pkg2.Axis, pkg2.MoveProfile, !isInvert);
                    arrowButtonBackward.Tag = CreatePackage(pkg2.Axis, pkg2.MoveProfile, isInvert);
                    break;
                case AxisOrientation.UpDown:
                    AxisAndMovePackage pkg3 = (AxisAndMovePackage)arrowButtonUp.Tag;
                    arrowButtonUp.Tag = CreatePackage(pkg3.Axis, pkg3.MoveProfile, !isInvert);
                    arrowButtonDown.Tag = CreatePackage(pkg3.Axis, pkg3.MoveProfile, isInvert);
                    break;
                case AxisOrientation.Rotation:
                    AxisAndMovePackage pkg4 = (AxisAndMovePackage)arrowButtonCCW.Tag;
                    arrowButtonCCW.Tag = CreatePackage(pkg4.Axis, pkg4.MoveProfile, !isInvert);
                    arrowButtonCW.Tag = CreatePackage(pkg4.Axis, pkg4.MoveProfile, isInvert);
                    break;
            }
        }

        public void AssignDigitalInputs(params IDigitalInput[] digitalInputs)
        {
            if (digitalInputs == null)
                return;
            _digitalInputs = new List<IDigitalInput>();
            _digitalInputs.AddRange(digitalInputs);
            buttonDIO.Visible = true;
        }

        public void AssignDigitalOutputs(params IDigitalOutput[] digitalOutputs)
        {
            if (digitalOutputs == null)
                return;
            _digitalOutputs = new List<IDigitalOutput>();
            _digitalOutputs.AddRange(digitalOutputs);
            buttonDIO.Visible = true;
        }

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
                                    if (_simulation)
                                    {
                                        if (sign == 1)
                                            _actualPosition = _actualPosition + relDist;
                                        else
                                            _actualPosition = _actualPosition - relDist;
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
                if (_simulation)
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
                _axis.MoveAbsoluteStart(_axisMoveProfile, _moveToPosition);

                if (_simulation)
                {
                    _actualPosition = Convert.ToDouble(textBoxMoveToPosition.Text);
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

        void SingleAxisTeachForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void buttonTeach_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirm teach the position?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            _teachPosition = Convert.ToDouble(touchscreenNumBoxActualPosition.Text);
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
                if (!_simulation)
                {
                    _actualPosition = Math.Round(_axis.GetActualPosition(), 3);
                    Application.DoEvents();
                }
                touchscreenNumBoxActualPosition.Text = Math.Round(_actualPosition, 3).ToString();
                touchscreenNumBoxTeachPosition.Text = _teachPosition.ToString();

                // Enable status
                ledToggleEnableX.State = _axis.IsEnabled;
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
            if (MessageBox.Show("Move to position?", textBoxMoveToPosition.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            _moveToPosition = Convert.ToDouble(textBoxMoveToPosition.Text);
            buttonMoveTo.Enabled = false;
            _moveToThread = new Thread(MoveToHandler);
            _moveToThread.IsBackground = true;
            _moveToThread.Start();
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            _axis.Stop();
        }

        private void ledToggleEnableX_Click(object sender, EventArgs e)
        {
            if (_axis != null)
            {
                _axis.Enable(!_axis.IsEnabled);
            }
        }

        private void SingleAxisTeachForm_VisibleChanged(object sender, EventArgs e)
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
    }
}
