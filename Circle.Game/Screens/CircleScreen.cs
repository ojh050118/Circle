using Circle.Game.Input;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace Circle.Game.Screens
{
    public class CircleScreen : Screen, ICircleScreen, IKeyBindingHandler<InputAction>
    {
        public virtual bool BlockExit => false;

        public virtual string Header => string.Empty;

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
        }

        public override bool OnExiting(IScreen next)
        {
            this.MoveToX(DrawWidth, 1000, Easing.OutPow10);

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            X = DrawWidth;
            this.MoveToX(0, 1000, Easing.OutPow10);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);

            this.MoveToX(DrawWidth, 1000, Easing.OutPow10);
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
