// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable IDE0250, MA0102, SA1137
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
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
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
    {
        var e1 = left.GetEnumerator();
        var e2 = right.GetEnumerator();

        while (e1.MoveNext())
            if (!(e2.MoveNext() && e1.Current.SequenceEqual(e2.Current)))
                return false;

        return !e2.MoveNext();
    }

    /// <inheritdoc cref="Splits{T}(ReadOnlySpan{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> Splits(this string span, char separator) => span.AsSpan().Splits(separator);

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAny(this string span, string separator) =>
        span.AsSpan().SplitAny(separator.AsSpan());

    /// <inheritdoc cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAny(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAny(separator);

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAll(this string span, string separator) =>
        span.AsSpan().SplitAll(separator.AsSpan());

    /// <inheritdoc cref="SplitAll{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitAll(this string span, ReadOnlySpan<char> separator) =>
        span.AsSpan().SplitAll(separator);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this string span) => span.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> span) =>
        new(span, Whitespaces.Breaking.AsSpan(), true);

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitLines(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitLines();

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this string span) => span.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="span">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> span) =>
        new(span, Whitespaces.Unicode.AsSpan(), true);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<char> SplitWhitespace(this Span<char> span) => ((ReadOnlySpan<char>)span).SplitWhitespace();

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> Splits<T>(this ReadOnlySpan<T> span, T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            new(span, separator);

    /// <inheritdoc cref="Splits{T}(ReadOnlySpan{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SplitSpan<T> Splits<T>(this Span<T> span, T separator)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>
#else
        where T : IEquatable<T>
#endif
        =>
            ((ReadOnlySpan<T>)span).Splits(separator);

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

    /// <summary>Copies the values to a new <see cref="List{T}"/>.</summary>
    /// <param name="split">The instance to get the list from.</param>
    /// <returns>The list containing the copied values of this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static List<string> ToList(this SplitSpan<char> split)
    {
        List<string> ret = new();

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
        where T : IEquatable<T>
#endif
    {
        List<T[]> ret = new();

        foreach (var next in split)
            ret.Add(next.ToArray());

        return ret;
    }
}
#endif

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the span.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
ref
#endif
    partial struct SplitSpan<T>
#if UNMANAGED_SPAN
    where T : unmanaged, IEquatable<T>
#else
    where T : IEquatable<T>
#endif
{
    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="span">The line to split.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> span) => Span = span;

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="span">The line to split.</param>
    /// <param name="head">The head used as a separator.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> span, T head)
    {
        Span = span;
        Head = head;
    }

    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="span">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    /// <param name="isAny">When <see langword="true"/>, treat separator as a big pattern match.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(ReadOnlySpan<T> span, ReadOnlySpan<T> separator, bool isAny)
    {
        IsAny = isAny;
        Separator = separator;
        Span = span;
    }

    /// <summary>Gets the empty split span.</summary>
    public SplitSpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    /// <summary>
    /// Gets a value indicating whether it should split based on any character in <see cref="Separator"/>,
    /// or if all of them match.
    /// </summary>
    public bool IsAny { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>Gets the line.</summary>
    public ReadOnlySpan<T> Span { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the separator.</summary>
    public ReadOnlySpan<T> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the head.</summary>
    public T? Head { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator
    {
        readonly SplitSpan<T> _split;

        [ValueRange(-1, int.MaxValue)]
        int _end = -1;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">Tne entry to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitSpan<T> split) => _split = split;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public ReadOnlySpan<T> Current { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; private set; }

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
            while (true)
            {
                if (Step(out var start, out var end) is ControlFlow.Break)
                    return false;

                if ((Current = _split.Span[start..end]).IsEmpty)
                    continue;

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ControlFlow Break(out int end)
        {
            end = 0;
            return ControlFlow.Break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ControlFlow Step(out int start, out int end)
        {
            if (_split.Separator.IsEmpty)
            {
                start = ++_end;

                if (StepSingle() is ControlFlow.Break)
                    return Break(out end);

                end = _end;
                return ControlFlow.Continue;
            }

            if (_split.IsAny)
            {
                start = ++_end;

                if (StepAny() is ControlFlow.Break)
                    return Break(out end);

                end = _end;
                return ControlFlow.Continue;
            }

            start = Math.Max(_end++, 0);

            if (StepAll() is ControlFlow.Break)
                return Break(out end);

            var span = _split.Span.Length;
            var separator = _split.Separator.Length;

            end = _end != span || _split.Span[(_end - separator).._end].SequenceEqual(_split.Separator)
                ? _end - separator
                : _end;

            return ControlFlow.Continue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ControlFlow StepAny()
        {
            var span = _split.Span.Length;

            for (; _end < span; _end++)
                foreach (var t in _split.Separator)
                    if (_split.Span[_end].Equals(t))
                        return ControlFlow.Continue;

            return _end > span ? ControlFlow.Break : ControlFlow.Continue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ControlFlow StepAll()
        {
            var span = _split.Span.Length;
            var separator = _split.Separator.Length;

            if (span < separator || span == separator && _split.Span.SequenceEqual(_split.Separator))
                return ControlFlow.Break;

            if (_end is 0)
                _end = separator;

            for (; _end < span; _end++)
                if (_split.Span[(_end - separator).._end].SequenceEqual(_split.Separator))
                    return ControlFlow.Continue;

            return _end > span ? ControlFlow.Break : ControlFlow.Continue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ControlFlow StepSingle()
        {
            var span = _split.Span.Length;

#pragma warning disable 8604
            for (; _end < span; _end++)
                if (_split.Span[_end].Equals(_split.Head))
                    return ControlFlow.Continue;
#pragma warning restore 8604

            return _end > span ? ControlFlow.Break : ControlFlow.Continue;
        }
    }
}
