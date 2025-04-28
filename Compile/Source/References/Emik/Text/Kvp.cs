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
        public static ImmutableArray<KeyValuePair<string, DeserializeWriter>> Deserializers { get; } =
            MakeDeserializers();

        [Pure]
        public static SerializeWriter Serializer { get; } = MakeSerializer();

        [MustUseReturnValue]
        static bool IsAppendFormatted(MethodInfo x) =>
            x.Name is nameof(DefaultInterpolatedStringHandler.AppendFormatted) &&
            x.GetParameters() is [{ ParameterType.IsGenericMethodParameter: true }];

        [MustUseReturnValue]
        static ImmutableArray<KeyValuePair<string, DeserializeWriter>> MakeDeserializers()
        {
            var members = s_members.Where(x => x is FieldInfo { IsInitOnly: false } or PropertyInfo { CanWrite: true })
               .ToIList();

            var builder = ImmutableArray.CreateBuilder<KeyValuePair<string, DeserializeWriter>>(members.Count);

            foreach (var member in members)
            {
                var t = UnderlyingType(member);
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantTypeArgumentsInsideNameof
                var spanToString = typeof(ReadOnlySpan<char>).GetMethod(nameof(ReadOnlySpan<char>.ToString), [])!;
                var exIFormatProvider = Expression.Constant(CultureInfo.InvariantCulture);
                var exReader = Expression.Parameter(typeof(ReadOnlySpan<char>));
                var exWriter = Expression.Parameter(typeof(T).MakeByRefType());
                var exNumberStyles = Expression.Constant(NumberStyles.Any);
                var exTemp = Expression.Variable(t);

                var exCall = 0 switch
                {
                    _ when t == typeof(string) => Expression.Assign(exTemp, Expression.Call(exReader, spanToString)),
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
                    _ => (Expression)Expression.Assign(exTemp, Expression.Default(t)),
                };

                var exAssign = exWriter.AssignFieldOrProperty(member, exTemp);
                var exBlock = Expression.Block([exTemp], exCall, exAssign);
                var lambda = Expression.Lambda<DeserializeWriter>(exBlock, exReader, exWriter).Compile();
                var key = member.Name.Trim().Replace("-", "").Replace("_", "");
                builder.Add(new(key, lambda));
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

        [MustUseReturnValue]
        static SerializeWriter MakeSerializer()
        {
            const string Assignment = " = ", Separator = "\n";

            [MustUseReturnValue]
            static int ConstantLength(MemberInfo m) => m.Name.Length + Assignment.Length + Separator.Length;

            var exReader = Expression.Parameter(typeof(T).MakeByRefType());
            var exWriter = Expression.Parameter(typeof(DefaultInterpolatedStringHandler).MakeByRefType());
            var appendFormatted = typeof(DefaultInterpolatedStringHandler).GetMethods().Single(IsAppendFormatted);

            var appendLiteral = typeof(DefaultInterpolatedStringHandler).GetMethod(
                nameof(DefaultInterpolatedStringHandler.AppendLiteral),
                [typeof(string)] // ReSharper disable once NullableWarningSuppressionIsUsed
            )!;

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
                var exAssignment = Expression.Call(exWriter, appendLiteral, str);
                exBlockArgs.Add(exAssignment);

                var exMember = Expression.MakeMemberAccess(exReader, member);
                var underlyingType = UnderlyingType(member);
                var genericMethod = appendFormatted.MakeGenericMethod(underlyingType);
                var exFormatted = Expression.Call(exWriter, genericMethod, exMember);
                exBlockArgs.Add(exFormatted);

                var separator = Expression.Constant(Separator);
                var exSep = Expression.Call(exWriter, appendLiteral, separator);
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
        in KeyValuePair<string, KvpCache<T>.DeserializeWriter> deserializer,
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
                deserializer.Value(slice.UnsafelySkip(equals + 1).Trim(), ref writer);
                break;
            }
    }
}
#endif
