using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Seagate.AAS.Utils;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public abstract class IOPoint
    {
        // Nested declarations -------------------------------------------------

        public enum Type
        {
            Undefined = -1,
            DigitalInput,
            DigitalOutput,
            AnalogInput,
            AnalogOutput,
            Axis,
            Axes,
        }

        // Member variables ----------------------------------------------------

        protected A3200HC a3200HC;

        protected string name       = "n/a";
        protected uint   pointId    = 0;
        protected uint   nodeId     = 0;
        protected Type   type       = Type.Undefined;

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public int      timeOut     = 0;
        public int      errorCode   = 0;
        public bool     updatePending = false;
        public StopwatchW32HiRes timeoutStopwatch = new StopwatchW32HiRes();
        public AutoResetEvent updateDone          = new AutoResetEvent(false);

        // Constructors & Finalizers -------------------------------------------

        public IOPoint(A3200HC a3200HC, string name, uint nodeId, uint pointId)
        {
            this.a3200HC    = a3200HC;
            this.name       = name;
            this.nodeId     = nodeId;
            this.pointId    = pointId;
        }

        // Properties ----------------------------------------------------------

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int NodeId
        {
            get { return (int)nodeId; }
        }

        public int PointId
        {
            get { return (int)pointId; }
        }

        public Type IOType
        {
            get { return type; }
        }

        // Methods -------------------------------------------------------------

        public override string ToString()
        {
            return name;
        }

        public void WaitIOComplete()
        {
            WaitHandle[] eventGroup = new WaitHandle[2];
            eventGroup[0] = this.updateDone;
            eventGroup[1] = this.a3200HC.eventStopIOExecThread;

            WaitHandle.WaitAny(eventGroup);
        }
    }
}
