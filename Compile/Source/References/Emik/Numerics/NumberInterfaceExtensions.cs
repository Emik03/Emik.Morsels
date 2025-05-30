// SPDX-License-Identifier: MPL-2.0
#if NET7_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for <see cref="INumber{TSelf}"/>.</summary>
static partial class NumberInterfaceExtensions
{
    /// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadBigEndian(ReadOnlySpan{byte}, bool, out TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryReadBigEndian<TSelf>(this ReadOnlySpan<byte> source, bool isUnsigned, out TSelf value)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.TryReadBigEndian(source, isUnsigned, out value);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadLittleEndian(ReadOnlySpan{byte}, bool, out TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryReadLittleEndian<TSelf>(this ReadOnlySpan<byte> source, bool isUnsigned, out TSelf value)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.TryReadLittleEndian(source, isUnsigned, out value);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteBigEndian<TSelf>(this TSelf value, Span<byte> destination, out int bytesWritten)
        where TSelf : IBinaryInteger<TSelf> =>
        value.TryWriteBigEndian(destination, out bytesWritten);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteLittleEndian<TSelf>(this TSelf value, Span<byte> destination, out int bytesWritten)
        where TSelf : IBinaryInteger<TSelf> =>
        value.TryWriteLittleEndian(destination, out bytesWritten);

