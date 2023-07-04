// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantNameQualifier
#pragma warning disable 1696, SA1137, SA1216
#if WAWA
namespace Wawa.Modules;
#else
namespace Emik.Morsels;
#endif

#if !(NET20 || NET30)
using static Expression;
#endif

/// <summary>Provides stringification methods.</summary>
// ReSharper disable once BadPreprocessorIndent
#if WAWA
public
#endif
static partial class Stringifier
{
    const int MaxIteration = 32, MaxRecursion = 3;
#if !WAWA
    const RegexOptions Options = RegexOptions.Multiline | RegexOptions.Compiled;
#endif

    // ReSharper disable UnusedMember.Local
#pragma warning disable CA1823, IDE0051
    const string
        Else = "th",
        False = "false",
        FirstOrd = "st",
        Invalid = $"!<{nameof(InvalidOperationException)}>",
        KeyValueSeparator = ": ",
        Negative = "-",
        Null = "null",
        SecondOrd = "nd",
        Separator = ", ",
        ThirdOrd = "rd",
        True = "true",
        Unsupported = $"!<{nameof(NotSupportedException)}>",
        UnsupportedPlatform = $"!<{nameof(PlatformNotSupportedException)}>";
#pragma warning restore CA1823, IDE0051

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static readonly Dictionary<Type, bool> s_hasMethods = new();

    static readonly Dictionary<Type, Delegate> s_stringifiers = new();

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

    static readonly MethodInfo
        s_combine = ((Func<string, string, string>)string.Concat).Method,
        s_stringify = ((Func<bool, int, bool, string>)Stringify).Method.GetGenericMethodDefinition();
#endif
    static readonly MethodInfo s_toString = ((Func<string?>)s_hasMethods.ToString).Method;
#if !WAWA

#pragma warning disable MA0110
    static readonly Regex
        s_parentheses = new(@"\((?>(?:\((?<A>)|\)(?<-A>)|[^()]+){2,})\)", Options),
        s_brackets = new(@"\[(?>(?:\[(?<A>)|\](?<-A>)|[^\[\]]+){2,})\]", Options),
        s_curlies = new("{(?>(?:{(?<A>)|}(?<-A>)|[^{}]+){2,})}", Options),
        s_angles = new("<(?>(?:<(?<A>)|>(?<-A>)|[^<>]+){2,})>", Options),
        s_quotes = new(@"""(?>(?:{(?<A>)|}(?<-A>)|[^""]+){2,})""", Options);
#pragma warning restore MA0110

    /// <summary>Creates the collapsed form of the string.</summary>
    /// <param name="s">The string to collapse.</param>
    /// <returns>The collapsed string.</returns>
    public static string Collapse(this string s)
    {
        s = s_parentheses.Replace(s, "(…)");
        s = s_brackets.Replace(s, "[…]");
        s = s_curlies.Replace(s, "{…}");
        s = s_angles.Replace(s, "<…>");
        return s_quotes.Replace(s, "\"…\"");
    }

    /// <summary>Creates the prettified form of the string.</summary>
    /// <param name="s">The string to prettify.</param>
    /// <returns>The prettified string.</returns>
    public static string Prettify(this string s) => Prettify(s, separator: ",;");

    /// <summary>Creates the prettified form of the string.</summary>
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
#pragma warning disable CA1508
    {
        // Inspired by https://gist.github.com/kodo-pp/89cefb17a8772cd9fd7b875d94fd29c7.
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
#pragma warning restore CA1508
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
    public static string Concat(this IEnumerable<char> chars) => string.Concat(chars);
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
#if !WAWA
        this
#endif
            IEnumerable<T> values,
        char separator
    )
    {
        StringBuilder builder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return "";

        while (enumerator.MoveNext())
            builder.Append(separator).Append(enumerator.Current);

        return $"{builder}";
    }

    /// <summary>Joins a set of values into one long <see cref="string"/>.</summary>
    /// <typeparam name="T">The type of each item in the collection.</typeparam>
    /// <param name="values">The values to join.</param>
    /// <param name="separator">The separator between each item.</param>
    /// <returns>One long <see cref="string"/>.</returns>
    [Pure]
    public static string Conjoin<T>(
#if !WAWA
        this
#endif
            IEnumerable<T> values,
        string separator = Separator
    )
    {
        if (values is string value && separator is "")
            return value;

        StringBuilder builder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return "";

        while (enumerator.MoveNext())
            builder.Append(separator).Append(enumerator.Current);

        return $"{builder}";
    }

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
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
        s_unfoldedNames[type] = $"{type.UnfoldedName(new())}";
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
    ) =>
        source.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

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
#pragma warning disable SA1114 RCS1163
            T? source,
        int depth,
        bool useQuotes = false
