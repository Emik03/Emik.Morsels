// <copyright file="Heap.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Provides methods for heap-allocation analysis.</summary>
static class Heap
{
    /// <summary>
    /// A <see langword="string"/> to use in an <see cref="ObsoleteAttribute"/> to indicate that the API isn't meant
    /// for production, but not for deprecated reasons.
    /// </summary>
    const string NotForProduction = "NOT deprecated. While this can be used in Release builds to run this on " +
        "optimized code; This API exists for internal builds and should be excluded from final production builds.";

    /// <summary>Swallows all exceptions from a callback; Use with caution.</summary>
    /// <param name="action">The dangerous callback.</param>
    [Inline, Obsolete(NotForProduction)]
    internal static void Swallow([InstantHandle] this Action action)
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
    internal static long CountAllocation([InstantHandle, RequireStaticDelegate] Action heap, bool willWarmup = true)
    {
        if (willWarmup)
            heap.Swallow();

        var mode = GCSettings.LatencyMode;

        try
        {
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;

            var before = GC.GetTotalMemory(true);

            heap.Swallow();

            var after = GC.GetTotalMemory(false);

            return after - before;
        }
        finally
        {
            GCSettings.LatencyMode = mode;
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
    internal static long[] CountAllocations(
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
}
