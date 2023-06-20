// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the method to skip initialization.</summary>
static partial class Skip
{
    /// <summary>Skips initialization based on framework.</summary>
    /// <typeparam name="T">The type of value to skip initialization.</typeparam>
    /// <param name="ret">The value to skip initialization.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Init<T>(out T ret) =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        Unsafe.SkipInit(out ret);
#else
        ret = default;
#endif
}
