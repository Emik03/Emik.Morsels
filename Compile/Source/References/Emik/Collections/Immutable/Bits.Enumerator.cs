// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly RedundantReadonlyModifier
#pragma warning disable 8500, IDE0251, MA0102
namespace Emik.Morsels;

using static CollectionAccessType;
using static Span;

/// <summary>Provides the enumeration of individual bits from the given <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
/// <param name="bits">The item to use.</param>
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>
{
    /// <summary>An enumerator over <see cref="Bits{T}"/>.</summary>
    /// <param name="value">The item to use.</param>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(T value) : IEnumerator<T>
    {
        const int Start = -1;

        /// <summary>Gets the current mask.</summary>
        [CollectionAccess(None)]
        public nuint Mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <summary>Gets the current index.</summary>
        [CLSCompliant(false), CollectionAccess(None)]
        public nint Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        } = Start;

        /// <summary>Gets the reconstruction of the original enumerable that can create this instance.</summary>
        [CollectionAccess(Read)]
        public readonly Bits<T> AsBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => value;
        }

        /// <summary>Gets the underlying value that is being enumerated.</summary>
        [CollectionAccess(Read)]
        public readonly T AsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => value;
        }

        /// <inheritdoc />
        [CollectionAccess(None)]
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get
            {
                T t = default;
                Unsafe.Add(ref Unsafe.As<T, nuint>(ref t), Index) = Mask;
                return t;
            }
        }

        /// <inheritdoc />
        [CollectionAccess(None)]
        readonly object IEnumerator.Current
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Current;
        }

        /// <summary>Implicitly calls the constructor.</summary>
        /// <param name="value">The value to pass into the constructor.</param>
        /// <returns>A new instance of <see cref="Enumerator"/> with <paramref name="value"/> passed in.</returns>
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator Enumerator(T value) => new(value);

        /// <summary>Implicitly calls <see cref="Current"/>.</summary>
        /// <param name="value">The value to call <see cref="Current"/>.</param>
        /// <returns>The value that was passed in to this instance.</returns>
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static explicit operator T(Enumerator value) => value.Current;

        /// <inheritdoc />
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            Index = Start;
            Mask = 0;
        }

        /// <inheritdoc />
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            Mask <<= 1;

            if (Mask is 0)
            {
                Index++;
                Mask++;
            }

            if (Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>() is not 0 && FindNativelySized() ||
                Unsafe.SizeOf<T>() % Unsafe.SizeOf<nint>() is not 0 && FindRest())
                return true;

            Index = Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>();
            Mask = FalsyMask();
            return false;
        }

        /// <inheritdoc />
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public readonly override string ToString()
        {
            var that = this;
            return that.ToRemainingString();
        }

        /// <summary>Enumerates over the remaining elements to give a <see cref="string"/> result.</summary>
        /// <returns>The <see cref="string"/> result of this instance.</returns>
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
        public string ToRemainingString()
        {
            Span<char> span = stackalloc char[Unsafe.SizeOf<T>() * BitsInByte];
            ref var last = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Unsafe.SizeOf<T>() * BitsInByte - 1);
            span.Fill('0');

            while (MoveNext())
                Unsafe.Subtract(ref last, (int)(Index * (Unsafe.SizeOf<nint>() * BitsInByte) + TrailingZeroCount(Mask)))
                    ^= '\x01';

            return span.ToString();
        }

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static nuint FalsyMask() => (nuint)1 << Unsafe.SizeOf<nint>() * BitsInByte - 2;

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static nuint LastRest() => ((nuint)1 << Unsafe.SizeOf<T>() % Unsafe.SizeOf<nint>() * BitsInByte) - 1;

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        bool FindNativelySized()
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index < 0)
                return false;

            for (; Index < Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>(); Index++, Mask = 1)
                for (; Mask is not 0; Mask <<= 1)
                    if (IsNonZero())
                        return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        bool FindRest()
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index != Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>())
                return false;

            for (; (Mask & LastRest()) is not 0; Mask <<= 1)
                if (IsNonZero())
                    return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        readonly bool IsNonZero() => (Unsafe.Add(ref Unsafe.As<T, nuint>(ref AsRef(value)), Index) & Mask) is not 0;
    }
}
#endif
