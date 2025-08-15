// SPDX-License-Identifier: MPL-2.0
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Extension methods to generate random numbers.</summary>
static partial class RandomGeneration
{
    /// <summary>Generates a random value of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the random value.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <returns>The random value.</returns>
    public static T Next<T>(this System.Random random)
        where T : unmanaged
    {
        T output = default;
        random.NextBytes(MemoryMarshal.Cast<T, byte>(Ref(ref output)));
        return output;
    }
}
#endif
