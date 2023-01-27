// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels;

/// <summary>Defines methods for callbacks with fat pointers. Methods here do not clear the allocated buffer.</summary>
/// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
static partial class FatPointer
{
    /// <summary>A callback for a fat pointer.</summary>
    /// <typeparam name="TFat">The inner type of the fat pointer.</typeparam>
    /// <param name="fat">The allocated buffer.</param>
    public delegate void FatPointerAction<TFat>(FatPointer<TFat> fat)
        where TFat : unmanaged;

    /// <summary>A callback for a fat pointer with a reference parameter.</summary>
    /// <typeparam name="TFat">The inner type of the fat pointer.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="fat">The allocated buffer.</param>
    /// <param name="param">The parameter.</param>
    public delegate void FatPointerAction<TFat, in TParam>(FatPointer<TFat> fat, TParam param)
        where TFat : unmanaged;

    /// <summary>A callback for a fat pointer with a return value.</summary>
    /// <typeparam name="TFat">The inner type of the fat pointer.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="fat">The allocated buffer.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult FatPointerFunc<TFat, out TResult>(FatPointer<TFat> fat)
        where TFat : unmanaged;

    /// <summary>A callback for a fat pointer with a reference parameter with a return value.</summary>
    /// <typeparam name="TFat">The inner type of the fat pointer.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="fat">The allocated buffer.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult FatPointerFunc<TFat, in TParam, out TResult>(FatPointer<TFat> fat, TParam param)
        where TFat : unmanaged;

    /// <summary>The maximum size for the number of bytes a stack allocation will occur in this class.</summary>
    /// <remarks><para>
    /// Stack allocating arrays is an incredibly powerful tool that gets rid of a lot of the overhead that comes from
    /// instantiating arrays normally. Notably, that all classes (such as <see cref="Array"/> or <see cref="List{T}"/>)
    /// are heap allocated, and moreover are garbage collected. This can put a strain in methods that are called often.
    /// </para><para>
    /// However, there isn't as much stack memory available as there is heap, which can cause a DoS (Denial of Service)
    /// vulnerability if you aren't careful. The methods in <c>FatPointer</c> will automatically switch to unmanaged
    /// heap allocation if the type argument and length create an array that exceeds 1kB (1024 bytes).
    /// </para></remarks>
    public const int Stackalloc = 1 << 10;

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate(
        [NonNegativeValue] int length,
        [InstantHandle, RequireStaticDelegate] FatPointerAction<byte> del
    ) =>
        Allocate<byte>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TFat">The type of parameter in the fat pointer.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TFat>(
        int length,
        [InstantHandle, RequireStaticDelegate] FatPointerAction<TFat> del
    )
        where TFat : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TFat>(length))
        {
            var ptr = stackalloc TFat[value];
            del(new(ptr, value));
            return;
        }

        var array = Marshal.AllocHGlobal(value);
        FatPointer<TFat> fat = new((TFat*)array, value);
        del(fat);

        Marshal.FreeHGlobal(array);
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate<TParam>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] FatPointerAction<byte, TParam> del
    ) =>
        Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TFat">The type of parameter in the fat pointer.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TFat, TParam>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] FatPointerAction<TFat, TParam> del
    )
        where TFat : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TFat>(length))
        {
            var ptr = stackalloc TFat[value];
            del(new(ptr, value), param);
            return;
        }

        var array = Marshal.AllocHGlobal(value);
        FatPointer<TFat> fat = new((TFat*)array, value);
        del(fat, param);

        Marshal.FreeHGlobal(array);
    }

    /// <summary>Determines if a given length and type should be stack-allocated.</summary>
    /// <remarks><para>
    /// See <see cref="Stackalloc"/> for details about stack- and heap-allocation.
    /// </para></remarks>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool IsStack<T>(int length)
        where T : unmanaged =>
        InBytes<T>(length) <= Stackalloc;

    /// <summary>Gets the byte length needed to allocate the current length, used in <see cref="IsStack{T}"/>.</summary>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
    [NonNegativeValue, Pure]
    public static unsafe int InBytes<T>(int length)
        where T : unmanaged =>
        length * sizeof(T);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static TResult Allocate<TResult>(
        int length,
        [InstantHandle, RequireStaticDelegate] FatPointerFunc<byte, TResult> del
    ) =>
        Allocate<byte, TResult>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TFat">The type of parameter in the fat pointer.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TFat, TResult>(
        int length,
        [InstantHandle, RequireStaticDelegate] FatPointerFunc<TFat, TResult> del
    )
        where TFat : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TFat>(length))
        {
            var ptr = stackalloc TFat[value];
            return del(new(ptr, value));
        }

        var array = Marshal.AllocHGlobal(value);
        FatPointer<TFat> fat = new((TFat*)array, value);
        var result = del(fat);

        Marshal.FreeHGlobal(array);

        return result;
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] FatPointerFunc<byte, TParam, TResult> del
    ) =>
        Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="FatPointer{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TFat">The type of parameter in the fat pointer.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TFat, TParam, TResult>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] FatPointerFunc<TFat, TParam, TResult> del
    )
        where TFat : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TFat>(length))
        {
            var ptr = stackalloc TFat[value];
            return del(new(ptr, value), param);
        }

        var array = Marshal.AllocHGlobal(value);
        FatPointer<TFat> fat = new((TFat*)array, value);
        var result = del(fat, param);

        Marshal.FreeHGlobal(array);

        return result;
    }
}

