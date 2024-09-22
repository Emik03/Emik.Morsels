// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Class for obtaining the underlying data for lists.</summary>
static partial class ListMarshal
{
#if !NET9_0_OR_GREATER
    /// <summary>Contains the cached method for obtaining the underlying array.</summary>
    /// <typeparam name="T">The element type within the collection.</typeparam>
    static class ListCache<T>
    {
        /// <summary>Gets the converter.</summary>
        public static Converter<List<T>, T[]> Converter { get; } =
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
            typeof(List<T>)
           .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
           .FirstOrDefault(x => x.FieldType == typeof(T[])) is { } method
            ? CreateGetter(method)
            :
#endif
            x => [.. x];
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
        /// <summary>Creates the getter to the inner array.</summary>
        /// <param name="field">The field to the list's array.</param>
        /// <exception cref="InvalidOperationException">The field has no declaring type.</exception>
        /// <returns>The getter to the inner array.</returns>
        static Converter<List<T>, T[]> CreateGetter(FieldInfo field)
        {
            if (field.DeclaringType is not { } declaringType)
                throw new InvalidOperationException("Field has no declaring type.");

            var param = Expression.Parameter(declaringType, field.Name);
            var access = Expression.Field(param, field);
            return Expression.Lambda<Converter<List<T>, T[]>>(access, param).Compile();
#endif
        }
    }
#endif
    /// <summary>Gets the underlying array of the <see cref="List{T}"/>.</summary>
    /// <remarks><para>
    /// While unlikely, it is theoretically possible that the framework's implementation of
    /// <see cref="List{T}"/> lacks any references to its underlying array, or at least
    /// directly. In that case, a new array is made, holding no reference to the list.
    /// </para><para>
    /// If you want to ensure maximum compatibility, the implementation should not rely on whether
    /// mutations within the array would affect the <see cref="List{T}"/>, and vice versa.
    /// </para><para>
    /// Regardless of framework, mutations within the array will not notify the list during its enumerations which can
    /// easily cause bugs to slip through.
    /// </para><para>
    /// The array may contain uninitialized memory for all elements past <see cref="List{T}.Count"/>.
    /// </para><para>
    /// Uses of this method include obtaining a <see cref="ReadOnlySpan{T}"/> or <see cref="Span{T}"/> outside of
    /// .NET 5+ projects, as <c>CollectionsMarshal.AsSpan&lt;T&gt;</c> is not available there, or obtaining
    /// <c>ReadOnlyMemory&lt;T&gt;</c> or <c>Memory&lt;T&gt;</c> of a <see cref="List{T}"/>, normally impossible,
    /// or if growth of an array is no longer needed but the contents are expected to be long-lasting.
    /// </para><para>
    /// Whatever your use case, remember this: "It's not a b&#x0075;g, it's an undocumented feature.".
    /// </para></remarks>
    /// <typeparam name="T">The type of list.</typeparam>
    /// <param name="list">The list to obtain the underlying array.</param>
    /// <returns>The array of the parameter <paramref name="list"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T[] UnsafelyToArray<T>(this List<T> list)
#if NET9_0_OR_GREATER
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
        static extern ref readonly T[] Items(List<T> list);

        return Items(list);
    }
#else
        =>
            ListCache<T>.Converter(list);
#endif
}
