using System;
using System.Collections.Generic;
using System.Linq;

using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.HGA.HST.Vision;

namespace Seagate.AAS.HGA.HST.Machine
{
    class HSTHwSystem : Seagate.AAS.Parsel.Hw.HwSystem
    {


        public enum HardwareComponent
        {
            VisionSystem
        };

        #region Singleton

        public static HSTHwSystem Instance { get { return _instance.Value; } }

        private static readonly Lazy<HSTHwSystem> _instance
             = new Lazy<HSTHwSystem>(() => new HSTHwSystem());

        private HSTHwSystem()
        {
            // TODO: 
        }

        #endregion

        public override void RegisterHwComponents()
        {            
            RegisterHwComponent(((IHardwareComponent)VisionHardware.Instance), (int)HardwareComponent.VisionSystem, "Vision System");
      
        }

        public override void InitializeHwComponents()
        {
            
            try
            {
                if (HSTMachine.Workcell.HSTSettings.Install.EnableVision)
                {
                    Seagate.AAS.Parsel.Equipment.Machine.DisplayStatus("Initializing Vision System");
                    GetHwComponent((int)HardwareComponent.VisionSystem).Initialize(HSTMachine.Workcell.HSTSettings.Install.OperationMode == Utils.OperationMode.Simulation);
                }
            }
            catch (Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                Seagate.AAS.Parsel.Equipment.Machine.DisplayStartupError("Failed to initialize Vision System. The system cannot run. Please check hardware and use System Setup to re-initialize vision system.\r\n" + msg);
            }
        }

        public override string ToString()
        {
            return "HSTHwSystem";
        }

    }
}
