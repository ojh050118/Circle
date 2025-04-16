using Circle.Game;
using Foundation;

namespace Circle.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : GameApplicationDelegate
    {
        protected override osu.Framework.Game CreateGame() => new CircleGame();
    }
}
