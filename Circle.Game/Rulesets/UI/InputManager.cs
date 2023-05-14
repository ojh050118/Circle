using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Circle.Game.Rulesets.UI
{
    public class InputManager : Container
    {
        public override bool PropagatePositionalInputSubTree => false;
        public override bool PropagateNonPositionalInputSubTree => false;

        public Beatmap Beatmap { get; set; }

        private readonly IReadOnlyList<Key> allowedKeys;
        private IReadOnlyList<double> tileHitTimes => Beatmap.TileStartTime;
        public double TimeUntilNextBeat;
        public double TimeSinceLastBeat;
        public int Floor = 1;

        public InputManager()
        {
            List<Key> keys = new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right, Key.Space };

            for (int i = 67; i <= 130; i++)
                keys.Add((Key)i);

            allowedKeys = keys;
        }

        protected override void LoadComplete()
        {
            for (int i = 1; i < tileHitTimes.Count - 1; i++)
            {
                using (BeginAbsoluteSequence(tileHitTimes[i], false))
                    this.TransformTo("Floor", i + 1);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Floor < tileHitTimes.Count)
            {
                TimeUntilNextBeat = tileHitTimes[Floor] - Time.Current;

                if (Floor - 1 > 0)
                    TimeSinceLastBeat = Time.Current - tileHitTimes[Floor - 1];
            }
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            return allowedKeys.All(key => e.Key != key);
        }
    }
}
