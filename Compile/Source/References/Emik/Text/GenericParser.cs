// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides extension methods to convert representations of text into destination types.</summary>
static partial class GenericParser
{
    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this string s) => Parse<T>(s, out _);

    /// <summary>Parses the <see cref="string"/> into a <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type to parse into.</typeparam>
    /// <param name="s">The buffer source.</param>
    /// <param name="success">Whether the parsing was successful.</param>
    /// <returns>The parsed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this string s, out bool success) => FindTryParseFor<T>.WithString(s, out success);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this ReadOnlySpan<byte> s) => Parse<T>(s, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this ReadOnlySpan<byte> s, out bool success) =>
        FindTryParseFor<T>.WithByteSpan(s, out success);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this ReadOnlySpan<char> s) => Parse<T>(s, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Parse<T>(this ReadOnlySpan<char> s, out bool success) =>
        FindTryParseFor<T>.WithCharSpan(s, out success);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this string s)
        where T : IParsable<T> =>
        Into<T>(s, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this string s, IFormatProvider? provider)
        where T : IParsable<T> =>
        Into<T>(s, provider, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this string s, out bool success)
        where T : IParsable<T> =>
        (success = T.TryParse(s, CultureInfo.InvariantCulture, out var result)) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this string s, IFormatProvider? provider, out bool success)
        where T : IParsable<T> =>
        (success = T.TryParse(s, provider, out var result)) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<byte> s)
        where T : IUtf8SpanParsable<T> =>
        Into<T>(s, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<byte> s, IFormatProvider? provider)
        where T : IUtf8SpanParsable<T> =>
        Into<T>(s, provider, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<byte> s, out bool success)
        where T : IUtf8SpanParsable<T> =>
        (success = T.TryParse(s, CultureInfo.InvariantCulture, out var result)) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<byte> s, IFormatProvider? provider, out bool success)
        where T : IUtf8SpanParsable<T> =>
        (success = T.TryParse(s, provider, out var result)) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<char> s)
        where T : ISpanParsable<T> =>
        Into<T>(s, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : ISpanParsable<T> =>
        Into<T>(s, provider, out _);

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<char> s, out bool success)
        where T : ISpanParsable<T> =>
        (success = T.TryParse(s, CultureInfo.InvariantCulture, out var result)) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Into<T>(this ReadOnlySpan<char> s, IFormatProvider? provider, out bool success)
        where T : ISpanParsable<T> =>
        (success = T.TryParse(s, provider, out var result)) ? result : default;
#endif

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryParse<T>(this string s)
        where T : struct =>
        Parse<T>(s, out var success) is var value && success ? value : null;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryParse<T>(this ReadOnlySpan<byte> s)
        where T : struct =>
        Parse<T>(s, out var success) is var value && success ? value : null;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryParse<T>(this ReadOnlySpan<char> s)
        where T : struct =>
        Parse<T>(s, out var success) is var value && success ? value : null;

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this string s)
        where T : struct, IParsable<T> =>
        T.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this string s, IFormatProvider? provider)
        where T : struct, IParsable<T> =>
        T.TryParse(s, provider, out var result) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this ReadOnlySpan<byte> s)
        where T : struct, IUtf8SpanParsable<T> =>
        T.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this ReadOnlySpan<byte> s, IFormatProvider? provider)
        where T : struct, IUtf8SpanParsable<T> =>
        T.TryParse(s, provider, out var result) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this ReadOnlySpan<char> s)
        where T : struct, ISpanParsable<T> =>
        T.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : default;

    /// <inheritdoc cref="Parse{T}(string, out bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? TryInto<T>(this ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : struct, ISpanParsable<T> =>
        T.TryParse(s, provider, out var result) ? result : default;
