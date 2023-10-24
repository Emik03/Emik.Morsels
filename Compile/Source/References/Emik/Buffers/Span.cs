// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Defines methods for callbacks with spans. Methods here do not clear the allocated buffer.</summary>
/// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
static partial class Span
{
    /// <summary>A callback for a span.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <param name="span">The allocated span.</param>
    public delegate void SpanAction<TSpan>(scoped Span<TSpan> span);

    /// <summary>A callback for a span with a reference parameter.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The parameter.</param>
    public delegate void SpanAction<TSpan, in TParam>(scoped Span<TSpan> span, TParam param);

    /// <summary>A callback for a span with a reference parameter that is also a span, but immutable.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    public delegate void SpanActionReadOnlySpan<TSpan, TParam>(scoped Span<TSpan> span, ReadOnlySpan<TParam> param);

    /// <summary>A callback for a span with a reference parameter that is also a span.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the span parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    public delegate void SpanActionSpan<TSpan, TParam>(scoped Span<TSpan> span, Span<TParam> param);

    /// <summary>A callback for a span with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFunc<TSpan, out TResult>(scoped Span<TSpan> span);

    /// <summary>A callback for a span with a reference parameter with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFunc<TSpan, in TParam, out TResult>(scoped Span<TSpan> span, TParam param);

    /// <summary>A callback for a span with a reference parameter that is also a span, with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFuncReadOnlySpan<TSpan, TParam, out TResult>(
        scoped Span<TSpan> span,
        ReadOnlySpan<TParam> param
    );

    /// <summary>
    /// A callback for a span with a reference parameter that is also a span, but immutable, with a return value.
    /// </summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFuncSpan<TSpan, TParam, out TResult>(scoped Span<TSpan> span, Span<TParam> param);

    /// <summary>The maximum size for the number of bytes a stack allocation will occur in this class.</summary>
    /// <remarks><para>
    /// Stack allocating arrays is an incredibly powerful tool that gets rid of a lot of the overhead that comes from
    /// instantiating arrays normally. Notably, that all classes (such as <see cref="Array"/> or <see cref="List{T}"/>)
    /// are heap allocated, and moreover are garbage collected. This can put a strain in methods that are called often.
    /// </para><para>
    /// However, there isn't as much stack memory available as there is heap, which can cause a DoS (Denial of Service)
    /// vulnerability if you aren't careful. The methods in <c>Span</c> will automatically switch to unmanaged heap
    /// allocation if the type argument and length create an array that exceeds 1kB (1024 bytes).
    /// </para></remarks>
    public const int StackallocSize = 1 << 10;
#if !NETSTANDARD1_0
    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Allocate(
        [NonNegativeValue] int length,
        [InstantHandle, RequireStaticDelegate] SpanAction<byte> del
    ) =>
        Allocate<byte>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan>(
        int length,
        [InstantHandle, RequireStaticDelegate] SpanAction<TSpan> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value]);
            return;
        }

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            del(new((void*)ptr, value));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Allocate<TParam>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanAction<byte, TParam> del
    ) =>
        Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan, TParam>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanAction<TSpan, TParam> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value], param);
            return;
        }

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Allocate<TParam>(
        int length,
        scoped ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<byte, TParam> del
    )
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
        =>
            Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan, TParam>(
        int length,
        scoped ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<TSpan, TParam> del
    )
        where TSpan : unmanaged
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value], param);
            return;
        }

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Allocate<TParam>(
        int length,
        scoped Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<byte, TParam> del
    )
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
        =>
            Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan, TParam>(
        int length,
        scoped Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<TSpan, TParam> del
    )
        where TSpan : unmanaged
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value], param);
            return;
        }

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
#endif

    /// <summary>Determines if a given length and type should be stack-allocated.</summary>
    /// <remarks><para>
    /// See <see cref="StackallocSize"/> for details about stack- and heap-allocation.
    /// </para></remarks>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsStack<T>([NonNegativeValue] int length) => InBytes<T>(length) <= StackallocSize;

    /// <summary>Gets the byte length needed to allocate the current length, used in <see cref="IsStack{T}"/>.</summary>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="length">The amount of items.</param>
    /// <returns>
    /// The value <see langword="true"/>, if it should be stack-allocated, otherwise <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] // ReSharper disable once RedundantUnsafeContext
    public static unsafe int InBytes<T>([NonNegativeValue] int length) =>
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
        length * Unsafe.SizeOf<T>();
