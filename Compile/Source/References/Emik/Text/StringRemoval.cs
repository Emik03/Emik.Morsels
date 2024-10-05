// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using Range = System.Range;

/// <summary>Provides extension methods for <see cref="char"/>.</summary>
static partial class StringRemoval
{
    /// <summary>Removes the single character based on the index from the langword="string"/>.</summary>
    /// <param name="str">The builder to take the character from.</param>
    /// <param name="index">The index to remove.</param>
    /// <param name="popped">The resulting character that was removed, or <see langword="default"/>.</param>
    /// <returns>The parameter <paramref name="str"/>.</returns>
    public static string Pop(this string str, int index, out char popped)
    {
        if (index >= 0 && index < str.Length)
        {
            popped = str[index];
            return str.Remove(index, 1);
        }

        popped = default;
        return str;
    }

    /// <inheritdoc cref="Pop(StringBuilder, int, out char)"/>
    public static string Pop(this string str, Index index, out char popped) =>
        str.Pop(index.GetOffset(str.Length), out popped);

    /// <summary>Removes the substring based on the range from the langword="string"/>.</summary>
    /// <param name="str">The builder to take the character from.</param>
    /// <param name="range">The range to remove.</param>
    /// <param name="popped">The resulting character that was removed, or <see langword="default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="range"/> is out of range when indexing the parameter <paramref name="str"/>.
    /// </exception>
    /// <returns>The parameter <paramref name="str"/>.</returns>
    public static string Pop(this string str, Range range, out string popped)
    {
        range.GetOffsetAndLength(str.Length, out var startIndex, out var length);
        popped = str[range];
        return str.Remove(startIndex, length);
    }

    /// <summary>Removes the substring based on the range from the <see langword="string"/>.</summary>
    /// <param name="str">The builder to take the character from.</param>
    /// <param name="range">The range to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="range"/> is out of range when indexing the parameter <paramref name="str"/>.
    /// </exception>
    /// <returns>The parameter <paramref name="str"/>.</returns>
    public static string Remove(this string str, Range range)
    {
        range.GetOffsetAndLength(str.Length, out var startIndex, out var length);
        return str.Remove(startIndex, length);
    }

    /// <summary>Removes the single character based on the index from the <see cref="StringBuilder"/>.</summary>
    /// <param name="builder">The builder to take the character from.</param>
    /// <param name="index">The index to remove.</param>
    /// <param name="popped">The resulting character that was removed, or <see langword="default"/>.</param>
    /// <returns>The parameter <paramref name="builder"/>.</returns>
    public static StringBuilder Pop(this StringBuilder builder, int index, out char popped)
    {
        if (index >= 0 && index < builder.Length)
        {
            popped = builder[index];
            return builder.Remove(index, 1);
        }

        popped = default;
        return builder;
    }

    /// <inheritdoc cref="Pop(StringBuilder, int, out char)"/>
    public static StringBuilder Pop(this StringBuilder builder, Index index, out char popped) =>
        builder.Pop(index.GetOffset(builder.Length), out popped);

    /// <summary>Removes the substring based on the range from the <see cref="StringBuilder"/>.</summary>
    /// <param name="builder">The builder to take the character from.</param>
    /// <param name="range">The range to remove.</param>
    /// <param name="popped">The resulting character that was removed, or <see langword="default"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="range"/> is out of range when indexing the parameter <paramref name="builder"/>.
    /// </exception>
    /// <returns>The parameter <paramref name="builder"/>.</returns>
    public static StringBuilder Pop(this StringBuilder builder, Range range, out string popped)
    {
        range.GetOffsetAndLength(builder.Length, out var startIndex, out var length);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        popped = string.Create(
            length,
            (builder, startIndex),
            static (span, tuple) =>
            {
                var (builder, startIndex) = tuple;

                for (var i = 0; i < span.Length; i++)
                    span[i] = builder[i + startIndex];
            }
        );
#else
        StringBuilder poppedBuilder = new(length);

        for (var i = 0; i < length; i++)
            poppedBuilder[i] = builder[startIndex + i];

        popped = $"{poppedBuilder}";
#endif
        return builder.Remove(startIndex, length);
    }

    /// <summary>Removes the substring based on the range from the <see cref="StringBuilder"/>.</summary>
    /// <param name="builder">The builder to take the character from.</param>
    /// <param name="range">The range to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="range"/> is out of range when indexing the parameter <paramref name="builder"/>.
    /// </exception>
    /// <returns>The parameter <paramref name="builder"/>.</returns>
    public static StringBuilder Remove(this StringBuilder builder, Range range)
    {
        range.GetOffsetAndLength(builder.Length, out var startIndex, out var length);
        return builder.Remove(startIndex, length);
    }

    /// <summary>Creates the <see cref="StringBuilder"/> around the provided <see cref="string"/>.</summary>
    /// <param name="str">The <see cref="string"/> to create the <see cref="StringBuilder"/> around.</param>
    /// <returns>The <see cref="StringBuilder"/> of the parameter <paramref name="str"/>.</returns>
    [Pure]
    public static StringBuilder ToBuilder(this string? str) => new(str);
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="string.Trim()"/>
    public static Memory<char> Trim(this Memory<char> memory) => memory.TrimStart().TrimEnd();

    /// <inheritdoc cref="string.Trim()"/>
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> memory) => memory.TrimStart().TrimEnd();

    /// <inheritdoc cref="string.TrimStart(char[])"/>
    public static Memory<char> TrimStart(this Memory<char> memory)
    {
        var span = memory.Span;

        for (var i = 0; i < span.Length; i++)
            if (!char.IsWhiteSpace(span[i]))
                return memory[..i];

        return default;
    }

    /// <inheritdoc cref="string.TrimStart(char[])"/>
    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> memory)
    {
        var span = memory.Span;

        for (var i = 0; i < span.Length; i++)
            if (!char.IsWhiteSpace(span[i]))
                return memory[..i];

        return default;
    }

    /// <inheritdoc cref="string.TrimEnd(char[])"/>
    public static Memory<char> TrimEnd(this Memory<char> memory)
    {
        var span = memory.Span;

        for (var i = span.Length - 1; i >= 0; i--)
            if (!char.IsWhiteSpace(span[i]))
                return memory[i..];

        return default;
    }

    /// <inheritdoc cref="string.TrimEnd(char[])"/>
    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> memory)
    {
        var span = memory.Span;

        for (var i = span.Length - 1; i >= 0; i--)
            if (!char.IsWhiteSpace(span[i]))
                return memory[i..];

        return default;
    }
#endif
    /// <inheritdoc cref="string.Trim()"/>
    public static StringBuilder Trim(this StringBuilder builder) => builder.TrimStart().TrimEnd();

    /// <inheritdoc cref="string.TrimEnd(char[])"/>
    public static StringBuilder TrimEnd(this StringBuilder builder)
    {
        for (var i = builder.Length - 1; i >= 0; i--)
            if (!char.IsWhiteSpace(builder[i]))
                return builder.Remove(i + 1, builder.Length - i - 1);

        return builder.Remove(0, builder.Length);
    }

    /// <inheritdoc cref="string.TrimStart(char[])"/>
    public static StringBuilder TrimStart(this StringBuilder builder)
    {
        for (var i = 0; i < builder.Length; i++)
            if (!char.IsWhiteSpace(builder[i]))
                return builder.Remove(0, i);

        return builder.Remove(0, builder.Length);
    }
}
