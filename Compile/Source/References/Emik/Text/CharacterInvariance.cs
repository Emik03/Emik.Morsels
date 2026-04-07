// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides extension methods for <see cref="char"/>.</summary>
static partial class CharacterInvariance
{
    extension(char c)
    {
        /// <inheritdoc cref="char.IsControl(char)"/>
        [Pure]
        public bool IsControl() => char.IsControl(c);

        /// <inheritdoc cref="char.IsDigit(char)"/>
        [Pure]
        public bool IsDigit() => char.IsDigit(c);

        /// <inheritdoc cref="char.IsHighSurrogate(char)"/>
        [Pure]
        public bool IsHighSurrogate() => char.IsHighSurrogate(c);

        /// <inheritdoc cref="char.IsLetter(char)"/>
        [Pure]
        public bool IsLetter() => char.IsLetter(c);

        /// <inheritdoc cref="char.IsLetterOrDigit(char)"/>
        [Pure]
        public bool IsLetterOrDigit() => char.IsLetterOrDigit(c);

        /// <inheritdoc cref="char.IsLower(char)"/>
        [Pure]
        public bool IsLower() => char.IsLower(c);

        /// <inheritdoc cref="char.IsLowSurrogate(char)"/>
        [Pure]
        public bool IsLowSurrogate() => char.IsLowSurrogate(c);
    }

    /// <inheritdoc cref="string.IsNullOrEmpty(string)"/>
    [Pure]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);
#if NETFRAMEWORK && !NET40_OR_GREATER
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
        value?.All(char.IsWhiteSpace) != false;
#else
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string)"/>
    [Pure]
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
#endif
    /// <param name="c">The character to convert.</param>
    extension(char c)
    {
        /// <inheritdoc cref="char.IsNumber(char)"/>
        [Pure]
        public bool IsNumber() => char.IsNumber(c);

        /// <inheritdoc cref="char.IsPunctuation(char)"/>
        [Pure]
        public bool IsPunctuation() => char.IsPunctuation(c);

        /// <inheritdoc cref="char.IsSeparator(char)"/>
        [Pure]
        public bool IsSeparator() => char.IsSeparator(c);

        /// <inheritdoc cref="char.IsSurrogate(char)"/>
        [Pure]
        public bool IsSurrogate() => char.IsSurrogate(c);

        /// <inheritdoc cref="char.IsSymbol(char)"/>
        [Pure]
        public bool IsSymbol() => char.IsSymbol(c);

        /// <inheritdoc cref="char.IsUpper(char)"/>
        public bool IsUpper() => char.IsUpper(c);

        /// <inheritdoc cref="char.IsWhiteSpace(char)"/>
        [Pure]
        public bool IsWhitespace() => char.IsWhiteSpace(c);

        /// <summary>Converts the character to the byte-equivalent, 0-9.</summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The parameter <paramref name="c"/> isn't between '0' and '9', inclusively on both ends.
        /// </exception>
        /// <returns>The number 0-9 representing the character.</returns>
        [Pure]
        public byte AsDigit() =>
            c is >= '0' and <= '9'
                ? (byte)(c - '0')
                : throw new ArgumentOutOfRangeException(nameof(c), c, "Character must be 0-9.");

        /// <summary>Attempts to convert the character to the byte-equivalent, 0-9.</summary>
        /// <returns>The number 0-9 representing the character, or <see langword="null"/>.</returns>
        [Pure]
        public byte? TryAsDigit() => c is >= '0' and <= '9' ? (byte)(c - '0') : null;

        /// <inheritdoc cref="char.ToLower(char)"/>
        [Pure]
        public char ToLower() => char.ToLowerInvariant(c);

        /// <inheritdoc cref="char.ToUpper(char)"/>
        [Pure]
        public char ToUpper() => char.ToUpperInvariant(c);

        /// <inheritdoc cref="char.GetNumericValue(char)"/>
        [Pure]
        public double GetNumericValue() => char.GetNumericValue(c);
    }

    extension(string s)
    {
        /// <inheritdoc cref="string.Trim(char[])"/>
        [Pure]
        public string Trim(string trim)
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
        public string TrimEnd(string trim)
        {
            for (var i = 1; i <= s.Length; i++)
                if (i > trim.Length || s[^i] != trim[^i])
                    return s[..^(i - 1)];

            return "";
        }

        /// <inheritdoc cref="string.TrimStart(char[])"/>
        [Pure]
        public string TrimStart(string trim)
        {
            for (var i = 0; i < s.Length; i++)
                if (i >= trim.Length || s[i] != trim[i])
                    return s[i..];

            return "";
        }
    }

    /// <inheritdoc cref="char.GetUnicodeCategory(char)"/>
    [Pure]
    public static UnicodeCategory GetUnicodeCategory(this char c) => char.GetUnicodeCategory(c);
}