#pragma warning restore SA1114 RCS1163
    ) =>
        source switch
        {
            null => Null,
            true => True,
            false => False,
            nint x => $"{x}",
            nuint x => $"{x}",
            char x => useQuotes ? Escape(x) : $"{x}",
            string x => useQuotes ? $@"""{x}""" : x,
            Enum x => $"{x.GetType().Name}({(
                x.IsFlagsDefined() ? $"0x{System.Convert.ToInt32(x):x}" : System.Convert.ToInt32(x)
            )}) = {x.EnumStringifier()}",
            Type x => UnfoldedName(x),
#if KTANE
            Object x => x.name,
#endif
            IConvertible x => x.ToString(CultureInfo.InvariantCulture),
            _ when depth <= 0 =>
#if NET20 || NET30 || !(!NETSTANDARD || NETSTANDARD2_0_OR_GREATER)
                source.ToString(),
#else
                source.StringifyObject(depth - 1),
#endif
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            IEnumerable<char> x => useQuotes ? $@"""{x.Concat()}""" : x.Concat(),
#else
            IEnumerable<char> x => useQuotes ? $@"""{Conjoin(x, "")}""" : Conjoin(x, ""),
#endif
            IDictionary { Count: 0 } => "{ }",
            IDictionary x => $"{{ {x.DictionaryStringifier(depth - 1, useQuotes)} }}",
            ICollection { Count: var count } x => Count(x, depth - 1, useQuotes, count),
            IEnumerable x => $"[{x.GetEnumerator().EnumeratorStringifier(depth - 1, useQuotes)}]",
#if NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            ITuple x => $"({x.AsEnumerable().GetEnumerator().EnumeratorStringifier(depth - 1, useQuotes)})",
#endif
            IStructuralComparable x when new FakeComparer(depth - 1) is var c && x.CompareTo(x, c) is var _ => $"{c}",
            IStructuralEquatable x when new FakeComparer(depth - 1) is var c && x.GetHashCode(c) is var _ => $"{c}",
#if NET20 || NET30 || !(!NETSTANDARD || NETSTANDARD2_0_OR_GREATER)
            _ => source.ToString(),
#else
            _ => source.StringifyObject(depth - 1),
#endif
        };

    static void AppendKeyValuePair(this StringBuilder builder, string key, string value) =>
        builder.Append(key).Append(KeyValueSeparator).Append(value);

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    [MustUseReturnValue]
    static bool CanUse(PropertyInfo p) =>
        p.CanRead &&
        p.GetIndexParameters().Length is 0 &&
        p.GetCustomAttributes(true).All(x => x?.GetType() != typeof(ObsoleteAttribute));
