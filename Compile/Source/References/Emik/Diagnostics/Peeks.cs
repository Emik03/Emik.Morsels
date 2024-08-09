// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to use callbacks within a statement.</summary>
static partial class Peeks
{
#if !RELEASE
#if ROSLYN // ReSharper disable once RedundantExtendsListEntry
    /// <summary>The Serilog sink that creates <see cref="Diagnostic"/> instances.</summary>
    public sealed partial class DiagnosticSink : ILogEventSink
    {
        /// <summary>Contains the state of the <see cref="Accumulator"/>.</summary>
        public sealed class Accumulator
        {
            /// <summary>The maximum number of arguments when called from <see cref="Peeks.Do{T}"/>.</summary>
            const int UsualMaxCapacity = 5;

            readonly List<string> _list = new(UsualMaxCapacity);

            readonly StringBuilder _builder = new();

            int _index;

            /// <summary>Gets the message template.</summary>
            public string Template => $"{_builder}";

            /// <summary>Gets the list of property names.</summary>
            public IReadOnlyList<string> Names => _list;

            /// <summary>Steps the <see cref="Accumulator"/> forward.</summary>
            /// <param name="accumulator">The accumulator.</param>
            /// <param name="token">The token to process.</param>
            /// <returns>The parameter <paramref name="accumulator"/>.</returns>
            public static Accumulator Next(Accumulator accumulator, MessageTemplateToken token) =>
                accumulator.Next(token);

            /// <summary>Deconstructs the <see cref="Accumulator"/>.</summary>
            /// <param name="template">The message template.</param>
            /// <param name="names">The list of property names. </param>
            public void Deconstruct(out string template, out IReadOnlyList<string> names) =>
                (template, names) = (Template, Names);

            /// <summary>Steps the <see cref="Accumulator"/> forward.</summary>
            /// <param name="token">The token to process.</param>
            /// <returns>Itself.</returns>
            public Accumulator Next(MessageTemplateToken token)
            {
                if (token is TextToken { Text: var text })
                {
                    _builder.Append(text);
                    return this;
                }

                if (token is not PropertyToken property)
                    throw Unreachable;

                _builder.Append('{').Append(_index++).Append('}');
                _list.Add(property.PropertyName);
                return this;
            }
        }

        /// <summary>Gets the logged diagnostics.</summary>
        [Pure]
        public ConcurrentQueue<Diagnostic> UnreportedDiagnostics { get; } = [];

        /// <summary>Gets or sets the additional locations.</summary>
        [Pure]
        public IEnumerable<Location>? AdditionalLocations { get; set; }

        /// <summary>Gets or sets the location.</summary>
        [Pure]
        public Location Location { get; set; } = Location.None;

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            var (template, list) = logEvent.MessageTemplate.Tokens.Aggregate(new Accumulator(), Accumulator.Next);
            var level = ToDiagnosticSeverity(logEvent.Level);
            DiagnosticDescriptor descriptor = new(Name, $"{s_guid}", template, Name, level, true);
            var args = list.Select(x => (object?)logEvent.Properties[x]).ToArray();
            var diagnostic = Diagnostic.Create(descriptor, Location, AdditionalLocations, args);

            UnreportedDiagnostics.Enqueue(diagnostic);
        }

        static DiagnosticSeverity ToDiagnosticSeverity(LogEventLevel level) =>
            level switch
            {
                LogEventLevel.Debug => DiagnosticSeverity.Info,
                LogEventLevel.Error => DiagnosticSeverity.Error,
                LogEventLevel.Fatal => DiagnosticSeverity.Error,
                LogEventLevel.Information => DiagnosticSeverity.Info,
                LogEventLevel.Verbose => DiagnosticSeverity.Info,
                LogEventLevel.Warning => DiagnosticSeverity.Warning,
                _ => throw Unreachable,
            };
    }
#endif

    /// <summary>The character often used to identify scripted assemblies.</summary>
    public const char R = '\u211b';

    /// <summary>The escape sequence to clear the screen.</summary>
    // ReSharper disable CanSimplifyStringEscapeSequence
    public const string Clear = "\x1b\x5b\x48\x1b\x5b\x32\x4a\x1b\x5b\x33\x4a";
#if ROSLYN // ReSharper restore CanSimplifyStringEscapeSequence
    const string Name = nameof(DiagnosticSink);

    static readonly DiagnosticSink s_diagnosticSink = new();

    static readonly Guid s_guid = Guid.NewGuid();
#endif
#if NET462_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    static readonly string s_path = Path.Combine(
        Path.GetTempPath(),
        typeof(Assert).Assembly.GetName().Name is [not R, ..] name ? name : $"{R}"
    ); // ReSharper disable once RedundantNameQualifier

    static readonly ITextFormatter s_json =
#if CSHARPREPL
        new JsonFormatter();
#else
        new CompactJsonFormatter();
#endif // ReSharper disable once RedundantNameQualifier
    static readonly Serilog.Core.Logger
        s_clef = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.File(s_json, $"{s_path}.clef").CreateLogger(),
#if ROSLYN
        s_roslyn = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(s_diagnosticSink).CreateLogger();
