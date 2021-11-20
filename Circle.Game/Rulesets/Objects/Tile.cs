using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Graphics.Effects;

namespace Circle.Game.Rulesets.Objects
{
    public abstract class Tile : Container
    {
        public Bindable<bool> Reverse;
        public Bindable<float> BPM;
        public Bindable<Easing> Easing;
        public readonly float Angle;

        public const float WIDTH = 150;
        public const float HEIGHT = 50;

        public Tile(float angle)
        {
            Size = new Vector2(WIDTH, HEIGHT);
            Alpha = 0.6f;
            Anchor = Anchor.Centre;
            OriginPosition = new Vector2(25);
            Angle = angle;
        }
    }
}
