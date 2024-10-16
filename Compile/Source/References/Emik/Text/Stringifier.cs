// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract CheckNamespace RedundantNameQualifier RedundantNullableFlowAttribute RedundantUsingDirective UseSymbolAlias
#if WAWA
namespace Wawa.Modules;
#else
namespace Emik.Morsels;
#endif

using Enum = System.Enum;

#if !(NET20 || NET30)
using static System.Linq.Expressions.Expression;
using Expression = System.Linq.Expressions.Expression;
#endif
using FieldInfo = System.Reflection.FieldInfo;

/// <summary>Provides stringification methods.</summary>
// ReSharper disable once BadPreprocessorIndent
#if WAWA
public
#endif
static partial class Stringifier
{
    const int MaxIteration = 32, MaxRecursion = 3;

    unsafe delegate nuint VoidPointer(void* v);
#if !WAWA
    const RegexOptions Options = RegexOptions.Multiline | RegexOptions.Compiled;
#endif // ReSharper disable UnusedMember.Local
    const string
        Apology = "I am so sorry that you have to deal with double pointers, but this cannot be supported.",
        BitFlagSeparator = " | ",
        Else = "th",
        EqualityContract = nameof(EqualityContract),
        False = "false",
        FirstOrd = "st",
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        HexCharactersTable = "0123456789ABCDEF",
#endif
        Invalid = $"!<{nameof(InvalidOperationException)}>",
        KeyValueSeparator = ": ",
        Null = "null",
        SecondOrd = "nd",
        Separator = ", ",
        Slashes = @"/\",
        ThirdOrd = "rd",
        True = "true",
        Unsupported = $"!<{nameof(NotSupportedException)}>",
        UnsupportedPlatform = $"!<{nameof(PlatformNotSupportedException)}>";
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static readonly Dictionary<Type, bool>
#if !WAWA
        s_fullyUnmanaged = [],
#endif
        s_hasMethods = [];

    static readonly Dictionary<Type, Delegate> s_stringifiers = [];
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static readonly Dictionary<Type, string> s_unfoldedNames = new()
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(char)] = "char",
        [typeof(decimal)] = "decimal",
        [typeof(double)] = "double",
        [typeof(float)] = "float",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(nint)] = "nint",
        [typeof(nuint)] = "nuint",
        [typeof(object)] = "object",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(string)] = "string",
        [typeof(uint)] = "uint",
        [typeof(ulong)] = "ulong",
        [typeof(ushort)] = "ushort",
        [typeof(void)] = "void",
    };
#endif
    static readonly ConstantExpression
        s_exEmpty = Constant(""),
#if !NETFRAMEWORK || NET40_OR_GREATER
        s_exInvalid = Constant(Invalid),
        s_exUnsupported = Constant(Unsupported),
        s_exUnsupportedPlatform = Constant(UnsupportedPlatform),
