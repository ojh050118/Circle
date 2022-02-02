using Circle.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.Containers
{
    public abstract class CircleFocusedOverlayContainer : FocusedOverlayContainer, IKeyBindingHandler<InputAction>
    {
        private Sample samplePopIn;
        private Sample samplePopOut;

        private Box dim;

        protected override bool BlockNonPositionalInput => true;

        public new Container<Drawable> Content { get; set; }

        private readonly BufferedContainer bluredBackground;

        public bool BlockInputAction { get; set; }

        protected CircleFocusedOverlayContainer(BufferedContainer screenContainer)
        {
            bluredBackground = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = screenContainer.CreateView().With(d =>
                {
                    d.RelativeSizeAxes = Axes.Both;
                    d.SynchronisedDrawQuad = true;
                    d.DisplayOriginalEffects = true;
                })
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load(AudioManager audio)
        {
            samplePopIn = audio.Samples.Get("overlay-pop-in");
            samplePopOut = audio.Samples.Get("overlay-pop-out");
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                bluredBackground,
                dim = new Box
                {
                    Colour = Color4.Black,
                    Alpha = 0.3f,
                    RelativeSizeAxes = Axes.Both,
                },
                Content
            };
        }

        protected override void UpdateState(ValueChangedEvent<Visibility> state)
        {
            bool didChange = state.OldValue != state.NewValue;

            switch (state.NewValue)
            {
                case Visibility.Visible:
                    if (didChange)
                        samplePopIn?.Play();
                    break;

                case Visibility.Hidden:
                    if (didChange)
                        samplePopOut?.Play();
                    break;
            }

            base.UpdateState(state);
        }

        public virtual bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            if (e.Action == InputAction.Back && !BlockInputAction)
            {
                Hide();
                return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }

        protected override void PopIn()
        {
            dim.FadeTo(0.4f, 1000, Easing.OutPow10);
            bluredBackground.FadeIn(0).BlurTo(new Vector2(10), 1000, Easing.OutPow10);
            base.PopIn();
        }

        protected override void PopOut()
        {
            dim.FadeOut(1000, Easing.OutPow10);
            bluredBackground.BlurTo(new Vector2(0), 1000, Easing.OutPow10).Then().FadeOut(0);
            base.PopOut();
        }
    }
}
