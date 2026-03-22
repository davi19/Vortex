using Vortex.Rendering;

namespace Vortex;

public enum PlaybackSource
{
    Idle,
    Mpris,
    Demo
}

public enum FrameSinkType
{
    Null,
    Console,
    Spi
}

public enum DbusBus
{
    Session,
    System
}

public sealed record AppOptions(
    int Width,
    int Height,
    int Fps,
    PlaybackSource Source,
    FrameSinkType Sink,
    string MprisService,
    DbusBus Bus,
    string SpiDevice,
    int SpiHz,
    byte Brightness,
    ColorOrder ColorOrder,
    bool Serpentine,
    bool OriginBottomLeft,
    bool FlipX)
{
    public static AppOptions Parse(string[] args)
    {
        var width = 16;
        var height = 16;
        var fps = 30;
        var source = PlaybackSource.Mpris;
        var sink = FrameSinkType.Null;
        var mprisService = "org.mpris.MediaPlayer2.librespot";
        var bus = DbusBus.Session;
        var spiDevice = "/dev/spidev0.0";
        var spiHz = 2_400_000;
        var brightness = (byte)255;
        var colorOrder = ColorOrder.GRB;
        var serpentine = true;
        var originBottomLeft = true;
        var flipX = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "--width":
                    width = int.Parse(args[++i]);
                    break;
                case "--height":
                    height = int.Parse(args[++i]);
                    break;
                case "--fps":
                    fps = int.Parse(args[++i]);
                    break;
                case "--source":
                    source = ParseEnum(args[++i], PlaybackSource.Mpris);
                    break;
                case "--sink":
                    sink = ParseEnum(args[++i], FrameSinkType.Null);
                    break;
                case "--mpris":
                    mprisService = args[++i];
                    break;
                case "--bus":
                    bus = ParseEnum(args[++i], DbusBus.Session);
                    break;
                case "--spi-device":
                    spiDevice = args[++i];
                    break;
                case "--spi-hz":
                    spiHz = int.Parse(args[++i]);
                    break;
                case "--brightness":
                    brightness = byte.Parse(args[++i]);
                    break;
                case "--color-order":
                    colorOrder = ParseEnum(args[++i], ColorOrder.GRB);
                    break;
                case "--serpentine":
                    serpentine = ParseBool(args[++i], true);
                    break;
                case "--origin-bottom-left":
                    originBottomLeft = ParseBool(args[++i], true);
                    break;
                case "--flip-x":
                    flipX = ParseBool(args[++i], false);
                    break;
            }
        }

        return new AppOptions(
            width,
            height,
            fps,
            source,
            sink,
            mprisService,
            bus,
            spiDevice,
            spiHz,
            brightness,
            colorOrder,
            serpentine,
            originBottomLeft,
            flipX);
    }

    private static T ParseEnum<T>(string value, T fallback) where T : struct
    {
        return Enum.TryParse<T>(value, true, out var parsed) ? parsed : fallback;
    }

    private static bool ParseBool(string value, bool fallback)
    {
        return bool.TryParse(value, out var parsed) ? parsed : fallback;
    }
}
