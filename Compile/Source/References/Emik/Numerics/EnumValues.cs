// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET35_OR_GREATER
namespace Emik.Morsels;

using static Enum;
using static Expression;

/// <summary>Class to get values from enums.</summary>
public static class EnumValues
{
    /// <summary>Casts <typeparamref name="T"/> to <see langword="int"/>.</summary>
    /// <remarks><para>This method does not cause boxing. Additionally, the cast is unchecked.</para></remarks>
    /// <typeparam name="T">The source type to cast from.</typeparam>
    /// <param name="t">The enum value to cast into an integer.</param>
    /// <returns>An integer representation of the parameter <paramref name="t"/>.</returns>
    public static int AsInt<T>(this T t)
        where T : Enum =>
        Convert<T>.AsInt(t);

    static class Convert<T>
        where T : Enum
    {
        static Convert()
        {
            var parameter = Parameter(typeof(T));
            var underlying = GetUnderlyingType(typeof(T));
            var cast = Convert(parameter, underlying);

            if (underlying != typeof(int))
                cast = Convert(cast, typeof(int));

            AsInt = Lambda<Converter<T, int>>(cast, parameter).Compile();
        }

        public static Converter<T, int> AsInt { get; }
    }
}
#endif
