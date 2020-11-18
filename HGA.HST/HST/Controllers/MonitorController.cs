using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Hw;
using System.Threading;
using System.Collections;
using System.IO;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Data;
using System.Windows.Forms;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class MonitorController : ControllerHST
    {

        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
        private bool _isDoorLockedAndRecovered = false;
        private bool _recorded = false;
        private Hashtable _axesCommandPositionMap;

        private OmronSafetyController _safetyController;

        private IDigitalInput _diGroundMasterOk;
        private bool _criticalTriggeringActivae = false;

        //Axis
        IAxis _inputEndEffectorZAxis;
        IAxis _outputEndEffectorZAxis;
        IAxis _testProbeZAxis;

        IAxis _precisorNestXAxis;
        IAxis _precisorNestYAxis;
        IAxis _precisorNestThetaAxis;
       
        // Constructors ------------------------------------------------------------
        public MonitorController(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            _workcell = workcell;
            _ioManifest = (HSTIOManifest)workcell.IOManifest;
            _safetyController = new OmronSafetyController(workcell, "SafetyController", "SafetyController");

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
            {
                _diGroundMasterOk = _ioManifest.GetDigitalInput((int)HSTIOManifest.DigitalInputs.Ground_Master);
                _inputEndEffectorZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z1);
                _outputEndEffectorZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z3);
                _testProbeZAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Z2);

                _precisorNestXAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.X);
                _precisorNestYAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Y);
                _precisorNestThetaAxis = _ioManifest.GetAxis((int)HSTIOManifest.Axes.Theta);
                _axesCommandPositionMap = new Hashtable();
            }
        }


        // Properties ----------------------------------------------------------
        public bool IsDoorLockedAndRecovered { get { return _isDoorLockedAndRecovered; } }
        public bool IsCriticalTriggeringActivated
        {
            get { return _criticalTriggeringActivae; }
            set { _criticalTriggeringActivae = value; }
        }

        //Method
        public override void InitializeController()
        {

        }

        public void SendRequestToSafetyController(bool towerlightAmberOn, bool towerLightRedOn, bool sirenOn,
            bool powerLEDOn, bool AutomationRunLEDOn, bool AutomationStopLEDOn, bool StopAutomation, bool PowerOnConveyor, bool PowerOffConveyor)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            _safetyController.SendRequestToSafetyController(towerlightAmberOn, towerLightRedOn, sirenOn,
                powerLEDOn, AutomationRunLEDOn, AutomationStopLEDOn, StopAutomation, PowerOnConveyor, PowerOffConveyor);
        }

        public bool isEMOPressed()
        {
            return (_safetyController != null) ? _safetyController.EMOPressed: false;
        }

        public bool isPowerButtonPressed()
        {
            return (_safetyController != null) ? _safetyController.PowerButtonPressed : false;
        }

        public bool isAutomationEnabledButtonPressed()
        {
            return (_safetyController != null) ? _safetyController.AutomationEnabledButtonPressed : false;
        }

        public bool isAutomationStopButtonPressed()
        {
            return (_safetyController != null) ? _safetyController.AutomationStopButtonPressed: false;
        }

        public bool isDoorLocked()
        {
            return (_safetyController != null) ? _safetyController.DoorLocked : false;
        }

        public bool isPowerOn()
        {
            return (_safetyController != null) ? _safetyController.PowerOn : false;
        }

        public bool isAutomationEnabled()
        {
            return (_safetyController != null) ? _safetyController.AutomationEnabled : false;
        }

        public bool isConveyorEnabled()
        {
            return (_safetyController != null) ? _safetyController.ConveyorEnabled : false;
        }

        public bool isAutomationPrepareToStop()
        {
            return (_safetyController != null) ? _safetyController.AutomationPrepareToStop : false;
        }

        public bool isGroundMasterWorking()
        {
            return (_diGroundMasterOk.Get() == DigitalIOState.On);
        }

        public List<string> getErrorList()
        {
            return (_safetyController != null) ? _safetyController.errorList : new List<string>();
        }


        public void EnableAllEndEffectorAxes(bool enable)
        {
            _inputEndEffectorZAxis.Enable(enable);
            _outputEndEffectorZAxis.Enable(enable);
            _testProbeZAxis.Enable(enable);

            _precisorNestXAxis.Enable(enable);
            _precisorNestYAxis.Enable(enable);
            _precisorNestThetaAxis.Enable(enable);
        }

        public void MachineDoorLock(bool doorlock)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Simulation)
            {
                return;
            }

            if (doorlock)
            {
                try
                {
                    if (!isDoorLocked())
                        throw new Exception("Door is not locked. Please close door properly.");
                    
                    EnableAllEndEffectorAxes(false);
                    EnableAllEndEffectorAxes(true);
                    _isDoorLockedAndRecovered = true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Machine Door Lock Error.", ex);
                }
            }
            else
            {
                // Stop machine first.
                HSTMachine.Workcell.Process.Stop();                
                _isDoorLockedAndRecovered = false;
            }
        }        
        
        protected override void AddControllerError()
        {

            //Detailed Controller Error need to be added later

        }

        

    }
}
