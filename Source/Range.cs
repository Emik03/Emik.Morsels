#region Emik.MPL

// <copyright file="Range.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

namespace System;

#pragma warning disable MA0008, MA0048, SA1515, SA1611, SA1615, SA1623, SA1642
#if !NETSTANDARD2_1 && !NETCOREAPP
/// <summary>Represent a range has start and end indexes.</summary>
/// <remarks><para>
/// Range is used by the C# compiler to support the range syntax.
/// </para><code>
/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
/// int[] subArray1 = someArray[0..2]; // { 1, 2 }
/// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
/// </code></remarks>
readonly partial struct Range : IEquatable<Range>
{
    /// <summary>Construct a Range object using the start and end indexes.</summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="end">Represent the exclusive end index of the range.</param>
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Create a Range object starting from first element to the end.</summary>
    [Pure]
    public static Range All => new(Index.Start, Index.End);

    /// <summary>Represent the inclusive start index of the Range.</summary>
    [Pure]
    public Index Start { get; }

    /// <summary>Represent the exclusive end index of the Range.</summary>
    [Pure]
    public Index End { get; }

    /// <summary>Create a Range object starting from first element in the collection to the end Index.</summary>
    [Pure]
    public static Range EndAt(Index end) => new(Index.Start, end);

    /// <summary>Create a Range object starting from start index to the end of the collection.</summary>
    [Pure]
    public static Range StartAt(Index start) => new(start, Index.End);

    /// <inheritdoc />
    [Pure]
    public override bool Equals([NotNullWhen(true)] object? value) =>
        value is Range r && r.Start.Equals(Start) && r.End.Equals(End);

    /// <inheritdoc />
    [Pure]
    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => Start.GetHashCode() * 31 + End.GetHashCode();

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"{Start}..{End}";
}
#endif

/// <summary>Implements a <see cref="GetOffsetAndLength"/> overload that doesn't rely on tuples.</summary>
static class RangeMethodsWithoutValueTuples
{
#pragma warning disable CS1574
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
#pragma warning restore CS1574
    public static void GetOffsetAndLength(this Range range, int length, out int outOffset, out int outLength)
    {
        var startIndex = range.Start;
        var start = startIndex.IsFromEnd ? length - startIndex.Value : startIndex.Value;

        var endIndex = range.End;
        var end = endIndex.IsFromEnd ? length - endIndex.Value : endIndex.Value;

        if ((uint)end > (uint)length || (uint)start > (uint)end)
            throw new ArgumentOutOfRangeException(nameof(length));

        outOffset = start;
        outLength = end - start;
    }
}
