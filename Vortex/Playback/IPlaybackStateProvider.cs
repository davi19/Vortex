namespace Vortex.Playback;

public interface IPlaybackStateProvider : IAsyncDisposable
{
    Task<PlaybackState> GetStateAsync(CancellationToken cancellationToken);
}
