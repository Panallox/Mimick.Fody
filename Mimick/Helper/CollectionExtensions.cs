using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common collection operations.
    /// </summary>
    static class CollectionExtensions
    {
        /// <summary>
        /// Adds an item to the collection if the item does not exist already.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="list">The collection.</param>
        /// <param name="item">The item.</param>
        public static void AddIfMissing<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (list.Contains(item))
                return;

            list.Add(item);
        }
    }
}
