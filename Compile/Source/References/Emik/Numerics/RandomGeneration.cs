// SPDX-License-Identifier: MPL-2.0

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
    public static unsafe T Next<T>(this Func<int, int, int> random)
        where T : unmanaged
    {
        T output = default;

        if (sizeof(T) >= sizeof(int))
            for (var i = 0; i < sizeof(T) / sizeof(int); i++)
                *((int*)&output + i) = random(int.MinValue, int.MaxValue);

        if (sizeof(T) % sizeof(int) is 2 or 3)
            *((ushort*)((byte*)&output + sizeof(T)) - 1) = (ushort)random(ushort.MinValue, ushort.MinValue);

        if (sizeof(T) % sizeof(int) is 1 or 3)
            *((byte*)&output + sizeof(T) - sizeof(T) % 4) = (byte)random(byte.MinValue, byte.MinValue);

        return output;
    }

    /// <summary>Generates a random value of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the random value.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <returns>The random value.</returns>
    public static T Next<T>(this Random random)
        where T : unmanaged
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        T output = default;
        random.NextBytes(MemoryMarshal.Cast<T, byte>(Ref(ref output)));
        return output;
#else
        return Next<T>(random.Next);
#endif
    }
}
