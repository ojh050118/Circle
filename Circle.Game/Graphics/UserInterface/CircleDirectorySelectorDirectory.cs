using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    internal class CircleDirectorySelectorDirectory : DirectorySelectorDirectory
    {
        private Box hover;

        public CircleDirectorySelectorDirectory(DirectoryInfo directory, string displayName = null)
            : base(directory, displayName)
        {
        }

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
                    Depth = 2
                },
                hover = new Box
                {
                    Colour = Color4.White,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    Depth = 1
                }
            });
        }

        protected override SpriteText CreateSpriteText() => new SpriteText();

        protected override IconUsage? Icon => Directory.Name.Contains(Path.DirectorySeparatorChar)
            ? FontAwesome.Solid.Database
            : FontAwesome.Regular.Folder;

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(0.25f, 250, Easing.OutQuint);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            hover.FadeTo(0, 500, Easing.OutQuint);
        }

        internal class Background : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.Both;
                InternalChild = new Box
                {
                    Colour = Color4.Black.Opacity(0.4f),
                    RelativeSizeAxes = Axes.Both,
                };
            }
        }
    }
}
