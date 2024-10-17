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
    /// <exception cref="ArgumentOutOfRangeException">Any span provided does not have the same length.</exception>
#pragma warning disable MA0051
    public static void FFT<T>(
#pragma warning restore MA0051
        scoped ReadOnlySpan<T> bre,
        scoped ReadOnlySpan<T> bim,
        scoped Span<T> re,
        scoped Span<T> im
    )
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        if (re.Length is var length && length != im.Length || length != bre.Length || length != bim.Length)
            throw new ArgumentOutOfRangeException(nameof(re), "All spans must be the same length.");

        if (length is 0)
            return;

        if (length.IsPow2())
        {
            Parallelize(re, im);
            return;
        }

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

    /// <summary>Performs a radix-2 step.</summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    /// <param name="re">The real part.</param>
    /// <param name="im">The imaginary part.</param>
    /// <param name="e">The exponent.</param>
    static void Radix2<T>(Span<T> re, Span<T> im, int e)
        where T : ITrigonometricFunctions<T>
    {
        // ReSharper disable once RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.Assert(re.Length == im.Length, "buffers must be the same length");
        Radix2Reorder(re, im);

        for (var size = 1; size < re.Length; size *= 2)
            for (var k = 0; k < size && T.CreateChecked(e * k) * T.Pi / T.CreateChecked(size) is var a; k++)
                for (var i = k; i < re.Length; i += size * 2)
                {
                    var nextRe = a.Cos() * re.UnsafelyIndex(i + size) - a.Sin() * im.UnsafelyIndex(i + size);
                    var nextIm = a.Cos() * im.UnsafelyIndex(i + size) + a.Sin() * re.UnsafelyIndex(i + size);
                    (re[i + size], im[i + size]) = (re.UnsafelyIndex(i) - nextRe, im.UnsafelyIndex(i) - nextIm);
                    (re[i], im[i]) = (re.UnsafelyIndex(i) + nextRe, im.UnsafelyIndex(i) + nextIm);
                }
    }

    static void Radix2Reorder<T>(Span<T> re, Span<T> im)
    {
        for (int i = 0, j = 0; j < re.Length - 1; j++)
        {
            if (i > j)
                (re[i], re[j], im[i], im[j]) =
                    (re.UnsafelyIndex(j), re.UnsafelyIndex(i), im.UnsafelyIndex(j), im.UnsafelyIndex(i));

            var length = re.Length;

            do i ^= length >>= 1;
            while ((i & length) is 0);
        }
    }

    static unsafe void Parallelize<T>(Span<T> re, Span<T> im)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        Radix2Reorder(re, im);
#pragma warning disable 8500
        Span<T>* pre = &re, pim = &im;

        for (var l = 1; l < re.Length; l *= 2)
        {
            nint nre = (nint)pre, nim = (nint)pim;
            var s = l;

            void Body(Tuple<int, int> range)
            {
                Span<T> real = *(Span<T>*)nre, imaginary = *(Span<T>*)nim;
#pragma warning restore 8500
                for (var i = range.Item1; i < range.Item2; i++)
                {
                    var exponent = -T.One * T.CreateChecked(i) * T.Pi / T.CreateChecked(s);
                    var wim = T.Sin(exponent);
                    var wre = T.Cos(exponent);

                    // (x.re * y.re - x.im * y.im,
                    //  x.re * y.im + x.im * y.re)
                    for (var j = i; j < real.Length; j += s << 1)
                    {
                        var (are, aim) = (real[j], imaginary[j]);

                        var (tre, tim) = (wre * real[j + s] - wim * imaginary[j + s],
                            wre * imaginary[j + s] + wim * real[j + s]);

                        (real[j], imaginary[j], real[j + s], imaginary[j + s]) =
                            (are + tre, aim + tre, are - tre, aim - tim);
                    }
                }
            }

            const int RangeSize = 64;

            if (s >= RangeSize)
                Parallel.ForEach(Partitioner.Create(0, s, RangeSize), Body);
            else
                Body(new(0, s));
        }
    }
}
#endif
