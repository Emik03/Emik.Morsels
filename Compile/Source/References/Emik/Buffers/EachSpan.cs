// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Efficient LINQ-like methods for <see cref="ReadOnlySpan{T}"/> and siblings.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
static partial class EachSpan
{
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> BreakableFor<T>(
        this IMemoryOwner<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable.Memory.Span, func);
        return iterable;
    }

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> BreakableFor<T>(
        this in Memory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable.Span, func);
        return iterable;
    }
#endif

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> BreakableFor<T>(
        this scoped in Span<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable, func);
        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> BreakableFor<T>(
        this in ReadOnlyMemory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, ControlFlow> func
    )
    {
        BreakableFor(iterable.Span, func);
        return iterable;
    }
#endif

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> BreakableFor<T>(
        this scoped in ReadOnlySpan<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, ControlFlow> func
    )
    {
        foreach (var x in iterable)
            if (func(x) is ControlFlow.Break)
                break;

        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, int, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> BreakableFor<T>(
        this IMemoryOwner<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, int, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable.Memory.Span, func);
        return iterable;
    }

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, int, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> BreakableFor<T>(
        this in Memory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, int, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable.Span, func);
        return iterable;
    }
#endif

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, int, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> BreakableFor<T>(
        this scoped in Span<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, int, ControlFlow> func
    )
    {
        BreakableFor((ReadOnlySpan<T>)iterable, func);
        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, int, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> BreakableFor<T>(
        this in ReadOnlyMemory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, int, ControlFlow> func
    )
    {
        BreakableFor(iterable.Span, func);
        return iterable;
    }
#endif

    /// <inheritdoc cref="EachWithControlFlow.BreakableFor{T}(IEnumerable{T}, Func{T, int, ControlFlow})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> BreakableFor<T>(
        this scoped in ReadOnlySpan<T> iterable,
        [InstantHandle, RequireStaticDelegate] Func<T, int, ControlFlow> func
    )
    {
        for (var i = 0; i < iterable.Length; i++)
            if (func(iterable[i], i) is ControlFlow.Break)
                break;

        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> For<T>(
        this IMemoryOwner<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T> action
    )
    {
        For((ReadOnlySpan<T>)iterable.Memory.Span, action);
        return iterable;
    }

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> For<T>(this in Memory<T> iterable, [InstantHandle, RequireStaticDelegate] Action<T> action)
    {
        For((ReadOnlySpan<T>)iterable.Span, action);
        return iterable;
    }
#endif

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> For<T>(
        this scoped in Span<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T> action
    )
    {
        For((ReadOnlySpan<T>)iterable, action);
        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> For<T>(
        this in ReadOnlyMemory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T> action
    )
    {
        For(iterable.Span, action);
        return iterable;
    }
#endif

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> For<T>(
        this scoped in ReadOnlySpan<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T> action
    )
    {
        foreach (var x in iterable)
            action(x);

        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T, int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> For<T>(
        this IMemoryOwner<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T, int> action
    )
    {
        For((ReadOnlySpan<T>)iterable.Memory.Span, action);
        return iterable;
    }

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T, int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> For<T>(
        this in Memory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T, int> action
    )
    {
        For((ReadOnlySpan<T>)iterable.Span, action);
        return iterable;
    }
#endif

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T, int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> For<T>(
        this scoped in Span<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T, int> action
    )
    {
        For((ReadOnlySpan<T>)iterable, action);
        return iterable;
    }
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T, int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> For<T>(
        this in ReadOnlyMemory<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T, int> action
    )
    {
        For(iterable.Span, action);
        return iterable;
    }
#endif

    /// <inheritdoc cref="Each.For{T}(IEnumerable{T}, Action{T, int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> For<T>(
        this scoped in ReadOnlySpan<T> iterable,
        [InstantHandle, RequireStaticDelegate] Action<T, int> action
    )
    {
        for (var i = 0; i < iterable.Length; i++)
            action(iterable[i], i);

        return iterable;
    }
}
