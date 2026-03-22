using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleStars : IAnimation
{
    private readonly int _width;
    private readonly int _height;
    private readonly Random _random = new();
    private readonly List<Star> _stars = new();

    public IdleStars(int width, int height)
    {
        _width = width;
        _height = height;

        var count = Math.Max(8, (width * height) / 12);
        for (var i = 0; i < count; i++)
        {
            _stars.Add(NewStar());
        }
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        buffer.Clear();

        for (var i = 0; i < _stars.Count; i++)
        {
            var star = _stars[i];
            var t = elapsed.TotalSeconds + star.Offset;
            var pulse = (Math.Sin(t * 1.5) + 1) * 0.5;
            var color = ColorUtils.FromHsv(200 + star.HueShift, 0.4, 0.2 + 0.8 * pulse);
            buffer.SetPixel(star.X, star.Y, color);
        }
    }

    private Star NewStar()
    {
        return new Star(
            _random.Next(0, _width),
            _random.Next(0, _height),
            _random.NextDouble() * Math.PI * 2,
            _random.NextDouble() * 40);
    }

    private readonly record struct Star(int X, int Y, double Offset, double HueShift);
}
