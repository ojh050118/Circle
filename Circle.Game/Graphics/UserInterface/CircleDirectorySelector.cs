using System.IO;
using Circle.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Circle.Game.Graphics.UserInterface
{
    public class CircleDirectorySelector : DirectorySelector
    {
        public const float ITEM_HEIGHT = 20;

        public CircleDirectorySelector(string initialPath = null)
            : base(initialPath)
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

        protected override void NotifySelectionError() => this.FlashColour(Colour4.Red, 300);
    }
}
