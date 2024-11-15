// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent RedundantUnsafeContext UseSymbolAlias
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable 8500
// ReSharper disable RedundantNameQualifier RedundantUsingDirective
using static System.Runtime.CompilerServices.RuntimeHelpers;

/// <summary>Defines methods for spans.</summary>
/// <remarks><para>See <see cref="MaxStackalloc"/> for details about stack- and heap-allocation.</para></remarks>
static partial class Span
{
    /// <summary>Provides reinterpret span methods.</summary>
    /// <typeparam name="TTo">The type to convert to.</typeparam>
    public static class To<TTo>
    {
        /// <summary>
        /// Encapsulates the functionality to determine if a conversion is supported between two types.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        public static class Is<TFrom>
#if !NO_ALLOWS_REF_STRUCT
            where TFrom : allows ref struct
#endif
        {
            /// <summary>
            /// Gets a value indicating whether the conversion between types
            /// <typeparamref name="TFrom"/> and <see name="TTo"/> in <see cref="To{TTo}"/> is defined.
            /// </summary>
            public static bool Supported { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
#if NETSTANDARD && !NETSTANDARD2_0_OR_GREATER
                typeof(TFrom) == typeof(TTo);
#else
                typeof(TFrom) == typeof(TTo) ||
                Unsafe.SizeOf<TFrom>() >= Unsafe.SizeOf<TTo>() &&
                (IsReinterpretable(typeof(TFrom), typeof(TTo)) ||
                    !IsReferenceOrContainsReferences<TFrom>() && !IsReferenceOrContainsReferences<TTo>());

            [Pure]
            static bool IsReinterpretable(Type first, Type second)
            {
                while (first.IsValueType && first.GetFields() is [{ FieldType: var next }])
                    first = next;

                while (second.IsValueType && second.GetFields() is [{ FieldType: var next }])
                    second = next;

                return first == second;
            }
#endif
        }

        /// <summary>
        /// Converts a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TFrom"/>
        /// to a <see cref="ReadOnlySpan{T}"/> of type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to convert from.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="Is{TFrom}.Supported"/> is <see langword="false"/>.
        /// </exception>
        /// <returns>
        /// The reinterpretation of the parameter <paramref name="source"/> from its original type
        /// <typeparamref name="TFrom"/> to the destination type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static unsafe ReadOnlySpan<TTo> From<TFrom>(ReadOnlySpan<TFrom> source)
        {
            System.Diagnostics.Debug.Assert(Is<TFrom>.Supported, "No out-of-bounds access.");
#if (NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP) && !CSHARPREPL
            // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
            // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
            // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
            InlineIL.IL.Emit.Ldarg_0();
            InlineIL.IL.Emit.Ret();
            throw InlineIL.IL.Unreachable();
#else
            return *(ReadOnlySpan<TTo>*)&source;
#endif
        }

        /// <summary>
        /// Converts a <see cref="Span{T}"/> of type <typeparamref name="TFrom"/>
        /// to a <see cref="Span{T}"/> of type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="source">The <see cref="Span{T}"/> to convert from.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="Is{TFrom}.Supported"/> is <see langword="false"/>.
        /// </exception>
        /// <returns>
        /// The reinterpretation of the parameter <paramref name="source"/> from its original
        /// type <typeparamref name="TFrom"/> to the destination type <see name="TTo"/> in <see cref="To{TTo}"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static unsafe Span<TTo> From<TFrom>(Span<TFrom> source)
        {
            System.Diagnostics.Debug.Assert(Is<TFrom>.Supported, "No out-of-bounds access.");
#if (NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP) && !CSHARPREPL
            // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
            // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
            // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
            InlineIL.IL.Emit.Ldarg_0();
            InlineIL.IL.Emit.Ret();
            throw InlineIL.IL.Unreachable();
#else
            return *(Span<TTo>*)&source;
#endif
        }
    }

