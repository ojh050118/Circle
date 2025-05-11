#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Circle.Game.Rulesets.UI
{
    public partial class InputManager : Container
    {
        private readonly IReadOnlyList<Key> allowedKeys;
        public int Floor = 1;
        public double TimeSinceLastBeat;
        public double TimeUntilNextBeat;

        public InputManager()
        {
            List<Key> keys = new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right, Key.Space };

            for (int i = 67; i <= 130; i++)
                keys.Add((Key)i);

            allowedKeys = keys;
        }

        public override bool PropagatePositionalInputSubTree => false;
        public override bool PropagateNonPositionalInputSubTree => false;

        public Beatmap Beatmap { get; set; }

        private IReadOnlyList<Tile> tiles;

        [BackgroundDependencyLoader]
        private void load()
        {
            tiles = Beatmap.Tiles.ToList();
        }

        protected override void LoadComplete()
        {
            for (int i = 1; i < tiles.Count - 1; i++)
            {
                using (BeginAbsoluteSequence(tiles[i].HitTime, false))
                    this.TransformTo(nameof(Floor), i + 1);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Floor < tiles.Count)
            {
                TimeUntilNextBeat = tiles[Floor].HitTime - Time.Current;

                if (Floor - 1 > 0)
                    TimeSinceLastBeat = Time.Current - tiles[Floor - 1].HitTime;
            }
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            return allowedKeys.All(key => e.Key != key);
        }
    }
}
