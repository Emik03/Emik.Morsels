// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if NET461_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
/// <summary>Methods to split spans into multiple spans.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitLines(this string s) => s.AsSpan().SplitLines();

    /// <summary>Splits a span by line breaks.</summary>
    /// <remarks><para>Line breaks are considered any character in <see cref="Whitespaces.Breaking"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitLines(this ReadOnlySpan<char> s) => new(s, Whitespaces.Breaking.AsSpan());

    /// <inheritdoc cref="SplitLines(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitLines(this Span<char> s) => ((ReadOnlySpan<char>)s).SplitLines();

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitTerminated(
        this string s,
        string sep,
        IEqualityComparer<char>? comparer = null
    ) =>
        s.AsSpan().SplitTerminated(sep.AsSpan(), comparer);

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitWhitespace(this string s) => s.AsSpan().SplitWhitespace();

    /// <summary>Splits a span by whitespace.</summary>
    /// <remarks><para>Whitespace is considered any character in <see cref="Whitespaces.Unicode"/>.</para></remarks>
    /// <param name="s">The span to split.</param>
    /// <returns>The enumerable object that references the parameter <paramref name="s"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitWhitespace(this ReadOnlySpan<char> s) => new(s, Whitespaces.Unicode.AsSpan());

    /// <inheritdoc cref="SplitWhitespace(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<char> SplitWhitespace(this Span<char> s) => ((ReadOnlySpan<char>)s).SplitWhitespace();

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

    /// <inheritdoc cref="SplitTerminated{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<T> SplitTerminated<T>(
        this Span<T> s,
        ReadOnlySpan<T> sep,
        IEqualityComparer<T>? comparer = null
    ) =>
        ((ReadOnlySpan<T>)s).SplitTerminated(sep, comparer);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitSpan(
        ReadOnlySpan<T> span,
        ReadOnlySpan<T> separator,
        IEqualityComparer<T>? comparer = null
    )
    {
        Comparer = comparer ?? EqualityComparer<T>.Default;
        Span = span;
        Separator = separator;
    }

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
        readonly SplitSpan<T> _split;

        int _end = -1;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">Tne entry to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitSpan<T> split) => _split = split;

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public ReadOnlySpan<T> Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        /// <inheritdoc cref="IEnumerator.Reset"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _end = -1;

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            while (true)
            {
                var start = ++_end;
                Terminate();

                if (_end > _split.Span.Length)
                    return false;

                Current = _split.Span[start.._end];

                if (Current.IsEmpty)
                    continue;

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Terminate()
        {
            for (; _end < _split.Span.Length; _end++)
                foreach (var t in _split.Separator)
                    if (_split.Comparer.Equals(_split.Span[_end], t))
                        return;
        }
    }
}