    /// <summary>Provides interpret methods.</summary>
    /// <typeparam name="TTo">The type to convert to.</typeparam>
    public static class Ret<TTo>
#if !NO_ALLOWS_REF_STRUCT
        where TTo : allows ref struct
#endif
    {
        /// <summary>Performs a reinterpret cast from <typeparamref name="TFrom"/> to <see name="TTo"/>.</summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="source">The value to convert.</param>
        /// <returns>The result of the reinterpret cast.</returns>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#else
        [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#endif
        public static unsafe TTo From<TFrom>(TFrom source)
#if !NO_ALLOWS_REF_STRUCT
            where TFrom : allows ref struct
#endif
        {
            System.Diagnostics.Debug.Assert(Unsafe.SizeOf<TFrom>() >= Unsafe.SizeOf<TTo>(), "No out-of-bounds access.");
#if CSHARPREPL
            return Unsafe.As<TFrom, TTo>(ref source);
#elif NET452_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
            // We have to resort to inline IL because Unsafe.As<T> has a constraint for classes,
            // and Unsafe.As<TFrom, TTo> introduces a miniscule amount of overhead.
            // Doing it like this reduces the IL size from 9 to 2 bytes, and the JIT assembly from 9 to 3 bytes.
            InlineIL.IL.Emit.Ldarg_0();
            InlineIL.IL.Emit.Ret();
            throw InlineIL.IL.Unreachable();
#else
            return *(TTo*)&source;
#endif
        }
    }

    /// <summary>The maximum size for stack allocations in bytes.</summary>
    /// <remarks><para>
    /// Stack allocating arrays is an incredibly powerful tool that gets rid of a lot of the overhead that comes
    /// from instantiating arrays normally. Notably, that all classes (such as <see cref="List{T}"/>) are heap
    /// allocated, and moreover are garbage collected. This can cause strain in methods that are called often.
    /// </para><para>
    /// However, there isn't as much stack memory available as there is heap, which can cause a DoS (Denial of Service)
    /// vulnerability if you aren't careful. Use this constant to determine if you should use a heap allocation.
    /// </para></remarks>
    public const int MaxStackalloc = 1 << 11;

    /// <summary>Sets the reference to the address within the null range.</summary>
    /// <remarks><para>
    /// This is a highly unsafe function. The runtime reserves the first 2kiB for null-behaving values, which means a
    /// valid reference will never be within this range. This allows reference types to be a disjoint union of a valid
    /// reference, and an 11-bit number. Be careful with the values returned by this function: <see langword="null"/>
    /// comparisons can <see langword="return"/> <see langword="false"/>, but will behave as such.
    /// </para></remarks>
    /// <typeparam name="T">The type of the nullable reference type.</typeparam>
    /// <param name="reference">
    /// The resulting reference that contains the address of the parameter <paramref name="address"/>.
    /// </param>
    /// <param name="address">The number to set.</param>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static unsafe void UnsafelySetNullishTo<T>(out T? reference, byte address)
        where T : class
    {
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        fixed (T* ptr = &reference)
            *(nuint*)ptr = address;
#else
        Unsafe.SkipInit(out reference);
        Unsafe.As<T?, nuint>(ref reference) = address;
#endif
    }

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this string left, scoped ReadOnlySpan<char> right) =>
        left.AsSpan().Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this IMemoryOwner<char> left, scoped ReadOnlySpan<char> right) =>
        left.Memory.Span.ReadOnly().Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this Memory<char> left, scoped ReadOnlySpan<char> right) =>
        left.Span.ReadOnly().Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this scoped Span<char> left, scoped ReadOnlySpan<char> right) =>
        left.ReadOnly().Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this ReadOnlyMemory<char> left, scoped ReadOnlySpan<char> right) =>
        left.Span.Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this scoped ReadOnlySpan<char> left, scoped ReadOnlySpan<char> right) =>
        left.Equals(right, StringComparison.OrdinalIgnoreCase);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this Memory<T> span,
        ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);

    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this ReadOnlyMemory<T> span,
        ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);
#endif
    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this Memory<T> span, ReadOnlyMemory<T> other)
        where T : IEquatable<T>? =>
        span.Span.SequenceEqual(other.Span);

    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this ReadOnlyMemory<T> span, ReadOnlyMemory<T> other)
        where T : IEquatable<T>? =>
        span.Span.SequenceEqual(other.Span);

    /// <summary>Determines if a given length and type should be stack-allocated.</summary>
    /// <remarks><para>
    /// See <see cref="MaxStackalloc"/> for details about stack- and heap-allocation.
    /// </para></remarks>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#else
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#endif
    public static bool IsStack<T>([NonNegativeValue] int length)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
        =>
            InBytes<T>(length) <= MaxStackalloc;

    /// <summary>Reads the raw memory of the object.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="value">The value to read.</param>
    /// <returns>The raw memory of the parameter <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe byte[] Raw<T>(T value)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
        =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            [.. MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref AsRef(value)), Unsafe.SizeOf<T>())];
