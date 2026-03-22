using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class PsychedelicPlasma : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public PsychedelicPlasma(int width, int height)
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
                var nx = (x - _width / 2.0) / _width;
                var ny = (y - _height / 2.0) / _height;
                var v = Math.Sin((nx * 4.0) + t) + Math.Sin((ny * 4.0) - t * 1.3) + Math.Sin((nx + ny) * 3.5 + t * 0.7);
                var hue = (v + 3.0) / 6.0 * 360.0 + t * 40.0;
                var color = ColorUtils.FromHsv(hue, 0.9, 0.9);
                buffer.SetPixel(x, y, color);
            }
        }
    }
}
