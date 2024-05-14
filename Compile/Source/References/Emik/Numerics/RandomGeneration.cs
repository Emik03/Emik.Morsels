// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to generate random numbers.</summary>
static partial class RandomGeneration
{
    /// <summary>Generates a random value of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the random value.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <returns>The random value.</returns>
    public static unsafe T Next<T>(this Random random)
        where T : unmanaged
    {
        T output = default;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        random.NextBytes(new Span<byte>(&output, sizeof(T)));
#else
        if (sizeof(T) >= sizeof(int))
            for (var i = 0; i < sizeof(T) / sizeof(int); i++)
                *((int*)&t + i) = random.Next(int.MinValue, int.MaxValue);

        if (sizeof(T) % sizeof(int) is 2 or 3)
            *((ushort*)((byte*)&t + sizeof(T)) - 1) = (ushort)random.Next(ushort.MinValue, ushort.MinValue);

        if (sizeof(T) % sizeof(int) is 1 or 3)
            *((byte*)&t + sizeof(T) - sizeof(T) % 4) = (byte)random.Next(byte.MinValue, byte.MinValue);
#endif
        return output;
    }
}
