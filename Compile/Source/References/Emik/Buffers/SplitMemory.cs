// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator InvertIf RedundantExtendsListEntry RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
namespace Emik.Morsels;
#pragma warning disable 8618, 9193, CA1823, IDE0250, IDE0251, MA0071, MA0102, RCS1158, SA1137
using static SmallList;
using static Span;
using static SplitMemoryFactory;
using static SplitSpanFactory;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitMemoryFactory
#pragma warning restore MA0048
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

    /// <inheritdoc cref="SplitSpanFactory.SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitAny<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(span, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAny> SplitAny<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)span).SplitAny(separator);

    /// <inheritdoc cref="SplitSpanFactory.SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitAll<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        new(span, separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchAll> SplitAll<T>(this Memory<T> span, ReadOnlyMemory<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)span).SplitAll(separator);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this ReadOnlyMemory<T> span,
        in OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        new(span, separator.Memory);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, SearchValues<T>, MatchAny> SplitOn<T>(
        this Memory<T> span,
        in OnceMemoryManager<SearchValues<T>> separator
    )
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this ReadOnlyMemory<T> span, in OnceMemoryManager<T> separator)
        where T : IEquatable<T> =>
        new(span, separator.Memory);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<T, T, MatchOne> SplitOn<T>(this Memory<T> span, in OnceMemoryManager<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlyMemory<T>)span).SplitOn(separator);
