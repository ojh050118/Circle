#nullable disable

using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play
{
    public partial class ObjectContainer : Container<DrawableTile>
    {
        private readonly Beatmap currentBeatmap;

        [Resolved]
        private CircleConfigManager config { get; set; }

        public ObjectContainer(Beatmap beatmap)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AutoSizeAxes = Axes.Both;
            currentBeatmap = beatmap;
        }

        public void AddTileTransforms()
        {
            float bpm;
            var tiles = currentBeatmap.Tiles.ToArray();
            int frontVisibilityCount = config.Get<int>(CircleSetting.TileFrontDistance);
            int backVisibilityCount = config.Get<int>(CircleSetting.TileBackDistance);

            for (int i = frontVisibilityCount; i < tiles.Length; i++)
                Children[i].Alpha = 0;

            // Fade in
            for (int i = frontVisibilityCount; i < tiles.Length; i++)
            {
                // TODO: 저 8은 뭐지??
                bpm = tiles[i - 8].Bpm;
                Children[i].LifetimeStart = tiles[i - frontVisibilityCount].HitTime;

                using (Children[i].BeginAbsoluteSequence(tiles[i - frontVisibilityCount].HitTime, false))
                    Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);
            }

            // Fade out
            for (int i = 0; i < tiles.Length; i++)
            {
                bpm = tiles[i].Bpm;

                if (i > backVisibilityCount - 1)
                {
                    Children[i - backVisibilityCount].LifetimeEnd = tiles[i].HitTime + 60000 / bpm;
                    using (Children[i - backVisibilityCount].BeginAbsoluteSequence(tiles[i].HitTime, false))
                        Children[i - backVisibilityCount].FadeOut(60000 / bpm, Easing.Out).Then().Expire();
                }
            }
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            createTiles();
        }

        private void createTiles()
        {
            var tiles = currentBeatmap.Tiles.ToArray();

            for (int i = 0; i < tiles.Length; i++)
            {
                switch (tiles[i].TileType)
                {
                    case TileType.Normal:
                        Add(new BasicTile
                        {
                            Position = tiles[i].Position,
                            Rotation = tiles[i].Angle,
                            Tile = tiles[i]
                        });

                        break;

                    case TileType.Midspin:
                        Add(new MidspinTile
                        {
                            Position = tiles[i].Position,
                            Rotation = tiles[i].Angle,
                            Tile = tiles[i]
                        });

                        break;

                    case TileType.Short:
                        Add(new ShortTile
                        {
                            Position = tiles[i].Position,
                            Rotation = tiles[i].Angle,
                            Tile = tiles[i]
                        });

                        break;

                    case TileType.Circular:
                        Add(new CircularTile
                        {
                            Position = tiles[i].Position,
                            Rotation = tiles[i].Angle,
                            Tile = tiles[i]
                        });

                        break;
                }
            }
        }
    }
}
