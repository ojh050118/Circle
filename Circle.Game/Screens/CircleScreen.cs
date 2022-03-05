using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace Circle.Game.Screens
{
    public class CircleScreen : Screen, ICircleScreen, IKeyBindingHandler<InputAction>
    {
        public virtual bool BlockExit => false;

        public virtual string Header => string.Empty;

        public virtual bool FadeBackground => true;

        public virtual bool PlaySample => true;

        private Sample sampleBack;

        [Resolved]
        private Background background { get; set; }

        public CircleScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            if (PlaySample)
                sampleBack = audio.Samples.Get("screen-back");
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            this.MoveToX(DrawWidth).MoveToX(0, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(background.Dim + 0.4f, 1000, Easing.OutPow10);
                background.BlurTo(background.BlurSigma + new Vector2(10), 1000, Easing.OutPow10);
            }
        }

        public override bool OnExiting(IScreen next)
        {
            this.MoveToX(DrawWidth, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(background.Dim - 0.4f, 1000, Easing.OutPow10);
                background.BlurTo(background.BlurSigma - new Vector2(10), 1000, Easing.OutPow10);
            }

            sampleBack?.Play();

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            this.MoveToX(0, 1000, Easing.OutPow10);
            this.FadeIn(1000, Easing.OutPow10);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);

            this.MoveToX(-DrawWidth * 0.5f, 2500, Easing.OutPow10);
            this.FadeOut(1250, Easing.OutPow10);
        }

        public virtual bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            if (e.Action == InputAction.Back && !BlockExit)
            {
                OnExit();
                return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }

        public virtual void OnExit() => this.Exit();
    }
}