#else
            new Span<byte>(&value, Unsafe.SizeOf<T>()).ToArray();
#endif
    /// <summary>Gets the byte length needed to allocate the current length, used in <see cref="IsStack{T}"/>.</summary>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
#else
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
#endif
    public static int InBytes<T>([NonNegativeValue] int length)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
        =>
            length * Unsafe.SizeOf<T>();

    /// <summary>Returns the memory address of a given reference object.</summary>
    /// <remarks><para>The value is not pinned; do not read values from this location.</para></remarks>
    /// <param name="_">The reference <see cref="object"/> for which to get the address.</param>
    /// <returns>The memory address of the reference object.</returns>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#else
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#endif
    public static nuint ToAddress<T>(this T? _)
        where T : class =>
        Ret<nuint>.From(_);

    /// <summary>Creates a new <see cref="ReadOnlySpan{T}"/> of length 1 around the specified reference.</summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> In<T>(in T reference) =>
#if NET8_0_OR_GREATER || CSHARPREPL
        new(ref AsRef(reference));
#elif NET7_0_OR_GREATER
        new(AsRef(reference));
#elif !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        new([reference]);
#else
        MemoryMarshal.CreateReadOnlySpan(ref AsRef(reference), 1);
#endif
    /// <summary>Creates a new reinterpreted <see cref="ReadOnlySpan{T}"/> over the specified reference.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The destination type.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    public static ReadOnlySpan<TTo> In<TFrom, TTo>(in TFrom reference)
        where TFrom : struct
        where TTo : struct =>
        MemoryMarshal.Cast<TFrom, TTo>(In(reference));

    /// <summary>Converts the <see cref="Memory{T}"/> to the <see cref="ReadOnlyMemory{T}"/>.</summary>
    /// <typeparam name="T">The type of memory.</typeparam>
    /// <param name="memory">The memory to convert.</param>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> of the parameter <paramref name="memory"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> ReadOnly<T>(this Memory<T> memory) => memory;

    /// <summary>Converts the <see cref="Span{T}"/> to the <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> of the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> ReadOnly<T>(this Span<T> span) => span;

    /// <summary>Creates a new <see cref="Span{T}"/> of length 1 around the specified reference.</summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Ref<T>(ref T reference) =>
#if NET7_0_OR_GREATER
        new(ref reference);
#else
        MemoryMarshal.CreateSpan(ref reference, 1);
#endif
    /// <summary>Creates a new reinterpreted <see cref="Span{T}"/> over the specified reference.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The destination type.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    public static Span<TTo> Ref<TFrom, TTo>(ref TFrom reference)
        where TFrom : struct
        where TTo : struct =>
        MemoryMarshal.Cast<TFrom, TTo>(Ref(ref reference));
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY && !CSHARPREPL
    /// <summary>Creates the stack allocation of the type.</summary>
    /// <typeparam name="T">The type of the resulting <see cref="Span{T}"/>.</typeparam>
    /// <param name="length">The length of the stack-allocation. This value is unchecked.</param>
    /// <returns>The resulting <see cref="Span{T}"/> pointing to the created stack allocation.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe Span<T> Stackalloc<T>([NonNegativeValue] in int length)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        System.Diagnostics.Debug.Assert(length is >= 0 and <= MaxStackalloc, "Length is within bounds");
        System.Diagnostics.Debug.Assert(IsReferenceOrContainsReferences<T>(), "Contains references");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        Span<byte> span = stackalloc byte[InBytes<T>(length)];
        return MemoryMarshal.CreateSpan(ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(span)), length);
#else
        var ptr = stackalloc byte[InBytes<T>(length)];
        return new(ptr, length);
#endif
    }
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Reinterprets the given read-only reference as a mutable reference.</summary>
    /// <typeparam name="T">The underlying type of the reference.</typeparam>
    /// <param name="source">The read-only reference to reinterpret.</param>
    /// <returns>A mutable reference to a value of type <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe ref T AsRef<T>(in T source)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
        =>
            ref Unsafe.AsRef(source);