#endif
        s_exSeparator = Constant(Separator),
        s_exTrue = Constant(true);

    static readonly unsafe MethodInfo s_readVoidPointer = ((VoidPointer)ReadVoidPointer).Method;

    static readonly MethodInfo // ReSharper disable NullableWarningSuppressionIsUsed
        s_boolStringify = ((Func<bool, int, bool, string>)Stringify).Method,
        s_combine = ((Func<string, string, string>)string.Concat).Method,
        s_readPointer = s_boolStringify.DeclaringType!
           .GetMethod(nameof(ReadPointer), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!
           .GetGenericMethodDefinition(),
        s_stringify = s_boolStringify.GetGenericMethodDefinition();
#endif
#if !WAWA
#if NET8_0_OR_GREATER
    static readonly OnceMemoryManager<SearchValues<char>> s_slashes = new(SearchValues.Create(Slashes));
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
#endif
    /// <summary>Gets the field count of the version.</summary>
    /// <param name="version">The <see cref="Version"/> to use.</param>
    /// <returns>The field count of the parameter <paramref name="version"/>.</returns>
    [Pure]
    public static int FieldCount(
#if !WAWA
        this
#endif // ReSharper disable once BadPreprocessorIndent
            Version? version
    ) =>
        version switch
        {
            (_, <= 0, <= 0, <= 0) => 1,
            (_, _, <= 0, <= 0) => 2,
            (_, _, _, <= 0) => 3,
            _ => 4,
        };
#if !WAWA
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
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        // ReSharper disable once RedundantUnsafeContext
        static unsafe StringBuilder Accumulator(StringBuilder accumulator, scoped ReadOnlySpan<char> next)
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            =>
                accumulator.Append(next.Trim());
#else
        {
            var trimmed = next.Trim();

            fixed (char* ptr = &trimmed[0])
                accumulator.Append(ptr, trimmed.Length);

            return accumulator;
        }
#endif

        return expression?.Collapse()
           .SplitSpanLines()
           .Aggregate(prefix.ToBuilder(), Accumulator)
           .Trim()
           .ToString();
#else
        return expression
          ?.Collapse() // ReSharper disable once RedundantCast
           .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            // ReSharper disable once RedundantSuppressNullableWarningExpression
           .Select(x => x.Trim())!
           .Prepend(prefix)
           .Conjoin("");
#endif
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
    [return: NotNullIfNotNull(nameof(path))]
#endif
    public static
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        ReadOnlyMemory<char>
#else
        string
#endif
        FileName(this string? path) =>
        path is null
#if NET8_0_OR_GREATER
            ? default
            : path.SplitOn(s_slashes).Last.Trim();
#elif ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            ? default
            : path.SplitOnAny(Slashes.AsMemory()).Last.Trim();
#else
            ? "" // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            : Path.GetFileName(path).Trim() ?? "";
#endif
    /// <summary>Extracts the file name from the path.</summary>
    /// <remarks><para>
    /// The return type depends on what framework is used. Ensure that the caller doesn't care about the return type.
    /// </para></remarks>
    /// <param name="path">The path to extract the file name from.</param>
    /// <returns>The file name.</returns>
    [Pure]
#if !ROSLYN && !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP2_1_OR_GREATER
    [return: NotNullIfNotNull(nameof(path))]
#endif
    public static
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        ReadOnlyMemory<char>
#else
        string
#endif
        UntrimmedFileName(this string? path) =>
        path is null
#if NET8_0_OR_GREATER
            ? default
            : path.SplitOn(s_slashes).Last;
#elif ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            ? default
            : path.SplitOnAny(Slashes.AsMemory()).Last;
#else
            ? ""
            : Path.GetFileName(path) ?? "";
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
#endif
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
    public static string Conjoin<T>(
        [InstantHandle]
#if !WAWA
        this
#endif
            IEnumerable<T> values,
        char separator
    ) =>
        $"{new StringBuilder().AppendMany(values, separator)}";

    /// <summary>Joins a set of values into one long <see cref="string"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>One long <see cref="string"/>.</returns>
    [Pure]
    public static string Conjoin<T>(
        [InstantHandle]
#if !WAWA
        this
#endif
            IEnumerable<T> values,
        string separator = Separator
    ) =>
        $"{new StringBuilder().AppendMany(values, separator)}";

    /// <summary>Converts the <see cref="Stopwatch"/> to its concise <see cref="string"/> representation.</summary>
    /// <param name="stopwatch">The <see cref="Stopwatch"/> to convert.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="stopwatch"/>.</returns>
    [Pure]
    public static string ToConciseString(
#if !WAWA
        this
#endif
            Stopwatch? stopwatch
    ) =>
        stopwatch is null ? "0" : ToConciseString(stopwatch.Elapsed);

    /// <summary>Converts the <see cref="TimeSpan"/> to its concise <see cref="string"/> representation.</summary>
    /// <param name="time">The <see cref="TimeSpan"/> to convert.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="time"/>.</returns>
    [Pure]
    public static string ToConciseString(
#if !WAWA
        this
#endif
        TimeSpan time
    )
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

    /// <summary>Converts a <see cref="Pointer"/> to a <see cref="string"/>.</summary>
    /// <param name="value">The <see cref="Pointer"/> to convert.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="value"/>.</returns>
    [CLSCompliant(false), Pure]
    public static unsafe string ToHexString(
#if !WAWA
        this
#endif
            Pointer? value
    ) =>
        (value is null ? 0 : (nuint)Pointer.Unbox(value)).ToHexString();

    /// <summary>Gets the short display form of the version.</summary>
    /// <param name="version">The <see cref="Version"/> to convert.</param>
    /// <returns>The full name of the parameter <paramref name="version"/>.</returns>
    [Pure]
    public static string ToShortString(
#if !WAWA
        this
#endif
            Version? version
    )
    {
        if (version is not var (major, minor, build, revision) ||
            major <= 0 && minor <= 0 && build <= 0 && revision <= 0)
            return "v0";

        var length = Length(major, revision, minor, build);

        Span<char> span = stackalloc char[length];
        Format(span, version);
        return span.ToString();
    }
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
#if !WAWA
    /// <summary>Gets the full type name, with its generics extended.</summary>
    /// <param name="type">The <see cref="Type"/> to get the full name of.</param>
    /// <returns>The full name of the parameter <paramref name="type"/>.</returns>
    [Pure]
    public static string UnfoldedFullName(this Type? type) =>
        type is null ? Null :
        s_unfoldedNames.TryGetValue(type, out var val) ? val :
        s_unfoldedNames[type] = $"{type.UnfoldedName(new(), x => x.FullName)}";
#endif
    /// <summary>Gets the type name, with its generics extended.</summary>
    /// <param name="type">The <see cref="Type"/> to get the name of.</param>
    /// <returns>The name of the parameter <paramref name="type"/>.</returns>
    [Pure]
    public static string UnfoldedName(
#if !WAWA
        this
#endif
            Type? type
    ) =>
        type is null ? Null :
        s_unfoldedNames.TryGetValue(type, out var val) ? val :
        s_unfoldedNames[type] = $"{type.UnfoldedName(new(), x => x.Name)}";
#endif
    /// <summary>Converts a number to an ordinal.</summary>
    /// <param name="i">The number to convert.</param>
    /// <param name="indexByZero">Determines whether to index from zero or one.</param>
    /// <returns>The parameter <paramref name="i"/> as an ordinal.</returns>
    [Pure]
    public static string Nth(
#if !WAWA
        this
#endif
            int i,
        bool indexByZero = false
    ) =>
        indexByZero ? (i + 1).ToOrdinal() : i.ToOrdinal();

    /// <inheritdoc cref="string.Split(string[], StringSplitOptions)"/>
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static string[] Split(
#if !WAWA
        this
#endif
            string source,
        string separator
    ) => // ReSharper disable once RedundantCast
        source.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Converts <paramref name="source"/> into a <see cref="string"/> representation of <paramref name="source"/>.
    /// </summary>
    /// <remarks><para>
    /// Unlike <see cref="object.ToString"/>, the values of all properties are printed out,
    /// unless they explicitly define a <see cref="object.ToString"/>, or implement <see cref="IEnumerable{T}"/>,
    /// in which case each item within is printed out separately.
    /// </para></remarks>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The item to get a <see cref="string"/> representation of.</param>
    /// <returns><paramref name="source"/> as <see cref="string"/>.</returns>
    [MustUseReturnValue]
    public static string Stringify<T>(
#if !WAWA
        this
#endif
            T? source
    ) =>
        Stringify(source, MaxRecursion);

    /// <summary>
    /// Converts <paramref name="source"/> into a <see cref="string"/> representation of <paramref name="source"/>.
    /// </summary>
    /// <remarks><para>
    /// Unlike <see cref="object.ToString"/>, the values of all properties are printed out,
    /// unless they explicitly define a <see cref="object.ToString"/>, or implement <see cref="IEnumerable{T}"/>,
    /// in which case each item within is printed out separately.
    /// </para></remarks>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The item to get a <see cref="string"/> representation of.</param>
    /// <param name="depth">Determines how deep the recursive function should go.</param>
    /// <param name="useQuotes">
    /// Determines whether <see cref="string"/> and <see cref="char"/> have a " and ' surrounding them.
    /// </param>
    /// <returns><paramref name="source"/> as <see cref="string"/>.</returns>
    [MustUseReturnValue]
    public static string Stringify<T>(
#if !WAWA
        this
#endif
            T? source,
        int depth,
        bool useQuotes = false
    ) =>
        source switch
        {
            null => Null,
            true => True,
            false => False,
            nint x => $"{x}",
            nuint x => $"{x}",
            char x => useQuotes ? Escape(x) : $"{x}",
            string x => useQuotes ? @$"""{x}""" : x,
            Enum x when x.AsInt() is var i && x.GetType().IsDefined(typeof(FlagsAttribute), false) is var b =>
                $"{x.GetType().Name}({(b ? $"0x{i:x}" : i)}) = {(b
                    ? Conjoin(i.AsBits().Select(x.GetType().Into), BitFlagSeparator)
                    : x)}",
            TimeSpan x => ToConciseString(x),
            Type x => UnfoldedName(x),
            Pointer x => ToHexString(x),
            Version x => ToShortString(x),
