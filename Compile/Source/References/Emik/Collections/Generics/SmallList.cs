// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SmallFactory
#pragma warning restore MA0048
{
    /// <summary>Collects the enumerable; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerable<T>? iterable) => new(iterable);

    /// <summary>Mutates the enumerator; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterator"/>.</returns>
    [Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerator<T>? iterator) => new(iterator);
}
#endif

/// <summary>Inlines 3 elements before falling back on the heap with an expandable <see cref="IList{T}"/>.</summary>
/// <typeparam name="T">The element type.</typeparam>
[StructLayout(LayoutKind.Auto)]
partial struct SmallList<T> : IList<T>, IReadOnlyList<T>
{
    /// <summary>Number of items to keep inline for <see cref="SmallList{T}"/>.</summary>
    /// <remarks><para>
    /// And Saint Attila raised the <see cref="SmallList{T}"/> up on high, saying, "O Lord, bless this Thy
    /// <see cref="SmallList{T}"/> that, with it, Thou mayest blow Thine allocation costs to tiny bits in Thy mercy.".
    /// </para><para>
    /// And the Lord did grin, and the people did feast upon the lambs and sloths and carp and anchovies and orangutans
    /// and breakfast cereals and fruit bats and large chu...
    /// </para><para>
    /// And the Lord spake, saying, "First shalt thou recreate the <c>smallvec</c> (https://crates.io/crates/smallvec)
    /// crate. Then, shalt thou keep three inline. No more. No less. Three shalt be the number thou shalt keep inline,
    /// and the number to keep inline shalt be three. Four shalt thou not keep inline, nor either keep inline thou two,
    /// excepting that thou then proceed to three. Five is right out. Once the number three,  being the third number,
    /// be reached, then, lobbest thou thy <see cref="SmallList{T}"/> towards thy heap, who, being slow and
    /// cache-naughty in My sight, shall snuff it.".
    /// </para><para>
    /// (Source: https://github.com/rhaiscript/rhai/blob/ca18cdd7f47f8ae8bd6e2b7a950ad4815d62f026/src/lib.rs#L373).
    /// </para></remarks>
    public const int InlinedLength = 3;

    static readonly object
        s_one = new(),
        s_two = new(),
        s_three = new();

    [ProvidesContext]
    object? _rest;

