using System.IO;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Graphics.UserInterface
{
    internal class CircleDirectorySelectorParentDirectory : CircleDirectorySelectorDirectory
    {
        protected override IconUsage? Icon => FontAwesome.Solid.Folder;

        public CircleDirectorySelectorParentDirectory(DirectoryInfo directory)
            : base(directory, "..")
        {
        }
    }
}
