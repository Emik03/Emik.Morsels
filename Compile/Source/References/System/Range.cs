// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace EmptyNamespace RedundantExtendsListEntry StructCanBeMadeReadOnly
namespace System;
#pragma warning disable IDE0250, MA0008, MA0048, MA0102, SA1137, SA1515, SA1611, SA1615, SA1623, SA1642, SA1649
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP
/// <summary>Represent a range has start and end indexes.</summary>
/// <remarks><para>
/// Range is used by the C# compiler to support the range syntax.
/// </para><code>
/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
/// int[] subArray1 = someArray[0..2]; // { 1, 2 }
/// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
/// </code></remarks>
#if !NO_READONLY_STRUCTS
readonly
#endif
partial struct Range : IEquatable<Range>
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

    /// <inheritdoc cref="object.Equals(object?)"/>
    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Range r && r.Start.Equals(Start) && r.End.Equals(End);

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
