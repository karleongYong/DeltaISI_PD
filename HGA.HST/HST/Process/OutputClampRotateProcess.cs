using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using qf4net;
using System.Linq;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Data;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Process
{
    public class OutputClampRotateProcess : ActiveProcessHST
    {
        #region HSM declaration
        //Run mode
        private QState stateRunStart;
        private QState stateWaitForOutputCarrierComplete;
        private QState stateClampAndRotateClose;
        private QState stateClampAndRotateOpen;
        private QState statePublishUpdateClampDone;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private OutputClampRotateController _controller;

        private const int Auto_Retry_count = 2;
        private int _retryCount;

        #endregion       

        public OutputClampRotateProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateWaitForOutputCarrierComplete = new QState(this.StateWaitForOutputCarrierComplete);
            stateClampAndRotateClose = new QState(this.StateClampAndRotateClose);
            stateClampAndRotateOpen = new QState(this.StateClampAndRotateOpen);
            statePublishUpdateClampDone = new QState(this.StatePublishUpdateClampDone);
        }

        // Internal methods ---------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigOutputStartRFIDProcess);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new OutputClampRotateController(_workcell, "P8", "Clamp Rotate Controller");
            _controller.InitializeController();
            base.Start(priority);
        }

        // Properties ----------------------------------------------------------
        public OutputClampRotateController Controller { get { return _controller; } }

        protected override QState StateRunInit(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                TransitionTo(StateRunStart);
                return null;
            }
            return base.StateRunInit(qEvent);
        }

        private QState StateRunStart(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (_workcell.IsMachineHomed)
                {
                    TransitionTo(stateWaitForOutputCarrierComplete);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForOutputCarrierComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigOutputStartRFIDProcess))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputStartRFIDProcess);
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputStartRFIDProcess))
            {
                TransitionTo(stateClampAndRotateClose);
                return null;
            }
            return stateRun;
        }

        private QState StateClampAndRotateClose(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.OutputStationForwardClamp(true);
                    Thread.Sleep(200);

                    _controller.OutputStationClampRotaryOpenCover(false);
                    Thread.Sleep(200);

                    _retryCount = 0;
                    TransitionTo(stateClampAndRotateOpen);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 2)
                    {
                        Log.Error(this, "Failed to forward clamp and rotate close. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRun;
        }

        private QState StateClampAndRotateOpen(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    _controller.OutputStationForwardClamp(false);
                    Thread.Sleep(200);
                    _controller.OutputStationClampRotaryOpenCover(true);
                    TransitionTo(statePublishUpdateClampDone);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 2)
                    {
                        Log.Error(this, "Failed to backward clamp and rotate open. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(this.targetState);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;
                }
                return null;
            }
            return stateRun;
        }

        private QState StatePublishUpdateClampDone(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    PublishSignal(new QEvent(HSTWorkcell.SigOutputClampRotateProcessComplete));
                    TransitionTo(stateWaitForOutputCarrierComplete);
                }
                catch (Exception ex)
                {
                    Log.Error(this, "Failed to update clamp close signal: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.Stop, ErrorButton.NoButton);
                    TransitionToErrorState(btnlst, ex);
                }
                return null;
            }
            return stateRun;

        }
    }
}
