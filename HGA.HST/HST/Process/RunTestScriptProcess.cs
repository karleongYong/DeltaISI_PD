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
using System.Threading;
using System.IO;

using qf4net;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Equipment.HGA.UI.Utils;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;

//using Seagate.AAS.HGA.Tbp.RFL.UI;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Exceptions;
using System.Windows.Forms;
//using Seagate.AAS.Parsel.Device.ADLink8164;
//using Seagate.AAS.Parsel.Device.LightTower;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using XyratexOSC.XMath;

namespace Seagate.AAS.HGA.HST.Process
{
    /// <summary>
    /// Execute test script commands.
    /// </summary>
    public class RunTestScriptProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        protected QState stateMonitoring;

        private HSTIOManifest ioManifest;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;
        private MonitorController _controller;

        private string[] _commandLines;

        private Queue<string> CommandQueue
        {
            get;
            set;
        }

        private int _minPrecisorJogX = 0;
        private int _minPrecisorJogY = 0;
        private int _minPrecisorJogTheta = 0;
        private int _maxPrecisorJogX = 0;
        private int _maxPrecisorJogY = 0;
        private int _maxPrecisorJogTheta = 0;
        private int _stepPrecisorStepIncrementX = 0;
        private int _stepPrecisorStepIncrementY = 0;
        private int _stepPrecisorStepIncrementTheta = 0;
        private int _maxAlignmentCameraFiducialRead = 0;
        private int _hgaProductTailType = 0;
        private int _hgaProductTabType = 0;
        private int _xAxisOffsetPlusMinus = 0;
        private int _yAxisOffsetPlusMinus = 0;
        private double _thetaAxisOffsetPlusMinus = 0.03;


        // Constructors & Finalizers -------------------------------------------
        public RunTestScriptProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this.ioManifest = (HSTIOManifest)workcell.IOManifest;
            _isNonIdleProcess = true;

            // initialize HSM delegates
            stateMonitoring = new QState(this.Monitoring);
        }

        public MonitorController Controller
        {
            get { return _controller; }
        }

        // Methods -------------------------------------------------------------
        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()

