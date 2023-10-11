// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
// ReSharper disable RedundantNameQualifier RedundantUnsafeContext RedundantUsingDirective
namespace System.Runtime.InteropServices;
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
#pragma warning disable 8500
#endif
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
using static System.Linq.Expressions.Expression;
#endif

#pragma warning disable 1574 // Reference to System.Memory may not exist.
/// <summary>
/// Provides a collection of methods for interoperating with <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>,
/// <see cref="Span{T}"/>, and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
#pragma warning restore 1574
static partial class MemoryMarshal
{
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
    public static unsafe Span<T> CreateSpan<T>(scoped ref T reference, int length)
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (T* ptr = &reference)
            return new(ptr, length);
    }
#else
        =>
            Cache<T>.Span(ref reference, length);
#endif

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
    public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped ref T reference, int length)
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (T* ptr = &reference)
            return new(ptr, length);
    }
#else
        =>
            Cache<T>.ReadOnlySpan(ref reference, length);
#endif
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
    public static ref T GetReference<T>(ReadOnlySpan<T> span) => ref Unsafe.AsRef(span.GetPinnableReference());

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
    public static ref T GetReference<T>(Span<T> span) => ref span.GetPinnableReference();
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    static class Cache<T>
    {
        static Cache()
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

            Type[] args = { typeof(T).MakeByRefType(), typeof(int) };

            ParameterExpression[] parameters =
            {
                Parameter(typeof(T).MakeByRefType(), nameof(T)), Parameter(typeof(int), nameof(Int32)),
            };

            ReadOnlySpan = Make<ReadOnlySpanCreator<T>>(typeof(ReadOnlySpan<T>));
            Span = Make<SpanCreator<T>>(typeof(Span<T>));

            TTarget Make<TTarget>(Type type) =>
                Lambda<TTarget>(New(Constructor(type), parameters.AsEnumerable()), parameters).Compile();

            // ReSharper disable once NullableWarningSuppressionIsUsed
            ConstructorInfo Constructor(Type type) => type.GetConstructor(Flags, null, args, null)!;
        }

        /// <summary>Gets the invocation for creating a <see cref="ReadOnlySpan{T}"/>.</summary>
        [Pure]
        public static ReadOnlySpanCreator<T> ReadOnlySpan { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        /// <summary>Gets the invocation for creating a <see cref="Span{T}"/>.</summary>
        [Pure]
        public static SpanCreator<T> Span { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
    }

    delegate ReadOnlySpan<T> ReadOnlySpanCreator<T>(scoped ref T reference, int length);

    delegate Span<T> SpanCreator<T>(scoped ref T reference, int length);
#endif
}
#endif
