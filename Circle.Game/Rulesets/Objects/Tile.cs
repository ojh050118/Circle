#nullable disable

using Circle.Game.Beatmaps;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public class Tile
    {
        /// <summary>
        /// 행성이 공전을 시작하는 시간. 박자의 타이밍이자 이벤트 실행 시점이 됩니다.
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// 이 타일에 도착하기 까지의 소요시간.
        /// </summary>
        public double Duration => HitTime - StartTime;

        /// <summary>
        /// 행성이 도착하는 시간.
        /// </summary>
        public double EndTime => HitTime;

        /// <summary>
        /// 행성이 도착하는 시간.
        /// </summary>
        public double HitTime { get; set; }

        public int Floor { get; set; }

        public float Angle { get; set; }

        public Vector2 Position { get; set; }

        public bool Clockwise { get; set; }

        public float Bpm { get; set; }

        public ActionEvents[] Actions { get; set; }

        public TileType TileType { get; set; }

        public Easing Easing { get; set; }

        public override string ToString()
        {
            return $"{Floor}. {TileType} | Hit time: {HitTime} | Actions: {Actions.Length}";
        }
    }
}
