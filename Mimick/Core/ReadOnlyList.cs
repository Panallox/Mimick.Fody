using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class representing a <see cref="IReadOnlyList{T}"/> implementation. A read-only list copies the elements from
    /// a source collection to ensure that the collection is immutable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        public ReadOnlyList() => collection = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public ReadOnlyList(IEnumerable<T> enumerable) => collection = new List<T>(enumerable);

        #region Properties

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index] => collection[index];

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => collection.Count;

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)collection).GetEnumerator();
    }
}
