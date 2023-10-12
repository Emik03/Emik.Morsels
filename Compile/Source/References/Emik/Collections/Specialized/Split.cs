// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if !NET20 && !NET30
using static SplitFactory;

/// <summary>Extension methods that act as factories for <see cref="Split{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
    /// <summary>Gets all booleans, in the order defined by <see cref="Split{T}"/>.</summary>
    public static bool[] Booleans { get; } = { true, false };

    /// <summary>Splits an <see cref="IEnumerable{T}"/> in two based on a number.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="count">The number of elements in the first half.</param>
    /// <returns>
    /// A <see cref="Split{T}"/> instance that contains 2 enumerables containing the two halves of the underlying
    /// collection. The first half is as long as the parameter <paramref name="count"/> or shorter.
    /// </returns>
    [Pure]
    public static Split<IEnumerable<T>> SplitAt<T>(this ICollection<T> source, [NonNegativeValue] int count) =>
        new(source.Take(count), source.Skip(count));

    /// <summary>Splits an <see cref="IEnumerable{T}"/> in two based on a method provided.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="predicate">The method that decides where the item ends up.</param>
    /// <returns>
    /// A <see cref="Split{T}"/> instance that contains 2 enumerables containing the two halves of the underlying
    /// collection. The first half lasts until the first element that returned <see langword="true"/>.
    /// </returns>
    [Pure]
    public static Split<IEnumerable<T>> SplitWhen<T>(
        [InstantHandle] this ICollection<T> source,
        [InstantHandle] Func<T, bool> predicate
    )
    {
        var index = source.TakeWhile(Not1(predicate)).Count();
        return source.SplitAt(index);
    }

    /// <summary>Splits an <see cref="IEnumerable{T}"/> in two based on a method provided.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="predicate">The method that decides where the item ends up.</param>
    /// <returns>
    /// A <see cref="Split{T}"/> instance that contains 2 lists containing the elements that returned
    /// <see langword="true"/> and <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static Split<List<T>> SplitBy<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Predicate<T> predicate
    )
    {
        List<T> t = new(), f = new();

        foreach (var item in source)
#pragma warning disable RCS1235 // While AddRange is faster, the item is required for context.
            (predicate(item) ? t : f).Add(item);
#pragma warning restore RCS1235

        return new(t, f);
    }

    /// <summary>Splits an <see cref="IEnumerable{T}"/> in two based on a method provided.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="predicate">The method that decides where the item ends up.</param>
    /// <returns>
    /// A <see cref="Split{T}"/> instance that contains 2 lists containing the elements that returned
    /// <see langword="true"/> and <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static Split<SmallList<T>> SmallSplitBy<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Predicate<T> predicate
    )
    {
        SmallList<T> t = default, f = default;

        foreach (var item in source)
            (predicate(item) ? t : f).Add(item);

        return new(t, f);
    }
}
#endif

