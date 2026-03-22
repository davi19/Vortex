using Vortex.Rendering;

namespace Vortex.Animations;

public interface IAnimation
{
    void Update(TimeSpan elapsed, FrameBuffer buffer);
}
