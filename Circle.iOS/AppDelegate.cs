using Circle.Game;
using Foundation;
using osu.Framework.iOS;

namespace Circle.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : GameAppDelegate
    {
        protected override osu.Framework.Game CreateGame() => new CircleGame();
    }
}
