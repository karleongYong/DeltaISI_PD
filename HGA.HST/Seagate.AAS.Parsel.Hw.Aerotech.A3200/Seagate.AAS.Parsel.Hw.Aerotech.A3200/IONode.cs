using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    internal class IONode
    {
        // Nested declarations -------------------------------------------------



        // Member variables ----------------------------------------------------
        public readonly uint nodeID          = 0;
        public readonly string nodeName     = "n/a";

        public readonly int digInCount      = -1;
        public readonly int digOutCount     = -1;
        public readonly int analogInCount   = -1;
        public readonly int analogOutCount  = -1;
        public readonly int axisCount       = -1;

        public Hashtable digitalInputMap    = null;
        public Hashtable digitalOutputMap   = null;
        public Hashtable analogInputMap     = null;
        public Hashtable analogOutputMap    = null;
        public Axis axis = null;

        public short[] group;

        private A3200HC a3200HC;
        // Constructors & Finalizers -------------------------------------------

        public IONode(A3200HC a3200HC, uint nodeID, string nodeName
                    , uint digInCount
                    , uint digOutCount
                    , uint analogInCount
                    , uint analogOutCount)
        {
            this.a3200HC        = a3200HC;
            this.nodeID         = nodeID;
            this.nodeName       = nodeName;
            this.digInCount     = (int)digInCount;
            this.digOutCount    = (int)digOutCount;
            this.analogInCount  = (int)analogInCount;
            this.analogOutCount = (int)analogOutCount;
            this.axisCount      = 1; //Fix 1 node = 1 axis

            digitalInputMap     = new Hashtable();
            digitalOutputMap    = new Hashtable();
            analogInputMap      = new Hashtable();
            analogOutputMap     = new Hashtable();

            // create all IO points here and assign default names
            string pointName;
            for (uint point = 0; point < this.digInCount; point++)
            {
                pointName = string.Format("DigitalIn {0}", point);
                this.CreateDigitalInput(pointName, point);
            }

            for (uint point = 0; point < this.digOutCount; point++)
            {
                pointName = string.Format("DigitalOut {0}", point);
                this.CreateDigitalOutput(pointName, point);
            }

            for (uint point = 0; point < this.analogInCount; point++)
            {
                pointName = string.Format("AnalogIn {0}", point);
                this.CreateAnalogInput(pointName, point);
            }

            for (uint point = 0; point < this.analogOutCount; point++)
            {
                pointName = string.Format("AnalogOut {0}", point);
                this.CreateAnalogOutput(pointName, point);
            }

            for (uint point = 0; point < this.axisCount; point++)
            {
                pointName = string.Format("Axis {0}", nodeID);
                this.CreateAxis(pointName, nodeID);
            }
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        // IO point creation

        public DigitalInput CreateDigitalInput(string pointName, uint point)
        {
            if (digitalInputMap.ContainsKey(point))
            {
                throw new Exception("The digital IO already exist.");
            }

            if (point > digInCount)
            {
                throw new Exception("The assigned point is beyond limit.");
            }

            DigitalInput ioPoint = new DigitalInput(a3200HC, pointName, nodeID, point);
            digitalInputMap.Add(point, ioPoint);
            return ioPoint;
        }

        public DigitalOutput CreateDigitalOutput(string pointName, uint point)
        {
            if (digitalOutputMap.ContainsKey(point))
            {
                throw new Exception("The digital IO already exist.");
            }
            if (point > digOutCount)
            {
                throw new Exception("The assigned point is beyond limit.");
            }

            DigitalOutput ioPoint = new DigitalOutput(a3200HC, pointName, nodeID, point);
            digitalOutputMap.Add(point, ioPoint);
            return ioPoint;
        }

        public AnalogInput CreateAnalogInput(string pointName, uint point)
        {
            if (analogInputMap.ContainsKey(point))
            {
                throw new Exception("The analog IO already exist.");
            }
            if (point > analogInCount)
            {
                throw new Exception("The assigned point is beyond limit.");
            }

            AnalogInput ioPoint = new AnalogInput(a3200HC, pointName, nodeID, point);
            analogInputMap.Add(point, ioPoint);
            return ioPoint;
        }

        public AnalogOutput CreateAnalogOutput(string pointName, uint point)
        {
            if (analogOutputMap.ContainsKey(point))
            {
                throw new Exception("The analog IO already exist.");
            }
            if (point > analogOutCount)
            {
                throw new Exception("The assigned point is beyond limit.");
            }

            AnalogOutput ioPoint = new AnalogOutput(a3200HC, pointName, nodeID, point);
            analogOutputMap.Add(point, ioPoint);
            return ioPoint;
        }

        public Axis CreateAxis(string pointName, uint point)
        {
            axis = new Axis(a3200HC, pointName, nodeID, point);
            return axis;
        }

        public IDigitalInput GetDigitalInput(uint point)
        {
            if (!digitalInputMap.ContainsKey(point))
            {
                throw new Exception("Digital Input does not exist.");
            }

            DigitalInput ioPoint = (DigitalInput)digitalInputMap[point];
            return (IDigitalInput)ioPoint;
        }

        public IDigitalOutput GetDigitalOutput(uint point)
        {
            if (!digitalOutputMap.ContainsKey(point))
            {
                throw new Exception("Digital Output does not exist.");
            }

            DigitalOutput ioPoint = ((DigitalOutput)digitalOutputMap[point]);
            return (IDigitalOutput)ioPoint;
        }

        public IAnalogInput GetAnalogInput(uint point)
        {
            if (!analogInputMap.ContainsKey(point))
            {
                throw new Exception("Analog Input does not exist.");
            }

            AnalogInput ioPoint = ((AnalogInput)analogInputMap[point]);
            return (IAnalogInput)ioPoint;
        }

        public IAnalogOutput GetAnalogOutput(uint point)
        {
            if (!analogOutputMap.ContainsKey(point))
            {
                throw new Exception("Analog Output does not exist.");
            }

            AnalogOutput ioPoint = ((AnalogOutput)analogOutputMap[point]);
            return (IAnalogOutput)ioPoint;
        }

        public IAxis GetAxis()
        {
            return axis;
        }
    }
}
