// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="Split{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SplitFactory
#pragma warning restore MA0048
{
    /// <summary>Splits an <see cref="IEnumerable{T}"/> in two based on a method provided.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="predicate">The method that decides where the item ends up.</param>
    /// <returns>
    /// A <see cref="Split{T}"/> instance that contains 2 enumerables containing the two halves of the underlying
    /// collection. The first half lasts until the first element that returned <see langword="true"/>.
    /// </returns>
    internal static Split<IEnumerable<T>> SplitAt<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var index = source.TakeWhile(Not1(predicate)).Count();
        return new(source.Skip(index), source.Take(index));
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
    internal static Split<List<T>> SplitBy<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        List<T> t = new(), f = new();

        foreach (var item in source)
#pragma warning disable RCS1235 // While AddRange is faster, the item is required for context.
            (predicate(item) ? t : f).Add(item);
#pragma warning restore RCS1235

        return new(t, f);
    }
}

/// <summary>Represents a fixed collection of 2 items.</summary>
/// <typeparam name="T">The type of item in the collection.</typeparam>
sealed partial class Split<T> : ICollection<T>,
    IDictionary<bool, T>,
    IReadOnlyCollection<T>,
    IReadOnlyDictionary<bool, T>
{
    [ProvidesContext]
    static readonly bool[] s_booleans = { true, false };

    /// <summary>Initializes a new instance of the <see cref="Split{T}"/> class.</summary>
    /// <param name="value">The value representing both values.</param>
    public Split(T value)
        : this(value, value) { }

    /// <summary>Initializes a new instance of the <see cref="Split{T}"/> class.</summary>
    /// <param name="truthy">The value representing a <see langword="true"/> value.</param>
    /// <param name="falsy">The value representing a <see langword="false"/> value.</param>
    public Split(T truthy, T falsy)
    {
        Truthy = truthy;
        Falsy = falsy;
    }

    /// <summary>Gets or sets the value representing a <see langword="false"/> value.</summary>
    [Pure]
    public T Falsy { get; set; }

    /// <summary>Gets or sets the value representing a <see langword="true"/> value.</summary>
    [Pure]
    public T Truthy { get; set; }

    /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
    [Pure]
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [Pure, ValueRange(2)]
    int ICollection<T>.Count => 2;

    /// <inheritdoc />
    public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
    {
        array[arrayIndex] = Truthy;
        array[arrayIndex + 1] = Falsy;
    }

    /// <inheritdoc />
    [Pure]
    public bool Contains(T item) =>
        EqualityComparer<T>.Default.Equals(Truthy, item) ||
        EqualityComparer<T>.Default.Equals(Falsy, item);

    /// <inheritdoc />
    [Pure]
    public IEnumerator<T> GetEnumerator()
    {
        yield return Truthy;
        yield return Falsy;
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
    ICollection<bool> IDictionary<bool, T>.Keys => s_booleans;

    /// <inheritdoc cref="IDictionary{TKey, TValue}.this" />
    [Pure]
    public T this[bool key]
    {
        get => key ? Truthy : Falsy;
        set => _ = key ? Truthy = value : Falsy = value;
    }

    /// <inheritdoc />
    public void Add(bool key, T value) => _ = key ? Truthy = value : Falsy = value;

    /// <inheritdoc />
    public void Add(KeyValuePair<bool, T> item) => _ = item.Key ? Truthy = item.Value : Falsy = item.Value;

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<bool, T>[] array, [NonNegativeValue] int arrayIndex)
    {
        array[arrayIndex] = new(true, Truthy);
        array[arrayIndex + 1] = new(false, Falsy);
    }

    /// <inheritdoc />
    [Pure]
    public bool Contains(KeyValuePair<bool, T> item) =>
        item.Key
            ? EqualityComparer<T>.Default.Equals(Truthy, item.Value)
            : EqualityComparer<T>.Default.Equals(Falsy, item.Value);

    /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue" />
    [Pure]
    public bool TryGetValue(bool key, out T value)
    {
        value = key ? Truthy : Falsy;
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
        yield return new(true, Truthy);
        yield return new(false, Falsy);
    }

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
    [Pure, ValueRange(2)]
    int IReadOnlyCollection<T>.Count => 2;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
    [Pure, ValueRange(2)]
    int IReadOnlyCollection<KeyValuePair<bool, T>>.Count => 2;

    /// <inheritdoc />
    [Pure]
    IEnumerable<bool> IReadOnlyDictionary<bool, T>.Keys => s_booleans;

    /// <inheritdoc />
    [Pure]
    IEnumerable<T> IReadOnlyDictionary<bool, T>.Values => Values;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey" />
    [Pure]
    bool IReadOnlyDictionary<bool, T>.ContainsKey(bool key) => true;

    /// <summary>Deconstructs a <see cref="Split{T}"/> into its components.</summary>
    /// <param name="truthy">The value to get assigned as <see cref="Truthy"/>.</param>
    /// <param name="falsy">The value to get assigned as <see cref="Falsy"/>.</param>
    public void Deconstruct(out T truthy, out T falsy)
    {
        truthy = Truthy;
        falsy = Falsy;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"Split({Truthy}, {Falsy})";
}
