// SPDX-License-Identifier: MPL-2.0
#if NET35 && WAWA
namespace Wawa.Modules;
#pragma warning disable SA1137
#else
// ReSharper disable CheckNamespace
namespace Emik.Morsels;
#endif

/// <summary>Provides stringification methods.</summary>
// ReSharper disable once BadPreprocessorIndent
#if NET35 && WAWA
public
#endif
    static partial class Stringifier
{
    // ReSharper disable UnusedMember.Local
#pragma warning disable CA1823, IDE0051
    const string
        Else = "th",
        False = "false",
        FirstOrd = "st",
        KeyValueSeparator = ": ",
        Negative = "-",
        Null = "null",
        SecondOrd = "nd",
        Separator = ", ",
        ThirdOrd = "rd",
        True = "true";
#pragma warning restore CA1823, IDE0051

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    static readonly Dictionary<Type, bool> s_hasMethods = new();

    static readonly Dictionary<Type, Delegate> s_stringifiers = new();

    static readonly Dictionary<Type, string> s_unfoldedNames = new()
    {
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

    static readonly ConstantExpression
        s_exEmpty = Expression.Constant(""),
        s_exFalse = Expression.Constant(false),
        s_exSeparator = Expression.Constant(Separator),
        s_exTrue = Expression.Constant(true);

    static readonly MethodInfo
        s_combine = ((Func<string, string, string>)string.Concat).Method,
        s_stringify = ((Func<bool, bool, bool, bool, string>)Stringify).Method.GetGenericMethodDefinition();
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    static readonly MethodInfo s_toString = ((Func<string?>)s_hasMethods.ToString).Method;
#endif

#if !NET35 // This method purely exists to take advantage of .NET 5's blazingly fast alternative.
    /// <summary>Concatenates an enumeration of <see cref="char"/> into a <see cref="string"/>.</summary>
    /// <remarks><para>
    /// This method is more efficient than using <see cref="Conjoin"/> for <see cref="char"/> enumerations.
    /// </para></remarks>
    /// <param name="chars">The enumeration of characters.</param>
    /// <returns>A <see cref="string"/> built from concatenating <paramref name="chars"/>.</returns>
    [Pure]
    public static string Conjoin(this IEnumerable<char> chars) => string.Concat(chars);
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
        StringBuilder stringBuilder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            stringBuilder.Append(enumerator.Current);
        else
            return "";

        while (enumerator.MoveNext())
            stringBuilder.Append(separator).Append(enumerator.Current);

        return stringBuilder.ToString();
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
        StringBuilder stringBuilder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            stringBuilder.Append(enumerator.Current);
        else
            return "";

        while (enumerator.MoveNext())
            stringBuilder.Append(separator).Append(enumerator.Current);

        return stringBuilder.ToString();
    }

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
        s_unfoldedNames[type] = type.IsGenericType ? $"{type.UnfoldedName(new())}" : type.Name;

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

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    /// <summary>
    /// Converts <paramref name="source"/> into a <see cref="string"/> representation of <paramref name="source"/>.
    /// </summary>
    /// <remarks><para>
    /// Unlike <see cref="object.ToString"/>, the values of all properties are printed out,
    /// unless they explicitly define a <see cref="object.ToString"/>, or inherit <see cref="IEnumerable"/>,
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
        Stringify(source, false, true, false);

    /// <summary>
    /// Converts <paramref name="source"/> into a <see cref="string"/> representation of <paramref name="source"/>.
    /// </summary>
    /// <remarks><para>
    /// Unlike <see cref="object.ToString"/>, the values of all properties are printed out,
    /// unless they explicitly define a <see cref="object.ToString"/>, or inherit <see cref="IEnumerable"/>,
    /// in which case each item within is printed out separately.
    /// </para></remarks>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The item to get a <see cref="string"/> representation of.</param>
    /// <param name="isSurrounded">
    /// Determines whether <see cref="string"/> and <see cref="char"/> have a " and ' surrounding them.
    /// </param>
    /// <param name="isRecursive">
    /// Determines whether it re-calls <see cref="Stringify{T}(T, bool, bool, bool)"/>
    /// on each property in <paramref name="source"/>.
    /// </param>
    /// <param name="forceReflection">
    /// Determines whether it uses its own reflective stringification regardless of type.
    /// </param>
    /// <returns><paramref name="source"/> as <see cref="string"/>.</returns>
    [MustUseReturnValue]
    public static string Stringify<T>(
#if !WAWA
        this
#endif
        T? source,
        bool isSurrounded,
        bool isRecursive = true,
        bool forceReflection = true
    ) =>
        source switch
        {
            _ when forceReflection => source.UseStringifier(),
            null => Null,
            bool b => b ? True : False,
            char c => isSurrounded ? $"'{c}'" : $"{c}",
            string s => isSurrounded ? $@"""{s}""" : s,
#if NET35 && WAWA
            Object o => o.name,
#endif
            IFormattable i => i.ToString(null, CultureInfo.InvariantCulture),
            IDictionary d => $"{{ {d.DictionaryStringifier()} }}",
            ICollection l => $"{l.Count} [{l.GetEnumerator().EnumeratorStringifier()}]",
            IEnumerable e => $"[{e.GetEnumerator().EnumeratorStringifier()}]",
            _ => source.StringifyObject(isRecursive),
        };

