using Vortex.Playback;
using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class AnimationController
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<Func<IAnimation>> _musicAnimations;
    private readonly List<Func<IAnimation>> _idleAnimations;
    private readonly Random _random = new();

    private PlaybackState _state = PlaybackState.Stopped;
    private IAnimation _current;
    private TimeSpan _elapsed;
    private TimeSpan _idleElapsed;

    public AnimationController(int width, int height)
    {
        _width = width;
        _height = height;
        _musicAnimations = new List<Func<IAnimation>>
        {
            () => new PsychedelicPlasma(width, height),
            () => new PsychedelicTunnel(width, height),
            () => new FlowBars(width, height),
            () => new ColorWaves(width, height)
        };

        _idleAnimations = new List<Func<IAnimation>>
        {
            () => new IdleClock(width, height),
            () => new IdleStars(width, height),
            () => new IdleBreathe(width, height)
        };

        _current = CreateIdleAnimation();
    }

    public void SetState(PlaybackState state)
    {
        if (state.IsPlaying)
        {
            if (!_state.IsPlaying || !string.Equals(state.TrackId, _state.TrackId, StringComparison.Ordinal))
            {
                _current = CreateMusicAnimation(state.TrackId);
                _elapsed = TimeSpan.Zero;
            }
        }
        else if (_state.IsPlaying)
        {
            _current = CreateIdleAnimation();
            _elapsed = TimeSpan.Zero;
            _idleElapsed = TimeSpan.Zero;
        }

        _state = state;
    }

    public void Update(TimeSpan delta, FrameBuffer buffer)
    {
        _elapsed += delta;
        if (!_state.IsPlaying)
        {
            _idleElapsed += delta;
            if (_idleElapsed > TimeSpan.FromSeconds(25))
            {
                _current = CreateIdleAnimation();
                _elapsed = TimeSpan.Zero;
                _idleElapsed = TimeSpan.Zero;
            }
        }

        _current.Update(_elapsed, buffer);
    }

    private IAnimation CreateMusicAnimation(string trackId)
    {
        var seed = trackId.GetHashCode();
        var rng = new Random(seed);
        var index = rng.Next(_musicAnimations.Count);
        return _musicAnimations[index]();
    }

    private IAnimation CreateIdleAnimation()
    {
        var index = _random.Next(_idleAnimations.Count);
        return _idleAnimations[index]();
    }
}
