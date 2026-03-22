namespace Vortex.Rendering;

public readonly struct Rgb24
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;

    public Rgb24(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static readonly Rgb24 Black = new(0, 0, 0);
}
