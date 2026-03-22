namespace Vortex.Rendering;

public sealed class ConsoleFrameSink : IFrameSink
{
    private readonly int _width;
    private readonly int _height;
    private bool _initialized;

    public ConsoleFrameSink(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Render(FrameBuffer buffer)
    {
        if (!_initialized)
        {
            Console.CursorVisible = false;
            _initialized = true;
        }

        Console.SetCursorPosition(0, 0);
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var pixel = buffer.GetPixel(x, y);
                var intensity = (pixel.R + pixel.G + pixel.B) / 3;
                Console.Write(intensity switch
                {
                    > 220 => '█',
                    > 170 => '▓',
                    > 120 => '▒',
                    > 70 => '░',
                    _ => ' '
                });
            }
            Console.WriteLine();
        }
    }
}
