using System.IO;
using Circle.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class CircleFileSelector : FileSelector
    {
        public CircleFileSelector(string initialPath = null, string[] validFileExtensions = null)
            : base(initialPath, validFileExtensions)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Padding = new MarginPadding(10);
        }

        protected override ScrollContainer<Drawable> CreateScrollContainer() => new CircleScrollContainer();

        protected override DirectorySelectorBreadcrumbDisplay CreateBreadcrumb() => new CircleDirectorySelectorBreadcrumbDisplay();

        protected override DirectorySelectorDirectory CreateParentDirectoryItem(DirectoryInfo directory) => new CircleDirectorySelectorParentDirectory(directory);

        protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null) => new CircleDirectorySelectorDirectory(directory, displayName);

        protected override DirectoryListingFile CreateFileItem(FileInfo file) => new CircleDirectoryListingFile(file);

        protected override void NotifySelectionError() => this.FlashColour(Colour4.Red, 300);

        protected class CircleDirectoryListingFile : DirectoryListingFile
        {
            private Box hover;

            public CircleDirectoryListingFile(FileInfo file)
                : base(file)
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
                    new CircleDirectorySelectorDirectory.Background
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

            protected override IconUsage? Icon
            {
                get
                {
                    switch (File.Extension)
                    {
                        case @".ogg":
                        case @".mp3":
                        case @".wav":
                            return FontAwesome.Regular.FileAudio;

                        case @".jpg":
                        case @".jpeg":
                        case @".png":
                            return FontAwesome.Regular.FileImage;

                        case @".mp4":
                        case @".avi":
                        case @".mov":
                        case @".flv":
                            return FontAwesome.Regular.FileVideo;

                        case @".circle":
                        case @".circlez":
                            return FontAwesome.Solid.File;

                        default:
                            return FontAwesome.Regular.File;
                    }
                }
            }

            protected override SpriteText CreateSpriteText() => new SpriteText();
        }
    }
}
