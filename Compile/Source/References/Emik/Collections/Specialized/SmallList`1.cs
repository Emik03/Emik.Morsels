// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable MEN010
using static CollectionAccessType;
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
using static SmallList;
#endif
using static Span;

/// <summary>Inlines 3 elements before falling back on the heap with an expandable <see cref="IList{T}"/>.</summary>
/// <typeparam name="T">The element type.</typeparam>
[NoStructuralTyping, StructLayout(LayoutKind.Sequential)]
partial struct SmallList<T> :
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
    IConvertible,
#endif
    IEquatable<SmallList<T>>,
    IList<T>,
    IReadOnlyList<T>
{
    static readonly object
        s_one = new(),
        s_two = new();

    static readonly T[] s_empty =
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
        new T[0];
#else
        [];
#endif

    // DO NOT PLACE _rest BELOW GENERICS;
    // Doing so will cause only unmanaged generics to be mapped before the object,
    // leading to inconsistent mapping of memory when dereferencing pointers in HeadSpan functions.
    // See the following link for a lengthier explanation and example:
    //
    // ReSharper disable CommentTypo
    // https://sharplab.io/#v2:EYLgtghglgdgPgYgA4CcIHNIAIDuEUyzpYAmUAzhMADYCmWAHAKwAMLAsAFBcACLWPAIwA6AEoBXGABcoYWsICS02igD2SAMoqAblADGtcgG5e/IQDYBIidNnylUleq0pdB8sIAyEAJ6rxUgDSsCQm3JwA9BFYALIQMBi0JLiqKADW5FgQmbQAHki0eo4kIFxRWADkAILAqtq0FVgotACO4lDNmfFYqgBmveS0UljAhRDig1hSABb0qsAAVoXDeqpymb0d5FLCZdEVAEK01Ko4jc1tHYZYMKo9/YPDo3rjkzNzi8tYq+tZvY4oXbhbT4LK1eoaKQoLAAXhutBwWBqdVoAB4hCwAHwACgARJsUNtcQAaLC4warGAkXEASjCIOhoxOOEh0LhMARWCOzPRgixeIJRNJ5MKqiptLCXEklF6tC4AG8uFhlVhsdiMQAqGkAMioKNZNIA2gwALqKGDkApFPEACVoEGSfS5x1OvKxuCgM3uAyGWBYuRYDAlSpVas1OqZpwNhpYZqUluWtvtjt6SPBaIxmI9Xr6PuG/rYwc4AF8uHssABVGCQBLoJJYCi4WbNKazBswSnkCiODs+LDUKCGUqRfbI+qNHCpDKt+iUORZTKjXqpeTlw4us5ZajUe5ZLDkcQkEi0GBNVrtTrex5+0iGJCe+ijIi3vTUfD1wU7MucBlglEOWF4URMc0VgKQcUEUkACY6S4X9IxwAD2U5blXTAiDoNg8JpQgWUFRDZU1TArVdXTBwjSguMLStKQkwdXcQNRdDs2mK9fQLIMsJVVVsWIiMN3ImMqITa1cTteinVQnAmOkLMcE9Vjc2vAsWCLAisHKAB5B52IDBgUnSchSS7Dt6EY5j4mSKSZPArIW22FBxCKcQ0G3Hx1PKKBj1sF4d1uHBSWPS0HxfN80BkMUehQY9oUcbdnz8cQeneFB5MGIFuKI6QSIQwTKPNETaLE5Nd2s5j5JzHT8z0otS3CQ1WScqRvES2itDaE8ZAgagaRNLgHKatMUVRAAVHERqwT9SQmikxRIGl8M4bj5iWIoAH4sAAfU6YZkJwbEsPUkaNs2z9AKmrbZqpQCrtCLg6q4BqoSalr/Da89Oqgbrev656imdHkxuxCaLpm0UqQWzhFSWlVjq2s64Quzbbpu8G7vCZbPnWradsAjl9qw4sgA==
    // ReSharper restore CommentTypo

    [ProvidesContext]
    object? _rest;

    T? _first, _second, _third;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with no elements.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallList{T}"/> struct.
    /// Collects the enumerable; allocating the heaped list lazily.
    /// </summary>
    /// <param name="enumerable">The enumerable to collect.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList([InstantHandle] IEnumerable<T>? enumerable)
        : this(enumerable?.GetEnumerator()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallList{T}"/> struct.
    /// Mutates the enumerator; allocating the heaped list lazily.
    /// </summary>
    /// <param name="enumerator">The enumerator to mutate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(IEnumerator<T>? enumerator)
    {
        if (!enumerator?.MoveNext() ?? true)
            return;

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        _first = enumerator!.Current;

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

        List<T> list = [];

        do
            list.Add(enumerator.Current);
        while (enumerator.MoveNext());

        _rest = list;
    }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 1 element.</summary>
    /// <param name="first">The first element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(T first)
        : this(first, default, default, s_one) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 2 elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(T first, T second)
        : this(first, second, default, s_two) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with 3 elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(T first, T second, T third)
        : this(first, second, third, s_empty) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with arbitrary elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The rest of the elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(T first, T second, T third, IList<T> rest)
        : this(first, second, third, (object)rest) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct with arbitrary elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The rest of the elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList(T first, T second, T third, params T[] rest)
        : this(first, second, third, (object)rest) { }

    /// <summary>Initializes a new instance of the <see cref="SmallList{T}"/> struct. For internal use only.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The backing rest object, either a list or an object representing the length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    SmallList(T? first, T? second, T? third, object? rest)
    {
        _first = first;
        _second = second;
        _third = third;
        _rest = rest;
    }

    /// <summary>Gets the empty list.</summary>
    public static SmallList<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default;
    }

    /// <summary>Gets a value indicating whether determines whether the collection is empty.</summary>
    public readonly bool IsEmpty
    {
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _rest is null;
    }

    /// <inheritdoc />
    public readonly bool IsReadOnly
    {
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => false;
    }

    /// <inheritdoc cref="ICollection{T}.Count" />
    public readonly int Count
    {
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get =>
            _rest switch
            {
                null => 0,
                _ when _rest == s_one => 1,
                _ when _rest == s_two => 2,
                _ when _rest == s_empty => 3,
                _ => Rest!.Count + InlinedLength,
            };
    }

    /// <summary>Gets the number of head elements used.</summary>
    public readonly int HeadCount
    {
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Math.Min(Count, 3);
    }

    /// <summary>Gets the deep clone of this instance.</summary>
    public readonly SmallList<T> Cloned
    {
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            var clone = Uninit(Count);
            CopyTo(ref clone);
            return clone;
        }
    }

    /// <inheritdoc cref="IList{T}.this" />
    public T this[int index]
    {
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
        [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    public T First
    {
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] readonly get => this[0];
        [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[0] = value;
    }

    /// <summary>Gets or sets the second element.</summary>
    [CollectionAccess(Read), Pure]
    public T Second
    {
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] readonly get => this[1];
        [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[1] = value;
    }

    /// <summary>Gets or sets the third element.</summary>
    [CollectionAccess(Read), Pure]
    public T Third
    {
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure] readonly get => this[2];
        [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[2] = value;
    }
#pragma warning restore MA0102

    /// <summary>Gets the rest of the elements.</summary>
    public readonly IList<T>? Rest
    {
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), ProvidesContext, Pure]
        get => _rest as IList<T>;
    }

    /// <summary>Determines whether both sequence are equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both sequences are equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(SmallList<T> left, SmallList<T> right) => left.Equals(right);

    /// <summary>Determines whether both sequence are not equal.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both sequences are not equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(SmallList<T> left, SmallList<T> right) => !left.Equals(right);

    /// <summary>Creates the collection with 1 item in it.</summary>
    /// <param name="value">The single item to use.</param>
    /// <returns>The collection with 1 item.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SmallList<T>(T value) => new(value);

    /// <summary>Creates the collection with 2 items in it.</summary>
    /// <param name="tuple">The tuple containing 2 items to destructure and use.</param>
    /// <returns>The collection with 2 items.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SmallList<T>((T First, T Second) tuple) => new(tuple.First, tuple.Second);

    /// <summary>Creates the collection with 3 items in it.</summary>
    /// <param name="tuple">The tuple containing 3 items to destructure and use.</param>
    /// <returns>The collection with 3 items.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SmallList<T>((T First, T Second, T Third) tuple) =>
        new(tuple.First, tuple.Second, tuple.Third);

    /// <summary>Creates the collection with 3 or more items in it.</summary>
    /// <param name="tuple">The tuple containing 3 or more items to destructure and use.</param>
    /// <returns>The collection with 3 or more items.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SmallList<T>((T First, T Second, T Third, IList<T> List) tuple) =>
        new(tuple.First, tuple.Second, tuple.Third, tuple.List);

    /// <summary>Skips initialization of inlined elements.</summary>
    /// <param name="length">The length of the <see cref="SmallList{T}"/>.</param>
    /// <returns>The <see cref="SmallList{T}"/> of length <paramref name="length"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Uninit(int length)
    {
        Unsafe.SkipInit(out SmallList<T> output);
        RestFromLength(length, out output._rest);
        return output;
    }

    /// <summary>Skips initialization of unreachable inlined elements.</summary>
    /// <param name="length">The length of the <see cref="SmallList{T}"/>.</param>
    /// <returns>The <see cref="SmallList{T}"/> of length <paramref name="length"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> Zeroed(int length)
    {
        var output = Uninit(length);

        switch (length)
        {
            case >= 3:
                output._third = default!;
                goto case 2;
            case 2:
                output._second = default!;
                goto case 1;
            case 1:
                output._first = default!;
                break;
        }

        return output;
    }

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        switch (Count)
        {
            case 0:
                (_first, _rest) = (item, s_one);
                break;
            case 1:
                (_second, _rest) = (item, s_two);
                break;
            case 2:
                (_third, _rest) = (item, s_empty);
                break;
            default:
                EnsureMutability().Add(item);
                break;
        }
    }

    /// <summary>Adds the elements of the specified collection to the end of the <see cref="SmallList{T}"/>.</summary>
    /// <param name="collection">
    /// The collection whose elements should be added to the end of the <see cref="SmallList{T}"/>.
    /// </param>
    [CollectionAccess(UpdatedContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(IEnumerable<T>? collection)
    {
        if (collection?.ToCollectionLazily() is not { Count: var count and not 0 } c)
            return;

        if (InlinedLength - HeadCount is var stackExpand && stackExpand is not 0)
        {
            using var e = c.GetEnumerator();

            for (var i = 0; i < stackExpand; i++)
                if (e.MoveNext())
                    Add(e.Current);
                else
                    return;
        }

        if (count - stackExpand <= 0)
            return;

        var rest = _rest as List<T> ?? [.. Rest!];
        rest.AddRange(stackExpand is 0 ? c : c.Skip(stackExpand).ToCollectionLazily());
        _rest = rest;
    }

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _rest = null;

    /// <summary>Copies all values onto the destination.</summary>
    /// <param name="list">The destination.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="list"/> has less elements than itself.
    /// </exception>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(ref SmallList<T> list)
    {
        if (Count is var count && count is 0)
            return;

        list.BoundsCheck(count - 1, out _);

        // Takes advantage of fallthrough in switch-cases.
        switch (count)
        {
            case > InlinedLength:
                IList<T>
                    from = Rest!,
                    to = list.Rest!;

                for (var i = 0; i < from.Count; i++)
                    to[i] = from[i];

                goto case 3;
            case 3:
                list._third = _third!;
                goto case 2;
            case 2:
                list._second = _second!;
                goto case 1;
            case 1:
                list._first = _first!;
                break;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
    {
        // Takes advantage of fallthrough in switch-cases.
        switch (Count)
        {
            case > InlinedLength:
                Rest!.CopyTo(array, arrayIndex + InlinedLength);
                goto case 3;
            case 3:
                array[arrayIndex + 2] = _third!;
                goto case 2;
            case 2:
                array[arrayIndex + 1] = _second!;
                goto case 1;
            case 1:
                array[arrayIndex] = _first!;
                break;
        }
    }

    /// <summary>Deconstructs this instance with its properties.</summary>
    /// <param name="head">The first three elements.</param>
    /// <param name="tail">The remaining elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out (T? First, T? Second, T? Third) head, out IList<T> tail) =>
        (head, tail) = ((_first, _second, _third), Rest ?? s_empty);

    /// <summary>Deconstructs this instance with the 3 first elements.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out T? first, out T? second, out T? third) =>
        (first, second, third) = (_first, _second, _third);

    /// <summary>Deconstructs this instance with its properties.</summary>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second element.</param>
    /// <param name="third">The third element.</param>
    /// <param name="rest">The remaining elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out T? first, out T? second, out T? third, out IList<T> rest) =>
        (first, second, third, rest) = (_first, _second, _third, Rest ?? s_empty);
#if !UNMANAGED_SPAN
#pragma warning disable 8500
    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <param name="del">The function to use.</param>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void HeadSpan([InstantHandle, RequireStaticDelegate] SpanAction<T> del)
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount));
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount));
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void HeadSpan<TParam>(
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanAction<T, TParam> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void HeadSpan<TParam>(
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<T, TParam> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void HeadSpan<TParam>(
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<T, TParam> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif
#endif
#pragma warning restore 8500
    /// <inheritdoc />
    [CollectionAccess(UpdatedContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Contains(T item) =>
        Count switch
        {
            0 => Eq(_first, item),
            1 => Eq(_first, item) || Eq(_second, item),
            2 => Eq(_first, item) || Eq(_second, item) || Eq(_third, item),
            _ => Eq(_first, item) || Eq(_second, item) || Eq(_third, item) || Rest!.Contains(item),
        };

    /// <summary>Determines whether the item exists in the collection.</summary>
    /// <param name="item">The item to check.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The value determining whether the parameter <paramref name="item"/> exists in the collection.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Contains(T item, IEqualityComparer<T?> comparer) =>
        Count switch
        {
            0 => comparer.Equals(_first, item),
            1 => comparer.Equals(_first, item) || comparer.Equals(_second, item),
            2 => comparer.Equals(_first, item) || comparer.Equals(_second, item) || comparer.Equals(_third, item),
            _ => comparer.Equals(_first, item) ||
                comparer.Equals(_second, item) ||
                comparer.Equals(_third, item) ||
                Rest!.Contains(item, comparer),
        };

    /// <inheritdoc cref="object.Equals(object)"/>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SmallList<T> other && Equals(other);

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(SmallList<T> other) =>
        Count == other.Count &&
        Eq(_first, other._first) &&
        Eq(_second, other._second) &&
        Eq(_third, other._third) &&
        (other.Rest is [_, ..] rest ? Rest?.SequenceEqual(rest) ?? false : other.Rest is null);

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent), MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override int GetHashCode()
    {
        unchecked
        {
            const int Prime = 397;

            var hashCode = 0;

            switch (Count)
            {
                case > InlinedLength:
                    hashCode = _rest!.GetHashCode();
                    goto case 3;
                case 3:
                    hashCode = hashCode * Prime ^ EqualityComparer<T?>.Default.GetHashCode(_third!);
                    goto case 2;
                case 2:
                    hashCode = hashCode * Prime ^ EqualityComparer<T?>.Default.GetHashCode(_second!);
                    goto case 1;
                case 1:
                    hashCode = hashCode * Prime ^ EqualityComparer<T?>.Default.GetHashCode(_first!);
                    break;
            }

            return hashCode;
        }
    }

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
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
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() =>
        Count switch
        {
            0 => "[]",
            1 => $"[{_first}]",
            2 => $"[{_first}, {_second}]",
            3 => $"[{_first}, {_second}, {_third}]",
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            _ => $"[{_first}, {_second}, {_third}, {Rest!.Conjoin()}]",
#else
            _ => $"[{_first}, {_second}, {_third}, {Rest}]",
#endif
        };

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Enumerator GetEnumerator() => new(this);

    /// <summary>Gets the enumeration object that returns the values in reversed order.</summary>
    /// <returns>The backwards enumerator.</returns>
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Enumerator GetReversedEnumerator() => new(this, true);

    /// <summary>Forms a slice out of the current list that begins at a specified index.</summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <returns>
    /// A list that consists of all elements of the current list from <paramref name="start"/> to the end of the span.
    /// </returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#pragma warning disable IDE0057
    public readonly SmallList<T> Slice(int start) => Slice(start, Count - start);
#pragma warning restore IDE0057
    /// <summary>Forms a slice out of the current list starting at a specified index for a specified length.</summary>
    /// <param name="start">The index at which to begin this slice.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>
    /// A span that consists of <paramref name="length"/> elements from
    /// the current span starting at <paramref name="start"/>.
    /// </returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly SmallList<T> Slice(int start, int length)
    {
        var count = Count;
        start = Math.Max(start, 0);
        length = Math.Min(length, count - start);

        if (length <= 0)
            return default;

        if (start is 0 && length == count)
            return Cloned;

        Unsafe.SkipInit(out SmallList<T> output);

        if (length >= InlinedLength && Rest?.Skip(start).Take(length - InlinedLength).ToList() is { } list)
            output._rest = list;
        else
            RestFromLength(length, out output._rest);

        switch (length)
        {
            case >= 3:
                output._third = start is 0 ? _third : Rest![start - 1];
                goto case 2;
            case 2:
                output._second = start switch
                {
                    0 => _second,
                    1 => _third,
                    _ => Rest![start - 2],
                };

                goto case 1;
            case 1:
                output._first = start switch
                {
                    0 => _first,
                    1 => _second,
                    2 => _third,
                    _ => Rest![start - 3],
                };

                break;
        }

        return output;
    }
#pragma warning disable CS8500
#if !UNMANAGED_SPAN
    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="del">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="del"/>.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public unsafe TResult HeadSpan<TResult>([InstantHandle, RequireStaticDelegate] SpanFunc<T, TResult> del)
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            return del(MemoryMarshal.CreateSpan(ref _first!, HeadCount));
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount));
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="del"/>.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanFunc<T, TParam, TResult> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            return del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="del"/>.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<T, TParam, TResult> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            return del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif

    /// <summary>Creates the temporary span to be passed into the function.</summary>
    /// <typeparam name="TParam">The type of reference parameter to pass into the function.</typeparam>
    /// <typeparam name="TResult">The resulting type of the function.</typeparam>
    /// <param name="param">The reference parameter to pass into the function.</param>
    /// <param name="del">The function to use.</param>
    /// <returns>The result of the parameter <paramref name="del"/>.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public unsafe TResult HeadSpan<TParam, TResult>(
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<T, TParam, TResult> del
    )
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    {
        fixed (SmallList<T>* unused = &this)
            return del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
    }
#else
        =>
            del(MemoryMarshal.CreateSpan(ref _first!, HeadCount), param);
#endif
#endif
#pragma warning restore CS8500
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => !IsEmpty;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly byte IConvertible.ToByte(IFormatProvider? provider) => unchecked((byte)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly char IConvertible.ToChar(IFormatProvider? provider) => unchecked((char)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => new(Count, DateTimeKind.Utc);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => Count;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly double IConvertible.ToDouble(IFormatProvider? provider) => Count;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly short IConvertible.ToInt16(IFormatProvider? provider) => unchecked((short)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly int IConvertible.ToInt32(IFormatProvider? provider) => Count;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly long IConvertible.ToInt64(IFormatProvider? provider) => Count;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => unchecked((sbyte)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly float IConvertible.ToSingle(IFormatProvider? provider) => Count;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly string IConvertible.ToString(IFormatProvider? provider) => ToString();

    /// <inheritdoc />
    [DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider) =>
        throw new InvalidOperationException();

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => unchecked((ushort)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => unchecked((uint)Count);

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => unchecked((ulong)Count);
#endif

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void RestFromLength(int length, out object? rest)
    {
        if (length is 0 or 1 or 2 or 3)
            RestFromLengthWithoutAllocations(length, out rest);
        else
            rest = new T[length - InlinedLength];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void RestFromLengthWithoutAllocations(int length, out object? rest)
    {
        Unsafe.SkipInit(out rest);

        rest = length switch
        {
            <= 0 => null,
            1 => s_one,
            2 => s_two,
            3 => s_empty,
            _ => rest,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool Eq(T? x, T? y) => x is null ? y is null : y is not null && EqualityComparer<T>.Default.Equals(x, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void BoundsCheck(int index, [ValueRange(1, int.MaxValue)] out int count)
    {
        count = Count;

        if (unchecked((uint)index >= count))
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Must be between 0 and {count - 1}");
    }

    // ReSharper disable once UnusedParameter.Local
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    IList<T> EnsureMutability()
    {
        var rest = Rest switch
        {
            { IsReadOnly: false, Count: not 0 } x => x, // ReSharper disable once RedundantAssignment
            { Count: not 0 } x => [.. x],
            _ => (IList<T>)new List<T>(),
        };

        _rest = rest;
        return rest;
    }

    /// <summary>An enumerator over <see cref="SmallList{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IEnumerator<T>
    {
        readonly bool _isReversed;

        readonly int _count;

        readonly SmallList<T> _list;

        int _state = -1;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="list">The <see cref="SmallList{T}"/> to enumerate over.</param>
        /// <param name="isReversed">Determines whether to go backwards.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(SmallList<T> list, bool isReversed = false)
        {
            _list = list;
            _isReversed = isReversed;
            _count = list.Count;
        }

        /// <inheritdoc />
        public T Current { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; private set; } = default!;

        /// <inheritdoc />
        readonly object? IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Current;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _state = -1;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() =>
            ++_state < _count &&
            (Current = (_isReversed ? _count - _state - 1 : _state) switch
            {
                0 => _list._first!,
                1 => _list._second!,
                2 => _list._third!,
                var x => _list.Rest![x - InlinedLength],
            }) is var _;
    }
}
