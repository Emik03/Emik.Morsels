// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET40_OR_GREATER
// ReSharper disable once CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using static System.Linq.Expressions.Expression;

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumStrings
{
    /// <summary>Converts the value to a constant <see cref="string"/>.</summary>
    /// <remarks><para>
    /// Combinations via <see cref="FlagsAttribute"/> are ignored. Only explicit fields count.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentOutOfRangeException">The value doesn't represent an exact value.</exception>
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
    /// <exception cref="ArgumentOutOfRangeException">The value doesn't represent an exact value.</exception>
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

        static TFunc Make<TFunc>(bool inReverse)
            where TFunc : Delegate
        {
            var parameter = Parameter(typeof(string), nameof(T));
            var thrower = Thrower(nameof(T));
            var cases = Cases(inReverse);
            var ret = Switch(parameter, thrower, cases);

            return Lambda<TFunc>(ret, parameter).Compile();
        }

        static SwitchCase Case(FieldInfo x, bool inReverse)
        {
            var str = Constant(x.Name, typeof(string));
            var t = Constant(x.GetValue(null), typeof(T));
            var from = inReverse ? str : t;
            var to = inReverse ? t : str;

            return SwitchCase(from, to);
        }

        static SwitchCase[] Cases(bool inReverse) => typeof(T).GetFields().Select(x => Case(x, inReverse)).ToArray();

        static UnaryExpression Thrower(string paramName) =>
            Throw(Constant(new ArgumentOutOfRangeException(paramName), typeof(ArgumentOutOfRangeException)));
    }
}
#endif
