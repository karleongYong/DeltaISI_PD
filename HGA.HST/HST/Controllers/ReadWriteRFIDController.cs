//
//  ?Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.IO.Serial;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class ReadWriteRFIDController : ControllerHST
    {
        // Nested declarations -------------------------------------------------   
        public enum ReaderType
        {
            Bola,
            Fola
        }

        public enum Status
        {
            Pass = 'A',         // from RFID spec
            //Failed = 'F', // No need
            Empty = 'B'
        }

        public enum RFIDState
        {
            Reading,
            Writing,
            Idle
        }

        public enum RFIDErrors
        {
            NoError = 0,
            RFIDReadError = 1,
            StationCodeUnmatched = 2,
            AllHGAAreFailed = 3,
            FailedToLoadRecipe = 4,
            FailedRecipeRule = 5,
            ProcessStepNotDefined = 6
        }

        public delegate void RFIDFolaReadEventHandler(FolaTagData tag);
        public event RFIDFolaReadEventHandler OnRFIDFolaReadInputStationDone;
        public event RFIDFolaReadEventHandler OnRFIDFolaReadOutputStationDone;        

        public delegate void CarrierEventHandler(FolaTagData tag);
        public event CarrierEventHandler OnCarrierTransferToWorkZone;

        // Member variables ----------------------------------------------------

        private ReadWriteRFIDController _controller;
        private RFIDErrors _rfidError;
        private SerialPortSettings _portSetting;
        private RFIDState _state = RFIDState.Idle;

        private FolaTagData _readFolaTagData = new FolaTagData();
        private FolaTagData _writeFolaTagData = new FolaTagData();
        private FolaTagData.PartData[] _partData = new FolaTagData.PartData[10];


        private FolaReader _RFIDScanner;                
        private string _RFIDRecipeName;
        private bool _isReflowRequired = false;
        private bool _isRecipeChanged = false;

        // Constructors & Finalizers -------------------------------------------
        public ReadWriteRFIDController(HSTWorkcell workcell, string controllerID, string controllerName, ReaderType readerType, RFHead head)
            : base(workcell, controllerID, controllerName)
        {
            _workcell = workcell;               
            this._controllerID = controllerID;            
            // Rfid reader
            try
            {
                if (readerType == ReaderType.Fola)
                {
                    _RFIDScanner = HSTMachine.Workcell.RFIDScanner;                    
                }                
            }
            catch (Exception ex)
            {
                ExceptionRFID _exceptionRFID = new ExceptionRFID();
            }
        }

        public void Dispose()
        {
            try
            {

            }
            catch
            {
            }
        }

        // Properties ----------------------------------------------------------
        public FolaReader RFIDScanner
        {
            get { return _RFIDScanner; }
        }        

        public FolaTagData FolaTagDataReadInfor
        {
            get { return _readFolaTagData; }
        }

        public FolaTagData FolaTagDataWriteInfor
        {
            get { return _writeFolaTagData; }
            set { _writeFolaTagData = value; }
        }       

        public FolaTagData.PartData[] RFIDPartData
        {
            get { return _partData; }
        }

        public string RFIDRecipeName
        {
            get { return _RFIDRecipeName; }
        }

        public bool IsReflowRequired
        {
            get { return _isReflowRequired; }
            set { _isReflowRequired = value; }
        }

        public bool IsRecipeChanged
        { get { return _isRecipeChanged; } }

        public RFIDErrors RFIDError
        { get { return _rfidError; } set { _rfidError = value; } }

        public SerialPortSettings PortSetting
        { get { return _portSetting; } set { _portSetting = value; } }

        public RFIDState State
        { get { return _state; } }

        public int CarrierCount
        {
            get;
            set;
        }

        // Methods -------------------------------------------------------------
        public override void InitializeController()
        {
            try
            {
                _controller.InitializeController();
            }
            catch //(Exception ex)
            {
                // throw CreateControllerException(1);
            }
        }

        /// <summary>
        /// Read RFID
        /// </summary>
        public void ReadFolaRFID(RFHead RFIDHead)
        {
            _isReflowRequired = true;
            _rfidError = RFIDErrors.NoError;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            
            _state = RFIDState.Reading;
            if (RFIDHead == RFHead.Head1)
            {
                try
                {
                    _readFolaTagData = _RFIDScanner.ReadRFIDTag(RFHead.Head1);  // Read RFID from Input Station
                    _state = RFIDState.Idle;

                    if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            XyratexOSC.Logging.Log.Info(this, "Read RFID tag at Input Station for Carrier: {0}", _readFolaTagData.CarrierID);                            
                            XyratexOSC.Logging.Log.Info(this, "Carrier Size: {0}", _readFolaTagData.CarrierSize);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Station Code: {0}", _readFolaTagData.CurrentProcessStep.StationCode);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Recipe: {0}", _readFolaTagData.CurrentProcessStep.ProcessRecipe);
                            for (int x = 0; x < _readFolaTagData.CarrierSize; x++)
                                XyratexOSC.Logging.Log.Info(this, "HGA {0} Serial Number: {1}, Status: {2}, HGA Process Recipe: {3}, HGA Station Code: {4}", (x + 1), _readFolaTagData.HGAData[x].HgaSN, _readFolaTagData.HGAData[x].Status.ToString(), _readFolaTagData.ProcStep[x].ProcessRecipe, _readFolaTagData.ProcStep[x].StationCode.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Last Step: {0}", _readFolaTagData.LastStep);
                            XyratexOSC.Logging.Log.Info(this, "MaxProcStep: {0}", _readFolaTagData.MaxProcStep);

                            XyratexOSC.Logging.Log.Info(this, "Work Order: {0}", _readFolaTagData.WorkOrder);
                            XyratexOSC.Logging.Log.Info(this, "Work Order Version: {0}", _readFolaTagData.WorkOrderVersion.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Write Count: {0}", _readFolaTagData.WriteCount);
                        }

                        if (OnRFIDFolaReadInputStationDone != null)
                        {
                            OnRFIDFolaReadInputStationDone(_readFolaTagData);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    _state = RFIDState.Idle;
                    _rfidError = RFIDErrors.RFIDReadError;
                    HSTException.Throw(HSTErrors.InputRFIDReadFailed, ex);
                }
            }
            else if (RFIDHead == RFHead.Head2)
            {
                try
                {
                    _readFolaTagData = _RFIDScanner.ReadRFIDTag(RFHead.Head2);  // Read RFID from Output Station
                    _state = RFIDState.Idle;

                    if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            XyratexOSC.Logging.Log.Info(this, "Read RFID tag at Output Station for Carrier: {0}", _readFolaTagData.CarrierID);                            
                            XyratexOSC.Logging.Log.Info(this, "Carrier Size: {0}", _readFolaTagData.CarrierSize);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Station Code: {0}", _readFolaTagData.CurrentProcessStep.StationCode);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Recipe: {0}", _readFolaTagData.CurrentProcessStep.ProcessRecipe);
                            for (int x = 0; x < _readFolaTagData.CarrierSize; x++)
                                XyratexOSC.Logging.Log.Info(this, "HGA {0} Serial Number: {1}, Status: {2}, HGA Process Recipe: {3}, HGA Station Code: {4}", (x + 1), _readFolaTagData.HGAData[x].HgaSN, _readFolaTagData.HGAData[x].Status.ToString(), _readFolaTagData.ProcStep[x].ProcessRecipe, _readFolaTagData.ProcStep[x].StationCode.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Last Step: {0}", _readFolaTagData.LastStep);
                            XyratexOSC.Logging.Log.Info(this, "MaxProcStep: {0}", _readFolaTagData.MaxProcStep);

                            XyratexOSC.Logging.Log.Info(this, "Work Order: {0}", _readFolaTagData.WorkOrder);
                            XyratexOSC.Logging.Log.Info(this, "Work Order Version: {0}", _readFolaTagData.WorkOrderVersion.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Write Count: {0}", _readFolaTagData.WriteCount);
                        }

                        if (OnRFIDFolaReadOutputStationDone != null)
                        {
                            OnRFIDFolaReadOutputStationDone(_readFolaTagData);
                        }      
                    }
                }
                catch (Exception ex)
                {
                    _state = RFIDState.Idle;
                    _rfidError = RFIDErrors.RFIDReadError;
                    HSTException.Throw(HSTErrors.OutputRFIDReadError, ex);
                }
            }
                             
            return;
        }

        public void VerifyFolaTag()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }    // No need to verify tag during dry run mode
        }

		public bool CompareWorkorder(ref string previousWO, ref string currentWO)
        {
            bool isFirstBoat = false;

            currentWO = string.Format("{0}-{1}", _readFolaTagData.WorkOrder, _readFolaTagData.WorkOrderVersion);
            if (previousWO == "" || !HSTMachine.Workcell.WorkOrder.Unloading.IsLoaded)
            {
                previousWO = currentWO;
                isFirstBoat = true;
            }
            else
            {
                if (currentWO.Equals(previousWO, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
        }
		
            try
            {
                string serverFileName = Path.Combine(HSTMachine.Workcell.HSTSettings.Directory.TSRRecipeGlobalPath, string.Format("{0}-{1}.wo", _readFolaTagData.WorkOrder, _readFolaTagData.WorkOrderVersion));
                HSTMachine.Workcell.WorkOrder.LoadNewUnloadingWO(serverFileName);//It must be same

                previousWO = currentWO;
                return isFirstBoat; // IsFirstBoat return true , dont' need to prompt error, otherwise return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string CreateSerialNum(string CarrierId, int Slot, int WriteCycleCounter)
        {
            string retSerialNumber = null;

            string DataSlot = Slot.ToString("X");
            string DataWriteCycle = DecToBase36(WriteCycleCounter);

            int Dec = 0;

            List<char> StrData = new List<char>();
            List<int> DecData = new List<int>();

            if (DataWriteCycle.Length < 5)
            {
                int LengthCnt = DataWriteCycle.Length;

                for (int i = LengthCnt; i < 5; ++i)
                {
                    DataWriteCycle = "0" + DataWriteCycle;
                }
            }

            string DataTemp = CarrierId + DataSlot + DataWriteCycle;

            StrData.Clear();
            DecData.Clear();

            for (int i = 0; i < 9; ++i)
            {
                string StrTmp = DataTemp.Substring(i, 1);
                int DecTmp = (int)Base36ToDec(StrTmp);

                Dec = Dec + DecTmp;
            }

            string CheckSum = DecToBase36(Dec % 36);

            retSerialNumber = CarrierId + DataSlot + DataWriteCycle + CheckSum;

            return retSerialNumber;
        }

        private string DecToBase36(int num)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int nbase = 36;

            // check if we can convert to another base
            if (nbase < 2 || nbase > chars.Length)
                return "";

            int r;
            string newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                r = num % nbase;
                newNumber = chars[r] + newNumber;
                num = num / nbase;
            }
            // the last number to convert
            newNumber = chars[num] + newNumber;

            return newNumber;
        }

        private long Base36ToDec(string number)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int nbase = 36;

            if (nbase < 2 || nbase > chars.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " +
                    chars.Length.ToString());

            if (String.IsNullOrEmpty(number))
                return 0;

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = chars.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException(
                        "Invalid character in the arbitrary numeral system number",
                        "number");

                result += digit * multiplier;
                multiplier *= nbase;
            }

            return result;
        }

        public void VerifyRfidFolaTag()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }    // No need to verify tag during dry run mode
            if (HSTMachine.Workcell.CalibrationSettings.RFID.RfidWriteCounterLimit == 0)
                return;
            if (_readFolaTagData.WriteCount > HSTMachine.Workcell.CalibrationSettings.RFID.RfidWriteCounterLimit)
                throw (new Exception("RFID Out of write counter limit"));
            else
                return;
        }        

        public void WriteFolaRFID()
        {
            //SetAction(9);
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDAndSeatrackWriteAtOutput == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    if (HSTMachine.Workcell.HSTSettings.Install.DataLoggingForRFIDAndSeatrackRecordUpdateEnabled)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                        {
                            XyratexOSC.Logging.Log.Info(this, "Write RFID tag at Output Station for Carrier: {0}", _writeFolaTagData.CarrierID);                            
                            XyratexOSC.Logging.Log.Info(this, "Carrier Size: {0}", _writeFolaTagData.CarrierSize);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Station Code: {0}", _writeFolaTagData.CurrentProcessStep.StationCode);
                            XyratexOSC.Logging.Log.Info(this, "Current Process Recipe: {0}", _writeFolaTagData.CurrentProcessStep.ProcessRecipe);
                            for (int x = 0; x < _writeFolaTagData.CarrierSize; x++)
                                XyratexOSC.Logging.Log.Info(this, "HGA {0} Serial Number: {1}, Status: {2}, HGA Process Recipe: {3}, HGA Station Code: {4}", (x + 1), _writeFolaTagData.HGAData[x].HgaSN, _writeFolaTagData.HGAData[x].Status.ToString(), _writeFolaTagData.ProcStep[x].ProcessRecipe, _writeFolaTagData.ProcStep[x].StationCode.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Last Step: {0}", _writeFolaTagData.LastStep);
                            XyratexOSC.Logging.Log.Info(this, "MaxProcStep: {0}", _writeFolaTagData.MaxProcStep);

                            XyratexOSC.Logging.Log.Info(this, "Work Order: {0}", _writeFolaTagData.WorkOrder);
                            XyratexOSC.Logging.Log.Info(this, "Work Order Version: {0}", _writeFolaTagData.WorkOrderVersion.ToString());
                            XyratexOSC.Logging.Log.Info(this, "Write Count: {0}", _writeFolaTagData.WriteCount);
                        }
                    }
                
                
                    _state = RFIDState.Writing;
                    _RFIDScanner.WriteRFIDTag(RFHead.Head2, _writeFolaTagData, false);
                    _state = RFIDState.Idle;

                }
                catch (Exception ex)
                {
                    _state = RFIDState.Idle;                
                    HSTException.Throw(HSTErrors.OutputRFIDWriteError, ex);
                }
            }
        }
        
        // Event handlers ------------------------------------------------------

        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------

        protected override void AddControllerError()
        {

            //Detailed Controller Error need to be added later
            string str = _controllerName + " failed to ";
            AddError(1, str + "initialize. Check for inner exception and remedy it. Shutdown and reload application to recover.");
            AddError(2, str + "read RFID.");
            AddError(3, _controllerName + " has station code unmatched error.");
            AddError(4, _controllerName + " has All HGAs were failed error.");
            AddError(5, str + "load recipe based on recipe name read from RFID tag.");

            AddError(9, "write RFID.");
        }
       
    }
}