/// <summary>Represents a continuous buffer of arbitrary memory.</summary>
/// <remarks><para>This type delegates the responsibility of pinning the pointer to the consumer.</para></remarks>
/// <typeparam name="T">The type of pointer.</typeparam>
[DebuggerTypeProxy(typeof(FatPointer<>.FatPointerDebugView)), DebuggerDisplay("{ToString(),raw}"),
 StructLayout(LayoutKind.Auto)]
readonly unsafe struct FatPointer<T> : IEquatable<FatPointer<T>>, IReadOnlyList<T>
    where T : unmanaged
{
    /// <summary>Initializes a new instance of the <see cref="FatPointer{T}"/> struct.</summary>
    /// <param name="reference">The pointer to the first element of the buffer.</param>
    /// <param name="length">The length of the buffer.</param>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FatPointer(T* reference, int length)
    {
        Reference = reference;
        Length = Math.Max(length, 0);
    }

    /// <summary>Initializes a new instance of the <see cref="FatPointer{T}"/> struct.</summary>
    /// <param name="reference">The pointer to the first element of the buffer.</param>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FatPointer(T* reference)
        : this(reference, 1) { }

    /// <summary>Performs the index operation.</summary>
    /// <param name="index">The index to apply to.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="index"/> is less than 0 or is equal or greater the <see cref="Length"/>.
    /// </exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ValidateIndex(index);
            return Reference[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            ValidateIndex(index);
            Reference[index] = value;
        }
    }

    /// <summary>Gets the empty buffer, which is also the default value.</summary>
#pragma warning disable CA1000
    public static FatPointer<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default;
    }
