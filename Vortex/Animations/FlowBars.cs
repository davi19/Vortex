using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class FlowBars : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public FlowBars(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        var t = elapsed.TotalSeconds * 1.2;
        buffer.Clear();

        for (var x = 0; x < _width; x++)
        {
            var phase = t + x * 0.35;
            var height = (Math.Sin(phase) + 1) * 0.5;
            var barHeight = (int)Math.Round(height * (_height - 1));
            var hue = (t * 40 + x * 12) % 360;

            for (var y = 0; y <= barHeight; y++)
            {
                var brightness = (double)y / (_height - 1);
                var color = ColorUtils.FromHsv(hue, 0.9, 0.2 + 0.8 * brightness);
                buffer.SetPixel(x, _height - 1 - y, color);
            }
        }
    }
}
