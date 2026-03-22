using System.Diagnostics;

namespace Vortex.Playback;

public sealed class LibrespotLogPlaybackStateProvider : IPlaybackStateProvider, IAsyncDisposable
{
    private readonly string _serviceName;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _readerTask;
    private readonly object _lock = new();
    private PlaybackState _state = PlaybackState.Stopped;
    private Process? _process;

    public LibrespotLogPlaybackStateProvider(string serviceName)
    {
        _serviceName = serviceName;
        _readerTask = Task.Run(ReadLoopAsync);
    }

    public Task<PlaybackState> GetStateAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            return Task.FromResult(_state);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_process is not null && !_process.HasExited)
        {
            try
            {
                _process.Kill();
            }
            catch
            {
                // Best effort.
            }
        }

        try
        {
            await _readerTask;
        }
        catch
        {
            // Ignore shutdown errors.
        }
    }

    private async Task ReadLoopAsync()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "journalctl",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        psi.ArgumentList.Add("-u");
        psi.ArgumentList.Add(_serviceName);
        psi.ArgumentList.Add("-f");
        psi.ArgumentList.Add("-n");
        psi.ArgumentList.Add("0");
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add("cat");
        psi.ArgumentList.Add("--no-pager");

        try
        {
            _process = Process.Start(psi);
        }
        catch
        {
            return;
        }

        if (_process is null)
        {
            return;
        }

        while (!_cts.IsCancellationRequested && !_process.HasExited)
        {
            string? line;
            try
            {
                line = await _process.StandardOutput.ReadLineAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var updated = TryParseLine(line, out var state);
            if (updated)
            {
                lock (_lock)
                {
                    _state = state;
                }
            }
        }
    }

    private static bool TryParseLine(string line, out PlaybackState state)
    {
        if (TryParseLoading(line, out var title, out var trackId))
        {
            state = new PlaybackState(true, trackId, title, string.Empty);
            return true;
        }

        var lowered = line.ToLowerInvariant();
        if (lowered.Contains("device became inactive"))
        {
            state = PlaybackState.Stopped;
            return true;
        }

        if (lowered.Contains("paused") || lowered.Contains("pause") ||
            lowered.Contains("stopped") || lowered.Contains("stop"))
        {
            state = PlaybackState.Stopped;
            return true;
        }

        state = PlaybackState.Stopped;
        return false;
    }

    private static bool TryParseLoading(string line, out string title, out string trackId)
    {
        title = string.Empty;
        trackId = string.Empty;

        const string prefix = "Loading <";
        const string mid = "> with Spotify URI <";
        const string suffix = ">";

        var prefixIndex = line.IndexOf(prefix, StringComparison.Ordinal);
        if (prefixIndex < 0)
        {
            return false;
        }

        var midIndex = line.IndexOf(mid, prefixIndex + prefix.Length, StringComparison.Ordinal);
        if (midIndex < 0)
        {
            return false;
        }

        var titleStart = prefixIndex + prefix.Length;
        title = line.Substring(titleStart, midIndex - titleStart);

        var uriStart = midIndex + mid.Length;
        var endIndex = line.IndexOf(suffix, uriStart, StringComparison.Ordinal);
        trackId = endIndex > uriStart ? line.Substring(uriStart, endIndex - uriStart) : line.Substring(uriStart);

        return true;
    }
}
