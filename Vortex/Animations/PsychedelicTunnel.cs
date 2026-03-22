using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class PsychedelicTunnel : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public PsychedelicTunnel(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        var t = elapsed.TotalSeconds * 0.9;
        var cx = _width / 2.0;
        var cy = _height / 2.0;

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var dx = x - cx;
                var dy = y - cy;
                var dist = Math.Sqrt(dx * dx + dy * dy) / (_width * 0.5);
                var angle = Math.Atan2(dy, dx);
                var hue = (angle / Math.PI * 180.0) + t * 120.0;
                var pulse = (Math.Sin(dist * 10.0 - t * 3.0) + 1) * 0.5;
                var value = Math.Clamp(1.0 - dist, 0, 1) * (0.3 + 0.7 * pulse);
                buffer.SetPixel(x, y, ColorUtils.FromHsv(hue, 0.95, value));
            }
        }
    }
}