/// <summary>Represents a fixed collection of 2 items.</summary>
/// <typeparam name="T">The type of item in the collection.</typeparam>
/// <param name="truthy">The value representing a <see langword="true"/> value.</param>
/// <param name="falsy">The value representing a <see langword="false"/> value.</param>
sealed partial class Split<T>(T truthy, T falsy) : ICollection<T>,
    IDictionary<bool, T>,
    IReadOnlyCollection<T>,
    IReadOnlyDictionary<bool, T>
{
#pragma warning disable SA1642
    /// <summary>Initializes a new instance of the <see cref="Split{T}"/> class.</summary>
    /// <param name="value">The value representing both values.</param>
#pragma warning restore SA1642
    public Split(T value)
        : this(value, value) { }

    /// <summary>Gets or sets the value representing a <see langword="false"/> value.</summary>
    [Pure]
    public T Falsy
    {
        get => falsy;
        set => falsy = value;
    }

    /// <summary>Gets or sets the value representing a <see langword="true"/> value.</summary>
    [Pure]
    public T Truthy
    {
        get => truthy;
        set => truthy = value;
    }

    /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
    [Pure]
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [Pure, ValueRange(2)]
    int ICollection<T>.Count => 2;

    /// <inheritdoc />
    [Pure]
    public ICollection<T> Values => this;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
    [Pure]
    bool ICollection<KeyValuePair<bool, T>>.IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [Pure, ValueRange(2)]
    int ICollection<KeyValuePair<bool, T>>.Count => 2;

    /// <inheritdoc />
    [Pure]
    ICollection<bool> IDictionary<bool, T>.Keys => Booleans;

    /// <inheritdoc cref="IDictionary{TKey, TValue}.this" />
    [Pure]
    public T this[bool key]
    {
        get => key ? truthy : falsy;
        set => _ = key ? truthy = value : falsy = value;
    }

    /// <inheritdoc cref="ICollection{T}.Count" />
    [Pure, ValueRange(2)]
    int IReadOnlyCollection<T>.Count => 2;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [Pure, ValueRange(2)]
    int IReadOnlyCollection<KeyValuePair<bool, T>>.Count => 2;

    /// <inheritdoc />
    [Pure]
    IEnumerable<bool> IReadOnlyDictionary<bool, T>.Keys => Booleans;

    /// <inheritdoc />
    [Pure]
    IEnumerable<T> IReadOnlyDictionary<bool, T>.Values => Values;

    /// <inheritdoc />
    public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
    {
        array[arrayIndex] = truthy;
        array[arrayIndex + 1] = falsy;
    }

    /// <inheritdoc />
    [Pure]
    public bool Contains(T item) =>
        EqualityComparer<T>.Default.Equals(truthy, item) || EqualityComparer<T>.Default.Equals(falsy, item);

    /// <inheritdoc />
    [Pure]
    public IEnumerator<T> GetEnumerator()
    {
        yield return truthy;
        yield return falsy;
    }

    /// <inheritdoc />
    void ICollection<T>.Add(T item) { }

    /// <inheritdoc cref="ICollection{T}.Clear" />
    void ICollection<T>.Clear() { }

    /// <inheritdoc />
    [Pure]
    bool ICollection<T>.Remove(T item) => false;

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(bool key, T value) => _ = key ? truthy = value : falsy = value;

    /// <inheritdoc />
    // ReSharper disable once NullnessAnnotationConflictWithJetBrainsAnnotations
    public void Add(KeyValuePair<bool, T> item) => _ = item.Key ? truthy = item.Value : falsy = item.Value;

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<bool, T>[] array, [NonNegativeValue] int arrayIndex)
    {
        array[arrayIndex] = new(true, truthy);
        array[arrayIndex + 1] = new(false, falsy);
    }

    /// <inheritdoc />
    [Pure] // ReSharper disable once NullnessAnnotationConflictWithJetBrainsAnnotations
    public bool Contains(KeyValuePair<bool, T> item) =>
        item.Key
            ? EqualityComparer<T>.Default.Equals(truthy, item.Value)
            : EqualityComparer<T>.Default.Equals(falsy, item.Value);

    /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue" />
    [Pure]
    public bool TryGetValue(bool key, out T value)
    {
        value = key ? truthy : falsy;
        return true;
    }

    /// <inheritdoc cref="ICollection{T}.Clear" />
    void ICollection<KeyValuePair<bool, T>>.Clear() { }

    /// <inheritdoc />
    [Pure]
    bool ICollection<KeyValuePair<bool, T>>.Remove(KeyValuePair<bool, T> item) => false;

    /// <inheritdoc />
    [Pure]
    bool IDictionary<bool, T>.Remove(bool key) => false;

    /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey" />
    [Pure]
    bool IDictionary<bool, T>.ContainsKey(bool key) => true;

    /// <inheritdoc />
    [Pure]
    IEnumerator<KeyValuePair<bool, T>> IEnumerable<KeyValuePair<bool, T>>.GetEnumerator()
    {
        yield return new(true, truthy);
        yield return new(false, falsy);
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey" />
    [Pure]
    bool IReadOnlyDictionary<bool, T>.ContainsKey(bool key) => true;

    /// <summary>Deconstructs a <see cref="Split{T}"/> into its components.</summary>
    /// <param name="t">The value to get assigned as <see cref="Truthy"/>.</param>
    /// <param name="f">The value to get assigned as <see cref="Falsy"/>.</param>
    public void Deconstruct(out T t, out T f)
    {
        t = truthy;
        f = falsy;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"Split({truthy}, {falsy})";
}
