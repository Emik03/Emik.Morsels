// SPDX-License-Identifier: MPL-2.0
#if IMGUI && XNA
// ReSharper disable RedundantNameQualifier
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable CA1707, CA2213
using Buffer = System.Buffer;

/// <summary>ImGui renderer for use with MonoGame.</summary>
/// <param name="game">The game to use as reference.</param>
/// <param name="shared">Whether the <see cref="Context"/> is shared between instances of this type.</param>
[CLSCompliant(false)] // ReSharper disable CognitiveComplexity
public sealed class ImGuiRenderer(Game game, bool shared = false) : IDisposable
{
    delegate (bool Return, string NewInput) TextInput(
        ReadOnlySpan<char> label,
        ReadOnlySpan<char> hint,
        string input,
        uint maxLength,
        System.Numerics.Vector2 size,
        ImGuiInputTextFlags flags
    );

    const float WheelDelta = 120;

    static readonly ImmutableArray<Keys> s_keys = ImmutableCollectionsMarshal.AsImmutableArray(Enum.GetValues<Keys>());

    static readonly List<(ReadOnlyMemory<char> Label, string Input)> s_queued = [];

    static readonly VertexDeclaration s_declaration = new(
        Unsafe.SizeOf<ImDrawVert>(),
        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
        new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
    );
#if TEXTCOPY
    static nint s_clipboard;
#endif
    static nint s_sharedContext;

    readonly Dictionary<nint, Texture2D> _loadedTextures = [];

    readonly (Game Game, bool Shared, nint Context) _m = (game, shared, SetupInput(game, shared));

    readonly GraphicsDevice _graphicsDevice = game.GraphicsDevice;

    readonly RasterizerState _rasterizerState = new()
    {
        CullMode = CullMode.None,
        DepthBias = 0,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = false,
        ScissorTestEnable = true,
        SlopeScaleDepthBias = 0,
    };

    byte[]? _indexData, _vertexData;

    int _horizontalScrollWheelValue, _indexBufferSize, _scrollWheelValue, _textureId, _vertexBufferSize;

    nint? _fontTextureId;

    BasicEffect? _effect;

    IndexBuffer? _indexBuffer;

    VertexBuffer? _vertexBuffer;

    /// <summary>Gets the value determining whether this instance is disposed.</summary>
    public bool IsDisposed { get; private set; }

    /// <summary>Gets the context.</summary>
    public nint Context => _m.Context;

    /// <summary>Shows the keyboard display for mobile. Does nothing on desktop builds.</summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="defaultText">The default text.</param>
    /// <param name="usePasswordMode">Whether to hide the characters.</param>
    public static void AddKeyboardInputToIO(
        string title,
        string description,
        string defaultText = "",
        bool usePasswordMode = false
    )
    {
        async Task ShowAsync()
        {
            var input = await KeyboardInput.Show(title, description, defaultText, usePasswordMode)
               .ConfigureAwait(false);

            var io = ImGui.GetIO();

            foreach (var c in input.AsSpan())
                if (c is not '\t')
                    io.AddInputCharacter(c);
        }

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
#pragma warning disable MA0134
            _ = Task.Run(ShowAsync);
#pragma warning restore MA0134
    }

    /// <inheritdoc cref="BeginTabItem(ReadOnlySpan{char}, ref bool, ImGuiTabItemFlags)"/>
    // ReSharper disable once InconsistentNaming
    public static bool BeginTabItem(string label, ref bool p_open, ImGuiTabItemFlags flags) =>
        BeginTabItem(label.AsSpan(), ref p_open, flags);

    /// <inheritdoc cref="ImGui.BeginTabItem(string, ref bool, ImGuiTabItemFlags)"/>
    /// <remarks><para>
    /// The original binding doesn't work when <paramref name="p_open"/> is null reference.
    /// However, it being a null ref has significance to imgui, as it means
    /// the item is always shown and the selection is managed by imgui itself.
    /// So, we fix this ourselves and wait till it gets fixed upstream.
    /// </para></remarks>
    // ReSharper disable once InconsistentNaming
    public static unsafe bool BeginTabItem(ReadOnlySpan<char> label, ref bool p_open, ImGuiTabItemFlags flags)
    {
        var nativeLabel = Encoding.UTF8.GetByteCount(label) + 1 is var length && length <= Span.MaxStackalloc
            ? stackalloc byte[length]
            : new byte[length];

        nativeLabel[Encoding.UTF8.GetBytes(label, nativeLabel)] = 0;

        fixed (byte* nativeLabelPtr = nativeLabel)
        fixed (bool* open = &p_open)
            return ImGuiNative.igBeginTabItem(nativeLabelPtr, (byte*)open, flags) is not 0;
    }

