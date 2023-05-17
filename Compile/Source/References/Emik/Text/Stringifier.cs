// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantNameQualifier
#pragma warning disable 1696, SA1137, SA1216
#if WAWA
namespace Wawa.Modules;
#else
namespace Emik.Morsels;
#endif

#if !(NET20 || NET30)
using Expression = System.Linq.Expressions.Expression;
using static System.Linq.Expressions.Expression;
#endif

/// <summary>Provides stringification methods.</summary>
// ReSharper disable once BadPreprocessorIndent
#if WAWA
public
#endif
static partial class Stringifier
{
    const int MaxIteration = 32, MaxRecursion = 2;

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
        s_exFalse = Constant(false),
#if !NETFRAMEWORK || NET40_OR_GREATER
        s_exInvalid = Constant(Invalid),
        s_exUnsupported = Constant(Unsupported),
        s_exUnsupportedPlatform = Constant(UnsupportedPlatform),
#endif
        s_exSeparator = Constant(Separator),
        s_exTrue = Constant(true),
        s_exZero = Constant(0);

    static readonly MethodInfo
        s_combine = ((Func<string, string, string>)string.Concat).Method,
        s_stringify = ((Func<bool, bool, bool, int, string>)Stringify).Method.GetGenericMethodDefinition();
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    static readonly MethodInfo s_toString = ((Func<string?>)s_hasMethods.ToString).Method;
#endif

#if NETFRAMEWORK && NET40_OR_GREATER // This method exists to automatically overload to a blazingly fast alternative.
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
        StringBuilder builder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return "";

        var i = 0;

        while (enumerator.MoveNext())
        {
            if (checked(++i) >= MaxIteration)
            {
                var etcetera = Etcetera(values is ICollection { Count: var count } ? count - i : null);
                builder.Append(separator).Append(etcetera);
                break;
            }

            builder.Append(separator).Append(enumerator.Current);
        }

        return builder.ToString();
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
        StringBuilder builder = new();
        using var enumerator = values.GetEnumerator();

        if (enumerator.MoveNext())
            builder.Append(enumerator.Current);
        else
            return "";

        var i = 0;

        while (enumerator.MoveNext())
        {
            if (checked(++i) >= MaxIteration)
            {
                var etcetera = Etcetera(values is ICollection { Count: var count } ? count - i : null);
                builder.Append(separator).Append(etcetera);
                break;
            }

            builder.Append(separator).Append(enumerator.Current);
        }

        return builder.ToString();
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
        s_unfoldedNames[type] = type.IsGenericType ? $"{type.UnfoldedName(new())}" : type.Name;
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
        Stringify(source, false);

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
    /// <param name="forceReflection">
    /// Determines whether it uses its own reflective stringification regardless of type.
    /// </param>
    /// <param name="isSurrounded">
    /// Determines whether <see cref="string"/> and <see cref="char"/> have a " and ' surrounding them.
    /// </param>
    /// <param name="depth">Determines how deep the recursive function should go.</param>
    /// <returns><paramref name="source"/> as <see cref="string"/>.</returns>
    [MustUseReturnValue]
    public static string Stringify<T>(
#if !WAWA
        this
#endif
#pragma warning disable SA1114 RCS1163
            T? source,
        bool forceReflection,
        bool isSurrounded = true,
        int depth = MaxRecursion
#pragma warning restore SA1114 RCS1163
    ) =>
        source switch
        {
#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
            _ when forceReflection => source.UseStringifier(),
#endif
            null => Null,
            bool b => b ? True : False,
            char c => isSurrounded ? $"'{c}'" : $"{c}",
            string s => isSurrounded ? $@"""{s}""" : s,
#if KTANE
            Object o => o.name,
#endif
#if NET20 || NET30 || !(!NETSTANDARD || NETSTANDARD2_0_OR_GREATER)
            _ when depth <= 0 => source.ToString(),
#else
            _ when depth <= 0 => source.StringifyObject(depth - 1),
#endif
            IFormattable i => i.ToString(null, CultureInfo.InvariantCulture),
            IDictionary d => $"{{ {d.DictionaryStringifier(depth - 1)} }}",
            ICollection { Count: var count } l => Count(l, depth, count),
#if !WAWA
            IEnumerable<object> e when e.TryGetNonEnumeratedCount(out var count) => Count(e, depth, count),
#endif
            IEnumerable e => $"[{e.GetEnumerator().EnumeratorStringifier(depth - 1)}]",
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
    static int Mod(this in int i) => Math.Abs(i) / 10 % 10 == 1 ? 0 : Math.Abs(i) % 10;

    [Pure]
    static string Count(IEnumerable e, int depth, int count) =>
        count is 0 ? "[Count: 0]" : $"[Count: {count}; {e.GetEnumerator().EnumeratorStringifier(depth - 1, count)}]";

    [Pure]
    static string Etcetera(this int? i) => i is null ? "..." : $"...{i} more";

    [Pure]
    static string ToOrdinal(this in int i) =>
        $@"{(i < 0 ? Negative : "")}{i}{Mod(i) switch
        {
            1 => FirstOrd,
            2 => SecondOrd,
            3 => ThirdOrd,
            _ => Else,
        }}";

