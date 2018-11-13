using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a cache containing values. A cache functions similar to a <see cref="IDictionary{TKey, TValue}"/> implementation,
    /// but exposes additional configurations for managing the capacity and duration of entries, and supports automatic thread-safety.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public interface ICache<TKey, TValue>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the value from the cache with the provided key.
        /// </summary>
        /// <param name="key">The cache entry key.</param>
        /// <returns>The cache entry value; otherwise, the default value of <typeparamref name="TValue"/>.</returns>
        TValue this[TKey key]
        {
            get; set;
        }

        /// <summary>
        /// Gets the number of entries within the cache.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets or sets the maximum number of entries which can exist within the cache. If the maximum count is reached,
        /// the oldest records in the cache will be removed.
        /// </summary>
        int MaximumCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maximum amount of time that a value within the cache should persist. If this value is
        /// <see cref="TimeSpan.MaxValue"/> then the values will persist until the end of the application.
        /// </summary>
        TimeSpan MaximumTime
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Adds a value to the cache with the provided key. If an entry already exists for the provided key, the value
        /// is updated to the latest and the entry time is updated.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value from the cache with the provided key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>The cache entry value; otherwise, the default value of <typeparamref name="TValue"/>.</returns>
        TValue Get(TKey key);

        /// <summary>
        /// Removes an entry from the cache with the proivded key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        void Remove(TKey key);

        /// <summary>
        /// Gets a value from the cache with the provided key, and returns whether the value was retrieved successfully.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value target.</param>
        /// <returns><c>true</c> if the value was retrieved; otherwise, <c>false</c>.</returns>
        bool TryGet(TKey key, out TValue value);
    }
}
