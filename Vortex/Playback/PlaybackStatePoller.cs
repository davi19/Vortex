namespace Vortex.Playback;

public sealed class PlaybackStatePoller : IAsyncDisposable
{
    private readonly IPlaybackStateProvider _provider;
    private readonly TimeSpan _pollInterval;
    private PlaybackState _current = PlaybackState.Stopped;
    private bool _initialized;

    public PlaybackStatePoller(IPlaybackStateProvider provider, TimeSpan pollInterval)
    {
        _provider = provider;
        _pollInterval = pollInterval;
    }

    public event Action<PlaybackState>? StateChanged;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var state = await _provider.GetStateAsync(cancellationToken);
            if (!_initialized || !state.Equals(_current))
            {
                _current = state;
                _initialized = true;
                StateChanged?.Invoke(state);
            }

            try
            {
                await Task.Delay(_pollInterval, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
    }
}
