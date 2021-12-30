using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Objects
{
    public class Planet : CompositeDrawable
    {
        private float expansion;
        private readonly Container adjustableContent;

        public const float PLANET_SIZE = 50;
        public const float DISTANCE = 100;

        public float Expansion
        {
            get => expansion;
            set
            {
                expansion = value;
                adjustableContent.Width = value * DISTANCE;
            }
        }

        public Planet(Color4 planetColor)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(DISTANCE * 2);
            InternalChild = adjustableContent = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                Height = 50,
                Width = DISTANCE,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.Centre,
                        Size = new Vector2(PLANET_SIZE),
                        Child = new CircularContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = planetColor
                            }
                        }
                    },
                    new Box
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 15,
                        Colour = planetColor,
                        Alpha = 0.75f
                    }
                }
            };
        }
    }
}
