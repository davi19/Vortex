using Tmds.DBus;
using Vortex;

namespace Vortex.Playback;

[DBusInterface("org.mpris.MediaPlayer2.Player")]
public interface IMprisPlayer : IDBusObject
{
    Task<string> GetPlaybackStatusAsync();
    Task<IDictionary<string, object>> GetMetadataAsync();
}

public sealed class MprisPlaybackStateProvider : IPlaybackStateProvider
{
    private readonly string _service;
    private readonly DbusBus _bus;
    private readonly Connection _connection;
    private readonly IMprisPlayer _player;

    public MprisPlaybackStateProvider(string service, DbusBus bus)
    {
        _service = service;
        _bus = bus;
        _connection = bus == DbusBus.System ? Connection.System : Connection.Session;
        _player = _connection.CreateProxy<IMprisPlayer>(_service, "/org/mpris/MediaPlayer2");
    }

    public async Task<PlaybackState> GetStateAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _connection.ConnectAsync();

            var status = await _player.GetPlaybackStatusAsync();
            if (!string.Equals(status, "Playing", StringComparison.OrdinalIgnoreCase))
            {
                return PlaybackState.Stopped;
            }

            var metadata = await _player.GetMetadataAsync();
            var trackId = GetString(metadata, "mpris:trackid");
            var title = GetString(metadata, "xesam:title");
            var artist = GetArtist(metadata);

            return new PlaybackState(true, trackId, title, artist);
        }
        catch
        {
            return PlaybackState.Stopped;
        }
    }

    public ValueTask DisposeAsync()
    {
        _connection.Dispose();
        return ValueTask.CompletedTask;
    }

    private static string GetString(IDictionary<string, object> metadata, string key)
    {
        return metadata.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
    }

    private static string GetArtist(IDictionary<string, object> metadata)
    {
        if (!metadata.TryGetValue("xesam:artist", out var value) || value is null)
        {
            return string.Empty;
        }

        if (value is string artist)
        {
            return artist;
        }

        if (value is string[] artists)
        {
            return string.Join(", ", artists);
        }

        if (value is IEnumerable<string> list)
        {
            return string.Join(", ", list);
        }

        return value.ToString() ?? string.Empty;
    }
}
