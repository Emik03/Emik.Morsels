// SPDX-License-Identifier: MPL-2.0
#if NET7_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for <see cref="INumber{TSelf}"/>.</summary>
static partial class NumberInterfaceExtensions
{
    /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsCanonical<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsCanonical(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsComplexNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsComplexNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsEvenInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsEvenInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsFinite"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsFinite<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsFinite(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsImaginaryNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsImaginaryNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInfinity"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsInfinity(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInteger"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNaN"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNaN<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNaN(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegative"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNegative<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNegative(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNegativeInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNegativeInfinity(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNormal"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsNormal<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsNormal(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsOddInteger<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsOddInteger(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositive"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPositive<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsPositive(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPositiveInfinity<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsPositiveInfinity(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsRealNumber<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsRealNumber(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsSubnormal<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsSubnormal(value);

    /// <inheritdoc cref="INumberBase{TSelf}.IsZero"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsZero<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.IsZero(value);

    /// <inheritdoc cref="INumber{TSelf}.Sign"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Sign<TSelf>(this TSelf value)
        where TSelf : INumber<TSelf> =>
        TSelf.Sign(value);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Abs<TSelf>(this TSelf value)
        where TSelf : INumberBase<TSelf> =>
        TSelf.Abs(value);

    /// <inheritdoc cref="INumber{TSelf}.Clamp"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Clamp<TSelf>(this TSelf value, TSelf min, TSelf max)
        where TSelf : INumber<TSelf> =>
        TSelf.Clamp(value, min, max);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf CopySign<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumber<TSelf> =>
        TSelf.CopySign(value, sign);
#if NET9_0_OR_GREATER
    /// <inheritdoc cref="INumberBase{TSelf}.MultiplyAddEstimate"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MultiplyAddEstimate<TSelf>(this TSelf left, TSelf right, TSelf addend)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MultiplyAddEstimate(left, right, addend);
#endif
    /// <inheritdoc cref="INumber{TSelf}.Min"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Min<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumber<TSelf> =>
        TSelf.Min(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinMagnitude<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MinMagnitude(value, sign);

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinMagnitudeNumber<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MinMagnitudeNumber(value, sign);

    /// <inheritdoc cref="INumber{TSelf}.MinNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MinNumber<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumber<TSelf> =>
        TSelf.MinNumber(value, sign);

    /// <inheritdoc cref="INumber{TSelf}.Max"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf Max<TSelf>(this TSelf x, TSelf y)
        where TSelf : INumber<TSelf> =>
        TSelf.Max(x, y);

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxMagnitude<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MaxMagnitude(value, sign);

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxMagnitudeNumber<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumberBase<TSelf> =>
        TSelf.MaxMagnitudeNumber(value, sign);

    /// <inheritdoc cref="INumber{TSelf}.MaxNumber"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TSelf MaxNumber<TSelf>(this TSelf value, TSelf sign)
        where TSelf : INumber<TSelf> =>
        TSelf.MaxNumber(value, sign);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateChecked{TOther}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateChecked<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateChecked(value);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateSaturating{TOther}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateSaturating<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateSaturating(value);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateTruncating{TOther}"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static TOther CreateTruncating<TSelf, TOther>(this TSelf value)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther> =>
        TOther.CreateTruncating(value);
}
#endif