#else
#pragma warning disable 8500
        length * sizeof(T);
#pragma warning restore 8500
#endif // ReSharper disable RedundantUnsafeContext
#pragma warning disable 9091 // InlineAttribute makes this okay.
#pragma warning disable RCS1242 // Normally causes defensive copies; Parameter is unused though.
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <summary>Allocates an inlined span of the specified size.</summary>
    /// <remarks><para>
    /// The returned <see cref="Span{T}"/> will point to uninitialized memory.
    /// Be sure to call <see cref="Span{T}.Fill"/> or otherwise written to first before enumeration or reading.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="_">The discard, which is used to let the compiler track lifetimes.</param>
    /// <returns>The <see cref="Span{T}"/> of the specified size.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL // ReSharper disable once NullableWarningSuppressionIsUsed
    public static Span<T> Inline1<T>(in T _ = default!)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Ref(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline1<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out T x);
        return Ref(ref x);
    }
#endif
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline2<T>(in Two<T> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<T>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline2<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<T> x);
        return PooledSmallList<T>.Validate<Two<T>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline4<T>(in Two<Two<T>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<T>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline4<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<T>> x);
        return PooledSmallList<T>.Validate<Two<Two<T>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline8<T>(in Two<Two<Two<T>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<T>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline8<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<T>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<T>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline16<T>(in Two<Two<Two<Two<T>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<T>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline16<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<T>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<T>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline32<T>(in Two<Two<Two<Two<Two<T>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<T>>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline32<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<T>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<T>>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline64<T>(in Two<Two<Two<Two<Two<Two<T>>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<T>>>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline64<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<T>>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<T>>>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline128<T>(in Two<Two<Two<Two<Two<Two<Two<T>>>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline128<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<T>>>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline256<T>(in Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline256<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline512<T>(in Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>.AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline512<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>.AsSpan(ref x);
    }
#endif

    /// <inheritdoc cref="Inline1{T}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DEBUG || CSHARPREPL
    public static Span<T> Inline1024<T>(in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> _ = default)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            PooledSmallList<T>
               .Validate<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>
               .AsSpan(ref Unsafe.AsRef(_));
#else
    public static unsafe Span<T> Inline1024<T>(in bool _ = false)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Unsafe.SkipInit(out Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> x);
        return PooledSmallList<T>.Validate<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>.AsSpan(ref x);
    }
#endif
#endif
#pragma warning restore RCS1242
    /// <summary>Creates a new <see cref="Span{T}"/> of length 1 around the specified reference.</summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Ref<T>(ref T reference) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        new(ref reference);
#else
        MemoryMarshal.CreateSpan(ref reference, 1);
#endif
#if NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP
    /// <summary>Creates a new <see cref="ReadOnlySpan{T}"/> of length 1 around the specified reference.</summary>
    /// <typeparam name="T">The type of <paramref name="reference"/>.</typeparam>
    /// <param name="reference">A reference to data.</param>
    /// <returns>The created span over the parameter <paramref name="reference"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> In<T>(in T reference) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        new(Unsafe.AsRef(reference));
#else
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(reference), 1);
#endif
#endif
#if !NETSTANDARD1_0
    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public static TResult Allocate<TResult>(
        int length,
        [InstantHandle, RequireStaticDelegate] SpanFunc<byte, TResult> del
    ) =>
        Allocate<byte, TResult>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TSpan, TResult>(
        int length,
        [InstantHandle, RequireStaticDelegate] SpanFunc<TSpan, TResult> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value]);

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            return del(new((void*)ptr, value));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanFunc<byte, TParam, TResult> del
    ) =>
        Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TSpan, TParam, TResult>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanFunc<TSpan, TParam, TResult> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value], param);

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            return del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        scoped ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<byte, TParam, TResult> del
    )
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
        =>
            Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TSpan, TParam, TResult>(
        int length,
        scoped ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<TSpan, TParam, TResult> del
    )
        where TSpan : unmanaged
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value], param);

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            return del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        scoped Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<byte, TParam, TResult> del
    )
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
        =>
            Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="StackallocSize"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static unsafe TResult Allocate<TSpan, TParam, TResult>(
        int length,
        scoped Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<TSpan, TParam, TResult> del
    )
        where TSpan : unmanaged
#if UNMANAGED_SPAN
        where TParam : unmanaged
#endif
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value], param);

        var ptr = Marshal.AllocHGlobal(value);

        try
        {
            return del(new((void*)ptr, value), param);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
#endif
}
