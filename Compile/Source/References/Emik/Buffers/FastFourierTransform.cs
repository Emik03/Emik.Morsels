// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NET7_0_OR_GREATER
/// <summary>Computes the Fast Fourier Transform.</summary>
static partial class FastFourierTransform
{
    /// <summary>Computes the Fast Fourier Transform in place.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="bluestein">The bluestein transform.</param>
    /// <param name="real">The real part.</param>
    /// <param name="imaginary">The imaginary part.</param>
    /// <exception cref="ArgumentOutOfRangeException">Any span provided does not have the same length.</exception>
    public static void FFT<T>(
        this (ImmutableArray<T> Real, ImmutableArray<T> Imaginary) bluestein,
        scoped Span<T> real,
        scoped Span<T> imaginary
    )
        where T : IRootFunctions<T>, ITrigonometricFunctions<T> =>
        FFT(bluestein.Real.AsSpan(), bluestein.Imaginary.AsSpan(), real, imaginary);

    /// <summary>Computes the Fast Fourier Transform in place.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="bre">The bluestein real part.</param>
    /// <param name="bim">The bluestein imaginary part.</param>
    /// <param name="re">The real part.</param>
    /// <param name="im">The imaginary part.</param>
    /// <exception cref="InvalidOperationException">Any span provided does not have the same length.</exception>
    public static void FFT<T>(
        scoped ReadOnlySpan<T> bre,
        scoped ReadOnlySpan<T> bim,
        scoped Span<T> re,
        scoped Span<T> im
    )
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        if (re.Length is var length && length != im.Length || length != bre.Length || length != bim.Length)
            throw new InvalidOperationException($"re/{re.Length}, im/{im.Length}, bre/{bre.Length}, bim/{bim.Length}");

        if (length is 0)
            return;

        var subLength = (int)(length * 2 - 1).RoundUpToPowerOf2();
        var rent = ArrayPool<T>.Shared.Rent(subLength * 4);

        try
        {
            Span<T> ar = rent.AsSpan(0, subLength),
                ai = rent.AsSpan(subLength, subLength),
                br = rent.AsSpan(subLength * 2, subLength),
                bi = rent.AsSpan(subLength * 3, subLength);

            bre.CopyTo(br);
            bim.CopyTo(bi);
            ar.UnsafelySkip(length).Clear();
            ai.UnsafelySkip(length).Clear();
            var i = subLength - length + 1;
            br.UnsafelySlice(length, i - length).Clear();
            bi.UnsafelySlice(length, i - length).Clear();

            for (; i < subLength; i++)
                (br[i], bi[i]) = (bre.UnsafelyIndex(subLength - i), bim.UnsafelyIndex(subLength - i));

            Radix2(br, bi, -1);

            for (i = 0; i < length; i++)
            {
                ar[i] = bre.UnsafelyIndex(i) * re.UnsafelyIndex(i) + bim.UnsafelyIndex(i) * im.UnsafelyIndex(i);
                ai[i] = bre.UnsafelyIndex(i) * im.UnsafelyIndex(i) - bim.UnsafelyIndex(i) * re.UnsafelyIndex(i);
            }

            Radix2(ar, ai, -1);

            for (i = 0; i < subLength; i++)
            {
                var r = ar.UnsafelyIndex(i);
                ar[i] = r * br.UnsafelyIndex(i) - ai.UnsafelyIndex(i) * bi.UnsafelyIndex(i);
                ai[i] = r * bi.UnsafelyIndex(i) + ai.UnsafelyIndex(i) * br.UnsafelyIndex(i);
            }

            Radix2(ar, ai, 1);
            T n = T.One / T.CreateChecked(subLength), halfRescale = (T.One / T.CreateChecked(length)).Sqrt();

            for (i = 0; i < length; i++)
            {
                re[i] = (n * bre.UnsafelyIndex(i) * ar[i] - n * -bim.UnsafelyIndex(i) * ai[i]) * halfRescale;
                im[i] = (n * bre.UnsafelyIndex(i) * ai[i] + n * -bim.UnsafelyIndex(i) * ar[i]) * halfRescale;
            }
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rent);
        }
    }

    /// <summary>Computes the Bluestein transform.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="length">The length.</param>
    /// <returns>The real and imaginary parts.</returns>
    public static (ImmutableArray<T> Real, ImmutableArray<T> Imaginary) Bluestein<T>(this int length)
        where T : ITrigonometricFunctions<T>
    {
        T[] re = new T[length], im = new T[length];
        var scale = T.Pi / T.CreateChecked(length);
        var i = 0;
#if NET9_0_OR_GREATER
        if (Vector<T>.IsSupported && Vector.IsHardwareAccelerated)
            for (; i + Vector<T>.Count <= length; i += Vector<T>.Count)
            {
                var step = Vector.CreateSequence(T.CreateChecked(i), T.One);
                var (sin, cos) = (scale * step * step).SinCos();
                sin.StoreUnsafe(ref MemoryMarshal.GetArrayDataReference(im), unchecked((nuint)i));
                cos.StoreUnsafe(ref MemoryMarshal.GetArrayDataReference(re), unchecked((nuint)i));
            }
#endif
        for (; i < length; i++)
            (im[i], re[i]) = (scale * T.CreateChecked(i) * T.CreateChecked(i)).SinCos();

        return (ImmutableCollectionsMarshal.AsImmutableArray(re), ImmutableCollectionsMarshal.AsImmutableArray(im));
    }
