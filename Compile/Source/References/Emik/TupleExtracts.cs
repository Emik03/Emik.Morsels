// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if !NET47 && !NETSTANDARD2_0 // Filter required for frameworks whose tuples don't implement ITuple.
/// <summary>Methods to get elements of a tuple.</summary>
static partial class TupleExtracts
{
    /// <summary>Gets the enumeration of the tuple.</summary>
    /// <typeparam name="T">The type of tuple.</typeparam>
    /// <param name="tuple">The tuple to enumerate.</param>
    /// <returns>The enumeration of the parameter <paramref name="tuple"/>.</returns>
    public static IEnumerable<object?> AsEnumerable<T>(this T tuple)
        where T : ITuple =>
        Enumerable.Repeat(tuple, tuple.Length).Select((x, i) => x[i]);
}
#endif
