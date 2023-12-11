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
    public readonly Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Represents the enumeration object that views <see cref="SplitSpan{T, TSeparator, TStrategy}"/>.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator(ReadOnlySpan<TBody> body, ReadOnlySpan<TSeparator> separator)
    {
        readonly ReadOnlySpan<TSeparator> _separator = separator;

        ReadOnlySpan<TBody> _body = body, _current;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="body">The body.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ReadOnlySpan<TBody> body)
            : this(body, default) { }

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="split">The enumerable to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SplitSpan<TBody, TSeparator, TStrategy> split)
            : this(split._body, split._separator) { }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Body"/>
        public readonly ReadOnlySpan<TBody> Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _body;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _body = value;
        }

        /// <inheritdoc cref="IEnumerator.Current"/>
        public readonly ReadOnlySpan<TBody> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _current;
        }

        /// <inheritdoc cref="SplitSpan{T, TSeparator, TStrategy}.Separator"/>
        public readonly ReadOnlySpan<TSeparator> Separator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _separator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] init => _separator = value;
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
        public static bool Move(
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
        public bool MoveNext() => Move(_separator, ref _body, out _current);

        /// <summary>
        /// Checks if two sequences of type <see name="TBody"/> are equal while iterating through the next element.
        /// </summary>
        /// <typeparam name="TOtherSeparator">The type of separator used in the other sequence.</typeparam>
        /// <typeparam name="TOtherStrategy">The strategy used for splitting the other sequence.</typeparam>
        /// <param name="other">The enumerator for the other sequence.</param>
        /// <param name="reader">The <see cref="ReadOnlySpan{T}"/> representing this sequence.</param>
        /// <param name="otherReader">The <see cref="ReadOnlySpan{T}"/> representing the other sequence.</param>
        /// <param name="ret">
        /// Output parameter indicating if the sequences are equal.
        /// Note that this value is undefined if <see langword="false"/> is returned.
        /// </param>
        /// <returns>
        /// The value <see langword="true"/> if enumeration should be stopped; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualityMoveNext<TOtherSeparator, TOtherStrategy>(
            ref SplitSpan<TBody, TOtherSeparator, TOtherStrategy>.Enumerator other,
            ref ReadOnlySpan<TBody> reader,
            ref ReadOnlySpan<TBody> otherReader,
            out bool ret
        )
#if !NET7_0_OR_GREATER
            where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        {
            Unsafe.SkipInit(out ret);

            if (reader.Length is var length && otherReader.Length is var otherLength && length == otherLength)
                return SameLength(ref other, ref reader, ref otherReader, ref ret);

            if (length < otherLength)
            {
                if (!reader.SequenceEqual(otherReader[..length]) || !MoveNext())
                {
                    ret = false;
                    return true;
                }

                reader = Current;
                otherReader = otherReader[length..];
                return false;
            }

            if (!UnsafelyTake(reader, otherLength).SequenceEqual(otherReader) || !other.MoveNext())
            {
                ret = false;
                return true;
            }

            reader = UnsafelyTake(reader, otherLength);
            otherReader = other.Current;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MoveNextAll(
            scoped in ReadOnlySpan<TBody> sep,
            scoped ref ReadOnlySpan<TBody> body,
            out ReadOnlySpan<TBody> current
        )
        {
            System.Diagnostics.Debug.Assert(typeof(TStrategy) == typeof(MatchAll), "TStrategy is MatchAll");
            System.Diagnostics.Debug.Assert(!sep.IsEmpty, "separator is non-empty");
        Retry:

            switch (body.IndexOf(sep))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    if (body.Length != sep.Length)
                    {
                        body = UnsafelyAdvance(body, sep.Length);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = UnsafelyTake(body, i);
                    body = UnsafelyAdvance(body, i + sep.Length);
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
            switch (body.IndexOfAnyExcept(sep))
            {
                case -1:
                    current = default;
                    return false;
                case 0: break;
                case var i:
                    body = UnsafelyAdvance(body, i);
                    break;
            }

            if (body.IndexOfAny(sep) is not -1 and var length)
            {
                current = UnsafelyTake(body, length);
                body = UnsafelyAdvance(body, length + 1);
                return true;
            }

            current = body;
            body = default;
#else
        Retry:

            var min = int.MaxValue;

            foreach (var next in sep)
                switch (body.IndexOf(next))
                {
                    case -1: continue;
                    case 0:
                        if (body.Length is not 1)
                        {
                            body = UnsafelyAdvance(body, 1);
                            goto Retry;
                        }

                        current = default;
                        return false;
                    case var i when i < min:
                        min = i;
                        continue;
                }

            if (min is not int.MaxValue)
            {
                current = UnsafelyTake(body, min);
#pragma warning disable S3949
                body = UnsafelyAdvance(body, min + 1);
#pragma warning restore S3949
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

            switch (body.IndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case 0: break;
                case var offset:
                    body = UnsafelyAdvance(body, offset);
                    break;
            }

            if (body.IndexOfAny(single) is not -1 and var length)
            {
                current = UnsafelyTake(body, length);
                body = UnsafelyAdvance(body, length + 1);
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
            switch (body.IndexOfAnyExcept(single))
            {
                case -1:
                    current = default;
                    return false;
                case 0: break;
                case var offset:
                    body = UnsafelyAdvance(body, offset);
                    break;
            }

            if (body.IndexOf(single) is not -1 and var length)
            {
                current = UnsafelyTake(body, length);
                body = UnsafelyAdvance(body, length + 1);
                return true;
            }

            current = body;
            body = default;
            return true;
#else
            switch (body.IndexOf(single))
            {
                case -1:
                    current = body;
                    body = default;
                    return true;
                case 0:
                    if (body.Length is not 1)
                    {
                        body = UnsafelyAdvance(body, 1);
                        goto Retry;
                    }

                    current = default;
                    return false;
                case var i:
                    current = UnsafelyTake(body, i);
                    body = UnsafelyAdvance(body, i + 1);
                    return true;
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool SameLength<TOtherSeparator, TOtherStrategy>(
            ref SplitSpan<TBody, TOtherSeparator, TOtherStrategy>.Enumerator other,
            ref ReadOnlySpan<TBody> reader,
            ref ReadOnlySpan<TBody> otherReader,
            ref bool ret
        )
#if !NET7_0_OR_GREATER
            where TOtherSeparator : IEquatable<TOtherSeparator>?
#endif
        {
            if (!reader.SequenceEqual(otherReader))
            {
                ret = false;
                return true;
            }

            if (!MoveNext())
            {
                ret = !other.MoveNext();
                return true;
            }

            if (!other.MoveNext())
            {
                ret = false;
                return true;
            }

            reader = Current;
            otherReader = other.Current;
            return false;
        }
    }
}
