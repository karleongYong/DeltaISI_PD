using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// A factory element that provides a constructor for the generic type at runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NewableElement<T> : IFactoryElement
                                              where T : new()
    {
        /// <summary>
        /// Creates a new object of the factory type.
        /// </summary>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns></returns>
        public object New(params object[] args)
        {
            Debug.Assert(args.Length < 1, "Tried to pass arguments to the constructor for type '" + typeof(T).Name + "' that has only a no argument constructor.");

            return new T();
        }
    }
}
