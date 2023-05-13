using Circle.Game.Beatmaps;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public abstract class Tile : Container
    {
        public TileInfo TileInfo { get; set; }

        private readonly SpriteIcon icon;

        public const float WIDTH = 150;
        public const float HEIGHT = 50;

        protected Tile()
        {
            Anchor = Anchor.Centre;
            OriginPosition = new Vector2(25);
            Alpha = 0.45f;
            Size = new Vector2(WIDTH, HEIGHT);
            icon = new SpriteIcon
            {
                Size = new Vector2(35),
                Origin = Anchor.Centre,
                Position = new Vector2(25),
            };
        }

        protected override void LoadComplete()
        {
            if (TileInfo == null)
                return;

            foreach (var action in TileInfo.Action)
            {
                if (action.SpeedType != null)
                    icon.Icon = FontAwesome.Solid.TachometerAlt;

                if (action.EventType == EventType.Twirl)
                    icon.Icon = FontAwesome.Solid.UndoAlt;
            }

            Add(icon);

            base.LoadComplete();
        }
    }
}
