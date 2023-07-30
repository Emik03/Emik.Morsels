// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD1_3_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable once RedundantUsingDirective
using static Peeks;
using static Span;

/// <summary>An extremely bad logger.</summary>
sealed partial class BadLogger : IDisposable
{
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Clear"/>
#endif
    static readonly byte[] s_clear = { 0x1b, 0x5b, 0x48, 0x1b, 0x5b, 0x32, 0x4a, 0x1b, 0x5b, 0x33, 0x4a };
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Gets the buffer that clears the console when printed. Only on certain Linux terminals though.</summary>
    /// <remarks><para>
    /// Buffer is based on the following command:
    /// </para><code>
    /// ~$ clear | xxd
    /// 00000000: 1b5b 481b 5b32 4a1b 5b33 4a.[H.[2J.[3J
    /// </code></remarks>
    [Pure]
    public static ReadOnlySpan<byte> Clear => s_clear;
#endif

    /// <summary>Gets the default interval when one is left unspecified.</summary>
    [Pure]
    public static TimeSpan DefaultInterval { get; } = TimeSpan.FromMilliseconds(100);

    readonly Stopwatch _stopwatch = new();

    readonly FileStream _stream;

    readonly TimeSpan _interval;

    TimeSpan _last;

    /// <summary>Initializes a new instance of the <see cref="BadLogger"/> class.</summary>
    /// <param name="path">The path to the file to write.</param>
    /// <param name="interval">The rate of flushing the stream.</param>
    public BadLogger(string path = "/tmp/morsels.log", TimeSpan? interval = null)
    {
        _interval = interval ?? DefaultInterval;
        (_stream = File.OpenWrite(path)).SetLength(0);
        _stream.Write(s_clear, 0, s_clear.Length);
        OnWrite += Log;
        "..".Debug();
        _stopwatch.Start();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stopwatch.Stop();
        "(◕_◕)🎉".Debug();
        _stopwatch.ElapsedMilliseconds.ToString("0ms").Debug();
        OnWrite -= Log;
        _stream.Dispose();
    }

    /// <summary>Logs the message.</summary>
    /// <param name="entry">The entry to log.</param>
    public void Log(string entry)
    {
        var log = $"[{DateTime.Now:HH:mm:ss.fff}]: {entry}\n";
#if NETCOREAPP3_0_OR_GREATER
        const int MaxBytesInUtf16 = 3;
        Allocate(log.Length * MaxBytesInUtf16, (this, log), Write());
#else
        var bytes = Encoding.UTF8.GetBytes(log);
        _stream.Write(bytes, 0, bytes.Length);
#endif
        TryFlush();
    }

    /// <summary>Attempts to flush if enough time has elapsed.</summary>
    /// <returns>Whether it flushed.</returns>
    public bool TryFlush()
    {
        if (_stopwatch.Elapsed is var elapsed && elapsed - _last <= _interval)
            return false;

        _last = elapsed;
        _stream.Flush();
        return true;
    }

    /// <summary>Produces the side effect specified by the passed in <see cref="Action"/>.</summary>
    /// <param name="action">The <see cref="Action"/>.</param>
    /// <returns>Itself.</returns>
    public BadLogger Try([InstantHandle] Action action)
    {
        try
        {
            action();
            return this;
        }
        catch (Exception ex)
        {
            $"{ex}".Debug();
            throw;
        }
    }

    /// <summary>Produces the side effect specified by the passed in <see cref="Action"/>.</summary>
    /// <typeparam name="T">The type of <paramref name="context"/>.</typeparam>
    /// <param name="action">The <see cref="Action"/>.</param>
    /// <param name="context">The context value.</param>
    /// <returns>Itself.</returns>
    public BadLogger Try<T>([InstantHandle] Action<T> action, T context)
    {
        try
        {
            action(context);
            return this;
        }
        catch (Exception ex)
        {
            $"{ex}".Debug();
            throw;
        }
    }
#if NETCOREAPP3_0_OR_GREATER
    static SpanAction<byte, (BadLogger, string)> Write() =>
        static (span, tuple) =>
        {
            var (that, log) = tuple;
            Utf8.FromUtf16(log, span, out _, out var wrote);
            that._stream.Write(span[..wrote]);
        };
#endif
}
#endif
