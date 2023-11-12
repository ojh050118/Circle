#nullable disable

using Circle.Game.Converting.Adofai.Elements;

namespace Circle.Game.Converting.Adofai
{
    public class AdofaiBeatmap
    {
        public string PathData { get; set; }
        public float[] AngleData { get; set; }
        public Settings Settings { get; set; }
        public Action[] Actions { get; set; }
        public object[] Decorations { get; set; }
    }
}
