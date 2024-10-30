// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER
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
    public static int OffsetToStringData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Unsafe.SizeOf<nint>() + 4;
    }

    /// <summary>
    /// Returns a value that indicates whether the specified type is a reference
    /// type or a value type that contains references or by-refs.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>
    /// <see langword="true"/> if the given type is a reference type or a value type
    /// that contains references or by-refs; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsReferenceOrContainsReferences<T>() => !typeof(T).IsUnmanaged();

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

        if (default(T) is not null || typeof(T[]) == array.GetType() && length is 0)
            return [];

        var dest = default(T) is not null || typeof(T[]) == array.GetType()
            ? new T[length] // ReSharper disable once NullableWarningSuppressionIsUsed
            : (T[])Array.CreateInstance(array.GetType().GetElementType()!, length);

        Array.Copy(array, offset, dest, 0, length);
        return dest;
    }
}
#endif