#if KTANE
            Object x => x.name,
#endif
            IConvertible x => x.ToString(CultureInfo.InvariantCulture),
            ICustomFormatter x => x.Format("", x, CultureInfo.InvariantCulture),
            _ when depth <= 0 =>
#if NET20 || NET30 || !(!NETSTANDARD || NETSTANDARD2_0_OR_GREATER)
                source.ToString(),
#else
                source.StringifyObject(depth - 1),
#endif
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            IEnumerable<char> x => useQuotes ? @$"""{x.Concat()}""" : x.Concat(),
#else
            IEnumerable<char> x => useQuotes ? @$"""{Conjoin(x, "")}""" : Conjoin(x, ""),
#endif
            IDictionary { Count: 0 } => "{ }",
            IDictionary x => $"{{ {x.DictionaryStringifier(depth - 1, useQuotes)} }}",
            ICollection { Count: var count } x => Count(x, depth - 1, useQuotes, count),
            IEnumerable x => $"[{EnumeratorStringifier(x.GetEnumerator(), depth - 1, useQuotes)}]",
#if NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            ITuple x => $"({EnumeratorStringifier(x.AsEnumerable().GetEnumerator(), depth - 1, useQuotes)})",
#endif
#if !NETFRAMEWORK || NET40_OR_GREATER
            IStructuralComparable x when new FakeComparer(depth - 1) is var c && x.CompareTo(x, c) is var _ => $"{c}",
            IStructuralEquatable x when new FakeComparer(depth - 1) is var c && x.GetHashCode(c) is var _ => $"{c}",
