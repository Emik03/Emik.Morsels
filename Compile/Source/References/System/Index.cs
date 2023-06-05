// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP
#pragma warning disable MA0008, SA1515, SA1611, SA1615, SA1623, SA1642
// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
/// <remarks><para>
/// Index is used by the C# compiler to support the new index syntax.
/// </para><code>
/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 } ;
/// int lastElement = someArray[^1]; // lastElement = 5
/// </code></remarks>
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Index : IEquatable<Index>
{
    readonly int _value;

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks><para>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means
    /// pointing at beyond last element.
    /// </para></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index([NonNegativeValue] int value, bool fromEnd = false) =>
        // Runtime check.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        _value = value >= 0
            ? fromEnd ? ~value : value
            : throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

    // The following private constructors mainly created for perf reason to avoid the checks
    Index([NonNegativeValue] int value) => _value = value;

    /// <summary>Create an Index pointing at first element.</summary>
    public static Index Start => new(0);

    /// <summary>Create an Index pointing at beyond last element.</summary>
    public static Index End => new(~0);

    /// <summary>Indicates whether the index is from the start or the end.</summary>
    [Pure]
    public bool IsFromEnd => _value < 0;

    /// <summary>Returns the index value.</summary>
    [NonNegativeValue, Pure]
    public int Value => _value < 0 ? ~_value : _value;

    /// <summary>Converts integer number to an Index.</summary>
    [Pure]
    public static implicit operator Index([NonNegativeValue] int value) => FromStart(value);

    /// <summary>Create an Index from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Index FromEnd([NonNegativeValue] int value) =>
        // Runtime check.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        value >= 0
            ? new(~value)
            : throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

    /// <summary>Create an Index from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Index FromStart([NonNegativeValue] int value) =>
        // Runtime check.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        value >= 0
            ? new(value)
            : throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

    /// <inheritdoc cref="object.Equals(object?)" />
    [Pure]
    public override bool Equals([NotNullWhen(true)] object? value) => value is Index index && _value == index._value;

    /// <inheritdoc />
    [Pure]
    public bool Equals(Index other) => _value == other._value;

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => _value;

    /// <summary>Calculate the offset from the start using the giving collection length.</summary>
    /// <param name="length">
    /// The length of the collection that the Index will be used with. length has to be a positive value.
    /// </param>
    /// <remarks><para>
    /// For performance reasons, we don't validate the input length parameter and the returned offset value against
    /// negative values. We don't validate either the returned offset is greater than the input length.
    /// It is expected that Index be used with collections which always have non-negative length/count.
    /// If the returned offset is negative and then used to index a collection will get out of range exception which
    /// will be same affect as the validation.
    /// </para></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int GetOffset(int length)
    {
        var offset = _value;

        if (IsFromEnd)
            // offset = length - (~value)
            // offset = length + (~(~value) + 1)
            // offset = length + value + 1
            offset += length + 1;

        return offset;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => IsFromEnd ? $"^{(uint)Value}" : ((uint)Value).ToString();
}
#endif
