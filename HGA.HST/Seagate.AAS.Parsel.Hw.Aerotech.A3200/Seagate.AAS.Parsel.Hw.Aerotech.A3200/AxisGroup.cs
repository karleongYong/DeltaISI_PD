using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public class AxisGroup : IOPoint, IAxisGroup
    {
        // Nested declarations -------------------------------------------------
        public enum WaitCriteria
        {
            MoveDone,
            InPosition,
        }

        public enum AxisGroupType
        {
            DoubleCarriage,
            Parallel,
            Unknown,
        }

        // Member variables ----------------------------------------------------
        private int[] axisNoSet;
        private double safetyOffset = 0;
        private int plane           = 0;
        private bool safetyMove     = true;
        private WaitCriteria criteria = WaitCriteria.MoveDone;
        public AxisGroupType GroupType = AxisGroupType.Unknown;


        // Constructors & Finalizers -------------------------------------------
        public AxisGroup(A3200HC a3200HC, string name, uint[] axisNoSet)
            : base(a3200HC, name, 0, 0)
        {
            this.name           = name;
            this.a3200HC        = a3200HC;
            this.type           = Type.Axes;
            this.axisNoSet    = (int[])axisNoSet.Clone();

            if (axisNoSet.Length < 2)
                throw new Exception("Node name set must 2 or more name");
        }

        public AxisGroup(A3200HC a3200HC, string name, string[] nodeNameSet, AxisGroupType groupType, double safetyOffset)
            : base(a3200HC, name, 0, 0)
        {
            this.name           = name;
            this.a3200HC        = a3200HC;
            this.type           = Type.Axes;
            this.axisNoSet    = (int[])nodeNameSet.Clone();

            if (nodeNameSet.Length < 2)
                throw new Exception("Node name set must 2 or more name");

            this.GroupType = groupType;

            if (groupType == AxisGroupType.DoubleCarriage)
            {
                if (nodeNameSet.Length != 2)
                    throw new HardwareException("Double Carriage support only 2 axis number");

                if (safetyOffset == 0)
                    throw new HardwareException("Double Carriage must have safetyOffset configuration");
                else
                    this.safetyOffset = safetyOffset;
            }
        }

        // Properties ----------------------------------------------------------
        public int[] AxisNoSet
        {
            get { return axisNoSet; }
        }

        public int Plane
        {
            get { return plane; }
        }

        public bool IsSafetyMove
        {
            get { return safetyMove; }
        }

        public WaitCriteria Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }

        // Methods -------------------------------------------------------------

        #region IAxisGroup ---------------------------------------------
        public void GetMoveLimits(out double velocity, out double acceleration, out double jerk)
        {
            MoveProfile moveProfile = (MoveProfile)a3200HC.GetMoveProfile(this);
            velocity        = moveProfile.Velocity;
            acceleration    = moveProfile.Acceleration;
            jerk = 0;
        }

        public void Move(double[] position, uint msecTimeOut)
        {
            MoveStart(position);
            WaitInPosition(msecTimeOut);
        }

        public void MoveStart(double[] position)
        {
            if (safetyMove && (this.GroupType == AxisGroupType.DoubleCarriage))
            {
                if (position[1] - position[0] > this.safetyOffset)
                    throw new HardwareException(string.Format("Cannot move to target cause carriage may hit together {0} = {1} {2} = {3} and safetyOffset = {4}", axisNoSet[0], position[0], axisNoSet[1], position[1], this.safetyOffset));
            }
            safetyMove = true; // Always reset safety move every time
            a3200HC.MoveStart(this, position);
        }

        public void SetMoveLimits(double velocity, double acceleration, double jerk)
        {
            MoveProfile moveProfile = new MoveProfile(acceleration, velocity, 0.01, 0);
            a3200HC.SetMoveProfile(this, moveProfile);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void TrajectoryAdd(double[,] position, double[,] velocity, int[] time, uint lowWater)
        {
            throw new NotImplementedException();
        }

        public void TrajectoryInitialize(double[,] position, double[,] velocity, int[] time, uint lowWater)
        {
            throw new NotImplementedException();
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
        #endregion

        public void WaitInPosition(uint msecTimeOut)
        {
            if (a3200HC.simulation)
                return;

            a3200HC.StartWaitInPosition(this, msecTimeOut);
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

        public void SetPlane(int plane)
        {
            if (plane == 0)
                this.plane = 0;
            else
                this.plane = 1;
        }

        public void OneTimeSkipSafetyMove()
        {
            safetyMove = false;
        }

        public void SetSafetyOffset(double newSafetyOffset)
        {
            if (newSafetyOffset != 0)
                this.safetyOffset = newSafetyOffset;
        }

        public void SetMoveProfile(IMoveProfile moveProfile)
        {
            a3200HC.SetMoveProfile(this, moveProfile);
        }

        public IMoveProfile GetMoveProfile()
        {
            return a3200HC.GetMoveProfile(this);
        }

        // Internal methods ----------------------------------------------------
    }
}