#pragma warning restore CA1000

    /// <summary>Gets a value indicating whether the buffer is empty.</summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Length is 0;
    }

    /// <summary>Gets the length of the buffer.</summary>
    public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>Gets the pointer representing the first element in the buffer.</summary>
    [CLSCompliant(false)]
    public T* Reference { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc />
    int IReadOnlyCollection<T>.Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Length;
    }

    /// <summary>Determines equality. Pointers are only compared when non-empty.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> when both pointers represent the same value, otherwise <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FatPointer<T> left, FatPointer<T> right) =>
        left.IsEmpty ? right.IsEmpty : left.Length == right.Length && left.Reference == right.Reference;

    /// <summary>Determines inequality. Pointers are only compared when non-empty.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="false"/> when both pointers represent the same value, otherwise <see langword="true"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FatPointer<T> left, FatPointer<T> right) => !(left == right);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is FatPointer<T> fat && Equals(fat);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => (int)Reference + Length;

    /// <summary>Gets an enumeration of this buffer.</summary>
    /// <returns>An enumerator of this buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Sets all values to their default values.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Fill(default);

    /// <summary>Sets all values to a specified value.</summary>
    /// <param name="value">The value to set to all values.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(T value)
    {
        for (var i = 0; i < Length; i++)
            Reference[i] = value;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FatPointer<T> other) => this == other;

    /// <summary>Copies contents to the destination.</summary>
    /// <param name="destination">The destination to copy to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(FatPointer<T> destination)
    {
        ValidateDestination(destination.Length);

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];
    }

    /// <summary>Copies contents to the destination.</summary>
    /// <param name="destination">The destination to copy to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(IList<T> destination)
    {
        ValidateDestination(destination.Count);

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];
    }

    /// <summary>Attempts to copy contents to the destination.</summary>
    /// <param name="destination">The destination to attempt to copy to.</param>
    /// <returns>The value determining whether the operation succeeded.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(FatPointer<T> destination)
    {
        if ((uint)Length > (uint)destination.Length)
            return false;

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];

        return true;
    }

    /// <summary>Attempts to copy contents to the destination.</summary>
    /// <param name="destination">The destination to attempt to copy to.</param>
    /// <returns>The value determining whether the operation succeeded.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(IList<T> destination)
    {
        if ((uint)Length > (uint)destination.Count)
            return false;

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];

        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        typeof(T) == typeof(char)
            ? ToCharArray().Conjoin()
            : $"Emik.Morsels.FatPointer<{typeof(T).Name}>[{Length}]";

    /// <summary>Creates the slice of this buffer.</summary>
    /// <param name="start">The start of the slice from this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">An out-of-range buffer is created.</exception>
    /// <returns>The <see cref="FatPointer{T}"/> which is a slice of this buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FatPointer<T> Slice(int start) =>
        (uint)start > (uint)Length
            ? throw new ArgumentOutOfRangeException(nameof(start))
            : new(Reference + start, Length - start);

    /// <summary>Creates the slice of this buffer.</summary>
    /// <param name="start">The start of the slice from this buffer.</param>
    /// <param name="length">The length of the slice from this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">An out-of-range buffer is created.</exception>
    /// <returns>The <see cref="FatPointer{T}"/> which is a slice of this buffer.</returns>
#pragma warning disable CA2208, MA0015
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FatPointer<T> Slice(int start, int length) =>
        (ulong)(uint)start + (uint)length > (uint)Length
            ? throw new ArgumentOutOfRangeException()
            : new(Reference + start, length);
#pragma warning restore CA2208, MA0015

    /// <summary>Creates a managed collection from this instance.</summary>
    /// <returns>The list that has values from this buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IList<T> ToIList()
    {
        if (IsEmpty)
#if NETFRAMEWORK && NET46_OR_GREATER || NETSTANDARD && NETSTANDARD1_3_OR_GREATER || NETCOREAPP
            return Array.Empty<T>();
#else
            return new T[0];
#endif

        var destination = new T[Length];
        CopyTo(destination);
        return destination;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)Length)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"must be non-zero and below length {Length}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateDestination(int destination)
    {
        if ((uint)Length > (uint)destination)
            throw new ArgumentException(
                $"Destination length \"{destination}\" shorter than source \"{Length}\".",
                nameof(destination)
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IList<char> ToCharArray() => new FatPointer<char>((char*)Reference, Length).ToIList();

    /// <summary>Enumerates the elements of a <see cref="FatPointer{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
#pragma warning disable CA1034
    public struct Enumerator : IEnumerator<T>
#pragma warning restore CA1034
    {
        readonly FatPointer<T> _fat;

        int _index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="fat">The buffer to peek through.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(FatPointer<T> fat)
        {
            _fat = fat;
            _index = -1;
        }

        /// <inheritdoc />
        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Current;
        }

        /// <inheritdoc />
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _fat[_index];
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;

            if (index >= _fat.Length)
                return false;

            _index = index;
            return true;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose() { }
    }

    sealed class FatPointerDebugView
    {
        public FatPointerDebugView(FatPointer<T> fat) => Items = fat.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden), UsedImplicitly]
        public T[] Items { get; }
    }
}