    /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsCanonical<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsCanonical(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsComplexNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsComplexNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsEvenInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsEvenInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsFinite"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsFinite<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsFinite(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsImaginaryNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsImaginaryNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInfinity"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsInfinity(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInteger"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNaN"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNaN<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNaN(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegative"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNegative<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNegative(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNegativeInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNegativeInfinity(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNormal"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNormal<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNormal(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsOddInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsOddInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositive"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPositive<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsPositive(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPositiveInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsPositiveInfinity(value);

    /// <inheritdoc cref="IBinaryNumber{TSelf}.IsPow2"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2<TSelf>(this TSelf value)
        where TSelf : IBinaryNumber<TSelf> =>
        TSelf.IsPow2(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsRealNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsRealNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsSubnormal<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsSubnormal(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsZero"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsZero<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsZero(value);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.GetByteCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetByteCount<TSelf>(this TSelf x)
        where TSelf : IBinaryInteger<TSelf> =>
        x.GetByteCount();

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteBigEndian(byte[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteBigEndian<TSelf>(this TSelf value, byte[] destination)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteBigEndian(destination);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteBigEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteBigEndian<TSelf>(this TSelf value, byte[] destination, int startIndex)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteBigEndian(destination, startIndex);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteBigEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteBigEndian<TSelf>(this TSelf value, Span<byte> destination)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteBigEndian(destination);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteLittleEndian(byte[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteLittleEndian<TSelf>(this TSelf value, byte[] destination)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteLittleEndian(destination);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteLittleEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteLittleEndian<TSelf>(this TSelf value, byte[] destination, int startIndex)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteLittleEndian(destination, startIndex);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.WriteLittleEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteLittleEndian<TSelf>(this TSelf value, Span<byte> destination)
        where TSelf : IBinaryInteger<TSelf> =>
        value.WriteLittleEndian(destination);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.GetShortestBitLength"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetShortestBitLength<TSelf>(this TSelf x)
        where TSelf : IBinaryInteger<TSelf> =>
        x.GetShortestBitLength();

    /// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentByteCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetExponentByteCount<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        x.GetExponentByteCount();

    /// <inheritdoc cref="IFloatingPoint{TSelf}.GetExponentShortestBitLength"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetExponentShortestBitLength<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        x.GetExponentShortestBitLength();

    /// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandBitLength"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetSignificandBitLength<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        x.GetSignificandBitLength();

    /// <inheritdoc cref="IFloatingPoint{TSelf}.GetSignificandByteCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int GetSignificandByteCount<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        x.GetSignificandByteCount();

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ILogB"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once InconsistentNaming
    public static int ILogB<TSelf>(TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.ILogB(x);

    /// <inheritdoc cref="INumberBase{TSelf}.Radix"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Radix<TSelf>()
        where TSelf : INumberBase<TSelf> =>
        TSelf.Radix;

    /// <inheritdoc cref="INumber{TSelf}.Sign"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Sign<TSelf>(this TSelf value)
        where TSelf : INumber<TSelf> =>
        TSelf.Sign(value);

    /// <inheritdoc cref="INumberBase{TSelf}.Zero"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf AllBitsSet<TSelf>()
        where TSelf : IBinaryNumber<TSelf> =>
        TSelf.AllBitsSet;

    /// <inheritdoc cref="IBinaryNumber{TSelf}.Log2"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log2<TSelf>(this TSelf value)
        where TSelf : IBinaryNumber<TSelf> =>
        TSelf.Log2(value);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Abs<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.Abs(value);

    /// <inheritdoc cref="INumber{TSelf}.Clamp"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Clamp<TSelf>(this TSelf value, TSelf min, TSelf max)
        where TSelf : INumber<TSelf> =>
        TSelf.Clamp(value, min, max);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf CopySign<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumber<TSelf> =>
        TSelf.CopySign(value, sign);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Exp<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.Exp(x);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Exp10<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.Exp10(x);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp10M1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Exp10M1<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.Exp10M1(x);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Exp2<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.Exp2(x);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp2M1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Exp2M1<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.Exp2M1(x);

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.ExpM1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ExpM1<TSelf>(this TSelf x)
        where TSelf : IExponentialFunctions<TSelf> =>
        TSelf.ExpM1(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Ceiling<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Ceiling(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Floor<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Floor(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Round<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Round(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Round<TSelf>(this TSelf x, int digits)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Round(x, digits);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int, MidpointRounding)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Round<TSelf>(this TSelf x, MidpointRounding mode)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Round(x, mode);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf, int, MidpointRounding)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Round<TSelf>(this TSelf x, int digits, MidpointRounding mode)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Round(x, digits, mode);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate(TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Truncate<TSelf>(this TSelf x)
        where TSelf : IFloatingPoint<TSelf> =>
        TSelf.Truncate(x);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentBigEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteExponentBigEndian<TSelf>(this TSelf x, Span<byte> destination, out int bytesWritten)
        where TSelf : IFloatingPoint<TSelf> =>
        x.TryWriteExponentBigEndian(destination, out bytesWritten);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteExponentBigEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteExponentLittleEndian<TSelf>(this TSelf x, Span<byte> destination, out int bytesWritten)
        where TSelf : IFloatingPoint<TSelf> =>
        x.TryWriteExponentLittleEndian(destination, out bytesWritten);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandBigEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteSignificandBigEndian<TSelf>(this TSelf x, Span<byte> destination, out int bytesWritten)
        where TSelf : IFloatingPoint<TSelf> =>
        x.TryWriteExponentLittleEndian(destination, out bytesWritten);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.TryWriteSignificandLittleEndian(Span{byte}, out int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool TryWriteSignificandLittleEndian<TSelf>(
        this TSelf x,
        Span<byte> destination,
        out int bytesWritten
    )
        where TSelf : IFloatingPoint<TSelf> =>
        x.TryWriteSignificandLittleEndian(destination, out bytesWritten);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentBigEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentBigEndian<TSelf>(this TSelf x, byte[] destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentBigEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentBigEndian<TSelf>(this TSelf x, byte[] destination, int startIndex)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination, startIndex);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentBigEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentBigEndian<TSelf>(this TSelf x, Span<byte> destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentLittleEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentLittleEndian<TSelf>(this TSelf x, byte[] destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentLittleEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentLittleEndian<TSelf>(this TSelf x, byte[] destination, int startIndex)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination, startIndex);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteExponentLittleEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteExponentLittleEndian<TSelf>(this TSelf x, Span<byte> destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteExponentBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandBigEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandBigEndian<TSelf>(this TSelf x, byte[] destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandBigEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandBigEndian<TSelf>(this TSelf x, byte[] destination, int startIndex)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandBigEndian(destination, startIndex);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandBigEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandBigEndian<TSelf>(this TSelf x, Span<byte> destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandBigEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandLittleEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandLittleEndian<TSelf>(this TSelf x, byte[] destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandLittleEndian(destination);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandLittleEndian(byte[], int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandLittleEndian<TSelf>(this TSelf x, byte[] destination, int startIndex)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandLittleEndian(destination, startIndex);

    /// <inheritdoc cref="IFloatingPoint{TSelf}.WriteSignificandLittleEndian(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int WriteSignificandLittleEndian<TSelf>(this TSelf x, Span<byte> destination)
        where TSelf : IFloatingPoint<TSelf> =>
        x.WriteSignificandLittleEndian(destination);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.LeadingZeroCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf LeadingZeroCount<TSelf>(this TSelf x)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.LeadingZeroCount(x);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.PopCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf PopCount<TSelf>(this TSelf x)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.PopCount(x);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadBigEndian(byte[], bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadBigEndian<TSelf>(this byte[] source, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadBigEndian(source, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadBigEndian(byte[], int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadBigEndian<TSelf>(this byte[] source, int startIndex, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadBigEndian(source, startIndex, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadBigEndian(ReadOnlySpan{byte}, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadBigEndian<TSelf>(this ReadOnlySpan<byte> source, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadBigEndian(source, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadLittleEndian(byte[], bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadLittleEndian<TSelf>(this byte[] source, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadLittleEndian(source, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadLittleEndian(byte[], int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadLittleEndian<TSelf>(this byte[] source, int startIndex, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadLittleEndian(source, startIndex, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.ReadLittleEndian(ReadOnlySpan{byte}, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReadLittleEndian<TSelf>(this ReadOnlySpan<byte> source, bool isUnsigned)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.ReadLittleEndian(source, isUnsigned);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.RotateLeft"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf RotateLeft<TSelf>(this TSelf value, int rotateAmount)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.RotateLeft(value, rotateAmount);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.RotateRight"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf RotateRight<TSelf>(this TSelf value, int rotateAmount)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.RotateRight(value, rotateAmount);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.TrailingZeroCount"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf TrailingZeroCount<TSelf>(this TSelf value)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.TrailingZeroCount(value);

    /// <inheritdoc cref="IFloatingPointConstants{TSelf}.E"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf E<TSelf>()
        where TSelf : IFloatingPointConstants<TSelf> =>
        TSelf.E;

    /// <inheritdoc cref="IFloatingPointConstants{TSelf}.Pi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Pi<TSelf>()
        where TSelf : IFloatingPointConstants<TSelf> =>
        TSelf.Pi;

    /// <inheritdoc cref="IFloatingPointConstants{TSelf}.Tau"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Tau<TSelf>()
        where TSelf : IFloatingPointConstants<TSelf> =>
        TSelf.Tau;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Epsilon"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Epsilon<TSelf>()
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.Epsilon;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NaN"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf NaN<TSelf>()
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.NaN;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NegativeInfinity"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf NegativeInfinity<TSelf>()
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.NegativeInfinity;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.NegativeZero"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf NegativeZero<TSelf>()
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.NegativeZero;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.PositiveInfinity"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf PositiveInfinity<TSelf>()
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.PositiveInfinity;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Atan2<TSelf>(TSelf y, TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.Atan2(y, x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Atan2Pi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Atan2Pi<TSelf>(TSelf y, TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.Atan2Pi(y, x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitDecrement"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf BitDecrement<TSelf>(TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.BitDecrement(x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.BitIncrement"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf BitIncrement<TSelf>(TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.BitIncrement(x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.FusedMultiplyAdd"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf FusedMultiplyAdd<TSelf>(TSelf left, TSelf right, TSelf addend)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.FusedMultiplyAdd(left, right, addend);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Ieee754Remainder"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Ieee754Remainder<TSelf>(TSelf left, TSelf right)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.Ieee754Remainder(left, right);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.Lerp"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Lerp<TSelf>(TSelf value1, TSelf value2, TSelf amount)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.Lerp(value1, value2, amount);
#endif
    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ReciprocalEstimate"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReciprocalEstimate<TSelf>(TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.ReciprocalEstimate(x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ReciprocalSqrtEstimate"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ReciprocalSqrtEstimate<TSelf>(TSelf x)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.ReciprocalSqrtEstimate(x);

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.ScaleB"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf ScaleB<TSelf>(TSelf x, int n)
        where TSelf : IFloatingPointIeee754<TSelf> =>
        TSelf.ScaleB(x, n);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Acosh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Acosh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Acosh(x);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Asinh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Asinh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Asinh(x);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Atanh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Atanh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Atanh(x);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Cosh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Cosh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Cosh(x);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Sinh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Sinh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Sinh(x);

    /// <inheritdoc cref="IHyperbolicFunctions{TSelf}.Tanh"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Tanh<TSelf>(this TSelf x)
        where TSelf : IHyperbolicFunctions<TSelf> =>
        TSelf.Tanh(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf, TSelf)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log<TSelf>(this TSelf x, TSelf newBase)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log(x, newBase);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.LogP1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf LogP1<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.LogP1(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log2"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf LogTwo<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log2(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log2P1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log2P1<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log2P1(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log10<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log10(x);

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log10P1"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Log10P1<TSelf>(this TSelf x)
        where TSelf : ILogarithmicFunctions<TSelf> =>
        TSelf.Log10P1(x);
#if NET9_0_OR_GREATER
    /// <inheritdoc cref="INumberBase{TSelf}.MultiplyAddEstimate"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MultiplyAddEstimate<TSelf>(this TSelf left, TSelf right, TSelf addend)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MultiplyAddEstimate(left, right, addend);
#endif
    /// <inheritdoc cref="INumber{TSelf}.Min"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Min<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumber<TSelf> =>
        TSelf.Min(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinMagnitude<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MinMagnitude(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinMagnitudeNumber<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MinMagnitudeNumber(x, y);

    /// <inheritdoc cref="INumber{TSelf}.MinNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinNumber<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumber<TSelf> =>
        TSelf.MinNumber(value, sign);

    /// <inheritdoc cref="INumber{TSelf}.Max"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Max<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumber<TSelf> =>
        TSelf.Max(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxMagnitude<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MaxMagnitude(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxMagnitudeNumber<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MaxMagnitudeNumber(x, y);

    /// <inheritdoc cref="INumber{TSelf}.MaxNumber"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxNumber<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumber<TSelf> =>
        TSelf.MaxNumber(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.One"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf One<TSelf>()
        where TSelf : INumberBase<TSelf> =>
        TSelf.One;

    /// <inheritdoc cref="INumberBase{TSelf}.Zero"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Zero<TSelf>()
        where TSelf : INumberBase<TSelf> =>
        TSelf.Zero;

    /// <inheritdoc cref="IPowerFunctions{TSelf}.Pow"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Pow<TSelf>(this TSelf x, TSelf y)
        where TSelf : IPowerFunctions<TSelf> =>
        TSelf.Pow(x, y);

    /// <inheritdoc cref="IRootFunctions{TSelf}.Cbrt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Cbrt<TSelf>(this TSelf x)
        where TSelf : IRootFunctions<TSelf> =>
        TSelf.Cbrt(x);

    /// <inheritdoc cref="IRootFunctions{TSelf}.Hypot"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Hypot<TSelf>(this TSelf x, TSelf y)
        where TSelf : IRootFunctions<TSelf> =>
        TSelf.Hypot(x, y);

    /// <inheritdoc cref="IRootFunctions{TSelf}.RootN"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf RootN<TSelf>(this TSelf x, int n)
        where TSelf : IRootFunctions<TSelf> =>
        TSelf.RootN(x, n);

    /// <inheritdoc cref="IRootFunctions{TSelf}.Sqrt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Sqrt<TSelf>(this TSelf x)
        where TSelf : IRootFunctions<TSelf> =>
        TSelf.Sqrt(x);

    /// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf NegativeOne<TSelf>()
        where TSelf : ISignedNumber<TSelf> =>
        TSelf.NegativeOne;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Acos"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Acos<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Acos(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AcosPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf AcosPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.AcosPi(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Asin"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Asin<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Asin(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AsinPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf AsinPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.AsinPi(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Atan"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Atan<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Atan(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.AtanPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf AtanPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.AtanPi(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Cos"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Cos<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Cos(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.CosPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf CosPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.CosPi(x);
#if NET8_0_OR_GREATER
    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.DegreesToRadians"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf DegreesToRadians<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.DegreesToRadians(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.RadiansToDegrees"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf RadiansToDegrees<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.RadiansToDegrees(x);
#endif
    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Sin"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Sin<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Sin(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf SinPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.SinPi(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Tan"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Tan<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.Tan(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.TanPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf TanPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.TanPi(x);

    /// <inheritdoc cref="IBinaryInteger{TSelf}.DivRem"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static (TSelf Quotient, TSelf Remainder) DivRem<TSelf>(this TSelf left, TSelf right)
        where TSelf : IBinaryInteger<TSelf> =>
        TSelf.DivRem(left, right);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static (TSelf Sin, TSelf Cos) SinCos<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.SinCos(x);

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCosPi"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static (TSelf Sin, TSelf Cos) SinCosPi<TSelf>(this TSelf x)
        where TSelf : ITrigonometricFunctions<TSelf> =>
        TSelf.SinCosPi(x);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateChecked{TOther}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateChecked<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateChecked(value);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateSaturating{TOther}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateSaturating<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateSaturating(value);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateTruncating{TOther}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateTruncating<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateTruncating(value);
}
#endif
