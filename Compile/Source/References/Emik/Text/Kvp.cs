// SPDX-License-Identifier: MPL-2.0
#if NET8_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Contains fast parsing of key-value pairs.</summary>
static class Kvp
{
    static class KvpCache<T>
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        public delegate void SerializeWriter(in T reader, out DefaultInterpolatedStringHandler writer);

        public delegate void DeserializeWriter(scoped ReadOnlySpan<char> reader, ref T writer);

        static readonly ImmutableArray<MemberInfo> s_members = MakeMembers();

        [Pure]
        public static ImmutableArray<(string Key, bool IsCollection, DeserializeWriter Writer)> Deserializers { get; } =
            MakeDeserializers();

        [Pure]
        public static SerializeWriter Serializer { get; } = MakeSerializer();

        [Pure]
        static bool IsAdd(MethodInfo x) =>
            x.Name is nameof(IList.Add) && x.GetGenericArguments() is [] && x.GetParameters() is [_];

        [Pure]
        static bool IsCollection([NotNullWhen(true)] MethodInfo? adder, Type type) =>
            adder is not null && type.IsAssignableTo(typeof(ICollection)) && type.GetConstructor([]) is not null;

        [MustUseReturnValue]
        static Expression Convert(Type t, Expression exReader, Expression exTemp)
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantTypeArgumentsInsideNameof
#pragma warning disable IDE0340
            var spanToString = typeof(ReadOnlySpan<char>).GetMethod(nameof(ReadOnlySpan<char>.ToString), [])!;
#pragma warning restore IDE0340
            var exIFormatProvider = Expression.Constant(CultureInfo.InvariantCulture);
            var exReaderString = Expression.Call(exReader, spanToString);
            var exNumberStyles = Expression.Constant(NumberStyles.Any);
            var tru = Expression.Constant(true);

