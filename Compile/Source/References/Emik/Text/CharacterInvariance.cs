// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides extension methods for <see cref="char"/>.</summary>
static partial class CharacterInvariance
{
    /// <inheritdoc cref="char.IsControl(char)"/>
    [Pure]
    public static bool IsControl(this char c) => char.IsControl(c);

    /// <inheritdoc cref="char.IsDigit(char)"/>
    [Pure]
    public static bool IsDigit(this char c) => char.IsDigit(c);

    /// <inheritdoc cref="char.IsHighSurrogate(char)"/>
    [Pure]
    public static bool IsHighSurrogate(this char c) => char.IsHighSurrogate(c);

    /// <inheritdoc cref="char.IsLetter(char)"/>
    [Pure]
    public static bool IsLetter(this char c) => char.IsLetter(c);

    /// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
    [Pure]
    public static bool IsLetterOrDigit(this char c) => char.IsLetterOrDigit(c);

    /// <inheritdoc cref="char.IsLower(char)"/>
    [Pure]
    public static bool IsLower(this char c) => char.IsLower(c);

    /// <inheritdoc cref="char.IsLowSurrogate(char)"/>
    [Pure]
    public static bool IsLowSurrogate(this char c) => char.IsLowSurrogate(c);

    /// <inheritdoc cref="string.IsNullOrEmpty(string)"/>
    [Pure]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

#if NET35
    /// <summary>
    /// Indicates whether a specified string is <see langword="null"/>,
    /// empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter is <see langword="null"/>,
    /// or <see cref="string.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.
    /// </returns>
    [Pure]
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value) =>
        value is null || value.All(char.IsWhiteSpace);
#elif !NET20 && !NET30
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string)"/>
    [Pure]
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
#endif

    /// <inheritdoc cref="char.IsNumber(char)"/>
    [Pure]
    public static bool IsNumber(this char c) => char.IsNumber(c);

    /// <inheritdoc cref="char.IsPunctuation(char)"/>
    [Pure]
    public static bool IsPunctuation(this char c) => char.IsPunctuation(c);

    /// <inheritdoc cref="char.IsSeparator(char)"/>
    [Pure]
    public static bool IsSeparator(this char c) => char.IsSeparator(c);

    /// <inheritdoc cref="char.IsSurrogate(char)"/>
    [Pure]
    public static bool IsSurrogate(this char c) => char.IsSurrogate(c);

    /// <inheritdoc cref="char.IsSymbol(char)"/>
    [Pure]
    public static bool IsSymbol(this char c) => char.IsSymbol(c);

    /// <inheritdoc cref="char.IsUpper(char)"/>
    public static bool IsUpper(this char c) => char.IsUpper(c);

    /// <inheritdoc cref="char.IsWhiteSpace(char)"/>
    [Pure]
    public static bool IsWhitespace(this char c) => char.IsWhiteSpace(c);

    /// <summary>Converts the character to the byte-equivalent, 0-9.</summary>
    /// <param name="c">The character to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="c"/> isn't between '0' and '9', inclusively on both ends.
    /// </exception>
    /// <returns>The number 0-9 representing the character.</returns>
    [Pure]
    public static byte AsDigit(this char c) =>
        c is >= '0' and <= '9'
            ? (byte)(c - '0')
            : throw new ArgumentOutOfRangeException(nameof(c), c, "Character must be 0-9.");

    /// <summary>Attempts to convert the character to the byte-equivalent, 0-9.</summary>
    /// <param name="c">The character to convert.</param>
    /// <returns>The number 0-9 representing the character, or <see langword="null"/>.</returns>
    [Pure]
    public static byte? TryAsDigit(this char c) => c is >= '0' and <= '9' ? (byte)(c - '0') : null;

    /// <inheritdoc cref="char.ToLower(char)"/>
    [Pure]
    public static char ToLower(this char c) => char.ToLowerInvariant(c);

    /// <inheritdoc cref="char.ToUpper(char)"/>
    [Pure]
    public static char ToUpper(this char c) => char.ToUpperInvariant(c);

    /// <inheritdoc cref="char.GetNumericValue(char)"/>
    [Pure]
    public static double GetNumericValue(this char c) => char.GetNumericValue(c);

    /// <inheritdoc cref="string.Trim(char[])"/>
    [Pure]
    public static string Trim(this string s, string trim)
    {
        int start = 0, end = 1;

        for (; start < s.Length; start++)
            if (start >= trim.Length || s[start] != trim[start])
                break;

        for (; end <= s.Length; end++)
            if (end > trim.Length || s[^end] != trim[^end])
                return s[..^(end - 1)];

        return s[start..^end];
    }

    /// <inheritdoc cref="string.TrimEnd(char[])"/>
    [Pure]
    public static string TrimEnd(this string s, string trim)
    {
        for (var i = 1; i <= s.Length; i++)
            if (i > trim.Length || s[^i] != trim[^i])
                return s[..^(i - 1)];

        return "";
    }

    /// <inheritdoc cref="string.TrimStart(char[])"/>
    [Pure]
    public static string TrimStart(this string s, string trim)
    {
        for (var i = 0; i < s.Length; i++)
            if (i >= trim.Length || s[i] != trim[i])
                return s[i..];

        return "";
    }

    /// <inheritdoc cref="char.GetUnicodeCategory(char)"/>
    [Pure]
    public static UnicodeCategory GetUnicodeCategory(this char c) => char.GetUnicodeCategory(c);
}
