// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
// ReSharper disable BadPreprocessorIndent RedundantNameQualifier RedundantUnsafeContext RedundantUsingDirective
namespace System.Runtime.InteropServices;
#pragma warning disable 8500, SA1137
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP2_1_OR_GREATER
#pragma warning disable 1574 // Reference to System.Memory may not exist.
/// <summary>
/// Provides a collection of methods for interoperating with <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>,
/// <see cref="Span{T}"/>, and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
#pragma warning restore 1574
static partial class MemoryMarshal
{
    /// <summary>
    /// Casts a Span of one primitive type <typeparamref name="TFrom"/>
    /// to another primitive type <typeparamref name="TTo"/>.
    /// These types may not contain pointers or references. This is checked at runtime in order to preserve type safety.
    /// </summary>
    /// <remarks><para>
    /// Supported only for platforms that support misaligned memory
    /// access or when the memory block is aligned by other means.
    /// </para></remarks>
    /// <typeparam name="TFrom">The type of the source span.</typeparam>
    /// <typeparam name="TTo">The type of the target span.</typeparam>
    /// <param name="span">The source slice, of type <typeparamref name="TFrom"/>.</param>
    /// <returns>The converted span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe Span<TTo> Cast<TFrom, TTo>(Span<TFrom> span)
        where TFrom : struct
        where TTo : struct
    {
        // Use unsigned integers - unsigned division by constant (especially by power of 2)
        // and checked casts are faster and smaller.
        var fromSize = (uint)Unsafe.SizeOf<TFrom>();
        var toSize = (uint)Unsafe.SizeOf<TTo>();
        var fromLength = (uint)span.Length;

        var toLength = fromSize == toSize ? (int)fromLength :
            fromSize is not 1 &&
            (ulong)fromLength * fromSize / toSize is var toLengthUInt64 ? checked((int)toLengthUInt64) :
            (int)(fromLength / toSize);

        fixed (TFrom* ptr = span)
            return new(span.Align(ptr), toLength);
    }

    /// <summary>
    /// Casts a ReadOnlySpan of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
    /// These types may not contain pointers or references. This is checked at runtime in order to preserve type safety.
    /// </summary>
    /// <remarks><para>
    /// Supported only for platforms that support misaligned memory
    /// access or when the memory block is aligned by other means.
    /// </para></remarks>
    /// <typeparam name="TFrom">The type of the source span.</typeparam>
    /// <typeparam name="TTo">The type of the target span.</typeparam>
    /// <param name="span">The source slice, of type <typeparamref name="TFrom"/>.</param>
    /// <returns>The converted read-only span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe ReadOnlySpan<TTo> Cast<TFrom, TTo>(ReadOnlySpan<TFrom> span)
        where TFrom : struct
        where TTo : struct
    {
        // Use unsigned integers - unsigned division by constant (especially by power of 2)
        // and checked casts are faster and smaller.
        var fromSize = (uint)Unsafe.SizeOf<TFrom>();
        var toSize = (uint)Unsafe.SizeOf<TTo>();
        var fromLength = (uint)span.Length;

        var toLength = fromSize == toSize ? (int)fromLength :
            fromSize is not 1 &&
            (ulong)fromLength * fromSize / toSize is var toLengthUInt64 ? checked((int)toLengthUInt64) :
            (int)(fromLength / toSize);

        fixed (TFrom* ptr = span)
            return new(span.Align(ptr), toLength);
    }

    /// <summary>
    /// Create a new span over a portion of a regular managed object. This can be useful
    /// if part of a managed object represents a "fixed array." This is dangerous because the
    /// <paramref name="length"/> is not checked.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
    /// <returns>The lifetime of the returned span will not be validated for safety by span-aware languages.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> CreateSpan<T>(scoped ref T reference, int length) =>
        Cache<T>.Span(ref reference, length);

    /// <summary>
    /// Create a new read-only span over a portion of a regular managed object. This can be useful
    /// if part of a managed object represents a "fixed array." This is dangerous because the
    /// <paramref name="length"/> is not checked.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
    /// <returns>The lifetime of the returned span will not be validated for safety by span-aware languages.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped ref T reference, int length) =>
        Cache<T>.ReadOnlySpan(ref reference, length);
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Returns a reference to the element of the read-only span at index 0.</summary>
    /// <remarks><para>
    /// If the read-only span is empty, this method returns a reference to the location where the
    /// element at index 0 would have been stored. Such a reference may or may not be null.
    /// The returned reference can be used for pinning, but it must never be dereferenced.
    /// </para></remarks>
    /// <typeparam name="T">The type of items in the span.</typeparam>
    /// <param name="span">The read-only from which the reference is retrieved.</param>
    /// <returns>A reference to the element at index 0.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref T GetReference<T>(ReadOnlySpan<T> span) =>
        ref Unsafe.AsRef(span.GetPinnableReference());

