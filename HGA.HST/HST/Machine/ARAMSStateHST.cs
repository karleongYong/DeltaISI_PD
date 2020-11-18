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
using System.Timers;
using System.IO;
using Seagate.AAS.Parsel.Equipment.HGA.UI;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Logging;

namespace Seagate.AAS.HGA.HST.Machine
{   
    public class ARAMSStateHST
    {
        // Nested declarations -------------------------------------------------    
        public enum ARAMSSubstateCodes
        {
            Unknown = 0,
            PRD_ProductiveTime = 1000,
            SBY_StandByTime = 2000,
            ENG_EngineeringTime = 3000,
            SDT_ScheduledDownTime = 4000,
            UDT_UnscheduledDownTime = 5000,
            NST_NonScheduledTime = 6000
        }

        // Member variables ----------------------------------------------------
        private HSTWorkcell _workcell;
        private ARAMSSubstateCodes _currentState = ARAMSSubstateCodes.Unknown;
        private System.Timers.Timer _powerDownTimerTick;
        private string _powerDownTimeLogFile;
        private object _locker = new object();

        // Constructors & Finalizers -------------------------------------------
        public ARAMSStateHST(HSTWorkcell workcell)
        {
            _workcell = workcell;
            _powerDownTimerTick = new System.Timers.Timer();
            _powerDownTimerTick.Interval = 3000.00; // Every 30 secs fixed;
            _powerDownTimerTick.Elapsed += new ElapsedEventHandler(_powerDownTimerTick_Elapsed);
            _powerDownTimerTick.Enabled = true;
           _powerDownTimeLogFile = HSTMachine.Workcell.HSTSettings.Directory.LogFilePath + "\\PowerDownTime.log";//Zhang Jian by passed
        }

        void _powerDownTimerTick_Elapsed(object sender, ElapsedEventArgs e)
        {
            _powerDownTimerTick.Enabled = false;
            try
            {
                int logCode = (int)LoggerCategory.PowerDownTime;
                using (StreamWriter writer = new StreamWriter(_powerDownTimeLogFile, false))
                {
                    
                    writer.WriteLine(logCode.ToString() + "," + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                ParselMessageBox.Show("Error occurred when trying to write " + _powerDownTimeLogFile, ex, MessageBoxIcon.Error,
                     ErrorButton.NoButton, ErrorButton.NoButton, ErrorButton.OK);
            }
            _powerDownTimerTick.Enabled = true;
        }

        // Properties ----------------------------------------------------------
        public ARAMSSubstateCodes CurrentState { get { return _currentState; } }

        // Methods -------------------------------------------------------------
        public void ChangeState(ARAMSSubstateCodes newState)
        {
            lock (_locker)
            {
                if (_currentState != newState)
                {
                    int inew = (int)newState;
                    int icur = (int)_currentState;
                    if (HSTMachine.Workcell.HSTSettings.Install.EnableDebugLog)
                    {
                        Log.Info(this, "{0}, System state changed from CurrentState:{1} to NewState:{2}", LoggerCategory.StateTransition, _currentState.ToString(), newState.ToString());                        
                    }
                    _currentState = newState;
                }
            }
        }

        // Event handlers ------------------------------------------------------

        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------

    }
}