#endif

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitAny(this string span, string separator) =>
        span.AsMemory().SplitAny(separator.AsMemory());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitAny(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitAny(separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAny> SplitAny(this ReadOnlyMemory<char> span, string separator) =>
        span.SplitAny(separator.AsMemory());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitAll(this string span, string separator) =>
        span.AsMemory().SplitAll(separator.AsMemory());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitAll(this string span, ReadOnlyMemory<char> separator) =>
        span.AsMemory().SplitAll(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, char, MatchAll> SplitAll(this ReadOnlyMemory<char> span, string separator) =>
        span.SplitAll(separator.AsMemory());

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this string span) =>
        span.AsMemory().SplitLines();

    /// <inheritdoc cref="SplitSpanFactory.SplitSpanLines"/>
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

    /// <inheritdoc cref="SplitLines(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitLines(this Memory<char> span) =>
        ((ReadOnlyMemory<char>)span).SplitLines();

    // /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    // [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    // public static SplitMemory<char, char, One> SplitOn(this string span, in char separator) =>
    //     span.AsMemory().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this string span) =>
        span.AsMemory().SplitWhitespace();

    /// <inheritdoc cref="SplitSpanFactory.SplitSpanWhitespace"/>
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

    /// <inheritdoc cref="SplitWhitespace(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char,
#if NET8_0_OR_GREATER
        SearchValues<char>,
#else
        char,
#endif
        MatchAny> SplitWhitespace(this Memory<char> span) =>
        ((ReadOnlyMemory<char>)span).SplitWhitespace();

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlyMemory{T}, ReadOnlyMemory{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitMemory<char, SearchValues<char>, MatchAny> SplitOn(
        this string span,
        in OnceMemoryManager<SearchValues<char>> separator
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
    IEquatable<ISplitMemory<TBody, TSeparator>>,
    IEquatable<SplitMemory<TBody, TSeparator, TStrategy>>,
    ISplitMemory<TBody, TSeparator>
    where TBody : IEquatable<TBody>?
#if !NET7_0_OR_GREATER
    where TSeparator : IEquatable<TSeparator>?
#endif
{
    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Accumulator{TAccumulator}"/>
    public delegate TAccumulator RefAccumulator<TAccumulator>(
        TAccumulator accumulator,
        scoped in ReadOnlyMemory<TBody> next
    );

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

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override bool Equals(object? obj) =>
        obj is SplitMemory<TBody, TSeparator, TStrategy> other && Equals(other);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(ISplitMemory<TBody, TSeparator>? other) =>
        other?.GetType() == typeof(SplitMemory<TBody, TSeparator, TStrategy>) &&
        _body.Span.SequenceEqual(other.Body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other.Separator.Span));

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ConcatEqual{TOtherSeparator, TOtherStrategy}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool ConcatEqual<TOtherSeparator, TOtherStrategy>(
        SplitMemory<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        =>
            ConcatEqual(in other);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ConcatEqual{TOtherSeparator, TOtherStrategy}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool ConcatEqual<TOtherSeparator, TOtherStrategy>(
        scoped in SplitMemory<TBody, TOtherSeparator, TOtherStrategy> other
    )
#if !NET7_0_OR_GREATER
        where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        =>
            SplitSpan.ConcatEqual(other.SplitSpan);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(SplitMemory<TBody, TSeparator, TStrategy> other) =>
        _body.Span.SequenceEqual(other._body.Span) &&
        _separator.Span.SequenceEqual(To<TSeparator>.From(other._separator.Span));

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals<TOtherStrategy>(scoped in SplitMemory<TBody, TSeparator, TOtherStrategy> other) =>
        typeof(TStrategy) == typeof(TOtherStrategy) &&
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
            ConcatEqual(in other);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.SequenceEqual{TOtherSeparator, TOtherStrategy}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool SequenceEqual<TOtherSeparator, TOtherStrategy>(
        scoped in SplitMemory<TBody, TOtherSeparator, TOtherStrategy> other
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
    public readonly string ToString(scoped in ReadOnlyMemory<TBody> divider) => ToString(divider.Span);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToString(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(ReadOnlySpan<TBody> divider) => SplitSpan.ToString(divider);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToString(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToString(scoped in ReadOnlySpan<TBody> divider) => SplitSpan.ToString(divider);

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
        using var ret = New4<ReadOnlyMemory<TBody>>();

        foreach (var next in this)
            ret.Append(next);

        return ret.View.ToArray();
    }

    /// <summary>Copies the values to a new <see cref="ReadOnlyMemory{T}"/> <see cref="Array"/>.</summary>
    /// <param name="divider">The separator between each element.</param>
    /// <returns>
    /// The <see cref="ReadOnlyMemory{T}"/> <see cref="Array"/> containing the copied values of this instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReadOnlyMemory<TBody>[] ToArrayMemories(ReadOnlyMemory<TBody> divider) =>
        ToArrayMemories(in divider);

    /// <inheritdoc cref="ToArrayMemories(ReadOnlyMemory{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReadOnlyMemory<TBody>[] ToArrayMemories(scoped in ReadOnlyMemory<TBody> divider)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
            return [];

        using var ret = New4<ReadOnlyMemory<TBody>>();
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

    /// <inheritdoc />
    [MustDisposeResource(false), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator<ReadOnlyMemory<TBody>> IEnumerable<ReadOnlyMemory<TBody>>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="SplitSpan{TBody,TSeparator,TStrategy}.Aggregate{TAccumulator}(TAccumulator, SplitSpan{TBody, TSeparator, TStrategy}.RefAccumulator{TAccumulator})"/>
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

    /// <inheritdoc cref="SplitSpan{TBody,TSeparator,TStrategy}.Aggregate{TAccumulator}(TAccumulator, SplitSpan{TBody, TSeparator, TStrategy}.Accumulator{TAccumulator})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public readonly TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulator, ReadOnlyMemory<TBody>, TAccumulator> func
    )
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
    public readonly TBody[] ToArray(scoped in ReadOnlyMemory<TBody> divider) => ToArray(divider.Span);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArray(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(ReadOnlySpan<TBody> divider) => SplitSpan.ToArray(divider);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArray(ReadOnlySpan{TBody})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[] ToArray(scoped in ReadOnlySpan<TBody> divider) => SplitSpan.ToArray(divider);

    /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.ToArrays"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly TBody[][] ToArrays() => SplitSpan.ToArrays();

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static ReadOnlyMemory<T> Convert<T>(in ReadOnlyMemory<T> memory, scoped in ReadOnlySpan<T> span) =>
        Unsafe.IsNullRef(ref MemoryMarshal.GetReference(span))
            ? default
            : memory.Slice(
                (int)(Unsafe
                       .ByteOffset(ref MemoryMarshal.GetReference(memory.Span), ref MemoryMarshal.GetReference(span)) /
                    (nint)(uint)Unsafe.SizeOf<T>()),
                span.Length
            );

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitMemory{T, TSeparator, TStrategy}"/>.
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

        /// <inheritdoc cref="SplitMemory{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlyMemory<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _body = value;
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

        /// <inheritdoc cref="SplitMemory{T, TSeparator, TStrategy}.Separator"/>
        public readonly ReadOnlyMemory<TSeparator> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _separator = value;
        }

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Enumerator.Move"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Move(
            scoped in ReadOnlyMemory<TSeparator> sep,
            scoped ref ReadOnlyMemory<TBody> body,
            out ReadOnlyMemory<TBody> current
        )
        {
            var b = body.Span;
            var ret = SplitSpan<TBody, TSeparator, TStrategy>.Enumerator.Move(sep.Span, ref b, out var c);
            current = Convert(body, c);
            body = Convert(body, b);
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

        /// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}.Enumerator.Move"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext(
            scoped in ReadOnlyMemory<TSeparator> sep,
            scoped ref ReadOnlyMemory<TBody> body,
            out ReadOnlyMemory<TBody> current
        )
        {
            var b = body.Span;
            var ret = SplitSpan<TBody, TSeparator, TStrategy>.ReversedEnumerator.MoveNext(sep.Span, ref b, out var c);
            current = Convert(body, c);
            body = Convert(body, b);
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
