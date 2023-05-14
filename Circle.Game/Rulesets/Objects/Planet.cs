#nullable disable

using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using Shape = osu.Framework.Graphics.Shapes;

namespace Circle.Game.Rulesets.Objects
{
    public class Planet : CompositeDrawable
    {
        public const float PLANET_SIZE = 50;
        public const float DISTANCE = 100;
        private readonly Container adjustableContent;
        private readonly Shape.Circle planet;
        private float expansion;

        public Bindable<Color4Enum> PlanetColour;

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
                Child = planet = new Shape.Circle
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.Centre,
                    Size = new Vector2(PLANET_SIZE),
                    Colour = planetColor,
                }
            };
        }

        public float Expansion
        {
            get => expansion;
            set
            {
                expansion = value;
                adjustableContent.Width = value * DISTANCE;
            }
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (PlanetColour != null)
                PlanetColour.ValueChanged += color4Enum => planet.Colour = Color4Utils.GetColor4(color4Enum.NewValue);
        }
    }
}
