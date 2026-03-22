namespace Vortex.Playback;

public sealed class IdlePlaybackStateProvider : IPlaybackStateProvider
{
    public Task<PlaybackState> GetStateAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(PlaybackState.Stopped);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
