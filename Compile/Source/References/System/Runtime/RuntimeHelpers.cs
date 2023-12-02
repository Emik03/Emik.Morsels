// SPDX-License-Identifier: MPL-2.0
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP)
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides a set of static methods and properties that provide support for compilers. This class cannot be inherited.
/// </summary>
static partial class RuntimeHelpers
{
    /// <summary>Gets the byte offset, from the start of the <see cref="string"/> to the first character.</summary>
    /// <remarks><para>
    /// Compilers use this property for unsafe, but efficient, pointer operations on the characters in a managed string.
    /// Compilers should pin the string against movement by the garbage collector before use. Note that common language
    /// runtime strings are immutable; that is, their contents can be read but not changed.
    /// </para></remarks>
    public static unsafe int OffsetToStringData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => sizeof(nint) + 4;
    }

    /// <summary>Slices the specified array using the specified range.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to slice.</param>
    /// <param name="range">
    /// An object that determines the portion of <paramref name="array"/> to include in the slice.
    /// </param>
    /// <returns>The subarray defined by <paramref name="range"/>.</returns>
    [Pure]
    public static T[] GetSubArray<T>(T[] array, Range range)
    {
        range.GetOffsetAndLength(array.Length, out var offset, out var length);

        var isArrayTypeEqual = default(T) is not null || typeof(T[]) == array.GetType();

        if (isArrayTypeEqual && length is 0)
            return [];

        var dest = isArrayTypeEqual ? new T[length] :
            array.GetType().GetElementType() is { } element ? (T[])Array.CreateInstance(element, length) :
            throw Unreachable;

        Array.Copy(array, offset, dest, 0, length);
        return dest;
    }
}
#endif
