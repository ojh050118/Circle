#nullable disable

using osu.Framework.Graphics.Containers;

namespace Circle.Game.Graphics.Containers
{
    public partial class GameScreenContainer : BufferedContainer
    {
        public GameScreenContainer(bool cachedFrameBuffer = false)
            : base(cachedFrameBuffer: cachedFrameBuffer)
        {
        }
    }
}
