using Circle.Game.Beatmap;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public abstract class Tile : Container, IHasTileInfo
    {
        public int Floor { get; set; }

        public EventType? EventType { get; set; }

        public TileType TileType { get; }
        public SpeedType? SpeedType { get; set; } = null;

        public Easing Easing { get; set; } = Easing.None;

        public bool Twirl { get; set; }

        public float Bpm { get; set; }

        public float BpmMultiplier { get; set; }

        public float Angle { get; set; }

        private readonly SpriteIcon icon;

        public const float WIDTH = 150;
        public const float HEIGHT = 50;

        protected Tile(TileType type)
        {
            Anchor = Anchor.Centre;
            OriginPosition = new Vector2(25);
            Alpha = 0.6f;
            Size = new Vector2(WIDTH, HEIGHT);
            TileType = type;
            icon = new SpriteIcon
            {
                Size = new Vector2(35),
                Origin = Anchor.Centre,
                Position = new Vector2(25),
            };
        }

        protected override void LoadComplete()
        {
            if (SpeedType != null)
                icon.Icon = FontAwesome.Solid.TachometerAlt;

            if (Twirl)
                icon.Icon = FontAwesome.Solid.UndoAlt;

            Add(icon);

            base.LoadComplete();
        }
    }
}