#else
        s_console = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console().CreateLogger();
#endif
#endif
#endif
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
    static readonly string s_debugFile = Path.Combine(Path.GetTempPath(), "morsels.log");
#if !RELEASE && !CSHARPREPL
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
        Shout +
#if KTANE
        UnityEngine.Debug.Log +
#endif
        Console.WriteLine;
#endif
#if NETFRAMEWORK || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    /// <summary>Gets all the types currently loaded.</summary>
    [Pure]
    public static IEnumerable<Type> AllTypes =>
        AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.TryGetTypes());
#endif
#if !RELEASE && ROSLYN
    /// <inheritdoc cref="DiagnosticSink.UnreportedDiagnostics"/>
    [Pure]
    public static ConcurrentQueue<Diagnostic> Diagnostics => s_diagnosticSink.UnreportedDiagnostics;

    /// <summary>Gets the placeholder diagnostic.</summary>
    [Pure]
    public static DiagnosticDescriptor Bare { get; } = new(Name, $"{s_guid}", "", Name, DiagnosticSeverity.Error, true);
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
        // ReSharper disable once RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.WriteLine(message);
#if !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
#pragma warning disable S6670
        Trace.WriteLine(message);
#pragma warning restore S6670
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
#pragma warning disable RCS1196
    public static void Write<T>(T value) => Write(Stringifier.Stringify(value));
#pragma warning restore RCS1196
#if NET462_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
#if RELEASE && !CSHARPREPL
#if ROSLYN
    /// <inheritdoc cref="Mark(Location, IEnumerable{Location})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Location Mark(this Location location, [UsedImplicitly] params Location[]? additionalLocations) =>
        location;

    /// <summary>Marks the location in the next set of lints.</summary>
    /// <param name="location">The primary location.</param>
    /// <param name="additionalLocations">Additional locations.</param>
    /// <returns>The parameter <paramref name="location"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Location Mark(this Location location, [UsedImplicitly] IEnumerable<Location>? additionalLocations) =>
        location;
