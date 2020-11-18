using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// An object that can be created during runtime from reflection with or without optional parameters
    /// </summary>
    public interface IFactoryElement
    {
        /// <summary>
        /// Creates a new object of the factory type.
        /// </summary>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns></returns>
        object New(params object[] args);
    }
}
