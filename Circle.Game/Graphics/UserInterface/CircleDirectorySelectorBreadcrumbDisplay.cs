using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Circle.Game.Graphics.UserInterface
{
    internal class CircleDirectorySelectorBreadcrumbDisplay : DirectorySelectorBreadcrumbDisplay
    {
        protected override Drawable CreateCaption() => new SpriteText
        {
            Text = "Current directory: ",
            Font = FontUsage.Default.With(size: CircleDirectorySelector.ITEM_HEIGHT)
        };

        protected override DirectorySelectorDirectory CreateRootDirectoryItem() => new CircleBreadcrumbDisplayComputer();

        protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null) => new CircleBreadcrumbDisplayDirectory(directory, displayName);

        private class CircleBreadcrumbDisplayComputer : CircleBreadcrumbDisplayDirectory
        {
            protected override IconUsage? Icon => null;

            public CircleBreadcrumbDisplayComputer()
                : base(null, "Computer")
            {
            }
        }

        private class CircleBreadcrumbDisplayDirectory : CircleDirectorySelectorDirectory
        {
            public CircleBreadcrumbDisplayDirectory(DirectoryInfo directory, string displayName = null)
                : base(directory, displayName)
            {
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Flow.Add(new SpriteIcon
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Icon = FontAwesome.Solid.ChevronRight,
                    Size = new Vector2(FONT_SIZE / 2)
                });
            }

            protected override IconUsage? Icon => Directory.Name.Contains(Path.DirectorySeparatorChar) ? base.Icon : null;
        }
    }
}
