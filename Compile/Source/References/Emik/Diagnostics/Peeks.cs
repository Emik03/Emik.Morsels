// SPDX-License-Identifier: MPL-2.0
#pragma warning disable CS8632, RCS1196

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to use callbacks within a statement.</summary>
#pragma warning disable MA0048
static partial class Peeks
#pragma warning restore MA0048
{
#if ROSLYN // ReSharper disable once RedundantExtendsListEntry
    /// <summary>The Serilog sink that creates <see cref="Diagnostic"/> instances.</summary>
    public sealed partial class DiagnosticSink : ILogEventSink
    {
        /// <summary>Contains the state of the <see cref="Accumulator"/>.</summary>
        /// <param name="Builder">The resulting <see cref="string"/>.</param>
        /// <param name="List">The list of property names.</param>
        public sealed record Accumulator(StringBuilder Builder, SmallList<string> List = default)
        {
            int _index;

            /// <summary>Steps the <see cref="Accumulator"/> forward.</summary>
            /// <param name="accumulator">The accumulator.</param>
            /// <param name="token">The token to process.</param>
            /// <returns>The parameter <paramref name="accumulator"/>.</returns>
            public static Accumulator Next(Accumulator accumulator, MessageTemplateToken token) =>
                accumulator.Next(token);

            /// <summary>Steps the <see cref="Accumulator"/> forward.</summary>
            /// <param name="token">The token to process.</param>
            /// <returns>Itself.</returns>
            public Accumulator Next(MessageTemplateToken token)
            {
                if (token is TextToken { Text: var text })
                {
                    Builder.Append(text);
                    return this;
                }

                if (token is not PropertyToken property)
                    throw Unreachable;

                Builder.Append('{').Append(_index++).Append('}');
                List.Add(property.PropertyName);
                return this;
            }
        }

        /// <summary>Gets the logged diagnostics.</summary>
        [Pure]
        public ConcurrentQueue<Diagnostic> UnreportedDiagnostics { get; } = [];

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            var (builder, list) = logEvent.MessageTemplate.Tokens.Aggregate(new Accumulator(new()), Accumulator.Next);

            var descriptor = new DiagnosticDescriptor(
                nameof(DiagnosticSink),
                nameof(DiagnosticSink),
                $"{builder}",
                nameof(DiagnosticSink),
                ToDiagnosticSeverity(logEvent.Level),
                true
            );

            var properties = logEvent.Properties.Select(x => x.Value);
            var (first, rest) = Flatten(properties).OfType<ScalarValue>().Select(x => x.Value).OfType<Location>();
            var args = list.Select(x => (object)logEvent.Properties[x]).ToArray();
            var diagnostic = Diagnostic.Create(descriptor, first, rest, args);

            UnreportedDiagnostics.Enqueue(diagnostic);
        }

        static IEnumerable<LogEventPropertyValue> Flatten(IEnumerable<LogEventPropertyValue> values) =>
            values.SelectMany(
                x => x switch
                {
                    ScalarValue v => v.Yield(),
                    SequenceValue v => Flatten(v.Elements),
                    DictionaryValue v => v.Elements.SelectMany(x => Flatten([x.Key, x.Value])),
                    StructureValue v => Flatten(v.Properties.Select(x => x.Value)),
                    _ => [],
                }
            );

        static DiagnosticSeverity ToDiagnosticSeverity(LogEventLevel level) =>
            level switch
            {
                LogEventLevel.Debug => DiagnosticSeverity.Hidden,
                LogEventLevel.Error => DiagnosticSeverity.Error,
                LogEventLevel.Fatal => DiagnosticSeverity.Error,
                LogEventLevel.Information => DiagnosticSeverity.Info,
                LogEventLevel.Verbose => DiagnosticSeverity.Hidden,
                LogEventLevel.Warning => DiagnosticSeverity.Warning,
                _ => throw Unreachable,
            };
    }

    static readonly DiagnosticSink s_diagnosticSink = new();
#endif
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
    static readonly string s_debugFile = Path.Combine(Path.GetTempPath(), "morsels.log");
#if DEBUG
    static Peeks() => File.Create(s_debugFile).Dispose();
