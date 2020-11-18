using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Parsel.Hw.Aerotech.A3200
{
    public class IOStore
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        private A3200HC a3200HC;
        internal Hashtable _nodesMap = new Hashtable();
        //internal Hashtable _configMap = new Hashtable();
        internal Hashtable axisGroupMap = new Hashtable();
        internal uint axisCount = 0;

        internal string registrationErrors = "";

        // Constructors & Finalizers -------------------------------------------
        internal IOStore(A3200HC a3200HC)
        {
            this.a3200HC = a3200HC;
        }

        // Properties ----------------------------------------------------------
        public int NodeCount
        {
            get
            {
                return _nodesMap.Count;
            }

        }

        internal Hashtable Nodes
        {
            get
            {
                return _nodesMap;
            }
        }

        // Methods -------------------------------------------------------------
        public void RegisterController(uint nAxisCount)
        {
            axisCount = nAxisCount;
        }

        public void RegisterAxis(uint axisNo, string axisName, uint digInCount, uint digOutCount, uint anaInCount, uint anaOutCount)
        {
            if (_nodesMap.Count >= axisCount)
            {
                registrationErrors += string.Format("Requested axis [No={0}, Axis={1}] is greater than limited of registration [{2} axes].\n", axisNo, axisName, axisCount);
            }

            if (_nodesMap.Contains(axisNo))
            {
                registrationErrors += string.Format("Requested Node [No={0}, Axis={1}] has already been defined [existing name={2}]\n", axisNo, axisName, ((IONode)_nodesMap[axisNo]).nodeName);
            }
            else
            {
                IONode node = new IONode(a3200HC, axisNo, axisName, digInCount, digOutCount, anaInCount, anaOutCount);
                _nodesMap.Add(axisNo, node);
            }
        }

        public AxisGroup RegisterAxisGroup(uint[] axisNoSet, string name)
        {
            if (axisGroupMap.ContainsKey(name))
            {
                throw new Exception("The axis group already exist.");
            }

            AxisGroup axisGroup = new AxisGroup(a3200HC, name, axisNoSet);
            axisGroupMap.Add(name, axisGroup);
            return axisGroup;
        }

        public AxisGroup RegisterAxisGroup(string[] nodeNameSet, string name, AxisGroup.AxisGroupType groupType, double safetyOffset)
        {
            if (axisGroupMap.ContainsKey(name))
            {
                throw new Exception("The axis group already exist.");
            }

            AxisGroup axisGroup = new AxisGroup(a3200HC, name, nodeNameSet, groupType, safetyOffset);
            axisGroupMap.Add(name, axisGroup);
            return axisGroup;
        }

        public IAxisGroup GetAxisGroup(string groupName)
        {
            return (IAxisGroup)axisGroupMap[groupName];
        }

        public IDigitalInput GetDigitalInput(uint axisNo, uint point)
        {
            return ((IONode)_nodesMap[axisNo]).GetDigitalInput(point);
        }

        public IDigitalOutput GetDigitalOutput(uint axisNo, uint point)
        {
            return ((IONode)_nodesMap[axisNo]).GetDigitalOutput(point);
        }

        public IAnalogInput GetAnalogInput(uint axisNo, uint point)
        {
            return ((IONode)_nodesMap[axisNo]).GetAnalogInput(point);
        }

        public IAnalogOutput GetAnalogOutput(uint axisNo, uint point)
        {
            return ((IONode)_nodesMap[axisNo]).GetAnalogOutput(point);
        }

        public IAxis GetAxis(uint axisNo)
        {
            return ((IONode)_nodesMap[axisNo]).GetAxis();
        }

        // Internal methods ----------------------------------------------------
    }
}
