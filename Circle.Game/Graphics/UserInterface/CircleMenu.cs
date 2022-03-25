using Circle.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class CircleMenu : Menu
    {
        private Sample sampleOpen;
        private Sample sampleClose;

        // todo: this shouldn't be required after https://github.com/ppy/osu-framework/issues/4519 is fixed.
        private bool wasOpened;

        public CircleMenu(Direction direction, bool topLevelMenu = false)
            : base(direction, topLevelMenu)
        {
            BackgroundColour = Color4.Black.Opacity(0.4f);
            MaskingContainer.CornerRadius = 5;
            ItemsContainer.Padding = new MarginPadding(5);
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleOpen = audio.Samples.Get(@"overlay-pop-in");
            sampleClose = audio.Samples.Get(@"overlay-pop-out");
        }

        protected override void AnimateOpen()
        {
            if (!TopLevelMenu && !wasOpened)
                sampleOpen?.Play();

            this.FadeIn(300, Easing.OutQuint);
            wasOpened = true;
        }

        protected override void AnimateClose()
        {
            if (!TopLevelMenu && wasOpened)
                sampleClose?.Play();

            this.FadeOut(300, Easing.OutQuint);
            wasOpened = false;
        }

        protected override void UpdateSize(Vector2 newSize)
        {
            if (Direction == Direction.Vertical)
            {
                Width = newSize.X;
                this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
            }
            else
            {
                Height = newSize.Y;
                this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
            }
        }

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
        {
            switch (item)
            {
                case StatefulMenuItem stateful:
                    return new DrawableStatefulMenuItem(stateful);
            }

            return new DrawableCircleMenuItem(item);
        }

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new CircleScrollContainer(direction);

        protected override Menu CreateSubMenu() => new CircleMenu(Direction.Vertical)
        {
            Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
        };
    }
}
