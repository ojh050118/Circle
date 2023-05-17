#nullable disable

using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Graphics.Sprites
{
    public partial class CircleSpriteText : SpriteText
    {
        public CircleSpriteText()
        {
            Shadow = true;
            Font = CircleFont.Default;
        }
    }
}
