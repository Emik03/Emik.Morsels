// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf InvocationIsSkipped RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias

namespace Emik.Morsels;
#pragma warning disable 8618, 9193, CA1823, IDE0250, MA0071, MA0102, RCS1158, SA1137
using static SmallList;
using static Span;
using static SplitSpanFactory;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitSpanFactory
#pragma warning restore MA0048
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
    public static SplitSpan<T, T, MatchAny> SplitAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAny> SplitAny<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitAny(separator);

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAll> SplitAll<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchAll> SplitAll<T>(this Span<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitAll(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this ReadOnlySpan<T> span,
        in SearchValues<T> separator
    )
        where T : IEquatable<T> =>
        new(span, In(separator));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this Span<T> span,
        in SearchValues<T> separator
    )
        where T : IEquatable<T> =>
        ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchOne> SplitOn<T>(this ReadOnlySpan<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
        =>
            new(span, In(separator));

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T, T, MatchOne> SplitOn<T>(this Span<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAny> SplitSpanAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAny> SplitAny(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAll> SplitSpanAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchAll> SplitAll(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitSpanLines(this string span) =>
        span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.BreakingSearchMemory.Span);
#else
        new(span, Whitespaces.Breaking.AsSpan());
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, char, MatchOne> SplitOn(this string span, in char separator) =>
        span.AsSpan().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitSpanWhitespace(this string span) =>
        span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.UnicodeSearchMemory.Span);
#else
        new(span, Whitespaces.Unicode.AsSpan());
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this Span<char> span) =>
        ((ReadOnlySpan<char>)span).SplitWhitespace();
#endif
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char, SearchValues<char>, MatchAny> SplitSpanOn(
        this string span,
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
    public delegate TAccumulator Accumulator<TAccumulator>(TAccumulator accumulator, scoped ReadOnlySpan<TBody> next);

    /// <inheritdoc cref="Accumulator{TAccumulator}"/>
    public delegate TAccumulator RefAccumulator<TAccumulator>(
        TAccumulator accumulator,
        scoped in ReadOnlySpan<TBody> next
    );

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
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
    public readonly ReadOnlySpan<TBody> this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "must be positive");

            var e = GetEnumerator();

            for (var i = 0; i <= index; i++)
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
            {
                var forwards = GetEnumerator();

                for (var i = 0; i <= value; i++)
                    if (!forwards.MoveNext())
                        return default;

                return forwards.Current;
            }

            var backwards = GetReversedEnumerator();

            for (var i = 0; i <= value; i++)
                if (!backwards.MoveNext())
                    return default;

            return backwards.Current;
        }
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(
        scoped in SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped in SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(
        scoped in SplitSpan<TBody, TSeparator, TStrategy> left,
        scoped in SplitSpan<TBody, TSeparator, TStrategy> right
    ) =>
        !left.Equals(right);

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
    public readonly bool ConcatEqual<TOtherSeparator, TOtherStrategy>(
        scoped in SplitSpan<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
    {
        if (GetEnumerator() is var e && other.GetEnumerator() is var otherE && !e.MoveNext())
            return !otherE.MoveNext();

        if (!otherE.MoveNext())
            return false;

        ReadOnlySpan<TBody>
            reader = e.Current,
            otherReader = otherE.Current;

        while (true)
            if (e.EqualityMoveNext(ref otherE, ref reader, ref otherReader, out var ret))
                return ret;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals<TOtherStrategy>(scoped in SplitSpan<TBody, TSeparator, TOtherStrategy> other) =>
        typeof(TStrategy) == typeof(TOtherStrategy) &&
        _body.SequenceEqual(other._body) &&
        _separator.SequenceEqual(To<TSeparator>.From(other._separator));

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
        var e = GetEnumerator();
        var eOther = other.GetEnumerator();

        while (e.MoveNext())
            if (!(eOther.MoveNext() && e.Current.SequenceEqual(eOther.Current)))
                return false;

        return !eOther.MoveNext();
    }

    /// <summary>Computes the length.</summary>
    /// <returns>The length.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly int Count()
    {
        var e = GetEnumerator();
        var count = 0;

        while (e.MoveNext())
            count++;

        return count;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override int GetHashCode() => unchecked(typeof(TBody).GetHashCode() * 31);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() =>
        typeof(TBody) == typeof(char)
            ? Aggregate(new(), StringBuilderAccumulator()).ToString()
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            : ToArrays().Stringify(3, true);
#else
            : throw new NotSupportedException();
#endif

    /// <summary>
    /// Converts the elements of the collection to a <see cref="string"/> representation,
    /// using the specified divider between elements.
    /// </summary>
    /// <param name="divider">The divider to insert between elements.</param>
    /// <returns>A <see cref="string"/> representation of the collection.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(ReadOnlySpan<TBody> divider) => ToString(in divider);

    /// <inheritdoc cref="ToString(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(scoped in ReadOnlySpan<TBody> divider)
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        if (GetEnumerator() is var e && !e.MoveNext())
            return "";

        using var ret = New4<TBody>();
        ret.Append(e.Current);

        while (e.MoveNext())
            ret.Append(divider).Append(e.Current);

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

    /// <summary>Copies the values to a new <see cref="string"/> <see cref="Array"/>.</summary>
    /// <returns>The <see cref="string"/> <see cref="Array"/> containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string[] ToStringArray()
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        using var ret = New4<string>();

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
    {
        var accumulator = seed;

        foreach (var next in this)
            accumulator = func(accumulator, next);

        return accumulator;
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Aggregate{TAccumulator}(TAccumulator, Accumulator{TAccumulator})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] RefAccumulator<TAccumulator> func
    )
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
        using var ret = New4<TBody>();

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
    public readonly TBody[] ToArray(ReadOnlySpan<TBody> divider) => ToArray(in divider);

    /// <inheritdoc cref="ToArray(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(scoped in ReadOnlySpan<TBody> divider)
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        if (GetEnumerator() is var e && !e.MoveNext())
            return [];

        using var ret = New4<TBody>();
        ret.Append(e.Current);

        while (e.MoveNext())
            ret.Append(divider).Append(e.Current);

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
        using var ret = New4<TBody[]>();

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
    static unsafe RefAccumulator<StringBuilder> StringBuilderAccumulator() =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        static (StringBuilder builder, in ReadOnlySpan<TBody> span) => builder.Append(
            MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.As<TBody, char>(ref MemoryMarshal.GetReference(span)),
                span.Length
            )
        );
#else
        static (StringBuilder builder, in ReadOnlySpan<TBody> span) =>
        {
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
            for (var i = 0; i < span.Length; i++)
                builder.Append(((char*)span.Pointer)[i]);

            return builder;
#else
#pragma warning disable 8500
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
            var ptr = span.Pointer;
#else
            fixed (TBody* ptr = span)
#endif
#pragma warning restore 8500
                return builder.Append((char*)ptr, span.Length);
#endif
        };
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static ReadOnlySpan<TBody> UnsafelyAdvance(ReadOnlySpan<TBody> body, int start) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start),
            body.Length - start
        );
#else
        body[start..];
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static ReadOnlySpan<TBody> UnsafelyTake(ReadOnlySpan<TBody> body, int end) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(body), end);
#else
        body[..end];
#endif
}