    static void AppendKeyValuePair(this StringBuilder builder, string key, string value) =>
        builder.Append(key).Append(KeyValueSeparator).Append(value);
#endif

    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    [MustUseReturnValue]
    static bool CanUse(PropertyInfo p) =>
        p.CanRead &&
        p.GetIndexParameters().Length is 0 &&
        p.GetCustomAttributes(true).All(x => x?.GetType() != typeof(ObsoleteAttribute));

    [Pure]
    static int Mod(this in int i) => Math.Abs(i) / 10 % 10 == 1 ? 0 : Math.Abs(i) % 10;

    [Pure]
    static string ToOrdinal(this int i) =>
        $@"{(i < 0 ? Negative : "")}{i}{Mod(i) switch
        {
            1 => FirstOrd,
            2 => SecondOrd,
            3 => ThirdOrd,
            _ => Else,
        }}";
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER

    [Pure]
    static StringBuilder EnumeratorStringifier(this IEnumerator iterator)
    {
        StringBuilder builder = new();

        if (iterator.MoveNext())
            builder.Append(Stringify(iterator.Current));

        while (iterator.MoveNext())
            builder.Append(Separator).Append(Stringify(iterator.Current));

        return builder;
    }

    [MustUseReturnValue]
    static string StringifyObject<T>(this T source, bool isRecursive)
    {
        if (typeof(T) == typeof(object))
            return source?.ToString() ?? Null;

        if (!s_hasMethods.ContainsKey(typeof(T)))
#pragma warning disable CS0253
            s_hasMethods[typeof(T)] =
                typeof(object) == typeof(T).GetMethod(nameof(ToString), Type.EmptyTypes)?.DeclaringType;
#pragma warning restore CS0253

        if (s_hasMethods[typeof(T)] || !isRecursive)
            return source?.ToString() ?? Null;

        return UseStringifier(source);
    }

    [MustUseReturnValue]
    static string UseStringifier<T>(this T source)
    {
        if (!s_stringifiers.ContainsKey(typeof(T)))
            s_stringifiers[typeof(T)] = GenerateStringifier<T>();

        var name = source?.GetType() is { } type && type != typeof(T)
            ? $"{UnfoldedName(type)} as {UnfoldedName(typeof(T))}"
            : UnfoldedName(typeof(T));

        return $"{name} {{ {((Func<T, string>)s_stringifiers[typeof(T)])(source)} }}";
    }

    [MustUseReturnValue]
    static Func<T, string> GenerateStringifier<T>()
    {
        var exParam = Expression.Parameter(typeof(T), nameof(T));

        // ReSharper disable ArrangeStaticMemberQualifier ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var array = typeof(T)
           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
           .Where(CanUse)
           .Select(p => GetMethodCaller(p, exParam))
           .ToCollectionLazily();

        static MethodCallExpression Combine(MethodCallExpression prev, MethodCallExpression curr)
        {
            var call = Expression.Call(s_combine, prev, s_exSeparator);
            return Expression.Call(s_combine, call, curr);
        }

        Expression exResult = array.Any()
            ? array.Aggregate(Combine)
            : s_exEmpty;

        return Expression
           .Lambda<Func<T, string>>(exResult, exParam)
           .Compile();
    }

    [MustUseReturnValue]
    static MethodCallExpression GetMethodCaller(PropertyInfo info, Expression param)
    {
        var exConstant = Expression.Constant($"{info.Name}{KeyValueSeparator}");

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (info.PropertyType.IsByRefLike)
            return Expression.Call(s_combine, exConstant, Expression.Call(param, s_toString));
#endif

        var method = s_stringify.MakeGenericMethod(info.PropertyType);

        Expression
            exMember = Expression.MakeMemberAccess(param, info),
            exCall = Expression.Call(method, exMember, s_exTrue, s_exFalse, s_exFalse);

        return Expression.Call(s_combine, exConstant, exCall);
    }

    [Pure]
    static StringBuilder DictionaryStringifier(this IDictionary dictionary)
    {
        var iterator = dictionary.GetEnumerator();
        StringBuilder builder = new();

        if (iterator.MoveNext())
            builder.AppendKeyValuePair(Stringify(iterator.Key), Stringify(iterator.Value));

        while (iterator.MoveNext())
            builder.Append(Separator).AppendKeyValuePair(Stringify(iterator.Key), Stringify(iterator.Value));

        return builder;
    }

    static StringBuilder UnfoldedName(this Type? type, StringBuilder sb)
    {
        StringBuilder Append(Type x)
        {
            sb.Append(',').Append(' ');
            return x.UnfoldedName(sb);
        }

        if (type is null)
            return sb;

        if (s_unfoldedNames.TryGetValue(type, out var val))
            return sb.Append(val);

        var name = type.Name;

        if (!type.IsGenericType)
            return sb.Append(name);

        var len = name.IndexOf('`') is var i && i is -1 ? name.Length : i;
        var types = type.GetGenericArguments();

        types.FirstOrDefault()?.UnfoldedName(sb.Append(name, 0, len).Append('<'));
        _ = types.Skip(1).Select(Append).LastOrDefault(_ => false);

        return sb.Append('>');
    }
#endif
}
