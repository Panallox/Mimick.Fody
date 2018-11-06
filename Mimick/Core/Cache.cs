using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A cache class implementation of the <see cref="ICache{TKey, TValue}"/> interface.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly ReaderWriterLockSlim locking;
        private readonly SortedSet<CacheTime> times;
        private readonly Dictionary<TKey, CacheEntry> values;

        private bool hasMaximumCount;
        private bool hasMaximumTime;
        private int maximumCount;
        private TimeSpan maximumTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache" /> class.
        /// </summary>
        public Cache()
        {
            locking = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            times = new SortedSet<CacheTime>();
            values = new Dictionary<TKey, CacheEntry>();

            hasMaximumCount = false;
            hasMaximumTime = false;
            maximumCount = int.MaxValue;
            maximumTime = TimeSpan.MaxValue;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="TValue"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => GetInternal(key);
            set => AddInternal(key, value);
        }

        /// <summary>
        /// Gets the number of entries within the cache.
        /// </summary>
        public int Count => values.Count;

        /// <summary>
        /// Gets or sets the maximum number of entries which can exist within the cache. If the maximum count is reached,
        /// the oldest records in the cache will be removed.
        /// </summary>
        public int MaximumCount
        {
            get => maximumCount;
            set
            {
                maximumCount = value;
                hasMaximumCount = value != int.MaxValue;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of time that a value within the cache should persist. If this value is
        /// <see cref="F:System.TimeSpan.MaxValue" /> then the values will persist until the end of the application.
        /// </summary>
        public TimeSpan MaximumTime
        {
            get => maximumTime;
            set
            {
                maximumTime = value;
                hasMaximumTime = value != TimeSpan.MaxValue;
            }
        }

        #endregion

        /// <summary>
        /// Adds a value to the cache with the provided key. If an entry already exists for the provided key, the value
        /// is updated to the latest and the entry time is updated.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        public void Add(TKey key, TValue value) => AddInternal(key, value);

        /// <summary>
        /// Adds or updates an entry value within the cache for the provided key and value.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        private void AddInternal(TKey key, TValue value)
        {
            EnsureInCapacity(1);
            locking.EnterWriteLock();

            try
            {
                if (values.TryGetValue(key, out var entry))
                {
                    times.Remove(entry.Time);
                    entry.Time.Time = DateTime.Now + (hasMaximumTime ? MaximumTime : TimeSpan.Zero);
                    entry.Value = value;

                    times.Add(entry.Time);
                }
                else
                {
                    entry = new CacheEntry();
                    entry.Key = key;
                    entry.Time = new CacheTime();
                    entry.Time.Key = key;
                    entry.Time.Time = DateTime.Now + (hasMaximumTime ? MaximumTime : TimeSpan.Zero);
                    entry.Value = value;

                    times.Add(entry.Time);
                    values.Add(key, entry);
                }
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Ensures that the cache has enough capacity remaining for the provided number of elements.
        /// </summary>
        private void EnsureInCapacity(int count)
        {
            if (!hasMaximumCount)
                return;

            var maximum = MaximumCount;
            var current = values.Count;

            if (maximum >= current + count)
                return;

            locking.EnterWriteLock();

            try
            {
                var adjustment = Math.Max(10, count);
                var removals = times.Take(adjustment).ToArray();

                foreach (var remove in removals)
                {
                    values.Remove(remove.Key);
                    times.Remove(remove);
                }
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        public void Clear()
        {
            locking.EnterWriteLock();

            try
            {
                values.Clear();
                times.Clear();
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets a value from the cache with the provided key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>
        /// The cache entry value; otherwise, the default value of <typeparamref name="TValue" />.
        /// </returns>
        public TValue Get(TKey key) => GetInternal(key);

        /// <summary>
        /// Gets a value from the cache with the provided key, and performs any necessary invalidation of the record based on the timeout.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>An entry value; otherwise, the default value of <typeparamref name="TValue"/>.</returns>
        private TValue GetInternal(TKey key)
        {
            locking.EnterUpgradeableReadLock();

            try
            {
                if (values.TryGetValue(key, out var entry))
                {
                    if (hasMaximumTime && DateTime.Now.CompareTo(entry.Time.Time) > 0)
                        RemoveInternal(key);
                    else
                        return entry.Value;
                }
            }
            finally
            {
                locking.ExitUpgradeableReadLock();
            }

            return default(TValue);
        }

        /// <summary>
        /// Removes an entry from the cache with the proivded key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        public void Remove(TKey key) => RemoveInternal(key);

        /// <summary>
        /// Removes an entry value from within the cache for a provided key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        private void RemoveInternal(TKey key)
        {
            locking.EnterWriteLock();

            try
            {
                if (values.TryGetValue(key, out var entry))
                {
                    times.Remove(entry.Time);
                    values.Remove(key);
                }
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets a value from the cache with the provided key, and returns whether the value was retrieved successfully.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value target.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet(TKey key, out TValue value)
        {
            locking.EnterUpgradeableReadLock();

            try
            {
                if (values.TryGetValue(key, out var entry))
                {
                    if (hasMaximumTime && DateTime.Now.CompareTo(entry.Time.Time) > 0)
                        RemoveInternal(key);
                    else
                    {
                        value = entry.Value;
                        return true;
                    }
                }
            }
            finally
            {
                locking.ExitUpgradeableReadLock();
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// A class representing an entry within the cache, containing information on the value.
        /// </summary>
        class CacheEntry
        {
            public TKey Key;
            public CacheTime Time;
            public TValue Value;
        }
        
        /// <summary>
        /// A class representing an entry within the cache time store, containing information on a record.
        /// </summary>
        class CacheTime : IComparable<CacheTime>
        {
            public TKey Key;
            public DateTime Time;

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
            /// </summary>
            /// <param name="other">An object to compare with this instance.</param>
            /// <returns>
            /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
            /// </returns>
            public int CompareTo(CacheTime other)
            {
                return Time.CompareTo(other.Time);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, this))
                    return true;

                return obj is CacheTime o && Key.Equals(o.Key) && Time.Equals(o.Time);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + Key.GetHashCode();
                    hash = hash * 31 + Time.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
