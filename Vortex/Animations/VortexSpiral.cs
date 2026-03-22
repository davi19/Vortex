using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class VortexSpiral : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public VortexSpiral(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        var t = elapsed.TotalSeconds;
        var cx = (_width - 1) / 2.0;
        var cy = (_height - 1) / 2.0;
        var maxR = Math.Sqrt(cx * cx + cy * cy);

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var dx = x - cx;
                var dy = y - cy;
                var r = Math.Sqrt(dx * dx + dy * dy);
                var angle = Math.Atan2(dy, dx);

                var swirl = angle * 2.4 + r * 0.9 - t * 3.0;
                var wave = 0.5 + 0.5 * Math.Sin(swirl);
                var falloff = 0.25 + 0.75 * (1.0 - Math.Clamp(r / maxR, 0.0, 1.0));
                var hue = (angle * 180.0 / Math.PI + r * 28.0 + t * 40.0) % 360.0;
                var value = Math.Clamp(wave * falloff, 0.0, 1.0);

                buffer.SetPixel(x, y, ColorUtils.FromHsv(hue, 0.9, value));
            }
        }
    }
}
