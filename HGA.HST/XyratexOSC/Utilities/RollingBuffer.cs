using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// A generic collection with a fixed length that rolls over when add past capacity.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public class RollingBuffer<T> : IEnumerable<T>
    {
        private readonly T[] data;
        private int head;
        private int nextInsert = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingBuffer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public RollingBuffer(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("Rolling buffer must have a size greater than 0.");

            this.data = new T[capacity];
            this.head = -capacity;
        }

        /// <summary>
        /// Adds the specified T element. 
        /// </summary>
        /// <param name="element">The specified element to add.</param>
        public void Add(T element)
        {
            data[nextInsert] = element;
            nextInsert = (nextInsert + 1) % data.Length;
            if (head < 0)
                head++;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator&lt;T&gt;"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (head < 0)
            {
                for (int i = 0; i < nextInsert; i++)
                    yield return data[i];
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                    yield return data[(nextInsert + i) % data.Length];
            }
        }

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
