using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// A Factory that can create objects of type T based on a key of type K.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="T">The factory object type.</typeparam>
    public interface IFactory<K, T>
    {
        /// <summary>
        /// Determines whether this instance can create an object based on the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if this instance can create the specified key; otherwise, <c>false</c>.
        /// </returns>
        bool CanCreate(K key);

        /// <summary>
        /// Creates an object of type T based off of the key of type K.
        /// </summary>
        /// <param name="key">The object key.</param>
        /// <param name="args">The optional contructor aruments.</param>
        /// <returns></returns>
        T Create(K key, params object[] args);

        /// <summary>
        /// Gets a list of possible keys that have been registered with this factory.
        /// </summary>
        /// <returns></returns>
        IList<K> GetKeys();
    }
}
