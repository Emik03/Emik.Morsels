// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Defines methods for callbacks with spans. Methods here do not clear the allocated buffer.</summary>
/// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
static partial class Span
{
    /// <summary>A callback for a span.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <param name="span">The allocated span.</param>
    public delegate void SpanAction<TSpan>(in Span<TSpan> span)
        where TSpan : unmanaged;

    /// <summary>A callback for a span with a reference parameter.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The parameter.</param>
    public delegate void SpanAction<TSpan, in TParam>(in Span<TSpan> span, TParam param)
        where TSpan : unmanaged;

    /// <summary>A callback for a span with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFunc<TSpan, out TResult>(in Span<TSpan> span)
        where TSpan : unmanaged;

    /// <summary>A callback for a span with a reference parameter with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFunc<TSpan, in TParam, out TResult>(in Span<TSpan> span, TParam param)
        where TSpan : unmanaged;

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
    public const int Stackalloc = 1 << 10;

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate(
        [NonNegativeValue] int length,
        [InstantHandle, RequireStaticDelegate] SpanAction<byte> del
    ) =>
        Allocate<byte>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        del(span);

        Marshal.FreeHGlobal(array);
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate<TParam>(
        int length,
        TParam param,
        [InstantHandle, RequireStaticDelegate] SpanAction<byte, TParam> del
    ) =>
        Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        del(span, param);

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

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static TResult Allocate<TResult>(
        int length,
        [InstantHandle, RequireStaticDelegate] SpanFunc<byte, TResult> del
    ) =>
        Allocate<byte, TResult>(length, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        var result = del(span);

        Marshal.FreeHGlobal(array);

        return result;
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
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
        [InstantHandle, RequireStaticDelegate] SpanFunc<byte, TParam, TResult> del
    ) =>
        Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        var result = del(span, param);

        Marshal.FreeHGlobal(array);

        return result;
    }
#if !NETFRAMEWORK && !NETSTANDARD || NETSTANDARD2_1_OR_GREATER
    /// <summary>A callback for a span with a reference parameter that is also a span, but immutable.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    public delegate void SpanActionReadOnlySpan<TSpan, TParam>(in Span<TSpan> span, in ReadOnlySpan<TParam> param)
        where TSpan : unmanaged;

    /// <summary>A callback for a span with a reference parameter that is also a span.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the span parameter.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    public delegate void SpanActionSpan<TSpan, TParam>(in Span<TSpan> span, in Span<TParam> param)
        where TSpan : unmanaged;

    /// <summary>A callback for a span with a reference parameter that is also a span, with a return value.</summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFuncReadOnlySpan<TSpan, TParam, out TResult>(
        in Span<TSpan> span,
        in ReadOnlySpan<TParam> param
    )
        where TSpan : unmanaged;

    /// <summary>
    /// A callback for a span with a reference parameter that is also a span, but immutable, with a return value.
    /// </summary>
    /// <typeparam name="TSpan">The inner type of the span.</typeparam>
    /// <typeparam name="TParam">The inner type of the immutable span parameter.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="span">The allocated span.</param>
    /// <param name="param">The span parameter.</param>
    /// <returns>The returned value of this delegate.</returns>
    public delegate TResult SpanFuncSpan<TSpan, TParam, out TResult>(in Span<TSpan> span, in Span<TParam> param)
        where TSpan : unmanaged;
#endif
#if !NETFRAMEWORK && !NETSTANDARD || NETSTANDARD2_1_OR_GREATER
    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate<TParam>(
        int length,
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<byte, TParam> del
    ) =>
        Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan, TParam>(
        int length,
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionReadOnlySpan<TSpan, TParam> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value], param);
            return;
        }

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        del(span, param);

        Marshal.FreeHGlobal(array);
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static void Allocate<TParam>(
        int length,
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<byte, TParam> del
    ) =>
        Allocate<byte, TParam>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TSpan">The type of parameter in the span.</typeparam>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    public static unsafe void Allocate<TSpan, TParam>(
        int length,
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanActionSpan<TSpan, TParam> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
        {
            del(stackalloc TSpan[value], param);
            return;
        }

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        del(span, param);

        Marshal.FreeHGlobal(array);
    }
#endif
#if !NETFRAMEWORK && !NETSTANDARD || NETSTANDARD2_1_OR_GREATER
    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<byte, TParam, TResult> del
    ) =>
        Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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
        ReadOnlySpan<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncReadOnlySpan<TSpan, TParam, TResult> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value], param);

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        var result = del(span, param);

        Marshal.FreeHGlobal(array);

        return result;
    }

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
    /// <typeparam name="TParam">The type of the parameter within the span.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="length">The length of the buffer.</param>
    /// <param name="param">The parameter to pass in.</param>
    /// <param name="del">The callback to invoke.</param>
    /// <returns>The returned value from invoking <paramref name="del"/>.</returns>
    [MustUseReturnValue]
    public static TResult Allocate<TParam, TResult>(
        int length,
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<byte, TParam, TResult> del
    ) =>
        Allocate<byte, TParam, TResult>(length, param, del);

    /// <summary>Allocates memory and calls the callback, passing in the <see cref="Span{T}"/>.</summary>
    /// <remarks><para>See <see cref="Stackalloc"/> for details about stack- and heap-allocation.</para></remarks>
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
        Span<TParam> param,
        [InstantHandle, RequireStaticDelegate] SpanFuncSpan<TSpan, TParam, TResult> del
    )
        where TSpan : unmanaged
    {
        var value = Math.Max(length, 0);

        if (IsStack<TSpan>(length))
            return del(stackalloc TSpan[value], param);

        var array = Marshal.AllocHGlobal(value);
        Span<TSpan> span = new((void*)array, value);
        var result = del(span, param);

        Marshal.FreeHGlobal(array);

        return result;
    }
#endif
}
