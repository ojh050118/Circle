using Circle.Game.Beatmap;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Objects
{
    public abstract class Tile : Container
    {
        public Bindable<bool> Reverse;
        public Bindable<float> Bpm;
        public Bindable<float> BpmMultiplier;
        public Easing Easing = Easing.None;

        public readonly TileType TileType;
        public readonly float Angle;
        public SpeedType? SpeedType;

        public Color4 IconColour
        {
            get => icon.Colour;
            set => icon.Colour = value;
        }

        private readonly SpriteIcon icon;

        public const float WIDTH = 150;
        public const float HEIGHT = 50;

        protected Tile(TileType type, float angle)
        {
            Size = new Vector2(WIDTH, HEIGHT);
            Alpha = 0.6f;
            Anchor = Anchor.Centre;
            OriginPosition = new Vector2(25);
            TileType = type;
            Angle = angle;
            Reverse = new Bindable<bool>(false);
            Bpm = new Bindable<float>(0);
            BpmMultiplier = new Bindable<float>(1);
            icon = new SpriteIcon
            {
                Size = new Vector2(35),
                Origin = Anchor.Centre,
                Position = new Vector2(25),
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(icon);
            Reverse.ValueChanged += _ => icon.Icon = FontAwesome.Solid.UndoAlt;
            Bpm.ValueChanged += _ => icon.Icon = FontAwesome.Solid.TachometerAlt;
            BpmMultiplier.ValueChanged += _ => icon.Icon = FontAwesome.Solid.TachometerAlt;
        }
    }
}
