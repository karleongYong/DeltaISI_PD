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
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Process 
{
    public class DataManagingProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        private DataManagingController _controller;
        private Carrier _currentRunCarrier = null;
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private int _retryCount = 0;
        #region HSM declaration
        //Run mode
        private QState stateRunStart;
        private QState stateWaitForInputCarrierComplete;
        private QState stateISIDataProcess;
        private QState stateCheckGetputServerConnection;
        #endregion       

        public DataManagingProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //Run mode
           stateRunStart = new QState(this.StateRunStart);
           stateWaitForInputCarrierComplete = new QState(this.StateWaitForInputCarrierComplete);
           stateISIDataProcess = new QState(this.StateISIDataProcess);
           stateCheckGetputServerConnection = new QState(this.StateCheckGetputServerConnection);
        }


        // Internal methods ---------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigInputRFIDReadComplete);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new DataManagingController(_workcell, "P5", "Data Managing Controller");
            _controller.InitializeController();
            base.Start(priority);
        }

        // Properties ----------------------------------------------------------
        public DataManagingController Controller{ get { return _controller; } }

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
                    TransitionTo(stateWaitForInputCarrierComplete);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateWaitForInputCarrierComplete(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                if (RecallDeferredSignal(HSTWorkcell.SigInputRFIDReadComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigInputRFIDReadComplete);
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigInputRFIDReadComplete))
            {

                var inputDataCarrier = (Carrier)(qEvent as QEvent).EventObject;

                if (inputDataCarrier != null)
                    _currentRunCarrier = inputDataCarrier;


                TransitionTo(stateCheckGetputServerConnection);
                return null;
            }
            return stateRun;
        }

        private QState StateCheckGetputServerConnection(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob))
            {
                try
                {
                    if (FISManager.Instance.IsFISConnected)
                    {
                        TransitionTo(stateISIDataProcess);
                    }
                    else
                    {
                        throw new Exception("Can't connect to data server, please check GetputServer program or check network connection!!");
                    }

                }
                catch (Exception ex)
                {
                    ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.Abort);
                    TransitionToErrorState(btnlst, ex);
                }

                return null;
            }
            return stateRun;
        }

        private QState StateISIDataProcess(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {

                    if (CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable)
                    {
                        //_controller.GetSLDRData(_currentRunCarrier);
                        _controller.GetSLDRData_New(_currentRunCarrier);
                        _controller.CheckSLDRDataCorrect(_currentRunCarrier);
                    }

                    Carrier IncomingOutputCarrier = _currentRunCarrier.DeepCopy();
                    IncomingOutputCarrier.RFIDData = _currentRunCarrier.RFIDData;

                    lock (CommonFunctions.Instance.InputCarriersLock)
                    {
                        foreach (Carrier carrier in CommonFunctions.Instance.InputCarriers.ToArray())
                        {
                            if (carrier.CarrierID == IncomingOutputCarrier.CarrierID)
                            {
                                carrier.CarrierCurrentLocation = CarrierLocation.BufferStation;
                                break;
                            }
                        }
                    }
                    _workcell.Process.InputEEProcess.Controller.IncomingCarrier = _currentRunCarrier.DeepCopy();

                    //Public to get ISI data
                    QF.Instance.Publish(new QEvent(HSTWorkcell.SigInputGetISIDataComplete));

                    TransitionTo(stateWaitForInputCarrierComplete);
                }
                catch (Exception ex)
                {
                    if (_retryCount < 2)
                    {
                        Thread.Sleep(200);
                        Log.Error(this, "Failed to get ISI data. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCount, ex.Message, this.CurrentStateName);
                        TransitionTo(stateISIDataProcess);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCount++;

                }
                return null;
            }
            return stateRun;
        }




    }
}
