// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static Peeks;

/// <summary>An extremely bad logger.</summary>
sealed partial class BadLogger : IDisposable
{
    /// <inheritdoc cref="Clear"/>
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
    public ReadOnlySpan<byte> Clear => s_clear;
#endif
    readonly Stopwatch _stopwatch = new();

    readonly FileStream _stream;

    /// <summary>Initializes a new instance of the <see cref="BadLogger"/> class.</summary>
    /// <param name="path">The path to the file to write.</param>
    public BadLogger(string path = "/tmp/morsels.log")
    {
        (_stream = File.OpenWrite(path)).SetLength(0);
        _stream.Write(s_clear);
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
        _stream.Close();
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

    void Log(string entry)
    {
        _stream.Write(Encoding.UTF8.GetBytes($"[{DateTime.Now:HH:mm:ss.fff}]: {entry}\n"));
        _stream.Flush(true);
    }
}
