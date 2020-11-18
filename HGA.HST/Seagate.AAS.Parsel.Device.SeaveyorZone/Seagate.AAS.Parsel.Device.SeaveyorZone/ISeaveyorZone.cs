using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.Parsel.Device.SeaveyorZone
{
    public interface ISeaveyorZone
    {
        //Properties------------------------------------------------------------
        /// <summary>
        /// Name of the SeaveyorZone.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// This is for simulation mode.
        /// </summary>
        bool Simulation { get; set; }
        /// <summary>
        /// This is to check completed signal until time out.
        /// </summary>
        uint TimeOut { get; set; }

        DigitalIOState InPosition { get; }
        //Methods---------------------------------------------------------------
        /// <summary>
        /// Assign the I/O and intializes Others data.
        /// </summary>
        void Initialize();
        /// <summary>
        /// This is to check the movement completed signal.
        /// </summary>
        /// <returns>true:if movement competed else false</returns>
        bool IsInhibit();
        bool IsReverse();
        bool IsInPosition(DigitalIOState ioSate);
        void WaitForMoveCompleted(DigitalIOState ioSate, uint msecTimeOut);
        void Release();
        void Reverse();
        void Inhibit(bool stateInhibit);
        bool IsZoneSensorOn();
        bool IsInpositionSensorOn();
    }
}
