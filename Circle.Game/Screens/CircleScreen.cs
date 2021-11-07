using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Screens;
using osu.Framework.Graphics;

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
