using osu.Framework.iOS;
using UIKit;

namespace Circle.iOS
{
    public static class Application
    {
        public static void Main(string[] args)
        {
            UIApplication.Main(args, typeof(GameUIApplication), typeof(AppDelegate));
        }
    }
}
