using Vortex.Playback;
using Vortex.Rendering;

namespace Vortex.Animations;

public sealed class AnimationController
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<Func<IAnimation>> _musicAnimations;
    private readonly List<Func<IAnimation>> _idleAnimations;
    private readonly TimeSpan _idleRotation = TimeSpan.FromSeconds(25);
    private readonly TimeSpan _musicRotation = TimeSpan.FromSeconds(25);

    private PlaybackState _state = PlaybackState.Stopped;
    private IAnimation _current;
    private TimeSpan _elapsed;
    private TimeSpan _idleElapsed;
    private TimeSpan _musicElapsed;
    private int _idleIndex;
    private int _musicIndex;

    public AnimationController(int width, int height)
    {
        _width = width;
        _height = height;
        _musicAnimations = new List<Func<IAnimation>>
        {
            () => new PsychedelicPlasma(width, height),
            () => new VortexSpiral(width, height),
            () => new FlowBars(width, height),
            () => new ColorWaves(width, height)
        };

        _idleAnimations = new List<Func<IAnimation>>
        {
            () => new IdleClock(width, height),
            () => new IdleEmojiBounce(width, height),
            () => new IdleCatBlink(width, height),
            () => new IdleHeartBeat(width, height),
            () => new IdleStars(width, height),
            () => new IdleBreathe(width, height)
        };

        _idleIndex = 0;
        _musicIndex = 0;
        _current = CreateIdleAnimation();
    }

    public void SetState(PlaybackState state)
    {
        if (state.IsPlaying)
        {
            if (!_state.IsPlaying || !string.Equals(state.TrackId, _state.TrackId, StringComparison.Ordinal))
            {
                _current = CreateMusicAnimation();
                _elapsed = TimeSpan.Zero;
                _musicElapsed = TimeSpan.Zero;
            }
        }
        else if (_state.IsPlaying)
        {
            _idleIndex = 0;
            _current = CreateIdleAnimation();
            _elapsed = TimeSpan.Zero;
            _idleElapsed = TimeSpan.Zero;
            _musicElapsed = TimeSpan.Zero;
        }

        _state = state;
    }

    public void Update(TimeSpan delta, FrameBuffer buffer)
    {
        _elapsed += delta;
        if (!_state.IsPlaying)
        {
            _idleElapsed += delta;
            if (_idleElapsed > _idleRotation)
            {
                _current = CreateIdleAnimation();
                _elapsed = TimeSpan.Zero;
                _idleElapsed = TimeSpan.Zero;
            }
        }
        else
        {
            _musicElapsed += delta;
            if (_musicElapsed > _musicRotation)
            {
                _current = CreateMusicAnimation();
                _elapsed = TimeSpan.Zero;
                _musicElapsed = TimeSpan.Zero;
            }
        }

        _current.Update(_elapsed, buffer);
    }

    private IAnimation CreateMusicAnimation()
    {
        var index = _musicIndex % _musicAnimations.Count;
        _musicIndex++;
        return _musicAnimations[index]();
    }

    private IAnimation CreateIdleAnimation()
    {
        var index = _idleIndex % _idleAnimations.Count;
        _idleIndex++;
        return _idleAnimations[index]();
    }
}
