#nullable disable

using Circle.Game.Beatmaps;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public class Tile
    {
        // TODO: 레벨 설정에 맞게 타일 표시 시간 지정
        public double StartTime { get; set; }

        public double Duration { get; set; }

        public double EndTime => StartTime + Duration;

        /// <summary>
        /// 행성이 도착하는 시간. 박자의 타이밍이자 이벤트 실행 시점이 됩니다.
        /// </summary>
        public double HitTime { get; set; }

        public int Floor { get; set; }

        public float Angle { get; set; }

        public Vector2 Position { get; set; }

        public bool Clockwise { get; set; }

        public float Bpm { get; set; }

        public ActionEvents[] Actions { get; set; }

        public TileType TileType { get; set; }

        public override string ToString()
        {
            return $"{Floor}. {TileType} | Hit time: {HitTime} | Actions: {Actions.Length}";
        }
    }
}