#endif
#if ROSLYN
            ISymbol x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
#endif
#if NET20 || NET30 || !(!NETSTANDARD || NETSTANDARD2_0_OR_GREATER)
            _ => source.ToString(),
#else
            _ => source.StringifyObject(depth - 1),
#endif
        };
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
#if !WAWA
    public
#endif
        static unsafe string ToHexString<T>(this T value)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
#pragma warning disable 8500
    {
        var p = stackalloc char[sizeof(T) * 2];
        p[0] = '0';
        p[1] = 'x';

        fixed (char* rh = HexCharactersTable)
            for (int i = 0, j = sizeof(T) * 2; i < sizeof(T); i++, j -= 2)
            {
                var b = ((byte*)&value)[i];
                var low = b & 0x0f;
                var high = (b & 0xf0) >> 4;
                p[j + 1] = *(rh + low);
                p[j] = *(rh + high);
            }

        return new(p, 0, sizeof(T) * 2 + 2);
    }
#pragma warning restore 8500
#endif
    /// <summary>Forces the use of reflective stringification.</summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The item to get a <see cref="string"/> representation of.</param>
    /// <param name="depth">The amount of nesting.</param>
    /// <returns><paramref name="source"/> as <see cref="string"/>.</returns>
    [MustUseReturnValue]
#if !WAWA
    public
#endif
        static string UseStringifier<T>(this T source, int depth = MaxRecursion)
    {
        // Method can be called if 'forceReflection' is true.
        if (!typeof(T).IsValueType && source is null)
            return Null;

        if (!s_stringifiers.ContainsKey(typeof(T)))
            s_stringifiers[typeof(T)] = GenerateStringifier<T>();

        var name = source?.GetType() is { } type && type != typeof(T)
            ? $"{UnfoldedName(type)} as {UnfoldedName(typeof(T))}"
            : UnfoldedName(typeof(T));

        return ((Func<T, int, string>)s_stringifiers[typeof(T)])(source, depth) is not "" and var str
            ? $"{name} {{ {str} }}"
            : name;
    }
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
        string separator = Separator
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
#if !WAWA
    public
