// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the component that draws the frame rate.</summary>
/// <remarks><para>
/// Adapted from <a href="https://blogs.msdn.microsoft.com/shawnhar/2007/06/08/displaying-the-framerate/">Shawn Hargreaves's implementation</a>.
/// </para></remarks>
/// <param name="game">The game for this component.</param>
/// <param name="font">The font to draw with.</param>
/// <param name="batch">The batch to draw to.</param>
/// <param name="resolution">The function that gets the resolution of the application.</param>
sealed class FrameRateCounter(Game game, SpriteFont font, SpriteBatch batch, Func<Vector2> resolution)
    : DrawableGameComponent(game)
{
    const string Format = "FPS: ";

    /// <summary>Initializes a new instance of the <see cref="FrameRateCounter"/> class.</summary>
    /// <param name="game">The game to draw to.</param>
    /// <param name="font">The font to draw with.</param>
    /// <param name="batch">The batch to draw to.</param>
    public FrameRateCounter(Game game, SpriteFont font, SpriteBatch batch)
        : this(game, font, batch, () => new(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height)) { }

    /// <summary>Initializes a new instance of the <see cref="FrameRateCounter"/> class.</summary>
    /// <param name="game">The game to draw to.</param>
    /// <param name="font">The font to draw with.</param>
    public FrameRateCounter(Letterboxed2DGame game, SpriteFont font)
        : this(game, font, game.Batch, () => new(game.Width, game.Height)) { }

    static readonly TimeSpan s_frequency = TimeSpan.FromSeconds(1);

    readonly StringBuilder _builder = new(Format, Format.Length + 10);

    int _counter, _rate;

    TimeSpan _elapsed = TimeSpan.Zero;

    /// <summary>Gets the batch.</summary>
    public SpriteBatch Batch => batch;

    /// <summary>Gets the scale to draw the font at.</summary>
    public Vector2 Scale { get; set; } = Vector2.One;

    public override void Draw(GameTime gameTime)
    {
        if (!Visible)
            return;

        _counter++;
        _builder.Remove(Format.Length, _builder.Length - Format.Length).Append(_rate);

        const float Factor = 24;
        var position = resolution() / Factor;
        batch.DrawString(font, _builder, position + Vector2.One, Color.Black, 0, default, Scale, SpriteEffects.None, 0);
        batch.DrawString(font, _builder, position, Color.White, 0, default, Scale, SpriteEffects.None, 0);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Enabled)
            return;

        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed <= s_frequency)
            return;

        _elapsed -= s_frequency;
        (_counter, _rate) = (0, _counter);
    }
}
#endif
