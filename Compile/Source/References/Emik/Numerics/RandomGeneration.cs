// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
/// <summary>Extension methods to generate random numbers.</summary>
static partial class RandomGeneration
{
    /// <summary>Generates a random value of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the random value.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <returns>The random value.</returns>
    // ReSharper disable once RedundantNameQualifier
    public static T Next<T>(this System.Random random)
        where T : struct
    {
        Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
        random.NextBytes(bytes);
        return MemoryMarshal.Read<T>(bytes);
    }
}
#endif