            return 0 switch
            {
                _ when t == typeof(string) => Expression.Assign(exTemp, exReaderString),
                _ when t.GetMethod(
                    nameof(int.TryParse),
                    [typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider), t.MakeByRefType()]
                ) is { } x => Expression.Call(x, exReader, exNumberStyles, exIFormatProvider, exTemp),
                _ when t.GetMethod(
                    nameof(int.TryParse),
                    [typeof(ReadOnlySpan<char>), typeof(IFormatProvider), t.MakeByRefType()]
                ) is { } x => Expression.Call(x, exReader, exIFormatProvider, exTemp),
                _ when t.GetMethod(nameof(int.TryParse), [typeof(ReadOnlySpan<char>), t.MakeByRefType()]) is
                    { } x => Expression.Call(x, exReader, exTemp),
                _ when t.GetMethod(
                    nameof(int.TryParse),
                    [typeof(string), typeof(NumberStyles), typeof(IFormatProvider), t.MakeByRefType()]
                ) is { } x => Expression.Call(x, exReaderString, exNumberStyles, exIFormatProvider, exTemp),
                _ when t.GetMethod(
                    nameof(int.TryParse),
                    [typeof(string), typeof(IFormatProvider), t.MakeByRefType()]
                ) is { } x => Expression.Call(x, exReaderString, exIFormatProvider, exTemp),
                _ when t.GetMethod(nameof(int.TryParse), [typeof(string), t.MakeByRefType()]) is { } x =>
                    Expression.Call(x, exReaderString, exTemp),
                _ when t.IsEnum => Expression.Call(typeof(Enum), nameof(Enum.TryParse), [t], exReader, tru, exTemp),
                _ => Expression.Assign(exTemp, Expression.Default(t)),
            };
        }

        [MustUseReturnValue]
        static ImmutableArray<(string, bool, DeserializeWriter)> MakeDeserializers()
        {
            var members = s_members.Where(x => x is FieldInfo { IsInitOnly: false } or PropertyInfo { CanWrite: true })
               .ToIList();

            var builder = ImmutableArray.CreateBuilder<(string, bool, DeserializeWriter)>(members.Count);

            foreach (var member in members)
            {
                var type = UnderlyingType(member);
                var adder = type.GetMethods().FirstOrDefault(IsAdd);
                var t = IsCollection(adder, type) ? adder.GetParameters()[0].ParameterType : type;
                var isFlagEnum = type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() is not null;
                var exReader = Expression.Parameter(typeof(ReadOnlySpan<char>));
                var exWriter = Expression.Parameter(typeof(T).MakeByRefType());
                var exTemp = Expression.Variable(t);
                var exCall = Convert(t, exReader, exTemp);
                var fieldOrProperty = exWriter.FieldOrProperty(member);

                Expression exUpdate = IsCollection(adder, type)
                    ? Expression.IfThenElse(
                        Expression.ReferenceEqual(fieldOrProperty, Expression.Constant(null)),
                        Expression.Assign(fieldOrProperty, Expression.ListInit(Expression.New(type), exTemp)),
                        Expression.Call(fieldOrProperty, nameof(IList.Add), [], exTemp)
                    )
                    : Expression.Assign(
                        fieldOrProperty,
                        isFlagEnum
                            ? Expression.Convert(
                                Expression.Or(
                                    Expression.Convert(fieldOrProperty, type.GetEnumUnderlyingType()),
                                    Expression.Convert(exTemp, type.GetEnumUnderlyingType())
                                ),
                                type
                            )
                            : exTemp
                    );

                var exBlock = Expression.Block([exTemp], exCall, exUpdate);
                var lambda = Expression.Lambda<DeserializeWriter>(exBlock, exReader, exWriter).Compile();
                var key = member.Name.Trim().Replace("-", "").Replace("_", "");
                builder.Add((key, type.IsEnum || IsCollection(adder, type), lambda));
            }

            return builder.MoveToImmutable();
        }

        [MustUseReturnValue]
        static ImmutableArray<MemberInfo> MakeMembers()
        {
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            var fields = typeof(T).GetFields(Flags);
            var properties = typeof(T).GetProperties(Flags);

            return
            [
                ..fields.AsEnumerable<MemberInfo>()
                   .Concat(properties)
                   .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase),
            ];
        }

        static string ToString(ICollection values)
        {
            if (values.Count is 0)
                return "";

            DefaultInterpolatedStringHandler dish = new((values.Count - 1) * 2, values.Count);
            var enumerator = values.GetEnumerator();

            try
            {
                if (enumerator.MoveNext())
                    dish.AppendFormatted(enumerator.Current);
                else
                    return dish.ToStringAndClear();

                while (enumerator.MoveNext())
                {
                    dish.AppendFormatted(", ");
                    dish.AppendFormatted(enumerator.Current);
                }

                return dish.ToStringAndClear();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        [MustUseReturnValue]
        static SerializeWriter MakeSerializer()
        {
            const string AppendFormatted = nameof(AppendFormatted),
                AppendLiteral = nameof(AppendLiteral),
                Assignment = " = ",
                Separator = "\n";

            [MustUseReturnValue]
            static int ConstantLength(MemberInfo m) => m.Name.Length + Assignment.Length + Separator.Length;

            var exReader = Expression.Parameter(typeof(T).MakeByRefType());
            var exWriter = Expression.Parameter(typeof(DefaultInterpolatedStringHandler).MakeByRefType());
            // ReSharper disable once NullableWarningSuppressionIsUsed
            var constructor = typeof(DefaultInterpolatedStringHandler).GetConstructor([typeof(int), typeof(int)])!;
            var members = s_members.Where(x => x is FieldInfo or PropertyInfo { CanRead: true }).ToIList();
            var literalLength = members.Sum(ConstantLength);
            var exLiteralLength = Expression.Constant(literalLength);
            var exFormattedLength = Expression.Constant(members.Count);
            var exNew = Expression.New(constructor, exLiteralLength, exFormattedLength);
            List<Expression> exBlockArgs = [Expression.Assign(exWriter, exNew)];

            foreach (var member in members)
            {
                var str = Expression.Constant($"{member.Name}{Assignment}");
                var exAssignment = Expression.Call(exWriter, AppendLiteral, [], str);
                exBlockArgs.Add(exAssignment);

                var exMember = Expression.MakeMemberAccess(exReader, member);
                var underlyingType = UnderlyingType(member);

                var isCollection = underlyingType != typeof(string) &&
                    underlyingType.IsAssignableTo(typeof(ICollection));

                var exFormatted = Expression.Call(
                    exWriter,
                    AppendFormatted,
                    [isCollection ? typeof(string) : underlyingType],
                    isCollection
                        ? Expression.Call(((Converter<ICollection, string>)ToString).Method, exMember)
                        : exMember
                );

                exBlockArgs.Add(exFormatted);

                var separator = Expression.Constant(Separator);
                var exSep = Expression.Call(exWriter, AppendLiteral, [], separator);
                exBlockArgs.Add(exSep);
            }

            var exBlock = Expression.Block(exBlockArgs);
            return Expression.Lambda<SerializeWriter>(exBlock, exReader, exWriter).Compile();
        }

        [MustUseReturnValue]
        static Type UnderlyingType(MemberInfo member) =>
            member switch
            {
                FieldInfo f => f.FieldType,
                PropertyInfo p => p.PropertyType,
                _ => throw Unreachable,
            };
    }

    /// <summary>Serialize an object into a string.</summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>The serialized object.</returns>
    [Pure]
    public static string Serialize<T>(in T value)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        KvpCache<T>.Serializer(value, out var dish);
        return dish.ToStringAndClear();
    }

    /// <summary>Deserialize string into an object of the given type.</summary>
    /// <typeparam name="T">The type of the deserialized object.</typeparam>
    /// <param name="span">The string to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    [Pure]
    public static T Deserialize<T>(scoped ReadOnlySpan<char> span)
