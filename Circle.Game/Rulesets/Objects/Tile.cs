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
        public Easing Easing;
        public readonly float Angle;
        public Color4 IconColour
        {
            get => icon.Colour;
            set => icon.Colour = value;
        }

        private SpriteIcon icon;

        public const float WIDTH = 150;
        public const float HEIGHT = 50;

        protected Tile(float angle)
        {
            Size = new Vector2(WIDTH, HEIGHT);
            Alpha = 0.6f;
            Anchor = Anchor.Centre;
            OriginPosition = new Vector2(25);
            Angle = angle;
            Reverse = new Bindable<bool>(false);
            Bpm = new Bindable<float>(0);
            BpmMultiplier = new Bindable<float>(-1);
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
            Reverse.ValueChanged += value => addTwirlIcon(value.NewValue);
            Bpm.ValueChanged += _ => addSpeedIcon();
            BpmMultiplier.ValueChanged += _ => addSpeedIcon();
        }

        private void addTwirlIcon(bool reverse)
        {
            icon.Icon = FontAwesome.Solid.UndoAlt;
        }

        private void addSpeedIcon()
        {
            icon.Icon = FontAwesome.Solid.TachometerAlt;
        }
    }
}
