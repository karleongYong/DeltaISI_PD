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
//  [2007/09/28] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Seagate.AAS.Parsel.Device.RFID.Hga
{
    /// <summary>
    /// Summary description for RFIDTag.
    /// </summary>
    public class BolaTagData : RFIDTagData
    {
        public class BolaPartData
        {
            //private int _slot;
            private char _status;
            private string _hgaSN;

            public BolaPartData()
            {
                Clear();
            }

            //public int Slot
            //{
            //    get { return _slot; }
            //    set { _slot = value; }
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
                _status = 'B';
                _hgaSN = "";
            }
        }

        // Member variables ----------------------------------------------------

        private string _workOrder = "";
        private char _workOrderVersion = '0';
        private byte _checkSum = 0;
        private int _lastStep = 0;
        private int _vendorCode = 0;
        private string _partNumber = "";
        private char _tab = 'U'; // U, D
        private string _trayID = "";
        private int _trayType = 0;

        private BolaPartData[] _hgasInTray;

        //private const int _maxProcStep = 14;
        private const int _traySize = 20;

        private const int checkSumByteNo = 10;
        private const int trayIDByteNo = 243;

        // Constructors & Finalizers -------------------------------------------

        public BolaTagData() : base(checkSumByteNo, trayIDByteNo)
        {
            _hgasInTray = new BolaPartData[_traySize];

            // initialize content
            for (int slot = 0; slot < _traySize; slot++)
            {
                _hgasInTray[slot] = new BolaPartData();
               // _hgasInTray[slot].Slot = slot;
            }
            Clear();
        }

        // Properties ----------------------------------------------------------

        public BolaPartData this[int index]
        {
            get
            {
                //if (_hgasInCarrier.Contains(index))
                return _hgasInTray[index];
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

        public byte CheckSum
        {
            get { return _checkSum; }
            set { _checkSum = value; }
        }

        public byte CheckSumCalculate
        {
            get { return _checkSumCalculate; }
        }

        public int LastStep
        {
            get { return _lastStep; }
            set { _lastStep = value; }
        }

        public int VendorCode
        {
            get { return _vendorCode; }
            set { _vendorCode = value; }
        }

        public string PartNumber
        {
            get { return _partNumber; }
            set { _partNumber = value; }
        }

        /// <summary>
        /// U or D only
        /// </summary>
        public char Tab
        {
            get { return _tab; }
            set { _tab = value; }
        }

        public string TrayID
        {
            get { return _trayID; }
            set { _trayID = value; }
        }

        public int TrayType
        {
            get { return _trayType; }
            set { _trayType = value; }
        }

        public int TraySize
        { get { return _traySize; } }

        public BolaPartData[] HGAData
        {
            get { return _hgasInTray; }
        }

        //public int MaxProcStep
        //{ get { return _maxProcStep; } }

        // Methods -------------------------------------------------------------

        public BolaPartData SetHGAData(int slot, StringBuilder hgaSN, char status)
        {
            return SetHGAData(slot, hgaSN.ToString(), status);
        }

        public BolaPartData SetHGAData(int slot, string hgaSN, char status)
        {
            _hgasInTray[slot].HgaSN = hgaSN;
            _hgasInTray[slot].Status = status;
            return _hgasInTray[slot];
        }

        public void Clear()
        {
            _trayID = "";
            _workOrder = "";
            _workOrderVersion = '0';
            _checkSum = 0;
            _lastStep = 0;
            _vendorCode = '0';
            _partNumber = "";
            _tab = '?';
            _trayID = "";
            _trayType = 0;

            for (int slot = 0; slot < _traySize; slot++)
            {
                _hgasInTray[slot].Clear();
            }
        }

        /// <summary>
        /// Creates a new BolaTagData that is a copy of the current instance.
        /// </summary>
        /// <returns>A new BolaTagData that is a copy of this instance.</returns>
        public BolaTagData Clone()
        {
            var newTag = new BolaTagData();
            newTag.Copy(this);
            return newTag;
        }

        /// <summary>
        /// Performs Deep Copy
        /// </summary>
        [Obsolete("Please use Copy method instead of this Clone method.")]
        public void Clone(BolaTagData tagSource)
        {
            Copy(tagSource);
        }

        /// <summary>
        /// Performs Deep Copy
        /// </summary>
        public void Copy(BolaTagData tagSource)
        {
            if (tagSource == null) return;

            _workOrder = tagSource.WorkOrder;
            _workOrderVersion = tagSource.WorkOrderVersion;
            _checkSum = tagSource.CheckSum;
            _checkSumCalculate = tagSource.CheckSumCalculate;
            _lastStep = tagSource.LastStep;
            _vendorCode = tagSource.VendorCode;
            _partNumber = tagSource.PartNumber;
            _tab = tagSource.Tab;
            _trayID = tagSource.TrayID;
            _trayType = tagSource.TrayType;

            for (int slot = 0; slot < _hgasInTray.Length; slot++)
                SetHGAData(slot, tagSource[slot].HgaSN, tagSource[slot].Status);
        }

        public override void ExtractReadData(string rfidData)
        {
            Clear();
            _workOrder = rfidData.Substring(1, 8);
            _workOrderVersion = rfidData[9];
            _checkSum = (byte)rfidData[checkSumByteNo];
            _lastStep = rfidData[11];

            int byteNo = 12;
            for (int slot = 0; slot < _traySize; slot++)
                _hgasInTray[slot].Status = rfidData[byteNo++];
            for (int slot = 0; slot < _traySize; slot++)
            {
                _hgasInTray[slot].HgaSN = rfidData.Substring(byteNo, 10).Trim();
                byteNo += 10;
            }
            _vendorCode = (int)rfidData[232];
            _partNumber = rfidData.Substring(233, 9).Trim();
            _tab = rfidData[242];
            _trayID = rfidData.Substring(243, 3);
            _trayType = (int)rfidData[246];
        }

        public override string GetWriteString(out List<int> ascii13ByteNoList)
        {
            ascii13ByteNoList = new List<int>();

            StringBuilder writeStr = new StringBuilder("0"); // Write Protect
            writeStr.Append(TrimWriteString(_workOrder, 8));
            writeStr.Append(_workOrderVersion.ToString());
            writeStr.Append("0"); // temp check sum
            writeStr.Append((char)_lastStep);
            for (int slot = 0; slot < _traySize; slot++)
                writeStr.Append(_hgasInTray[slot].Status);
            for (int slot = 0; slot < _traySize; slot++)
                writeStr.Append(TrimWriteString(_hgasInTray[slot].HgaSN, 10));
            writeStr.Append((char)_vendorCode);
            writeStr.Append(TrimWriteString(_partNumber, 9));
            writeStr.Append(_tab);
            writeStr.Append(TrimWriteString(_trayID, 3));
            writeStr.Append((char)_trayType);
            writeStr.Append("\0\0\0\0\0\0\0");

            byte calCS = CalculateCheckSum(writeStr.ToString());
            writeStr[checkSumByteNo] = (char)calCS;

            // Ascii 13 , need to rewrite by Hex Command, otherwise write by ASCII
            if (calCS == 13)
                ascii13ByteNoList.Add(checkSumByteNo);
            if (_lastStep == 13)
            {
                ascii13ByteNoList.Add(11);
                writeStr[11] = '0';
            }
            if (_vendorCode == 13)
            {
                ascii13ByteNoList.Add(232);
                writeStr[232] = '0';
            }
            // ------------------------------------

            return writeStr.ToString();
        }

        public override bool IsCorrectID(string id)
        {
            return IsCorrectTrayID(id);
        }

        private bool IsCorrectTrayID(string id)
        {
            return id.Equals(_trayID) ? true : false;
        }
    }
}
