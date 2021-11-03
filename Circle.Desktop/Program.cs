using osu.Framework.Platform;
using osu.Framework;
using Circle.Game;

namespace Circle.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"Circle"))
            using (osu.Framework.Game game = new CircleGame())
                host.Run(game);
        }
    }
}