    [Pure]
    static StringBuilder EnumeratorStringifier(this IEnumerator iterator, int depth, int? count = null)
    {
        StringBuilder builder = new();

        if (iterator.MoveNext())
            builder.Append(Stringify(iterator.Current, false, depth: depth));

        var i = 0;

        while (iterator.MoveNext())
        {
            if (checked(++i) >= MaxIteration)
            {
                builder.Append(Separator).Append(Etcetera(count - i));
                break;
            }

            builder.Append(Separator).Append(Stringify(iterator.Current, false, depth: depth));
        }

        return builder;
    }

#if !NET20 && !NET30 && !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
    [MustUseReturnValue]
    static string StringifyObject<T>(this T source, int depth)
    {
        if (!s_hasMethods.ContainsKey(typeof(T)))
            s_hasMethods[typeof(T)] =
                typeof(object) != typeof(T).GetMethod(nameof(ToString), Type.EmptyTypes)?.DeclaringType;

        if (s_hasMethods[typeof(T)])
            return source?.ToString() ?? Null;

        return depth <= 0 ? UnfoldedName(source?.GetType()) : UseStringifier(source);
    }

    [MustUseReturnValue]
    static string UseStringifier<T>(this T source)
    {
        if (!s_stringifiers.ContainsKey(typeof(T)))
            s_stringifiers[typeof(T)] = GenerateStringifier<T>();

        var name = source?.GetType() is { } type && type != typeof(T)
            ? $"{UnfoldedName(type)} as {UnfoldedName(typeof(T))}"
            : UnfoldedName(typeof(T));

        return ((Func<T, string>)s_stringifiers[typeof(T)])(source) is not "" and var str
            ? $"{name} {{ {str} }}"
            : $"{name};";
    }

    [MustUseReturnValue]
    static Func<T, string> GenerateStringifier<T>()
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

        var exParam = Parameter(typeof(T), nameof(T));

        // ReSharper disable ArrangeStaticMemberQualifier ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var properties = typeof(T)
           .GetProperties(Flags)
           .Where(CanUse)
#if NETFRAMEWORK && !NET40_OR_GREATER
           .Select(p => GetMethodCaller<T, PropertyInfo>(p, exParam, static x => x.PropertyType));
#else
           .Select(p => GetMethodCaller(p, exParam, static x => x.PropertyType));
#endif
        var fields = typeof(T)
           .GetFields(Flags)
#if NETFRAMEWORK && !NET40_OR_GREATER
           .Select(f => GetMethodCaller<T, FieldInfo>(f, exParam, static x => x.FieldType));
#else
           .Select(f => GetMethodCaller(f, exParam, static x => x.FieldType));
#endif

        var all = fields
           .Concat(properties)
#if WAWA
           .ToList();
#else
           .ToCollectionLazily();
#endif
        static MethodCallExpression Combine(Expression prev, Expression curr)
        {
            var call = Call(s_combine, prev, s_exSeparator);
            return Call(s_combine, call, curr);
        }

        var exResult = all.Any()
            ? all.Aggregate(Combine)
            : s_exEmpty;

        return Lambda<Func<T, string>>(exResult, exParam).Compile();
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    [MustUseReturnValue]
#if NETFRAMEWORK && !NET40_OR_GREATER
    static Expression GetMethodCaller<T, TMember>(
#else
    static Expression GetMethodCaller<TMember>(
#endif
        TMember info,
        ParameterExpression param,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<TMember, Type> selector
    )
        where TMember : MemberInfo
    {
        var type = selector(info);

        var exConstant = Constant($"{info.Name}{KeyValueSeparator}");
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (type.IsByRefLike)
            return Call(s_combine, exConstant, Call(param, s_toString));
#endif
        var method = s_stringify.MakeGenericMethod(type);

        Expression
            exMember = MakeMemberAccess(param, info),
            exCall = Call(method, exMember, s_exFalse, s_exTrue, s_exZero);

#if NETFRAMEWORK && !NET40_OR_GREATER // Doesn't support CatchBlock. Workaround works but causes more heap allocations.
        var call = Lambda<Func<T, string>>(exCall, param).Compile();
        Expression<Func<T, string>> wrapped = t => TryStringify(t, call);

        exCall = Invoke(wrapped, param);
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
    static string TryStringify<T>(T instance, [InstantHandle] Func<T, string> stringify)
    {
        try
        {
            return stringify(instance);
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
    static StringBuilder DictionaryStringifier(this IDictionary dictionary, int depth)
    {
        var iterator = dictionary.GetEnumerator();
        StringBuilder builder = new();

        if (iterator.MoveNext())
            builder.AppendKeyValuePair(Stringify(iterator.Key), Stringify(iterator.Value));

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
                    Stringify(iterator.Key, false, depth: depth),
                    Stringify(iterator.Value, false, depth: depth)
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

        var name = type.Name;

        if (!type.IsGenericType)
            return builder.Append(name);

        var len = name.IndexOf('`') is var i && i is -1 ? name.Length : i;
        var types = type.GetGenericArguments();

        types.FirstOrDefault()?.UnfoldedName(builder.Append(name, 0, len).Append('<'));
        _ = types.Skip(1).Select(Append).LastOrDefault(_ => false);

        return builder.Append('>');
    }
#endif
}
