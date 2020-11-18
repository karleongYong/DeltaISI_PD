using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Hw;
using System.Runtime.InteropServices;

using Aerotech.A3200;
using Aerotech.A3200.Status;
using Aerotech.A3200.Commands;
using Aerotech.A3200.Configuration;
using Aerotech.A3200.DataCollection;
using Aerotech.A3200.Exceptions;
using Aerotech.A3200.Information;
using Aerotech.A3200.Parameters;
using Aerotech.A3200.Tasks;

using Aerotech.Common;
using Aerotech.Common.Collections;

using Aerotech.AeroBasic;


namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public class A3200HC : IHardwareComponent,
        IProviderAnalogInput,
        IProviderAnalogOutput,
        IProviderDigitalInput,
        IProviderDigitalOutput,
        IProviderAxis
    {
        // Nested declarations -------------------------------------------------
        private class IOCommReqList : WaitList
        {
            public ArrayList GetList()
            {
                return base.list;
            }
        }

        public enum PerformanceMetric
        {
            GetDI = 0,
            SetDO,
            Move,
            GetPos,
            DiagPacket,
            Count
        }


        // Member variables ----------------------------------------------------
        private IOStore _ioStore;
        private Thread _threadIOExec;
        private IOCommReqList _ioRequestList = new IOCommReqList();

        internal ManualResetEvent eventStopIOExecThread = new ManualResetEvent(true);
        internal ManualResetEvent eventA3200UpdatedRequest = new ManualResetEvent(false);

        private bool initialized = false;
        private string name            = "Aerotech A3200";
        internal bool simulation        = false;
        internal int commandRetries     = 0;
        internal int commErrors         = 0;

        public delegate void InitializationHandler();
        public event InitializationHandler OnInitialize;

        private Controller controller;
        private ControllerInformation controllerInfo;

        StopwatchW32HiRes[] timer = new StopwatchW32HiRes[(int)PerformanceMetric.Count];
        public double[] TotalTime = new double[(int)PerformanceMetric.Count];
        public int[] Transations = new int[(int)PerformanceMetric.Count];
        public StopwatchW32HiRes timerMain = new StopwatchW32HiRes();

        private ControllerDiagPacket diagPacketLatest;
        int packetCount = 0;
        DateTime startTime = DateTime.Now;
        bool needUpdatePackage = false;


        // Constructors & Finalizers -------------------------------------------
        public A3200HC()
        {
            _ioStore = new IOStore(this);

            for (int i = 0; i < (int)PerformanceMetric.Count; i++)
            {
                timer[i] = new StopwatchW32HiRes();
            }
            timerMain.Start();
        }


        // Properties ----------------------------------------------------------
        public IOStore IOStore
        {
            get { return _ioStore; }
        }

        // Methods -------------------------------------------------------------

        internal void InitializeNetwork()
        {
            if (simulation)
                initialized = true;

            if (initialized)
                return;

            try
            {
                // Connect to A3200 controller.
                controller = Controller.Connect();
                initialized = true;
            }
            catch (Exception ex)
            {   // if an exception occurs, indicate it in the status bar
                throw new HardwareException("Error initializing Aerotech A3200", ex);
            }
        }

        internal void ShutDownNetwork()
        {
            if (simulation)
                return;

            if (controller == null)
                return;

            try
            {
                // Disconnect to A3200 controller.
                Controller.Disconnect();
                controller.Dispose();
                controller = null;
                initialized = false;
            }
            catch (Exception ex)
            {
                throw new HardwareException("Error shutdown Aerotech A3200", ex);
            }
        }

        public void ResetCommandRetries()
        {
            commandRetries = 0;

            for (int i = 0; i < (int)PerformanceMetric.Count; i++)
            {
                TotalTime[i] = 0;
                Transations[i] = 0;
            }
            timerMain.Reset();
            timerMain.Start();
        }

        public void DiagHandler(object sender, NewDiagPacketArrivedEventArgs diagPacket)
        {
            if (needUpdatePackage)
                needUpdatePackage = false;

            eventA3200UpdatedRequest.Set();
        }

        private void UpdateDiagnosticInfo()
        {
            if (this.simulation)
                return;

            diagPacketLatest = controller.DataCollection.RetrieveDiagnostics();
        }

        public void ExecuteCommand(string code)
        {
            controller.Commands.Execute(code);
        }

        #region IHardwareComponent ---------------------------------------------
        public void EMOReset()
        {
            ShutDown();
            InitializeNetwork();
            SpawnIOExecThread();
        }

        public void Initialize(bool simulation)
        {
            this.simulation = simulation;
            InitializeNetwork();
            SpawnIOExecThread();

            if (OnInitialize != null)
            {
                OnInitialize();
            }

            if (_ioStore.registrationErrors != "")
            {
                throw new Exception(_ioStore.registrationErrors);
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public void ShutDown()
        {
            KillIOExecThread();
            ShutDownNetwork();
        }

        public bool Simulation
        {
            get { return simulation; }
        }
        #endregion

        #region IProviderAnalogInput -------------------------------------------
        public double GetRawValue(IAnalogInput analogIn)
        {
            lock (this)
            {
                AnalogInput ioAnalogIn = analogIn as AnalogInput;
                if (ioAnalogIn == null)
                    throw new Exception("Invalid Analog Input object");

                if (simulation)
                    return ioAnalogIn.Value;

                return controller.Commands.IO.AnalogInput(ioAnalogIn.PointId, ioAnalogIn.NodeId);
            }
        }

        public void WaitForAboveThreshold(IAnalogInput analogIn, double threshold, int timeOut)
        {
            lock (this)
            {
                AnalogInput ioAnalogIn = analogIn as AnalogInput;
                ioAnalogIn.timeOut = (int)timeOut;
                ioAnalogIn.Threshold = threshold;
                ioAnalogIn.Criteria = AnalogInput.WaitCriteria.Above;
                ScheduleIORequest(ioAnalogIn);
            }
        }

        public void WaitForBelowThreshold(IAnalogInput analogIn, double threshold, int timeOut)
        {
            lock (this)
            {
                AnalogInput ioAnalogIn = analogIn as AnalogInput;
                ioAnalogIn.timeOut = (int)timeOut;
                ioAnalogIn.Threshold = threshold;
                ioAnalogIn.Criteria = AnalogInput.WaitCriteria.Below;
                ScheduleIORequest(ioAnalogIn);
            }
        }

        public void WaitForInRange(IAnalogInput analogIn, double target, double tolerance, int timeOut)
        {
            lock (this)
            {
                AnalogInput ioAnalogIn = analogIn as AnalogInput;
                ioAnalogIn.timeOut = (int)timeOut;
                ioAnalogIn.Threshold = target;
                ioAnalogIn.Tolerance = tolerance;
                ioAnalogIn.Criteria = AnalogInput.WaitCriteria.InRange;
                ScheduleIORequest(ioAnalogIn);
            }
        }
        #endregion

        #region IProviderAnalogOutput ------------------------------------------
        public double GetRawValue(IAnalogOutput analogOut)
        {
            throw new NotImplementedException();
        }

        public void SetRawValue(IAnalogOutput analogOut, double ioValue)
        {
            lock (this)
            {
                AnalogOutput ioAnalogOut = analogOut as AnalogOutput;
                if (ioAnalogOut == null)
                    throw new Exception("Invalid Analog Output object");

                if (simulation)
                    return;

                controller.Commands.IO.AnalogOutput(ioAnalogOut.PointId, ioAnalogOut.NodeId, ioValue);
            }
        }
        #endregion

        #region IProviderDigitalInput ------------------------------------------
        public DigitalIOState GetState(IDigitalInput digIn)
        {
            lock (this)
            {
                DigitalIOState ioState;

                timer[(int)PerformanceMetric.GetDI].Reset();
                timer[(int)PerformanceMetric.GetDI].Start();

                DigitalInput ioDigIn = digIn as DigitalInput;
                if (ioDigIn == null)
                    throw new Exception("Invalid IO point.");

                if (simulation)
                    ioState = ioDigIn.RequestedState;
                else
                {
                    if (0 == controller.Commands.IO.DigitalInputBit(ioDigIn.PointId, ioDigIn.NodeId))
                        ioState = DigitalIOState.Off;
                    else
                        ioState = DigitalIOState.On;

                }
                timer[(int)PerformanceMetric.GetDI].Stop();
                Transations[(int)PerformanceMetric.GetDI] += 1;
                TotalTime[(int)PerformanceMetric.GetDI] += timer[(int)PerformanceMetric.GetDI].ElapsedTime_sec;

                return ioState;
            }
        }

        public bool GetDigitalInputBit(int Axis, int Bit)
        {
            lock (this)
            {
                if (simulation)
                    return true;

                int state;
                state = controller.Commands.IO.DigitalInputBit(Bit, Axis);
                if (state > 0)
                    return true;
                else
                    return false;
            }
        }

        public void StartInputWait(IDigitalInput digIn, DigitalIOState state, uint timeOut)
        {
            lock (this)
            {
                DigitalInput ioDigIn = digIn as DigitalInput;
                ioDigIn.timeOut = (int)timeOut;
                ioDigIn.RequestedState = state;
                ScheduleIORequest(ioDigIn);
            }
        }
        #endregion

        #region IProviderDigitalOutput -----------------------------------------
        public DigitalIOState GetState(IDigitalOutput digOut)
        {
            lock (this)
            {
                DigitalOutput ioDigOut = digOut as DigitalOutput;
                if (ioDigOut == null)
                    throw new Exception("Invalid Digital Output object");

                if (simulation)
                    return ioDigOut.State;

                IONode node = (IONode)_ioStore._nodesMap[ioDigOut.NodeId];
                int state = 0;

                int mask = 1 << ((int)ioDigOut.PointId);
                state = diagPacketLatest[(int)ioDigOut.NodeId].DigitalOutputs & mask;

                if (state > 0)
                {
                    return DigitalIOState.On;
                }
                else
                {
                    return DigitalIOState.Off;
                }
            }
        }

        /// <summary>
        /// Get the digital output state.
        /// </summary>
        /// <param name="axis">The digital output axis on which to get the value.</param>
        /// <param name="bit">The digital output bit on which to get the value.</param>
        /// <returns>Boolean</returns>
        public bool GetDigitalOutputBit(IAxis axis, int bit)
        {
            if (simulation)
            {
                return false;
            }
            Axis ioAxis = axis as Axis;
            if (ioAxis == null)
                throw new Exception("Invalid Axis object.");

            int state = 0;

            AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];
            int result = axisDiagPacket.DigitalOutputs;
            int mask = 1 << bit;
            state = result & mask;

            if (state > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetState(IDigitalOutput digOut, DigitalIOState state)
        {
            lock (this)
            {
                timer[(int)PerformanceMetric.SetDO].Reset();
                timer[(int)PerformanceMetric.SetDO].Start();

                DigitalOutput ioDigOut = digOut as DigitalOutput;
                if (ioDigOut == null)
                    throw new Exception("Invalid Digital Output object");

                if (simulation)
                {
                    ioDigOut.State = state;
                    return;
                }

                int iState = 0;
                iState = (state == DigitalIOState.Off) ? 0 : 1;

                
                controller.Commands.IO.DigitalOutputBit(ioDigOut.PointId, ioDigOut.NodeId, iState);

                timer[(int)PerformanceMetric.SetDO].Stop();
                Transations[(int)PerformanceMetric.SetDO] += 1;
                TotalTime[(int)PerformanceMetric.SetDO] += timer[(int)PerformanceMetric.SetDO].ElapsedTime_sec;
            }
        }

        public void SetDigitalOutputBit(int Axis, int Bit, bool Value)
        {
            lock (this)
            {
                if (simulation)
                    return;

                if (Value)
                {
                    controller.Commands.IO.DigitalOutputBit(Bit, Axis, 1);
                }
                else
                {
                    controller.Commands.IO.DigitalOutputBit(Bit, Axis, 0);
                }
            }
        }
        #endregion

        #region IProviderAxis --------------------------------------------------
        public void Enable(IAxis axis, bool enable)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                    
                try
                {
                    if (enable)
                    {
                        AxisFault axisFault = diagPacketLatest[(int)ioAxis.PointId].AxisFault;
                        if (axisFault.None)
                        {
                            controller.Commands.Motion.Enable(ioAxis.PointId);
                        }
                        else if (axisFault.CwEndOfTravelLimitFault || axisFault.CwSoftwareLimitFault || axisFault.CcwEndOfTravelLimitFault || axisFault.CcwSoftwareLimitFault)
                        {
                            controller.Commands.Motion.Enable(ioAxis.NodeId);
                            controller.Commands.Motion.FaultAck(ioAxis.PointId);
                        }
                        else
                        {
                            controller.Commands.Motion.Abort(ioAxis.PointId);
                            controller.Commands.Motion.FaultAck(ioAxis.PointId);
                        }
                    }
                    else
                    {
                        controller.Commands.Motion.Abort(ioAxis.PointId);
                        controller.Commands.Motion.Disable(ioAxis.PointId);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public double GetActualPosition(IAxis axis)
        {
            timer[(int)PerformanceMetric.GetPos].Reset();
            timer[(int)PerformanceMetric.GetPos].Start();

            Axis ioAxis = axis as Axis;
            if (ioAxis == null)
                throw new Exception("Invalid Axis object.");

            if (simulation)
                return 0.0;

            double position = diagPacketLatest[ioAxis.PointId].PositionFeedback;

            timer[(int)PerformanceMetric.GetPos].Stop();
            Transations[(int)PerformanceMetric.GetPos] += 1;
            TotalTime[(int)PerformanceMetric.GetPos] += timer[(int)PerformanceMetric.GetPos].ElapsedTime_sec;

            return position;
        }

        public double GetCommandPosition(IAxis axis)
        {
            Axis ioAxis = axis as Axis;
            if (ioAxis == null)
                throw new Exception("Invalid Axis object.");

            if (simulation)
                return 0.0;

            return diagPacketLatest[ioAxis.PointId].PositionCommand;
        }

        public void Home(IAxis axis)
        {
            lock (this)
            {

                if (simulation)
                    return;

                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                try
                {
                    controller.Commands.Motion.Home(ioAxis.PointId);
                }
                catch (Exception ex)
                {
                    throw new HardwareException(string.Format("Failed to home axis [{0}]", axis.Name), ex);
                }
            }
        }

        public void HomeAsync(IAxis axis)
        {
            lock (this)
            {

                if (simulation)
                    return;

                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                try
                {
                    controller.Commands.Motion.Advanced.HomeAsync(ioAxis.PointId);
                }
                catch (Exception ex)
                {
                    throw new HardwareException(string.Format("Failed to home axis [{0}]", axis.Name), ex);
                }
            }
        }

        public bool IsEnabled(IAxis axis)
        {
            lock (this)
            {
                timer[(int)PerformanceMetric.GetPos].Reset();
                timer[(int)PerformanceMetric.GetPos].Start();

                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                Axis ioAxis = axis as Axis;

                bool isEnabled = diagPacketLatest[ioAxis.PointId].DriveStatus.Enabled;


                timer[(int)PerformanceMetric.GetPos].Stop();
                Transations[(int)PerformanceMetric.GetPos] += 1;
                TotalTime[(int)PerformanceMetric.GetPos] += timer[(int)PerformanceMetric.GetPos].ElapsedTime_sec;

                return isEnabled;
            }
        }

        public bool IsHomed(IAxis axis)
        {
            lock (this)
            {
                timer[(int)PerformanceMetric.GetPos].Reset();
                timer[(int)PerformanceMetric.GetPos].Start();

                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                Axis ioAxis = axis as Axis;

                bool isHomed = diagPacketLatest[ioAxis.PointId].AxisStatus.Homed;

                timer[(int)PerformanceMetric.GetPos].Stop();
                Transations[(int)PerformanceMetric.GetPos] += 1;
                TotalTime[(int)PerformanceMetric.GetPos] += timer[(int)PerformanceMetric.GetPos].ElapsedTime_sec;

                return isHomed;
            }
        }

        public bool IsMoveDone(IAxis axis)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                Axis ioAxis = axis as Axis;
                //bool inPosition = diagPacketLatest[ioAxis.PointId].DriveStatus.InPosition;
                bool moveActive = diagPacketLatest[ioAxis.PointId].AxisStatus.MoveDone;

                return moveActive;// && inPosition;
            }
        }

        public void Move(IAxis axis, double acceleration, double velocity, double position)
        {
            lock (this)
            {


                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    controller.Commands.Motion.Setup.RampRate(ioAxis.PointId, acceleration);
                    controller.Commands.Motion.MoveAbs(ioAxis.PointId, position, velocity);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public void Move(IAxis axis, double position)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    double acceleration = GetMoveProfile(axis).Acceleration;
                    double velocity = GetMoveProfile(axis).Velocity;

                    controller.Commands.Motion.Setup.RampRate(ioAxis.PointId, acceleration);
                    controller.Commands.Motion.MoveAbs(ioAxis.PointId, position, velocity);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public void MoveRel(IAxis axis, double acceleration, double velocity, double positionRelative)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    controller.Commands.Motion.Setup.RampRate(ioAxis.PointId, acceleration);
                    controller.Commands.Motion.MoveInc(ioAxis.PointId, positionRelative, velocity);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public void MoveRel(IAxis axis, double positionRelative)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    double acceleration = GetMoveProfile(axis).Acceleration;
                    double velocity = GetMoveProfile(axis).Velocity;

                    controller.Commands.Motion.Setup.RampRate(ioAxis.PointId, acceleration);
                    controller.Commands.Motion.MoveInc(ioAxis.PointId, positionRelative, velocity);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public void StartWaitMoveDone(IAxis axis, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.MoveDone;
                //needUpdatePackage = true;
                //while (needUpdatePackage)
                //{
                //    Thread.Sleep(10);
                //}

                ScheduleIORequest(ioAxis);
            }
        }

        public void Stop(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    controller.Commands.Motion.Abort(ioAxis.PointId);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }
        #endregion

        #region Axis -----------------------------------------------------------
        public bool IsMoving(IAxis axis)
        {
            lock (this)
            {
                timer[(int)PerformanceMetric.GetPos].Reset();
                timer[(int)PerformanceMetric.GetPos].Start();

                if (simulation)
                    return true;

                Axis ioAxis = axis as Axis;
                AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];
                bool isMoving = axisDiagPacket.DriveStatus.MoveActive;

                timer[(int)PerformanceMetric.GetPos].Stop();
                Transations[(int)PerformanceMetric.GetPos] += 1;
                TotalTime[(int)PerformanceMetric.GetPos] += timer[(int)PerformanceMetric.GetPos].ElapsedTime_sec;

                return isMoving;
            }
        }

        public bool IsInPosition(IAxis axis)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                Axis ioAxis = axis as Axis;
                AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];
                bool inposition = axisDiagPacket.DriveStatus.InPosition;

                return inposition;
            }
        }

        public bool IsAxisFault(IAxis axis)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return false;
                }
                Axis ioAxis = axis as Axis;
                AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];
                bool isAxisFault = !axisDiagPacket.AxisFault.None;

                return isAxisFault;
            }
        }

        public bool IsHomeDone(IAxis axis)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }

                Axis ioAxis = axis as Axis;
                AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];
                bool isHoming = axisDiagPacket.AxisStatus.Homing;
                bool isHomed = axisDiagPacket.AxisStatus.Homed;

                return !isHoming && isHomed;
            }
        }

        public double GetPeakCurrent(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0;

                return controller.Commands.Status.AxisStatus(ioAxis.PointId, AxisStatusSignal.PeakCurrent);
            }
        }

        public double GetCountsPerUnit(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return controller.Parameters.Axes[ioAxis.PointId].Units.CountsPerUnit.Value;
            }
        }

        public bool WasMoveAborted(IAxis axis)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return false;
                }
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                AxisDiagPacket axisDiagPacket = diagPacketLatest[ioAxis.PointId];

                bool wasAbort = false;
                wasAbort |= !axisDiagPacket.AxisFault.None;
                return wasAbort;
            }
        }

        public bool WasMoveAborted(IAxisGroup axes)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return false;
                }
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                bool wasAbort = false;
                foreach (int nodeId in Axes.AxisNoSet)
                {
                    AxisDiagPacket axisDiagPacket = diagPacketLatest[nodeId];
                    wasAbort |= !axisDiagPacket.AxisFault.None;
                }

                return wasAbort;
            }
        }

        /// <summary>
        ///  FREERUN command to start continuous axis motion at the specified speed. The arithmetic sign of Velocity determines the direction.
        /// </summary>
        /// <param name="axis">The axis (or axes) on which to execute the command.</param>
        /// <param name="velocity">You can specify a velocity of zero, in which case no motion occurs, but the axis is still considered to be moving. A negative value for the Velocity argument reverses the direction of motion.</param>
        public void FreeRun(IAxis axis, double velocity)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    controller.Commands.Motion.FreeRun(ioAxis.PointId, velocity);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        public void FreeRunStop(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;
                try
                {
                    timer[(int)PerformanceMetric.Move].Reset();
                    timer[(int)PerformanceMetric.Move].Start();

                    controller.Commands.Motion.FreeRunStop(ioAxis.PointId);

                    timer[(int)PerformanceMetric.Move].Stop();
                    Transations[(int)PerformanceMetric.Move] += 1;
                    TotalTime[(int)PerformanceMetric.Move] += timer[(int)PerformanceMetric.Move].ElapsedTime_sec;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }
            }
        }

        /// <summary>
        /// Blocks calling thread until axis move is complete or timeout occurs
        /// </summary>
        /// <param name="timeOut"></param>
        /// 
        public void StartWaitMoveStart(IAxis axis, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.MoveStart;

                ScheduleIORequest(ioAxis);
            }
        }

        public void WaitMoveDone(IAxis axis, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Commands.Axes[new int[] { ioAxis.NodeId }].Motion.WaitForMotionDone(WaitOption.InPosition, (int)timeOut);
            }
        }

        public void StartWaitInPosition(IAxis axis, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.InPostion;

                ScheduleIORequest(ioAxis);
            }
        }

        public void StartWaitHomeDone(IAxis axis, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.HomeDone;
                ScheduleIORequest(ioAxis);
            }
        }

        public void StartWaitPositionAboveThreshold(IAxis axis, double threshold, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.PositionAbove;
                ioAxis.PositionThreshold = threshold;
                ScheduleIORequest(ioAxis);
            }
        }

        public void StartWaitPositionBelowThreshold(IAxis axis, double threshold, uint timeOut)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                ioAxis.timeOut = (int)timeOut;
                ioAxis.Criteria = Axis.WaitCriteria.PositionBelow;
                ioAxis.PositionThreshold = threshold;
                ScheduleIORequest(ioAxis);
            }
        }

        public void FaultAchnowledge(IAxis axis)
        {
            lock (this)
            {
                if (simulation) return;

                if (axis == null)
                    throw new Exception("Invalid Axes object.");

                controller.Commands.Motion.FaultAck(((Axis)axis).PointId);
            }
        }

        public void FaultAchnowledge(IAxisGroup axes)
        {
            lock (this)
            {
                if (simulation) return;

                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                AxisGroup Axes = axes as AxisGroup;
                controller.Commands.Motion.FaultAck(Axes.AxisNoSet);

            }
        }
        #endregion

        #region IAxisGroup

        /// <summary>
        /// Move Axes group to absolute position
        /// </summary>
        /// <param name="axes"></param>
        /// <param name="position"></param>
        /// <param name="msecTimeOut"></param>
        public void Move(IAxisGroup axes, double[] absolutePosition, uint msecTimeOut)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                if (simulation)
                    return;

                try
                {
                    double velocity = GetMoveProfile(axes).Velocity;
                    double accel = GetMoveProfile(axes).Acceleration;

                    //controller.Commands.Motion.WaitMode(WaitType.NoWait);
                    //controller.Commands.Motion.Setup.Absolute();
                    //controller.Commands.Motion.Setup.Plane(Axes.Plane);
                    //controller.Commands.Motion.Setup.RampRate(Axes.NodeNameSet, accel);
                    controller.Commands.Axes[Axes.AxisNoSet].Motion.Linear(absolutePosition, velocity);
                    controller.Commands.Axes[Axes.AxisNoSet].Motion.WaitForMotionDone(WaitOption.InPosition, (int)msecTimeOut);
                    //controller.Commands.Axes[Axes.NodeNameSet].Motion.Linear(absolutePosition);
                    //controller.Commands.Axes[Axes.NodeNameSet].Motion.WaitForMotionDone(WaitOption.MoveDone, (int)msecTimeOut);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axes.Name, ex.Message), ex);
                }
            }
        }

        public void StartWaitMoveDone(IAxisGroup axes, uint timeOut)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                Axes.timeOut = (int)timeOut;
                Axes.Criteria = AxisGroup.WaitCriteria.MoveDone;
                //needUpdatePackage = true;
                //while (needUpdatePackage)
                //{
                //    Thread.Sleep(10);
                //}

                ScheduleIORequest(Axes);
            }
        }

        public void StartWaitInPosition(IAxisGroup axes, uint timeOut)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                Axes.timeOut = (int)timeOut;
                Axes.Criteria = AxisGroup.WaitCriteria.MoveDone;
                //needUpdatePackage = true;
                //while (needUpdatePackage)
                //{
                //    Thread.Sleep(10);
                //}

                ScheduleIORequest(Axes);
            }
        }

        public bool IsInPosition(IAxisGroup axes, double[] absolutePosition)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");


                int i = 0;
                bool inPosition = true;
                foreach (int nodeId in Axes.AxisNoSet)
                {
                    AxisDiagPacket axisDiagPacket = diagPacketLatest[nodeId];
                    inPosition &= axisDiagPacket.DriveStatus.Enabled;
                    inPosition &= axisDiagPacket.DriveStatus.InPosition;
                    inPosition &= (axisDiagPacket.PositionCommand == absolutePosition[i]);

                    if (!inPosition)
                        break;

                    i++;
                }

                return inPosition;
            }
        }

        public bool IsInPosition(IAxisGroup axes)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                bool inPosition = true;
                foreach (int nodeId in Axes.AxisNoSet)
                {
                    AxisDiagPacket axisDiagPacket = diagPacketLatest[nodeId];
                    inPosition &= !axisDiagPacket.DriveStatus.MoveActive;
                    inPosition &= axisDiagPacket.DriveStatus.InPosition;

                    if (!inPosition)
                        break;
                }

                return inPosition;
            }
        }

        public bool IsMoveDone(IAxisGroup axes)
        {
            lock (this)
            {
                if (simulation)
                {
                    Thread.Sleep(100);
                    return true;
                }
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                bool isMoveDone = true;
                foreach (int nodeId in Axes.AxisNoSet)
                {
                    AxisDiagPacket axisDiagPacket = diagPacketLatest[nodeId];
                    isMoveDone &= !axisDiagPacket.DriveStatus.MoveActive;

                    if (!isMoveDone)
                        break;
                }

                return isMoveDone;
            }
        }

        public void WaitMoveDone(IAxisGroup axes, uint msecTimeOut)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                try
                {
                    controller.Commands.Axes[Axes.AxisNoSet].Motion.WaitForMotionDone(WaitOption.InPosition, (int)msecTimeOut);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axes.Name, ex.Message), ex);
                }
            }

        }

        public void MoveStart(IAxisGroup axes, double[] absolutePosition)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");

                if (simulation)
                    return;


                try
                {
                    if (IsInPosition(axes, absolutePosition))
                    {
                        return;
                    }

                    //ControllerDiagPacket diagPacket = controller.DataCollection.RetrieveDiagnostics();

                    //foreach (int nodeId in Axes.AxisNoSet)
                    //{
                    //    //if (!diagPacket[nodeName].AxisStatus.Enabled)
                    //    if (!diagPacketLatest[nodeId].AxisStatus.Enabling)
                    //        throw new Exception(string.Format("axis no. {0} is not enable.", nodeId));
                    //}

                    double velocity = GetMoveProfile(axes).Velocity;
                    //double accel = GetMoveProfile(axes).Acceleration;
                    //controller.Commands.Motion.WaitMode(WaitType.NoWait);
                    controller.Commands.Motion.Setup.Absolute();
                    //controller.Commands.Motion.Setup.Plane(Axes.Plane);
                    //controller.Commands.Motion.Setup.RampRate(Axes.NodeNameSet, accel);
                    controller.Commands.Axes[Axes.AxisNoSet].Motion.Linear(absolutePosition, velocity);
                    //controller.Commands.Axes[Axes.NodeNameSet].Motion.Linear(absolutePosition);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axes.Name, ex.Message), ex);
                }
            }
        }

        public void Stop(IAxisGroup axes)
        {
            lock (this)
            {
                AxisGroup Axes = axes as AxisGroup;
                if (axes == null)
                    throw new Exception("Invalid Axes object.");
                try
                {
                    controller.Commands.Motion.Abort(Axes.AxisNoSet);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axes.Name, ex.Message), ex);
                }
            }
        }

        //public void WaitMoveDone(IAxisGroup axes, uint timeOut)
        //{
        //    AxisGroup Axes = axes as AxisGroup;

        //    foreach (string axisNodeName in Axes.NodeNameSet)
        //    {
        //        Axis ioAxis = _ioStore.GetAxis(axisNodeName, 0) as Axis;
        //        if (ioAxis == null)
        //            throw new Exception("Invalid Axis object.");

        //        ioAxis.timeOut = (int)timeOut;
        //        ioAxis.Criteria = Axis.WaitCriteria.MoveDone;
        //        ScheduleIORequest(ioAxis);
        //    }


        //    //Axis ioAxis = axis as Axis;
        //}

        #endregion

        #region MoveProfile

        private Hashtable _moveProfileMap = new Hashtable();

        public IMoveProfile GetMoveProfile(IAxis axis)
        {
            lock (this)
            {
                MoveProfile moveProfile = new MoveProfile();

                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return moveProfile as IMoveProfile;

                if (_moveProfileMap.Contains(ioAxis.NodeId))
                    moveProfile = (MoveProfile)_moveProfileMap[ioAxis.NodeId];
                else
                    _moveProfileMap.Add(ioAxis.NodeId, moveProfile);

                return moveProfile as IMoveProfile;
            }
        }

        public void SetMoveProfile(IAxis axis, IMoveProfile moveProfile)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                //MoveProfile newMoveProfile = moveProfile as MoveProfile;
                MoveProfile newMoveProfile = new MoveProfile(moveProfile.Acceleration, moveProfile.Velocity, moveProfile.SettlingWindow, (double)moveProfile.SettlingTime);

                try
                {
                    double defaultAccel = (double)controller.Parameters.Axes[ioAxis.NodeId].Motion.DefaultRampRate.Value;
                    double defaultSpeed = (double)controller.Parameters.Axes[ioAxis.NodeId].Motion.DefaultSpeed.Value;
                    //double defaultAccel = 1000;
                    //double defaultSpeed = 100;

                    double percentAccel;
                    double percentSpeed;
                    if (moveProfile.Acceleration > 100)
                        percentAccel = 100;
                    else if (moveProfile.Acceleration <= 0)
                        percentAccel = 1;
                    else
                        percentAccel = moveProfile.Acceleration;

                    if (moveProfile.Velocity > 100)
                        percentSpeed = 100;
                    else if (moveProfile.Velocity <= 0)
                        percentSpeed = 1;
                    else
                        percentSpeed = moveProfile.Velocity;

                    newMoveProfile.Velocity = moveProfile.Velocity * percentSpeed / 100;
                    newMoveProfile.Acceleration = moveProfile.Acceleration * defaultAccel / 100;

                    controller.Commands.Motion.Setup.RampRate(ioAxis.NodeId, newMoveProfile.Acceleration);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Aerotech A3200 SetMoveProfile error on axis {0}: {1}", axis.Name, ex.Message), ex);
                }

                if (_moveProfileMap.Contains(ioAxis.NodeId))
                {
                    // Remove Old MoveProfile
                    _moveProfileMap.Remove(ioAxis.NodeId);
                }
                _moveProfileMap.Add(ioAxis.NodeId, newMoveProfile);
            }
        }

        public IMoveProfile GetMoveProfile(IAxisGroup axisGroup)
        {
            lock (this)
            {
                MoveProfile moveProfile = new MoveProfile();

                AxisGroup ioAxisGroup = axisGroup as AxisGroup;
                if (ioAxisGroup == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return moveProfile as IMoveProfile;

                if (_moveProfileMap.Contains(ioAxisGroup.Name))
                    moveProfile = (MoveProfile)_moveProfileMap[ioAxisGroup.Name];
                else
                    _moveProfileMap.Add(ioAxisGroup.Name, moveProfile);

                return moveProfile as IMoveProfile;
            }
        }

        public void SetMoveProfile(IAxisGroup axisGroup, IMoveProfile moveProfile)
        {
            lock (this)
            {
                AxisGroup ioAxisGroup = axisGroup as AxisGroup;
                if (ioAxisGroup == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                MoveProfile newMoveProfile = new MoveProfile();

                foreach (int nodeId in ioAxisGroup.AxisNoSet)
                {
                    try
                    {
                        double defaultAccel = (double)controller.Parameters.Axes[nodeId].Motion.DefaultRampRate.Value;
                        //double defaultSpeed = (double)controller.Parameters.Axes[nodeName].Motion.DefaultSpeed.Value;

                        double percentAccel;
                        if (moveProfile.Acceleration > 100)
                            percentAccel = 100;
                        else if (moveProfile.Acceleration <= 0)
                            percentAccel = 1;
                        else
                            percentAccel = moveProfile.Acceleration;

                        //double percentSpeed;
                        //if (moveProfile.Velocity > 100)
                        //    percentSpeed = 100;
                        //else if (moveProfile.Velocity <= 0)
                        //    percentSpeed = 1;
                        //else
                        //    percentSpeed = moveProfile.Velocity;

                        newMoveProfile.Velocity = moveProfile.Velocity;
                        newMoveProfile.Acceleration = percentAccel * defaultAccel / 100;

                        controller.Commands.Motion.Setup.RampRate(nodeId, newMoveProfile.Acceleration);
                        //MessageBox.Show(nodeName + ":" + newMoveProfile.Acceleration.ToString("0.000") + "," + newMoveProfile.Velocity.ToString("0.000"));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axisGroup.Name, ex.Message), ex);
                    }
                }

                if (_moveProfileMap.Contains(ioAxisGroup.Name))
                {
                    // Remove Old MoveProfile
                    _moveProfileMap.Remove(ioAxisGroup.Name);
                }
                _moveProfileMap.Add(ioAxisGroup.Name, newMoveProfile);


                //try
                //{
                //    Controller controller = Controller.ConnectedControllers[(string)_ioStore._configMap[ioAxisGroup.NodeName]];
                //    double defaultAccel = (double)controller.Parameters.Axes[ioAxisGroup.NodeName].Motion.DefaultRampRate.Value;
                //    double percentAccel;
                //    double[] newAccel = new double[ioAxisGroup.NodeNameSet.Length];

                //    if (moveProfile.Acceleration > 100)
                //        percentAccel = 100;
                //    else if (moveProfile.Acceleration <= 0)
                //        percentAccel = 1;
                //    else
                //        percentAccel = moveProfile.Acceleration;

                //    for (int i = 0; i < ioAxisGroup.NodeNameSet.Length; i++ )
                //    {
                //        newAccel[i] = moveProfile.Acceleration * defaultAccel / 100;
                //    }

                //    controller.Commands.Axes[ioAxisGroup.NodeNameSet].Motion.Setup.RampRate(newAccel);
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(string.Format("Aerotech A3200 error on axis {0}: {1}", axisGroup.Name, ex.Message), ex);
                //}


            }
        }

        #endregion

        #region IO Scheduler Thread

        private void ThreadHandler()
        {
            StopwatchW32HiRes loopStopwatch = new StopwatchW32HiRes();
            ArrayList executionList = new ArrayList();
            int pollDelay = 20;// Timeout.Infinite;

            // Thread loop ////////////////
            while (true)
            {
                loopStopwatch.Start();

                // block thread and wait for communication request 
                int ret = _ioRequestList.WaitForNotEmpty(eventStopIOExecThread, pollDelay);
                if (ret == 1)  // stop
                    break;

                // move from protected list to copy list
                _ioRequestList.Lock();
                executionList.AddRange(_ioRequestList.GetList());
                _ioRequestList.RemoveAll();
                _ioRequestList.Unlock();

                // At this point the request list is empty and free to use by other threads.
                // Further operation is done on the copy list only.

                // prepare io's 
                foreach (IOPoint ioPoint in executionList)
                {
                    ioPoint.timeoutStopwatch.Start();  // does nothing if already started
                    ioPoint.updatePending = true;
                }

                timer[(int)PerformanceMetric.DiagPacket].Reset();
                timer[(int)PerformanceMetric.DiagPacket].Start();
                try
                {
                    UpdateDiagnosticInfo();
                } catch(Exception ex)
                {
                    // do nothing
                }

                timer[(int)PerformanceMetric.DiagPacket].Stop();
                Transations[(int)PerformanceMetric.DiagPacket] += 1;
                TotalTime[(int)PerformanceMetric.DiagPacket] += timer[(int)PerformanceMetric.DiagPacket].ElapsedTime_sec;


                // check IO wait results
                foreach (IOPoint ioPoint in executionList)
                {
                    switch (ioPoint.IOType)
                    {
                        case IOPoint.Type.DigitalInput:
                            if (((DigitalInput)ioPoint).RequestedState == GetState((DigitalInput)ioPoint))
                            {
                                ioPoint.updatePending = false;
                                ioPoint.errorCode = 0;
                            }
                            break;
                        case IOPoint.Type.Axes:
                            switch (((AxisGroup)ioPoint).Criteria)
                            {
                                case AxisGroup.WaitCriteria.MoveDone:
                                    if (IsMoveDone((AxisGroup)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((AxisGroup)ioPoint) ? 2 : 0;
                                    }
                                    break;
                                case AxisGroup.WaitCriteria.InPosition:
                                    if (IsInPosition((AxisGroup)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((AxisGroup)ioPoint) ? 2 : 0;
                                    }
                                    break;
                            }
                            break;
                        case IOPoint.Type.Axis:
                            switch (((Axis)ioPoint).Criteria)
                            {
                                case Axis.WaitCriteria.MoveStart:
                                    if (IsMoving((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                    }
                                    break;
                                case Axis.WaitCriteria.MoveDone:
                                    if (IsMoveDone((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((Axis)ioPoint) ? 2 : 0;
                                    }
                                    break;
                                case Axis.WaitCriteria.InPostion:
                                    if (IsInPosition((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((Axis)ioPoint) ? 2 : 0;
                                    }
                                    break;
                                case Axis.WaitCriteria.HomeStart:
                                    if (!IsHomeDone((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((Axis)ioPoint) ? 2 : 0;
                                    }
                                    break;
                                case Axis.WaitCriteria.HomeDone:
                                    if (IsHomeDone((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = WasMoveAborted((Axis)ioPoint) ? 2 : 0;
                                    }
                                    break;
                                case Axis.WaitCriteria.PositionAbove:
                                    if (WasMoveAborted((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 2;
                                    }
                                    else if (GetActualPosition((Axis)ioPoint) > ((Axis)ioPoint).PositionThreshold)
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 0;
                                    }
                                    break;
                                case Axis.WaitCriteria.PositionBelow:
                                    if (WasMoveAborted((Axis)ioPoint))
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 2;
                                    }
                                    else if (GetActualPosition((Axis)ioPoint) < ((Axis)ioPoint).PositionThreshold)
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 0;
                                    }
                                    break;
                            }
                            break;
                        case IOPoint.Type.AnalogInput:
                            double currentValue = ((AnalogInput)ioPoint).Get();

                            switch (((AnalogInput)ioPoint).Criteria)
                            {
                                case AnalogInput.WaitCriteria.Above:
                                    if (currentValue > ((AnalogInput)ioPoint).Threshold)
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 0;
                                    }
                                    break;
                                case AnalogInput.WaitCriteria.Below:
                                    if (currentValue < ((AnalogInput)ioPoint).Threshold)
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 0;
                                    }
                                    break;
                                case AnalogInput.WaitCriteria.InRange:
                                    if (currentValue >= ((AnalogInput)ioPoint).Threshold - ((AnalogInput)ioPoint).Tolerance &&
                                        currentValue <= ((AnalogInput)ioPoint).Threshold + ((AnalogInput)ioPoint).Tolerance)
                                    {
                                        ioPoint.updatePending = false;
                                        ioPoint.errorCode = 0;
                                    }
                                    break;
                            }
                            break;
                    }

                    // check for timeout, if necessary
                    if (ioPoint.updatePending && ioPoint.timeOut >= 0)
                    {
                        if (ioPoint.timeoutStopwatch.ElapsedTime_sec > ioPoint.timeOut / 1000)
                        {
                            ioPoint.updatePending = false;
                            ioPoint.errorCode = 1; // timeout error
                        }
                    }
                }

                // remove updated io's 
                for (int i = 0; i < executionList.Count; i++)
                {
                    IOPoint pnt = (IOPoint)executionList[i];
                    if (pnt.updatePending == false)
                    {
                        executionList.RemoveAt(i);
                        pnt.timeoutStopwatch.Stop();
                        pnt.updateDone.Set();
                    }
                }

                //// set polling rate
                //if (executionList.Count == 0)
                //    pollDelay = Timeout.Infinite;
                //else
                //{
                //    // calculate remaining time
                //    loopStopwatch.Stop();
                //    int currentLoopTime = (int)loopStopwatch.ElapsedTime_sec * 1000;
                //    pollDelay = (currentLoopTime >= 20) ? 0 : (20 - currentLoopTime); // check every 20 msec for updates
                //}

                // calculate remaining time
                loopStopwatch.Stop();
                int currentLoopTime = (int)loopStopwatch.ElapsedTime_sec * 1000;
                pollDelay = (currentLoopTime >= 20) ? 0 : (20 - currentLoopTime); // check every 20 msec for updates

            } // Thread loop ////////////////        
        }

        public void SpawnIOExecThread()
        {
            if (_threadIOExec == null)
            {
                eventStopIOExecThread.Reset();

                _threadIOExec = new Thread(new ThreadStart(this.ThreadHandler));
                _threadIOExec.Name = "A3200 IOScheduler";
                _threadIOExec.Start();
            }
        }

        public void KillIOExecThread()
        {
            // stop and exit IOExec thread
            eventStopIOExecThread.Set();
            if (_threadIOExec != null)
            {
                _threadIOExec.Join();
                _threadIOExec = null;
            }
        }

        internal void ScheduleIORequest(IOPoint ioPoint)
        {
            ioPoint.updateDone.Reset();
            ioPoint.updatePending = true;
            _ioRequestList.AddTail(ioPoint);
        }

        #endregion

        #region Task

        public void LoadProgram(int taskIndex, string filePath)
        {
            try
            {
                if (simulation) return;
                controller.Tasks[taskIndex].Program.Load(filePath);
            }
            catch (A3200Exception ex)
            {
                throw new Exception("Cannot load program file path: " + filePath + Environment.NewLine + "Exception: " + ex.Message);
            }
        }

        public void RunProgram(int taskIndex)
        {
            if (simulation) return;
            if (controller.Tasks[taskIndex].Program.Associated)
            {
                controller.Tasks[taskIndex].Program.Start();
            }
            else
            {
                throw new Exception("There is no program associated in task no. " + taskIndex.ToString());
            }
        }

        public void RunProgram(int taskIndex, string filePath)
        {
            try
            {
                if (simulation) return;
                controller.Tasks[taskIndex].Program.Run(filePath);
            }
            catch (A3200Exception ex)
            {
                throw new Exception("Cannot run program file path: " + filePath + Environment.NewLine + "Exception: " + ex.Message);
            }
        }

        public void StopProgram(int taskIndex)
        {
            if (simulation) return;
            controller.Tasks[taskIndex].Program.Stop();
        }

        public bool IsLoadedProgram(string name)
        {
            if (simulation) return true;
            foreach (LoadedProgram loadedProgram in controller.LoadedPrograms)
            {
                if (loadedProgram.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public LoadedProgramCollection LoadedProgram
        {
            get { return controller.LoadedPrograms; }
        }

        public void UnloadAllPrograms()
        {
            if (simulation) return;
            foreach (LoadedProgram loadedProgram in controller.LoadedPrograms)
            {
                foreach (Task associatedTask in loadedProgram.AssociatedTasks)
                {
                    associatedTask.Program.Stop();
                }
                loadedProgram.Unload();
            }
        }

        public void UnloadProgram(string name)
        {
            if (simulation) return;
            foreach (LoadedProgram loadedProgram in controller.LoadedPrograms)
            {
                if (loadedProgram.Name == name)
                {
                    foreach (Task associatedTask in loadedProgram.AssociatedTasks)
                    {
                        associatedTask.Program.Stop();
                    }
                    loadedProgram.Unload();
                }
            }
        }

        public TasksCollection TaskCollection
        {
            get { return controller.Tasks; }
        }

        public Task GetTask(int taskIndex)
        {
            return controller.Tasks[taskIndex];
        }
        #endregion

        #region Get diagnotic information about an axis

        public double GetCurrentCommand(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].CurrentCommand;
            }
        }

        public double GetCurrentError(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].CurrentError;
            }
        }

        public double GetCurrentFeedback(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].CurrentFeedback;
            }
        }

        public double GetExternalPosition(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].PositionFeedbackAuxiliary;
            }
        }

        public double GetPositionCommand(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].PositionCommand;
            }
        }

        public double GetPositionError(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].PositionError;
            }
        }

        public double GetPositionFeedback(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].PositionFeedback;
            }
        }

        public double GetProgramPositionCommand(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].ProgramPositionCommand;
            }
        }

        public double GetProgramPositionError(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].ProgramPositionError;
            }
        }

        public double GetProgramPositionFeedback(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return diagPacketLatest[ioAxis.PointId].ProgramPositionFeedback;
            }
        }

        #endregion

        #region Get/Set limit parameters
        public void SaveParameters()
        {
            ParameterFile paramFile = new ParameterFile(@"C:\Users\Public\Documents\Aerotech\A3200\User Files\168828-A-1-1.prma");
            paramFile.Axes[0].Limits.SoftwareLimitHigh.Value = 80.1;
            paramFile.Axes[1].Limits.SoftwareLimitHigh.Value = 60.1;
            paramFile.Axes[2].Limits.SoftwareLimitHigh.Value = 25.1;
            paramFile.Axes[0].Limits.SoftwareLimitLow.Value = -80.1;
            paramFile.Axes[1].Limits.SoftwareLimitLow.Value = -0.1;
            paramFile.Axes[2].Limits.SoftwareLimitLow.Value = -25.1;
                paramFile.Save();
                controller.Reset();
        }

        /// <summary>
        /// Use the SoftwareLimitLow parameter to set the software limit lower threshold.
        /// The value specified for this parameter corresponds to the minimum position to which the axis can travel before violating the software limit. 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetSoftwareLimitLow(IAxis axis, double limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.SoftwareLimitLow.Value = limit;
            }
        }

        /// <summary>
        /// Use the SoftwareLimitHigh parameter to set the software limit upper threshold.
        /// The value specified for this parameter corresponds to the maximum position to which the axis can travel before violating the software limit. 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetSoftwareLimitHigh(IAxis axis, double limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.SoftwareLimitHigh.Value = limit;
            }
        }

        /// <summary>
        /// Use the LimitDecelDistance parameter to define the maximum distance that the axis will travel when decelerating within a end-of-travel or software limit.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetLimitDecelDistance(IAxis axis, double limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.LimitDecelDistance.Value = limit;
            }
        }

        /// <summary>
        /// Use the LimitDebounceDistance parameter to specify the distance that an axis will travel when debouncing a limit switch.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetLimitDebounceDistance(IAxis axis, double limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.LimitDebounceDistance.Value = limit;
            }
        }

        /// <summary>
        /// Use the LimitDebounceTime parameter to specify the number of consecutive milliseconds that an end-of-travel or 
        /// home limit switch must be read in the inactive state before the limit is considered to be cleared.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetLimitDebounceTime(IAxis axis, int limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.LimitDebounceTime.Value = limit;
            }
        }


        /// <summary>
        /// Use the EndOfTravelLimitSetup parameter to configure settings related to the end-of-travel limits. 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetEndOfTravelLimitSetup(IAxis axis, int limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.EndOfTravelLimitSetup.Value = limit;
            }
        }

        /// <summary>
        /// Use the EndOfTravelCurrentThresholdLow parameter to set the current threshold below which the axis is considered to have encountered the CCW or Negative end-of-travel limit.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetEndOfTravelCurrentThresholdLow(IAxis axis, float limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.EndOfTravelCurrentThresholdLow.Value = limit;
            }
        }

        /// <summary>
        /// Use the EndOfTravelCurrentThresholdHigh parameter to set the current threshold above which the axis is considered to have encountered the CW or Positive end-of-travel limit.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetEndOfTravelCurrentThresholdHigh(IAxis axis, float limit)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Limits.EndOfTravelCurrentThresholdHigh.Value = limit;
            }
        }

        /// <summary>
        /// Use the FaultAckMoveOutOfLimit parameter to specify whether axes moves out of an end-of-travel or software limit when you issue a fault acknowledge.
        /// </summary>
        /// <param name="limit">When you select No, the axis does not move when you issue a fault acknowledge, regardless of the active faults.
        /// When you select Yes, the axis moves out of an end-of-travel or software limit condition when you issue a fault acknowledge.</param>
        public void SetFaultAckMoveOutOfLimit(bool limit)
        {
            lock (this)
            {
                if (simulation)
                    return;

                if (limit)
                    controller.Parameters.System.FaultAckMoveOutOfLimit.Value = 1;
                else
                    controller.Parameters.System.FaultAckMoveOutOfLimit.Value = 0;
            }
        }

        /// <summary>
        /// Use the HomeSpeed parameter to set the speed of the axis during the home cycle. 
        /// This parameter is typically set to a low value to allow for controlled conditions during system startup. 
        /// This value is also used when the axis is moved out of a limit condition during a Fault Acknowledge or by using the MOVEOUTLIM command.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="limit"></param>
        public void SetHomeSpeed(IAxis axis, double speed)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return;

                controller.Parameters.Axes[ioAxis.PointId].Motion.Home.HomeSpeed.Value = speed;
            }
        }
        #endregion

        #region Get/Set Tuning Parameters: Kp,Kp1,Kpi,Kpos,Ki,Kv,Kd1,Aff,Dff,Pff,Vff,StaticFrictionCompensation
        /// <summary>
        /// Sets all the servo control loop gains.
        /// </summary>
        /// <param name="Axis">The axis on which to execute the command.</param>
        /// <param name="GainKpos">Set the position loop gain for the axis.</param>
        /// <param name="GainKi">Set the integral gain of the velocity loop for the axis.</param>
        /// <param name="GainKp">Set the proportional gain of the velocity loop for the axis.</param>
        /// <param name="GainAff">Set the acceleration feed forward gain.</param>
        public void setGain(IAxis axis, double GainKpos, double GainKi, double GainKp, double GainAff)
        {
            Axis ioAxis = axis as Axis;
            if (ioAxis == null)
                throw new Exception("Invalid Axis object.");

            if (!simulation)
                controller.Commands.Tuning.SetGain(ioAxis.PointId, GainKpos, GainKi, GainKp, GainAff);
        }

        /// <summary>
        /// Use the GainKp parameter to set the proportional gain of the velocity loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current KP value</returns>
        public double GetGainKp(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKp.Value;
            }
        }
        
        /// <summary>
        /// Use the GainKp parameter to set the proportional gain of the velocity loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKp(IAxis axis, double kP)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKp.Value = (float)kP;
            }
        }

        /// <summary>
        /// Use the GainKp1 parameter to set the proportional gain of the primary (position) loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current KP1 value</returns>
        public double GetGainKp1(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKp1.Value;
            }
        }

        /// <summary>
        /// Use the GainKp1 parameter to set the proportional gain of the primary (position) loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKp1(IAxis axis, double kP1)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKp1.Value = (float)kP1;
            }
        }

        /// <summary>
        /// Use the GainKpi parameter to set the integral gain of the position loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current KPi value</returns>
        public double GetGainKpi(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKpi.Value;
            }
        }

        /// <summary>
        /// Use the GainKpi parameter to set the integral gain of the position loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKpi(IAxis axis, double kPi)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKpi.Value = (float)kPi;
            }
        }

        /// <summary>
        /// Use the GainKpos parameter to set the proportional gain of the position loop.
        /// The GainKpos reduces the overall position error through the entire move and is applied in the position loop of the servo loop.
        /// It also reduces settling time by making sure that the axis is in position at the end of the move.
        /// If GainKpos is too high, the system can become unstable and start to oscillate.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current KPos value</returns>
        public double GetGainKpos(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKpos.Value;
            }
        }

        /// <summary>
        /// Use the GainKpos parameter to set the proportional gain of the position loop.
        /// The GainKpos reduces the overall position error through the entire move and is applied in the position loop of the servo loop.
        /// It also reduces settling time by making sure that the axis is in position at the end of the move.
        /// If GainKpos is too high, the system can become unstable and start to oscillate.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKpos(IAxis axis, double kPos)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKpos.Value = (float)kPos;
            }
        }

        /// <summary>
        /// Use the GainKi parameter to set the proportional gain of the position loop.
        /// GainKi eliminates steady-state velocity error during motion and is applied in the velocity loop of the servo loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Ki value</returns>
        public double GetGainKi(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKi.Value;
            }
        }

        /// <summary>
        /// Use the GainKi parameter to set the proportional gain of the position loop.
        /// GainKi eliminates steady-state velocity error during motion and is applied in the velocity loop of the servo loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKi(IAxis axis, double ki)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKi.Value = (float)ki;
            }
        }

        /// <summary>
        /// Use the GainKv parameter to set the scaling from the position loop to the velocity loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Kv value</returns>
        public double GetGainKv(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKv.Value;
            }
        }

        /// <summary>
        /// Use the GainKv parameter to set the scaling from the position loop to the velocity loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKv(IAxis axis, double kv)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKv.Value = (float)kv;
            }
        }

        /// <summary>
        /// Use the GainKd1 parameter to set the derivative gain of the primary (position) loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Kd1 value</returns>
        public double GetGainKd1(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKd1.Value;
            }
        }

        /// <summary>
        /// UUse the GainKd1 parameter to set the derivative gain of the primary (position) loop.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainKd1(IAxis axis, double kd1)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainKd1.Value = (float)kd1;
            }
        }

        /// <summary>
        /// Use the GainAff parameter to set the acceleration feed forward gain.
        /// GainAff compensates for the inertia of the system and reduces position error during the acceleration and deceleration phases of motion.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Aff value</returns>
        public double GetGainAff(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainAff.Value;
            }
        }

        /// <summary>
        /// Use the GainAff parameter to set the acceleration feed forward gain.
        /// GainAff compensates for the inertia of the system and reduces position error during the acceleration and deceleration phases of motion.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainAff(IAxis axis, double aff)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainAff.Value = (float)aff;
            }
        }

        /// <summary>
        /// Use the GainDff parameter to set the acceleration feed forward gain that will be used when an axis is decelerating.
        /// This gain is applied in exactly the same manner as GainAff, as shown in the Servo Loop Block Diagram.
        /// By default this parameter is set to zero, which indicates that the value of GainAff is used during both accleration and deceleration.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Dff value</returns>
        public double GetGainDff(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainDff.Value;
            }
        }

        /// <summary>
        /// Use the GainDff parameter to set the acceleration feed forward gain that will be used when an axis is decelerating.
        /// This gain is applied in exactly the same manner as GainAff, as shown in the Servo Loop Block Diagram.
        /// By default this parameter is set to zero, which indicates that the value of GainAff is used during both accleration and deceleration.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainDff(IAxis axis, double dff)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainDff.Value = (float)dff;
            }
        }

        /// <summary>
        /// Use the GainPff parameter to set the position feed forward gain.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Pff value</returns>
        public double GetGainPff(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainPff.Value;
            }
        }

        /// <summary>
        /// Use the GainPff parameter to set the position feed forward gain.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainPff(IAxis axis, double pff)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainPff.Value = (float)pff;
            }
        }

        /// <summary>
        /// Use the GainAff parameter to set the acceleration feed forward gain.
        /// GainAff compensates for the inertia of the system and reduces position error during the acceleration and deceleration phases of motion.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current Vff value</returns>
        public double GetGainVff(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainAff.Value;
            }
        }

        /// <summary>
        /// Use the GainAff parameter to set the acceleration feed forward gain.
        /// GainAff compensates for the inertia of the system and reduces position error during the acceleration and deceleration phases of motion.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetGainVff(IAxis axis, double vff)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.GainAff.Value = (float)vff;
            }
        }

        /// <summary>
        /// Use the StaticFrictionCompensation parameter to provide compensation for static friction.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <returns>double  type to show current StaticFrictionCompensation value</returns>
        public double GetStaticFrictionCompensation(IAxis axis)
        {
            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                if (simulation)
                    return 0.0;

                return (double)controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.StaticFrictionCompensation.Value;
            }
        }

        /// <summary>
        /// Use the StaticFrictionCompensation parameter to provide compensation for static friction.
        /// </summary>
        /// <param name="axis">IAxis object</param>
        /// <param name="kP">double type</param>
        public void SetStaticFrictionCompensation(IAxis axis, double staticFrictionCompensation)
        {
            if (simulation) return;

            lock (this)
            {
                Axis ioAxis = axis as Axis;
                if (ioAxis == null)
                    throw new Exception("Invalid Axis object.");

                controller.Parameters.Axes[ioAxis.PointId].ServoLoop.Gains.StaticFrictionCompensation.Value = (float)staticFrictionCompensation;
            }
        }
        #endregion
    }
}
