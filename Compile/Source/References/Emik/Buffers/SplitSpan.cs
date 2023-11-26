// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace InvertIf RedundantUsingDirective StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable 8618, IDE0250, MA0071, MA0102, SA1137
using static Span;

/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitSpanFactory
#pragma warning restore MA0048
{
    /// <summary>Determines whether both splits are eventually equal when concatenating all slices.</summary>
    /// <typeparam name="T">The type of <see cref="SplitSpan{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool ConcatEqual<T>(this SplitSpan<T> left, SplitSpan<T> right)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        if (left == right)
            return true;

        if (left.GetEnumerator() is var e1 && right.GetEnumerator() is var e2 && !e1.MoveNext())
            return !e2.MoveNext();

        if (!e2.MoveNext())
            return false;

        ReadOnlySpan<T>
            reader1 = e1.Current,
            reader2 = e2.Current;

        while (true)
            if (Next(ref reader1, ref reader2, ref e1, ref e2, out var ret))
                return ret;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <typeparam name="T">The type of <see cref="SplitSpan{T}"/>.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <paramref langword="true"/> if both sequences are equal, otherwise; <paramref langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this SplitSpan<T> left, SplitSpan<T> right)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        if (left == right)
            return true;

        var e1 = left.GetEnumerator();
        var e2 = right.GetEnumerator();

        while (e1.MoveNext())
            if (!(e2.MoveNext() && e1.Current.SequenceEqual(e2.Current)))
                return false;

        return !e2.MoveNext();
    }

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator, true);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAny<T>(this Span<T> span, ReadOnlySpan<T> separator)
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
    public static SplitSpan<T> SplitAll<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator, false);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitAll<T>(this Span<T> span, ReadOnlySpan<T> separator)
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
    public static SplitSpan<T> SplitOn<T>(this ReadOnlySpan<T> span, SearchValues<T> separator)
        where T : IEquatable<T> =>
        new(span, separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitOn<T>(this Span<T> span, SearchValues<T> separator)
        where T : IEquatable<T> =>
        ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitOn<T>(this ReadOnlySpan<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, In(separator), true);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> SplitOn<T>(this Span<T> span, in T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).SplitOn(separator);
#endif

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList(this SplitSpan<char> split)
    {
        List<string> ret = [];

        foreach (var next in split)
            ret.Add(next.ToString());

        return ret;
    }

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<T[]> ToList<T>(this SplitSpan<T> split)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>?
#endif
    {
        List<T[]> ret = [];

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Next<T>(
        ref ReadOnlySpan<T> reader1,
        ref ReadOnlySpan<T> reader2,
        ref SplitSpan<T>.Enumerator e1,
        ref SplitSpan<T>.Enumerator e2,
        out bool ret
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        Unsafe.SkipInit(out ret);

        if (reader1.Length is var length1 && reader2.Length is var length2 && length1 == length2)
            return SameLength(ref reader1, ref reader2, ref e1, ref e2, ref ret);

        if (length1 < length2)
        {
            if (!reader1.SequenceEqual(reader2[..length1]) || !e1.MoveNext())
            {
                ret = false;
                return true;
            }

            reader1 = e1.Current;
            reader2 = reader2[length1..];
            return false;
        }

        if (!reader1[..length2].SequenceEqual(reader2) || !e2.MoveNext())
        {
            ret = false;
            return true;
        }

        reader1 = reader1[length2..];
        reader2 = e2.Current;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool SameLength<T>(
        ref ReadOnlySpan<T> reader1,
        ref ReadOnlySpan<T> reader2,
        ref SplitSpan<T>.Enumerator e1,
        ref SplitSpan<T>.Enumerator e2,
        ref bool ret
    )
        where T : IEquatable<T>?
    {
        if (!reader1.SequenceEqual(reader2))
        {
            ret = false;
            return true;
        }

        if (!e1.MoveNext())
        {
            ret = !e2.MoveNext();
            return true;
        }

        if (!e2.MoveNext())
        {
            ret = false;
            return true;
        }

        reader1 = e1.Current;
        reader2 = e2.Current;
        return false;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitSpanAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAny(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitSpanAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAll(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitSpanLines(this string span) => span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.BreakingSearch);
#else
        new(span, Whitespaces.Breaking.AsSpan(), true);
#endif

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitOn(this string span, in char separator) => span.AsSpan().SplitOn(separator);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitSpanWhitespace(this string span) => span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> span) =>
#if NET8_0_OR_GREATER
        new(span, Whitespaces.UnicodeSearch);
#else
        new(span, Whitespaces.Unicode.AsSpan(), true);
#endif

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitWhitespace();
#endif
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitSpanOn(this string span, SearchValues<char> separator) =>
        span.AsSpan().SplitOn(separator);
#endif
}

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the span.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct SplitSpan<T>
#if UNMANAGED_SPAN
    where T : unmanaged, IEquatable<T>?
#else
    where T : IEquatable<T>?
#endif
{
    /// <summary>Represents the accumulator function for the enumeration of this type.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="accumulator">The accumulator.</param>
    /// <param name="next">The next slice from the enumeration.</param>
    /// <returns>The final accumulator value.</returns>
    public delegate TAccumulator Accumulator<TAccumulator>(TAccumulator accumulator, scoped ReadOnlySpan<T> next);

    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Of<TStrategy>(SplitMemory<T> split)
        where TStrategy : IStrategy
    {
        /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public
#if !NO_REF_STRUCTS
            ref
#endif
            partial struct Enumerator(SplitSpan<T> split)
        {
            [ValueRange(-1, int.MaxValue)]
#pragma warning disable IDE0044
            int _end = -1;
#pragma warning restore IDE0044

            /// <summary>Gets the current index.</summary>
            public readonly int Index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(-1, int.MaxValue)] get => _end;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current"/>
            public ReadOnlySpan<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
                private set;
            }

            /// <summary>Gets the enumerable used to create this instance.</summary>
            public SplitSpan<T> Enumerable { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } = split;

#if NET8_0_OR_GREATER
            /// <summary>Attempts to step through to the next slice.</summary>
            /// <param name="isAny">Determines whether to call <see cref="StepAny"/> or <see cref="StepAll"/>.</param>
            /// <param name="search">The search value.</param>
            /// <param name="body">The reference to its body.</param>
            /// <param name="separator">The reference to its separator.</param>
            /// <param name="end">The ending index of the slice.</param>
            /// <param name="start">The starting index of the slice.</param>
            /// <returns>Whether or not to continue looping.</returns>
#else
            /// <summary>Attempts to step through to the next slice.</summary>
            /// <param name="isAny">Determines whether to call <see cref="StepAny"/> or <see cref="StepAll"/>.</param>
            /// <param name="body">The reference to its body.</param>
            /// <param name="separator">The reference to its separator.</param>
            /// <param name="end">The ending index of the slice.</param>
            /// <param name="start">The starting index of the slice.</param>
            /// <returns>Whether or not to continue looping.</returns>
#endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool Step(
                bool isAny,
#if NET8_0_OR_GREATER
                SearchValues<T>? search,
#endif
                in ReadOnlySpan<T> body,
                in ReadOnlySpan<T> separator,
                scoped ref int end,
                out int start
            ) =>
                isAny
                    ? StepAny(
#if NET8_0_OR_GREATER
#pragma warning disable SA1114
                        search,
#pragma warning restore SA1114
#endif
                        body,
                        separator,
                        ref end,
                        out start
                    )
                    : StepAll(body, separator, ref end, out start);

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _end = -1;

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>
            /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
            /// <see langword="false"/> if the enumerator has passed the end of the collection.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var body = Enumerable.Body;
                var separator = Enumerable.Separator;

                if (separator.IsEmpty)
                    return !body.IsEmpty && Current.IsEmpty && (Current = body) is var _;

                int start;

                var output = typeof(TStrategy) switch
                {
                    var x when x == typeof(All) => StepAll(body, separator, ref _end, out start),
                    var x when x == typeof(Any) => StepAny(body, separator, ref _end, out start),
                    var x when x == typeof(Any.WithSearchValues) => StepAnySearch(body, separator, ref _end, out start),
                    var x when x == typeof(All.One) => StepAnySearch(body, separator, ref _end, out start),
                    _ => throw new NotSupportedException($"No case for {typeof(TStrategy).Name}."),
                };

                if (output)
                    Current = body[start.._end];

                return output;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool StepAll(
                in ReadOnlySpan<T> body,
                in ReadOnlySpan<T> separator,
                scoped ref int end,
                out int start
            )
            {
                Unsafe.SkipInit(out start);

                if (body.Length is var bodyLength && separator.Length is var length && bodyLength == length)
                {
                    if (body.SequenceEqual(separator))
                        return false;

                    start = 0;
                    end = bodyLength;
                    return true;
                }

                start = end is -1 ? ++end : end += length;

                while (end <= bodyLength)
                    switch (body[end..].IndexOf(separator))
                    {
                        case -1:
                            end = bodyLength;
                            return true;
                        case 0:
                            end = start += length;
                            continue;
                        case var i:
                            end += i;
                            return true;
                    }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool StepAny(
                in ReadOnlySpan<T> body,
                in ReadOnlySpan<T> separator,
                ref int end,
                out int start
            )
            {
                Unsafe.SkipInit(out start);

                if (body.Length is var bodyLength && ++end >= bodyLength)
                    return false;
#if NET7_0_OR_GREATER
                return body[end..].IndexOfAnyExcept(separator) is not -1 and var startSeparator &&
                    (start = end += startSeparator) is var _ &&
                    body[end..].IndexOfAny(separator) is var endSeparator &&
                    (end = endSeparator is -1 ? body.Length : end + endSeparator) is var _;
#else
                start = end;
                goto Begin;

            Increment:
                start++;
                end++;

            Begin:
                var min = int.MaxValue;

                foreach (var next in separator)
                    switch (body[end..].IndexOf(next))
                    {
                        case -1: continue;
                        case 0: goto Increment;
                        case var i when i < min:
                            min = i;
                            continue;
                    }

                end = min is int.MaxValue ? bodyLength : end + min;
                return true;
#endif
            }

 #if NET8_0_OR_GREATER
            static bool StepAnySearch(
                ReadOnlySpan<T> body,
                ReadOnlySpan<T> separator,
                ref int end,
                out int start
            )
            {
                Unsafe.SkipInit(out start);
                ref readonly var search = ref Unsafe.As<T, SearchValues<T>>(ref MemoryMarshal.GetReference(separator));

                return body[end..].IndexOfAnyExcept(search) is not -1 and var startSearch &&
                    (start = end += startSearch) is var _ &&
                    body[end..].IndexOfAny(search) is var endSearch &&
                    (end = endSearch is -1 ? body.Length : end + endSearch) is var _;
            }
#endif
        }
    }

    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct Any : IStrategy
    {
#if NET8_0_OR_GREATER
        public
#if !NO_READONLY_STRUCTS
            readonly
#endif
            struct WithSearchValues : IStrategy;
#endif
    }

    public
#if !NO_READONLY_STRUCTS
        readonly
#endif
        struct All : IStrategy
    {
        public
#if !NO_READONLY_STRUCTS
            readonly
#endif
            struct One : IStrategy;
    }

    public interface IStrategy;

#if NET8_0_OR_GREATER
    readonly SearchValues<T>? _search;
#else
    readonly bool _isAny;
#endif

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body) => Body = body;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="body">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    /// <param name="isAny">When <see langword="true"/>, treat separator as a big pattern match.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body, ReadOnlySpan<T> separator, bool isAny)
    {
        Body = body;
        Separator = separator;
#if NET8_0_OR_GREATER
        if (isAny)
            UnsafelySetNullishTo(out _search, 1);
#else
        _isAny = isAny;
#endif
    }
#if NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitSpan{T}"/> struct
    /// where <see cref="IsAny"/> is <see langword="true"/>.
    /// </summary>
    /// <remarks><para>This constructor is only available starting from .NET 8.0 or later.</para></remarks>
    /// <param name="body">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> body, SearchValues<T> separator)
    {
        Body = body;
        _search = separator;
    }
#endif

    /// <summary>Gets the specified index.</summary>
    /// <param name="index">The index to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="index"/> is negative.</exception>
    public ReadOnlySpan<T> this[[NonNegativeValue] int index]
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

    /// <summary>Gets the empty split span.</summary>
    public static SplitSpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <summary>
    /// Gets a value indicating whether it should split based on any character in <see cref="Separator"/>,
    /// or if all of them match.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(_search))]
#endif
    public bool IsAny
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get =>
#if NET8_0_OR_GREATER
            _search is not null
#else
            _isAny
#endif
          ||
            Separator.Length is 1;
    }

    /// <summary>Gets the line.</summary>
    public ReadOnlySpan<T> Body
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] init;
    }

    /// <summary>Gets the separator.</summary>
    public ReadOnlySpan<T> Separator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] init;
    }

    /// <summary>Determines whether both splits are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(scoped SplitSpan<T> left, scoped SplitSpan<T> right) => left.Equals(right);

    /// <summary>Determines whether both splits are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both splits are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(scoped SplitSpan<T> left, scoped SplitSpan<T> right) => !left.Equals(right);

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to call the constructor.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Enumerator(SplitSpan<T> value) => new(value);

    /// <summary>Implicitly calls <see cref="Enumerator.Enumerable"/>.</summary>
    /// <param name="value">The value to call <see cref="Enumerator.Enumerable"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SplitSpan<T>(Enumerator value) => value.Enumerable;

    /// <summary>Separates the head from the tail of this <see cref="SplitSpan{T}"/>.</summary>
    /// <param name="head">The first element of this enumeration.</param>
    /// <param name="tail">The rest of this enumeration.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlySpan<T> head, out SplitSpan<T> tail)
    {
        if (GetEnumerator() is var e && !e.MoveNext())
        {
            head = default;
            tail = default;
            return;
        }

        head = e.Current;
        tail = this with { Body = Body[e.Index..] };
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override bool Equals(object? obj) => false;

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(scoped SplitSpan<T> other) =>
#if NET8_0_OR_GREATER
        _search == other._search &&
#endif // ReSharper disable once ArrangeRedundantParentheses
#pragma warning disable SA1119, RCS1032
        (Body.IsEmpty && other.Body.IsEmpty ||
        Separator.IsEmpty && other.Separator.IsEmpty && Body.SequenceEqual(other.Body) ||
        IsAny == other.IsAny && Separator.SequenceEqual(other.Separator) && Body.SequenceEqual(other.Body));
#pragma warning restore SA1119, RCS1032

    /// <summary>Computes the length.</summary>
    /// <returns>The length.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int Count()
    {
        var e = GetEnumerator();
        var count = 0;

        while (e.MoveNext())
            count++;

        return count;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() => unchecked(IsAny.GetHashCode() * 31);

    /// <inheritdoc />
    public override string ToString() =>
        typeof(T) == typeof(char)
            ? Aggregate(new(), StringBuilderAccumulator()).ToString()
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            : this.ToList().Stringify(3, true);
#else
            : throw new NotSupportedException();
#endif

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    // ReSharper restore NullableWarningSuppressionIsUsed
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>Gets the first element.</summary>
    /// <returns>The first span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<T> First() => GetEnumerator() is var e && e.MoveNext() ? e.Current : default;

    /// <summary>Gets the last element.</summary>
    /// <returns>The last span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<T> Last()
#if NET8_0_OR_GREATER
        =>
            _search.ToAddress() switch
            {
                0 => Body.LastIndexOf(Separator),
                1 => Body.LastIndexOfAny(Separator), // ReSharper disable once NullableWarningSuppressionIsUsed
                _ => Body.LastIndexOfAny(_search!),
            } is not -1 and var index
                ? Body[index..]
                : default;
#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        =>
            (IsAny ? Body.LastIndexOfAny(Separator) : Body.LastIndexOf(Separator))
            is not -1 and var i
                ? Body[i..]
                : default;
#else
    {
        var e = GetEnumerator();

        while (e.MoveNext()) { }

        return e.Current;
    }
#endif

    /// <summary>Gets the single element.</summary>
    /// <returns>The single span from this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<T> Single() =>
        GetEnumerator() is var e && e.MoveNext() && e.Current is var ret && !e.MoveNext() ? ret : default;

    /// <summary>Gets the accumulated result of a set of callbacks where each element is passed in.</summary>
    /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
    /// <param name="seed">The accumulator.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The accumulated result of <paramref name="seed"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public TAccumulator Aggregate<TAccumulator>(
        TAccumulator seed,
        [InstantHandle, RequireStaticDelegate] Accumulator<TAccumulator> func
    )
    {
        var accumulator = seed;

        foreach (var next in this)
            accumulator = func(accumulator, next);

        return accumulator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once RedundantUnsafeContext
    static unsafe Accumulator<StringBuilder> StringBuilderAccumulator() =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        static (builder, span) => builder.Append(
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length)
        );
#else
        static (builder, span) =>
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
            fixed (T* ptr = span)
#endif
#pragma warning restore 8500
                return builder.Append((char*)ptr, span.Length);
#endif
        };
#endif
}
