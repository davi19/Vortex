namespace Vortex.Playback;

public sealed record PlaybackState(
    bool IsPlaying,
    string TrackId,
    string Title,
    string Artist)
{
    public static PlaybackState Stopped { get; } = new(false, string.Empty, string.Empty, string.Empty);
}
