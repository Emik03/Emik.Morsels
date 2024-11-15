// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable CheckNamespace RedundantNameQualifier UseSymbolAlias
namespace Emik.Morsels;
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
using Substring = System.ReadOnlyMemory<char>;
#else
using Substring = string;
#endif

/// <summary>Provides methods to convert instances to a <see cref="string"/>.</summary>
static partial class StringFactory
{
    const RegexOptions Options = RegexOptions.Multiline | RegexOptions.Compiled;
#if NET8_0_OR_GREATER
    static readonly OnceMemoryManager<SearchValues<char>> s_slashes = new(SearchValues.Create(@"/\"));
#endif
#pragma warning disable MA0110, SYSLIB1045
    static readonly Regex
        s_angles = new("<(?>(?:<(?<A>)|>(?<-A>)|[^<>]+){2,})>", Options),
        s_curlies = new("{(?>(?:{(?<A>)|}(?<-A>)|[^{}]+){2,})}", Options),
        s_singleQuotes = new(@"'(?>(?:{(?<A>)|}(?<-A>)|[^""]+){2,})'", Options),
        s_doubleQuotes = new(@"""(?>(?:{(?<A>)|}(?<-A>)|[^""]+){2,})""", Options),
        s_brackets = new(@"\[(?>(?:\[(?<A>)|\](?<-A>)|[^\[\]]+){2,})\]", Options),
        s_parentheses = new(@"\((?>(?:\((?<A>)|\)(?<-A>)|[^()]+){2,})\)", Options);
#pragma warning restore MA0110, SYSLIB1045
    /// <summary>Creates the collapsed form of the string.</summary>
    /// <param name="s">The string to collapse.</param>
    /// <returns>The collapsed string.</returns>
    public static string Collapse(this string s)
    {
        s = s_parentheses.Replace(s, "(…)");
        s = s_brackets.Replace(s, "[…]");
        s = s_curlies.Replace(s, "{…}");
        s = s_angles.Replace(s, "<…>");
        s = s_singleQuotes.Replace(s, "'…'");
        return s_doubleQuotes.Replace(s, "\"…\"");
    }

    /// <summary>Collapses the <see cref="string"/> to a single line.</summary>
    /// <param name="expression">The <see cref="string"/> to collapse.</param>
    /// <param name="prefix">The prefix to use.</param>
    /// <returns>The collapsed <see cref="string"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(expression))]
    public static string? CollapseToSingleLine(this string? expression, string? prefix = null)
    {
        // ReSharper disable once RedundantUnsafeContext
        static unsafe StringBuilder Accumulator(StringBuilder accumulator, scoped ReadOnlySpan<char> next)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            =>
                accumulator.Append(next.Trim());
#else
        {
            var trimmed = next.Trim();
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            fixed (char* ptr = trimmed)
                accumulator.Append(trimmed.Align(ptr), trimmed.Length);
#else
            foreach (var t in trimmed)
                accumulator.Append(t);
#endif
            return accumulator;
        }
#endif
        return expression?.Collapse().SplitSpanLines().Aggregate(prefix.ToBuilder(), Accumulator).Trim().ToString();
    }

    /// <summary>Converts a number to an ordinal.</summary>
    /// <param name="i">The number to convert.</param>
    /// <param name="one">The string for the value 1 or -1.</param>
    /// <param name="many">The string to concatenate. Use prefixed dashes to trim <paramref name="one"/>.</param>
    /// <returns>The conjugation of all the parameters.</returns>
    [Pure]
    public static string Conjugate(this int i, string one, string many = "s") =>
        i is not 1 and not -1 &&
#if NET7_0_OR_GREATER
        (many.AsSpan().IndexOfAnyExcept('-') is not -1 and var found ? found : 0)
#else
        Math.Min(many.TakeWhile(x => x is '-').Count(), one.Length)
#endif // ReSharper disable once BadPreprocessorIndent
        is var trim
            ? $"{i} {one[..^trim]}{many[trim..]}"
            : $"{i} {one}";
#if NET7_0_OR_GREATER
    /// <inheritdoc cref="Conjugate(int, string, string)"/>
    [Pure]
    public static string Conjugate<T>(this T i, string one, string many = "s")
        where T : INumberBase<T>, IComparisonOperators<T, T, bool> =>
        (T.IsZero(i) || T.Abs(i) > T.One) && (many.AsSpan().IndexOfAnyExcept('-') is not -1 and var f ? f : 0) is var tr
            ? $"{i} {one[..^tr]}{many[tr..]}"
            : $"{i} {one}";
#endif
    /// <summary>Extracts the file name from the path.</summary>
    /// <remarks><para>
    /// The return type depends on what framework is used. Ensure that the caller doesn't care about the return type.
    /// </para></remarks>
    /// <param name="path">The path to extract the file name from.</param>
    /// <returns>The file name.</returns>
    [Pure]
#if !ROSLYN && !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP2_1_OR_GREATER
    // ReSharper disable once RedundantNullableFlowAttribute
    [return: NotNullIfNotNull(nameof(path))]
#endif
    public static Substring FileName(this string? path) =>
        path is null
#if NET8_0_OR_GREATER
            ? default
            : path.SplitOn(s_slashes).Last.Trim();
#elif ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            ? default
            : path.SplitOnAny(@"/\".AsMemory()).Last.Trim();
#else
            ? "" // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            : Path.GetFileName(path).Trim() ?? "";
#endif
    /// <summary>Creates the prettified form of the string.</summary>
    /// <param name="s">The string to prettify.</param>
    /// <returns>The prettified string.</returns>
    public static string Prettify(this string s) => Prettify(s, separator: ",;");

    /// <summary>Creates the prettified form of the string.</summary>
    /// <remarks><para>
    /// The functionality is based on
    /// <a href="https://gist.github.com/kodo-pp/89cefb17a8772cd9fd7b875d94fd29c7">this gist by kodo-pp</a>.
    /// </para></remarks>
    /// <param name="s">The string to prettify.</param>
    /// <param name="start">The characters considered to be starting blocks.</param>
    /// <param name="end">The characters considered to be ending blocks.</param>
    /// <param name="separator">The characters considered to be separators.</param>
    /// <param name="indent">The amount of spaces for indentation.</param>
    /// <returns>The prettified string.</returns>
    public static string Prettify(
        this string s, // ReSharper disable once MethodOverloadWithOptionalParameter
        string start = "([{<",
        string end = ")]}>",
        string separator = ",;",
        string indent = "    "
    )
    {
        var seen = false;
        var nest = 0;
        StringBuilder sb = new();

        for (var i = 0; i < s.Length; i++)
            (seen, nest, sb) = s[i] switch
            {
                not ' ' when seen && sb.Indent(indent, nest) is var _ && (seen = false) => throw Unreachable,
                _ when start.Contains(s[i]) && (s.Nth(i + 1) is not { } next || !end.Contains(next)) =>
                    (seen, ++nest, sb.Append(s[i]).Indent(indent, nest)),
                _ when end.Contains(s[i]) && (s.Nth(i - 1) is not { } prev || !start.Contains(prev)) =>
                    (seen, --nest, sb.Indent(indent, nest).Append(s[i])),
                _ when separator.Contains(s[i]) => (true, nest, sb.Append(s[i])),
                ' ' when seen && nest > 0 ||
                    s.Nth(i - 1) is { } prev && start.Contains(prev) ||
                    s.Nth(i + 1) is { } next && end.Contains(next) => (seen, nest, sb),
                _ => (seen, nest, sb.Append(s[i])),
            };

        return $"{sb}";
    }
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>Concatenates an enumeration of <see cref="char"/> into a <see cref="string"/>.</summary>
    /// <remarks><para>
    /// This method is more efficient than using <see cref="Conjoin{T}(IEnumerable{T}, string)"/>
    /// for <see cref="char"/> enumerations.
    /// </para></remarks>
    /// <param name="chars">The enumeration of characters.</param>
    /// <returns>A <see cref="string"/> built from concatenating <paramref name="chars"/>.</returns>
    [Pure]
    public static string Concat([InstantHandle] this IEnumerable<char> chars) => string.Concat(chars);
#endif
    /// <summary>Joins a set of values into one long <see cref="string"/>.</summary>
    /// <remarks><para>
    /// This method is more efficient than using
    /// <see cref="Conjoin{T}(IEnumerable{T}, string)"/> for <see cref="char"/> separators.
    /// </para></remarks>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>One long <see cref="string"/>.</returns>
    // ReSharper disable BadPreprocessorIndent
    [Pure]
    public static string Conjoin<T>([InstantHandle] this IEnumerable<T> values, char separator) =>
        $"{new StringBuilder().AppendMany(values, separator)}";

    /// <summary>Joins a set of values into one long <see cref="string"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>One long <see cref="string"/>.</returns>
    [Pure]
    public static string Conjoin<T>([InstantHandle] this IEnumerable<T> values, string separator = ", ") =>
        $"{new StringBuilder().AppendMany(values, separator)}";

    /// <summary>Converts the <see cref="Stopwatch"/> to its concise <see cref="string"/> representation.</summary>
    /// <param name="stopwatch">The <see cref="Stopwatch"/> to convert.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="stopwatch"/>.</returns>
    [Pure]
    public static string ToConciseString(this Stopwatch? stopwatch) =>
        stopwatch is null ? "0" : ToConciseString(stopwatch.Elapsed);

    /// <summary>Converts the <see cref="TimeSpan"/> to its concise <see cref="string"/> representation.</summary>
    /// <param name="time">The <see cref="TimeSpan"/> to convert.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="time"/>.</returns>
    [Pure]
    public static string ToConciseString(this TimeSpan time)
    {
        var sign = time.Ticks < 0 ? "-" : "";
        var ticks = Math.Abs(time.Ticks);

        return ticks switch
        {
            0 => "0",
            >= TimeSpan.TicksPerDay * 7 => $"{sign}{ticks / TimeSpan.TicksPerDay}d",
            >= TimeSpan.TicksPerDay => $"{sign}{ticks / TimeSpan.TicksPerDay
            }d{ticks % TimeSpan.TicksPerDay / TimeSpan.TicksPerHour
            }h",
            >= TimeSpan.TicksPerHour => $"{sign}{ticks / TimeSpan.TicksPerHour
            }h{ticks % TimeSpan.TicksPerHour / TimeSpan.TicksPerMinute
            }m{ticks % TimeSpan.TicksPerMinute / TimeSpan.TicksPerSecond}s",
            >= TimeSpan.TicksPerMinute => $"{sign}{ticks / TimeSpan.TicksPerMinute
            }m{ticks % TimeSpan.TicksPerMinute / TimeSpan.TicksPerSecond}s",
            >= TimeSpan.TicksPerSecond => $"{sign}{Math.Round(ticks / (double)TimeSpan.TicksPerSecond, 1)}s",
            >= TimeSpan.TicksPerMillisecond => $"{sign}{Math.Round(ticks / (double)TimeSpan.TicksPerMillisecond, 1)}ms",
            _ => $"{sign}{ticks / 10.0}µs",
        };
    }
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
    /// <summary>Converts the value to a hex <see cref="string"/>.</summary>
    /// <remarks><para>The implementation is based on
    /// <a href="https://github.com/CommunityToolkit/dotnet/blob/7b53ae23dfc6a7fb12d0fc058b89b6e948f48448/src/CommunityToolkit.Diagnostics/Extensions/ValueTypeExtensions.cs#L44">
    /// CommunityToolkit.Diagnostics.ValueTypeExtensions.ToHexString
    /// </a>.
    /// </para></remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>The hex <see cref="string"/>.</returns>
    [Pure]
    public static unsafe string ToHexString<T>(this T value)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
#pragma warning disable 8500
    {
        var p = stackalloc char[Unsafe.SizeOf<T>() * 2];
        p[0] = '0';
        p[1] = 'x';

        fixed (char* rh = "0123456789ABCDEF")
            for (int i = 0, j = Unsafe.SizeOf<T>() * 2; i < Unsafe.SizeOf<T>(); i++, j -= 2)
            {
                var b = ((byte*)&value)[i];
                var low = b & 0x0f;
                var high = (b & 0xf0) >> 4;
                p[j + 1] = *(rh + low);
                p[j] = *(rh + high);
            }

        return new(p, 0, Unsafe.SizeOf<T>() * 2 + 2);
    }
#pragma warning restore 8500
#endif
#if NET6_0_OR_GREATER
    /// <summary>Appends an enumeration onto the <see cref="DefaultInterpolatedStringHandler"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="dish">
    /// The <see cref="DefaultInterpolatedStringHandler"/> to mutate and <see langword="return"/>.
    /// </param>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>The parameter <paramref name="dish"/>.</returns>
    public static DefaultInterpolatedStringHandler AppendMany<T>(
        this ref DefaultInterpolatedStringHandler dish,
        [InstantHandle] IEnumerable<T> values,
        char separator
    )
    {
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            dish.AppendFormatted(enumerator.Current);
        else
            return dish;

        while (enumerator.MoveNext())
        {
            dish.AppendFormatted(separator);
            dish.AppendFormatted(enumerator.Current);
        }

        return dish;
    }

