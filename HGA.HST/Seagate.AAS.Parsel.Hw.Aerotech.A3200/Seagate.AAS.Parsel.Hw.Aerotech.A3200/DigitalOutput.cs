using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    internal class DigitalOutput : IOPoint, IDigitalOutput
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private DigitalIOState _state = DigitalIOState.Unknown;
        private DigitalIOState _stateRequested = DigitalIOState.Unknown;

        // Constructors & Finalizers -------------------------------------------

        public DigitalOutput(A3200HC iai, string name, uint nodeId, uint pointId)
            : base(iai, name, nodeId, pointId)
        {
            base.type = Type.DigitalOutput;
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        #region IDigitalOutput ---------------------------------------------
        public DigitalIOState Get()
        {
            _state = a3200HC.GetState(this);
            return _state;
        }

        public void Set(DigitalIOState state)
        {
            _state = state;
            a3200HC.SetState(this, _state);
        }

        public void SetRequest(DigitalIOState state)
        {
            
        }

        public DigitalIOState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public DigitalIOState StateRequested
        {
            get { return _stateRequested; }
        }

        public void Toggle()
        {
            _state = (_state == DigitalIOState.On) ? DigitalIOState.Off : DigitalIOState.On;
        }
        #endregion
    }
}
