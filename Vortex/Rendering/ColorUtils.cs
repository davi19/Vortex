namespace Vortex.Rendering;

public static class ColorUtils
{
    public static Rgb24 FromHsv(double hue, double saturation, double value)
    {
        hue = hue % 360.0;
        if (hue < 0) hue += 360.0;

        var c = value * saturation;
        var x = c * (1 - Math.Abs((hue / 60.0 % 2) - 1));
        var m = value - c;

        (double r, double g, double b) = hue switch
        {
            < 60 => (c, x, 0),
            < 120 => (x, c, 0),
            < 180 => (0, c, x),
            < 240 => (0, x, c),
            < 300 => (x, 0, c),
            _ => (c, 0, x)
        };

        return new Rgb24(
            (byte)Math.Clamp((r + m) * 255, 0, 255),
            (byte)Math.Clamp((g + m) * 255, 0, 255),
            (byte)Math.Clamp((b + m) * 255, 0, 255));
    }

    public static Rgb24 Blend(Rgb24 a, Rgb24 b, double t)
    {
        var it = 1 - t;
        return new Rgb24(
            (byte)Math.Clamp(a.R * it + b.R * t, 0, 255),
            (byte)Math.Clamp(a.G * it + b.G * t, 0, 255),
            (byte)Math.Clamp(a.B * it + b.B * t, 0, 255));
    }
}
