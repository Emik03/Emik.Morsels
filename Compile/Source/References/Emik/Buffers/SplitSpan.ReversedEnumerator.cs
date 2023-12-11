// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace ConvertToAutoPropertyWhenPossible InvertIf InvocationIsSkipped RedundantNameQualifier RedundantReadonlyModifier RedundantUsingDirective StructCanBeMadeReadOnly UseSymbolAlias

namespace Emik.Morsels;
#pragma warning disable 8618, 9193, CA1823, IDE0250, MA0071, MA0102, RCS1158, SA1137
using static Span;
using static SplitSpanFactory;

/// <inheritdoc cref="SplitSpan{TBody, TSeparator, TStrategy}"/>
readonly ref partial struct SplitSpan<TBody, TSeparator, TStrategy>
{
    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ReversedEnumerator GetReversedEnumerator() => new(this);

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct ReversedEnumerator(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
    {
        readonly ReadOnlySpan<TSeparator> _separator = separator;

        ReadOnlySpan<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(ReadOnlySpan<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="ReversedEnumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReversedEnumerator(SplitSpan<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlySpan<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
        }

        /// <inheritdoc cref="IEnumerator.Current"/>
        public readonly ReadOnlySpan<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <summary>Performs one step of an enumeration over the provided spans.</summary>
        /// <param name="sep">The separator span.</param>
        /// <param name="body">The span that contains the current state of the enumeration.</param>
        /// <param name="current">The current span.</param>
        /// <returns>
        /// <see langword="true"/> if a step was able to be performed successfully;
        /// <see langword="false"/> if the end of the collection is reached.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext(
            scoped in ReadOnlySpan<TSeparator> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        ) =>
            0 switch
            {
                _ when body.IsEmpty && (current = default) is var _ => false,
                _ when sep.IsEmpty => (current = body) is var _ && (body = default) is var _,
                _ when typeof(TStrategy) == typeof(MatchAll) =>
                    MoveNextAll(To<TBody>.From(sep), ref body, out current),
#if NET8_0_OR_GREATER
                _ when typeof(TStrategy) == typeof(MatchAny) && typeof(TSeparator) == typeof(SearchValues<TBody>) =>
                    MoveNextAny(To<SearchValues<TBody>>.From(sep), ref body, out current),
#endif
                _ when typeof(TStrategy) == typeof(MatchAny) =>
                    MoveNextAny(To<TBody>.From(sep), ref body, out current),
                _ when typeof(TStrategy) == typeof(MatchOne) =>
                    MoveNextOne(To<TBody>.From(sep), ref body, out current),
                _ => throw Error,
            };

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => MoveNext(_separator, ref _body, out _current);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAll(
            scoped in ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAll), "TStrategy is MatchAll");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");

            if (sep.Length < body.Length)
            {
                current = default;
                return false;
            }

        Retry:
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            switch (body.LastIndexOf(sep))
#else
            int lower = 0, upper = body.Length - sep.Length;

            for (; lower < upper; lower++)
                if (UnsafelyTake(UnsafelyAdvance(body, lower), sep.Length).SequenceEqual(sep))
                    break;

            if (lower == upper)
                lower = -1;

            // The worst way to suppress warnings about inlining variables.
            switch (+lower)
#endif
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case var i when i == body.Length - sep.Length:
                    if (body.Length != sep.Length)
                    {
                        body = UnsafelyTake(body, i);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = UnsafelyAdvance(body, i + sep.Length);
                    body = UnsafelyTake(body, i);
                    return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051
        static bool MoveNextAny(
            scoped in ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
#pragma warning restore MA0051
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
#if NET7_0_OR_GREATER
            switch (body.LastIndexOfAnyExcept(sep))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var i:
                    body = UnsafelyTake(body, i + 1);
                    break;
            }

            if (body.LastIndexOfAny(sep) is not -1 and var length)
            {
                current = UnsafelyAdvance(body, length + 1);
                body = UnsafelyTake(body, length);
                return true;
            }

            current = body;
            body = default;
#else
        Retry:

            var max = -1;

            foreach (var next in sep)
                switch (body.LastIndexOf(next))
                {
                    case -1: continue;
                    case var i when i == body.Length - 1:
                        if (body.Length is not 1)
                        {
                            body = UnsafelyTake(body, body.Length - 1);
                            goto Retry;
                        }

                        current = default;
                        return false;
                    case var i when i > max:
                        max = i;
                        continue;
                }

            if (max is not -1)
            {
                current = UnsafelyAdvance(body, max + 1);
                body = UnsafelyTake(body, max);
                return true;
            }

            current = body;
            body = default;
#endif
            return true;
        }

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAny(
            scoped in ReadOnlySpan<SearchValues<TBody>> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAny), "TStrategy is MatchAny");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
            ref var single = ref MemoryMarshal.GetReference(sep);

            switch (body.LastIndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var i:
                    body = UnsafelyTake(body, i + 1);
                    break;
            }

            if (body.LastIndexOfAny(single) is not -1 and var length)
            {
                current = UnsafelyAdvance(body, length + 1);
                body = UnsafelyTake(body, length);
                return true;
            }

            current = body;
            body = default;
            return true;
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextOne(
            scoped in ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchOne), "TStrategy is MatchOne");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            ref var single = ref MemoryMarshal.GetReference(sep);
#else
            var single = sep[0];
#endif
#if !NET7_0_OR_GREATER
        Retry:
#endif
#if NET7_0_OR_GREATER
            switch (body.LastIndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case var i when i == body.Length - 1: break;
                case var offset:
                    body = UnsafelyTake(body, offset + 1);
                    break;
            }

            if (body.IndexOf(single) is not -1 and var length)
            {
                current = UnsafelyAdvance(body, length + 1);
                body = UnsafelyTake(body, length);
                return true;
            }

            current = body;
            body = default;
            return true;
#else
            switch (body.LastIndexOf(single))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case var i when i == body.Length - 1:
                    if (body.Length is not 1)
                    {
                        body = UnsafelyTake(body, 1);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = UnsafelyAdvance(body, i);
                    body = UnsafelyTake(body, i - 1);
                    return true;
            }
#endif
        }
    }
}