#endif
    /// <summary>Separates the head from the tail of a <see cref="Memory{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="memory"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="memory"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this Memory<T> memory, out T? head, out Memory<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (memory.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = memory.Span.UnsafelyIndex(0);
        tail = memory[1..];
    }

    /// <summary>Separates the head from the tail of a <see cref="Memory{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="memory"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="memory"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this ReadOnlyMemory<T> memory, out T? head, out ReadOnlyMemory<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (memory.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = memory.Span.UnsafelyIndex(0);
        tail = memory[1..];
    }

    /// <summary>Separates the head from the tail of a <see cref="Span{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this Span<T> span, out T? head, out Span<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span.UnsafelyIndex(0);
        tail = span.UnsafelySkip(1);
    }

    /// <summary>Separates the head from the tail of a <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this ReadOnlySpan<T> span, out T? head, out ReadOnlySpan<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span.UnsafelyIndex(0);
        tail = span.UnsafelySkip(1);
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>
    /// Gets the index of an element of a given <see cref="Memory{T}"/> from its <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type if items in the input <see cref="Memory{T}"/>.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> to calculate the index for.</param>
    /// <param name="span">The reference to the target item to get the index for.</param>
    /// <returns>The index of <paramref name="memory"/> within <paramref name="span"/>, or <c>-1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOf<T>(ReadOnlyMemory<T> memory, scoped ReadOnlySpan<T> span) =>
        memory.Span.IndexOf(ref MemoryMarshal.GetReference(span));
