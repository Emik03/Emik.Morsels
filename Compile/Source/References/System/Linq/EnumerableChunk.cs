// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NET6_0_OR_GREATER
/// <summary>The backport of the Chunk method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableChunk
{
    /// <summary>Splits the elements of a sequence into chunks of size at most <paramref name="size"/>.</summary>
    /// <remarks><para>
    /// Each chunk except the last one will be of size <paramref name="size"/>.
    /// The last chunk will contain the remaining elements and may be of smaller size.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to chunk.</param>
    /// <param name="size">The maximum size of each chunk.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements the
    /// input sequence split into chunks of size <paramref name="size"/>.
    /// </returns>
    [Pure] // ReSharper disable once CognitiveComplexity
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        using var e = source.GetEnumerator();

        // Before allocating anything, make sure there's at least one element.
        if (!e.MoveNext())
            yield break;

        // Now that we know we have at least one item, allocate an initial storage array. This is not
        // the array we'll yield. It starts out small in order to avoid significantly overallocating
        // when the source has many fewer elements than the chunk size.
        var arraySize = Math.Min(size, 4);
        int i;

        do
        {
            var array = new TSource[arraySize];

            // Store the first item.
            array[0] = e.Current;
            i = 1;

            if (size != array.Length) // This is the first chunk. As we fill the array, grow it as needed.
                for (; i < size && e.MoveNext(); i++)
                {
                    if (i >= array.Length)
                    {
                        arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                        Array.Resize(ref array, arraySize);
                    }

                    array[i] = e.Current;
                }
            else
            {
                // For all but the first chunk, the array will already be correctly sized.
                // We can just store into it until either it's full or MoveNext returns false.
                // ReSharper disable once InlineTemporaryVariable
                // Avoid bounds checks by using cached local. (`array` is lifted to iterator object as a field)
                var local = array;

                // ReSharper disable once RedundantNameQualifier UseSymbolAlias
                System.Diagnostics.Debug.Assert(local.Length == size, "local.Length == size");

                for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                    local[i] = e.Current;
            }

            if (i != array.Length)
                Array.Resize(ref array, i);

            yield return array;
        } while (i >= size && e.MoveNext());
    }
}
#endif
