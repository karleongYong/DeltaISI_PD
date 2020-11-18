using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    internal class DigitalInput : IOPoint, IDigitalInput
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private DigitalIOState _state = DigitalIOState.Unknown;
        private DigitalIOState _requestedState = DigitalIOState.Unknown;

        // Constructors & Finalizers -------------------------------------------

        public DigitalInput(A3200HC a3200HC, string name, uint nodeID, uint pointId)
            : base(a3200HC, name, nodeID, pointId)
        {
            base.type = Type.DigitalInput;
        }

        // Properties ----------------------------------------------------------
        public DigitalIOState RequestedState
        {
            get { return _requestedState; }
            set { _requestedState = value; }
        }

        // Methods -------------------------------------------------------------

        #region IDigitalInput ---------------------------------------------
        DigitalIOState IDigitalInput.Get()
        {
            _state = a3200HC.GetState(this);
            return _state;
        }

        DigitalIOState IDigitalInput.State
        {
            get { return _state; }
        }

        void IDigitalInput.WaitForState(DigitalIOState stateTarget, uint nTimeOut)
        {
            a3200HC.StartInputWait(this, stateTarget, nTimeOut);
            this.WaitIOComplete();
            if (this.errorCode != 0)
                throw new HardwareException(string.Format("Timed out waiting for digital input [{0}] to be {1}.", this.Name, stateTarget.ToString()));
        }
        #endregion
    }
}
