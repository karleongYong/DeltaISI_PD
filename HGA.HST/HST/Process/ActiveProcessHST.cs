//
//  (c) Copyright 2013 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2013/08/01] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using qf4net;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Parsel.Services;
using System.IO;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Utils;
using System.Diagnostics;
using XyratexOSC.Logging;
using XyratexOSC.Utilities;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Process
{
    public abstract class ActiveProcessHST : Seagate.AAS.Parsel.Equipment.ActiveProcess
    {
        // Nested declarations -------------------------------------------------
        public class ErrorMessageHST : APErrorMessage
        {
            public ErrorMessageHST(Active ap, ButtonList buttonList, Exception ex)
                : base(ap, buttonList)
            {
                this.Exception = ex;

                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);

                if (frame.GetFileName() == null)
                    this.SourceFile = "Not Available.";
                else
                {
                    string[] SourceFileName = frame.GetFileName().Split('\\');
                    if (SourceFileName.Length > 0)
                    {
                        this.SourceFile = String.Format("{0} (Line: {1})", SourceFileName[SourceFileName.Length - 1], frame.GetFileLineNumber());
                    }
                    else
                    {
                        this.SourceFile = String.Format("{0} (Line: {1})", frame.GetFileName(), frame.GetFileLineNumber());
                    }
                }
            }

            public string SourceProcess { get; set; }
            public string SourceState { get; set; }
            public Exception Exception { get; private set; }
            public string WorkInstruction { get; private set; }
            public string TimeStamp { get; private set; }
            public string SourceFile { get; set; }

            public virtual void Format()
            {
                if (this.Exception is HSTException)
                {
                    this.Source = /*"E" + */((HSTException)this.Exception).ErrorCode;
                    this.WorkInstruction = ((HSTException)this.Exception).WorkInstruction;
                    this.TimeStamp = ((HSTException)this.Exception).TimeStamp;
                }
                else
                {
                    this.Source = /*"E000"*/ "0000";
                    this.WorkInstruction = "N/A";
                    this.TimeStamp = "N/A";
                }

                _fullDescription = "";
                Exception ex = null;
                ex = this.Exception;
                while (ex != null)
                {
                    _fullDescription += ex.Message + "\r\n";
                    _maintenanceDescription += ex.Message + ",";
                    ex = ex.InnerException;
                    

                }

                _fullDescription += "\r\n";
                _fullDescription += "Source File: " + SourceFile + "\r\n";
                _fullDescription += "\r\n";
                _fullDescription += "Work Instruction: " + WorkInstruction + "\r\n";
                _fullDescription += "Process: " + SourceProcess + "\r\n";
                _fullDescription += "State: " + SourceState + "\r\n";
                _fullDescription += "Timestamp: " + TimeStamp;

                
            }
        }

        // Generic MSEM Processing states

        protected QState stateCalibration;
        protected QState stateRecovery;

        protected bool _isStateInitialized;
        protected bool _isNonIdleProcess;
        protected bool _isErrorstate = false;

        // Member variables ----------------------------------------------------
        

        protected List<Signal> _signalList;
        private bool _stepModeToggle;        
        protected HSTWorkcell _workcell;        
        private ControllerHST controller;
        protected string _processID;
        protected string _processName;
        protected ErrorMessageHST errorMessageHST;

        // Constructors & Finalizers -------------------------------------------
        public ActiveProcessHST(HSTWorkcell workcell, string processID, string processName)
            : base(workcell)
        {
            this._workcell = workcell;
            this._processID = processID;
            this._processName = processName;
           
            _processErrorList = new Dictionary<int, string>();
            AddProcessError();

            _signalList = new List<Signal>();
        }        

        // Properties ----------------------------------------------------------
        public string ProcessID { get { return _processID; } }
        public string ProcessName { get { return _processName; } }        
        public bool IsErrorState
        {
            get { return _isErrorstate; }
        }

        protected Dictionary<int, string> _processErrorList;
        /// <summary>
        /// This list store exceptions that are capable be thrown by this type.
        /// </summary>
        public Dictionary<int, string> ProcessErrorList
        {
            get { return _processErrorList; }
        }

        public bool IsNonIdleProcess
        {
            get { return _isNonIdleProcess; }
        }

        public bool IsStateInitialized
        {
            get { return _isStateInitialized; }
            set { _isStateInitialized = value; }
        }
                
       
        public void DelayStep(int dlyInMs)
        {
            if (dlyInMs != 0)
            {
            }
        }

        public virtual void OutputProcessErrorListToTextFile()
        {
            OutputProcessErrorListToTextFile("");
        }

        public virtual void OutputProcessErrorListToTextFile(string fileName)
        {
            string fn = "";
            // if (fileName == "")//ZJ By passed

            StreamWriter sw = new StreamWriter(fn);
            sw.WriteLine("[" + _processName + "]");
            foreach (KeyValuePair<int, string> err in _processErrorList)
            {
                sw.WriteLine(_processID + err.Key.ToString("000000") + ": " + err.Value);
            }
            sw.Close();
        }

        // Methods -------------------------------------------------------------
        public void ResetInit()
        {
            _isStateInitialized = false;
        }

        /// <summary>
        /// Add signals that require deffer handling.
        /// </summary>
        /// <param name="signal"></param>

        public virtual string GetErrorCode(int errorNum)
        {
            return _processID + errorNum.ToString("00000");
        }

        public void UpdateSignalRecipient(Signal signal)
        {
            if (HSTMachine.Workcell != null)
            {
                if (HSTMachine.Workcell.getPanelOperation() != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                    {
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelRecipientSignalTimestamp.Text = DateTime.Now.ToString();
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelRecipientSignalName.Text = signal.ToString();
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelRecipientSignalSource.Text = _processName;
                        });
                    }
                }
            }
        }

        public void AddAndSubscribeSignal(Signal signal)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "SubscribeSignal, ProcessName:{0}, SignalName:{1}", _processName, signal.ToString());
            }
            if (!_signalList.Contains(signal))
            {                
                QF.Instance.Subscribe(this, signal);
                _signalList.Add(signal);
            }            
        }

        public void PublishSignal(QEvent qEvent)
        {
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
            {
                Log.Info(this, "PublishSignal, ProcessName:{0}, SignalName:{1}", _processName, qEvent.QSignal.ToString());
            }

            if (HSTMachine.Workcell != null)
            {
                if (HSTMachine.Workcell.getPanelOperation() != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                    {
                        UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                        {
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelOriginatorSignalTimestamp.Text = DateTime.Now.ToString();
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelOriginatorSignalName.Text = qEvent.QSignal.ToString();
                            HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().labelOriginatorSignalSource.Text = _processName;
                        });
                    }
                }
            }

            QF.Instance.Publish(qEvent);                                   
        }

        protected override void InitializeStateMachine()
        {
            stateCalibration = new QState(this.StateCalibration);
            stateRecovery = new QState(this.StateRecovery);
            base.InitializeStateMachine();
        }

        #region HSM

        protected override QState StateError(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(QSignals.Entry))
            {
                _isErrorstate = true;
                string str = this.ToString();
                str = str.Substring(str.LastIndexOf(".") + 1);
                str = str.Remove(str.LastIndexOf("-"));
                str = str + ": " + this.currentException.Source;
                _workcell.DisplayTitleMessage("Error Occurred (" + str + ")");
            }
            if (qEvent.IsSignal(QSignals.Exit))
            {
                _isErrorstate = false;
                string str = this.ToString();
                str = str.Substring(str.LastIndexOf(".") + 1);
                str = str.Remove(str.LastIndexOf("-"));
                str = str + ": " + this.currentException.Source;
                _workcell.DisplayTitleMessage("Error Cleared (" + str + ")");

                // Log alarm cleared.
                string errMsg = this.GetErrorLogFormatString();                
                Log.Error(this, "{0} Message:{1}", LoggerCategory.AlarmCleared, errMsg);
            }
            if (qEvent.IsSignal(base.SigStateJob))
            {
                // Format to CLU error format.
                this.errorMessageHST.Format();

                this.DefaultErrorHandler();

                // Change state UDT.
                _workcell.ARAMSState.ChangeState(ARAMSStateHST.ARAMSSubstateCodes.UDT_UnscheduledDownTime);

                // Log alarm occured
                string errMsg = this.GetErrorLogFormatString();                
                Log.Error(this, "{0} Message:{1}", LoggerCategory.AlarmOccured, errMsg);

                string [] maintenance_msg = this.GetMaintenanceLogFormatString();
                Log.Maintenance(this, "{0}, {1}", maintenance_msg[0], maintenance_msg[1]);
                return null;
            }
            if (!qEvent.IsSignal(Active.SigResume))
            {
                return this.statePaused;
            }
            System.Threading.Thread.Sleep(500);
            _workcell.OffAlarm = false;
            switch (base.errorMessage.ButtonSelected)
            {
                case ErrorButton.Abort:
                    this._workcell.MainProcess.Abort();
                    break;

                case ErrorButton.Retry:
                    {
                        base.targetState = base.stateHistory;
                        base.TransitionToWithoutJob(base.targetState);
                        QEvent event2 = new QEvent(base.SigStateJob);
                        event2.EventObject = base.errorMessage.ButtonSelected;
                        this.errorMessage = null;
                        base.PostFIFO(event2);
                        break;
                    }
                case ErrorButton.Stop:
                    this._workcell.MainProcess.Stop();

                    for (int i = 0; i < ServiceManager.ErrorHandler.ErrorList.Count; i++)
                    {
                        IErrorMessage currentMessage = ServiceManager.ErrorHandler.ErrorList[i] as IErrorMessage;
                        ServiceManager.ErrorHandler.UnRegisterMessage(currentMessage);
                    }
                    break;
                case ErrorButton.NoButton:
                    //TY: Treat NoButton as Retry 
                    Console.Beep();
                    base.targetState = base.stateHistory;
                    base.TransitionToWithoutJob(base.targetState);
                    QEvent event4 = new QEvent(base.SigStateJob);
                    event4.EventObject = base.errorMessage.ButtonSelected;
                    this.errorMessage = null;
                    base.PostFIFO(event4);
                    break;

                default:
                    {
                        base.targetState = base.stateHistory;
                        base.TransitionToWithoutJob(base.targetState);
                        QEvent event3 = new QEvent(base.SigRecover);
                        event3.EventObject = base.errorMessage.ButtonSelected;
                        base.PostFIFO(event3);
                        break;
                    }
            }
            return null;
        }

        protected override QState StateActive(IQEvent qEvent)
        {
            LogMessage("StateActive", qEvent);

            // We add deferred signal when pause.
            if (_signalList.Contains(qEvent.QSignal))
            {
                AddDeferredSignal(qEvent);
                return null;
            }
            return base.StateActive(qEvent);
        }

        protected override QState StateRunInit(IQEvent qEvent)
        {
            LogMessage("StateRunInit", qEvent);

            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !qEvent.QSignal.ToString().Equals("Empty") && !qEvent.QSignal.ToString().Equals("Init") && !qEvent.QSignal.ToString().Equals("SigTimeout"))
            {
                Log.Info(this, "{0}, ProcessName:{1}, StateName:StateRunInit, QEvent:{2}, QSignal:{3}, Message: Initialization completed.", LoggerCategory.StateTransition, _processName, qEvent.ToString(), qEvent.QSignal.ToString());
            }            

            LogMessage("StateRunInit", qEvent);

            if (qEvent.IsSignal(QSignals.Entry))
            {
                _isStateInitialized = false;
                ClearDeferredSignals();
            }

            // Initialization completed. Signal to stop state.
            if (qEvent.IsSignal(QSignals.Exit))
            {
                if (!_workcell.Process.IsErrorState && !this._isErrorstate)
                {                    
                    this.stateHistory = this.targetState;
                    ClearDeferredSignals();
                    _isStateInitialized = true;
                }
            }
            return stateProcess;
        }

        protected override QState StateProcess(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(Active.SigStop))
            {
                base.stateHistory = base.targetState;
                this.TransitionTo(this.stateStopping);
                return null;
            }
            if (qEvent.IsSignal(Active.SigPause))
            {
                base.stateHistory = base.targetState;                
                this.TransitionTo(this.statePaused);
                return null;
            }
            if (qEvent.IsSignal(base.SigError))
            {
                this._isErrorstate = true; this.TransitionTo(this.stateError);
                return null;
            }
            return this.stateActive;
        }

        protected override QState StateRun(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(QSignals.Entry))
            {
                if (HSTMachine.Workcell != null)
                {
                    if (HSTMachine.Workcell.getPanelOperation() != null)
                    {
                        if (HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel() != null)
                        {
                            UIUtility.Invoke(HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel(), () =>
                            {
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnGetConversionBoardD.Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().btnStartMeasurementTest.Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationMainPanel().getMeasurementTestUserControl().cboTabType.Enabled = false;

                                HSTMachine.Workcell.getPanelOperation().getOperationStatusPanel().Enabled = false;
                                HSTMachine.Workcell.getPanelOperation().getOperationModuleStatePanel().Enabled = false;

                                HSTMachine.Instance.MainForm.getPanelNavigation().btnSetup.Enabled = false;
                                HSTMachine.Instance.MainForm.getPanelNavigation().btnRecipe.Enabled = false;
                                HSTMachine.Instance.MainForm.getPanelNavigation().btnDiagnostic.Enabled = false;
                            });
                        }
                    }
                }
                
                if (!_workcell.Process.IsInit)
                {
                    _workcell.ARAMSState.ChangeState(ARAMSStateHST.ARAMSSubstateCodes.PRD_ProductiveTime);
                }
                
                int ProcessCount = HSTMachine.Workcell.Process.ActiveProcess.Count - 4;
                int MatchedCount = 0;
                foreach (Active activeProcess in HSTMachine.Workcell.Process.ActiveProcess.Values)
                {
                    if(String.Compare(activeProcess.CurrentStateName, "StateIdle", true) == 0 ||                        
                        activeProcess.CurrentStateName.IndexOf("StateRunInit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        activeProcess.CurrentStateName.IndexOf("BoundaryCheck", StringComparison.OrdinalIgnoreCase) >= 0)
                    {                        
                        break;
                    }
                    else
                    {
                        MatchedCount++;                        
                    }
                }

                if (MatchedCount >= 5)
                {
                    CommonFunctions.Instance.SystemInitializationCompleted = true;                    
                }              
            }
           
            if (qEvent.IsSignal(Active.SigStop))
            {
                _workcell.DisplayTitleMessage("System Stopped"); 
                base.stateHistory = base.targetState;
                this.TransitionTo(this.stateStopping);
                return null;
            }

            if (qEvent.IsSignal(Active.SigPause))
            {
                _workcell.DisplayTitleMessage("System Paused"); 
                base.stateHistory = base.targetState;
                this.TransitionTo(this.statePaused);
                return null;
            }
            return this.stateProcess;
        }

        protected override QState StatePaused(IQEvent qEvent)
        {
            LogMessage("Paused", qEvent);

            if (qEvent.IsSignal(SigStop))
            {
                TransitionTo(stateStopping);
                return null;
            }
            if (qEvent.IsSignal(SigAbort))
            {
                TransitionTo(stateAborting);
                return null;
            }
            if (qEvent.IsSignal(SigResume))
            {
                this.targetState = stateHistory;
                base.TransitionToWithoutJob(targetState);
                QEvent event2 = new QEvent(base.SigStateJob);
                base.PostFIFO(event2);
                return null;
            }
            return stateActive;
        }

        protected override QState StateIdle(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            // We add deferred signal when stopped.
            if (_signalList.Contains(qEvent.QSignal))
            {
                AddDeferredSignal(qEvent);
                return null;
            }

            if (qEvent.IsSignal(QSignals.Entry))
            {
                if (!_workcell.Process.IsInit)
                {
                    _workcell.ARAMSState.ChangeState(ARAMSStateHST.ARAMSSubstateCodes.SBY_StandByTime);
                }
            }

            if (qEvent.IsSignal(Active.SigStart))
            {
                if (!_workcell.Process.IsErrorState)
                {
                    if (_workcell.Process.IsInit     // Force initialization (e.g. from UI)//ZJ Bypass
                        || base.stateHistory == null
                        || !this.IsStateInitialized)
                    {
                        this.TransitionTo(this.stateRunInit);
                    }
                    else if (!_workcell.Process.IsErrorState && _workcell.IsMachineHomed)
                    {
                        TransitionTo(stateHistory);
                    }
                }
                else
                {
                    _workcell.Process.Stop();
                }
                return null;
            }
            if (qEvent.IsSignal(base.SigError))
            {
                this.TransitionTo(this.stateIdleWithError);
                return null;
            }
            return base.TopState;
        }

        protected virtual QState StateRecovery(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                TransitionTo(base.stateHistory);
                return null;
            }
            return stateProcess;
        }

        protected override QState StateAborting(IQEvent qEvent)
        {
            if (qEvent.IsSignal(QSignals.Entry))
            {
                _isStateInitialized = false;
                _workcell.IsMachineHomed = false;
                _workcell.DisplayTitleMessage("System Not Initialized");
            }
            return base.StateAborting(qEvent);
        }

        protected virtual QState StateCalibration(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStop))
            {
                TransitionTo(stateStopping);
                return null;
            }
            return stateProcess;
        }
        #endregion

        protected override void TransitionTo(QState targetState)
        {
            if (_workcell.Process.IsStepMode
                && !_stepModeToggle
                && !_isNonIdleProcess && !_workcell.Process.IsInit
                && this.CurrentNestedStateName.Contains("[StateRun]"))
            {
                 if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                {
                    Log.Info(this, "{0}, ProcessName:{1}, StateName:TransitionTo State:{2}, Step mode at:{3}", LoggerCategory.StateTransition, _processName, targetState.ToString(), targetState.Method.Name);
                }
                _workcell.Process.Stop();
                _stepModeToggle = true;
            }
            else
            {
                _stepModeToggle = false;
            }
            base.TransitionTo(targetState);
        }
        
        protected override void TransitionToErrorState(Exception exception)
        {
            ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);
            TransitionToErrorState(btnlst,exception);
        }

        protected override void TransitionToErrorState(ButtonList buttonList, Exception exception)
        {
            this.currentException = exception;
                        
            {
                this.errorMessageHST = new ErrorMessageHST(this, buttonList, exception);
                this.errorMessageHST.Format(this.currentException);
                this.errorMessageHST.SourceProcess = this._processName;
                this.errorMessageHST.SourceState = this.CurrentStateName;

                if (this.errorMessageHST.Exception is HSTException)
                {
                    if (String.IsNullOrEmpty(((HSTException)this.errorMessageHST.Exception).ErrorCode) == false)
                    {
                        int value;
                        bool result = int.TryParse(((HSTException)this.errorMessageHST.Exception).ErrorCode, out value);
                        if (result == false)
                        {
                            // String is not a number.
                        }
                        else
                        {
                            int errorType = value - (value % 1000000);
                            if (errorType == (int)AutomationSystem.ImmediateStop)
                            {
                                HSTWorkcell.stopSystemDueToAxisError = true;
                                Log.Error(this, "stopSystemDueToAxisError: True. Automation system has to be stopped immediately due to critical error code: {0} generated by {1}.", this.errorMessageHST.Source, this.errorMessageHST.SourceProcess);
                            }
                        }
                    }
                }            

                this.errorMessage = new APErrorMessage(this, buttonList);
                this.errorMessage = this.errorMessageHST;
                this.errorMessage.Format(this.currentException);
                this.errorMessage.Priority = 999;
            }
            this.stateHistory = this.targetState;
            base.PostFIFO(new QEvent(this.SigError));
        }

        protected virtual string GetErrorLogFormatString()
        {
            string str;
            string det = this.errorMessageHST.FullDescription.Replace("\r\n", "->");
            str = string.Format("ErrorCode:{0};Detail:{1}",
                this.errorMessageHST.Source,
                det);    
            
            return str;
        }

        protected virtual string [] GetMaintenanceLogFormatString()
        {
            string[] str = new string[2];
            
            str[0] = this.errorMessageHST.Source; //error code
            str[1] = this.errorMessageHST.MaintenanceDescription;
            
            return str;
        }

        protected bool IsSigStateJob(IQEvent qEvent, string state)
        {
            if (qEvent.IsSignal(SigStateJob))
            {
                EventParam evtParam = ((QEvent)qEvent).EventObject as EventParam;
                if (evtParam == null)
                    return false;
                if (evtParam.Consumer == null)
                    return false;

                if (evtParam.Consumer == state)
                {
                    return true;
                }
                else
                {
                     if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !qEvent.QSignal.ToString().Equals("Empty") && !qEvent.QSignal.ToString().Equals("Init") && !qEvent.QSignal.ToString().Equals("SigTimeout"))
                    {                        
                        Log.Info(this, "{0}, ProcessName:{1}, StateName:In IsSigStateJob, Unable to consume signal {2}. Handler:{3}, Consumer:{4}", LoggerCategory.StateTransition, _processName, qEvent.QSignal.ToString(), state, evtParam.Consumer); 
                    }                        
                    return false;
                }
            }
            else
            {
                return false;
            }
        }       

        // Internal methods ----------------------------------------------------
        protected virtual void AddProcessError()
        { }
        protected void AddError(int errorNum, string errorMessage)
        {
            _processErrorList.Add(errorNum, errorMessage);
        }

        protected QState StateAlertOperatorToRemoveCarrier(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);
            
            if (qEvent.IsSignal(SigStateJob))
            {
                Notify.PopUp("User Action Required", "Carrier detected in the machine. Please remove it before running the machine.", "", "OK");
                HSTMachine.Instance.MainForm.getPanelCommand().stopSystemProcess();
                return stateStopping;
            }
            return stateStopping;
        }

        protected void LogStateInfo(string processName, string stateName, IQEvent qEvent)
        {
            LogMessage(stateName, qEvent);
            if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog && !qEvent.QSignal.ToString().Equals("Empty") && !qEvent.QSignal.ToString().Equals("Entry") && 
                !qEvent.QSignal.ToString().Equals("Exit") && !qEvent.QSignal.ToString().Equals("Init") && !qEvent.QSignal.ToString().Equals("SigTimeout"))
            {
                Log.Info(this, "{0}, ProcessName:{1}, StateName:{2}, QSignal:{3}", LoggerCategory.StateTransition, processName, stateName, qEvent.QSignal.ToString());
            }
        }

        public void PromptExceptionErrorMessage(Active ap, string ProcessName, Exception ex)
        {
            ButtonList btnlst = new ButtonList(ErrorButton.OK, ErrorButton.NoButton, ErrorButton.NoButton);

            ErrorMessageHST errorMessageHST = new ErrorMessageHST(ap, btnlst, ex);
            errorMessageHST.Format(ex);
            errorMessageHST.SourceProcess = ProcessName;
            errorMessageHST.SourceState = ap.CurrentStateName;
            errorMessageHST.Format();
            Log.Error(this, "{0}, ProcessName:{1}, Error:{2}", LoggerCategory.StateTransition, ProcessName, ex.ToString());

            ServiceManager.ErrorHandler.RegisterMessage(errorMessageHST);
        }
    }
}
