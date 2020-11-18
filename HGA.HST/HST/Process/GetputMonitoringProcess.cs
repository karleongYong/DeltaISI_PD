using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using qf4net;
using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Process
{
    public class GetputMonitoringProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        protected QState stateStartRunning;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;

        // Constructors & Finalizers -------------------------------------------
        public GetputMonitoringProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            // initialize HSM delegates
            _isNonIdleProcess = true;
            stateStartRunning = new QState(this.StartRunning);
            _stateTimer = new QTimer(this);

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // Internal Methods -------------------------------------------------------------
        protected override void InitializeStateMachine()
        {
            base.InitializeStateMachine();
            TransitionTo(stateStartRunning);
        }

        // Methods -------------------------------------------------------------
        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            base.Start(priority);
        }

        protected QState StartRunning(IQEvent qEvent)
        {

            LogMessage("Getput Monitoring", qEvent);
            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTWorkcell.terminatingHSTApps)
                {
                    return null;
                }

                try
                {
                    if (HSTMachine.Workcell.IsMainFormloaded)
                    {
                        if (HSTMachine.Workcell.HSTSettings.Install.OperationMode == OperationMode.Auto)
                        {
                            //FISManager.Instance.Launch();
                            //FISManager.Instance.StartGetPutServer();
                            FISManager.Instance.TestConnection();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(this, "GetputserverMonitoringProcess failed: Exception: {0}, StateName: {1}", ex.Message, this.CurrentStateName);
                }

                _loopTimeOut = new TimeSpan(0, 0, 0, 0, 500);
                _stateTimer.FireIn(_loopTimeOut, new QEvent(SigStateJob));

                return null;
            }

            return base.TopState;
        }
    }
}
