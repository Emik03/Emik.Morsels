// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

static partial class MessageBox
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly partial struct ButtonColorScheme(
        uint background = 0,
        uint title = 0,
        uint buttonBorder = 0,
        uint buttonBackground = 0,
        uint buttonSelected = 0
    )
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly unsafe partial struct ButtonData(uint flags, int buttonId, byte* text)
        {
            readonly uint _flags = flags;

            readonly int _buttonId = buttonId;

            readonly byte* _text = text;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly unsafe partial struct Data(
            uint flags,
            nint window,
            string? title,
            string? message,
            int numButtons,
            ButtonData* buttons,
            ButtonColorScheme* colorScheme
        )
        {
            readonly uint _flags = flags;

            readonly nint _window = window;

            readonly string? _title = title, _message = message;

            readonly int _numButtons = numButtons;

            readonly ButtonData* _buttons = buttons;

            readonly ButtonColorScheme* _colorScheme = colorScheme;
        }

        readonly byte
            _backgroundR = (byte)(background >> 16),
            _backgroundG = (byte)(background >> 8),
            _backgroundB = (byte)background,
            _titleR = (byte)(title >> 16),
            _titleG = (byte)(title >> 8),
            _titleB = (byte)title,
            _buttonBorderR = (byte)(buttonBorder >> 16),
            _buttonBorderG = (byte)(buttonBorder >> 8),
            _buttonBorderB = (byte)buttonBorder,
            _buttonBackgroundR = (byte)(buttonBackground >> 16),
            _buttonBackgroundG = (byte)(buttonBackground >> 8),
            _buttonBackgroundB = (byte)buttonBackground,
            _buttonSelectedR = (byte)(buttonSelected >> 16),
            _buttonSelectedG = (byte)(buttonSelected >> 8),
            _buttonSelectedB = (byte)buttonSelected;
    }

    public static int? Error(
        this string? title,
        string? message,
        ReadOnlySpan<string> buttons,
        ButtonColorScheme? colorScheme = null,
        nint window = 0
    ) =>
        Show(title, message, buttons, colorScheme, window, 16);

    public static int? Info(
        this string? title,
        string? message,
        ReadOnlySpan<string> buttons,
        ButtonColorScheme? colorScheme = null,
        nint window = 0
    ) =>
        Show(title, message, buttons, colorScheme, window, 64);

    public static int? Warn(
        this string? title,
        string? message,
        ReadOnlySpan<string> buttons,
        ButtonColorScheme? colorScheme = null,
        nint window = 0
    ) =>
        Show(title, message, buttons, colorScheme, window, 32);

    static unsafe int? Show(
        string? title,
        string? message,
        ReadOnlySpan<string> buttons,
        ButtonColorScheme? colorScheme,
        nint window,
        uint flags
    )
    {
        [DllImport("sdl2", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Else(ref ButtonColorScheme.Data messageBoxData, out int buttonId);

        [DllImport("SDL2.dll", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Windows(ref ButtonColorScheme.Data messageBoxData, out int buttonId);

        [DllImport("libSDL2.dylib", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int OSX(ref ButtonColorScheme.Data messageBoxData, out int buttonId);

        [DllImport("libSDL2-2.0.so.0", EntryPoint = "SDL_ShowMessageBox", CharSet = CharSet.Ansi, ExactSpelling = true)]
        static extern int Linux(ref ButtonColorScheme.Data messageBoxData, out int buttonId);

        using var _ = buttons.Length.Alloc(out Span<Rented<byte>.Pinned> pins);
        using var __ = buttons.Length.Alloc(out ButtonColorScheme.ButtonData* buttonDatas);

        for (var i = 0; i < buttons.Length; i++)
            fixed (char* chars = buttons[i])
            {
                var length = buttons[i].Length * 4 + 1;
                pins[i] = new(length, out var bytes);
                bytes[Encoding.UTF8.GetBytes(chars, buttons[i].Length, bytes, length) + 1] = 0;
                buttonDatas[i] = new(3, i, bytes);
            }

        try
        {
            var color = colorScheme ?? default;

            ButtonColorScheme.Data data = new(
                flags,
                window,
                message,
                title,
                buttons.Length,
                buttonDatas,
                colorScheme.HasValue ? &color : null
            );
#if NET5_0_OR_GREATER
            return (OperatingSystem.IsWindows() ? Windows(ref data, out var id) :
                OperatingSystem.IsMacOS() ? OSX(ref data, out id) :
                OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() ? Linux(ref data, out id) :
                Else(ref data, out id)) is 0
                ? id
                : null;
#else
#pragma warning disable MA0144
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