#endif

    static class FindTryParseFor<T>
    {
        [Pure]
        public delegate T? ByteParser(in ReadOnlySpan<byte> s, out bool success);

        [Pure]
        public delegate T? CharParser(in ReadOnlySpan<char> s, out bool success);

        [Pure]
        public delegate T? Parser(in string s, out bool success);

        [Pure]
        delegate bool InByteParser(ReadOnlySpan<byte> s, CultureInfo info, out T? result);

        [Pure]
        delegate bool InCharParser(ReadOnlySpan<char> s, CultureInfo info, out T? result);

        [Pure]
        delegate bool InNumberByteParser(ReadOnlySpan<byte> s, CultureInfo info, NumberStyles style, out T? result);

        [Pure]
        delegate bool InNumberCharParser(ReadOnlySpan<char> s, CultureInfo info, NumberStyles style, out T? result);

        [Pure]
        delegate bool InNumberParser(string s, CultureInfo info, NumberStyles style, out T? result);

        [Pure]
        delegate bool InParser(string s, CultureInfo info, out T? result);

        static readonly InByteParser? s_byteParse = Make<InByteParser>();

        static readonly InNumberByteParser? s_byteParseNumber = Make<InNumberByteParser>();

        static readonly InCharParser? s_charParse = Make<InCharParser>();

        static readonly InNumberCharParser? s_charParseNumber = Make<InNumberCharParser>();

        static readonly InParser? s_parse = Make<InParser>();

        static readonly InNumberParser? s_parseNumber = Make<InNumberParser>();

        public static Parser WithString { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            s_parseNumber is not null ? ParseNumberInvoker :
            s_parse is not null ? ParseInvoker : FailedParseInvoker;

        public static ByteParser WithByteSpan { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            s_byteParseNumber is not null ? ByteParseNumberInvoker :
            s_byteParse is not null ? ByteParseInvoker : ByteFailedParseInvoker;

        public static CharParser WithCharSpan { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            s_charParseNumber is not null ? CharParseNumberInvoker :
            s_charParse is not null ? CharParseInvoker : CharFailedParseInvoker;

        // ReSharper disable NullableWarningSuppressionIsUsed
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? ByteFailedParseInvoker(in ReadOnlySpan<byte> _, out bool b)
        {
            b = false;
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? ByteParseInvoker(in ReadOnlySpan<byte> s, out bool b)
        {
            b = s_byteParse!(s, CultureInfo.InvariantCulture, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? ByteParseNumberInvoker(in ReadOnlySpan<byte> s, out bool b)
        {
            b = s_byteParseNumber!(s, CultureInfo.InvariantCulture, NumberStyles.Any, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? CharFailedParseInvoker(in ReadOnlySpan<char> _, out bool b)
        {
            b = false;
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? CharParseInvoker(in ReadOnlySpan<char> s, out bool b)
        {
            b = s_charParse!(s, CultureInfo.InvariantCulture, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? CharParseNumberInvoker(in ReadOnlySpan<char> s, out bool b)
        {
            b = s_charParseNumber!(s, CultureInfo.InvariantCulture, NumberStyles.Any, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? FailedParseInvoker(in string _, out bool b)
        {
            b = false;
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? ParseInvoker(in string s, out bool b)
        {
            b = s_parse!(s, CultureInfo.InvariantCulture, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static T? ParseNumberInvoker(in string s, out bool b)
        {
            b = s_parseNumber!(s, CultureInfo.InvariantCulture, NumberStyles.Any, out var result);
            return result;
        }

        [Pure]
        static TDelegate? Make<TDelegate>()
            where TDelegate : Delegate => // ReSharper disable once NullableWarningSuppressionIsUsed
            typeof(TDelegate).GetMethod(nameof(Invoke))!.GetParameters() is var parameters &&
            Array.ConvertAll(parameters, x => x.ParameterType) is var types &&
            typeof(T).GetMethod(nameof(int.TryParse), types) is { } method
                ? Delegate.CreateDelegate(typeof(TDelegate), method) as TDelegate
                : null;
    }
}