#endif
#endif

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
#if NETFRAMEWORK || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    /// <summary>Gets all of the types currently loaded.</summary>
    [Pure]
    public static IEnumerable<Type> AllTypes =>
        AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.TryGetTypes());
#endif
#if ROSLYN
    /// <inheritdoc cref="DiagnosticSink.UnreportedDiagnostics"/>
    [Pure]
    public static ConcurrentQueue<Diagnostic> Diagnostics => s_diagnosticSink.UnreportedDiagnostics;

    /// <summary>Gets the dummy diagnostic.</summary>
    [Pure]
    public static DiagnosticDescriptor Dummy { get; } = new(
        nameof(DiagnosticSink),
        nameof(DiagnosticSink),
        "",
        nameof(DiagnosticSink),
        DiagnosticSeverity.Error,
        true
    );
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
        // ReSharper disable once InvocationIsSkipped RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.WriteLine(message);
#if !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        Trace.WriteLine(message);
#endif
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
        if (File.Exists(s_debugFile))
            File.AppendAllText(s_debugFile, $"[{DateTime.Now.ToLongTimeString()}]: {message}\n");
#endif
    }

    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <param name="message">The value to send a message.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    public static void Write(this string message) => (OnWrite ?? throw new InvalidOperationException(message))(message);
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    /// <summary>Quick and dirty debugging function, invokes <see cref="OnWrite"/>.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to stringify.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="OnWrite"/> is <see langword="null"/>, which can only happen if
    /// every callback has been manually removed as it is always valid by default.
    /// </exception>
    // ReSharper disable once InvokeAsExtensionMethod
    public static void Write<T>(T value) => Write(Stringifier.Stringify(value));
#if NET462_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
#if RELEASE && !CSHARPREPL
    /// <summary>Write a log event with the <see cref="LogEventLevel.Debug"/> level.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="x">The value to write.</param>
    /// <param name="map">When specified, overrides the value that is logged.</param>
    /// <returns>The parameter <paramref name="x"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Debug<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Debug<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Debug<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Debug<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Debug<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Error"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Error<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Error<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Error<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Error<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Error<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Fatal"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Fatal<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Fatal<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Fatal<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Fatal<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Fatal<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Information"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Info<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Info<T>(this Span<T> x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Info<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Info<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Info<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Verbose"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Verbose<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Verbose<T>(this Span<T> x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Verbose<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Verbose<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Verbose<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object})"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Warning"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Warn<T>(this T x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null) => x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Warn<T>(this Span<T> x, [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Warn<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Warn<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
        =>
            x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Warn<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;
