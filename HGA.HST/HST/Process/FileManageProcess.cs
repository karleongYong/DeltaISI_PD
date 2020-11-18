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
using System.Linq;
using System.Text;
using System.Threading;
using qf4net;
using Seagate.AAS.Parsel;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Equipment.HGA.UI.Utils;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Services;

using Seagate.AAS.HGA.HST.Controllers;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Exceptions;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.UI.Utils;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Process
{
    public class FileManageProcess : ActiveProcessHST
    {
        // Member variables ----------------------------------------------------
        protected QState stateStartRunning;
        private FileManageController _controller;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;


        // Constructors & Finalizers -------------------------------------------
        public FileManageProcess(HSTWorkcell workcell, string processID, string processName)
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

            _controller = new FileManageController(_workcell, "NN", "File Manage Controller");
            _controller.InitializeController();
            base.Start(priority);
        }

        protected QState StartRunning(IQEvent qEvent)
        {
            if (qEvent.IsSignal(SigStateJob))
            {
                if (HSTWorkcell.terminatingHSTApps)
                {
                    return null;
                }

                ////TDF file managing
                try
                {
                    if (FormMainE95.ActiveForm != null && FormMainE95.ActiveForm.Visible && !HSTMachine.Workcell.IsMainFormloaded)
                        HSTMachine.Workcell.IsMainFormloaded = true;


                    var isMappedDrive = HSTMachine.Workcell.HSTSettings.iSTDFGlobalDriveConnected = _controller.MappingNetworkDrive();
                    HSTMachine.Workcell.HSTSettings.TurnOnTestRunWithoutData = false;
                    if (isMappedDrive && !HSTMachine.Workcell.HSTSettings.TurnOnTestRunWithoutData)
                        if (!_workcell.TDFOutputObj.IsInProcessing && HSTMachine.Workcell.HSTSettings.Install.EnabledTDFFileSystem && 
                            HSTMachine.Workcell.IsMainFormloaded)
                            _controller.TDFFilesManaging();

                    if (isMappedDrive)
                        _controller.SearchUselessFileFromGlobal();

                }
                catch (Exception ex)
                {
                    Log.Error(this, "FileManageProcess failed to manage file: Exception: {0}, StateName: {1}", ex.Message, this.CurrentStateName);
                }

                _loopTimeOut = new TimeSpan(0, 0, 0, 0, 500);
                _stateTimer.FireIn(_loopTimeOut, new QEvent(SigStateJob));

                return null;
            }

            return base.TopState;
        }

    }
}
