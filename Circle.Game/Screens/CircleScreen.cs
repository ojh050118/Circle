using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace Circle.Game.Screens
{
    public class CircleScreen : Screen, ICircleScreen
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

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.Escape && !e.Repeat && !BlockExit)
            {
                OnExit();
                return true;
            }

            return base.OnKeyDown(e);
        }

        protected virtual void OnExit() => this.Exit();
    }
}
