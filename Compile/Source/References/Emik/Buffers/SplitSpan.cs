// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias
namespace Emik.Morsels;
#pragma warning disable 8631, IDE0032, RCS1158
using static Span;
using static SplitSpanFactory;
#if NET8_0_OR_GREATER
using ComptimeString = SearchValues<char>;

// -
#else
using ComptimeString = char;
#endif

/// <summary>Methods to split spans into multiple spans.</summary>
static partial class SplitSpanFactory
{
    /// <summary>The type that indicates to match all elements.</summary>
    public struct MatchAll;

    /// <summary>The type that indicates to match any element.</summary>
    public struct MatchAny;

    /// <summary>The type that indicates to match exactly one element.</summary>
    public struct MatchOne;

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAny> SplitOnAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAny> SplitOnAny<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            span.ReadOnly().SplitOnAny(separator);

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAll> SplitOn<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAll> SplitOn<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            span.ReadOnly().SplitOn(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this ReadOnlySpan<T> span,
        in SearchValues<T> separator
    )
        where T : IEquatable<T> =>
        new(span, In(separator));

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this Span<T> span,
        in SearchValues<T> separator
    )
        where T : IEquatable<T> =>
        span.ReadOnly().SplitOn(separator);
#endif
    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchOne> SplitOn<T>(this ReadOnlySpan<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
        =>
            new(span, In(separator));

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchOne> SplitOn<T>(this Span<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().SplitOn(separator);
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<byte, byte, MatchOne> SplitOn(this ReadOnlySpan<byte> span, byte separator) =>
        new(span, separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<byte, byte, MatchOne> SplitOn(this Span<byte> span, byte separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchOne> SplitOn(this ReadOnlySpan<char> span, char separator) =>
        new(span, separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchOne> SplitOn(this Span<char> span, char separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<sbyte, sbyte, MatchOne> SplitOn(this ReadOnlySpan<sbyte> span, sbyte separator) =>
        new(span, separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<sbyte, sbyte, MatchOne> SplitOn(this Span<sbyte> span, sbyte separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<short, short, MatchOne> SplitOn(this ReadOnlySpan<short> span, short separator) =>
        new(span, separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<short, short, MatchOne> SplitOn(this Span<short> span, short separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<ushort, ushort, MatchOne> SplitOn(this ReadOnlySpan<ushort> span, ushort separator) =>
        new(span, separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<ushort, ushort, MatchOne> SplitOn(this Span<ushort> span, ushort separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchOne> SplitSpanOn(this string? span, char separator) =>
        span.AsSpan().SplitOn(separator);
#endif
    /// <inheritdoc cref="SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAny> SplitSpanOnAny(this string? span, string? separator) =>
        span.AsSpan().SplitOnAny(separator.AsSpan());

    /// <inheritdoc cref="SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAny> SplitOnAny(this string? span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitOnAny(separator);

    /// <inheritdoc cref="SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAny> SplitOnAny(this ReadOnlySpan<char> span, string? separator) =>
        span.SplitOnAny(separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAll> SplitSpanOn(this string? span, string? separator) =>
        span.AsSpan().SplitOn(separator.AsSpan());

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAll> SplitOn(this string? span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitOn(separator);

    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAll> SplitOn(this ReadOnlySpan<char> span, string? separator) =>
        span.SplitOn(separator.AsSpan());

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitSpanLines(this string? span) =>
        span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitLines(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.BreakingSearch.GetSpan());
#else
        new(span, Whitespaces.Breaking.AsSpan());
#endif
    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitLines(this Span<char> span) =>
        span.ReadOnly().SplitLines();

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitSpanWhitespace(this string? span) =>
        span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitWhitespace(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.UnicodeSearch.Memory.Span);
#else
        new(span, Whitespaces.Unicode.AsSpan());
#endif
    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, ComptimeString, MatchAny> SplitWhitespace(this Span<char> span) =>
        span.ReadOnly().SplitWhitespace();
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitOn{T}(ReadOnlySpan{T}, in SearchValues{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, SearchValues<char>, MatchAny> SplitSpanOn(
        this string? span,
        in SearchValues<char> separator
    ) =>
        span.AsSpan().SplitOn(separator);
#endif
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="TBody">The type of element from the span.</typeparam>
/// <typeparam name="TSeparator">The type of separator.</typeparam>
/// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
/// <param name="body">The line to split.</param>
/// <param name="separator">The separator.</param>
[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct SplitSpan<TBody, TSeparator, TStrategy>(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
#if UNMANAGED_SPAN
    where TBody : unmanaged, IEquatable<TBody>?
#else
    where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
    where TSeparator : IEquatable<TSeparator>?
#endif
{
    /// <summary>Represents the accumulator function for the enumeration of this type.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="accumulator">The accumulator.</param>
    /// <param name="next">The next slice from the enumeration.</param>
    /// <returns>The final accumulator value.</returns>
    public delegate TAccumulator Accumulator<TAccumulator>(TAccumulator accumulator, ReadOnlySpan<TBody> next)
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulator : allows ref struct
#endif
    ;

    readonly ReadOnlySpan<TBody> _body = body;

    readonly ReadOnlySpan<TSeparator> _separator = separator;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T, TSeparator, TStrategy}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<TBody> body)
        : this(body, default) { }

    /// <summary>Gets the error thrown by this type.</summary>
    public static NotSupportedException Error
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => new($"Unrecognized type: {typeof(TStrategy).Name}");
    }

    /// <summary>Gets the line to split.</summary>
    public readonly ReadOnlySpan<TBody> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
    }

    /// <summary>Gets the first element.</summary>
    public readonly ReadOnlySpan<TBody> First
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;
    }

    /// <summary>Gets the last element.</summary>
    public readonly ReadOnlySpan<TBody> Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetReversedEnumerator() is var e && e.MoveNext() ? e.Current : default;
    }

    /// <summary>Gets the separator.</summary>
    public readonly ReadOnlySpan<TSeparator> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
    }

    /// <summary>Gets the single element.</summary>
    public readonly ReadOnlySpan<TBody> Single
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;
    }

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    public readonly ReadOnlySpan<TBody> this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            var e = GetEnumerator();

            for (; index >= 0; index--)
                if (!e.MoveNext())
                    return default;

            return e.Current;
        }
    }

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    public readonly ReadOnlySpan<TBody> this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            if (index.Value is var value && !index.IsFromEnd)
                return this[value];

            var backwards = GetReversedEnumerator();

            for (; value > 0; value--)
                if (!backwards.MoveNext())
                    return default;

            return backwards.Current;
        }
    }

    /// <summary>Gets the specified range.</summary>
    /// <param name="range">The range to get.</param>
    public readonly SplitSpan<TBody, TSeparator, TStrategy> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get =>
            new(
                _body.OffsetOf(this[range.Start]) is not -1 and var start &&
                this[Decrement(range.End)] is var slice &&
                _body.OffsetOf(slice) is not -1 and var end &&
                end + slice.Length - start is > 0 and var length
                    ? _body.UnsafelySlice(start, length)
                    : default,
                _separator
            );
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(
        scoped SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(
        scoped SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        !left.Equals(right);

    /// <summary>
    /// Explicitly converts the parameter by creating the new instance of
    /// <see cref="SplitSpan{TBody, TSeparator, TStrategy}"/> by using the constructor
    /// <see cref="Emik.Morsels.SplitSpan{TBody, TSeparator, TStrategy}(ReadOnlySpan{TBody})"/>.
    /// </summary>
    /// <param name="body">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of SplitSpan{TBody, TSeparator, TStrategy} by passing the parameter <paramref name="body"/>
    /// to the constructor <see cref="Emik.Morsels.SplitSpan{TBody, TSeparator, TStrategy}(ReadOnlySpan{TBody})"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static explicit operator SplitSpan<TBody, TSeparator, TStrategy>(ReadOnlySpan<TBody> body) => new(body);

    /// <summary>Separates the head from the tail of this <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.</summary>
    /// <param name="head">The first element of this enumeration.</param>
    /// <param name="tail">The rest of this enumeration.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out ReadOnlySpan<TBody> head, out SplitSpan<TBody, TSeparator, TStrategy> tail)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
        {
            head = default;
            tail = default;
            return;
        }

        head = e.Current;
        tail = new(e.Body, _separator);
    }

    /// <summary>Determines whether both splits are eventually equal when concatenating all slices.</summary>
    /// <typeparam name="TOtherSeparator">The type of separator for the other side.</typeparam>
    /// <typeparam name="TOtherStrategy">The strategy for splitting for the other side.</typeparam>
    /// <param name="other">The other side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly unsafe bool ConcatEqual<TOtherSeparator, TOtherStrategy>(
        scoped SplitSpan<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
    {
        if (GetEnumerator() is var e && other.GetEnumerator() is var otherE && !e.MoveNext())
            return !otherE.MoveNext();

        if (!otherE.MoveNext())
            return false;

        ReadOnlySpan<TBody> reader = e.Current, otherReader = otherE.Current;

        while (true)
#pragma warning disable 9080 // Dangerous!
            if (e.EqualityMoveNext(ref otherE, ref reader, ref otherReader, out var ret))
#pragma warning restore 9080
                return ret;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Obsolete("Always returns false", true), Pure]
    public readonly override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(scoped SplitSpan<TBody, TSeparator, TStrategy> other) =>
        _body.SequenceEqual(other._body) && _separator.SequenceEqual(other._separator);

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="TOtherSeparator">The type of separator for the right-hand side.</typeparam>
    /// <typeparam name="TOtherStrategy">The strategy for splitting elements for the right-hand side.</typeparam>
    /// <param name="other">The side to compare to.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool SequenceEqual<TOtherSeparator, TOtherStrategy>(
        scoped in SplitSpan<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
    {
        Enumerator e = this;
        var eOther = other.GetEnumerator();

        while (e.MoveNext())
            if (!eOther.MoveNext() || !e.Current.SequenceEqual(eOther.Current))
                return false;

        return !eOther.MoveNext();
    }

    /// <summary>Computes the length.</summary>
    /// <returns>The length.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly int Count()
    {
        var count = 0;

        for (var e = GetEnumerator(); e.MoveNext(); count++) { }

        return count;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override int GetHashCode() => unchecked(typeof(TBody).GetHashCode() * 31);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() =>
        typeof(TBody) == typeof(char)
            ? Aggregate(new StringBuilder(), StringBuilderAccumulator).ToString()
            : $"[[{ToArrays().Conjoin("], [")}]]";

    /// <summary>
    /// Converts the elements of the collection to a <see cref="string"/> representation,
    /// using the specified divider between elements.
    /// </summary>
    /// <param name="divider">The divider to insert between elements.</param>
    /// <returns>A <see cref="string"/> representation of the collection.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(scoped ReadOnlySpan<TBody> divider)
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        if (GetEnumerator() is var e && !e.MoveNext())
            return "";

        using var ret = 4.Alloc<TBody>();
        ret.Append(e.Current);

        while (e.MoveNext())
        {
            ret.Append(divider);
            ret.Append(e.Current);
        }

        return typeof(TBody) == typeof(char) ? ret.View.ToString() : ret.View.ToArray().Conjoin();
#else
        var e = GetEnumerator();

        if (!e.MoveNext())
            return "";

        List<TBody> ret = [];

        foreach (var next in e.Current)
            ret.Add(next);

        while (e.MoveNext())
        {
            foreach (var next in divider)
                ret.Add(next);

            foreach (var next in e.Current)
                ret.Add(next);
        }

        return ret.Conjoin(typeof(TBody) == typeof(char) ? "" : ", ");
#endif
    }

    /// <summary>Copies the values to a new <see cref="string"/> array.</summary>
    /// <returns>The <see cref="string"/> array containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string[] ToStringArray()
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        using var ret = 4.Alloc<string>();

        foreach (var next in this)
            ret.Append(typeof(TBody) == typeof(char) ? next.ToString() : next.ToArray().Conjoin());

        return ret.View.ToArray();
#else
        List<string> ret = [];

        foreach (var next in this)
            ret.Add(typeof(TBody) == typeof(char) ? next.ToString() : next.ToArray().Conjoin());

        return [.. ret];
#endif
    }

    /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="seed">The accumulator.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Accumulator<TAccumulator> func
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulator : allows ref struct
#endif
    {
        var accumulator = seed;

        foreach (var next in this)
            accumulator = func(accumulator, next);

        return accumulator;
    }

    /// <summary>Copies the values to a new flattened array.</summary>
    /// <returns>The array containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray()
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        using var ret = 4.Alloc<TBody>();

        foreach (var next in this)
            ret.Append(next);

        return ret.View.ToArray();
#else
        List<TBody> ret = [];

        foreach (var next in this)
            foreach (var element in next)
                ret.Add(element);

        return [.. ret];
#endif
    }

    /// <summary>Copies the values to a new flattened array.</summary>
    /// <param name="divider">The separator between each element.</param>
    /// <returns>The array containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(scoped ReadOnlySpan<TBody> divider)
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        if (GetEnumerator() is var e && !e.MoveNext())
            return [];

        using var ret = 4.Alloc<TBody>();
        ret.Append(e.Current);

        while (e.MoveNext())
        {
            ret.Append(divider);
            ret.Append(e.Current);
        }

        return ret.View.ToArray();
#else
        if (GetEnumerator() is var e && !e.MoveNext())
            return [];

        List<TBody> ret = [];

        foreach (var next in e.Current)
            ret.Add(next);

        while (e.MoveNext())
        {
            foreach (var next in divider)
                ret.Add(next);

            foreach (var next in e.Current)
                ret.Add(next);
        }

        return [.. ret];
#endif
    }

    /// <summary>Copies the values to a new nested array.</summary>
    /// <returns>The nested array containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[][] ToArrays()
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        using var ret = 4.Alloc<TBody[]>();

        foreach (var next in this)
            ret.Append(next.ToArray());

        return ret.View.ToArray();
#else
        List<TBody[]> ret = [];

        foreach (var next in this)
            ret.Add(next.ToArray());

        return [.. ret];
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static unsafe StringBuilder StringBuilderAccumulator(StringBuilder builder, scoped ReadOnlySpan<TBody> span)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        =>
            builder.Append(To<char>.From(span));
#else
    {
#pragma warning disable 8500
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
        fixed (TBody* pin = span)
        {
            var ptr = span.Align(pin);

            for (var i = 0; i < span.Length; i++)
                builder.Append(((char*)ptr)[i]);
        }

        return builder;
#else
        fixed (TBody* ptr = span)
            return builder.Append((char*)span.Align(ptr), span.Length);
#endif
#pragma warning restore 8500
    }
#endif
    /// <summary>Decrements the index. If already <c>0</c>, flips the "from end" boolean.</summary>
    /// <param name="index">The index to decrement.</param>
    /// <returns>The decremented index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static Index Decrement(Index index) =>
        Unsafe.SizeOf<Index>() is sizeof(int) ?
            Ret<Index>.From(Ret<int>.From(index) - 1) : // Branchless (assumes bit layout)
            index is { Value: 0, IsFromEnd: false } ? new(0, true) :
                new(index.IsFromEnd ? index.Value + 1 : index.Value - 1, index.IsFromEnd);
}