#else
    /// <summary>Write a log event with the <see cref="LogEventLevel.Debug"/> level.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="x">The value to write.</param>
    /// <param name="map">When specified, overrides the value that is logged.</param>
    /// <param name="e">Automatically filled by compilers; the source code of <paramref name="x"/>.</param>
    /// <param name="path">Automatically filled by compilers; the file's path where this method was called.</param>
    /// <param name="name">Automatically filled by compilers; the member's name where this method was called.</param>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <returns>The parameter <paramref name="x"/>.</returns>
    public static T Debug<T>(
        this T x,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(x))] string? e = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(x, map, e, path, name, line, LogEventLevel.Debug);

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Debug<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Debug<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Debug<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Debug<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Error"/> level.</summary>
    public static T Error<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, expression, path, name, line, LogEventLevel.Error);

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Error<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Error<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Error<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Error<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Fatal"/> level.</summary>
    public static T Fatal<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, expression, path, name, line, LogEventLevel.Fatal);

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Fatal<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Fatal<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Fatal<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Fatal<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Information"/> level.</summary>
    public static T Info<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, expression, path, name, line, LogEventLevel.Information);

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Info<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Info<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Info<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Info<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Verbose"/> level.</summary>
    public static T Verbose<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, expression, path, name, line, LogEventLevel.Verbose);

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Verbose<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Verbose<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Verbose<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Verbose<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Warning"/> level.</summary>
    public static T Warn<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, expression, path, name, line, LogEventLevel.Warning);

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static Span<T> Warn<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static PooledSmallList<T> Warn<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Warn<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        Do(value.ToArrays(), map, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, string, string, string, int)"/>
    public static ReadOnlySpan<T> Warn<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    static T Do<T>(
        T value,
        [InstantHandle] Converter<T, object?>? map,
        string? e,
        string? path,
        string? name,
        int line,
        LogEventLevel level
    )
    {
        static void EnsureLoggerIsInitialized()
        {
            const char Eval = '\u211B';

            // ReSharper disable once RedundantNameQualifier
            if (Log.Logger != Serilog.Core.Logger.None || typeof(Assert).Assembly.GetName().Name is not [var first, ..] name)
                return;

            var path = Path.Combine(Path.GetTempPath(), first is Eval ? new(Eval, 1) : name);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Is(LogEventLevel.Verbose)
#if ROSLYN
               .WriteTo.Sink(s_diagnosticSink)
#else
               .WriteTo.Console()
#endif
               .WriteTo.File(Path.ChangeExtension(path, "log"))
               .WriteTo.File(new CompactJsonFormatter(), Path.ChangeExtension(path, "clef"))
               .CreateLogger();
        }

        EnsureLoggerIsInitialized();

        if (!Log.IsEnabled(level))
            return value;

        var f = path.FileName();
        var t = value?.GetType() ?? typeof(T);
        var x = (map ?? (x => x))(value);

        if (typeof(T) == typeof(string) || typeof(T).IsPrimitive || value is ICustomAttributeProvider)
            if (f is { Length: 0 })
                Log.Write(level, "[{@Member}:{@Line} ({@Expression})] {@Value}", name, line, e, x);
            else
                Log.Write(level, "[{$File}.{@Member}:{@Line} ({@Expression})] {@Value}", f, name, line, e, x);
        else if (f is { Length: 0 })
            Log.Write(level, "[{@Member}:{@Line}, {@Expression}] {@Type} {$Value}", name, line, e, t, x);
        else
            Log.Write(level, "[{$File}.{@Member}:{@Line}, {@Expression}] {@Type} {$Value}", f, name, line, e, t, x);

        return value;
    }
#endif
#else
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

        var logExpression = shouldLogExpression ? expression.CollapseToSingleLine(Of) : "";
        var log = $"{stringified}{logExpression}{Indent}at {member} in {path.FileName()}:line {line}";
        logger(log);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, Predicate{T}?, Action{string}?, string?, string?, int, string?)"/>
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

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, Predicate{T}?, Action{string}?, string?, string?, int, string?)"/>
    public static Span<T> Debug<T>(
        this in Span<T> value,
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
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, Predicate{T}?, Action{string}?, string?, string?, int, string?)"/>
    public static PooledSmallList<T> Debug<T>(
        this in PooledSmallList<T> value,
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
           .View
           .ToArray()
           .Debug(shouldPrettify, shouldLogExpression, map, filter, logger, expression, path, line, member);

        // ReSharper restore ExplicitCallerInfoArgument
        return value;
    }
#endif

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, Predicate{T}?, Action{string}?, string?, string?, int, string?)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Debug<TBody, TSeparator, TStrategy>(
        this in SplitSpan<TBody, TSeparator, TStrategy> value,
        bool shouldPrettify = true,
        bool shouldLogExpression = false,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [InstantHandle] Predicate<TBody[][]>? filter = null,
        [InstantHandle] Action<string>? logger = null,
        [CallerArgumentExpression(nameof(value))] string? expression = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int line = default,
        [CallerMemberName] string? member = null
    )
#if UNMANAGED_SPAN
        where TBody : unmanaged, IEquatable<TBody>
#else
        where TBody : IEquatable<TBody>?
#endif
#if !NET7_0_OR_GREATER
        where TSeparator : IEquatable<TSeparator>?
#endif
    {
        // ReSharper disable ExplicitCallerInfoArgument
        _ = value
           .ToArrays()
           .Debug(shouldPrettify, shouldLogExpression, map, filter, logger, expression, path, line, member);

        // ReSharper restore ExplicitCallerInfoArgument
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, bool, bool, Converter{T, object?}?, Predicate{T}?, Action{string}?, string?, string?, int, string?)"/>
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
#endif
#endif

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
