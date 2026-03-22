namespace Vortex.Rendering;

public sealed class FrameBuffer
{
    private readonly Rgb24[] _pixels;

    public int Width { get; }
    public int Height { get; }

    public FrameBuffer(int width, int height)
    {
        Width = width;
        Height = height;
        _pixels = new Rgb24[width * height];
    }

    public void Clear()
    {
        Array.Fill(_pixels, Rgb24.Black);
    }

    public void Fill(Rgb24 color)
    {
        Array.Fill(_pixels, color);
    }

    public void SetPixel(int x, int y, Rgb24 color)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
        {
            return;
        }

        _pixels[(y * Width) + x] = color;
    }

    public Rgb24 GetPixel(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
        {
            return Rgb24.Black;
        }

        return _pixels[(y * Width) + x];
    }

    public ReadOnlySpan<Rgb24> Pixels => _pixels;
}