#endif
        static StringBuilder AppendMany<T>(
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
#if !WAWA
    public
#endif
        static StringBuilder AppendMany<T>(
            this StringBuilder builder,
            [InstantHandle] IEnumerable<T> values,
            string separator = Separator
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
#if !WAWA
    /// <summary>Gets the type name, with its generics extended.</summary>
    /// <param name="type">The <see cref="Type"/> to get the name of.</param>
    /// <returns>The name of the parameter <paramref name="type"/>.</returns>
    [Pure]
    public static bool IsUnmanaged([NotNullWhen(true)] this Type? type) =>
        type is not null &&
        (s_fullyUnmanaged.TryGetValue(type, out var answer) ? answer :
            !type.IsValueType ? s_fullyUnmanaged[type] = false :
            type.IsEnum || type.IsPointer || type.IsPrimitive ? s_fullyUnmanaged[type] = true :
            s_fullyUnmanaged[type] = type.IsGenericTypeDefinition
                ? type
                   .GetCustomAttributes()
                   .Any(x => x?.GetType().FullName is "System.Runtime.CompilerServices.IsUnmanagedAttribute")
                : Array.TrueForAll(
                    type.GetFields(
                        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    ),
                    x => IsUnmanaged(x.FieldType)
                ));
#endif
    static void AppendKeyValuePair(this StringBuilder builder, string key, string value) =>
        builder.Append(key).Append(KeyValueSeparator).Append(value);

    static void Push(char c, scoped ref Span<char> span)
    {
        span[0] = c;
#if WAWA
        span = span.Slice(1);
#else
        span = span.UnsafelySkip(1);
#endif
    }

    static void Push([NonNegativeValue] int next, scoped ref Span<char> span)
    {
        if (!next.TryFormat(span, out var slice))
            System.Diagnostics.Debug.Fail("TryFormat");
#if WAWA
        span = span.Slice(slice);
#else
        span = span.UnsafelySkip(slice);
#endif
    }

    // ReSharper disable RedundantAssignment
    static void Push([NonNegativeValue] int next, char c, scoped ref Span<char> span)
    {
        Push(next, ref span);
        Push(c, ref span);
    }

    static void Format(scoped Span<char> span, Version version)
    {
        Push('v', ref span);

        switch (version)
        {
            case (var major, var minor, var build, > 0 and var revision):
                Push(major, '.', ref span);
                Push(minor, '.', ref span);
                Push(build, '.', ref span);
                Push(revision, ref span);
                break;
            case (var major, var minor, > 0 and var build):
                Push(major, '.', ref span);
                Push(minor, '.', ref span);
                Push(build, ref span);
                break;
            case (var major, > 0 and var minor):
                Push(major, '.', ref span);
                Push(minor, ref span);
                break;
            default:
                Push(version.Major, ref span);
                break;
        }

        System.Diagnostics.Debug.Assert(span.IsEmpty, "span is drained");
    }

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER // ReSharper restore RedundantAssignment
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    [MustUseReturnValue]
    static bool CanUse(PropertyInfo p) =>
        p is { CanRead: true, PropertyType.Name: not "SyntaxTree" } &&
        p.GetIndexParameters().Length is 0 &&
        Array.TrueForAll(p.GetCustomAttributes(true), x => x?.GetType() != typeof(ObsoleteAttribute));
#endif
    [Pure]
    static bool IsEqualityContract(PropertyInfo x) =>
        x is { CanRead: true, CanWrite: false, Name: EqualityContract } &&
        x.PropertyType == typeof(Type) &&
        x.GetIndexParameters().Length is 0;

    [Pure]
    static bool IsRecord<T>() =>
        Array.Exists(
            typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic),
            IsEqualityContract
        );

    [Pure]
    static int Length(int major, int revision, int minor, int build) =>
        (major.DigitCount() + 1 is var length && revision > 0 ?
            minor.DigitCount() + build.DigitCount() + revision.DigitCount() + 3 :
            build > 0 ? minor.DigitCount() + build.DigitCount() + 2 :
                minor > 0 ? minor.DigitCount() + 1 : 0) +
        length;

    [Inline, Pure]
    static int Mod(this int i) => Math.Abs(i) / 10 % 10 is 1 ? 0 : Math.Abs(i) % 10;

    [MustUseReturnValue]
    static string Count(IEnumerable e, int depth, bool useQuotes, int count) =>
        count is 0
            ? "[Count: 0]"
            : $"[Count: {count}; {EnumeratorStringifier(e.GetEnumerator(), depth, useQuotes, count)}]";

    [Pure]
    static string Escape(char c) =>
        c switch
        {
            '\'' => "'\\''",
            '\"' => "'\\\"'",
            '\\' => @"'\\'",
            '\0' => "'\\0'",
            '\a' => "'\\a'",
            '\b' => "'\\b'",
            '\f' => "'\\f'",
            '\n' => "'\\n'",
            '\r' => "'\\r'",
            '\t' => "'\\t'",
            '\v' => "'\\v'",
            _ => $"{c}",
        };

    [Pure]
    static string Etcetera(this int? i) => i is null ? "…" : $"…{i} more";

    [Pure]
    static string ToOrdinal(this int i) =>
        $"{i}{Mod(i) switch
        {
            1 => FirstOrd,
            2 => SecondOrd,
            3 => ThirdOrd,
            _ => Else,
        }}";

    [Pure]
    static object Into(this Type type, int i) =>
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
        Enum.ToObject(type, i);
