#nullable disable

using Circle.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Graphics.Containers
{
    public partial class ScalingContainer : DrawSizePreservingFillContainer
    {
        private Bindable<float> scale;

        public ScalingContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager localConfig)
        {
            scale = localConfig.GetBindable<float>(CircleSetting.Scale);
            scale.BindValueChanged(scaleChanged, true);
        }

        private void scaleChanged(ValueChangedEvent<float> e)
        {
            this.ScaleTo(e.NewValue, 1000, Easing.OutPow10);
            this.ResizeTo(new Vector2(1 / e.NewValue), 1000, Easing.OutPow10);
        }
    }
}
