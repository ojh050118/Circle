using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Circle.Game.Screens
{
    public class CircleScreen : Screen
    {
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
    }
}
