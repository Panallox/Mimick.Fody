using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods for the <see cref="IEnumerable{T}"/> class.
/// </summary>
static class EnumerableExtensions
{
    public static IEnumerable<T> Nested<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> nested, Func<IEnumerable<T>, IEnumerable<T>> perform)
    {
        foreach (var item in perform(enumerable))
        {
            yield return item;

            foreach (var child in perform(nested(item)))
                yield return child;
        }
    }
}
