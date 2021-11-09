using osu.Framework;
using osu.Framework.Platform;

namespace Circle.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"Circle"))
            using (osu.Framework.Game game = new CircleGameDesktop())
                host.Run(game);
        }
    }
}
