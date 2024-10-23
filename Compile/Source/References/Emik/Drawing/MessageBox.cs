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

    public static int? Error(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, buttons, 0, 16);

    public static int? Error(
        this string? title,
        string? message,
        nint ptr,
        ReadOnlySpan<string> buttons = default
    ) =>
        Show(title, message, buttons, ptr, 16);

    public static int? Info(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, buttons, 0, 64);

    public static int? Info(this string? title, string? message, nint ptr, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, buttons, ptr, 64);

    public static int? Warn(this string? title, string? message, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, buttons, 0, 32);

    public static int? Warn(this string? title, string? message, nint ptr, ReadOnlySpan<string> buttons = default) =>
        Show(title, message, buttons, ptr, 32);
#pragma warning disable MA0144
    static unsafe int? Show(
        string? title,
        string? message,
        ReadOnlySpan<string> buttons,
        nint ptr,
        uint flags
    )
    {
        [DllImport("sdl2", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Else(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("SDL2.dll", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Windows(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("libSDL2.dylib", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int OSX(ref MessageBoxData messageBoxData, out int buttonId);

        [DllImport("libSDL2-2.0.so.0", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Linux(ref MessageBoxData messageBoxData, out int buttonId);

        const int Flags = 3;
        var nonZeroLength = buttons.Length.Max(1);
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
            MessageBoxData messageBoxData = new(flags, ptr, title, message, nonZeroLength, buttonDatas);
#if NET5_0_OR_GREATER
            return (OperatingSystem.IsWindows() ? Windows(ref messageBoxData, out var buttonId) :
                OperatingSystem.IsMacOS() ? OSX(ref messageBoxData, out buttonId) :
                OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() ? Linux(ref messageBoxData, out buttonId) :
                Else(ref messageBoxData, out buttonId)) is 0
                ? buttonId
                : null;
#else
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Windows(ref d, out var buttonId) :
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSX(ref d, out buttonId) :
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Linux(ref d, out buttonId) :
                Else(ref d, out buttonId)) is 0
                ? buttonId
                : null;
#endif
        }
        finally
        {
            foreach (var pin in pins)
                pin.Dispose();
        }
    }
}
