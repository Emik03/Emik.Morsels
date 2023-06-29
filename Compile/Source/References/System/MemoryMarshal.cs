// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Runtime.InteropServices;

using static Expression;

#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || !NO_SYSTEM_MEMORY
/// <summary>
/// Provides a collection of methods for interoperating with <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>,
/// <see cref="Span{T}"/>, and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
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
    public static Span<T> CreateSpan<T>(ref T reference, int length) => Cache<T>.Span(ref reference, length);

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
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(ref T reference, int length) =>
        Cache<T>.ReadOnlySpan(ref reference, length);

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

    delegate ReadOnlySpan<T> ReadOnlySpanCreator<T>(ref T reference, int length);

    delegate Span<T> SpanCreator<T>(ref T reference, int length);
}
#endif