#else
        Enum.Parse(type, $"{i}");
#endif
    [MustUseReturnValue]
    static StringBuilder EnumeratorStringifier(
        [HandlesResourceDisposal] this IEnumerator iterator,
        [NonNegativeValue] int depth,
        bool useQuotes,
        [NonNegativeValue] int? count = null
    )
    {
        try
        {
            StringBuilder builder = new();

            if (iterator.MoveNext())
                builder.Append(Stringify(iterator.Current, depth, useQuotes));

            var i = 0;

            while (iterator.MoveNext())
            {
                if (checked(++i) >= MaxIteration)
                {
                    builder.Append(Separator).Append(Etcetera(count - i));
                    break;
                }

                builder.Append(Separator).Append(Stringify(iterator.Current, depth, useQuotes));
            }

            return builder;
        }
        finally
        {
            (iterator as IDisposable)?.Dispose();
        }
    }
#if !WAWA
    [MustUseReturnValue]
    static StringBuilder Indent(this StringBuilder sb, string indent, int nest)
    {
        sb.AppendLine();

        for (var i = 0; i < nest && nest >= 0; i++)
            sb.Append(indent);

        return sb;
    }
#endif
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    [MustUseReturnValue]
    static string StringifyObject<T>(this T source, int depth)
    {
        if (source is null)
            return Null;

        if (!s_hasMethods.ContainsKey(typeof(T)))
            s_hasMethods[typeof(T)] =
                source.GetType().GetMethod(nameof(ToString), Type.EmptyTypes)?.DeclaringType != typeof(object) &&
                !IsRecord<T>();

        // ReSharper disable once ConstantNullCoalescingCondition NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        if (depth < 0)
            return s_hasMethods[typeof(T)] ? source.ToString() ?? Null : UnfoldedName(source.GetType());

        if (source.GetType() is var t && t != typeof(T))
            return (string)s_stringify.MakeGenericMethod(t).Invoke(null, [source, depth, false])!;

        // ReSharper disable once ConstantNullCoalescingCondition ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return UseStringifier(source, depth);
    }

    [MustUseReturnValue]
    static Func<T, int, string> GenerateStringifier<T>()
    {
        static MethodCallExpression Combine(Expression prev, Expression curr)
        {
            var call = Call(s_combine, prev, s_exSeparator);
            return Call(s_combine, call, curr);
        }

        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        ParameterExpression
            exInstance = Parameter(typeof(T), nameof(T)),
            exDepth = Parameter(typeof(int), nameof(Int32));

        var deepProperties = typeof(T).IsInterface ? typeof(T).GetInterfaces().SelectMany(x => x.GetProperties()) : [];
        var deepFields = typeof(T).IsInterface ? typeof(T).GetInterfaces().SelectMany(x => x.GetFields()) : [];

        // ReSharper disable ArrangeStaticMemberQualifier ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var properties = typeof(T)
           .GetProperties(Flags)
           .Concat(deepProperties)
           .Where(CanUse)
           .OrderBy(x => x.Name, StringComparer.Ordinal)
#if NETFRAMEWORK && !NET40_OR_GREATER
           .Select(p => GetMethodCaller<T, PropertyInfo>(p, exInstance, exDepth, static x => x.PropertyType));
#else
           .Select(p => GetMethodCaller(p, exInstance, exDepth, static x => x.PropertyType));
#endif
        var fields = typeof(T)
           .GetFields(Flags)
           .Concat(deepFields)
           .OrderBy(x => x.Name, StringComparer.Ordinal)
#if NETFRAMEWORK && !NET40_OR_GREATER
           .Select(f => GetMethodCaller<T, FieldInfo>(f, exInstance, exDepth, static x => x.FieldType));
#else
           .Select(f => GetMethodCaller(f, exInstance, exDepth, static x => x.FieldType));
#endif
        var all = fields
           .Concat(properties)
#if WAWA
           .ToList();
#else
           .ToICollection();
#endif
        var exResult = all.Count is 0 ? s_exEmpty : all.Aggregate(Combine);
        return Lambda<Func<T, int, string>>(exResult, exInstance, exDepth).Compile();
    }

    // ReSharper disable SuggestBaseTypeForParameter
    [MustUseReturnValue]
