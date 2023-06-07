// SPDX-License-Identifier: MPL-2.0
#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using static Expression;

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumStrings
{
    const string ParameterName = "value";
#pragma warning disable CA2208, MA0015
    static readonly ConstantExpression s_parameterName = Constant(ParameterName, typeof(string));
#pragma warning restore CA2208, MA0015

    static readonly ConstructorInfo s_newArgument = typeof(ArgumentException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[] { typeof(string), typeof(string) },
            null
        ) ??
        throw Unreachable;

    static readonly ConstructorInfo s_newInvalidEnumArgument = typeof(InvalidEnumArgumentException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[] { typeof(string), typeof(int), typeof(Type) },
            null
        ) ??
        throw Unreachable;

    /// <summary>Converts the value to a constant <see cref="string"/>.</summary>
    /// <remarks><para>
    /// Combinations via <see cref="FlagsAttribute"/> are ignored. Only explicit fields count.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <exception cref="InvalidEnumArgumentException">The value doesn't represent an exact value.</exception>
    /// <returns>The negated value of the parameter <paramref name="value"/>.</returns>
    [Pure]
    public static string AsString<T>(this T value)
        where T : Enum =>
        StringCaching<T>.From(value);

    /// <summary>Converts the <see cref="string"/> to a constant value.</summary>
    /// <remarks><para>
    /// Combinations via <see cref="FlagsAttribute"/> are ignored. Only explicit fields count.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentException">The value doesn't represent an exact value.</exception>
    /// <returns>The negated value of the parameter <paramref name="value"/>.</returns>
    [Pure]
    public static T As<T>(this string value)
        where T : Enum =>
        StringCaching<T>.To(value);

    static class StringCaching<T>
        where T : Enum
    {
        public static Converter<T, string> From { get; } = Make<Converter<T, string>>(false);

        public static Converter<string, T> To { get; } = Make<Converter<string, T>>(true);

        static TFunc Make<TFunc>(bool isToT)
            where TFunc : Delegate
        {
            var parameter = Parameter(isToT ? typeof(string) : typeof(T), ParameterName);
            var cases = Cases(isToT);
            var thrower = Thrower(parameter, isToT);
            var ret = Switch(parameter, thrower, cases);

            return Lambda<TFunc>(ret, parameter).Compile();
        }

        static SwitchCase[] Cases(bool isToT) =>
            typeof(T)
               .GetFields(BindingFlags.Static | BindingFlags.Public)
               .Select(x => Case(x, isToT))
               .ToArray();

        static SwitchCase Case(FieldInfo x, bool isToT)
        {
            var str = Constant(x.Name, typeof(string));
            var t = Constant(x.GetValue(null), typeof(T));
            var from = isToT ? str : t;
            var to = isToT ? t : str;

            return SwitchCase(to, from);
        }

        static UnaryExpression Thrower(Expression parameter, bool isToT) =>
            Throw(isToT ? Format(parameter) : InvalidEnumArgument(parameter), isToT ? typeof(T) : typeof(string));

        static NewExpression Format(Expression parameter) => New(s_newArgument, parameter, s_parameterName);

        static NewExpression InvalidEnumArgument(Expression parameter) =>
            New(
                s_newInvalidEnumArgument,
                s_parameterName,
                Convert(parameter, typeof(int)),
                Constant(typeof(T), typeof(Type))
            );
    }
}
#endif
