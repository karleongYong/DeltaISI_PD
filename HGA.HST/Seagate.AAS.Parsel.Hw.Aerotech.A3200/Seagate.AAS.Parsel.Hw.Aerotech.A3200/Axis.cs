using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public class Axis : IOPoint, IAxis
    {
        // Nested declarations -------------------------------------------------
        public enum WaitCriteria
        {
            Enable,
            HomeStart,
            HomeDone,
            MoveStart,
            MoveDone,
            InPostion,
            Stop,
            PositionAbove,
            PositionBelow,
        };

        // Member variables ----------------------------------------------------        
        protected bool    homed                   = false;
        private   bool    enabled                 = false;
        private   bool    swEnabled               = false;
        private   double  countsPerUnit           = 1.0;
        private   string  unitName                = "mm";
        private   double  secondaryCountsPerUnit  = 1.0;
        private   string  secondaryUnitName       = "mm";

        private IMoveProfile lastProfile;
        private IMoveProfile nativeProfile;

        // Support for "rounded corner" moves -- HG
        // used in WaitPositionAboveThreshold and WaitPositionBelowThreshold methods
        private WaitCriteria criteria = WaitCriteria.MoveDone;
        private double positionThreshold = 0.0;

        // native Ensemble units are mm/s for velocity and G for accel


        // Constructors & Finalizers -------------------------------------------
        public Axis(A3200HC a3200HC, string name, uint nodeId, uint pointId)
            : base(a3200HC, name, nodeId, pointId)
        {
            base.type       = Type.Axis;
            lastProfile     = new MoveProfile();
            nativeProfile   = new MoveProfile();
        }

        // Properties ----------------------------------------------------------
        public bool IsSoftwareEnabled
        {
            get
            {
                return swEnabled;
            }
        }

        public bool IsMoving
        {
            get
            {
                return a3200HC.IsMoving(this);
            }
        }

        public double SecondaryCountsPerUnit
        {
            get { return secondaryCountsPerUnit; }
        }

        public string SecodaryUnit
        {
            get { return secondaryUnitName; }
        }

        public WaitCriteria Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }

        public double PositionThreshold
        {
            get { return positionThreshold; }
            set { positionThreshold = value; }
        }

        // Methods -------------------------------------------------------------

        #region IAxis ---------------------------------------------
        public double CountsPerUnit
        {
            get { return countsPerUnit; }
        }

        public void Enable(bool enable)
        {
            swEnabled = enable;

            if (a3200HC.simulation)
                enabled = enable;

            if (enable != this.IsEnabled)
			    a3200HC.Enable(this, enable);
        }

        public double GetActualPosition()
        {
            double position = a3200HC.GetActualPosition(this);
            return position/countsPerUnit;
        }

        public double GetCommandPosition()
        {
            double position = a3200HC.GetCommandPosition(this);
            return position/countsPerUnit;
        }

        public IMoveProfile GetMoveProfile()
        {
            return lastProfile;
        }

        public void Home(uint msecTimeOut)
        {
			HomeStart();
			WaitHomeDone(msecTimeOut);

			if (!this.IsHomed)
			{
				throw new Exception(string.Format("Axis [{0}] homing timed out.", this.Name));
			}
        }

        public void HomeAsync(uint msecTimeOut)
        {
            HomeAsyncStart();
            WaitMoveDone(msecTimeOut);

            if (!this.IsHomed)
            {
                throw new Exception(string.Format("Axis [{0}] homing timed out.", this.Name));
            }
        }

        public void HomeAsyncStart()
        {
            if (!a3200HC.simulation)
                a3200HC.HomeAsync(this);
            else
                homed = true;
        }

        public void HomeStart()
        {
            if (!a3200HC.simulation)
                a3200HC.Home(this);
            else
                homed = true;
        }

        public bool IsEnabled
        {
            get 
            {
                if (!a3200HC.simulation)
                    enabled = a3200HC.IsEnabled(this);
                return enabled; 
            }
        }

        public bool IsHomed
        {
            get 
            { 
                if (!a3200HC.simulation)
                    homed = a3200HC.IsHomed(this);
                return homed; 
            }
        }

        public bool IsInRange(double position, double range)
        {
            return (Math.Abs(GetActualPosition()-position) <= range);
        }

        public bool IsMoveDone
        {
            get 
            { 
                return a3200HC.IsMoveDone(this); 
            }
        }

        public double MotorCurrent
        {
            get { return a3200HC.GetCurrentFeedback(this); }
        }

        public void MoveAbsolute(double acceleration, double velocity, double position, uint msecTimeOut)
        {
            MoveAbsoluteStart(acceleration, velocity, position);
			WaitMoveDone(msecTimeOut);
        }

        public void MoveAbsolute(IMoveProfile moveProfile, double position, uint msecTimeOut)
        {
            MoveAbsoluteStart(moveProfile, position);
			WaitMoveDone(msecTimeOut);
        }

        public void MoveAbsoluteStart(double acceleration, double velocity, double position)
        {
            if (!this.IsHomed)
				throw new HardwareException (string.Format("Axis [{0}] not homed. Cannot move absolutely.", this.Name) );

			if(acceleration != lastProfile.Acceleration || velocity != lastProfile.Velocity)
			{
                // profile parameters are stored in user-specified units
				lastProfile.Acceleration = acceleration;
				lastProfile.Deceleration = acceleration;
				lastProfile.Velocity = velocity;
			}
            a3200HC.Move(this, acceleration, velocity, position);

            // wait for move to start before continuing
            //a3200HC.StartWaitMoveStart(this, 1000);
            //this.WaitIOComplete();
        }

        public void MoveAbsoluteStart(IMoveProfile moveProfile, double position)
        {
            if (!this.IsHomed)
                throw new HardwareException (string.Format("Axis [{0}] not homed. Cannot move absolutely.", this.Name) );

            SetMoveProfile(moveProfile);
            
            a3200HC.Move(this, position);
        }

        public void MoveRelative(double acceleration, double velocity, double positionRelative, uint msecTimeOut)
        {
            MoveRelativeStart(acceleration, velocity, positionRelative);
			WaitMoveDone(msecTimeOut);
        }

        public void MoveRelative(IMoveProfile moveProfile, double positionRelative, uint msecTimeOut)
        {
            MoveRelativeStart(moveProfile, positionRelative);
			WaitMoveDone(msecTimeOut);
        }

        public void MoveRelativeStart(double acceleration, double velocity, double positionRelative)
        {
            if(acceleration != lastProfile.Acceleration || velocity != lastProfile.Velocity)
			{
				lastProfile.Acceleration = acceleration;
				lastProfile.Deceleration = acceleration;
				lastProfile.Velocity = velocity;
			}
            a3200HC.MoveRel(this, acceleration, velocity, positionRelative);
        }

        public void MoveRelativeStart(IMoveProfile moveProfile, double positionRelative)
        {
            SetMoveProfile(moveProfile);
            a3200HC.MoveRel(this, positionRelative);
        }

        public void MoveVelocity(double acceleration, double velocity, double position)
        {
            
        }

        public double PeakCurrentLimit
        {
            get { return 0; }
			set {  }
        }

        public int PeakCurrentTime
        {
            get { return 0; }
			set {  }
        }

        public double PositionErrorWindow
        {
            get { return a3200HC.GetPositionError(this) / countsPerUnit; }
            set { }
        }

        public void SetMoveProfile(IMoveProfile moveProfile)
        {
            if(lastProfile.Equals(moveProfile))
				return;
			UpdateLastProfile(moveProfile);

			a3200HC.SetMoveProfile(this, moveProfile);
        }

        public void SetSecondaryUnit(string unitName, double countsPerUnit)
        {
            this.unitName = unitName;
            if (countsPerUnit == 0)
            {
                throw new Exception("cannot set secondaryCountsPerUnit to 0.0");
            }
            this.secondaryCountsPerUnit = countsPerUnit;
        }

        public void SetUnit(string unitName, double countsPerUnit)
        {
            this.unitName = unitName;
            if (countsPerUnit == 0)
            {
                throw new Exception("cannot set countsPerUnit to 0.0");
            }
            this.countsPerUnit = countsPerUnit;
            MakeNativeProfile(lastProfile);
        }

        public void Stop()
        {
            a3200HC.Stop(this);
        }

        public string Unit
        {
            get { return unitName; }
        }

        public double VelocityActual
        {
            get { return 0; }
            set { }
        }

        public void WaitMoveDone(uint msecTimeOut)
        {
            if (a3200HC.simulation)
                return;

            a3200HC.StartWaitMoveDone(this, msecTimeOut);
            this.WaitIOComplete();

            if (this.errorCode != 0)
            {
                switch (this.errorCode)
                {
                    case 1:
                        throw new HardwareException(string.Format("Axis [{0}] timed out waiting for move done.", this.Name));
                    case 2:
                        throw new HardwareException(string.Format("Axis [{0}] move was aborted.", this.Name));
                    default:
                        throw new HardwareException(string.Format("Axis [{0}] had unknown error = {1}", this.Name, this.errorCode));
                }
            }
        }

        public void WaitPositionAboveThreshold(double threshold, uint msecTimeout)
        {
            if (GetActualPosition() > threshold)			// return immediately if condition already true to preserve 
                return;									// the thread's time-slice

            a3200HC.StartWaitPositionAboveThreshold(this, threshold * countsPerUnit, msecTimeout);
            this.WaitIOComplete();
            if (a3200HC.simulation)
                this.errorCode = 0;

            if (this.errorCode != 0)
            {
                switch (this.errorCode)
                {
                    case 1:
                        throw new HardwareException(string.Format("Axis [{0}] timed out waiting for position above threshold.", this.Name));
                    case 2:
                        throw new HardwareException(string.Format("Axis [{0}] move was aborted.", this.Name));
                    default:
                        throw new HardwareException(string.Format("Axis [{0}] had unknown error = {1}", this.Name, this.errorCode));
                }
            }
        }

        public void WaitPositionBelowThreshold(double threshold, uint msecTimeout)
        {
            if (GetActualPosition() < threshold)		// return immediately if condition already true to preserve
                return;								// the thread's time-slice

            a3200HC.StartWaitPositionBelowThreshold(this, threshold * countsPerUnit, msecTimeout);
            this.WaitIOComplete();
            if (a3200HC.simulation)
                this.errorCode = 0;

            if (this.errorCode != 0)
            {
                switch (this.errorCode)
                {
                    case 1:
                        throw new HardwareException(string.Format("Axis [{0}] timed out waiting for position below threshold.", this.Name));
                    case 2:
                        throw new HardwareException(string.Format("Axis [{0}] move was aborted.", this.Name));
                    default:
                        throw new HardwareException(string.Format("Axis [{0}] had unknown error = {1}", this.Name, this.errorCode));
                }
            }
        }
        #endregion       




        public double GetNativeCommandPosition()
        {
            return a3200HC.GetCommandPosition(this);
        }

        private void MakeNativeProfile(IMoveProfile moveProfile)
        {
            nativeProfile.Acceleration = moveProfile.Acceleration * countsPerUnit;  // convert acceleration to G's
            nativeProfile.Deceleration = moveProfile.Deceleration * countsPerUnit;
            nativeProfile.Velocity = moveProfile.Velocity * countsPerUnit;
            nativeProfile.FollowingError = moveProfile.FollowingError * countsPerUnit;
            nativeProfile.Jerk = moveProfile.Jerk * countsPerUnit;
            nativeProfile.SettlingWindow = moveProfile.SettlingWindow * countsPerUnit;
        }

        public IMoveProfile GetNativeMoveProfile()
        {
            return nativeProfile;
        }

        private bool ProfileUnchanged(IMoveProfile moveProfile)
        {
            return moveProfile.MoveType == lastProfile.MoveType &
                moveProfile.Jerk == lastProfile.Jerk &
                moveProfile.Acceleration == lastProfile.Acceleration &
                moveProfile.Deceleration == lastProfile.Deceleration &
                moveProfile.Velocity == lastProfile.Velocity &
                moveProfile.SettlingWindow == lastProfile.SettlingWindow &
                moveProfile.SettlingTime == lastProfile.SettlingTime &
                moveProfile.FollowingError == lastProfile.FollowingError;
        }

        private void UpdateLastProfile(IMoveProfile moveProfile)
        {
            lastProfile.MoveType = moveProfile.MoveType;
            lastProfile.Jerk = moveProfile.Jerk;
            lastProfile.Acceleration = moveProfile.Acceleration;
            lastProfile.Deceleration = moveProfile.Deceleration;
            lastProfile.Velocity = moveProfile.Velocity;
            lastProfile.SettlingWindow = moveProfile.SettlingWindow;
            lastProfile.SettlingTime = moveProfile.SettlingTime;
            lastProfile.FollowingError = moveProfile.FollowingError;

            MakeNativeProfile(moveProfile);

        }

        public void WaitHomeDone(uint msecTimeOut)
        {
            if (a3200HC.simulation)
                return;

            updateDone.Reset();
            a3200HC.StartWaitHomeDone(this, msecTimeOut);
            this.WaitIOComplete();

            if (!this.IsHomed)
            {
                throw new HardwareException(string.Format("Axis [{0}] failed to home.", this.Name));
            }
        }

        public void WaitMoveStart(uint msecTimeOut)
        {
            if (a3200HC.simulation)
                return;

            a3200HC.StartWaitMoveStart(this, msecTimeOut);
            this.WaitIOComplete();
        }

        public void FreeRun(double velocity)
        {
            a3200HC.FreeRun(this, velocity);
        }

        public void FreeRunStop()
        {
            a3200HC.FreeRunStop(this);
        }

        public bool IsAxisFault
        {
            get
            {
                return a3200HC.IsAxisFault(this);
            }
        }

        public void FaultAcknowledge()
        {
            a3200HC.FaultAchnowledge(this);
        }



        public double FollowingErrorInMotionVariable
        {
            set { ; }
        }

        public DigitalIOState ForwardLimitSwitch
        {
            get { return DigitalIOState.Unknown; }
        }

        public DigitalIOState ReverseLimitSwitch
        {
            get { return DigitalIOState.Unknown; }
        }

        //public double PositionThreshold
        //{
        //    get { return 0.0; }
        //    set { ; }
        //}

        public double ForwardSoftwareLimit
        {
            get { return 0.0; }
            set { ; }
        }

        public double ReverseSoftwareLimit
        {
            get { return 0.0; }
            set { ; }
        }

        public void SetForwardSoftwareLimit(double limit)
        {
            a3200HC.SetSoftwareLimitHigh(this, limit);
        }

        public void SetReverseSoftwareLimit(double limit)
        {

        }

        public void DownLoadMoveTuneParams(double kPos, double ki, double kp, double aff)
        {
            a3200HC.setGain(this, kPos, ki, kp, aff);
        }

        public void DownLoadHomingTuneParams(double kP, double kI, double kD, double iT, double iL, double fA, double fV, double pL, double nB, double nF, double nZ)
        {

        }

        public void DownLoadShortMoveTuneParams(double kP, double kI, double kD, double iT, double iL, double fA, double fV, double pL, double nB, double nF, double nZ)
        {

        }

        public void DownLoadFineSettlingParams(double inPos, double timeWindow, double timeOut)
        {

        }

        public void DownLoadFineSettlingParams(double inPos, double timeWindow, double timeOut, double posErrorLimit)
        {

        }

        public void DownLoadCoarseSettlingParams(double inPos, double timeWindow, double timeOut)
        {

        }

        public void DownLoadCoarseSettlingParams(double inPos, double timeWindow, double timeOut, double posErrorLimit)
        {

        }

        public void SetTuneParams(double kP, double kI, double kD)
        {
        }

        public void SetTuneParams(double kP, double kI, double kD, double iT, double iL, double fA, double fV, double pL, double nB, double nF, double nZ)
        {

        }

        public void SetAcc(double acc)
        {

        }

        public void SetDcc(double dec)
        {

        }

        public void SetSpeed(double speed)
        {

        }

        public void SetSettling(double inPos, double timeWindow, double timeOut)
        {

        }

        public void SetSettling(double inPos, double timeWindow, double timeOut, double posErrorLimit)
        {

        }

        //public virtual void HomeStart()
        //{

        //}

        //public void WaitHomeDone(uint msecTimeOut)
        //{

        //}

        public void SetFollowingError(double error)
        {

        }

        // Internal methods ----------------------------------------------------

    }
}
