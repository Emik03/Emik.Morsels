// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP2_1_OR_GREATER
// ReSharper disable CheckNamespace RedundantAssignment
namespace System;
#pragma warning disable RCS1163
/// <summary>Provides the polyfill to <c>int.TryFormat</c>.</summary>
[Obsolete("This class shouldn't be referred to directly, as only the extension method is guaranteed.", true)]
static partial class Int32TryFormatPolyfill
{
    /// <summary>
    /// Tries to format the value of the current integer number instance into the provided span of characters.
    /// </summary>
    /// <param name="value">The instance.</param>
    /// <param name="destination">
    /// The span in which to write this instance's value formatted as a span of characters.
    /// </param>
    /// <param name="charsWritten">
    /// When this method returns, contains the number of characters that were written in <paramref name="destination"/>.
    /// </param>
    /// <param name="format">
    /// A span containing the characters that represent a standard or custom format
    /// string that defines the acceptable format for <paramref name="destination"/>.
    /// </param>
    /// <param name="provider">
    /// An optional object that supplies culture-specific formatting information for <paramref name="destination"/>.
    /// </param>
    /// <exception cref="NotSupportedException">The parameter <paramref name="format"/> isn't empty.</exception>
    /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryFormat(
        this int value,
        scoped Span<char> destination,
        out int charsWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null
    ) =>
        TryFormatInt32(value, ~0, format, provider, destination, out charsWritten);

    static bool TryFormatInt32(
        int value,
        [UsedImplicitly] int hexMask,
        ReadOnlySpan<char> format,
        IFormatProvider? provider,
        scoped Span<char> destination,
        out int charsWritten
    ) =>
        format.Length is 0
            ? value >= 0
                ? TryUInt32ToDecStr((uint)value, -1, destination, out charsWritten)
                : TryNegativeInt32ToDecStr(
                    value,
                    -1,
                    NumberFormatInfo.GetInstance(provider).NegativeSign,
                    destination,
                    out charsWritten
                )
            : throw new NotSupportedException();

    static unsafe bool TryUInt32ToDecStr(uint value, int digits, scoped Span<char> destination, out int charsWritten)
    {
        var bufferLength = Math.Max(digits, value.DigitCount());

        if (bufferLength > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        charsWritten = bufferLength;
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var buffer = (char*)destination.Pointer;
#else
        fixed (char* buffer = destination)
#endif
        {
            var p = buffer + bufferLength;
            p = digits <= 1 ? UInt32ToDecChars(p, value) : UInt32ToDecChars(p, value, digits);
            Debug.Assert(p == buffer, "p == buffer");
        }

        return true;
    }

    static unsafe bool TryNegativeInt32ToDecStr(
        int value,
        int digits,
        string sNegative,
        scoped Span<char> destination,
        out int charsWritten
    )
    {
        Debug.Assert(value < 0, "value < 0");

        if (digits < 1)
            digits = 1;

        var bufferLength = Math.Max(digits, ((uint)-value).DigitCount()) + sNegative.Length;

        if (bufferLength > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        charsWritten = bufferLength;

#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var buffer = (char*)destination.Pointer;
#else
        fixed (char* buffer = destination)
#endif
        {
            var p = UInt32ToDecChars(buffer + bufferLength, (uint)-value, digits);
            Debug.Assert(p == buffer + sNegative.Length, "p == buffer + sNegative.Length");

            for (var i = sNegative.Length - 1; i >= 0; i--)
                *--p = sNegative[i];

            Debug.Assert(p == buffer, "p == buffer");
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe char* UInt32ToDecChars(char* bufferEnd, uint value)
    {
        do
        {
            var quotient = value / 10;
            var remainder = value - quotient * 10;
            value = quotient;
            *--bufferEnd = (char)(remainder + '0');
        } while (value != 0);

        return bufferEnd;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe char* UInt32ToDecChars(char* bufferEnd, uint value, int digits)
    {
        while (--digits >= 0 || value != 0)
        {
            var quotient = value / 10;
            var remainder = value - quotient * 10;
            value = quotient;
            *--bufferEnd = (char)(remainder + '0');
        }

        return bufferEnd;
    }
}
#endif
