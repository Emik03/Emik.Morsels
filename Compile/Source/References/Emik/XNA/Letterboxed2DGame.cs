// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>The basic wrapper around <see cref="Game"/> that handles letterboxing for a 2D game.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
[CLSCompliant(false)]
public abstract partial class Letterboxed2DGame : Game
{
    /// <summary>Gets the target to draw to.</summary>
    RenderTarget2D? _target;

    /// <summary>Initializes a new instances of the <see cref="Letterboxed2DGame"/> class.</summary>
    /// <param name="width">The width of the world.</param>
    /// <param name="height">The height of the world.</param>
    /// <param name="scale">The scale relative to the native resolution to open the window to.</param>
    protected Letterboxed2DGame(int width, int height, float scale = 5 / 6f)
    {
        var ratio = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / (float)(Width = width))
           .Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / (float)(Height = height));

        (GraphicsDeviceManager = new(this)
        {
            SynchronizeWithVerticalRetrace = false,
            PreferredBackBufferWidth = (int)(Width * ratio * scale),
            PreferredBackBufferHeight = (int)(Height * ratio * scale),
        }).ApplyChanges();

        IsMouseVisible = true;
        IsFixedTimeStep = false;
        Window.AllowUserResizing = true;
        Window.KeyDown += CheckForBorderlessOrFullScreenBind;
        GraphicsDevice.BlendState = BlendState.NonPremultiplied;
    }

    /// <summary>Determines whether the game is being played in a desktop environment.</summary>
    [Pure, SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("macos"),
     SupportedOSPlatformGuard("windows")]
    public static bool IsDesktop =>
        OperatingSystem.IsWindows() ||
        OperatingSystem.IsMacOS() ||
        OperatingSystem.IsLinux() ||
        OperatingSystem.IsFreeBSD();

    /// <summary>Gets the height of the native (world) resolution.</summary>
    [ValueRange(1, int.MaxValue), Pure]
    public int Height { get; }

    /// <summary>Gets the width of the native (world) resolution.</summary>
    [ValueRange(1, int.MaxValue), Pure]
    public int Width { get; }

    /// <summary>Gets the background, shown in letterboxing.</summary>
    [Pure]
    public Color Background { get; set; }

    /// <summary>Gets the default blend state.</summary>
    [Pure] // ReSharper disable once VirtualMemberNeverOverridden.Global
    public virtual BlendState BatchBlendState => BlendState.NonPremultiplied;

    /// <summary>Gets the device manager that contains this instance.</summary>
    [Pure]
    public GraphicsDeviceManager GraphicsDeviceManager { get; }

    /// <summary>Gets the batch to draw with.</summary>
    [Pure]
    public SpriteBatch Batch { get; private set; } = null!;

    /// <summary>Gets the texture containing a single white pixel.</summary>
    [Pure]
    public Texture2D WhitePixel { get; private set; } = null!;

    /// <summary>Converts the window coordinate to the world coordinate.</summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The world coordinate of the parameters given.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Vector2 World(float x, float y)
    {
        var bounds = Window.ClientBounds;
        float width = bounds.Width, height = bounds.Height;
        var world = Width / (float)Height;
        var window = width / height;
        var ratio = window < world ? width / Width : height / Height;

        return window < world
            ? new(x / ratio, (y - (height - ratio * Height) / 2) / ratio)
            : new((x - (width - ratio * Width) / 2) / ratio, y / ratio);
    }

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Vector2 World(in MouseState v) => World(v.Position);

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Vector2 World(Point v) => World(v.X, v.Y);

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Vector2 World(in TouchLocation v) => World(v.Position);

    /// <summary>Converts the window coordinate to the world coordinate.</summary>
    /// <param name="v">The vector to convert.</param>
    /// <returns>The world coordinate of the vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Vector2 World(Vector2 v) => World(v.X, v.Y);

    /// <inheritdoc />
    protected override bool BeginDraw()
    {
        Debug.Assert(Batch is not null);
        GraphicsDevice.SetRenderTarget(_target);
        GraphicsDevice.Clear(Background);
        Batch.Begin(blendState: BatchBlendState);
        return base.BeginDraw();
    }

    /// <inheritdoc />
    protected override void EndDraw()
    {
        Debug.Assert(Batch is not null);
        Batch.End();
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Background);
        Batch.Begin();
        var resolution = GraphicsDevice.Resolution(Width, Height);
        Batch.Draw(_target, resolution, Color.White);
        Batch.End();
        base.EndDraw();
    }

    /// <inheritdoc />
    [CLSCompliant(false), MemberNotNull(nameof(Batch), nameof(_target), nameof(WhitePixel))]
    protected override void Initialize()
    {
        base.Initialize();
        _target = new(GraphicsDevice, Width, Height);
        Services.AddService(Batch = new(GraphicsDevice));
        WhitePixel = new(GraphicsDevice, 1, 1);
        WhitePixel.SetData([Color.White]);
    }

    /// <summary>Invoked when a keyboard button is pressed.</summary>
    /// <param name="_">The sender, ignored.</param>
    /// <param name="e">The event arguments containing the key that was pressed.</param>
    void CheckForBorderlessOrFullScreenBind([UsedImplicitly] object? _, InputKeyEventArgs e)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.Key)
        {
            case Keys.F9 when IsDesktop:
                Window.IsBorderless = !Window.IsBorderless;
                break;
            case Keys.F11:
                GraphicsDeviceManager.IsFullScreen = !GraphicsDeviceManager.IsFullScreen;
                GraphicsDeviceManager.ApplyChanges();
                break;
        }
    }
}
#endif