#endif

    [Pure]
    static bool IsAccessible([NotNullWhen(true)] this Type? t) => t is { IsPublic: true, IsNestedPublic: true };

    [Pure]
    static bool IsFlagsDefined(this Enum value) => value.GetType().IsDefined(typeof(FlagsAttribute), false);

    [Pure]
    static bool IsOneBitSet(this Enum value, Enum next) =>
        System.Convert.ToInt32(next) is not 0 and var filter &&
        (filter & filter - 1) is 0 &&
        System.Convert.ToInt32(value) is var bits &&
        (bits & filter) is not 0;

    [Pure]
    static int Mod(this in int i) => Math.Abs(i) / 10 % 10 == 1 ? 0 : Math.Abs(i) % 10;

    [MustUseReturnValue]
    static string Count(IEnumerable e, int depth, bool useQuotes, int count) =>
        count is 0
            ? "[Count: 0]"
            : $"[Count: {count}; {e.GetEnumerator().EnumeratorStringifier(depth, useQuotes, count)}]";

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
    static string EnumStringifier(this Enum value)
    {
        var values = Enum
           .GetValues(value.GetType())
           .Cast<Enum>()
           .Where(value.IsOneBitSet)
           .OrderBy(System.Convert.ToInt32)
#if WAWA
           .ToList();
#else
           .ToCollectionLazily();
#endif

        string BitStringifier(int x) =>
#if WAWA
            values.Find(y => System.Convert.ToInt32(y) == 1L << x) is { } member ? $"{member}" :
#else
            values.FirstOrDefault(y => System.Convert.ToInt32(y) == 1L << x) is { } member ? $"{member}" :
#endif
            x is -1 ? "0" : $"1 << {x}";

        return value.IsFlagsDefined()
            ? Conjoin(System.Convert.ToInt32(value).Bits().Select(BitStringifier), " | ")
            : $"{value}";
    }

    static IEnumerable<int> Bits(this int number)
    {
        const int BitsInByte = 8;

        if (number is 0)
        {
            yield return -1;

            yield break;
        }

        for (var i = 0; i < sizeof(int) * BitsInByte; i++)
            if ((number >> i & 1) is not 0)
                yield return i;
    }

    [Pure]
    static string Etcetera(this int? i) => i is null ? "…" : $"…{i} more";

    [Pure]
    static string ToOrdinal(this int i) =>
        $"{(i < 0 ? Negative : "")}{i}{Mod(i) switch
        {
            1 => FirstOrd,
            2 => SecondOrd,
            3 => ThirdOrd,
            _ => Else,
        }}";

    [MustUseReturnValue]
    static StringBuilder EnumeratorStringifier(
        this IEnumerator iterator,
        [NonNegativeValue] int depth,
        bool useQuotes,
        [NonNegativeValue] int? count = null
    )
    {
        StringBuilder builder = new();

        if (iterator.MoveNext())
            builder.Append(Stringify(iterator.Current, depth));

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
#pragma warning disable 8600, 8603 // Will never be null, we have access to this function.
        if (source.GetType() is var t && t != typeof(T) && IsAccessible(t))
            return (string)s_stringify.MakeGenericMethod(t).Invoke(null, new object[] { source, depth, false });
#pragma warning restore 8600, 8603
        if (!s_hasMethods.ContainsKey(typeof(T)))
            s_hasMethods[typeof(T)] =
                source.GetType().GetMethod(nameof(ToString), Type.EmptyTypes)?.DeclaringType != typeof(object);

        // ReSharper disable once ConstantNullCoalescingCondition ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return depth >= 0 ? UseStringifier(source, depth) :
            s_hasMethods[typeof(T)] ? source.ToString() ?? Null : UnfoldedName(source.GetType());
    }

    [MustUseReturnValue]
    static string UseStringifier<T>(this T source, int depth)
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

        // ReSharper disable ArrangeStaticMemberQualifier ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var properties = typeof(T)
           .GetProperties(Flags)
           .Where(CanUse)
#if NETFRAMEWORK && !NET40_OR_GREATER
           .Select(p => GetMethodCaller<T, PropertyInfo>(p, exInstance, exDepth, static x => x.PropertyType));
#else
           .Select(p => GetMethodCaller(p, exInstance, exDepth, static x => x.PropertyType));
#endif
        var fields = typeof(T)
           .GetFields(Flags)
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
           .ToCollectionLazily();
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
        TMember info,
        ParameterExpression exInstance,
        ParameterExpression exDepth,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<TMember, Type> selector
    )
#pragma warning restore CA1859
        where TMember : MemberInfo
    {
        var type = selector(info);
        var exConstant = Constant($"{info.Name}{KeyValueSeparator}");

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (type.IsByRef || type.IsByRefLike)
#else
        if (type.IsByRef)
#endif
            return Call(s_combine, exConstant, Call(exInstance, s_toString));

        var method = s_stringify.MakeGenericMethod(type);

        // ReSharper disable once NullableWarningSuppressionIsUsed
        Expression
            exMember = MakeMemberAccess(exInstance, info),
            exCall = Call(method, exMember, exDepth, s_exTrue);
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

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static StringBuilder UnfoldedName(this Type? type, StringBuilder builder)
    {
        StringBuilder Append(Type x)
        {
            builder.Append(',').Append(' ');
            return x.UnfoldedName(builder);
        }

        if (type is null)
            return builder;

        if (s_unfoldedNames.TryGetValue(type, out var val))
            return builder.Append(val);

        if (type.GetElementType() is { } underlying)
        {
            if (type.IsByRef)
                builder.Append('r').Append('e').Append('f').Append(' ');

            var underlyingName = UnfoldedName(underlying);
            builder.Append(underlyingName);

            if (type.IsArray)
                builder.Append('[').Append(']');

            if (type.IsPointer)
                builder.Append('*');

            return builder;
        }

        var name = type.Name;

        if (!type.IsGenericType)
            return builder.Append(name);

        var len = name.IndexOf('`') is var i && i is -1 ? name.Length : i;
        var types = type.GetGenericArguments();

        types.FirstOrDefault()?.UnfoldedName(builder.Append(name, 0, len).Append('<'));
        types.Skip(1).Select(Append).Enumerate();

        return builder.Append('>');
    }
#endif

    sealed class FakeComparer : IComparer, IEqualityComparer
    {
        readonly int _depth;

        StringBuilder? _builder;

        public FakeComparer(int depth) => _depth = depth;

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
            (_builder ??= new("(")).Append(obj.Stringify(_depth)).Append(Separator);
            return ret;
        }
    }
}