    /// <summary>Appends an enumeration onto the <see cref="DefaultInterpolatedStringHandler"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="dish">
    /// The <see cref="DefaultInterpolatedStringHandler"/> to mutate and <see langword="return"/>.
    /// </param>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>The parameter <paramref name="dish"/>.</returns>
    public static DefaultInterpolatedStringHandler AppendMany<T>(
        this ref DefaultInterpolatedStringHandler dish,
        [InstantHandle] IEnumerable<T> values,
        string separator = ", "
    )
    {
        if (separator is "")
            switch (values)
            {
                case char[] x:
                    dish.AppendFormatted(x);
                    return dish;
                case string x:
                    dish.AppendFormatted(x);
                    return dish;
            }

        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            dish.AppendFormatted(enumerator.Current);
        else
            return dish;

        while (enumerator.MoveNext())
        {
            dish.AppendLiteral(separator);
            dish.AppendFormatted(enumerator.Current);
        }

        return dish;
    }
#endif
    /// <summary>Appends an enumeration onto the <see cref="StringBuilder"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="builder">The <see cref="StringBuilder"/> to mutate and <see langword="return"/>.</param>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>The parameter <paramref name="builder"/>.</returns>
    public static StringBuilder AppendMany<T>(
        this StringBuilder builder,
        [InstantHandle] IEnumerable<T> values,
        char separator
    )
    {
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return builder;

        while (enumerator.MoveNext())
            builder.Append(separator).Append(enumerator.Current);

        return builder;
    }

    /// <summary>Appends an enumeration onto the <see cref="StringBuilder"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="builder">The <see cref="StringBuilder"/> to mutate and <see langword="return"/>.</param>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>The parameter <paramref name="builder"/>.</returns>
    public static StringBuilder AppendMany<T>(
        this StringBuilder builder,
        [InstantHandle] IEnumerable<T> values,
        string separator = ", "
    )
    {
        if (separator is "")
            switch (values)
            {
                case char[] x: return builder.Append(x);
                case string x: return builder.Append(x);
            }

        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return builder;

        while (enumerator.MoveNext())
            builder.Append(separator).Append(enumerator.Current);

        return builder;
    }

    [MustUseReturnValue]
    static StringBuilder Indent(this StringBuilder sb, string indent, int nest)
    {
        sb.AppendLine();

        for (var i = 0; i < nest && nest >= 0; i++)
            sb.Append(indent);

        return sb;
    }
}
#endif
