using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// A factory element that can create objects of the generic type T during runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreateableElement<T> : IFactoryElement
    {
        /// <summary>
        /// Creates a new object of the factory type T.
        /// </summary>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns></returns>
        public object New(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }
}
