using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleCatBlink : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public IdleCatBlink(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        buffer.Clear();

        var t = elapsed.TotalSeconds;
        var blink = (t % 3.6) < 0.2;
        var sprite = blink ? FaceClosed : FaceOpen;

        var x = (_width - SpriteWidth) / 2;
        var y = (_height - sprite.Length) / 2;

        DrawSprite(buffer, x, y, sprite);
    }

    private static void DrawSprite(FrameBuffer buffer, int x, int y, string[] rows)
    {
        for (var row = 0; row < rows.Length; row++)
        {
            var line = rows[row];
            for (var col = 0; col < line.Length; col++)
            {
                var symbol = line[col];
                if (symbol == '.')
                {
                    continue;
                }

                var color = symbol switch
                {
                    'f' => new Rgb24(255, 170, 90),
                    'e' => new Rgb24(30, 30, 30),
                    '-' => new Rgb24(30, 30, 30),
                    'n' => new Rgb24(255, 90, 120),
                    'm' => new Rgb24(140, 60, 60),
                    'w' => new Rgb24(235, 235, 235),
                    _ => Rgb24.Black
                };

                buffer.SetPixel(x + col, y + row, color);
            }
        }
    }

    private const int SpriteWidth = 12;

    private static readonly string[] FaceOpen =
    {
        "....ffff....",
        "...ffffff...",
        "..ffwffwff..",
        "..ffeffeff..",
        "..ffffnfff..",
        "..fffmmfff..",
        "...ffffff...",
        "....ffff...."
    };

    private static readonly string[] FaceClosed =
    {
        "....ffff....",
        "...ffffff...",
        "..ffwffwff..",
        "..ff-ff-ff..",
        "..ffffnfff..",
        "..fffmmfff..",
        "...ffffff...",
        "....ffff...."
    };
}
