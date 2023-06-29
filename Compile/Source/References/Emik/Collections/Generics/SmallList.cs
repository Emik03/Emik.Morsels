// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;
using static Span;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SmallFactory
#pragma warning restore MA0048
{
    /// <summary>Creates the <see cref="SmallList{T}"/> with the specified capacity.</summary>
    /// <typeparam name="T">The type of list.</typeparam>
    /// <param name="length">The capacity.</param>
    /// <param name="item">The item to repeat.</param>
    /// <returns>
    /// The <see cref="SmallList{T}"/> with the capacity specified according to <paramref name="length"/>.
    /// </returns>
    [Pure]
    public static SmallList<T> AsSmallList<T>(this int length, T item) =>
        length switch
        {
            <= 0 => default,
            1 => new(item),
            2 => new(item, item),
            3 => new(item, item, item),
            _ => new(item, item, item, Enumerable.Repeat(item, length - SmallList<T>.InlinedLength).ToListLazily()),
        };

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
[StructLayout(LayoutKind.Sequential)]
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
#pragma warning disable RCS1158
    public const int InlinedLength = 3;
#pragma warning restore RCS1158

    static readonly object
        s_one = new(),
        s_two = new();

    static readonly T[] s_empty =
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
        new T[0];
#else
        Array.Empty<T>();
#endif

    T? _first, _second, _third;

    [ProvidesContext]
    object? _rest;

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
            _rest = s_empty;
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
        : this(first, second, third, s_empty) { }

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
            _ when _rest == s_one => 1,
            _ when _rest == s_two => 2,
            _ when _rest == s_empty => 3,
            _ => Rest!.Count + InlinedLength,
        };

    /// <summary>Gets the number of head elements used.</summary>
    [CollectionAccess(None), Pure]
    public readonly int HeadCount => Math.Min(Count, 3);

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

    /// <summary>Creates the collection with 1 item in it.</summary>
    /// <param name="value">The single item to use.</param>
    /// <returns>The collection with 1 item.</returns>
    public static implicit operator SmallList<T>(T value) => new(value);

    /// <summary>Creates the collection with 2 items in it.</summary>
    /// <param name="tuple">The tuple containing 2 items to destructure and use.</param>
    /// <returns>The collection with 2 items.</returns>
    public static implicit operator SmallList<T>((T First, T Second) tuple) => new(tuple.First, tuple.Second);

    /// <summary>Creates the collection with 3 items in it.</summary>
    /// <param name="tuple">The tuple containing 3 items to destructure and use.</param>
    /// <returns>The collection with 3 items.</returns>
    public static implicit operator SmallList<T>((T First, T Second, T Third) tuple) =>
        new(tuple.First, tuple.Second, tuple.Third);

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
                _rest = s_empty;
                break;
            default:
                EnsureMutability().Add(item);
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => _rest = null;

    /// <summary>Copies all values onto the destination.</summary>
    /// <param name="list">The destination.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="list"/> has less elements than itself.
    /// </exception>
    [CollectionAccess(Read)]
    public readonly void CopyTo(ref SmallList<T> list)
    {
        var count = Count;
        list.BoundsCheck(count - 1, out _);

        // Takes advantage of fallthrough in switch-cases.
        switch (2 - count)
        {
            case < 0:
                IList<T>
                    from = Rest!,
                    to = list.Rest!;

                for (var i = 0; i < from.Count; i++)
                    to[i] = from[i];

                goto case 0;
            case 0:
                list._third = _third!;
                goto case 1;
            case 1:
                list._second = _second!;
                goto case 2;
            case 2:
                list._first = _first!;
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public readonly void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
    {
        // Takes advantage of fallthrough in switch-cases.
        switch (2 - Count)
        {
            case < 0:
                Rest!.CopyTo(array, arrayIndex + InlinedLength);
                goto case 0;
            case 0:
                array[arrayIndex + 2] = _third!;
                goto case 1;
            case 1:
                array[arrayIndex + 1] = _second!;
                goto case 2;
            case 2:
                array[arrayIndex] = _first!;
                break;
        }
    }

    /// <summary>Deconstructs this instance with its properties.</summary>
    /// <param name="head">The first three elements.</param>
    /// <param name="tail">The remaining elements.</param>
    public readonly void Deconstruct(out (T? First, T? Second, T? Third) head, out IList<T> tail) =>
        Deconstruct(out head.First, out head.Second, out head.Third, out tail);

    /// <summary>Deconstructs this instance with the 3 first elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    public readonly void Deconstruct(out T? first, out T? second, out T? third)
    {
        first = _first;
        second = _second;
        third = _third;
    }

    /// <summary>Deconstructs this instance with its properties.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The remaining elements.</param>
    public readonly void Deconstruct(out T? first, out T? second, out T? third, out IList<T> rest)
    {
        Deconstruct(out first, out second, out third);
        rest = Rest ?? s_empty;
    }

#pragma warning disable 8500
    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <param name="func">The function to use.</param>
    [CollectionAccess(Read)]
    public unsafe void HeadSpan([InstantHandle, RequireStaticDelegate] SpanAction<T> func)
    {
        fixed (SmallList<T>* ptr = &this)
            func(new(ptr, HeadCount));
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    [CollectionAccess(Read)]
    public unsafe void HeadSpan<TParam>(
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanAction<T, TParam> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            func(new(ptr, HeadCount), param);
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    [CollectionAccess(Read)]
    public unsafe void HeadSpan<TParam>(
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<T, TParam> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            func(new(ptr, HeadCount), param);
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    [CollectionAccess(Read)]
    public unsafe void HeadSpan<TParam>(
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<T, TParam> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            func(new(ptr, HeadCount), param);
    }
#pragma warning restore 8500

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T item)
    {
        BoundsCheck(index, out var count);

        _rest = count switch
        {
            0 => s_one,
            1 => s_two,
            2 => s_empty,
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

    /// <summary>Determines whether the item exists in the collection.</summary>
    /// <param name="item">The item to check.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The value determining whether the parameter <paramref name="item"/> exists in the collection.</returns>
    [CollectionAccess(Read), Pure]
    public readonly bool Contains(T item, IEqualityComparer<T?> comparer) =>
        Count switch
        {
            0 => comparer.Equals(_first, item),
            1 => comparer.Equals(_first, item) || comparer.Equals(_second, item),
            2 => comparer.Equals(_first, item) || comparer.Equals(_second, item) || comparer.Equals(_third, item),
            _ => comparer.Equals(_first, item) ||
                comparer.Equals(_second, item) ||
                comparer.Equals(_third, item) ||
                (Rest?.Contains(item, comparer) ?? false),
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
#pragma warning disable CS8500
    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="func">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="func"/>.</returns>
    [CollectionAccess(Read), MustUseReturnValue]
    public unsafe TResult HeadSpan<TResult>([InstantHandle, RequireStaticDelegate] SpanFunc<T, TResult> func)
    {
        fixed (SmallList<T>* ptr = &this)
            return func(new(ptr, HeadCount));
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="func"/>.</returns>
    [CollectionAccess(Read), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanFunc<T, TParam, TResult> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            return func(new(ptr, HeadCount), param);
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="func"/>.</returns>
    [CollectionAccess(Read), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<T, TParam, TResult> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            return func(new(ptr, HeadCount), param);
    }

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="func">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="func"/>.</returns>
    [CollectionAccess(Read), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<T, TParam, TResult> func
    )
    {
        fixed (SmallList<T>* ptr = &this)
            return func(new(ptr, HeadCount), param);
    }
#pragma warning restore CS8500
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    [CollectionAccess(None), Pure]
    public readonly Enumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    static bool Eq(T? x, T? y) => x is null ? y is null : y is not null && EqualityComparer<T>.Default.Equals(x, y);

    readonly void BoundsCheck(int index, [ValueRange(1, int.MaxValue)] out int count)
    {
        count = Count;

        if (unchecked((uint)index >= count))
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Must be between 0 and {count - 1}");
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
            { IsReadOnly: true, Count: not 0 } x => x.ToList(),
            { Count: not 0 } x => x,
            _ => new List<T>(),
        };

        _rest = rest;
        return rest;
    }

    /// <summary>An enumerator over <see cref="SmallList{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IEnumerator<T>
    {
        readonly SmallList<T> _list;

        readonly IEnumerator<T>? _enumerator;

        readonly int _count;

        int _state = -1;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="list">The <see cref="SmallList{T}"/> to enumerate over.</param>
        public Enumerator(SmallList<T> list)
        {
            _list = list;
            _count = list.Count;
            _enumerator = list.Rest?.GetEnumerator();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc />
        public readonly void Dispose() => _enumerator?.Dispose();

        /// <inheritdoc />
        public void Reset()
        {
            _state = -1;
            _enumerator?.Reset();
        }

        /// <inheritdoc />
        public bool MoveNext() =>
            ++_state < _count &&
            (_state < InlinedLength || (_enumerator?.MoveNext() ?? false)) ==
            (Current = _state switch
            {
                0 => _list._first!,
                1 => _list._second!,
                2 => _list._third!,
                _ => _enumerator is null ? default! : _enumerator.Current,
            }) is var _;
    }
}
