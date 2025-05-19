// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels; // ReSharper disable once RedundantUsingDirective
using static PlatformID;

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

        /// <summary>Displays a message box.</summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
        /// <param name="buttons">The buttons to display.</param>
        /// <param name="flags">The flags for the message box.</param>
        /// <returns>
        /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
        /// </returns>
        public static int Show(string? title, string? message, nint window, ReadOnlySpan<string> buttons, uint flags)
        {
            const int Flags = 3;

            static int One(string? title, string? message, nint window, uint flags)
            {
                var bytes = stackalloc byte[] { (byte)'O', (byte)'K', (byte)'\0' };
                Button buttonData = new(Flags, 0, bytes);
                return Show(new(flags, window, title, message, 1, &buttonData));
            }

            static int More(string? title, string? message, nint window, ReadOnlySpan<string> buttons, uint flags)
            {
                var buttonData = stackalloc Button[buttons.Length];
                var handles = stackalloc GCHandle[buttons.Length];

                for (var i = 0; i < buttons.Length; i++)
                    fixed (char* chars = buttons[i])
                    {
                        int charLength = buttons[i].Length, byteLength = charLength * 4 + 1;
                        var bytes = ArrayPool<byte>.Shared.Rent(byteLength);
                        handles[i] = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                        var ptr = (byte*)handles[i].AddrOfPinnedObject();
                        bytes[Encoding.UTF8.GetBytes(chars, charLength, ptr, byteLength)] = 0;
                        buttonData[i] = new(Flags, i, ptr);
                    }

                var ret = Show(new(flags, window, title, message, buttons.Length, buttonData));

                for (var i = 0; i < buttons.Length; i++) // ReSharper disable once NullableWarningSuppressionIsUsed
                {
                    ArrayPool<byte>.Shared.Return(Unsafe.As<byte[]>(handles[i].Target)!);
                    handles[i].Free();
                }

                return ret;
            }

            return buttons.IsEmpty ? One(title, message, window, flags) : More(title, message, window, buttons, flags);
        }

        static int Show(MessageBoxData data)
        {
            const string Entry = "SDL_ShowMessageBox";

            [DllImport("SDL2.dll", EntryPoint = Entry, CharSet = CharSet.Ansi, ExactSpelling = true)]
            static extern int Windows(ref MessageBoxData messageBoxData, out int buttonId);

            [DllImport("libSDL2.dylib", EntryPoint = Entry, CharSet = CharSet.Ansi, ExactSpelling = true)]
            static extern int OSX(ref MessageBoxData messageBoxData, out int buttonId);

            [DllImport("libSDL2-2.0.so.0", EntryPoint = Entry, CharSet = CharSet.Ansi, ExactSpelling = true)]
            static extern int Linux(ref MessageBoxData messageBoxData, out int buttonId);

            [DllImport("sdl2", EntryPoint = Entry, CharSet = CharSet.Ansi, ExactSpelling = true)]
            static extern int Else(ref MessageBoxData messageBoxData, out int buttonId);
#if NET5_0_OR_GREATER
            return (OperatingSystem.IsWindows() ? Windows(ref data, out var buttonId) :
                OperatingSystem.IsMacOS() ? OSX(ref data, out buttonId) :
                OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() ? Linux(ref data, out buttonId) :
#else
            var platform = Environment.OSVersion.Platform;

            // ReSharper disable once ConvertConditionalTernaryExpressionToSwitchExpression
            return (platform is WinCE or Win32NT or Win32S or Win32Windows ? Windows(ref data, out var buttonId) :
                platform is MacOSX ? OSX(ref data, out buttonId) :
                platform is Unix ? Linux(ref data, out buttonId) :
#endif
                Else(ref data, out buttonId)) is 0
                ? buttonId
                : -1;
        }
    }

    /// <summary>Displays a message box with an error icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowError(
        this string? title,
        string? message,
#if !CSHARPREPL // Remove this once CSharpRepl updates.
        params
#endif
            ReadOnlySpan<string> buttons
    ) =>
        MessageBoxData.Show(title, message, 0, buttons, 16);

    /// <summary>Displays a message box with an error icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="window">The pointer to the SDL window. Can be <c>0</c>.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int
        ShowError(
            this string? title,
            string? message,
            nint window,
#if !CSHARPREPL // Remove this once CSharpRepl updates.
            params
#endif
                ReadOnlySpan<string> buttons
        ) =>
        MessageBoxData.Show(title, message, window, buttons, 16);

    /// <summary>Displays a message box with an informational icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowInfo(
        this string? title,
        string? message,
#if !CSHARPREPL // Remove this once CSharpRepl updates.
        params
#endif
            ReadOnlySpan<string> buttons
    ) =>
        MessageBoxData.Show(title, message, 0, buttons, 64);

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
#if !CSHARPREPL // Remove this once CSharpRepl updates.
        params
#endif
            ReadOnlySpan<string> buttons
    ) =>
        MessageBoxData.Show(title, message, window, buttons, 64);

    /// <summary>Displays a message box with a warning icon.</summary>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>
    /// The index within <paramref name="buttons"/> that was pressed, or <c>-1</c> if an error occurred.
    /// </returns>
    public static int ShowWarn(
        this string? title,
        string? message,
#if !CSHARPREPL // Remove this once CSharpRepl updates.
        params
#endif
            ReadOnlySpan<string> buttons
    ) =>
        MessageBoxData.Show(title, message, 0, buttons, 32);

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
#if !CSHARPREPL // Remove this once CSharpRepl updates.
        params
#endif
            ReadOnlySpan<string> buttons
    ) =>
        MessageBoxData.Show(title, message, window, buttons, 32);
}
