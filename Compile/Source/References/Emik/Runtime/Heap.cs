// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for heap-allocation analysis.</summary>
static partial class Heap
{
    /// <summary>
    /// A <see langword="string"/> to use in an <see cref="ObsoleteAttribute"/> to indicate that the API isn't meant
    /// for production, but not for deprecated reasons.
    /// </summary>
    const string NotForProduction = "NOT deprecated. While this can be used in Release builds to run this on " +
        "optimized code; This API exists for debugging builds and should be excluded from final production builds.";

    /// <summary>Swallows all exceptions from a callback; Use with caution.</summary>
    /// <param name="action">The dangerous callback.</param>
    [Inline, Obsolete(NotForProduction)]
    public static void Swallow([InstantHandle] this Action action)
    {
        try
        {
            action();
        }
#pragma warning disable CA1031
        catch
#pragma warning restore CA1031
        {
            // ignored
        }
    }

    /// <summary>Swallows all exceptions from a callback; Use with caution.</summary>
    /// <typeparam name="T">The type of return.</typeparam>
    /// <param name="func">The dangerous callback.</param>
    /// <returns>The value returned from <paramref name="func"/>, or the exception caught.</returns>
    [Inline, Obsolete(NotForProduction)]
    public static (T?, Exception?) Swallow<T>([InstantHandle] this Func<T> func)
    {
        try
        {
            return (func(), null);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            return (default, ex);
        }
    }

    /// <summary>Gets the amount of bytes a callback uses.</summary>
    /// <remarks><para>
    /// This method temporarily tunes the <see cref="GC"/> to <see cref="GCLatencyMode.LowLatency"/>
    /// for accurate results. As such, the parameter <paramref name="heap"/> should not cause
    /// substantial allocation such that collecting mid-way is required.
    /// </para></remarks>
    /// <param name="heap">The callback that causes some amount of heap allocation.</param>
    /// <param name="willWarmup">Whether it should call the method once to initialize static/lazy-based values.</param>
    /// <returns>The number of bytes the <see cref="GC"/> allocated from calling <paramref name="heap"/>.</returns>
    [Inline, MustUseReturnValue, NonNegativeValue, Obsolete(NotForProduction)]
    public static long CountAllocation([InstantHandle, RequireStaticDelegate] Action heap, bool willWarmup = true)
    {
        if (willWarmup)
            heap.Swallow();
#if !(NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER)
        var mode = GCSettings.LatencyMode;
#endif
        try
        {
#if NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            GC.TryStartNoGCRegion(ushort.MaxValue, ushort.MaxValue);
#else
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
#endif
#if NETCOREAPP3_0_OR_GREATER
            var before = GC.GetTotalAllocatedBytes(true);
#else
            var before = GC.GetTotalMemory(true);
#endif
            heap.Swallow();
#if NETCOREAPP3_0_OR_GREATER
            var after = GC.GetTotalAllocatedBytes(true);
#else
            var after = GC.GetTotalMemory(false); // Prevents last-second garbage collection.
#endif

            return after - before;
        }
        finally
        {
#if NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            if (GCSettings.LatencyMode is GCLatencyMode.NoGCRegion)
                GC.EndNoGCRegion();
#else
            GCSettings.LatencyMode = mode;
#endif
        }
    }

    /// <summary>Gets multiple instances of the amount of bytes a callback uses.</summary>
    /// <param name="heap">The callback that causes some amount of heap allocation.</param>
    /// <param name="times">The amount of times to invoke <paramref name="heap"/>.</param>
    /// <param name="willWarmup">Whether it should call the method once to initialize static/lazy-based values.</param>
    /// <returns>
    /// An <see cref="Array"/> where each entry is a separate test of the number of
    /// bytes the <see cref="GC"/> allocated from calling <paramref name="heap"/>.
    /// </returns>
    [Inline, MustUseReturnValue, NonNegativeValue, Obsolete(NotForProduction)]
    public static long[] CountAllocations(
        [InstantHandle, RequireStaticDelegate] Action heap,
        [NonNegativeValue] int times = 256,
        bool willWarmup = true
    )
    {
        if (willWarmup)
            heap.Swallow();

        var all = new long[times];

        for (var i = 0; i < times; i++)
            all[i] += CountAllocation(heap, false);

        return all;
    }

    /// <summary>Gets multiple instances of the amount of bytes a callback uses.</summary>
    /// <param name="heap">The callback that causes some amount of heap allocation.</param>
    /// <param name="times">The amount of times to invoke <paramref name="heap"/>.</param>
    /// <param name="willWarmup">Whether it should call the method once to initialize static/lazy-based values.</param>
    /// <returns>
    /// An <see cref="Array"/> where each entry is a separate test of the number of
    /// bytes the <see cref="GC"/> allocated from calling <paramref name="heap"/>.
    /// </returns>
    [Inline, MustUseReturnValue, NonNegativeValue, Obsolete(NotForProduction)]
    public static bool HasAllocations(
        [InstantHandle, RequireStaticDelegate] Action heap,
        [NonNegativeValue] int times = 256,
        bool willWarmup = true
    )
    {
        if (willWarmup)
            heap.Swallow();

        for (var i = 0; i < times; i++)
            if (CountAllocation(heap, false) is not 0)
                return true;

        return false;
    }
}