#endif
    /// <summary>Gets the index of an element of a given <see cref="Span{T}"/> from its reference.</summary>
    /// <typeparam name="T">The type if items in the input <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> to calculate the index for.</param>
    /// <param name="value">The reference to the target item to get the index for.</param>
    /// <returns>The index of <paramref name="value"/> within <paramref name="span"/>, or <c>-1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this scoped ReadOnlySpan<T> span, scoped ref T value)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        =>
            Unsafe.ByteOffset(ref MemoryMarshal.GetReference(span), ref value) is var byteOffset &&
            byteOffset / (nint)(uint)Unsafe.SizeOf<T>() is var elementOffset &&
            (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
#else
    {
        fixed (T* ptr = &value)
        fixed (T* s = span)
            return (nint)(span.Align(s) - ptr) is var elementOffset && (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this scoped Span<T> origin, scoped ref T target) =>
        origin.ReadOnly().IndexOf(ref target);
#if !NET7_0_OR_GREATER
    /// <inheritdoc cref="IndexOfAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOfAny<T>(this scoped Span<T> span, scoped ReadOnlySpan<T> values)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().IndexOfAny(values);

    /// <summary>
    /// Searches for the first index of the specified values similar
    /// to calling IndexOf several times with the logical OR operator.
    /// </summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    /// <returns>The first index of the occurrence of the values in the span. If not found, returns -1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe int IndexOfAny<T>(this scoped ReadOnlySpan<T> span, scoped ReadOnlySpan<T> values)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        fixed (T* searchSpace = span)
        fixed (T* value = values)
            return SpanHelpers.IndexOfAny(span.Align(searchSpace), span.Length, values.Align(value), values.Length);
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int OffsetOf<T>(this scoped ReadOnlySpan<T> origin, scoped ReadOnlySpan<T> target)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        =>
            origin.IndexOf(ref MemoryMarshal.GetReference(target));
#else
    {
        fixed (T* value = target)
            return origin.IndexOf(ref *target.Align(value));
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OffsetOf<T>(this scoped Span<T> origin, scoped ReadOnlySpan<T> target) =>
        origin.ReadOnly().OffsetOf(target);

    /// <summary>Converts the provided <see cref="Span{T}"/> to the <see cref="Memory{T}"/>.</summary>
    /// <typeparam name="T">The type if items in the input <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to convert.</param>
    /// <param name="memory">The bounds.</param>
    /// <returns>The parameter <paramref name="span"/> as <see cref="ReadOnlyMemory{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe ReadOnlyMemory<T> AsMemory<T>(this scoped ReadOnlySpan<T> span, ReadOnlyMemory<T> memory)
    {
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return memory.Span.IndexOf(ref MemoryMarshal.GetReference(span)) is not -1 and var i
            ? memory.Slice(i, span.Length)
            : default;
#else
        var other = memory.Span;

        fixed (T* s = span)
        fixed (T* o = other)
            return ((nint)(span.Align(s) - other.Align(o)) is var elementOffset &&
                (nuint)elementOffset < (uint)span.Length
                    ? (int)elementOffset
                    : -1) is not -1 and var i
                ? memory.Slice(i, span.Length)
                : default;
#endif
    }

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this IMemoryOwner<T> owner, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(range);

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this ReadOnlyMemory<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Memory<T> Nth<T>(this Memory<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)memory.Length ? memory.Span[index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(memory.Length) is var o && (uint)o < (uint)memory.Length
                ? memory.Span.UnsafelyIndex(o)
                : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this IMemoryOwner<T> owner, int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.NthLast(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)memory.Length ? memory.Span[memory.Length - index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)memory.Length ? memory.Span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(memory.Length) is var off && (uint)off < (uint)memory.Length
                ? memory.Span.UnsafelyIndex(off)
                : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this Memory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)memory.Length ? memory.Span.UnsafelyIndex(memory.Length - index) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> Nth<T>(this ReadOnlySpan<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Nth<T>(this Span<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped Span<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped ReadOnlySpan<T> body, [NonNegativeValue] int index)
    {
        System.Diagnostics.Debug.Assert((uint)index < (uint)body.Length, "index is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        return body[index];
#endif
    }

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySkip<T>(this ReadOnlySpan<T> body, [NonNegativeValue] int start)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)start <= (uint)body.Length, "start is in range");
        return UnsafelySlice(body, start, body.Length - start);
    }

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySlice<T>(
        this ReadOnlySpan<T> body,
        [NonNegativeValue] int start,
        [NonNegativeValue] int length
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)(start + length) <= (uint)body.Length, "start and length is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        return body.Slice(start, length);
#endif
    }

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelyTake<T>(this ReadOnlySpan<T> body, [NonNegativeValue] int end)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)end <= (uint)body.Length, "end is in range");
        return UnsafelySlice(body, 0, end);
    }

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped Span<T> body, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)index < (uint)body.Length, "index is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        return body[index];
#endif
    }

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySkip<T>(this Span<T> body, [NonNegativeValue] int start)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)start <= (uint)body.Length, "start is in range");
        return UnsafelySlice(body, start, body.Length - start);
    }

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySlice<T>(
        this Span<T> body,
        [NonNegativeValue] int start,
        [NonNegativeValue] int length
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)(start + length) <= (uint)body.Length, "start and length is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        return body.Slice(start, length);
#endif
    }

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelyTake<T>(this Span<T> body, [NonNegativeValue] int end)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)end <= (uint)body.Length, "end is in range");
        return UnsafelySlice(body, 0, end);
    }

    /// <summary>Aligns the pointer obtained from a fixed expression to the first element of the pointer.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The span to obtain the pointer of.</param>
    /// <param name="pinned">The pointed obtained from pinning the parameter <paramref name="span"/>.</param>
    /// <returns>
    /// The pointer to the first element of the buffer, or <see langword="null"/>
    /// if the parameter <paramref name="span"/> is empty.
    /// </returns>
    [Inline]
    internal static unsafe T* Align<T>([UsedImplicitly] this ReadOnlySpan<T> span, T* pinned)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
            span.Length is 0 ? null :
            span.Pinnable is null ? (T*)span.ByteOffset :
            (T*)((byte*)Unsafe.AsPointer(ref span.Pinnable.Data) + span.ByteOffset);
#else
            pinned;
#endif
    /// <summary>Aligns the pointer obtained from a fixed expression to the first element of the pointer.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The span to obtain the pointer of.</param>
    /// <param name="pinned">The pointed obtained from pinning the parameter <paramref name="span"/>.</param>
    /// <returns>
    /// The pointer to the first element of the buffer, or <see langword="null"/>
    /// if the parameter <paramref name="span"/> is empty.
    /// </returns>
    [Inline]
    internal static unsafe T* Align<T>([UsedImplicitly] this Span<T> span, T* pinned)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
            span.Length is 0 ? null :
            span.Pinnable is null ? (T*)span.ByteOffset :
            (T*)((byte*)Unsafe.AsPointer(ref span.Pinnable.Data) + span.ByteOffset);
#else
            pinned;
#endif
}
