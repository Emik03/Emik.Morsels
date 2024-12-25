// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the component that draws the frame rate.</summary>
/// <remarks><para>
/// Adapted from <a href="https://blogs.msdn.microsoft.com/shawnhar/2007/06/08/displaying-the-framerate/">Shawn Hargreaves's implementation</a>.
/// </para></remarks>
sealed class FrameRateCounter(Letterboxed2DGame game, SpriteFont font) : DrawableGameComponent(game)
{
    const string Format = "FPS: ";

    static readonly TimeSpan s_frequency = TimeSpan.FromSeconds(1);

    readonly StringBuilder _builder = new(Format, Format.Length + 10);

    int _counter, _rate;

    TimeSpan _elapsed = TimeSpan.Zero;

    public override void Draw(GameTime gameTime)
    {
        if (!Visible)
            return;

        _counter++;
        _builder.Remove(Format.Length, _builder.Length - Format.Length).Append(_rate);

        const float Factor = 24;
        Vector2 position = new(game.Width / Factor, game.Height / Factor);

        var batch = game.Batch;
        batch.DrawString(font, _builder, position + Vector2.One, Color.Black);
        batch.DrawString(font, _builder, position, Color.White);
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
