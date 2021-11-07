using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using Circle.Resources;

namespace Circle.Game
{
    public class CircleGameBase : osu.Framework.Game
    {

        protected override Container<Drawable> Content { get; }

        protected CircleGameBase()
        {
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));
        }
    }
}
