using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common collection operations.
    /// </summary>
    public static partial class Extensions
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
        /// Gets whether the collection contains all of the provided values.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if the collection contains all values; otherwise, <c>false</c>.</returns>
        public static bool ContainsAll<T>(this ICollection<T> collection, params T[] values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var value in values)
            {
                if (!collection.Contains(value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether the collection contains any of the provided values.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if the collection contains any values; otherwise, <c>false</c>.</returns>
        public static bool ContainsAny<T>(this ICollection<T> collection, params T[] values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var value in values)
            {
                if (collection.Contains(value))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Gets whether the collection is <c>null</c> or contains no values.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if the collection is empty; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;
        
        /// <summary>
        /// Gets whether the collection is not <c>null</c> and contains values.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if the collection is not empty; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>(this ICollection<T> collection) => !IsEmpty(collection);

        /// <summary>
        /// Removes a collection of values from the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="values">The values to remove.</param>
        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T element in values)
                collection.Remove(element);
        }
    }
}
