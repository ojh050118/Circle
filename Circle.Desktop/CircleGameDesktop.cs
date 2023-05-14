using Circle.Game;
using osu.Framework.Platform;

namespace Circle.Desktop
{
    public partial class CircleGameDesktop : CircleGame
    {
        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Window.Title = Name;
        }
    }
}
