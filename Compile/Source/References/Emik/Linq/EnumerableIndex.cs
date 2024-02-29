// SPDX-License-Identifier: MPL-2.0

// NOTE: This file should be moved to ./Compile/Source/References/System/Linq/EnumerableIndex.cs when .NET 9 is released
// and CSharpRepl is updated to use it, as anything in ./Compile/Source/References/System/ is not included in REPL.csx.
#if !CSHARPREPL
// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#endif
#if !NETCOREAPP9_0_OR_GREATER
/// <summary>The backport of the Index method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableIndex
{
    /// <summary>Returns an enumerable that incorporates the element's index into a tuple.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">The source enumerable providing the elements.</param>
    /// <returns>The enumerable that incorporates the element's index into a tuple.</returns>
    public static IEnumerable<(int Index, TSource Item)> Index<TSource>(this IEnumerable<TSource> source)
    {
        var index = 0;

        foreach (var item in source)
            yield return (index++, item);
    }
}
#endif