//            _controller = new MonitorController(_workcell, "MM", "Monitor Controller");
//            _controller.InitializeController();
            base.Start(priority);
        }

        public override void Dispose()
        {
//            MonitorErrorMessage.Dispose();
            base.Dispose();
        }

        // Internal Methods -------------------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigRunTestScript);  
            base.InitializeStateMachine();
            TransitionTo(stateMonitoring);
        }

        protected QState Monitoring(IQEvent qEvent)
        {

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigRunTestScript))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigRunTestScript);
                    return null;
                }
                return null;
            }
            if (qEvent.IsSignal(HSTWorkcell.SigRunTestScript))
            {
                ResetAllVariables();
                _commandLines = System.IO.File.ReadAllLines(HSTMachine.Workcell.HSTSettings.Install.TestScript);

                CommandQueue = new Queue<string>();

                for (int i = 0; i < _commandLines.Length; i++)
                    CommandQueue.Enqueue(_commandLines[i]);
                
                ProcessNextCommand();

                return null;
            }
            return base.TopState;
        }

        private void ProcessNextCommand()
        {
            if (HSTWorkcell.stopRunScript)
            {
                HSTWorkcell.stopRunScript = false;
                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel(), () =>
                            {
                                HSTMachine.Instance.MainForm.getPanelCommand().EnableTestScriptRun();
                            });
                        }
                    }
                }

                MessageBox.Show("Test Stopped.");
            }
            else if (CommandQueue.Count > 0)
                //Lai: Testing purpose
                ExecuteCommand(CommandQueue.Dequeue());
            else
            {
                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationMainPanel(), () =>
                            {
                                HSTMachine.Instance.MainForm.getPanelCommand().EnableTestScriptRun();
                            });
                        }
                    }
                }
                MessageBox.Show("Test Completed.");
            }
        }

        private void ExecuteCommand(string command)
        {
            try
            {
                //            MessageBox.Show(command);
                if (command.Contains("#"))
                {
                    //Comment line. doing nothing.
                }
                else if (command.Equals("Home Input EE"))
                    _workcell.Process.InputEEProcess.Controller.HomeZAxis();
                else if (command.Equals("Home output EE"))
                    _workcell.Process.OutputEEProcess.Controller.HomeZAxis();
                else if (command.Equals("Move Precisor to Input Station"))
                    _workcell.Process.PrecisorStationProcess.Controller.MoveToInputStation(true, false);//assume up
                else if (command.Equals("Move Precisor to Precisor Station"))
                    _workcell.Process.PrecisorStationProcess.Controller.IsSafeToMoveToPrecisorStation();
                else if (command.Contains("MinPrecisorJogX"))
                {
                    ParseCommandValueInt(command, out _minPrecisorJogX);
                }
                else if (command.Contains("MinPrecisorJogY"))
                {
                    ParseCommandValueInt(command, out _minPrecisorJogY);
                }
                else if (command.Contains("MinPrecisorJogTheta"))
                {
                    ParseCommandValueInt(command, out _minPrecisorJogTheta);
                }
                else if (command.Contains("MaxPrecisorJogX"))
                {
                    ParseCommandValueInt(command, out _maxPrecisorJogX);
                }
                else if (command.Contains("MaxPrecisorJogY"))
                {
                    ParseCommandValueInt(command, out _maxPrecisorJogY);
                }
                else if (command.Contains("MaxPrecisorJogTheta"))
                {
                    ParseCommandValueInt(command, out _maxPrecisorJogTheta);
                }
                else if (command.Contains("StepPrecisorStepIncrementX"))
                {
                    ParseCommandValueInt(command, out _stepPrecisorStepIncrementX);
                }
                else if (command.Contains("StepPrecisorStepIncrementY"))
                {
                    ParseCommandValueInt(command, out _stepPrecisorStepIncrementY);
                }
                else if (command.Contains("StepPrecisorStepIncrementTheta"))
                {
                    ParseCommandValueInt(command, out _stepPrecisorStepIncrementTheta);
                }
                else if (command.Contains("MaxAlignmentCameraFiducialRead"))
                {
                    ParseCommandValueInt(command, out _maxAlignmentCameraFiducialRead);
                }
                else if (command.Contains("HGAProductTailType(1=LongTail;0=ShortTail)"))
                {
                    ParseCommandValueInt(command, out _hgaProductTailType);
                }
                else if (command.Contains("HGAProductTabType(1=Up;0=Down)"))
                {
                    ParseCommandValueInt(command, out _hgaProductTailType);
                }
                else if (command.Contains("XAxisOffsetPlusMinus"))
                {
                    ParseCommandValueInt(command, out _xAxisOffsetPlusMinus);
                }
                else if (command.Contains("YAxisOffsetPlusMinus"))
                {
                    ParseCommandValueInt(command, out _yAxisOffsetPlusMinus);
                }
                else if (command.Contains("ThetaAxisOffsetPlusMinus"))
                {
                    ParseCommandValueDouble(command, out _thetaAxisOffsetPlusMinus);
                }
                else if (command.Equals("RunPrecisorStationVisionTest"))
                {
                    RunPrecisorStationVisionTest();
                }
                else if (command.Equals("RunTestProbeToPrecisorCompatibilityTest"))
                {
                    RunTestProbeToPrecisorCompatibilityTest();
                }
            }
            catch (Exception ex)
            {
                ButtonList btnlst = new ButtonList(ErrorButton.NoButton, ErrorButton.OK, ErrorButton.NoButton);

                this.errorMessageHST = new ErrorMessageHST(this, btnlst, ex);
                this.errorMessageHST.Format(ex);
                this.errorMessageHST.SourceProcess = this._processName;
                this.errorMessageHST.SourceState = this.CurrentStateName;
                this.errorMessageHST.Format();
                ServiceManager.ErrorHandler.RegisterMessage(this.errorMessageHST);
                HSTWorkcell.stopRunScript = true;
            }
            ProcessNextCommand();
        }

        private void ResetAllVariables()
        {
            _minPrecisorJogX = 0;
            _minPrecisorJogY = 0;
            _minPrecisorJogTheta = 0;
            _maxPrecisorJogX = 0;
            _maxPrecisorJogY = 0;
            _maxPrecisorJogTheta = 0;
            _stepPrecisorStepIncrementX = 1;
            _stepPrecisorStepIncrementY = 1;
            _stepPrecisorStepIncrementTheta = 1;
            _maxAlignmentCameraFiducialRead = 1;
            _hgaProductTailType = 0;
            _hgaProductTabType = 0;
            _xAxisOffsetPlusMinus = 10;
            _yAxisOffsetPlusMinus = 10;
            _thetaAxisOffsetPlusMinus = 0.03;
        }

        private void ParseCommandValueInt(string command, out int commandValue)
        {
            String[] words = command.Split(' ');
            commandValue = (words.Length > 1) ? Int32.Parse(words[1]) : 0;
        }

        private void ParseCommandValueDouble(string command, out double commandValue)
        {
            String[] words = command.Split(' ');
            commandValue = (words.Length > 1) ? Double.Parse(words[1]) : 0.00;
        }

        private void RunPrecisorStationVisionTest()
        {
            int totalJogX = 0;
            int totalJogY = 0;
            int totalJogTheta = 0;
            double offset_x = 0;
            double offset_y = 0;
            double offset_theta = 0;

            while (totalJogX <= _maxPrecisorJogX)
            {
                while (totalJogY <= _maxPrecisorJogY)
                {
                    while (totalJogTheta <= _maxPrecisorJogTheta)
                    {
                        _workcell.Process.PrecisorStationProcess.Controller.MoveToInputStation(true, false);//assume is uptab
                        _workcell.Process.PrecisorStationProcess.Controller.IsSafeToMoveToPrecisorStation();
                        _workcell.Process.PrecisorStationProcess.Controller.MoveXAxisRelatively(_stepPrecisorStepIncrementX);
                        _workcell.Process.PrecisorStationProcess.Controller.MoveYAxisRelatively(_stepPrecisorStepIncrementY);
                        _workcell.Process.PrecisorStationProcess.Controller.MoveTAxisRelatively(_stepPrecisorStepIncrementTheta);
                        for (int i = 0; i < _maxAlignmentCameraFiducialRead; i++)
                        {
                            _workcell.Process.PrecisorStationProcess.Controller.VisionInspect(/*out offset_x, out offset_y, out offset_theta*/);
                            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                            {
                                Log.Info(this, "RunPrecisorStationVisionTest: totalJogX={0}, totalJogY={1}, totalJogTheta={2}, OffsetX={3}, OffsetY={4}, OffsetTheta={5}", totalJogX, totalJogY, totalJogTheta, offset_x, offset_y, offset_theta);
                            }
                            if (HSTWorkcell.stopRunScript)
                                return;
                        }
                        totalJogTheta += _stepPrecisorStepIncrementTheta;
                    }
                    totalJogTheta = _minPrecisorJogTheta;
                    totalJogY += _stepPrecisorStepIncrementY;
                }
                totalJogY = _minPrecisorJogY;
                totalJogX += _stepPrecisorStepIncrementX;
            }
        }

        private void RunTestProbeToPrecisorCompatibilityTest()
        {
            //preparation
            double offset_x = 0;
            double offset_y = 0;
            double offset_theta = 0;
            HGAProductTailType tailtype = HGAProductTailType.ShortTail;

            if (_hgaProductTailType == 1)
                tailtype = HGAProductTailType.LongTail;

            bool IsUp = false;
            if (_hgaProductTabType == 1)
                IsUp = true;          
              
            if (!_workcell.Process.PrecisorStationProcess.Controller.IsSafeToMoveToPrecisorStation())
                _workcell.Process.TestProbeProcess.Controller.HomeZAxis();

            _workcell.Process.PrecisorStationProcess.Controller.MoveToPrecisorStation(IsUp, false);

            //Log precisor positon before alignment
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "RunTestProbeToPrecisorCompatibilityTest: Precisor Positon Before Alignment: X:{0}; Y:{1}; Theta:{2}", _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX(),
                                                                                                                                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionY(),
                                                                                                                                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionTheta());
            }

            //Perform vision inspect
            _workcell.Process.PrecisorStationProcess.Controller.VisionInspect(/*out offset_x, out offset_y, out offset_theta*/);

            //perform alignment
            Point3D offset = new Point3D(offset_x, offset_y, offset_theta);
            _workcell.Process.PrecisorStationProcess.Controller.PrecisorAlignment(offset);

            //Log precisor positon after alignment
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "RunTestProbeToPrecisorCompatibilityTest: Precisor Positon After Alignment: X:{0}; Y:{1}; Theta:{2}", _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionX(),
                                                                                                                                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionY(),
                                                                                                                                        _workcell.Process.PrecisorStationProcess.Controller.GetPrecisorNestPositionTheta());
            }

            for (int i = 0; i < 8; i++)
            {
                Point3D testoffset = new Point3D(0.00, 0.00, 0.00);

                if (i == 0)
                {
                    testoffset.X = _xAxisOffsetPlusMinus;//+
                    testoffset.Y = _yAxisOffsetPlusMinus;//+
                    testoffset.Z = _thetaAxisOffsetPlusMinus;//+
                }
                else if (i == 1)
                {
                    testoffset.X = _xAxisOffsetPlusMinus;//+
                    testoffset.Y = _yAxisOffsetPlusMinus;//+
                    testoffset.Z = -_thetaAxisOffsetPlusMinus;//-
                }
                else if (i == 2)
                {
                    testoffset.X = _xAxisOffsetPlusMinus;//+
                    testoffset.Y = -_yAxisOffsetPlusMinus;//-
                    testoffset.Z = _thetaAxisOffsetPlusMinus;//+
                }
                else if (i == 3)
                {
                    testoffset.X = _xAxisOffsetPlusMinus;//+
                    testoffset.Y = -_yAxisOffsetPlusMinus;//-
                    testoffset.Z = -_thetaAxisOffsetPlusMinus;//-
                }
                else if (i == 4)
                {
                    testoffset.X = -_xAxisOffsetPlusMinus;//-
                    testoffset.Y = _yAxisOffsetPlusMinus;//+
                    testoffset.Z = _thetaAxisOffsetPlusMinus;//+
                }
                else if (i == 5)
                {
                    testoffset.X = -_xAxisOffsetPlusMinus;//-
                    testoffset.Y = _yAxisOffsetPlusMinus;//+
                    testoffset.Z = -_thetaAxisOffsetPlusMinus;//-
                }
                else if (i == 6)
                {
                    testoffset.X = -_xAxisOffsetPlusMinus;//-
                    testoffset.Y = -_yAxisOffsetPlusMinus;//-
                    testoffset.Z = _thetaAxisOffsetPlusMinus;//+
                }
                else if (i == 7)
                {
                    testoffset.X = -_xAxisOffsetPlusMinus;//-
                    testoffset.Y = -_yAxisOffsetPlusMinus;//-
                    testoffset.Z = -_thetaAxisOffsetPlusMinus;//-
                }

                //add offset for testplan purpose
                _workcell.Process.PrecisorStationProcess.Controller.MoveXAxisRelatively(testoffset.X);
                _workcell.Process.PrecisorStationProcess.Controller.MoveYAxisRelatively(testoffset.Y);
                _workcell.Process.PrecisorStationProcess.Controller.MoveTAxisRelatively(testoffset.Z);

                //Log precisor coordinate
                if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "RunTestProbeToPrecisorCompatibilityTest: Precisor coordinate for offset : X={0}; Y={1}; Theta={2}", testoffset.X, testoffset.Y, testoffset.Z);
                }

                //Move Probe pin down 
                _workcell.Process.TestProbeProcess.Controller.GoToProbePosition(IsUp, false);

                //perform test
#if TestOpt1SoftwareTrigger    
                _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl()._manualTest = false;
                _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest_Click(this, null);
                   
#elif TestOpt2HardwareTrigger
                //option 2
                _workcell.Process.TestProbeProcess.Controller.TurnOnStartMeasurementOuput();
#endif

                //wait for test to done
#if TestOpt1SoftwareTrigger                
            if(RecallDeferredSignal(HSTWorkcell.SigHGATestingDone))
            {
                UpdateSignalRecipient(HSTWorkcell.SigHGATestingDone);                
            }
#elif TestOpt2HardwareTrigger
                _workcell.Process.TestProbeProcess.Controller.WaitForMeasurementCompleted();
                _workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest_Click(this, null, false);
#endif

            //Move Probe pin up 
                _workcell.Process.TestProbeProcess.Controller.GoToParkPosition(false);
            }
        }
    }
}
