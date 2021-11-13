using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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

        [Resolved]
        private Background background { get; set; }

        public CircleScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            X = DrawWidth;
            this.MoveToX(0, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(0.5f, 1000, Easing.OutPow10);
                background.BlurTo(new Vector2(10), 1000, Easing.OutPow10);
            }
        }

        public override bool OnExiting(IScreen next)
        {
            this.MoveToX(DrawWidth, 1000, Easing.OutPow10);

            if (FadeBackground)
            {
                background.DimTo(0, 1000, Easing.OutPow10);
                background.BlurTo(Vector2.Zero, 1000, Easing.OutPow10);
            }

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            this.MoveToX(0, 1000, Easing.OutPow10);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);

            this.MoveToX(-DrawWidth * 0.5f, 2500, Easing.OutPow10);
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
