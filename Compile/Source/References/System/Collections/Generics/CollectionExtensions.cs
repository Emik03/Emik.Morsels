// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace
namespace System.Collections.Generic;
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
/// <summary>Provides extension methods for generic collections.</summary>
static class CollectionExtensions
{
#if NETFRAMEWORK && !NET45_OR_GREATER
    /// <inheritdoc cref="GetValueOrDefault{TKey,TValue}(IReadOnlyDictionary{TKey,TValue}, TKey)"/>
    // ReSharper disable once ReturnTypeCanBeNotNullable
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>

        // ReSharper disable once NullableWarningSuppressionIsUsed
        dictionary.GetValueOrDefault(key, default!);

    /// <inheritdoc cref="GetValueOrDefault{TKey,TValue}(IReadOnlyDictionary{TKey,TValue}, TKey, TValue)"/>
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue
    ) =>
        dictionary.TryGetValue(key, out var value) ? value : defaultValue;
#endif
    /// <summary>
    /// Tries to get the value associated with the specified
    /// <paramref name="key"/> in the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">
    /// A dictionary with keys of type <typeparamref name="TKey"/> and the values of type <typeparamref name="TValue"/>.
    /// </param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    /// A <typeparamref name="TValue"/> instance. When the method is successful, the returned
    /// object is the value associated with the specified <paramref name="key"/>.
    /// When the method fails, it returns the <see langword="default"/> value for <typeparamref name="TValue"/>.
    /// </returns>
    // ReSharper disable once ReturnTypeCanBeNotNullable
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key
    ) => // ReSharper disable once NullableWarningSuppressionIsUsed
        dictionary.GetValueOrDefault(key, default!);

    /// <summary>
    /// Tries to get the value associated with the specified
    /// <paramref name="key"/> in the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">
    /// A dictionary with keys of type <typeparamref name="TKey"/> and the values of type <typeparamref name="TValue"/>.
    /// </param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="defaultValue">
    /// The default value to return when the <paramref name="dictionary"/>
    /// cannot find a value associated with the specified <paramref name="key"/>.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TValue"/> instance. When the method is successful, the returned
    /// object is the value associated with the specified <paramref name="key"/>.
    /// When the method fails, it returns <paramref name="defaultValue"/>.
    /// </returns>
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue
    ) =>
        dictionary.TryGetValue(key, out var value) ? value : defaultValue;

    /// <summary>
    /// Tries to add the specified <paramref name="key"/> and <paramref name="value"/> to the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">
    /// A dictionary with keys of type <typeparamref name="TKey"/> and the values of type <typeparamref name="TValue"/>.
    /// </param>
    /// <param name="key">The key of the value to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>
    /// <see langword="true"/>when the <paramref name="key"/> and <paramref name="value"/> are successfully
    /// added to the <paramref name="dictionary"/>; <see langword="false"/> when the <paramref name="dictionary"/>
    /// already contains the specified <paramref name="key"/>, in which case nothing gets added.
    /// </returns>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
            return false;

        dictionary.Add(key, value);
        return true;
    }

    /// <summary>
    /// Tries to remove the value with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">
    /// A dictionary with keys of type <typeparamref name="TKey"/> and the values of type <typeparamref name="TValue"/>.
    /// </param>
    /// <param name="key">The key of the value to remove.</param>
    /// <param name="value">
    /// When this method returns <see langword="true"/>, the removed value; when this method returns
    /// <see langword="false"/>, the <see langword="default"/> value for <typeparamref name="TValue"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when a value is found in the <paramref name="dictionary"/> with the specified
    /// <paramref name="key"/>; <see langword="false"/> when the <paramref name="dictionary"/>
    /// cannot find a value associated with the specified <paramref name="key"/>.
    /// </returns>
    public static bool Remove<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [MaybeNullWhen(false)] out TValue value
    )
    {
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the specified list.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The list to wrap.</param>
    /// <returns>An object that acts as a read-only wrapper around the current <see cref="IList{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list) => new(list);
#if !NET20 && !NET30 && !NET35
    /// <summary>
    /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to wrap.</param>
    /// <returns>An object that acts as a read-only wrapper around the current <see cref="IDictionary{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull =>
        new(dictionary);
#endif
    /// <summary>Adds the elements of the specified span to the end of the <see cref="List{T}"/>.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to which the elements should be added.</param>
    /// <param name="source">The span whose elements should be added to the end of the <see cref="List{T}"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="list"/> is null.</exception>
    public static void AddRange<T>(this List<T> list, ReadOnlySpan<T> source)
    {
        if (source.IsEmpty)
            return;

        if (list.Capacity - list.Count < source.Length)
            list.Capacity = checked(list.Count + source.Length);

        foreach (var t in source)
            list.Add(t);
    }

    /// <summary>Inserts the elements of a span into the <see cref="List{T}"/> at the specified index.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list into which the elements should be inserted.</param>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
    /// <param name="source">The span whose elements should be added to the <see cref="List{T}"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than 0 or greater than <paramref name="list"/>'s <see cref="List{T}.Count"/>.
    /// </exception>
    public static void InsertRange<T>(this List<T> list, int index, ReadOnlySpan<T> source)
    {
        if ((uint)index > (uint)list.Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);

        if (source.IsEmpty)
            return;

        if (list.Capacity - list.Count < source.Length)
            list.Capacity = checked(list.Count + source.Length);

        for (var i = 0; i < source.Length; i++)
            list.Insert(index + i, source[i]);
    }

    /// <summary>Copies the entire <see cref="List{T}"/> to a span.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list from which the elements are copied.</param>
    /// <param name="destination">
    /// The span that is the destination of the elements copied from <paramref name="list"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The number of elements in the source <see cref="List{T}"/> is greater
    /// than the number of elements that the destination span can contain.
    /// </exception>
    public static void CopyTo<T>(this List<T> list, Span<T> destination)
    {
        for (var i = 0; i < list.Count; i++)
            destination[i] = list[i];
    }
}
#endif
