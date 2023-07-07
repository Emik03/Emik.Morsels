// SPDX-License-Identifier: MPL-2.0
#pragma warning disable CS8632, RCS1196

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to use callbacks within a statement.</summary>
#pragma warning disable MA0048
static partial class Peeks
#pragma warning restore MA0048
{
    /// <summary>An event that is invoked every time <see cref="Write"/> is called.</summary>
    // ReSharper disable RedundantCast
    // ReSharper disable once EventNeverSubscribedTo.Global
    public static event Action<string> OnWrite =
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2
        Shout;
#else
        (Action<string>)Shout +
#if KTANE
        (Action<string>)UnityEngine.Debug.Log +
#endif
        (Action<string>)Console.WriteLine;
#endif

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
    public static void Shout(string message)
    {
        // ReSharper disable once InvocationIsSkipped
        System.Diagnostics.Debug.WriteLine(message);
#if !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        Trace.WriteLine(message);
#endif
    }

    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <param name="message">The value to send a message.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    public static void Write(this string message) => (OnWrite ?? throw new InvalidOperationException(message))(message);

    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    // ReSharper disable once InvokeAsExtensionMethod
    public static void Write<T>(T value) => Write(Stringifier.Stringify(value));

    /// <summary>Quick and dirty debugging function.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify and return.</param>
    /// <param name="shouldPrettify">Determines whether to prettify the resulting <see cref="string"/>.</param>
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
    public static T Debug<T>(
        this T value,
        bool shouldPrettify = true,
        bool shouldLogExpression = true,
        [InstantHandle] Converter<T, object?>? map = null,
        [InstantHandle] Predicate<T>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
    {
        const string
            Indent = "\n        ",
            Of = $"{Indent}of ";

#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
#pragma warning disable 8500
        static unsafe StringBuilder Accumulator(StringBuilder accumulator, scoped ReadOnlySpan<char> next)
        {
            var trimmed = next.Trim();

            fixed (char* ptr = &trimmed[0])
                accumulator.Append(ptr, trimmed.Length).Append(' ');

            return accumulator;
        }
#pragma warning restore 8500
#endif

        if (!(filter ?? (_ => true))(value))
            return value;

        logger ??= Write;

        // ReSharper disable ExplicitCallerInfoArgument InvokeAsExtensionMethod RedundantNameQualifier
        var stringified = (map ?? (x => x))(value) switch
        {
            var x when typeof(T) == typeof(string) && !(shouldLogExpression = false) => x,
            string x => x,
            var x when shouldPrettify => Stringifier.Stringify(x).Prettify(),
            var x => Stringifier.Stringify(x),
        };

        var location = shouldLogExpression
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
            ? expression?.Collapse().SplitLines().Aggregate(new StringBuilder(Of), Accumulator)
#else
            ? expression
              ?.Collapse()
               .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(x => x.Trim())
               .Prepend(Of)
               .Conjoin("")
#endif
            : default;

        var log = $"{stringified}{location}{Indent}at {member} in {Path.GetFileName(path)}:line {line}";
        logger(log);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, System.Predicate{T}?, System.Action{string}?, string?, string?, int, string?)"/>
    [return: NotNullIfNotNull(nameof(value))]
    public static T Debug<T, TAs>(
        this T value,
        bool shouldPrettify = true,
        bool shouldLogExpression = true,
        [InstantHandle] Converter<T, TAs?>? map = null,
        [InstantHandle] Predicate<T>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
    {
        if (!(filter ?? (_ => true))(value))
            return value;

        var stringified = (map ?? (x => x is TAs t ? t : default))(value) switch
        {
            var x when typeof(T) == typeof(string) && !(shouldLogExpression = false) => x as object,
            string x => x,
            var x when shouldPrettify => Stringifier.Stringify(x).Prettify(),
            var x => Stringifier.Stringify(x),
        };

        Debug(
            stringified,
            false,
            shouldLogExpression,
            logger: logger,
            expression: expression,
            path: path,
            line: line,
            member: member
        );

        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, System.Predicate{T}?, System.Action{string}?, string?, string?, int, string?)"/>
    public static Span<T> Debug<T>(
        this Span<T> value,
        bool shouldPrettify = true,
        bool shouldLogExpression = false,
        [InstantHandle] Converter<T[], object?>? map = null,
        [InstantHandle] Predicate<T[]>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        // ReSharper disable ExplicitCallerInfoArgument
        _ = value
           .ToArray()
           .Debug(shouldPrettify, shouldLogExpression, map, filter, logger, expression, path, line, member);

        // ReSharper restore ExplicitCallerInfoArgument
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, System.Predicate{T}?, System.Action{string}?, string?, string?, int, string?)"/>
    public static SplitSpan<T> Debug<T>(
        this SplitSpan<T> value,
        bool shouldPrettify = true,
        bool shouldLogExpression = false,
        [InstantHandle] Converter<List<T[]>, object?>? map = null,
        [InstantHandle] Predicate<List<T[]>>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        // ReSharper disable ExplicitCallerInfoArgument
        _ = value
           .ToList()
           .Debug(shouldPrettify, shouldLogExpression, map, filter, logger, expression, path, line, member);

        // ReSharper restore ExplicitCallerInfoArgument
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, System.Predicate{T}?, System.Action{string}?, string?, string?, int, string?)"/>
    public static ReadOnlySpan<T> Debug<T>(
        this ReadOnlySpan<T> value,
        bool shouldPrettify = true,
        bool shouldLogExpression = false,
        [InstantHandle] Converter<T[], object?>? map = null,
        [InstantHandle] Predicate<T[]>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        // ReSharper disable ExplicitCallerInfoArgument
        _ = value
           .ToArray()
           .Debug(shouldPrettify, shouldLogExpression, map, filter, logger, expression, path, line, member);

        // ReSharper restore ExplicitCallerInfoArgument
        return value;
    }

    /// <summary>Executes an <see cref="Action{T}"/>, and returns the argument.</summary>
    /// <typeparam name="T">The type of value and action parameter.</typeparam>
    /// <param name="value">The value to pass into the callback.</param>
    /// <param name="action">The callback to perform.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    public static T Peek<T>(this T value, [InstantHandle] Action<T> action)
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
    public static unsafe T Peek<T>(this T value, [InstantHandle] delegate*<T, void> call)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (call is not null)
            call(value);

        return value;
    }
#endif

    /// <summary>Executes the function, and returns the result.</summary>
    /// <typeparam name="T">The type of value and input parameter.</typeparam>
    /// <typeparam name="TResult">The type of output and return value.</typeparam>
    /// <param name="value">The value to pass into the callback.</param>
    /// <param name="converter">The callback to perform.</param>
    /// <returns>The return value of <paramref name="converter"/> after passing in <paramref name="value"/>.</returns>
    public static TResult Then<T, TResult>(this T value, [InstantHandle] Converter<T, TResult> converter) =>
        converter(value);
}