#pragma warning disable CA1859
#if NETFRAMEWORK && !NET40_OR_GREATER
    static Expression GetMethodCaller<T, TMember>(
#else
    static Expression GetMethodCaller<TMember>(
#endif
#pragma warning restore CA1859
        TMember info,
        ParameterExpression exInstance,
        ParameterExpression exDepth,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<TMember, Type> selector
    )
        where TMember : MemberInfo
    {
        var type = selector(info);
        var exConstant = Constant($"{info.Name}{KeyValueSeparator}");
        Expression exAcc = MakeMemberAccess(exInstance, info);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (type.IsByRef || type.IsByRefLike)
#else
        if (type.IsByRef)
#endif
            return Call(
                s_combine,
                exConstant,
                type.GetMethod(nameof(ToString), Type.EmptyTypes) is { } method
                    ? Expression.Call(exAcc, method)
                    : Expression.Constant(UnfoldedName(exAcc.Type))
            );

        // ReSharper disable once NullableWarningSuppressionIsUsed
        while (type.IsPointer && (type = type.GetElementType()!) is var _)
            exAcc = type.IsPointer
                ? throw new NotSupportedException(Apology)
                : Call(type == typeof(void) ? s_readVoidPointer : s_readPointer.MakeGenericMethod(type), exAcc);

        Expression exCall =
            Call(s_stringify.MakeGenericMethod(type == typeof(void) ? typeof(nuint) : type), exAcc, exDepth, s_exTrue);
#if NETFRAMEWORK && !NET40_OR_GREATER // Doesn't support CatchBlock. Workaround works but causes more heap allocations.
        var call = Lambda<Func<T, int, string>>(exCall, exInstance, exDepth).Compile();
        Expression<Func<T, int, string>> wrapped = (t, i) => TryStringify(t, i, call);

        exCall = Invoke(wrapped, exInstance, exDepth);
#else
        CatchBlock
            invalid = Catch(typeof(InvalidOperationException), s_exInvalid),
            unsupported = Catch(typeof(NotSupportedException), s_exUnsupported),
            unsupportedPlatform = Catch(typeof(PlatformNotSupportedException), s_exUnsupportedPlatform);

        exCall = TryCatch(exCall, unsupportedPlatform, unsupported, invalid);
#endif
        return Call(s_combine, exConstant, exCall);
    }
#endif
#if NETFRAMEWORK && !NET40_OR_GREATER
    static string TryStringify<T>(T instance, int depth, [InstantHandle] Func<T, int, string> stringify)
    {
        try
        {
            return stringify(instance, depth);
        }
        catch (PlatformNotSupportedException)
        {
            return UnsupportedPlatform;
        }
        catch (NotSupportedException)
        {
            return Unsupported;
        }
        catch (InvalidOperationException)
        {
            return Invalid;
        }
    }
#endif
    [Pure]
    static StringBuilder DictionaryStringifier(this IDictionary dictionary, int depth, bool useQuotes)
    {
        var iterator = dictionary.GetEnumerator();

        try
        {
            StringBuilder builder = new();

            if (iterator.MoveNext())
                builder.AppendKeyValuePair(
                    Stringify(iterator.Key, depth, useQuotes),
                    Stringify(iterator.Value, depth, useQuotes)
                );

            var i = 0;

            while (iterator.MoveNext())
            {
                if (checked(++i) >= MaxIteration)
                {
                    builder.Append(Separator).Append(Etcetera(dictionary.Count - i));
                    break;
                }

                builder
                   .Append(Separator)
                   .AppendKeyValuePair(
                        Stringify(iterator.Key, depth, useQuotes),
                        Stringify(iterator.Value, depth, useQuotes)
                    );
            }

            return builder;
        }
        finally
        {
            (iterator as IDisposable)?.Dispose();
        }
    }

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static StringBuilder UnfoldedName(this Type? type, StringBuilder builder, Converter<Type, string?> naming)
    {
        StringBuilder Append(Type x)
        {
            builder.Append(',').Append(' ');
            return x.UnfoldedName(builder, naming);
        }

        if (type is null)
            return builder;

        if (s_unfoldedNames.TryGetValue(type, out var val))
            return builder.Append(val);

        if (type.GetElementType() is { } underlying)
            return UnfoldedElementName(type, builder, naming, underlying);

        if ((naming(type) ?? "") is var name && !type.IsGenericType)
            return builder.Append(name);

        var length = name.IndexOf('`') is var i && i is -1 ? name.Length : i;
        var types = type.GetGenericArguments();

        types.FirstOrDefault()?.UnfoldedName(builder.Append(name, 0, length).Append('<'), naming);
        types.Skip(1).Select(Append).Enumerate();

        return builder.Append('>');
    }

    static StringBuilder UnfoldedElementName(
        Type type,
        StringBuilder builder,
        Converter<Type, string?> naming,
        Type underlying
    )
    {
        if (type.IsByRef)
            builder.Append("ref ");

        builder.Append(UnfoldedName(underlying, new(), naming));

        if (type.IsArray)
            builder.Append('[').Append(']');

        if (type.IsPointer)
            builder.Append('*');

        return builder;
    }
#endif
#if WAWA
    static IEnumerable<int> AsBits(this int i)
    {
        for (var j = 1; j is not 0; j <<= 1)
            if ((i & j) is not 0)
                yield return j;
    }
#endif
    static unsafe nuint ReadVoidPointer(void* ptr) => (nuint)ptr;
#pragma warning disable 8500
    static unsafe T? ReadPointer<T>(T* ptr) => (nuint)ptr >= 1 << 11 ? *ptr : default;
#pragma warning restore 8500
#if !NETFRAMEWORK || NET40_OR_GREATER
    sealed class FakeComparer(int depth) : IComparer, IEqualityComparer
    {
        StringBuilder? _builder;

        /// <inheritdoc />
        public override string ToString() =>
            _builder?.Remove(_builder.Length - Separator.Length, Separator.Length).Append(')').ToString() ?? "()";

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object? x, object? y) => Append(x, true);

        /// <inheritdoc />
        int IComparer.Compare(object? x, object? y) => Append(x, 0);

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object? obj) => Append(obj, 0);

        T Append<T>(object? obj, T ret)
        {
#pragma warning disable RCS1196
            (_builder ??= new("(")).Append(Stringify(obj, depth)).Append(Separator);
#pragma warning restore RCS1196
            return ret;
        }
    }
#endif
}
#endif