#if NET9_0_OR_GREATER
    /// <summary>Computes the Fast Fourier Transform in place and returns the maximum hypotenuse.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="bluestein">The bluestein transform.</param>
    /// <param name="realBuffer">The buffer that will be replaced with the hypotenuse of the fourier transform.</param>
    /// <param name="skipHypotOnLastHalf">
    /// Whether to skip calculating the hypotenuse on the second half of the buffer. This is because the nature of the
    /// fourier transform with all zeros for the imaginary buffer causes the real buffer to be always symmetrical.
    /// If this is set to <see langword="true"/>, only the first half of the buffer will have the computed hypotenuse.
    /// The second half will still be written to, but only with its real counterpart. If set to <see langword="false"/>,
    /// the whole buffer will be written to, which guarantees that the buffer will end up symmetric.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Any span provided does not have the same length.</exception>
    public static T MaxHypotFFT<T>(
        this (ImmutableArray<T> Real, ImmutableArray<T> Imaginary) bluestein,
        scoped Span<T> realBuffer,
        bool skipHypotOnLastHalf = false
    )
        where T : IFloatingPointIeee754<T>
    {
        var rent = ArrayPool<T>.Shared.Rent(realBuffer.Length);

        try
        {
            var imaginaryBuffer = rent.AsSpan().UnsafelyTake(realBuffer.Length);
            imaginaryBuffer.Clear();
            bluestein.FFT(realBuffer, imaginaryBuffer);
            var max = T.Epsilon;
            ref var real = ref MemoryMarshal.GetReference(realBuffer);
            ref var imaginary = ref MemoryMarshal.GetReference(imaginaryBuffer);
            var length = skipHypotOnLastHalf ? (realBuffer.Length + 1) / 2 : realBuffer.Length;

            if (!Vector<T>.IsSupported || !Vector.IsHardwareAccelerated || Vector<T>.Count > length)
            {
                ref readonly var end = ref Unsafe.Add(ref real, length);

                while (Unsafe.IsAddressLessThan(real, end))
                {
                    max = max.Max(real = real.Hypot(imaginary));
                    // ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                    real = ref Unsafe.Add(ref real, 1)!;
                    imaginary = ref Unsafe.Add(ref imaginary, 1)!;
                }

                return max;
            }

            var maxVector = Vector<T>.Zero;
            ref var realLast = ref Unsafe.Add(ref real, length - Vector<T>.Count);
            ref readonly var imaginaryLast = ref Unsafe.Add(ref imaginary, length - Vector<T>.Count);
            StoreUnsafe(ref real, imaginary, ref maxVector);
            real = ref Unsafe.Add(ref real, Vector<T>.Count)!;
            imaginary = ref Unsafe.Add(ref imaginary, Vector<T>.Count)!;

            while (Unsafe.IsAddressLessThan(real, realLast))
            {
                StoreUnsafe(ref real, imaginary, ref maxVector);
                real = ref Unsafe.Add(ref real, Vector<T>.Count)!;
                imaginary = ref Unsafe.Add(ref imaginary, Vector<T>.Count)!;
                // ReSharper restore NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            }

            StoreUnsafe(ref realLast, imaginaryLast, ref maxVector);

            for (var index = 0; index < Vector<T>.Count; index++)
                max = max.Max(maxVector[index]);

            return max;
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rent);
        }
    }
#endif
    /// <summary>Performs a radix-2 step.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="re">The real part.</param>
    /// <param name="im">The imaginary part.</param>
    /// <param name="e">The exponent.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Radix2<T>(Span<T> re, Span<T> im, int e)
        where T : ITrigonometricFunctions<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void NextStep(Span<T> re, Span<T> im, T a, int i, int j)
        {
            var nextRe = a.Cos() * re.UnsafelyIndex(j) - a.Sin() * im.UnsafelyIndex(j);
            var nextIm = a.Cos() * im.UnsafelyIndex(j) + a.Sin() * re.UnsafelyIndex(j);
            (re[j], im[j]) = (re.UnsafelyIndex(i) - nextRe, im.UnsafelyIndex(i) - nextIm);
            (re[i], im[i]) = (re.UnsafelyIndex(i) + nextRe, im.UnsafelyIndex(i) + nextIm);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void NextReorder(ref int i, int j, Span<T> re, Span<T> im)
        {
            if (i > j)
                (re[i], re[j], im[i], im[j]) = (re.UnsafelyIndex(j), re.UnsafelyIndex(i), im.UnsafelyIndex(j),
                    im.UnsafelyIndex(i));

            var length = re.Length;

            do i ^= length >>= 1;
            while ((i & length) is 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Reorder(Span<T> re, Span<T> im)
        {
            for (int i = 0, j = 0; j < re.Length - 1; j++)
                NextReorder(ref i, j, re, im);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Step(Span<T> re, Span<T> im, int e)
        {
            for (var s = 1; s < re.Length; s *= 2)
                for (var k = 0; k < s && T.CreateChecked(e * k) * T.Pi / T.CreateChecked(s) is var a; k++)
                    for (var i = k; i < re.Length; i += s * 2)
                        NextStep(re, im, a, i, i + s);
        }

        // ReSharper disable once RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.Assert(re.Length == im.Length, "buffers must be the same length");
        Reorder(re, im);
        Step(re, im, e);
    }
#if NET9_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void StoreUnsafe<T>(ref T real, in T imaginary, ref Vector<T> max)
    {
        var hypot = Vector.LoadUnsafe(real).Hypot(Vector.LoadUnsafe(imaginary));
        max = Vector.Max(max, hypot);
        hypot.StoreUnsafe(ref real);
    }
#endif
}
#endif
