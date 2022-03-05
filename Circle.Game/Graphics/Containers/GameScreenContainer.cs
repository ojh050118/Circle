using osu.Framework.Graphics.Containers;

namespace Circle.Game.Graphics.Containers
{
    public class GameScreenContainer : BufferedContainer
    {
        public GameScreenContainer(bool cachedFrameBuffer = false)
            : base(cachedFrameBuffer: cachedFrameBuffer)
        {
        }
    }
}
