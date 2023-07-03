// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using Range = System.Range;

/// <summary>Implements a <see cref="GetOffsetAndLength"/> overload that doesn't rely on tuples.</summary>
static partial class RangeMethodsWithoutValueTuples
{
    /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
    /// <remarks><para>
    /// For performance reasons, we don't validate the input length parameter against negative values.
    /// It is expected Range will be used with collections which always have non negative length/count.
    /// We validate the range is inside the length scope though.
    /// </para></remarks>
    /// <param name="range">The <see cref="Range"/> that contains the range of elements.</param>
    /// <param name="length">
    /// The length of the collection that the range will be used with.
    /// <paramref name="length"/> has to be a positive value.
    /// </param>
    /// <param name="outOffset">The resulting offset.</param>
    /// <param name="outLength">The resulting length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetOffsetAndLength(this Range range, int length, out int outOffset, out int outLength)
    {
        if (!TryGetOffsetAndLength(range, length, out outOffset, out outLength))
            throw new ArgumentOutOfRangeException(nameof(length));
    }

    /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
    /// <param name="range">The <see cref="Range"/> that contains the range of elements.</param>
    /// <param name="length">
    /// The length of the collection that the range will be used with.
    /// <paramref name="length"/> has to be a positive value.
    /// </param>
    /// <param name="outOffset">The resulting offset.</param>
    /// <param name="outLength">The resulting length.</param>
    /// <returns>Whether the values are set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryGetOffsetAndLength(this Range range, int length, out int outOffset, out int outLength)
    {
        var startIndex = range.Start;
        var start = startIndex.IsFromEnd ? length - startIndex.Value : startIndex.Value;

        var endIndex = range.End;
        var end = endIndex.IsFromEnd ? length - endIndex.Value : endIndex.Value;

        outOffset = start;
        outLength = end - start;

        return unchecked((uint)end <= (uint)length && (uint)start <= (uint)end);
    }
}
