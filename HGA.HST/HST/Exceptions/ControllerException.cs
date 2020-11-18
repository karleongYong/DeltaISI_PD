using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.HGA.HST.Controllers;

namespace Seagate.AAS.HGA.HST.Exceptions
{
    public class ControllerException : HandlerExceptionBase
    {
        // Nested declarations -------------------------------------------------    

        // Member variables ----------------------------------------------------

        // Constructors & Finalizers -------------------------------------------
        public ControllerException()
        {
        }

        public ControllerException(string message)
            : base(message)
        {
        }

        public ControllerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Properties ----------------------------------------------------------
        ControllerHST _controller;
        public ControllerHST Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }


        // Methods -------------------------------------------------------------

        // Event handlers ------------------------------------------------------

        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------
    }
}