#endif

    /// <summary>Write a log event with the <see cref="LogEventLevel.Debug"/> level.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="x">The value to write.</param>
    /// <param name="map">When specified, overrides the value that is logged.</param>
    /// <param name="visit">The maximum number of times to recurse through an enumeration.</param>
    /// <param name="str">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="recurse">The maximum number of times to recurse a nested object or dictionary.</param>
    /// <returns>The parameter <paramref name="x"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Debug<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurse = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Debug<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurse = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Debug<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurse = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Debug<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurse = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Debug<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurse = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Error"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Error<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Error<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Error<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Error<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Error<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Fatal"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Fatal<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Fatal<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Fatal<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Fatal<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Fatal<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Information"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Info<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Info<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Info<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Info<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Info<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Verbose"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Verbose<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Verbose<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Verbose<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Verbose<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Verbose<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Warning"/> level.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Warn<T>(
        this T x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    ) =>
        x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Warn<T>(
        this Span<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PooledSmallList<T> Warn<T>(
        this PooledSmallList<T> x,
        [InstantHandle, UsedImplicitly] Converter<T, object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitSpan<TBody, TSeparator, TStrategy> Warn<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> x,
        [InstantHandle, UsedImplicitly] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
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

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Warn<T>(
        this ReadOnlySpan<T> x,
        [InstantHandle, UsedImplicitly] Converter<T[], object?>? map = null,
        [NonNegativeValue, UsedImplicitly] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue, UsedImplicitly] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue, UsedImplicitly] int recurseLength = DeconstructionCollection.DefaultRecurseLength
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            x;
#else
#if ROSLYN
    /// <inheritdoc cref="Mark(Location, IEnumerable{Location})"/>
    public static Location Mark(this Location location, params Location[]? additionalLocations) =>
        Mark(location, (IEnumerable<Location>?)additionalLocations);

    /// <summary>Marks the location in the next set of lints.</summary>
    /// <param name="location">The primary location.</param>
    /// <param name="additionalLocations">Additional locations.</param>
    /// <returns>The parameter <paramref name="location"/>.</returns>
    public static Location Mark(this Location location, IEnumerable<Location>? additionalLocations)
    {
        s_diagnosticSink.Location = location;
        s_diagnosticSink.AdditionalLocations = additionalLocations;
        return location;
    }
#endif

    /// <summary>Write a log event with the <see cref="LogEventLevel.Debug"/> level.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="x">The value to write.</param>
    /// <param name="map">When specified, overrides the value that is logged.</param>
    /// <param name="visitLength">The maximum number of times to recurse through an enumeration.</param>
    /// <param name="stringLength">The maximum length of any given <see cref="string"/>.</param>
    /// <param name="recurseLength">The maximum number of times to recurse a nested object or dictionary.</param>
    /// <param name="expression">Automatically filled by compilers; the source code of <paramref name="x"/>.</param>
    /// <param name="path">Automatically filled by compilers; the file's path where this method was called.</param>
    /// <param name="name">Automatically filled by compilers; the member's name where this method was called.</param>
    /// <param name="line">Automatically filled by compilers; the line number where this method was called.</param>
    /// <returns>The parameter <paramref name="x"/>.</returns>
    public static T Debug<T>(
        this T x,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(x))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(x, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Debug);

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Debug<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Debug<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Debug<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Debug<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Debug);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Error"/> level.</summary>
    public static T Error<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Error);

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Error<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Error<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Error<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Error{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Error<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Error);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Fatal"/> level.</summary>
    public static T Fatal<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Fatal);

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Fatal<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Fatal<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Fatal<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Fatal{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Fatal<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Fatal);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Information"/> level.</summary>
    public static T Info<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Information);

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Info<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Info<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Info<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Info{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Info<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Information);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Verbose"/> level.</summary>
    public static T Verbose<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Verbose);

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Verbose<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Verbose<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Verbose<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Verbose{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Verbose<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visitLength = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int stringLength = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurseLength = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visitLength, stringLength, recurseLength, expression, path, name, line, LogEventLevel.Verbose);
        return value;
    }

    /// <inheritdoc cref="Debug{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    /// <summary>Write a log event with the <see cref="LogEventLevel.Warning"/> level.</summary>
    public static T Warn<T>(
        this T value,
        [InstantHandle] Converter<T, object?>? map = null,
        [NonNegativeValue] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurse = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    ) =>
        Do(value, map, visit, str, recurse, expression, path, name, line, LogEventLevel.Warning);

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static Span<T> Warn<T>(
        this Span<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurse = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visit, str, recurse, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static PooledSmallList<T> Warn<T>(
        this PooledSmallList<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurse = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArrayLazily, map, visit, str, recurse, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static SplitSpan<TBody, TSeparator, TStrategy> Warn<TBody, TSeparator, TStrategy>(
        this SplitSpan<TBody, TSeparator, TStrategy> value,
        [InstantHandle] Converter<TBody[][], object?>? map = null,
        [NonNegativeValue] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurse = DeconstructionCollection.DefaultRecurseLength,
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
        Do(value.ToArrays(), map, visit, str, recurse, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    /// <inheritdoc cref="Warn{T}(T, Converter{T, object}, int, int, int, string, string, string, int)"/>
    public static ReadOnlySpan<T> Warn<T>(
        this ReadOnlySpan<T> value,
        [InstantHandle] Converter<T[], object?>? map = null,
        [NonNegativeValue] int visit = DeconstructionCollection.DefaultVisitLength,
        [NonNegativeValue] int str = DeconstructionCollection.DefaultStringLength,
        [NonNegativeValue] int recurse = DeconstructionCollection.DefaultRecurseLength,
        [CallerArgumentExpression(nameof(value))] string? expression = "",
        [CallerFilePath] string? path = null,
        [CallerMemberName] string? name = null,
        [CallerLineNumber] int line = default
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        Do(value.ToArray(), map, visit, str, recurse, expression, path, name, line, LogEventLevel.Warning);
        return value;
    }

    static T Do<T>(
        T value,
        [InstantHandle] Converter<T, object?>? map,
        [NonNegativeValue] int visitLength,
        [NonNegativeValue] int stringLength,
        [NonNegativeValue] int recurseLength,
        string? expression,
        string? path,
        string? name,
        int line,
        LogEventLevel level
    )
    {
        static object? Memory(T value) =>
            value switch
            {
                null => null,
#if ROSLYN || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                Memory<char> m => m.ToString(),
                ReadOnlyMemory<char> m => m.ToString(),
                _ when value.GetType().GetMethod(nameof(Memory<int>.ToArray), []) is
                    {
                        IsStatic: false, ReturnParameter.ParameterType: var ret, IsGenericMethod: false,
                    } method &&
                    ret != typeof(void) => method.Invoke(value, null),
#endif
                _ => value,
            };

        var x = (map ?? Memory)(value).ToDeconstructed(visitLength, stringLength, recurseLength) is var deconstructed &&
            deconstructed is DeconstructionCollection { Serialized: var serialized }
                ? serialized
                : deconstructed;
#if ROSLYN
        var y = (x as DeconstructionCollection)?.ToStringWithoutNewLines() ?? x;
#endif
        if (expression.CollapseToSingleLine() is var ex && path.FileName() is not { Length: not 0 } file)
        {
            s_clef.Write(level, "[{@Member}:{@Line} ({@Expression})] {@Value}", name, line, ex, x);
#if ROSLYN
            s_roslyn.Write(level, "[{@Member}:{@Line} ({@Expression})] {@Value}", name, line, ex, y);
#else
            s_console.Write(level, "[{@Member}:{@Line} ({@Expression})] {@Value}", name, line, ex, x);
#endif
            return value;
        }

        s_clef.Write(level, "[{$File}.{@Member}:{@Line} ({@Expression})] {@Value}", file, name, line, ex, x);
#if ROSLYN
        s_roslyn.Write(level, "[{$File}.{@Member}:{@Line} ({@Expression})] {@Value}", file, name, line, ex, y);
#else
        s_console.Write(level, "[{$File}.{@Member}:{@Line} ({@Expression})] {@Value}", file, name, line, ex, x);
#endif
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
