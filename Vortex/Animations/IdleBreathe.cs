using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleBreathe : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public IdleBreathe(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        var t = elapsed.TotalSeconds;
        var pulse = (Math.Sin(t * 0.6) + 1) * 0.5;
        var hue = 180 + Math.Sin(t * 0.2) * 60;
        var baseColor = ColorUtils.FromHsv(hue, 0.6, 0.2 + 0.6 * pulse);

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var dx = (x - _width / 2.0) / _width;
                var dy = (y - _height / 2.0) / _height;
                var falloff = Math.Clamp(1.0 - Math.Sqrt(dx * dx + dy * dy) * 1.6, 0, 1);
                var color = ColorUtils.Blend(Rgb24.Black, baseColor, falloff);
                buffer.SetPixel(x, y, color);
            }
        }
    }
}
