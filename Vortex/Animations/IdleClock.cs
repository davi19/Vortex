using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class IdleClock : IAnimation
{
    private readonly int _width;
    private readonly int _height;

    public IdleClock(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(TimeSpan elapsed, FrameBuffer buffer)
    {
        buffer.Clear();

        var now = DateTime.Now;
        var text = now.ToString("HHmm");

        var baseX = 0;
        var baseY = (_height - 5) / 2;

        var hue = (now.Second * 6) % 360;
        var color = ColorUtils.FromHsv(hue, 0.8, 0.8);

        DrawDigit(buffer, baseX, baseY, text[0], color);
        DrawDigit(buffer, baseX + 4, baseY, text[1], color);
        DrawColon(buffer, baseX + 7, baseY, color);
        DrawDigit(buffer, baseX + 9, baseY, text[2], color);
        DrawDigit(buffer, baseX + 13, baseY, text[3], color);
    }

    private static void DrawDigit(FrameBuffer buffer, int x, int y, char digit, Rgb24 color)
    {
        if (!DigitFont.TryGetValue(digit, out var rows))
        {
            return;
        }

        for (var row = 0; row < rows.Length; row++)
        {
            var bits = rows[row];
            for (var col = 0; col < 3; col++)
            {
                if ((bits & (1 << (2 - col))) != 0)
                {
                    buffer.SetPixel(x + col, y + row, color);
                }
            }
        }
    }

    private static void DrawColon(FrameBuffer buffer, int x, int y, Rgb24 color)
    {
        buffer.SetPixel(x, y + 1, color);
        buffer.SetPixel(x, y + 3, color);
    }

    private static readonly Dictionary<char, byte[]> DigitFont = new()
    {
        ['0'] = new byte[] { 0b111, 0b101, 0b101, 0b101, 0b111 },
        ['1'] = new byte[] { 0b010, 0b110, 0b010, 0b010, 0b111 },
        ['2'] = new byte[] { 0b111, 0b001, 0b111, 0b100, 0b111 },
        ['3'] = new byte[] { 0b111, 0b001, 0b111, 0b001, 0b111 },
        ['4'] = new byte[] { 0b101, 0b101, 0b111, 0b001, 0b001 },
        ['5'] = new byte[] { 0b111, 0b100, 0b111, 0b001, 0b111 },
        ['6'] = new byte[] { 0b111, 0b100, 0b111, 0b101, 0b111 },
        ['7'] = new byte[] { 0b111, 0b001, 0b010, 0b010, 0b010 },
        ['8'] = new byte[] { 0b111, 0b101, 0b111, 0b101, 0b111 },
        ['9'] = new byte[] { 0b111, 0b101, 0b111, 0b001, 0b111 }
    };
}
