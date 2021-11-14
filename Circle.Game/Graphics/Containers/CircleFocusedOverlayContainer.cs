﻿using Circle.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
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

        [BackgroundDependencyLoader(true)]
        private void load(AudioManager audio)
        {
            samplePopIn = audio.Samples.Get("overlay-pop-in");
            samplePopOut = audio.Samples.Get("overlay-pop-out");
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
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
            if (e.Action == InputAction.Back)
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
            dim.FadeTo(0.3f, 1000, Easing.OutPow10);
            base.PopIn();
        }

        protected override void PopOut()
        {
            dim.FadeOut(1000, Easing.OutPow10);
            base.PopOut();
        }
    }
}