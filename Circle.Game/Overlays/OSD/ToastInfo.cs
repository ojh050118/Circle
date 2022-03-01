using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game.Overlays.OSD
{
    public class ToastInfo
    {
        public string Description { get; set; }

        public string SubDescription { get; set; }

        public IconUsage Icon { get; set; }

        public Color4 IconColour { get; set; } = Color4.Gray;

        public bool Closable { get; set; }

        public string Sample { get; set; } = "notification-pop-in";
    }
}
