// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if NET461_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
    /// <inheritdoc cref="SplitFrom{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitFrom(this string s, string sep, IEqualityComparer<char>? comparer = null) =>
        s.AsSpan().SplitFrom(sep.AsSpan(), comparer);

    /// <inheritdoc cref="SplitFactory.SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitLines(this string s) => s.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> s) => new(s, Whitespaces.Breaking.AsSpan());

    /// <inheritdoc cref="SplitTerminated{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitTerminated(
        this string s,
        string sep,
        IEqualityComparer<char>? comparer = null
    ) =>
        s.AsSpan().SplitTerminated(sep.AsSpan(), comparer);

    /// <inheritdoc cref="SplitFactory.SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitWhitespace(this string s) => s.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> s) => new(s, Whitespaces.Unicode.AsSpan());

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="s">The span to split.</param>
    /// <param name="sep">The separator.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<T> SplitFrom<T>(
        this ReadOnlySpan<T> s,
        ReadOnlySpan<T> sep,
        IEqualityComparer<T>? comparer = null
    ) =>
        new(s, sep, comparer, false);

    /// <summary>Splits a span by the specified separator.</summary>
    /// <typeparam name="T">The type of element from the span.</typeparam>
    /// <param name="s">The span to split.</param>
    /// <param name="sep">The separator.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<T> SplitTerminated<T>(
        this ReadOnlySpan<T> s,
        ReadOnlySpan<T> sep,
        IEqualityComparer<T>? comparer = null
    ) =>
        new(s, sep, comparer);
}
#endif

/// <summary>Represents a split entry.</summary>
/// <typeparam name="T">The type of element from the span.</typeparam>
[StructLayout(LayoutKind.Auto)]
readonly ref partial struct SplitSpan<T>
{
    /// <summary>Initializes a new instance of the <see cref="SplitSpan{T}"/> struct.</summary>
    /// <param name="span">The line to split.</param>
    /// <param name="separator">The characters for separation.</param>
    /// <param name="comparer">The comparison to determine when to split.</param>
    /// <param name="isTerminated">When <see langword="true"/>, treat separator as a big pattern match.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(
        ReadOnlySpan<T> span,
        ReadOnlySpan<T> separator,
        IEqualityComparer<T>? comparer = null,
        bool isTerminated = true
    )
    {
        IsTerminated = isTerminated;
        Comparer = comparer ?? EqualityComparer<T>.Default;
        Span = span;
        Separator = separator;
    }

    /// <summary>
    /// Gets a value indicating whether it should split based on any character in <see cref="Separator"/>,
    /// or if all of them match.
    /// </summary>
    public bool IsTerminated { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>Gets the comparer that determines when to split.</summary>
    public IEqualityComparer<T> Comparer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>Gets the line.</summary>
    public ReadOnlySpan<T> Span { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>Gets the separator.</summary>
    public ReadOnlySpan<T> Separator { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>Represents the enumeration object that views <see cref="SplitSpan{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public ref partial struct Enumerator
    {
        readonly SplitSpan<T> _span;

        int _end;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">Tne entry to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitSpan<T> span) => _span = span;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public ReadOnlySpan<T> Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        /// <inheritdoc cref="IEnumerator{T}.Reset"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _end = 0;

        /// <inheritdoc cref="IEnumerator{T}.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var start = ++_end;
            var result = _span.IsTerminated ? Terminated() : Full();

            if (result)
                Current = _span.Span[start.._end];

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Full()
        {
            for (; _end < _span.Span.Length; _end++)
            {
                var limit = Math.Min(_span.Separator.Length, _span.Span.Length - _end);

                for (var sep = 0; sep < limit; sep++)
                {
                    if (!_span.Comparer.Equals(_span.Span[_end + sep], _span.Separator[sep]))
                        break;

                    if (sep - limit is -1)
                        return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Terminated()
        {
            for (; _end < _span.Span.Length; _end++)
                foreach (var t in _span.Separator)
                    if (_span.Comparer.Equals(_span.Span[_end], t))
                        return true;

            return false;
        }
    }
}
