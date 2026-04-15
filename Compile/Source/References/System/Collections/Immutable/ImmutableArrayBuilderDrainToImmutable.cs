// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Runtime.InteropServices;
#if NETCOREAPP && !NET8_0_OR_GREATER || ROSLYN // ReSharper disable once RedundantNameQualifier
using Unsafe = CompilerServices.Unsafe;

/// <summary>The backport of the DrainToImmutable methods for <see cref="ImmutableArray{T}"/>.</summary>
static partial class ImmutableArrayBuilderDrainToImmutable
{
    /// <summary>
    /// Returns the current contents as an <see cref="ImmutableArray{T}"/>
    /// and sets the collection to a zero length array.
    /// </summary>
    /// <remarks><para>
    /// If <see cref="ImmutableArray{T}.Capacity"/> equals <see cref="ImmutableArray{T}.Count"/>,
    /// the internal array will be extracted as an <see cref="ImmutableArray{T}"/> without copying the contents.
    /// Otherwise, the contents will be copied into a new array. The collection will then be set to a zero length array.
    /// </para></remarks>
    /// <returns>An immutable array.</returns>
    public static ImmutableArray<T> DrainToImmutable<T>(this ImmutableArray<T>.Builder builder)
    {
        if (builder.Capacity == builder.Count)
            return builder.MoveToImmutable();

        var ret = builder.ToImmutable();
        builder.Clear();
        return ret;
    }
}
#endif
