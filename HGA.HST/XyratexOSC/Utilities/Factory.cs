using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// A facotry that can create objects of type T based off of keys of type K.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="T">The factory object type.</typeparam>
    public class Factory<K, T> : IFactory<K, T>
    {
        /// Elements that can be created
        Dictionary<K, IFactoryElement> elements = new Dictionary<K, IFactoryElement>();

        /// <summary>
        ///  Add a new creatable (no args) kind of object to the factory, based on the specified key.
        /// </summary>
        public void AddNewable<V>(K key) where V : T, new()
        {
            elements.Add(key, new NewableElement<V>());
        }

        /// <summary>
        /// Add a creatable kind of object to the factory which can have any number of args.
        /// </summary>
        /// <remarks>
        /// These are not as safe as AddNewable, because the arguments supplied can only be checked at runtime.
        /// </remarks>
        public void AddCreatable<V>(K key) where V : T
        {
            elements.Add(key, new CreateableElement<V>());
        }

        /// <summary>
        /// Determines whether this factory can create an object based on the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if this instance can create the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool CanCreate(K key)
        {
            return elements.ContainsKey(key);
        }

        /// <summary>
        /// Creates a new object based on the specified key with any optional construtor arguments. 
        /// A NewableElement must have already been added to the factory using AddNewable.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Factory cannot create the unknown type ' + typeof(T).Name + '</exception>
        public T Create(K key, params object[] args)
        {
            if (elements.ContainsKey(key))
            {
                return (T)elements[key].New(args);
            }
            throw new ArgumentException("Factory cannot create the unknown type '" + typeof(T).Name + "'");
        }

        /// <summary>
        /// Gets a list of possible keys that have been registered with this factory.
        /// </summary>
        /// <returns></returns>
        public IList<K> GetKeys()
        {
            return elements.Keys.ToList();
        }
    }
}
