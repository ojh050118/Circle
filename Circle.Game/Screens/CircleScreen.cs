#nullable disable

using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using Circle.Game.Overlays;
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
    public partial class CircleScreen : Screen, ICircleScreen, IKeyBindingHandler<InputAction>
    {
        private Sample sampleBack;

        public CircleScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        public virtual bool FadeBackground => true;

        public virtual bool PlaySample => true;

        [Resolved]
        private Background background { get; set; }

        [Resolved]
        private ImportOverlay importOverlay { get; set; }

        public virtual bool BlockExit => false;

        public virtual string Header => string.Empty;

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);
            this.MoveToX(DrawWidth).MoveToX(0, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(background.Dim + 0.4f, 1000, Easing.OutPow10);
                background.BlurTo(background.BlurSigma + new Vector2(10), 1000, Easing.OutPow10);
            }
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.MoveToX(DrawWidth, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(background.Dim - 0.4f, 1000, Easing.OutPow10);
                background.BlurTo(background.BlurSigma - new Vector2(10), 1000, Easing.OutPow10);
            }

            sampleBack?.Play();

            return base.OnExiting(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);

            this.MoveToX(0, 1000, Easing.OutPow10);
            this.FadeIn(1000, Easing.OutPow10);
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            base.OnSuspending(e);

            this.MoveToX(-DrawWidth * 0.5f, 2500, Easing.OutPow10);
            this.FadeOut(1250, Easing.OutPow10);
        }

        public virtual bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.ToggleImportOverlay:
                    importOverlay.ToggleVisibility();
                    return true;

                case InputAction.ReloadBeatmap:
                    // TODO: 사라져야할 기능
                    return true;

                case InputAction.Back:
                    if (!BlockExit)
                    {
                        OnExit();
                        return true;
                    }

                    return false;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            if (PlaySample)
                sampleBack = audio.Samples.Get("screen-back");
        }

        public virtual void OnExit() => this.Exit();
    }
}
