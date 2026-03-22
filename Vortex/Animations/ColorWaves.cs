using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class ColorWaves : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public ColorWaves(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        var t = elapsed.TotalSeconds;
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var wave = Math.Sin((x * 0.5) + t * 2.3) + Math.Cos((y * 0.7) - t * 1.7);
                var hue = (wave + 2) / 4 * 360 + t * 25;
                var value = 0.4 + 0.6 * (Math.Sin(t + x * 0.2) * 0.5 + 0.5);
                buffer.SetPixel(x, y, ColorUtils.FromHsv(hue, 0.85, value));
            }
        }
    }
}
