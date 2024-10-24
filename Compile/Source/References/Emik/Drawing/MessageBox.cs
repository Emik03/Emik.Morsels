// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

static partial class MessageBox
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe partial struct MessageBoxData(
        uint flags,
        nint window,
        string? title,
        string? message,
        int numButtons,
        MessageBoxData.Button* buttons
    )
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly partial struct Button(uint flags, int buttonId, byte* text)
        {
            readonly uint _flags = flags;

            readonly int _buttonId = buttonId;

            readonly byte* _text = text;
        }

        readonly uint _flags = flags;

        readonly nint _window = window;

        readonly string? _title = title, _message = message;

        readonly int _numButtons = numButtons;

        readonly Button* _buttons = buttons;

        readonly nint _colorScheme;
    }

    /// <summary>Displays a message box with an error icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowError(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, 0, buttons, 16);

    /// <summary>Displays a message box with an error icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowError(
        this string? title,
        string? message,
        nint window,
        ReadOnlySpan<string> buttons = default
    ) =>
        Show(title, message, window, buttons, 16);

    /// <summary>Displays a message box with an informational icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowInfo(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, 0, buttons, 64);

    /// <summary>Displays a message box with an informational icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowInfo(
        this string? title,
        string? message,
        nint window,
        ReadOnlySpan<string> buttons = default
    ) =>
        Show(title, message, window, buttons, 64);

    /// <summary>Displays a message box with a warning icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowWarn(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, 0, buttons, 32);

    /// <summary>Displays a message box with a warning icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowWarn(
        this string? title,
        string? message,
        nint window,
        ReadOnlySpan<string> buttons = default
    ) =>
        Show(title, message, window, buttons, 32);

    /// <summary>Displays a message box.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <param name="flags">The flags for the message box.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    static unsafe int Show(string? title, string? message, nint window, ReadOnlySpan<string> buttons, uint flags)
    {
        [DllImport("sdl2", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Else(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("libSDL2-2.0.so.0", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Linux(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("libSDL2.dylib", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int OSX(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("SDL2.dll", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Windows(ref MessageBoxData messageBoxData, out int buttonId);

        const int Flags = 3;
        var nonZeroLength = Math.Max(buttons.Length, 1);
        using var _ = nonZeroLength.Alloc(out Span<Rented<byte>.Pinned> pins);
        using var __ = nonZeroLength.Alloc(out MessageBoxData.Button* buttonDatas);

        if (buttons.IsEmpty)
        {
            pins[0] = new(3, out var bytes);
            bytes[0] = (byte)'O';
            bytes[1] = (byte)'K';
            bytes[2] = (byte)'\0';
            buttonDatas[0] = new(Flags, 0, bytes);
        }
        else
            for (var i = 0; i < buttons.Length; i++)
                fixed (char* chars = buttons[i])
                {
                    var length = buttons[i].Length * 4 + 1;
                    pins[i] = new(length, out var bytes);
                    bytes[Encoding.UTF8.GetBytes(chars, buttons[i].Length, bytes, length)] = 0;
                    buttonDatas[i] = new(Flags, i, bytes);
                }

        try
        {
            MessageBoxData data = new(flags, window, title, message, nonZeroLength, buttonDatas);
#if NET5_0_OR_GREATER
            return (OperatingSystem.IsWindows() ? Windows(ref data, out var buttonId) :
                OperatingSystem.IsMacOS() ? OSX(ref data, out buttonId) :
                OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() ? Linux(ref data, out buttonId) :
#else
#pragma warning disable MA0144
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Windows(ref data, out var buttonId) :
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSX(ref data, out buttonId) :
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Linux(ref data, out buttonId) :
#pragma warning restore MA0144
#endif
                Else(ref data, out buttonId)) is 0
                ? buttonId
                : -1;
        }
        finally
        {
            foreach (var pin in pins)
                pin.Dispose();
        }
    }
}
