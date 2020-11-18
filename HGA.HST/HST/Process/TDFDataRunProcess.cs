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
using Seagate.AAS.Parsel.Device.RFID;
using Seagate.AAS.Parsel.Device.RFID.Hga;

namespace Seagate.AAS.HGA.HST.Process
{
    public class TDFDataRunProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        private TDFDataRunController _controller;
        private Carrier _currentRunCarrier = null;
        private FolaTagData _currentFolaTagInfo = new FolaTagData();
        private List<SeatrackLoadData> _currentLoadData = new List<SeatrackLoadData>();
        private Carrier _currentOutputRunCarrier = new Carrier();
        private QTimer _qTimer;
        private Signal _sigTimeout = new Signal("SigTimeout");
        private int _retryCountSendSeatrack = 0;
        private int _retryCountSaveTDF = 0;

        #region HSM declaration
        //Run mode
        private QState stateRunStart;
        private QState stateWaitForOutputCarrierComplete;
        private QState stateSentSeatrackData;
        private QState stateUpdateDataLogFile;
        private QState stateSaveTDFDataFile;

        private const int Auto_Retry_count = 2;

        #endregion       

        public TDFDataRunProcess(HSTWorkcell workcell, string processID, string processName)
            : base(workcell, processID, processName)
        {
            this._workcell = workcell;
            _qTimer = new QTimer(this);

            //Run mode
            stateRunStart = new QState(this.StateRunStart);
            stateWaitForOutputCarrierComplete = new QState(this.StateWaitForOutputCarrierComplete);
            stateSentSeatrackData = new QState(this.StateSentSeatrackData);
            stateUpdateDataLogFile = new QState(this.StateUpdateDataLogFile);
            stateSaveTDFDataFile = new QState(this.StateSaveTDFDataFile);
        }

        // Internal methods ---------------------------------------------------
        protected override void InitializeStateMachine()
        {
            AddAndSubscribeSignal(HSTWorkcell.SigOutputProcessDataComplete);
            base.InitializeStateMachine();
        }

        public override void Start(int priority)
        {
            // start is called by WorkCell.Startup()
            _controller = new TDFDataRunController(_workcell, "P5", "Tdf Data Run Controller");
            _controller.InitializeController();
            base.Start(priority);
        }

        // Properties ----------------------------------------------------------
        public TDFDataRunController Controller { get { return _controller; } }

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
                _retryCountSaveTDF = 0;
                _retryCountSendSeatrack = 0;
                if (RecallDeferredSignal(HSTWorkcell.SigOutputProcessDataComplete))
                {
                    UpdateSignalRecipient(HSTWorkcell.SigOutputProcessDataComplete);
                }

                return null;
            }

            if (qEvent.IsSignal(HSTWorkcell.SigOutputProcessDataComplete))
            {
                var outputData = (object[])(qEvent as QEvent).EventObject;
                if(outputData != null)
                {
                    _currentFolaTagInfo = (FolaTagData)outputData[0];
                    _currentOutputRunCarrier = (Carrier)outputData[1];
                }

                TransitionTo(stateSentSeatrackData);
                return null;
            }
            return stateRun;
        }

        private QState StateSentSeatrackData(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    _controller.PrepareCommonSeatrackLoadData(_currentFolaTagInfo, _currentOutputRunCarrier);

                    if (HSTMachine.Workcell.HSTSettings.Install.SeatrackRecordUpdateEnabled)
                        _controller.SendLoadDataToSeaTrack(_currentFolaTagInfo, _currentOutputRunCarrier);

                    TransitionTo(stateUpdateDataLogFile);
                }
                catch (Exception ex)
                {
                    if (_retryCountSendSeatrack < Auto_Retry_count)
                    {
                        Thread.Sleep(200);
                        Log.Error(this, "Failed to seatrack data. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCountSendSeatrack, ex.Message, this.CurrentStateName);
                        TransitionTo(stateSentSeatrackData);
                    }
                    else
                    {
                        Log.Error(this, "TDFDataRunProcess failed to upadte data to seatrack: {0}, Exception: {1}, StateName: {2}", _retryCountSendSeatrack, ex.Message, this.CurrentStateName);
                        TransitionTo(stateUpdateDataLogFile);
                    }
                    _retryCountSendSeatrack++;

                }
                return null;
            }
            return stateRun;
        }

        private QState StateUpdateDataLogFile(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    _controller.UpdateDataLog(_currentFolaTagInfo, _currentOutputRunCarrier);
                    TransitionTo(stateSaveTDFDataFile);
                }
                catch (Exception ex)
                {
                    Log.Error(this, "TDFDataRunProcess failed to upadte data to logfile: {0}, Exception: {1}, StateName: {2}", _retryCountSendSeatrack, ex.Message, this.CurrentStateName);
                    TransitionTo(stateSaveTDFDataFile);
                }
                return null;
            }
            return stateRun;
        }

        private QState StateSaveTDFDataFile(IQEvent qEvent)
        {
            LogStateInfo(_processName, System.Reflection.MethodBase.GetCurrentMethod().Name, qEvent);

            if (qEvent.IsSignal(SigStateJob) || qEvent.IsSignal(_sigTimeout))
            {
                try
                {
                    if(_workcell.HSTSettings.Install.EnabledTDFFileSystem || _workcell.HSTSettings.Install.EnabledSaveTDFFileOnly)
                        _controller.SaveTDFDataToLocalFile(_currentFolaTagInfo, _currentOutputRunCarrier);

                    TransitionTo(stateWaitForOutputCarrierComplete);
                }
                catch (Exception ex)
                {
                    if (_retryCountSaveTDF < 2)
                    {
                        Thread.Sleep(200);
                        Log.Error(this, "Failed to get ISI data. Retry count: {0}, Exception: {1}, StateName: {2}", _retryCountSendSeatrack, ex.Message, this.CurrentStateName);
                        TransitionTo(stateSaveTDFDataFile);
                    }
                    else
                    {
                        ButtonList btnlst = new ButtonList(ErrorButton.Retry, ErrorButton.NoButton, ErrorButton.NoButton);
                        TransitionToErrorState(btnlst, ex);
                    }
                    _retryCountSaveTDF++;

                }
                return null;
            }
            return stateRun;
        }

    }
}