    /// <inheritdoc cref="InputText(ReadOnlyMemory{char}, ref string, uint, ImGuiInputTextFlags)"/>
    public static bool InputText(
        string label,
        ref string input,
        uint maxLength,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        InputText(label.AsMemory(), ref input, maxLength, flags);

    /// <summary>
    /// Drop-in replacement for <see cref="ImGui.InputText(string, ref string, uint, ImGuiInputTextFlags)"/>
    /// that allows mobile devices to enter text fields explicitly.
    /// </summary>
    /// <param name="label">The label of the input text.</param>
    /// <param name="input">The current value of the input text.</param>
    /// <param name="maxLength">The maximum length for the parameter <paramref name="maxLength"/>.</param>
    /// <param name="flags">The flags of the input text.</param>
    public static bool InputText(
        ReadOnlyMemory<char> label,
        ref string input,
        uint maxLength,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        Text(
            label,
            "",
            ref input,
            maxLength,
            default,
            flags,
            static (label, _, input, maxLength, _, flags) =>
                (ImGui.InputText(label, ref input, maxLength, flags), input)
        );

    /// <inheritdoc cref="InputText(ReadOnlyMemory{char}, ref string, uint, ImGuiInputTextFlags)"/>
    public static bool InputTextMultiline(
        string label,
        ReadOnlySpan<char> hint,
        ref string input,
        uint maxLength,
        System.Numerics.Vector2 size,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        InputTextMultiline(label.AsMemory(), hint, ref input, maxLength, size, flags);

    /// <inheritdoc cref="InputText(ReadOnlyMemory{char}, ref string, uint, ImGuiInputTextFlags)"/>
    public static bool InputTextMultiline(
        ReadOnlyMemory<char> label,
        ReadOnlySpan<char> hint,
        ref string input,
        uint maxLength,
        System.Numerics.Vector2 size,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()
            ? Text(
                label,
                hint,
                ref input,
                maxLength,
                size,
                flags,
                static (label, _, input, maxLength, size, flags) =>
                    (ImGui.InputTextMultiline(label, ref input, maxLength, size, flags), input)
            )
            : ImGui.InputTextMultiline(label.Span, ref input, maxLength, size, flags);

    /// <inheritdoc cref="InputText(ReadOnlyMemory{char}, ref string, uint, ImGuiInputTextFlags)"/>
    public static bool InputTextWithHint(
        string label,
        ReadOnlySpan<char> hint,
        ref string input,
        uint maxLength,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        InputTextWithHint(label.AsMemory(), hint, ref input, maxLength, flags);

    /// <inheritdoc cref="InputText(ReadOnlyMemory{char}, ref string, uint, ImGuiInputTextFlags)"/>
    public static bool InputTextWithHint(
        ReadOnlyMemory<char> label,
        ReadOnlySpan<char> hint,
        ref string input,
        uint maxLength,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None
    ) =>
        OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()
            ? Text(
                label,
                hint,
                ref input,
                maxLength,
                default,
                flags,
                static (label, hint, input, maxLength, _, flags) =>
                    (ImGui.InputTextWithHint(label, hint, ref input, maxLength, flags), input)
            )
            : ImGui.InputTextWithHint(label.Span, hint, ref input, maxLength, flags);

    /// <summary>
    /// Creates a pointer to a texture, which can be passed through ImGui calls such a
    /// <see cref="ImGui.Image(System.IntPtr, System.Numerics.Vector2)" />.
    /// That pointer is then used by ImGui to let us know what texture to draw.
    /// </summary>
    public nint BindTexture(Texture2D texture)
    {
        var id = (nint)_textureId++;
        _loadedTextures.Add(id, texture);
        return id;
    }

    /// <summary>
    /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline,
    /// should be called after the UIs drawn using ImGui.* calls.
    /// </summary>
    public void AfterLayout()
    {
        ImGui.Render();
        RenderDrawData(ImGui.GetDrawData());
    }

    /// <summary>Sets up ImGui for a new frame, should be called at frame start.</summary>
    public void BeforeLayout(GameTime gameTime)
    {
        ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateInput();
        ImGui.NewFrame();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed)
            return;
#if TEXTCOPY
        FreeClipboard();
#endif
        IsDisposed = true;
        _effect?.Dispose();
        _indexBuffer?.Dispose();
        _vertexBuffer?.Dispose();

        if (!_m.Shared)
            ImGui.DestroyContext(_m.Context);

        (_effect, _indexBuffer, _vertexBuffer) = (null, null, null);
    }

    /// <summary>
    /// Creates a texture and loads the font data from ImGui. Should be called when the
    /// <see cref="GraphicsDevice" /> is initialized but before any rendering is done.
    /// </summary>
    public unsafe void RebuildFontAtlas()
    {
        var io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);
        Texture2D tex2d = new(_graphicsDevice, width, height, false, SurfaceFormat.Color);
        tex2d.SetData([..new ReadOnlySpan<byte>(pixelData, width * height * bytesPerPixel)]);

        if (_fontTextureId is { } id)
            UnbindTexture(id);

        var bind = BindTexture(tex2d);
        _fontTextureId = bind;
        io.Fonts.SetTexID(bind);
        io.Fonts.ClearTexData();
    }
#if TEXTCOPY
    /// <summary>Sets up the clipboard.</summary>
    static unsafe void SetupClipboard()
    {
        static nint Get(nint _)
        {
            FreeClipboard();
            return s_clipboard = Marshal.StringToHGlobalAnsi(ClipboardService.GetText());
        }

        static void Set(nint _, sbyte* text)
        {
            FreeClipboard();
            ClipboardService.SetText(new(text));
        }

        var platformIo = ImGui.GetPlatformIO();
        platformIo.Platform_GetClipboardTextFn = (nint)(delegate*<nint, nint>)&Get;
        platformIo.Platform_SetClipboardTextFn = (nint)(delegate*<nint, sbyte*, void>)&Set;
    }

    /// <summary>Frees the last recorded clipboard.</summary>
    static void FreeClipboard()
    {
        if (s_clipboard is 0)
            return;

        Marshal.FreeHGlobal(s_clipboard);
        s_clipboard = 0;
    }
#endif
    /// <summary>Sets up the IO.</summary>
    static void SetupIO()
    {
#if TEXTCOPY
        SetupClipboard();
#endif
        var io = ImGui.GetIO();
        io.ConfigErrorRecovery = true;
        io.ConfigErrorRecoveryEnableAssert = false;
        io.ConfigErrorRecoveryEnableTooltip = true;
        io.ConfigErrorRecoveryEnableDebugLog = false;
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated.
    /// </summary>
    public void UnbindTexture(nint textureId) => _loadedTextures.Remove(textureId);

    static bool TryMapKeys(Keys key, out ImGuiKey imGuiKey) =>
        (imGuiKey = key switch
        {
            Keys.Back => ImGuiKey.Backspace,
            Keys.Tab => ImGuiKey.Tab,
            Keys.Enter => ImGuiKey.Enter,
            Keys.CapsLock => ImGuiKey.CapsLock,
            Keys.Escape => ImGuiKey.Escape,
            Keys.Space => ImGuiKey.Space,
            Keys.PageUp => ImGuiKey.PageUp,
            Keys.PageDown => ImGuiKey.PageDown,
            Keys.End => ImGuiKey.End,
            Keys.Home => ImGuiKey.Home,
            Keys.Left => ImGuiKey.LeftArrow,
            Keys.Right => ImGuiKey.RightArrow,
            Keys.Up => ImGuiKey.UpArrow,
            Keys.Down => ImGuiKey.DownArrow,
            Keys.PrintScreen => ImGuiKey.PrintScreen,
            Keys.Insert => ImGuiKey.Insert,
            Keys.Delete => ImGuiKey.Delete,
            >= Keys.D0 and <= Keys.D9 => ImGuiKey._0 + (key - Keys.D0),
            >= Keys.A and <= Keys.Z => ImGuiKey.A + (key - Keys.A),
            >= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (key - Keys.NumPad0),
            Keys.Multiply => ImGuiKey.KeypadMultiply,
            Keys.Add => ImGuiKey.KeypadAdd,
            Keys.Subtract => ImGuiKey.KeypadSubtract,
            Keys.Decimal => ImGuiKey.KeypadDecimal,
            Keys.Divide => ImGuiKey.KeypadDivide,
            >= Keys.F1 and <= Keys.F24 => ImGuiKey.F1 + (key - Keys.F1),
            Keys.NumLock => ImGuiKey.NumLock,
            Keys.Scroll => ImGuiKey.ScrollLock,
            Keys.LeftShift => ImGuiKey.ModShift,
            Keys.LeftControl => ImGuiKey.ModCtrl,
            Keys.LeftAlt => ImGuiKey.ModAlt,
            Keys.OemSemicolon => ImGuiKey.Semicolon,
            Keys.OemPlus => ImGuiKey.Equal,
            Keys.OemComma => ImGuiKey.Comma,
            Keys.OemMinus => ImGuiKey.Minus,
            Keys.OemPeriod => ImGuiKey.Period,
            Keys.OemQuestion => ImGuiKey.Slash,
            Keys.OemTilde => ImGuiKey.GraveAccent,
            Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
            Keys.OemCloseBrackets => ImGuiKey.RightBracket,
            Keys.OemPipe => ImGuiKey.Backslash,
            Keys.OemQuotes => ImGuiKey.Apostrophe,
            Keys.BrowserBack => ImGuiKey.AppBack,
            Keys.BrowserForward => ImGuiKey.AppForward,
            _ => ImGuiKey.None,
        }) is not ImGuiKey.None ||
        key is Keys.None;

    static bool Text(
        ReadOnlyMemory<char> label,
        ReadOnlySpan<char> hint,
        ref string input,
        uint maxLength,
        System.Numerics.Vector2 size,
        ImGuiInputTextFlags flags,
        [RequireStaticDelegate(IsError = true)] TextInput textInput
    )
    {
        var copy = input;

        async Task ShowAsync()
        {
            var display = label.SplitOn('#').First;

            var result = await KeyboardInput.Show(
                    display.ToString(),
                    $"Enter a value for {display}.",
                    copy,
                    flags.Has(ImGuiInputTextFlags.Password)
                )
               .ConfigureAwait(false);

            s_queued.Add((label, result));
        }

        var labelSpan = label.Span;

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            for (var i = s_queued.Count - 1; i >= 0; i--)
                if (s_queued[i] is var (queuedLabel, queuedInput) &&
                    labelSpan.Equals(queuedLabel.Span, StringComparison.Ordinal))
                {
                    input = queuedInput.Length > maxLength ? queuedInput[..(int)maxLength] : queuedInput;
                    s_queued.RemoveAt(i);
                }

        (var ret, input) = textInput(labelSpan, hint, input, maxLength, size, flags);

        if ((OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()) && ret)
#pragma warning disable MA0134
            _ = Task.Run(ShowAsync);
#pragma warning restore MA0134
        return ret;
    }

    static nint SetupInput([UsedImplicitly] Game game, bool shared)
    {
        Debug.Assert(game is not null);

        if (shared && s_sharedContext is not 0)
            return s_sharedContext;

        var context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);
        SetupIO();

        if (shared && s_sharedContext is 0)
            s_sharedContext = context;
#if !ANDROID && !IOS
        var io = ImGui.GetIO();

        game.Window.TextInput += (_, a) =>
        {
            if (a.Character is not '\t' and var c)
                io.AddInputCharacter(c);
        };
#endif
        return context;
    }

    void RenderCommandLists(ImDrawDataPtr drawData)
    {
        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        for (int n = 0, idxOffset = 0, vtxOffset = 0;
            n < drawData.CmdListsCount && drawData.CmdLists[n] is var cmdList;
            n++, idxOffset += cmdList.IdxBuffer.Size, vtxOffset += cmdList.VtxBuffer.Size)
            for (var cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                if (cmdList.CmdBuffer[cmdI] is not { ElemCount: not 0 } drawCmd)
                    continue;

                if (!_loadedTextures.TryGetValue(drawCmd.TextureId, out var value))
                    throw new InvalidOperationException(
                        $"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings"
                    );

                _graphicsDevice.ScissorRectangle = new(
                    (int)drawCmd.ClipRect.X,
                    (int)drawCmd.ClipRect.Y,
                    (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                    (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                );

                foreach (var pass in UpdateEffect(value).CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _graphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        (int)drawCmd.VtxOffset + vtxOffset,
                        (int)drawCmd.IdxOffset + idxOffset,
                        (int)drawCmd.ElemCount / 3
                    );
                }
            }
    }

    /// <summary>Gets the geometry as set up by ImGui and sends it to the graphics device.</summary>
    void RenderDrawData(ImDrawDataPtr drawData)
    {
        var lastViewport = _graphicsDevice.Viewport;
        var lastScissorBox = _graphicsDevice.ScissorRectangle;
        var lastRasterizer = _graphicsDevice.RasterizerState;
        var lastDepthStencil = _graphicsDevice.DepthStencilState;
        var lastBlendFactor = _graphicsDevice.BlendFactor;
        var lastBlendState = _graphicsDevice.BlendState;
        _graphicsDevice.BlendFactor = Color.White;
        _graphicsDevice.BlendState = BlendState.NonPremultiplied;
        _graphicsDevice.RasterizerState = _rasterizerState;
        _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

        _graphicsDevice.Viewport = new(
            0,
            0,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        UpdateBuffers(drawData);
        RenderCommandLists(drawData);
        _graphicsDevice.Viewport = lastViewport;
        _graphicsDevice.ScissorRectangle = lastScissorBox;
        _graphicsDevice.RasterizerState = lastRasterizer;
        _graphicsDevice.DepthStencilState = lastDepthStencil;
        _graphicsDevice.BlendState = lastBlendState;
        _graphicsDevice.BlendFactor = lastBlendFactor;
    }

    unsafe void UpdateBuffers(ImDrawDataPtr drawData)
    {
        if (drawData.TotalVtxCount is 0)
            return;

        if (drawData.TotalVtxCount > _vertexBufferSize)
        {
            _vertexBuffer?.Dispose();
            _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5);
            _vertexBuffer = new(_graphicsDevice, s_declaration, _vertexBufferSize, BufferUsage.None);
            _vertexData = new byte[_vertexBufferSize * Unsafe.SizeOf<ImDrawVert>()];
        }

        if (drawData.TotalIdxCount > _indexBufferSize)
        {
            _indexBuffer?.Dispose();
            _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5);
            _indexBuffer = new(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
            _indexData = new byte[_indexBufferSize * sizeof(ushort)];
        }

        Debug.Assert(_indexData is not null);
        Debug.Assert(_vertexData is not null);
        Debug.Assert(_indexBuffer is not null);
        Debug.Assert(_vertexBuffer is not null);

        for (int n = 0, idxOffset = 0, vtxOffset = 0;
            n < drawData.CmdListsCount && drawData.CmdLists[n] is var cmdList;
            n++, idxOffset += cmdList.IdxBuffer.Size, vtxOffset += cmdList.VtxBuffer.Size)
            fixed (void* vtxDstPtr = &_vertexData[vtxOffset * Unsafe.SizeOf<ImDrawVert>()])
            fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
            {
                Buffer.MemoryCopy(
                    (void*)cmdList.VtxBuffer.Data,
                    vtxDstPtr,
                    _vertexData.Length,
                    cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()
                );

                Buffer.MemoryCopy(
                    (void*)cmdList.IdxBuffer.Data,
                    idxDstPtr,
                    _indexData.Length,
                    cmdList.IdxBuffer.Size * sizeof(ushort)
                );
            }

        _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
    }

    void UpdateInput()
    {
        if (!_m.Game.IsActive)
            return;

        var io = ImGui.GetIO();
        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();
        var touches = TouchPanel.GetState();

        if (touches.Count is not 0)
            foreach (var touch in touches)
            {
                io.AddMousePosEvent(touch.Position.X, touch.Position.Y);
                io.AddMouseButtonEvent(0, touch.State is TouchLocationState.Pressed);
            }
        else
            io.AddMousePosEvent(mouse.X, mouse.Y);

        io.AddMouseButtonEvent(0, mouse.LeftButton is ButtonState.Pressed);
        io.AddMouseButtonEvent(1, mouse.RightButton is ButtonState.Pressed);
        io.AddMouseButtonEvent(2, mouse.MiddleButton is ButtonState.Pressed);
        io.AddMouseButtonEvent(3, mouse.XButton1 is ButtonState.Pressed);
        io.AddMouseButtonEvent(4, mouse.XButton2 is ButtonState.Pressed);

        io.AddMouseWheelEvent(
            (mouse.HorizontalScrollWheelValue - _horizontalScrollWheelValue) / WheelDelta,
            (mouse.ScrollWheelValue - _scrollWheelValue) / WheelDelta
        );

        _scrollWheelValue = mouse.ScrollWheelValue;
        _horizontalScrollWheelValue = mouse.HorizontalScrollWheelValue;

        foreach (var key in s_keys)
            if (TryMapKeys(key, out var imGuiKey))
                io.AddKeyEvent(imGuiKey, keyboard.IsKeyDown(key));

        io.DisplaySize = new(
            _graphicsDevice.PresentationParameters.BackBufferWidth.Max(0),
            _graphicsDevice.PresentationParameters.BackBufferHeight.Max(0)
        );

        io.DisplayFramebufferScale = new(1, 1);
    }

    [MemberNotNull(nameof(_effect))]
    BasicEffect UpdateEffect(Texture2D texture)
    {
        _effect ??= new(_graphicsDevice);
        var io = ImGui.GetIO();
        _effect.World = Microsoft.Xna.Framework.Matrix.Identity;
        _effect.View = Microsoft.Xna.Framework.Matrix.Identity;

        _effect.Projection =
            Microsoft.Xna.Framework.Matrix.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1, 1);

        _effect.TextureEnabled = true;
        _effect.Texture = texture;
        _effect.VertexColorEnabled = true;
        return _effect;
    }
}
#endif
