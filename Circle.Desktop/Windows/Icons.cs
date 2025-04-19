using System.IO;

namespace Circle.Desktop.Windows
{
    public static class Icons
    {
        /// <summary>
        /// Fully qualified path to the directory that contains icons (in the installation folder).
        /// </summary>
        private static readonly string icon_directory = Path.GetDirectoryName(typeof(Icons).Assembly.Location)!;

        public static string Beatmap => Path.Join(icon_directory, "game.ico");
    }
}
