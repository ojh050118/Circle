using System;
using Circle.Desktop.Windows;
using osu.Framework;
using osu.Framework.Platform;
using Velopack;

namespace Circle.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            setupVelopack();

            using (GameHost host = Host.GetSuitableDesktopHost(@"Circle"))
            using (osu.Framework.Game game = new CircleGameDesktop())
                host.Run(game);
        }

        private static void setupVelopack()
        {
            VelopackApp
                .Build()
                .WithFirstRun(v =>
                {
                    if (OperatingSystem.IsWindows()) WindowsAssociationManager.InstallAssociations();
                }).Run();
        }
    }
}
