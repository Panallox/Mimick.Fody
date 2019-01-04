using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common collection operations.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds an item to the collection if the item does not exist already.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        public static void AddIfMissing<T>(this ICollection<T> collection, T item)
        {
            if (collection == null)
                throw new ArgumentNullException("list");

            if (collection.Contains(item))
                return;
            
            collection.Add(item);
        }

        /// <summary>
        /// Adds an item to the collection if the key does not exist already.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="dictionary">The collection.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.ContainsKey(key))
                return;

            dictionary.Add(key, value);
        }

        /// <summary>
        /// Counts the number of elements in a non-generic enumerable collection.
        /// </summary>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <returns>The number of elements in the collection.</returns>
        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return 0;

            var count = 0;
            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
                count++;

            return count;
        }

        /// <summary>
        /// Converts the enumerable collection to an immutable read-only list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> value.</returns>
        public static IReadOnlyList<T> ToReadOnly<T>(this IEnumerable<T> enumerable) => new ReadOnlyList<T>(enumerable);
    }
}
