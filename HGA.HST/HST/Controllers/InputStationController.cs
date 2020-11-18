using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.Parsel.Device.PneumaticControl;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Utils;
using FisApi;
using System.Threading;
using XyratexOSC.Logging;
using System.Xml;
using Seagate.AAS.HGA.HST.Settings;
using System.IO;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class InputStationController : ControllerHST
    {
        public delegate void OnUpdateUnloadWOHandler(string carrierID);
        public event OnUpdateUnloadWOHandler OnUploadUnloadWO;

        private ISeaveyorZone _seaveyorInputStation;
        private SeaveyorZoneIO _seaveyorInputStationZoneIO;

        private ISeaveyorZone _seaveyorBufferStation;
        private SeaveyorZoneIO _seaveyorBufferStationZoneIO;        

        private HSTIOManifest _ioManifest;                
        private ReadWriteRFIDController _rfidController;
        private RFHead _rfHead;        

        private ILinearActuator _stopper;
        private ILinearActuator _lifter;
        private ILinearActuator _clamp;
        private ILinearActuator _clampRotary;

        private string name = "InputStationController";

        private IDigitalInput _diInputStationInPosition;
        private IDigitalInput _diBufferStationInPosition;
        private IDigitalInput _diInputStationStopperUp;
        private IDigitalInput _diInputStationStopperDown;
        private IDigitalInput _diInputStationLifterUp;
        private IDigitalInput _diInputStationLifterDown;
        private IDigitalInput _diInputStationClampForward;
        private IDigitalInput _diInputStationClampBackward;
        private IDigitalInput _diInputStationClampRotateCwOpen;
        private IDigitalInput _diInputStationClampRotateCcwClose;
        private IDigitalInput _diInputCarrierClampOpen;

        private IDigitalOutput _doInputStationStopperExtendUp;
        private IDigitalOutput _doInputStationLifterExtendUp;
        private IDigitalOutput _doInputStationLifterRetractDown;
        private IDigitalOutput _doInputStationClampDeploy;
        private IDigitalOutput _doInputStationClampRotate;

        private const uint _carrierInPositionTimeout = 3000;
        private const uint _zoneClearTimeout = 3000;
        private const uint nCylinderTimeout = 10000; //5000;	//ms
        private const uint nSensorTimeout = 100;	//ms
       
        private int _hstProcessNumber = 0;
        // Constructors ------------------------------------------------------------
        public InputStationController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            _workcell = workcell;            
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;

            _rfidController = new ReadWriteRFIDController(workcell, "RfidInput", "RfidInput", ReadWriteRFIDController.ReaderType.Fola, RFHead.Head1);
            _rfHead = Seagate.AAS.Parsel.Device.RFID.RFHead.Head1;
            _seaveyorInputStationZoneIO = new SeaveyorZoneIO();
            _seaveyorInputStationZoneIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.CIS_Inhibit);
            _seaveyorBufferStationZoneIO = new SeaveyorZoneIO();
            _seaveyorBufferStationZoneIO.inhibit = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.BBZ_Inhibit);


            //Seaveyor
            SeaveyorZone _seaveyorZone = new SeaveyorZone(_seaveyorInputStationZoneIO);
            _seaveyorInputStation = _seaveyorZone as ISeaveyorZone;
            _seaveyorInputStation.Simulation = (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            _seaveyorInputStation.Name = "Input Station Zone";

            SeaveyorZone _seaveyorBufferZone = new SeaveyorZone(_seaveyorBufferStationZoneIO);
            _seaveyorBufferStation = _seaveyorBufferZone as ISeaveyorZone;
            _seaveyorBufferStation.Simulation = (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation);
            _seaveyorBufferStation.Name = "Buffer Station Zone";


            //digital input
            _diInputStationInPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BIS_Position_On);
            _diBufferStationInPosition = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.BBZ_Position_On);
            _diInputStationStopperUp = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Stopper_Up);
            _diInputStationStopperDown  = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Stopper_Down);
            _diInputStationLifterUp  = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Up);
            _diInputStationLifterDown  = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Lifter_Down);
            _diInputStationClampForward  = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Deploy);
            _diInputStationClampBackward  = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Retract);
            _diInputStationClampRotateCwOpen = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Unlock);
            _diInputStationClampRotateCcwClose = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_CS_Lock);
            _diInputCarrierClampOpen = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Input_Carrier_Clamp_Sensor);

            //Digital Output
            _doInputStationStopperExtendUp = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Stopper);
            _doInputStationLifterExtendUp = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Up);
            _doInputStationLifterRetractDown = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_Lifter_Down);
            _doInputStationClampDeploy = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_CS_Deploy);
            _doInputStationClampRotate = _ioManifest.GetDigitalOutput((int)HSTIOManifest.DigitalOutputs.Input_CS_Rotate);


            //1. Stopper
            LinearActuator StopperActuator = new LinearActuator(_doInputStationStopperExtendUp, _diInputStationStopperUp, _diInputStationStopperDown, DigitalIOState.On);
            _stopper = StopperActuator as ILinearActuator;
            _stopper.Name = "Zone 5 stopper actuator";
            _stopper.ExtendedDirectionName = "Up";
            _stopper.ExtendedStateName = "Up";
            _stopper.RetractedDirectionName = "Down";
            _stopper.RetractedStateName = "Down";


            //2. Lifter
            LinearActuator lifterActuator = new LinearActuator(_doInputStationLifterExtendUp, _doInputStationLifterRetractDown, _diInputStationLifterUp, _diInputStationLifterDown);
            _lifter = lifterActuator as ILinearActuator;
            _lifter.Name = "Zone 5 lifter actuator";
            _lifter.ExtendedDirectionName = "Up";
            _lifter.ExtendedStateName = "Up";
            _lifter.RetractedDirectionName = "Down";
            _lifter.RetractedStateName = "Down";

            //3. Clamp
            LinearActuator ClampActuator = new LinearActuator(_doInputStationClampDeploy, _diInputStationClampForward, _diInputStationClampBackward, DigitalIOState.On);
            _clamp = ClampActuator as ILinearActuator;
            _clamp.Name = "Zone 5 clamp actuator";
            _clamp.ExtendedDirectionName = "Forward";
            _clamp.ExtendedStateName = "Forward";
            _clamp.RetractedDirectionName = "Backward";
            _clamp.RetractedStateName = "Backward";

            //4. ClampRotary
            LinearActuator ClampRotaryActuator = new LinearActuator(_doInputStationClampRotate, _diInputStationClampRotateCwOpen, _diInputStationClampRotateCcwClose, DigitalIOState.On);
            _clampRotary = ClampRotaryActuator as ILinearActuator;
            _clampRotary.Name = "Zone 5 clamp rotary actuator";
            _clampRotary.ExtendedDirectionName = "Cw Open";
            _clampRotary.ExtendedStateName = "Cw Open";
            _clampRotary.RetractedDirectionName = "Ccw Close";
            _clampRotary.RetractedStateName = "Ccw Close";
        }


        // Properties ----------------------------------------------------------
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public RFHead RfHead
        {
            get { return _rfHead; }
        }

        public ReadWriteRFIDController RfidController
        {
            get { return _rfidController; }
        }

        public ReadWriteRFIDController.RFIDState RfidInputStationState
        {
            get
            {
                if (_rfidController == null)
                    return ReadWriteRFIDController.RFIDState.Idle;
                return _rfidController.State;
            }
        }
        
        public Carrier IncomingCarrier { get; set; }
        public CarrierSettings IncomingCarrierSettings { get; set; }

        // Methods -------------------------------------------------------------
        public override void InitializeController()
        {            
            SetAction(1);
            try
            {
            }
            catch (Exception ex)
            {
                throw CreateControllerException(1, ex);
            }
        }
        public void InputLifterMoveDown()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
        }

        public void BoundaryCheck()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }
        }

        public void WaitInputStationPartPresent()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    _diInputStationInPosition.WaitForState(DigitalIOState.On, _carrierInPositionTimeout);
                    HSTMachine.Workcell.InputStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.InputStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.InputStationInPositionNotOnError, ex);
                }
            }
        }

        public void WaitInputStationPartCleared()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                try
                {
                    _diBufferStationInPosition.WaitForState(DigitalIOState.On, _carrierInPositionTimeout);
                    HSTMachine.Workcell.BufferStationBoatPositionError = false;
                }
                catch (Exception ex)
                {
                    HSTMachine.Workcell.InputStationBoatPositionError = true;
                    HSTException.Throw(HSTErrors.BufferStationInPositionNotOnError, ex);
                }

                return;
            }
        }

        public bool IsInputStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return false;
            }
            return (_diInputStationInPosition.Get() == DigitalIOState.On ? true : false);
        }

        public bool IsBufferStationHoldCarrier()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.DryRun && HSTMachine.Workcell.HSTSettings.Install.DryRunWithoutBoat))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                if (_workcell.IsCarrierInBufferZone)
                    return true;
                else
                    return false;
            }
            return (_diBufferStationInPosition.Get() == DigitalIOState.On ? true : false);
        }

        #region data handling
        public void ReadRfid()
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||                 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassRFIDReadAtInput == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            else
            {
                _rfidController.ReadFolaRFID(RFHead.Head1);
            }
        }        
        
        public void PackageRfidInputStationData()
        {
            _rfidController.FolaTagDataWriteInfor = _rfidController.FolaTagDataReadInfor;
            _rfidController.FolaTagDataWriteInfor.CarrierID = _rfidController.FolaTagDataReadInfor.CarrierID;
            _rfidController.FolaTagDataWriteInfor.WorkOrder = HSTMachine.Workcell.WorkOrder.Loading.WorkOrderNo;
            _rfidController.FolaTagDataWriteInfor.WorkOrderVersion = HSTMachine.Workcell.WorkOrder.Loading.Version[0];
            _rfidController.FolaTagDataWriteInfor.WriteCount = _rfidController.FolaTagDataReadInfor.WriteCount; // will be plus 1 in library
            _rfidController.FolaTagDataWriteInfor.LastStep = HSTMachine.Workcell.WorkOrder.Loading.LoadingFolaStep;

            for (int i = 0; i < _rfidController.FolaTagDataWriteInfor.MaxProcStep; i++)
            {
                if (i < HSTMachine.Workcell.WorkOrder.Loading.NumberOfFolaSteps)
                {
                    _rfidController.FolaTagDataWriteInfor.ProcStep[i].StationCode = HSTMachine.Workcell.WorkOrder.Loading.FolaProcessStepList[i][0];  // <-- ProcessStep2=C
                    _rfidController.FolaTagDataWriteInfor.ProcStep[i].ProcessRecipe = HSTMachine.Workcell.WorkOrder.Loading.FolaProgramNameList[i]; // <-- ProgramName2=SAS-1
                }
                else
                {
                    _rfidController.FolaTagDataWriteInfor.ProcStep[i].StationCode = ' ';
                    _rfidController.FolaTagDataWriteInfor.ProcStep[i].ProcessRecipe = "";
                }
            }
            return;
        }

        public bool CheckCarrierInformation(Carrier inputCarrier)
        {
            bool returnResult = false;
            try
            {
                if (HSTMachine.Workcell.WorkOrder.Loading.FolaProcessStepList[inputCarrier.RFIDData.RFIDTagData.LastStep][0] == Convert.ToChar(CommonFunctions.HST_STATION_CODE))
                {
                    _hstProcessNumber = inputCarrier.RFIDData.RFIDTagData.LastStep + 1;
                    returnResult = true;
                }
            }
            catch (Exception)
            {
                returnResult = false;
            }

            return returnResult;
        }
        public bool CheckCompareWorkOrderName(Carrier inputCarrier)
        {
            bool isCompared = false;
            if ((string.Compare(inputCarrier.RFIDData.RFIDTagData.WorkOrder, HSTMachine.Workcell.WorkOrder.Loading.WorkOrderNo, true) == 0) &&
                (string.Compare(inputCarrier.RFIDData.RFIDTagData.WorkOrderVersion.ToString(), HSTMachine.Workcell.WorkOrder.Loading.Version, true) == 0))
            {
                isCompared = true;
            }
            return isCompared;
        }
        public bool CheckCompareHSTRecipeName(Carrier inputCarrier)
        {
            bool returnResult = false;

            var carrierRecipeName = inputCarrier.WorkOrderData.RecipeFileName;
            var selectedRecipeName = HSTMachine.Workcell.SetupConfig.LastRunRecipeName;

            if (string.Compare(carrierRecipeName, selectedRecipeName, true) == 0)
                returnResult = true;
            else
            {
                CommonFunctions.Instance.RecipeChangeErrorMessage = string.Format("Machine recipe changed, New recipe is [{0}]",
                    carrierRecipeName);

                CommonFunctions.Instance.AlertRecipeChangeErrorMessage = string.Format("Machine recipe changed, Operator select recipe is [{0}] New recipe is [{1}]",
                    selectedRecipeName, carrierRecipeName);
            }

            return returnResult;
        }
        #endregion

        public void RaiseInputLifter(bool on, out uint lifterTimeUsed)
        {
            lifterTimeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _lifter.Extend(nCylinderTimeout, out lifterTimeUsed);//chanfg mask LinearActuator call and replace with direct IO call
                }
                catch (Exception ex)
                {
                    
                    HSTException.Throw(HSTErrors.InputStationLifterExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _lifter.Retract(nCylinderTimeout, out lifterTimeUsed);//chanfg mask LinearActuator call and replace with direct IO call
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationLifterRetractError, ex);
                }
            }

            return;
        }


        public void InputStationForwardClamp(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _clamp.Extend(nCylinderTimeout);
                }
                catch (Exception ex)
                {                   
                    HSTException.Throw(HSTErrors.InputStationCarrierScrewDriverExtendError, ex);
                }
                }
            else
            {
                try
                {
                    _clamp.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationCarrierScrewDriverRetractError, ex);
                }
            }
            return;
        }

        public void InputStationClampRotaryOpenCover(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation || 
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                try
                {
                    _clampRotary.Extend(nCylinderTimeout);
                }
                catch (Exception ex)
                {                   
                    HSTException.Throw(HSTErrors.InputStationCarrierScrewDriverRotateClockwiseError, ex);
                }
            }
            else
            {
                try
                {
                    _clampRotary.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationCarrierScrewDriverRotateCounterClockwiseError, ex);
               }
            }
        }

        public bool IsInputStationClampForward()
        {
            return (_diInputStationClampForward.Get() == DigitalIOState.On ? true : false);
        }

        public bool IsInputStationClampRotateCwOpen()
        {
            return (_diInputStationClampRotateCwOpen.Get() == DigitalIOState.On ? true : false);
        }

        public bool IsInputStationCarrierClampOpen()
        {
            return (_diInputCarrierClampOpen.Get() == DigitalIOState.On ? true : false);
        }

        public void RaiseInputStationStopper(bool on, out uint timeUsed)
        {
            timeUsed = 0;
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _stopper.Extend(nCylinderTimeout, out timeUsed);
                    _stopper.WaitForExtend(nCylinderTimeout);                    
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationStopperExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _stopper.Retract(nCylinderTimeout, out timeUsed);
                    _stopper.WaitForRetract(nCylinderTimeout);
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationStopperRetractError, ex);
                }
            }
        }

        public void InhibitInputStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                _seaveyorInputStation.Inhibit(true);
            }
            else
            {
                _seaveyorInputStation.Inhibit(false);
            }
            return;
        }

        public void InhibitBufferStation(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }
            if (on)
            {
                _seaveyorBufferStation.Inhibit(true);
            }
            else
            {
                _seaveyorBufferStation.Inhibit(false);
            }
            return;
        }

        public void UpdateDisplayUnloadWO(string CarrierID)
        {
            OnUploadUnloadWO(CarrierID);
        }

        public void CheckWorkOrder(string CarrierID)
        {
            UpdateDisplayUnloadWO(CarrierID);
            return;
        }

        public void LoadRecipeInfo(Carrier inputCarrier)
        {            
            //Teach Point Recipe
            string recipeFileName = CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType;
            Log.Info(this, "Carrier ID:{0}, IsPassThrough:{1}, Loaded automation recipe file:{2}.", inputCarrier.CarrierID, inputCarrier.IsPassThroughMode.ToString(), recipeFileName);
            
            if(HSTSettings.Instance.Directory.MachineRobotPointPath != CommonFunctions.UNKNOWN)
            {
                string TeachPointRecipeFilePath = HSTSettings.Instance.Directory.MachineRobotPointPath + "\\" + recipeFileName + HSTMachine.Workcell.TeachPointRecipe.Ext;

                if (!Directory.Exists(HSTSettings.Instance.Directory.MachineRobotPointPath))
                {
                    Directory.CreateDirectory(HSTSettings.Instance.Directory.MachineRobotPointPath);
                }

                if (!File.Exists(TeachPointRecipeFilePath))
                {
                    File.Create(TeachPointRecipeFilePath).Dispose();
                    return;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(TeachPointRecipeFilePath);

                    HSTMachine.Workcell.TeachPointRecipe.Load(doc);

                    HSTMachine.Workcell.TeachPointRecipe.Name = recipeFileName;
                    HSTMachine.Workcell.TeachPointRecipe.FullPath = TeachPointRecipeFilePath;
                }
            }
            else
            {
                throw new Exception("File Robot point not found, please check file or check configuration path");
            }
        }

        public Boolean AllHGAsFailed(Carrier inputCarrier)
        {
            for(int i = 0; i < 10; i++)
            {
                if(inputCarrier.RFIDData.RFIDTagData.HGAData[i].Status == CommonFunctions.TEST_PASS_CODE)
                    return false;                
            }

            return true;
        }

        public int NumberOfHGAWithoutSerialNo(Carrier inputCarrier)
        {
            int numberOfHGAWithoutSerialNo = 0;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[0].HgaSN) &&
                inputCarrier.Hga1.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga1.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[1].HgaSN) &&
                inputCarrier.Hga2.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga2.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[2].HgaSN) &&
                inputCarrier.Hga3.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga3.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[3].HgaSN) &&
                inputCarrier.Hga4.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga4.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[4].HgaSN) &&
                inputCarrier.Hga5.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga5.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[5].HgaSN) &&
                inputCarrier.Hga6.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga6.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[6].HgaSN) &&
                inputCarrier.Hga7.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga7.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[7].HgaSN) &&
                inputCarrier.Hga8.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga8.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[8].HgaSN) &&
                inputCarrier.Hga9.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga9.ForceToSampling = false;

            if (String.IsNullOrEmpty(inputCarrier.RFIDData.RFIDTagData.HGAData[9].HgaSN) &&
                inputCarrier.Hga10.Hga_Status == HGAStatus.HGAPresent)
                numberOfHGAWithoutSerialNo++;
            else
            inputCarrier.Hga10.ForceToSampling = false;

            return numberOfHGAWithoutSerialNo;
        }

        public void FreeInputLifter()
        {
            _doInputStationLifterExtendUp.Set(DigitalIOState.Off);
            _doInputStationLifterRetractDown.Set(DigitalIOState.Off);
        }

        public void RaiseInputLifter(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation ||
                (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Bypass && HSTMachine.Workcell.HSTSettings.Install.BypassInputAndOutputEEsPickAndPlace == true))
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _lifter.Extend(nCylinderTimeout);                  
                }
                catch (Exception ex)
                {                   
                    HSTException.Throw(HSTErrors.InputStationLifterExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _lifter.Retract(nCylinderTimeout);
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationLifterRetractError, ex);
                }
            }

            return;
        }

        public void RaiseInputStationStopper(bool on)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                Thread.Sleep(CommonFunctions.SIMULATION_DELAY);
                return;
            }

            if (on)
            {
                try
                {
                    _stopper.Extend(nCylinderTimeout);//chanfg mask LinearActuator call and replace with direct IO call
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationStopperExtendError, ex);
                }
            }
            else
            {
                try
                {
                    _stopper.Retract(nCylinderTimeout);//chanfg mask LinearActuator call and replace with direct IO call
                }
                catch (Exception ex)
                {                    
                    HSTException.Throw(HSTErrors.InputStationStopperRetractError, ex);
                }
            }
        }

        public void SaveWoOrRecipeChangeToLog(string type, string logDetail)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            string logtxt  = string.Empty;

            logtxt = string.Format("{0},{1},{2}", type, System.DateTime.Now.ToString("MM-dd-yyyy:HH:mm:ss"), logDetail);

            string logFilePath = HSTSettings.Instance.Directory.LogFilePath;
            logFilePath = logFilePath + "\\FileChanged-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "log";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(logtxt);
            log.Close();
        }

    }
}
