using System.Device.Spi;

namespace Vortex.Rendering;

public sealed class SpiFrameSink : IFrameSink, IDisposable
{
    private readonly SpiDevice _spi;
    private readonly int _width;
    private readonly int _height;
    private readonly bool _serpentine;
    private readonly bool _originBottomLeft;
    private readonly byte _brightness;
    private readonly ColorOrder _colorOrder;
    private readonly byte[] _frameBytes;
    private readonly byte[] _resetBytes;

    public SpiFrameSink(
        int width,
        int height,
        string devicePath,
        int clockHz,
        bool serpentine,
        bool originBottomLeft,
        byte brightness,
        ColorOrder colorOrder)
    {
        _width = width;
        _height = height;
        _serpentine = serpentine;
        _originBottomLeft = originBottomLeft;
        _brightness = brightness;
        _colorOrder = colorOrder;

        var settings = ParseDevice(devicePath);
        settings.ClockFrequency = clockHz;
        settings.Mode = SpiMode.Mode0;
        settings.DataBitLength = 8;
        _spi = SpiDevice.Create(settings);

        var ledCount = width * height;
        _frameBytes = new byte[ledCount * 9];
        _resetBytes = new byte[Math.Max(1, clockHz / 40000)];
    }

    public void Render(FrameBuffer buffer)
    {
        var pixelIndex = 0;
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var (px, py) = MapPixel(x, y);
                var color = buffer.GetPixel(px, py);
                WriteColor(color, _frameBytes, pixelIndex * 9);
                pixelIndex++;
            }
        }

        _spi.Write(_frameBytes);
        _spi.Write(_resetBytes);
    }

    public void Dispose()
    {
        _spi.Dispose();
    }

    private (int x, int y) MapPixel(int x, int y)
    {
        var mappedY = _originBottomLeft ? _height - 1 - y : y;
        if (_serpentine && (mappedY % 2 == 1))
        {
            return (_width - 1 - x, mappedY);
        }

        return (x, mappedY);
    }

    private void WriteColor(Rgb24 color, byte[] target, int offset)
    {
        var (r, g, b) = ApplyBrightness(color);
        Span<byte> encoded = stackalloc byte[9];

        switch (_colorOrder)
        {
            case ColorOrder.RGB:
                EncodeByte(r, encoded, 0);
                EncodeByte(g, encoded, 3);
                EncodeByte(b, encoded, 6);
                break;
            case ColorOrder.BRG:
                EncodeByte(b, encoded, 0);
                EncodeByte(r, encoded, 3);
                EncodeByte(g, encoded, 6);
                break;
            case ColorOrder.GRB:
            default:
                EncodeByte(g, encoded, 0);
                EncodeByte(r, encoded, 3);
                EncodeByte(b, encoded, 6);
                break;
        }

        encoded.CopyTo(target.AsSpan(offset, 9));
    }

    private (byte r, byte g, byte b) ApplyBrightness(Rgb24 color)
    {
        if (_brightness >= 255)
        {
            return (color.R, color.G, color.B);
        }

        var scale = _brightness / 255.0;
        return (
            (byte)Math.Clamp(color.R * scale, 0, 255),
            (byte)Math.Clamp(color.G * scale, 0, 255),
            (byte)Math.Clamp(color.B * scale, 0, 255));
    }

    private static void EncodeByte(byte value, Span<byte> target, int offset)
    {
        for (var bit = 7; bit >= 0; bit--)
        {
            var one = (value & (1 << bit)) != 0;
            var pattern = one ? 0b110 : 0b100;
            var bitIndex = (7 - bit) * 3;
            SetPattern(target, offset, bitIndex, pattern);
        }
    }

    private static void SetPattern(Span<byte> target, int offset, int bitIndex, int pattern)
    {
        var byteIndex = offset + (bitIndex / 8);
        var bitOffset = 7 - (bitIndex % 8);

        for (var i = 2; i >= 0; i--)
        {
            var bit = (pattern >> i) & 1;
            target[byteIndex] = (byte)(target[byteIndex] & ~(1 << bitOffset));
            target[byteIndex] = (byte)(target[byteIndex] | (bit << bitOffset));
            bitOffset--;
            if (bitOffset < 0)
            {
                bitOffset = 7;
                byteIndex++;
            }
        }
    }

    private static SpiConnectionSettings ParseDevice(string devicePath)
    {
        var name = Path.GetFileName(devicePath);
        if (name.StartsWith("spidev", StringComparison.OrdinalIgnoreCase))
        {
            var parts = name.Substring(6).Split('.');
            if (parts.Length == 2 && int.TryParse(parts[0], out var bus) && int.TryParse(parts[1], out var chip))
            {
                return new SpiConnectionSettings(bus, chip);
            }
        }

        return new SpiConnectionSettings(0, 0);
    }
}

public enum ColorOrder
{
    GRB,
    RGB,
    BRG
}
