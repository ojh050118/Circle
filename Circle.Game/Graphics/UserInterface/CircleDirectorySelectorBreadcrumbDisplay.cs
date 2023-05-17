#nullable disable

using System.IO;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Circle.Game.Graphics.UserInterface
{
    internal partial class CircleDirectorySelectorBreadcrumbDisplay : DirectorySelectorBreadcrumbDisplay
    {
        protected override Drawable CreateCaption() => new CircleSpriteText
        {
            Text = "Current directory: ",
            Font = CircleFont.Default.With(size: CircleDirectorySelector.ITEM_HEIGHT)
        };

        protected override DirectorySelectorDirectory CreateRootDirectoryItem() => new CircleBreadcrumbDisplayComputer();

        protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null) => new CircleBreadcrumbDisplayDirectory(directory, displayName);

        private partial class CircleBreadcrumbDisplayComputer : CircleBreadcrumbDisplayDirectory
        {
            public CircleBreadcrumbDisplayComputer()
                : base(null, "Computer")
            {
            }

            protected override IconUsage? Icon => null;
        }

        private partial class CircleBreadcrumbDisplayDirectory : CircleDirectorySelectorDirectory
        {
            public CircleBreadcrumbDisplayDirectory(DirectoryInfo directory, string displayName = null)
                : base(directory, displayName)
            {
            }

            protected override IconUsage? Icon => Directory.Name.Contains(Path.DirectorySeparatorChar) ? base.Icon : null;

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
        }
    }
}
