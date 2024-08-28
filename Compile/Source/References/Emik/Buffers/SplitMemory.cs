// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator InvertIf RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
namespace Emik.Morsels;
#pragma warning disable IDE0032
using static SmallList;
using static Span;
using static SplitMemoryFactory;
using static SplitSpanFactory;

/// <summary>Methods to split spans into multiple spans.</summary>
static partial class SplitMemoryFactory
{
    /// <summary>
    /// Defines the values for <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> without a compile-time strategy.
    /// </summary>
    /// <typeparam name="TBody">The type of element from the span.</typeparam>
    /// <typeparam name="TSeparator">The type of separator.</typeparam>
    public interface ISplitMemory<TBody, TSeparator> : IEnumerable<ReadOnlyMemory<TBody>>
    {
        /// <summary>Gets the body.</summary>
        public ReadOnlyMemory<TBody> Body { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

        /// <summary>Gets the separator.</summary>
        public ReadOnlyMemory<TSeparator> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }
    }

    /// <inheritdoc cref="SplitSpanFactory.SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitOnAny<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(span, separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitOnAny<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        span.ReadOnly().SplitOnAny(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitOn<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(span, separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitOn<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        span.ReadOnly().SplitOn(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this ReadOnlyMemory<T> span,
        OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        new(span, separator.Memory);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this Memory<T> span,
        OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        span.ReadOnly().SplitOn(separator);
#endif
    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this ReadOnlyMemory<T> span, OnceMemoryManager<T> separator)
        where T : IEquatable<T> =>
        new(span, separator.Memory);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this Memory<T> span, OnceMemoryManager<T> separator)
        where T : IEquatable<T> =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<byte, byte, MatchOne> SplitOn(this Memory<byte> span, byte separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<byte, byte, MatchOne> SplitOn(this ReadOnlyMemory<byte> span, byte separator) =>
        new(span, separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchOne> SplitOn(this Memory<char> span, char separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchOne> SplitOn(this ReadOnlyMemory<char> span, char separator) =>
        new(span, separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<sbyte, sbyte, MatchOne> SplitOn(this Memory<sbyte> span, sbyte separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<sbyte, sbyte, MatchOne> SplitOn(this ReadOnlyMemory<sbyte> span, sbyte separator) =>
        new(span, separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<short, short, MatchOne> SplitOn(this Memory<short> span, short separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<short, short, MatchOne> SplitOn(this ReadOnlyMemory<short> span, short separator) =>
        new(span, separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<ushort, ushort, MatchOne> SplitOn(this Memory<ushort> span, ushort separator) =>
        span.ReadOnly().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<ushort, ushort, MatchOne> SplitOn(this ReadOnlyMemory<ushort> span, ushort separator) =>
        new(span, separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchOne> SplitOn(this string span, char separator) =>
        new(span.AsMemory(), separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitOnAny(this string span, string separator) =>
        span.AsMemory().SplitOnAny(separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitOnAny(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitOnAny(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOnAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitOnAny(this ReadOnlyMemory<char> span, string separator) =>
        span.SplitOnAny(separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitOn(this string span, string separator) =>
        span.AsMemory().SplitOn(separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitOn(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitOn(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitOn(this ReadOnlyMemory<char> span, string separator) =>
        span.SplitOn(separator.AsMemory());

    /// <inheritdoc cref="SplitSpanFactory.SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this string span) =>
        span.AsMemory().SplitLines();

    /// <inheritdoc cref="SplitSpanFactory.SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this ReadOnlyMemory<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.BreakingSearch.Memory);
#else
        new(span, Whitespaces.Breaking.AsMemory());
#endif
    /// <inheritdoc cref="SplitSpanFactory.SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this Memory<char> span) =>
        span.ReadOnly().SplitLines();

    /// <inheritdoc cref="SplitSpanFactory.SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this string span) =>
        span.AsMemory().SplitWhitespace();

    /// <inheritdoc cref="SplitSpanFactory.SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this ReadOnlyMemory<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.UnicodeSearch.Memory);
#else
        new(span, Whitespaces.Unicode.AsMemory());
#endif
    /// <inheritdoc cref="SplitSpanFactory.SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this Memory<char> span) =>
        span.ReadOnly().SplitWhitespace();
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitSpanFactory.SplitOn{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, SearchValues<char>, MatchAny> SplitOn(
        this string span,
        OnceMemoryManager<SearchValues<char>> separator
    ) =>
        span.AsMemory().SplitOn(separator);
#endif
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="TBody">The type of element from the span.</typeparam>
/// <typeparam name="TSeparator">The type of separator.</typeparam>
/// <typeparam name="TStrategy">The strategy for splitting elements.</typeparam>
[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct SplitMemory<TBody, TSeparator, TStrategy>(
        ReadOnlyMemory<TBody> body,
        ReadOnlyMemory<TSeparator> separator
    ) : IEquatable<object>,
    IEquatable<SplitMemory<TBody, TSeparator, TStrategy>>,
    ISplitMemory<TBody, TSeparator>
    where TBody : IEquatable<TBody>?
#if !NET7_0_OR_GREATER
    where TSeparator : IEquatable<TSeparator>?
#endif
{
    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Accumulator{TAccumulator}"/>
    public delegate TAccumulator RefAccumulator<TAccumulator>(TAccumulator accumulator, ReadOnlyMemory<TBody> next)
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulator : allows ref struct
#endif
    ;

    readonly ReadOnlyMemory<TBody> _body = body;

    readonly ReadOnlyMemory<TSeparator> _separator = separator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> struct.
    /// </summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitMemory(ReadOnlyMemory<TBody> body)
        : this(body, default) { }

    /// <inheritdoc />
    public readonly ReadOnlyMemory<TBody> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.First"/>
    public readonly ReadOnlyMemory<TBody> First
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Last"/>
    public readonly ReadOnlyMemory<TBody> Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetReversedEnumerator() is var e && e.MoveNext() ? e.Current : default;
    }

    /// <inheritdoc />
    public readonly ReadOnlyMemory<TSeparator> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Single"/>
    public readonly ReadOnlyMemory<TBody> Single
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;
    }

    /// <summary>Gets itself as <see cref="SplitSpan{TBody, TSeparator, TStrategy}"/>.</summary>
    public readonly SplitSpan<TBody, TSeparator, TStrategy> SplitSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => new(_body.Span, _separator.Span);
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.this[int]"/>
    public readonly ReadOnlyMemory<TBody> this[[NonNegativeValue] int index]
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

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.this[Index]"/>
    public readonly ReadOnlyMemory<TBody> this[Index index]
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

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.op_Equality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(
        SplitMemory<TBody, TSeparator, TStrategy> left,
        SplitMemory<TBody, TSeparator, TStrategy> right
    ) =>
        left.Equals(right);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.op_Inequality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(
        SplitMemory<TBody, TSeparator, TStrategy> left,
        SplitMemory<TBody, TSeparator, TStrategy> right
    ) =>
        !left.Equals(right);

    /// <summary>
    /// Explicitly converts the parameter by creating the new instance of
    /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> by using the constructor
    /// <see cref="Emik.Morsels.SplitMemory{TBody, TSeparator, TStrategy}(ReadOnlyMemory{TBody})"/>.
    /// </summary>
    /// <param name="body">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/>
    /// by passing the parameter <paramref name="body"/> to the constructor
    /// <see cref="Emik.Morsels.SplitMemory{TBody, TSeparator, TStrategy}(ReadOnlyMemory{TBody})"/>.
    /// </returns>
    [Pure]
    public static explicit operator SplitMemory<TBody, TSeparator, TStrategy>(ReadOnlyMemory<TBody> body) => new(body);

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of
    /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/> by using the constructor
    /// <see cref="Emik.Morsels.SplitMemory{TBody, TSeparator, TStrategy}(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
    /// </summary>
    /// <param name="tuple">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/>
    /// by passing the parameter <paramref name="tuple"/> to the constructor
    /// <see cref="Emik.Morsels.SplitMemory{TBody, TSeparator, TStrategy}(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
    /// </returns>
    [Pure]
    public static implicit operator SplitMemory<TBody, TSeparator, TStrategy>(
        (ReadOnlyMemory<TBody> Body, ReadOnlyMemory<TSeparator> Separator) tuple
    ) =>
        new(tuple.Body, tuple.Separator);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Deconstruct"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out ReadOnlyMemory<TBody> head, out SplitMemory<TBody, TSeparator, TStrategy> tail)
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

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ConcatEqual{TOtherSeparator, TOtherStrategy}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool ConcatEqual<TOtherSeparator, TOtherStrategy>(
         SplitMemory<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        =>
            SplitSpan.ConcatEqual(other.SplitSpan);

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override bool Equals(object? obj) =>
        obj is SplitMemory<TBody, TSeparator, TStrategy> other && Equals(other);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(SplitMemory<TBody, TSeparator, TStrategy> other) =>
        _body.Span.SequenceEqual(other._body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other._separator.Span));

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.SequenceEqual{TOtherSeparator, TOtherStrategy}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool SequenceEqual<TOtherSeparator, TOtherStrategy>(
         SplitMemory<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        =>
            SplitSpan.SequenceEqual(other.SplitSpan);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Count"/>
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
    public readonly override int GetHashCode() =>
        unchecked(typeof(SplitMemory<TBody, TSeparator, TStrategy>).GetHashCode() * 7);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() => SplitSpan.ToString();

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToString(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(ReadOnlyMemory<TBody> divider) => ToString(divider.Span);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToString(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(scoped ReadOnlySpan<TBody> divider) => SplitSpan.ToString(divider);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToStringArray"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string[] ToStringArray() => SplitSpan.ToStringArray();

    /// <summary>Copies the values to a new <see cref="ReadOnlyMemory{T}"/> <see cref="Array"/>.</summary>
    /// <returns>
    /// The <see cref="ReadOnlyMemory{T}"/> <see cref="Array"/> containing the copied values of this instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReadOnlyMemory<TBody>[] ToArrayMemories()
    {
        using var ret = 4.Alloc<ReadOnlyMemory<TBody>>();

        foreach (var next in this)
            ret.Append(next);

        return ret.View.ToArray();
    }

    /// <inheritdoc cref="ToArrayMemories(ReadOnlyMemory{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReadOnlyMemory<TBody>[] ToArrayMemories(ReadOnlyMemory<TBody> divider)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
            return [];

        using var ret = 4.Alloc<ReadOnlyMemory<TBody>>();
        ret.Append(e.Current);

        while (e.MoveNext())
        {
            ret.Append(divider);
            ret.Append(e.Current);
        }

        return ret.View.ToArray();
    }

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MustDisposeResource(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Enumerator GetEnumerator() => new(this);

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MustDisposeResource(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReversedEnumerator GetReversedEnumerator() => new(this);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Skipped"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly SplitMemory<TBody, TSeparator, TStrategy> Skipped([NonNegativeValue] int count)
    {
        Enumerator e = this;

        for (; count > 0 && e.MoveNext(); count--) { }

        return e.SplitMemory;
    }

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.SkippedLast"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly SplitMemory<TBody, TSeparator, TStrategy> SkippedLast([NonNegativeValue] int count)
    {
        ReversedEnumerator e = this;

        for (; count > 0 && e.MoveNext(); count--) { }

        return e.SplitMemory;
    }

    /// <inheritdoc />
    [MustDisposeResource(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator<ReadOnlyMemory<TBody>> IEnumerable<ReadOnlyMemory<TBody>>.GetEnumerator() => GetEnumerator();

    /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="seed">The accumulator.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] RefAccumulator<TAccumulator> func
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

    /// <inheritdoc cref="SplitSpan{TBody,TSeparator,TStrategy}.Aggregate{TAccumulator}(TAccumulator, SplitSpan{TBody, TSeparator, TStrategy}.Accumulator{TAccumulator})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulator, ReadOnlyMemory<TBody>, TAccumulator> func
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

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArray()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray() => SplitSpan.ToArray();

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArray(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(ReadOnlyMemory<TBody> divider) => ToArray(divider.Span);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArray(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(scoped ReadOnlySpan<TBody> divider) => SplitSpan.ToArray(divider);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArrays"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[][] ToArrays() => SplitSpan.ToArrays();

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitMemory{TBody, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public partial struct Enumerator(ReadOnlyMemory<TBody> body, ReadOnlyMemory<TSeparator> separator)
        : IEnumerator<ReadOnlyMemory<TBody>>
    {
        readonly ReadOnlyMemory<TSeparator> _separator = separator;

        readonly ReadOnlyMemory<TBody> _original = body;

        ReadOnlyMemory<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ReadOnlyMemory<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitMemory<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitMemory{TBody, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlyMemory<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _body = value;
        }

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Enumerator.SplitSpan"/>
        public readonly SplitMemory<TBody, TSeparator, TStrategy> SplitMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => new(_body, _separator);
        }

        /// <inheritdoc />
        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc />
        public readonly ReadOnlyMemory<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc cref="SplitMemory{TBody, TSeparator, TStrategy}.Separator"/>
        public readonly ReadOnlyMemory<TSeparator> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _separator = value;
        }

        /// <summary>
        /// Explicitly converts the parameter by creating the new instance
        /// of <see cref="Enumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(ReadOnlyMemory{TBody})"/>.
        /// </summary>
        /// <param name="body">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="Enumerator"/> by passing the
        /// parameter <paramref name="body"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(ReadOnlyMemory{TBody})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static explicit operator SplitMemory<TBody, TSeparator, TStrategy>.Enumerator(
            ReadOnlyMemory<TBody> body
        ) =>
            new(body);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance
        /// of <see cref="Enumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="Enumerator"/> by passing the
        /// parameter <paramref name="tuple"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator SplitMemory<TBody, TSeparator, TStrategy>.Enumerator(
            (ReadOnlyMemory<TBody> Body, ReadOnlyMemory<TSeparator> Separator) tuple
        ) =>
            new(tuple.Body, tuple.Separator);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance
        /// of <see cref="Enumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(SplitMemory{TBody, TSeparator, TStrategy})"/>.
        /// </summary>
        /// <param name="split">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="Enumerator"/> by passing the
        /// parameter <paramref name="split"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.Enumerator(SplitMemory{TBody, TSeparator, TStrategy})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator SplitMemory<TBody, TSeparator, TStrategy>.Enumerator(
            SplitMemory<TBody, TSeparator, TStrategy> split
        ) =>
            new(split);

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Enumerator.Move"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Move(
            ReadOnlyMemory<TSeparator> sep,
            scoped ref ReadOnlyMemory<TBody> body,
            out ReadOnlyMemory<TBody> current
        )
        {
            var b = body.Span;
            var ret = SplitSpan<TBody, TSeparator, TStrategy>.Enumerator.Move(sep.Span, ref b, out var c);
            current = c.AsMemory(body);
            body = b.AsMemory(body);
            return ret;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => Move(_separator, ref _body, out _current);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IEnumerator.Reset() => _body = _original;
    }

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitMemory{T, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public partial struct ReversedEnumerator(ReadOnlyMemory<TBody> body, ReadOnlyMemory<TSeparator> separator)
        : IEnumerator<ReadOnlyMemory<TBody>>
    {
        readonly ReadOnlyMemory<TBody> _original = body;

        readonly ReadOnlyMemory<TSeparator> _separator = separator;

        ReadOnlyMemory<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(ReadOnlyMemory<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(SplitMemory<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitMemory{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlyMemory<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _body = value;
        }

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ReversedEnumerator.SplitSpan"/>
        public readonly SplitMemory<TBody, TSeparator, TStrategy> SplitMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => new(_body, _separator);
        }

        /// <inheritdoc />
        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly ReadOnlyMemory<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc cref="SplitMemory{T, TSeparator, TStrategy}.Separator"/>
        public readonly ReadOnlyMemory<TSeparator> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _separator = value;
        }

        /// <summary>
        /// Explicitly converts the parameter by creating the new instance
        /// of <see cref="ReversedEnumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlyMemory{TBody})"/>.
        /// </summary>
        /// <param name="body">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="ReversedEnumerator"/> by passing
        /// the parameter <paramref name="body"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlyMemory{TBody})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static explicit operator SplitMemory<TBody, TSeparator, TStrategy>.ReversedEnumerator(
            ReadOnlyMemory<TBody> body
        ) =>
            new(body);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance
        /// of <see cref="ReversedEnumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="ReversedEnumerator"/> by passing
        /// the parameter <paramref name="tuple"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(ReadOnlyMemory{TBody}, ReadOnlyMemory{TSeparator})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator SplitMemory<TBody, TSeparator, TStrategy>.ReversedEnumerator(
            (ReadOnlyMemory<TBody> Body, ReadOnlyMemory<TSeparator> Separator) tuple
        ) =>
            new(tuple.Body, tuple.Separator);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance
        /// of <see cref="ReversedEnumerator"/> by using the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(SplitMemory{TBody, TSeparator, TStrategy})"/>.
        /// </summary>
        /// <param name="split">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="ReversedEnumerator"/> by passing
        /// the parameter <paramref name="split"/> to the constructor
        /// <see cref="SplitMemory{TBody, TSeparator, TStrategy}.ReversedEnumerator(SplitMemory{TBody, TSeparator, TStrategy})"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator SplitMemory<TBody, TSeparator, TStrategy>.ReversedEnumerator(
            SplitMemory<TBody, TSeparator, TStrategy> split
        ) =>
            new(split);

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Enumerator.Move"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext(
            ReadOnlyMemory<TSeparator> sep,
            scoped ref ReadOnlyMemory<TBody> body,
            out ReadOnlyMemory<TBody> current
        )
        {
            var b = body.Span;
            var ret = SplitSpan<TBody, TSeparator, TStrategy>.ReversedEnumerator.Move(sep.Span, ref b, out var c);
            current = c.AsMemory(body);
            body = b.AsMemory(body);
            return ret;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => MoveNext(_separator, ref _body, out _current);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IEnumerator.Reset() => _body = _original;
    }
}
#endif
