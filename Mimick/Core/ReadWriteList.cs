using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class representing a <see cref="List{T}"/> with internal read and write lock support.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ReadWriteList<T> : IList<T>
    {
        private readonly List<T> list;
        private readonly ReaderWriterLockSlim locking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteList{T}"/> class.
        /// </summary>
        public ReadWriteList()
        {
            list = new List<T>();
            locking = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ReadWriteList(int capacity)
        {
            list = new List<T>(capacity);
            locking = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ReadWriteList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
            locking = new ReaderWriterLockSlim();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <typeparamref name="T"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                locking.EnterReadLock();

                try
                {
                    return list[index];
                }
                finally
                {
                    locking.ExitReadLock();
                }
            }

            set
            {
                locking.EnterWriteLock();

                try
                {
                    list[index] = value;
                }
                finally
                {
                    locking.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                locking.EnterReadLock();

                try
                {
                    return list.Count;
                }
                finally
                {
                    locking.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        #endregion

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(T item)
        {
            locking.EnterWriteLock();

            try
            {
                list.Add(item);
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            locking.EnterWriteLock();

            try
            {
                list.Clear();
            }
            finally
            {
                locking.ExitWriteLock();

            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            locking.EnterReadLock();

            try
            {
                return list.Contains(item);
            }
            finally
            {
                locking.ExitReadLock();

            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            locking.EnterReadLock();

            try
            {
                list.CopyTo(array, arrayIndex);
            }
            finally
            {
                locking.ExitReadLock();

            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            locking.EnterReadLock();

            try
            {
                return list.IndexOf(item);
            }
            finally
            {
                locking.ExitReadLock();

            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, T item)
        {
            locking.EnterWriteLock();

            try
            {
                list.Insert(index, item);
            }
            finally
            {
                locking.ExitWriteLock();

            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(T item)
        {
            locking.EnterWriteLock();

            try
            {
                return list.Remove(item);
            }
            finally
            {
                locking.ExitWriteLock();

            }
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            locking.EnterWriteLock();

            try
            {
                list.RemoveAt(index);
            }
            finally
            {
                locking.ExitWriteLock();

            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
