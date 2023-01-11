#region Emik.MPL

// <copyright file="Peeks.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#pragma warning disable CS8632
namespace Emik.Morsels;

/// <summary>Provides methods to use callbacks within a statement.</summary>
#pragma warning disable MA0048
static partial class Peeks
#pragma warning restore MA0048
{
    /// <summary>An event that is invoked every time <see cref="Write"/> is called.</summary>
    // ReSharper disable once EventNeverSubscribedTo.Global
    internal static event Action<string> OnWrite =
        (Action<string>)Shout +
#if NET35
        (Action<string>)UnityEngine.Debug.Log +
#endif
        (Action<string>)Console.WriteLine;

#pragma warning disable CS1574
    /// <summary>
    /// Invokes <see cref="System.Diagnostics.Debug.WriteLine(string)"/>, and <see cref="Trace.WriteLine(string)"/>.
    /// </summary>
    /// <remarks><para>
    /// This method exists to be able to hook both conditional methods in <see cref="OnWrite"/>,
    /// and to allow the consumer to be able to remove this method to the same <see cref="OnWrite"/>.
    /// </para></remarks>
    /// <param name="message">The value to send a message.</param>
#pragma warning restore CS1574
    internal static void Shout(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
        Trace.WriteLine(message);
    }

    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <param name="message">The value to send a message.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    internal static void Write(this string message) =>
        (OnWrite ?? throw new InvalidOperationException(message))(message);

    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    internal static void Write<T>(T value) => Write(value.Stringify());

    /// <summary>Quick and dirty debugging function.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify and return.</param>
    /// <param name="shouldLogExpression">Determines whether <paramref name="expression"/> is logged.</param>
    /// <param name="map">The map callback.</param>
    /// <param name="filter">The filter callback.</param>
    /// <param name="logger">The logging callback.</param>
    /// <param name="expression">Automatically filled by compilers; the source code of <paramref name="value"/>.</param>
    /// <param name="path">Automatically filled by compilers; the file's path where this method was called.</param>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <param name="member">Automatically filled by compilers; the member's name where this method was called.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    internal static T Debug<T>(
        this T value,
        bool shouldLogExpression = false,
        [InstantHandle] Converter<T, object?>? map = null,
        [InstantHandle] Predicate<T>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
    {
        if ((filter ?? (_ => true))(value))
            (logger ?? Write)(
                @$"{(map ?? (x => x))(value).Stringify()}{(shouldLogExpression ? @$"
        of {expression}" : "")}
        at {member} in {path}:line {line}"
            );

        return value;
    }

    /// <summary>Executes an <see cref="Action{T}"/>, and returns the argument.</summary>
    /// <typeparam name="T">The type of value and action parameter.</typeparam>
    /// <param name="value">The value to pass into the callback.</param>
    /// <param name="action">The callback to perform.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    internal static T Peek<T>(this T value, [InstantHandle] Action<T> action)
    {
        action(value);

        return value;
    }

#if !NETFRAMEWORK
    /// <summary>Executes a <see langword="delegate"/> pointer, and returns the argument.</summary>
    /// <typeparam name="T">The type of value and delegate pointer parameter.</typeparam>
    /// <param name="value">The value to pass into the callback.</param>
    /// <param name="call">The callback to perform.</param>
    /// <exception cref="ArgumentNullException">
    /// The value <paramref name="call"/> points to <see langword="null"/>.
    /// </exception>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    internal static unsafe T Peek<T>(this T value, [InstantHandle, NonNegativeValue] delegate*<T, void> call)
    {
        (call is null ? throw new ArgumentNullException(nameof(call)) : call)(value);

        return value;
    }
#endif
#if NET20 || NET30
    static string Stringify<T>(this T value) => value?.ToString() ?? "";
#endif
}
