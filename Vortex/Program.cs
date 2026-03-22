using System.Diagnostics;
using Vortex;
using Vortex.Animations;
using Vortex.Playback;
using Vortex.Rendering;

var options = AppOptions.Parse(args);

Console.WriteLine($"[vortex] size={options.Width}x{options.Height} fps={options.Fps} source={options.Source} sink={options.Sink}");

IPlaybackStateProvider stateProvider = options.Source switch
{
    PlaybackSource.Mpris => new MprisPlaybackStateProvider(options.MprisService, options.Bus),
    PlaybackSource.Demo => new DemoPlaybackStateProvider(),
    _ => new DemoPlaybackStateProvider()
};

IFrameSink frameSink = options.Sink switch
{
    FrameSinkType.Console => new ConsoleFrameSink(options.Width, options.Height),
    FrameSinkType.Spi => new SpiFrameSink(
        options.Width,
        options.Height,
        options.SpiDevice,
        options.SpiHz,
        options.Serpentine,
        options.OriginBottomLeft,
        options.Brightness,
        options.ColorOrder),
    FrameSinkType.Null => new NullFrameSink(),
    _ => new NullFrameSink()
};

var buffer = new FrameBuffer(options.Width, options.Height);
var controller = new AnimationController(options.Width, options.Height);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var poller = new PlaybackStatePoller(stateProvider, TimeSpan.FromMilliseconds(600));
poller.StateChanged += controller.SetState;

var pollerTask = poller.RunAsync(cts.Token);

var frameDelay = TimeSpan.FromMilliseconds(1000.0 / options.Fps);
var sw = Stopwatch.StartNew();
var last = sw.Elapsed;

while (!cts.IsCancellationRequested)
{
    var now = sw.Elapsed;
    var delta = now - last;
    last = now;

    controller.Update(delta, buffer);
    frameSink.Render(buffer);

    var workTime = sw.Elapsed - now;
    var sleep = frameDelay - workTime;
    if (sleep > TimeSpan.Zero)
    {
        try
        {
            await Task.Delay(sleep, cts.Token);
        }
        catch (TaskCanceledException)
        {
            break;
        }
    }
}

await poller.DisposeAsync();
await pollerTask;

if (frameSink is IDisposable disposableSink)
{
    disposableSink.Dispose();
}
