using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.HGA.HST.Exceptions
{
    public class ProcessException: HandlerExceptionBase
    {

        // Nested declarations -------------------------------------------------    

        // Member variables ----------------------------------------------------

        // Constructors & Finalizers -------------------------------------------
        public ProcessException()
        {
        }

        public ProcessException(string message)
            : base(message)
        {
        }

        public ProcessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        // Event handlers ------------------------------------------------------

        // Interface implementation --------------------------------------------

        // Internal methods ----------------------------------------------------
    }
}