#if NO_ALLOWS_REF_STRUCT
        where T : new()
#else
        where T : new(), allows ref struct
#endif
    {
        var ret = new T();
        Deserialize(span, ref ret);
        return ret;
    }

    /// <summary>Deserialize string into an object of the given type.</summary>
    /// <typeparam name="T">The type of the deserialized object.</typeparam>
    /// <param name="span">The string to deserialize.</param>
    /// <param name="writer">The object to write to.</param>
    /// <returns>The deserialized object.</returns>
    public static void Deserialize<T>(scoped ReadOnlySpan<char> span, ref T writer)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        for (;
            span.IndexOfAny(Whitespaces.BreakingSearch.GetSpan()[0]) is var separator;
            span = span.UnsafelySkip(separator + 1))
        {
            SplitToKeyValuePair(span, separator, ref writer);

            if (separator is -1)
                break;
        }
    }

    static void SplitToKeyValuePair<T>(scoped ReadOnlySpan<char> span, int separator, ref T writer)
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        var slice = span.UnsafelyTake(separator is -1 ? span.Length : separator);

        if (slice.IndexOfAny(';', '#') is not -1 and var comments)
            slice = slice.UnsafelyTake(comments);

        if (slice.IndexOf('=') is not (not -1 and var equals))
            return;

        foreach (var deserializer in KvpCache<T>.Deserializers)
            ProcessKeyValuePair(deserializer, slice, equals, ref writer);
    }

    static void ProcessKeyValuePair<T>(
        in (string Key, bool, KvpCache<T>.DeserializeWriter) deserializer,
        scoped ReadOnlySpan<char> slice,
        int equals,
        ref T writer
    )
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        for (ReadOnlySpan<char> expected = deserializer.Key, key = slice.UnsafelyTake(equals).Trim();
            key.IndexOfAny('-', '_') is var i && expected.StartsWith(i is -1 ? key : key.UnsafelyTake(i));
            expected = expected.UnsafelySkip(i + 1), key = key.UnsafelySkip(i + 1))
            if (i is -1 || i >= expected.Length)
            {
                ProcessValue(deserializer, slice.UnsafelySkip(equals + 1), ref writer);
                return;
            }
    }

    static void ProcessValue<T>(
        (string, bool IsCollection, KvpCache<T>.DeserializeWriter Writer) valueTuple,
        scoped ReadOnlySpan<char> slice,
        ref T writer
    )
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
    {
        if (!valueTuple.IsCollection)
        {
            valueTuple.Writer(slice.Trim(), ref writer);
            return;
        }

        for (; slice.IndexOf(',') is var j; slice = slice.UnsafelySkip(j + 1))
        {
            valueTuple.Writer((j is -1 ? slice : slice.UnsafelyTake(j)).Trim(), ref writer);

            if (j is -1)
                return;
        }
    }
}
#endif
