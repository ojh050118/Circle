#nullable disable

using System.IO;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Graphics.UserInterface
{
    internal partial class CircleDirectorySelectorParentDirectory : CircleDirectorySelectorDirectory
    {
        public CircleDirectorySelectorParentDirectory(DirectoryInfo directory)
            : base(directory, "..")
        {
        }

        protected override IconUsage? Icon => FontAwesome.Solid.Folder;
    }
}
