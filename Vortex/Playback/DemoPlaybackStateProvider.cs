namespace Vortex.Playback;

public sealed class DemoPlaybackStateProvider : IPlaybackStateProvider
{
    private readonly DateTime _start = DateTime.UtcNow;

    public Task<PlaybackState> GetStateAsync(CancellationToken cancellationToken)
    {
        var elapsed = DateTime.UtcNow - _start;
        var phase = (int)(elapsed.TotalSeconds / 20) % 3;

        return Task.FromResult(phase switch
        {
            0 => new PlaybackState(true, "demo-track-1", "Demo Track 1", "Codex"),
            1 => new PlaybackState(true, "demo-track-2", "Demo Track 2", "Codex"),
            _ => PlaybackState.Stopped
        });
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
