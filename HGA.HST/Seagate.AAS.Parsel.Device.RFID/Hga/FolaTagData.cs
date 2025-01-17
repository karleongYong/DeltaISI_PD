//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/09/20] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
    [Serializable]
    public class FolaTagData : RFIDTagData
    {
        [Serializable]
        public class PartData
        {
            //private int _slot;
            //private char _incomingStatus;
            //private char _outgoingStatus;
            //private string _hgaSN;
            private char _status;
            private string _hgaSN;

            public PartData()
            {
                Clear();
            }

            public PartData(PartData data) // use to clone
            {
                this._status = data._status;
                this._hgaSN = data._hgaSN;
            }

            //public int Slot
            //{
            //    get { return _slot; }
            //    set { _slot = value; }
            //}

            //public char IncomingStatus
            //{
            //    get { return _incomingStatus; }
            //    set { _incomingStatus = value; }
            //}

            //public char OutgoingStatus
            //{
            //    get { return _outgoingStatus; }
            //    set { _outgoingStatus = value; }
            //}

            //public string HgaSN
            //{
            //    get { return _hgaSN; }
            //    set { _hgaSN = value; }
            //}

            public char Status
            {
                get { return _status; }
                set { _status = value; }
            }

            public string HgaSN
            {
                get { return _hgaSN; }
                set { _hgaSN = value; }
            }

            public void Clear()
            {
                //_slot = -1;
                //_incomingStatus = 'B';
                //_outgoingStatus = 'B';
                //_hgaSN = "";
                _status = 'B';
                _hgaSN = "";
            }
        }

        [Serializable]
        // Nested declarations -------------------------------------------------
        public class ProcessStep
        {
            //private int _stepNum;
            private char _stationCode;
            private string _processRecipe;

            public ProcessStep()
            {
                Clear();
            }

            //public int StepNumber
            //{
            //    get { return _stepNum; }
            //    set { _stepNum = value; }
            //}

            public char StationCode
            {
                get { return _stationCode; }
                set { _stationCode = value; }
            }

            public string ProcessRecipe
            {
                get { return _processRecipe; }
                set { _processRecipe = value; }
            }

            public void Clear()
            {
                //_stepNum = -1;
                _stationCode = '?';
                _processRecipe = "";
            }
        }

        // Member variables ----------------------------------------------------

        private string _carrierID = "";
        private string _workOrder = "";
        private char _workOrderVersion = '0';
        private byte _checkSum = 0;
        private int _writeCount = 0;

        private int _lastStep = 0;
        private const int _MAXSTEP = 12;       // this is also set to something in the RFID library - we're not really sure what...

        private ProcessStep[] _processSteps;
        private PartData[] _hgasInCarrier;

        private const int _maxProcStep = 14;
        private const int _carrierSize = 10;

        private const int checkSumByteNo = 13;
        private const int carrierIDByteNo = 1;

        // Constructors & Finalizers -------------------------------------------

        public FolaTagData()
            : base(checkSumByteNo, carrierIDByteNo)
        {
            _processSteps = new ProcessStep[_maxProcStep];
            _hgasInCarrier = new PartData[_carrierSize];

            // initialize content
            for (int slot = 0; slot < _carrierSize; slot++)
            {
                _hgasInCarrier[slot] = new PartData();
                //_hgasInCarrier[slot].Slot = slot;
            }
            for (int step = 0; step < _maxProcStep; step++)
            {
                _processSteps[step] = new ProcessStep();
                //_processSteps[step].StepNumber = step;
            }
            Clear();
        }

        // Properties ----------------------------------------------------------

        public PartData this[int index]
        {
            get
            {
                //if (_hgasInCarrier.Contains(index))
                return _hgasInCarrier[index];
                //else
                //    return null;
            }
        }

        public string WorkOrder
        {
            get { return _workOrder; }
            set { _workOrder = value; }
        }

        public char WorkOrderVersion
        {
            get { return _workOrderVersion; }
            set { _workOrderVersion = value; }
        }

        public int LastStep
        {
            get { return _lastStep; }
            set { if (value <= _MAXSTEP) _lastStep = value; }
        }

        public ProcessStep CurrentProcessStep
        {
            get { return _processSteps[_lastStep + 1]; }
        }

        public string CarrierID
        {
            get { return _carrierID; }
            set { _carrierID = value; }
        }

        public int CarrierSize
        { get { return _carrierSize; } }

        public int MaxProcStep
        { get { return _maxProcStep; } }

        public int WriteCount
        {
            get { return _writeCount; }
            set { _writeCount = value; }
        }

        public byte CheckSum
        {
            get { return _checkSum; }
            set { _checkSum = value; }
        }

        public byte CheckSumCalculate
        {
            get { return _checkSumCalculate; }
        }

        public PartData[] HGAData
        {
            get { return _hgasInCarrier; }
        }

        public ProcessStep[] ProcStep
        {
            get
            {
                return _processSteps;
            }
        }

        // Methods -------------------------------------------------------------

        public PartData SetHGAData(int slot, StringBuilder hgaSN, char status)
        {
            return SetHGAData(slot, hgaSN.ToString(), status);
        }

        public PartData SetHGAData(int slot, string hgaSN, char status)
        {
            _hgasInCarrier[slot].HgaSN = hgaSN;
            //_hgasInCarrier[slot].IncomingStatus = status;
            _hgasInCarrier[slot].Status = status;
            return _hgasInCarrier[slot];
        }

        public ProcessStep SetProcessStepData(int step, char stationCode, StringBuilder processRecipe)
        {
            return SetProcessStepData(step, stationCode, processRecipe.ToString());
        }

        public ProcessStep SetProcessStepData(int step, char stationCode, string processRecipe)
        {
            _processSteps[step].StationCode = stationCode;
            _processSteps[step].ProcessRecipe = processRecipe.ToString();
            return _processSteps[step];
        }

        public void Clear()
        {
            _carrierID = "";
            _workOrder = "";
            _workOrderVersion = '0';
            _checkSum = 0;
            _writeCount = 0;
            _lastStep = 0;

            for (int slot = 0; slot < _carrierSize; slot++)
            {
                _hgasInCarrier[slot].Clear();
            }
            for (int step = 0; step < _maxProcStep; step++)
            {
                _processSteps[step].Clear();
            }
        }

        /// <summary>
        /// Performs Deep Copy
        /// </summary>
        /// <returns></returns>
        public void Clone(FolaTagData tagSource)
        {
            if (tagSource == null) return;

            _carrierID = tagSource.CarrierID;
            _workOrder = tagSource.WorkOrder;
            _workOrderVersion = tagSource.WorkOrderVersion;
            _checkSum = tagSource.CheckSum;
            _checkSumCalculate = tagSource.CheckSumCalculate;
            _lastStep = tagSource.LastStep;
            _writeCount = tagSource.WriteCount;

            for (int slot = 0; slot < _hgasInCarrier.Length; slot++)
                SetHGAData(slot, tagSource[slot].HgaSN, tagSource[slot].Status);

            for (int step = 0; step < _processSteps.Length; step++)
                SetProcessStepData(step, tagSource.ProcStep[step].StationCode, tagSource.ProcStep[step].ProcessRecipe);
        }

        public override void ExtractReadData(string rfidData)
        {
            Clear();
            _carrierID = rfidData.Substring(1, 3);
            _workOrder = rfidData.Substring(4, 8);
            _workOrderVersion = rfidData[12];
            _checkSum = (byte)rfidData[checkSumByteNo];
            _writeCount = (rfidData[16] * 65536) + (rfidData[15] * 256) + rfidData[14];
            _lastStep = rfidData[17];
            
            int byteNo = 18;
            for (int slot = 0; slot < _carrierSize; slot++)
                _hgasInCarrier[slot].Status = rfidData[byteNo++];
            for (int slot = 0; slot < _carrierSize; slot++)
            {
                _hgasInCarrier[slot].HgaSN = rfidData.Substring(byteNo, 10).Trim();
                byteNo += 10;
            }

            for (int step = 0; step < _maxProcStep; step++)
                _processSteps[step].StationCode = rfidData[byteNo++];
            for (int step = 0; step < _maxProcStep; step++)
            {
                _processSteps[step].ProcessRecipe = rfidData.Substring(byteNo, 8).Trim();
                byteNo += 8;
            }
        }

        public override string GetWriteString(out List<int> ascii13ByteNoList)
        {
            ascii13ByteNoList = new List<int>();
            char[] writeCycleByteData = GetWriteCycleByteData();

            StringBuilder writeStr = new StringBuilder("0"); // Write Protect
            writeStr.Append(TrimWriteString(_carrierID, 3));
            writeStr.Append(TrimWriteString(_workOrder, 8));
            writeStr.Append(_workOrderVersion.ToString());
            writeStr.Append("0"); // temp check sum
            writeStr.Append(writeCycleByteData[0]);
            writeStr.Append(writeCycleByteData[1]);
            writeStr.Append(writeCycleByteData[2]);
            writeStr.Append((char)_lastStep);

            for (int slot = 0; slot < _carrierSize; slot++)
                writeStr.Append(_hgasInCarrier[slot].Status);
            for (int slot = 0; slot < _carrierSize; slot++)
                writeStr.Append(TrimWriteString(_hgasInCarrier[slot].HgaSN, 10));

            for (int step = 0; step < _maxProcStep; step++)
                writeStr.Append(_processSteps[step].StationCode);
            for (int step = 0; step < _maxProcStep; step++)
                writeStr.Append(TrimWriteString(_processSteps[step].ProcessRecipe, 8));

            byte calCS = CalculateCheckSum(writeStr.ToString());
            writeStr[checkSumByteNo] = (char)calCS;
            
            // Ascii 13 , need to rewrite by Hex Command, otherwise write by ASCII
            if (calCS == 13)
                ascii13ByteNoList.Add(checkSumByteNo);
            for (int i = 0; i < writeCycleByteData.Length; i++)
            {
                if ((int)writeCycleByteData[i] == 13)
                {
                    ascii13ByteNoList.Add(14 + i);
                    writeStr[14 + i] = '0';
                }
            }
            if (_lastStep == 13)
            {
                ascii13ByteNoList.Add(17);
                writeStr[17] = '0';
            }
            // ------------------------------------

            return writeStr.ToString();
        }

        public override bool IsCorrectID(string id)
        {
            return IsCorrectCarrierID(id);
        }

        private char[] GetWriteCycleByteData()
        {
            char[] wcByteData = new char[3];

            wcByteData[2] = (char)(_writeCount / 65536);
            int wrTmp = _writeCount - (65536 * (int)wcByteData[2]);
            wcByteData[1] = (char)(wrTmp / 256);
            wcByteData[0] = (char)(wrTmp % 256);
            
            return wcByteData;
        }

        private bool IsCorrectCarrierID(string id)
        {
            return id.Equals(_carrierID) ? true : false;
        }
    }
}