    /// <summary>Returns a reference to the element of the span at index 0.</summary>
    /// <remarks><para>
    /// If the span is empty, this method returns a reference to the location where the
    /// element at index 0 would have been stored. Such a reference may or may not be null.
    /// The returned reference can be used for pinning, but it must never be dereferenced.
    /// </para></remarks>
    /// <typeparam name="T">The type of items in the span.</typeparam>
    /// <param name="span">The span from which the reference is retrieved.</param>
    /// <returns>A reference to the element at index 0.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref T GetReference<T>(Span<T> span) =>
        ref Unsafe.AsRef(span.GetPinnableReference());
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once NullableWarningSuppressionIsUsed
    public static T GetReference<T>(Span<T> span) => span.IsEmpty ? default! : span.UnsafelyIndex(0);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once NullableWarningSuppressionIsUsed
    public static T GetReference<T>(ReadOnlySpan<T> span) => span.IsEmpty ? default! : span.UnsafelyIndex(0);
#endif
    static class Cache<T>
    {
        static Cache()
        {
            const BindingFlags ConstructorFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            const BindingFlags FactoryFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            Type[] args = [typeof(T).MakeByRefType(), typeof(int)];

            ParameterExpression[] parameters =
            [
                System.Linq.Expressions.Expression.Parameter(args[0], nameof(T)),
                System.Linq.Expressions.Expression.Parameter(args[1], nameof(Int32)),
            ];

            ReadOnlySpan = Make<ReadOnlySpanFactory<T>>() ?? InefficientReadOnlySpanFallback;
            Span = Make<SpanFactory<T>>() ?? InefficientSpanFallback;

            static MethodInfo? Factory(Type? type) =>
                type
                  ?.Assembly
                   .GetType("System.Runtime.InteropServices.MemoryMarshal")
                  ?.GetMethod($"Create{type.Name}", FactoryFlags)
                  ?.MakeGenericMethod(typeof(T));

            ConstructorInfo? Constructor(Type? type) => type?.GetConstructor(ConstructorFlags, null, args, null);

            TTarget? Make<TTarget>() =>
                typeof(TTarget).GetMethod(nameof(Invoke))?.ReturnType is var type &&
                Constructor(type) is { } constructor ?
                    System.Linq.Expressions.Expression.Lambda<TTarget>(
                            System.Linq.Expressions.Expression.New(constructor, parameters.OfType<Expression>()),
                            parameters
                        )
                       .Compile() :
                    Factory(type) is { } factory ?
                        System.Linq.Expressions.Expression.Lambda<TTarget>(
                                System.Linq.Expressions.Expression.Call(
                                    factory,
                                    parameters.OfType<Expression>().ToArray()
                                ),
                                parameters
                            )
                           .Compile() : default;
        }

        /// <summary>Gets the invocation for creating a <see cref="ReadOnlySpan{T}"/>.</summary>
        [Pure]
        public static ReadOnlySpanFactory<T> ReadOnlySpan { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        /// <summary>Gets the invocation for creating a <see cref="Span{T}"/>.</summary>
        [Pure]
        public static SpanFactory<T> Span { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static ReadOnlySpan<T> InefficientReadOnlySpanFallback(scoped ref T reference, int length) =>
            InefficientSpanFallback(ref reference, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static unsafe Span<T> InefficientSpanFallback(scoped ref T reference, int length)
        {
            Span<T> span = new T[length];
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            Unsafe.CopyBlock(
                ref Unsafe.As<T, byte>(ref reference),
                ref Unsafe.As<T, byte>(ref span.GetPinnableReference()),
                unchecked((uint)(length * Unsafe.SizeOf<T>()))
            );
#else
            fixed (T* ptr = &reference)
                new ReadOnlySpan<T>(ptr, length).CopyTo(span);
#endif
            return span;
        }
    }

    delegate ReadOnlySpan<T> ReadOnlySpanFactory<T>(scoped ref T reference, int length);

    delegate Span<T> SpanFactory<T>(scoped ref T reference, int length);
}
#endif
