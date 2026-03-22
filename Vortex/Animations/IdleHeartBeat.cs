using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleHeartBeat : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public IdleHeartBeat(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        buffer.Clear();

        var t = elapsed.TotalSeconds;
        var beat = 0.5 + 0.5 * Math.Sin(t * 3.4);
        var big = beat > 0.75;
        var heart = big ? HeartBig : HeartSmall;
        var hue = 350.0;
        var value = big ? 1.0 : 0.7 + 0.2 * beat;
        var color = ColorUtils.FromHsv(hue, 0.9, value);

        var x = (_width - 8) / 2;
        var y = (_height - 8) / 2;

        DrawBitmap(buffer, x, y, heart, color);
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

    private static readonly byte[] HeartSmall =
    {
        0b00000000,
        0b01100110,
        0b11111111,
        0b11111111,
        0b01111110,
        0b00111100,
        0b00011000,
        0b00000000
    };

    private static readonly byte[] HeartBig =
    {
        0b01100110,
        0b11111111,
        0b11111111,
        0b11111111,
        0b01111110,
        0b00111100,
        0b00011000,
        0b00000000
    };
}
