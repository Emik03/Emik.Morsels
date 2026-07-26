// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NET5_0_OR_GREATER
/// <summary>Xoshiro256** (xor, shift, rotate) pseudo-random number generator (PRNG).</summary>
/// <remarks><para>
/// Implementation based on
/// <a href="https://github.com/colgreen/Redzen/blob/main/Redzen/Random/Xoshiro256StarStarRandom.cs">Redzen's Xoshiro256StarStarRandom</a>.
/// </para></remarks>
sealed class XoshiroRandom(ulong seed) : Random
{
    ulong _s0 = Next(ref seed), _s1 = Next(ref seed), _s2 = Next(ref seed), _s3 = Next(ref seed);

    /// <summary>Sets the values to the next state.</summary>
    /// <param name="s0">The first state.</param>
    /// <param name="s1">The second state.</param>
    /// <param name="s2">The third state.</param>
    /// <param name="s3">The fourth state.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Next(ref ulong s0, ref ulong s1, ref ulong s2, ref ulong s3)
    {
        var t = s1 << 17;
        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;
        s2 ^= t;
        s3 = BitOperations.RotateLeft(s3, 45);
    }

    /// <summary>Splitmix64 PRNG.</summary>
    /// <param name="x">PRNG state. This can take any value, including zero.</param>
    /// <returns>A new random <see langword="uint"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Next(ref ulong x)
    {
        var z = x += 0x9e3779b97f4a7c15;
        z = (z ^ z >> 30) * 0xbf58476d1ce4e5b9;
        z = (z ^ z >> 27) * 0x94d049bb133111eb;
        return z ^ z >> 31;
    }

    /// <summary>Samples the next value.</summary>
    /// <param name="s">The value to use.</param>
    /// <returns>The sampled value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Sample(ulong s) => BitOperations.RotateLeft(s * 5, 7) * 9;

    /// <inheritdoc />
    public override void NextBytes(byte[] buffer) => NextBytes((Span<byte>)buffer);

    /// <inheritdoc/>
    public override void NextBytes(Span<byte> span)
    {
        var (s0, s1, s2, s3) = (_s0, _s1, _s2, _s3);

        while (span.Length >= sizeof(ulong))
        {
            Unsafe.As<byte, ulong>(ref MemoryMarshal.GetReference(span)) = Sample(s1);
            Next(ref s0, ref s1, ref s2, ref s3);
            span = span.UnsafelySkip(sizeof(ulong));
        }

        if (!span.IsEmpty)
        {
            var next = Sample(s1);

            for (var i = 0; i < span.Length; i++)
                span[i] = Unsafe.Add(ref Unsafe.As<ulong, byte>(ref next), i);

            Next(ref s0, ref s1, ref s2, ref s3);
        }

        (_s0, _s1, _s2, _s3) = (s0, s1, s2, s3);
    }

    /// <inheritdoc />
    public override double NextDouble() => (NextUInt64() >> 11) * 1.1102230246251565e-16;

    /// <inheritdoc cref="NextDouble"/>
    public
#if NET6_0_OR_GREATER
        override
#endif
        float NextSingle() =>
        (NextUInt64() >> 40) * 5.9604645e-08f;

    /// <inheritdoc />
    public override int Next()
    {
        ulong n;

        do
            n = NextUInt64() >> 33;
        while (n is int.MaxValue);

        return (int)n;
    }

    /// <inheritdoc />
    public override int Next(int maxValue) => (int)NextUInt32((uint)maxValue);

    /// <inheritdoc />
    public override int Next(int minValue, int maxValue) => (int)NextUInt32((uint)(maxValue - minValue)) + minValue;

    /// <inheritdoc cref="Next()"/>
    public
#if NET6_0_OR_GREATER
        override
#endif
        long NextInt64()
    {
        ulong n;

        do
            n = NextUInt64() >> 1;
        while (n is long.MaxValue);

        return (long)n;
    }

    /// <inheritdoc cref="Next(int)"/>
    public
#if NET6_0_OR_GREATER
        override
#endif
        long NextInt64(long maxValue) =>
        (long)NextUInt64((ulong)maxValue);

    /// <inheritdoc cref="Next(int, int)"/>
    public
#if NET6_0_OR_GREATER
        override
#endif
        long NextInt64(long minValue, long maxValue) =>
        (long)NextUInt64((ulong)(maxValue - minValue)) + minValue;

    /// <summary>Gets the next <see langword="uint"/>.</summary>
    /// <returns>The next <see langword="uint"/>.</returns>
    public uint NextUInt32() => (uint)(NextUInt64() >> 32);

    /// <summary>Gets the next <see langword="ulong"/>.</summary>
    /// <param name="maxValue">The exclusive maximum value.</param>
    /// <returns>The next <see langword="ulong"/>.</returns>
    public uint NextUInt32(uint maxValue)
    {
        var n1 = maxValue * (ulong)NextUInt32();
        var n2 = (uint)n1;

        // ReSharper disable once InvertIf
        if (n2 < maxValue)
            for (var index = (uint)-(int)maxValue % maxValue; n2 < index; n2 = (uint)n1)
                n1 = (ulong)maxValue * NextUInt32();

        return (uint)(n1 >> 32);
    }

    /// <summary>Gets the next <see langword="ulong"/>.</summary>
    /// <param name="minValue">The inclusive minimum value.</param>
    /// <param name="maxValue">The exclusive maximum value.</param>
    /// <returns>The next <see langword="ulong"/>.</returns>
    public ulong NextUInt32(uint minValue, uint maxValue) => NextUInt32(maxValue - minValue) + minValue;

    /// <summary>Gets the next <see langword="ulong"/>.</summary>
    /// <returns>The next <see langword="ulong"/>.</returns>
    public ulong NextUInt64()
    {
        var (s0, s1, s2, s3) = (_s0, _s1, _s2, _s3);
        var result = Sample(s1);
        Next(ref s0, ref s1, ref s2, ref s3);
        (_s0, _s1, _s2, _s3) = (s0, s1, s2, s3);
        return result;
    }

    /// <summary>Gets the next <see langword="ulong"/>.</summary>
    /// <param name="maxValue">The exclusive maximum value.</param>
    /// <returns>The next <see langword="ulong"/>.</returns>
    public ulong NextUInt64(ulong maxValue)
    {
        var n1 = Math.BigMul(maxValue, NextUInt64(), out var low);

        // ReSharper disable once InvertIf
        if (low < maxValue)
        {
            var n2 = (0 - maxValue) % maxValue;

            while (low < n2)
                n1 = Math.BigMul(maxValue, NextUInt64(), out low);
        }

        return n1;
    }

    /// <summary>Gets the next <see langword="ulong"/>.</summary>
    /// <param name="minValue">The inclusive minimum value.</param>
    /// <param name="maxValue">The exclusive maximum value.</param>
    /// <returns>The next <see langword="ulong"/>.</returns>
    public ulong NextUInt64(ulong minValue, ulong maxValue) => NextUInt64(maxValue - minValue) + minValue;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override double Sample() => (Sample(_s1) >> 11) * 1.1102230246251565e-16;
}
#endif
