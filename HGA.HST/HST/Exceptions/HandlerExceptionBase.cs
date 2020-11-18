using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel;

namespace Seagate.AAS.HGA.HST.Exceptions
{
    public abstract class HandlerExceptionBase : ParselException
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private string _errorCode;

        // Constructors & Finalizers -------------------------------------------
        public HandlerExceptionBase()
        {
        }

        public HandlerExceptionBase(string message)
            : base(message)
        {
        }

        public HandlerExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Properties ----------------------------------------------------------

        public string ErrorCode { get { return _errorCode; } set { _errorCode = value; } }

        // Methods -------------------------------------------------------------
    }
}