    T? _first, _second, _third;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with no elements.</summary>
    public SmallList() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallList{T}"/> struct.
    /// Collects the enumerable; allocating the heaped list lazily.
    /// </summary>
    /// <param name="enumerable">The enumerable to collect.</param>
    public SmallList(IEnumerable<T>? enumerable)
        : this(enumerable?.GetEnumerator()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallList{T}"/> struct.
    /// Mutates the enumerator; allocating the heaped list lazily.
    /// </summary>
    /// <param name="enumerator">The enumerator to mutate.</param>
    public SmallList(IEnumerator<T>? enumerator)
    {
        if (!enumerator?.MoveNext() ?? true)
            return;

        _first = enumerator.Current;

        if (!enumerator.MoveNext())
        {
            _rest = s_one;
            return;
        }

        _second = enumerator.Current;

        if (!enumerator.MoveNext())
        {
            _rest = s_two;
            return;
        }

        _third = enumerator.Current;

        if (!enumerator.MoveNext())
        {
            _rest = s_three;
            return;
        }

        List<T> list = new();

        do
            list.Add(enumerator.Current);
        while (enumerator.MoveNext());

        _rest = list;
    }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 1 element.</summary>
    /// <param name="first">The first element.</param>
    public SmallList(T first)
        : this(first, default, default, s_one) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 2 elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    public SmallList(T first, T second)
        : this(first, second, default, s_two) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 3 elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    public SmallList(T first, T second, T third)
        : this(first, second, third, s_three) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with arbitrary elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The rest of the elements.</param>
    public SmallList(T first, T second, T third, IList<T> rest)
        : this(first, second, third, (object)rest) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with arbitrary elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The rest of the elements.</param>
    public SmallList(T first, T second, T third, params T[] rest)
        : this(first, second, third, (object)rest) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct. For internal use only.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The backing rest object, either a list or an object representing the length.</param>
    SmallList(T? first, T? second, T? third, object? rest)
    {
        _first = first;
        _second = second;
        _third = third;
        _rest = rest;
    }

    /// <summary>Gets the empty list.</summary>
    public static SmallList<T> Empty => default;

    /// <summary>Gets a value indicating whether determines whether the collection is empty.</summary>
    [CollectionAccess(None), Pure]
    public readonly bool IsEmpty => _rest is null;

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    public readonly bool IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [CollectionAccess(None), Pure]
    public readonly int Count =>
        _rest switch
        {
            null => 0,
            IList<T> list => list.Count + InlinedLength,
            _ when ReferenceEquals(_rest, s_one) => 1,
            _ when ReferenceEquals(_rest, s_two) => 2,
            _ => InlinedLength,
        };

    /// <inheritdoc cref="IList{T}.this" />
    public T this[int index]
    {
        [CollectionAccess(Read)]
        readonly get
        {
            BoundsCheck(index, out _);

            return index switch
            {
                0 => _first!,
                1 => _second!,
                2 => _third!,
                _ => Rest![index - InlinedLength],
            };
        }
        [CollectionAccess(ModifyExistingContent)]
        set
        {
            BoundsCheck(index, out _);

            _ = index switch
            {
                0 => _first = value,
                1 => _second = value,
                2 => _third = value,
                _ => Rest![index - InlinedLength] = value,
            };
        }
    }
#pragma warning disable MA0102
    /// <summary>Gets or sets the first element.</summary>
    [CollectionAccess(Read), Pure]
    public T First
    {
        readonly get => this[0];
        set => this[0] = value;
    }

    /// <summary>Gets or sets the second element.</summary>
    [CollectionAccess(Read), Pure]
    public T Second
    {
        readonly get => this[1];
        set => this[1] = value;
    }

    /// <summary>Gets or sets the third element.</summary>
    [CollectionAccess(Read), Pure]
    public T Third
    {
        readonly get => this[2];
        set => this[2] = value;
    }
#pragma warning restore MA0102

    /// <summary>Gets the rest of the elements.</summary>
    [CollectionAccess(None), ProvidesContext, Pure]
    public readonly IList<T>? Rest => _rest as IList<T>;

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent)]
    public void Add(T item)
    {
        switch (Count)
        {
            case 0:
                _first = item;
                _rest = s_one;
                break;
            case 1:
                _second = item;
                _rest = s_two;
                break;
            case 2:
                _third = item;
                _rest = s_three;
                break;
            default:
                EnsureMutability().Add(item);
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => _rest = null;

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public readonly void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
    {
        switch (Count)
        {
            case 0:
                array[arrayIndex] = _first!;
                break;
            case 1:
                array[arrayIndex] = _first!;
                array[arrayIndex + 1] = _second!;
                break;
            case 2:
                array[arrayIndex] = _first!;
                array[arrayIndex + 1] = _second!;
                array[arrayIndex + 2] = _third!;
                break;
            default:
                array[arrayIndex] = _first!;
                array[arrayIndex + 1] = _second!;
                array[arrayIndex + 2] = _third!;
                Rest?.CopyTo(array, arrayIndex + InlinedLength);
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T item)
    {
        BoundsCheck(index, out var count);

        _rest = count switch
        {
            0 => s_one,
            1 => s_two,
            2 => s_three,
            _ => _rest,
        };

        if (count >= InlinedLength)
            EnsureMutability().Insert(0, _third!);

        switch (index)
        {
            case 0:
                _third = _second;
                _second = _first;
                _first = item;
                break;
            case 1:
                _third = _second;
                _second = item;
                break;
            case 2:
                _third = item;
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public void RemoveAt(int index)
    {
        BoundsCheck(index, out var count);

        if (index is 0)
            _first = _second;

        if (index is 0 or 1)
            _second = _third;

        if (index < InlinedLength && Rest is [var head, ..])
            _third = head;

        if (count > InlinedLength)
            EnsureMutability().RemoveAt(Math.Max(index - InlinedLength, 0));

        _rest = count switch
        {
            1 => null,
            2 => s_one,
            3 => s_two,
            _ => _rest,
        };
    }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public readonly bool Contains(T item) =>
        Count switch
        {
            0 => Eq(_first, item),
            1 => Eq(_first, item) || Eq(_second, item),
            2 => Eq(_first, item) || Eq(_second, item) || Eq(_third, item),
            _ => Eq(_first, item) || Eq(_second, item) || Eq(_third, item) || (Rest?.Contains(item) ?? false),
        };

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public bool Remove(T item) =>
        Count switch
        {
            0 => false,
            1 => Eq(_first, item) && (_rest = null) is var _,
            2 => Eq(_first, item)
                ? (_rest = s_one) is var _ && (_first = _second) is var _
                : Eq(_second, item) && (_rest = s_one) is var _,
            _ => Eq(_first, item) ? RemoveHead(_first = _second) :
                Eq(_second, item) ? RemoveHead(_second = _third) :
                Eq(_third, item) ? RemoveHead() : EnsureMutability().Remove(item),
        };

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public readonly int IndexOf(T item) =>
        Count switch
        {
            0 => Eq(_first, item) ? 0 : -1,
            1 => Eq(_first, item) ? 0 :
                Eq(_second, item) ? 1 : -1,
            2 => Eq(_first, item) ? 0 :
                Eq(_second, item) ? 1 :
                Eq(_third, item) ? 2 : -1,
            _ => Eq(_first, item) ? 0 :
                Eq(_second, item) ? 1 :
                Eq(_third, item) ? 2 : Rest?.IndexOf(item) ?? -1,
        };

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public readonly override string ToString() =>
        Count switch
        {
            0 => "[]",
            1 => $"[{_first}]",
            2 => $"[{_first}, {_second}]",
            3 => $"[{_first}, {_second}, {_third}]",
            _ when Rest is { } rest => $"[{_first}, {_second}, {_third}, {rest.Conjoin()}]",
            _ => $"[{_first}, {_second}, {_third}, ..{_rest} ]",
        };

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    public readonly IEnumerator<T> GetEnumerator()
    {
        if (Count is var count && count is 0)
            yield break;

        yield return _first!;

        if (count is 1)
            yield break;

        yield return _second!;

        if (count is 2)
            yield break;

        yield return _third!;

        if (Rest is not { } rest)
            yield break;

        foreach (var next in rest)
            yield return next;
    }

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    static bool Eq(T? left, T? right) => EqualityComparer<T>.Default.Equals(left, right);

    readonly void BoundsCheck(int index, [ValueRange(1, int.MaxValue)] out int count)
    {
        count = Count;

        if (unchecked((uint)index >= count))
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Must be between 0 and {count}");
    }

    // ReSharper disable once UnusedParameter.Local
    bool RemoveHead(T? _ = default)
    {
        if (Rest is [var head, ..])
        {
            _third = head;
            EnsureMutability().RemoveAt(0);
        }
        else
            _rest = s_two;

        return true;
    }

    IList<T> EnsureMutability()
    {
        var rest = Rest switch
        {
            { IsReadOnly: true } x => x.ToList(),
            { } x => x,
            _ => new List<T>(),
        };

        _rest = rest;
        return rest;
    }
}
