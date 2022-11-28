// <copyright file="Peeks.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
#pragma warning disable CS8632
namespace Emik.Morsels;

/// <summary>Provides methods to use callbacks within a statement.</summary>
#pragma warning disable MA0048
static class Peeks
#pragma warning restore MA0048
{
    /// <summary>Quick and dirty debugging function.</summary>
    /// <param name="message">The value to send a message.</param>
    internal static void Write(this string message)
    {
        Console.WriteLine(message); // ReSharper disable once RedundantNameQualifier
        System.Diagnostics.Debug.WriteLine(message);
        Trace.WriteLine(message);
    }

    /// <summary>Quick and dirty debugging function.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify.</param>
    internal static void Write<T>(T value) => Write(value.Stringify());

    /// <summary>Quick and dirty debugging function.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify and return.</param>
    /// <param name="map">The map callback.</param>
    /// <param name="filter">The filter callback.</param>
    /// <param name="logger">The logging callback.</param>
    /// <param name="expression">Automatically filled by compilers; the source code of <paramref name="value"/>.</param>
    /// <param name="path">Automatically filled by compilers; the file's path where this method was called.</param>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <param name="member">Automatically filled by compilers; the member's name where this method was called.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    internal static T Debug<T>(
        this T value,
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
                @$"{(map ?? (x => x))(value).Stringify()}
        of {expression}
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
}
