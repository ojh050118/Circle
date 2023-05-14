#nullable disable

using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace Circle.Game.Graphics.UserInterface
{
    internal partial class CircleDirectorySelectorDirectory : DirectorySelectorDirectory
    {
        public CircleDirectorySelectorDirectory(DirectoryInfo directory, string displayName = null)
            : base(directory, displayName)
        {
        }

        protected override IconUsage? Icon => Directory.Name.Contains(Path.DirectorySeparatorChar)
            ? FontAwesome.Solid.Database
            : FontAwesome.Regular.Folder;

        [BackgroundDependencyLoader]
        private void load()
        {
            Flow.AutoSizeAxes = Axes.X;
            Flow.Height = CircleDirectorySelector.ITEM_HEIGHT;
            Masking = true;
            CornerRadius = 5;

            AddRangeInternal(new Drawable[]
            {
                new Background
                {
                    Depth = 1
                }
            });
        }

        protected override SpriteText CreateSpriteText() => new SpriteText();

        internal class Background : CompositeDrawable
        {
            private Box box;

            private CircleColour colours;

            [BackgroundDependencyLoader]
            private void load(CircleColour colours)
            {
                this.colours = colours;
                RelativeSizeAxes = Axes.Both;
                InternalChild = box = new Box
                {
                    Colour = colours.TransparentBlack,
                    RelativeSizeAxes = Axes.Both,
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                box.FadeColour(colours.TransparentGray, 250, Easing.OutQuint);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                box.FadeColour(colours.TransparentBlack, 250, Easing.OutQuint);
            }
        }
    }
}
