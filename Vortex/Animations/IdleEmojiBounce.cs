using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleEmojiBounce : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public IdleEmojiBounce(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        buffer.Clear();

        var t = elapsed.TotalSeconds;
        var xRange = Math.Max(0, _width - 8);
        var x = (int)Math.Round((Math.Sin(t * 0.9) + 1) * 0.5 * xRange);
        var yBase = (_height - 8) / 2;
        var y = yBase + (int)Math.Round(Math.Sin(t * 1.7));

        var face = ColorUtils.FromHsv(45 + 8 * Math.Sin(t * 0.6), 0.95, 1.0);
        var features = new Rgb24(30, 20, 20);

        DrawBitmap(buffer, x, y, FaceMask, face);
        DrawBitmap(buffer, x, y, FaceFeatures, features);
    }

    private static void DrawBitmap(FrameBuffer buffer, int x, int y, byte[] rows, Rgb24 color)
    {
        for (var row = 0; row < rows.Length; row++)
        {
            var bits = rows[row];
            for (var col = 0; col < 8; col++)
            {
                if ((bits & (1 << (7 - col))) != 0)
                {
                    buffer.SetPixel(x + col, y + row, color);
                }
            }
        }
    }

    private static readonly byte[] FaceMask =
    {
        0b00111100,
        0b01111110,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b01111110,
        0b00111100
    };

    private static readonly byte[] FaceFeatures =
    {
        0b00000000,
        0b00000000,
        0b00100100,
        0b00000000,
        0b00000000,
        0b01000010,
        0b00111100,
        0b00000000
    };
}
