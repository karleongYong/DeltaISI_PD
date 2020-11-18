using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public class MoveProfile : IMoveProfile
    {
        // Nested declarations -------------------------------------------------


        // Member variables ----------------------------------------------------
        private MoveType moveType       = MoveType.Trap;
        private double jerk             = 50000.0;
        private double acceleration     = 2000.0;
        private double deceleration     = 2000.0;
        private double velocity         = 300.0;
        private double settlingWindow   = 0.1;
        private short settlingTime      = 10;
        private double followingError   = 10.0;

        // Constructors & Finalizers -------------------------------------------
        public MoveProfile()
        {

        }

        public MoveProfile(double acceleration, double velocity, double settlingWindow, double settlingTime)
        {
            this.acceleration   = acceleration;
            this.deceleration   = acceleration;
            this.velocity       = velocity;
            this.settlingWindow = settlingWindow;
            this.settlingTime   = (short)settlingTime;
        }

        // Properties ----------------------------------------------------------
        public MoveType MoveType
        {
            get { return moveType; }
            set { moveType = value; }
        }

        public double Jerk
        {
            get { return jerk; }
            set { jerk = value; }
        }

        public double Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public double Deceleration
        {
            get { return deceleration; }
            set { deceleration = value; }
        }

        public double Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public double SettlingWindow
        {
            get { return settlingWindow; }
            set { settlingWindow = value; }
        }

        public short SettlingTime
        {
            get { return settlingTime; }
            set { settlingTime = value; }
        }

        public double FollowingError
        {
            get { return followingError; }
            set { followingError = value; }
        }

        // Methods -------------------------------------------------------------


        // Internal methods ----------------------------------------------------
    }
}
