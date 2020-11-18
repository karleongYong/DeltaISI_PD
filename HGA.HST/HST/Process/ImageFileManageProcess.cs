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
    public class ImageFileManageProcess : ActiveProcessHST
    {
                // Member variables ----------------------------------------------------
        protected QState stateStartRunning;
        private ImageFileManageController _controller;
        private QTimer _stateTimer;
        private TimeSpan _loopTimeOut;


        // Constructors & Finalizers -------------------------------------------
        public ImageFileManageProcess(HSTWorkcell workcell, string processID, string processName)
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

            _controller = new ImageFileManageController(_workcell, "NN", "Image File Manage Controller");
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
                //Auto remove image
                try
                {
                    if (HSTMachine.Workcell.IsMainFormloaded)
                    {
                        System.TimeSpan datediff = System.DateTime.Now - HSTMachine.Workcell.InputImageDeletedList.LastDeletedDate;
                        if (datediff.TotalDays > HSTMachine.Workcell.CalibrationSettings.Vision.InputCamera.TotalDayToStoreImage)
                        {
                            _controller.AutoRemoveInputImageFile();
                        }

                        System.TimeSpan datediffOutput = System.DateTime.Now - HSTMachine.Workcell.OutputImageDeletedList.LastDeletedDate;
                        if (datediffOutput.TotalDays > HSTMachine.Workcell.CalibrationSettings.Vision.OutputCamera.TotalDayToStoreImage)
                        {
                            _controller.AutoRemoveOutputImageFile();
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.Error(this, "FileManageProcess failed to remove image file: Exception: {0}, StateName: {1}", ex.Message, this.CurrentStateName);
                }

                _loopTimeOut = new TimeSpan(0, 0, 0, 0, 500);
                _stateTimer.FireIn(_loopTimeOut, new QEvent(SigStateJob));

                return null;
            }

            return base.TopState;
        }
    }
}
