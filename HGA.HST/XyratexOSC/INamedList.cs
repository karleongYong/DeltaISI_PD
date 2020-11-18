using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC
{
    public interface INamedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : INamed
    {
        T this[string name]
        {
            get;
        }

        /// <summary>
        /// Occurs when the list has changed.
        /// </summary>
        event EventHandler ListChanged;

        /// <summary>
        /// Gets the names of all of the elements in this list.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetNames();

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="T:INamedList`1"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="T:INamedList`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <c>T</c> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        void AddRange(IEnumerable<T> collection);

        /// <summary>
        /// Determines whether the <see cref="INamed"/> element with the specified name exists in this <see cref="T:INamedList`1"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>true if an element with the specified name exists in this <see cref="T:INamedList`1"/>; otherwise, false.</returns>
        bool Contains(string name);

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="T:INamedList`1"/> at the specified index.
        /// 
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="T:INamedList`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <c>T</c> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than 0.
        ///             -or-
        ///     <paramref name="index"/> is greater than <see cref="P:INamedList`1.Count"/>.
        /// </exception>
        void InsertRange(int index, IEnumerable<T> collection);
    }
}
