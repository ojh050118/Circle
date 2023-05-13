using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play
{
    public class ObjectContainer : Container<Tile>
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

        [BackgroundDependencyLoader]
        private void load()
        {
            createTiles();
        }

        private void createTiles()
        {
            for (int i = 0; i < currentBeatmap.TilesInfo.Length; i++)
            {
                switch (currentBeatmap.TilesInfo[i].TileType)
                {
                    case TileType.Normal:
                        Add(new BasicTile
                        {
                            Position = currentBeatmap.TilesInfo[i].Position,
                            Rotation = currentBeatmap.TilesInfo[i].Angle,
                            TileInfo = currentBeatmap.TilesInfo[i]
                        });

                        break;

                    case TileType.Midspin:
                        Add(new MidspinTile
                        {
                            Position = currentBeatmap.TilesInfo[i].Position,
                            Rotation = currentBeatmap.TilesInfo[i].Angle,
                            TileInfo = currentBeatmap.TilesInfo[i]
                        });

                        break;

                    case TileType.Short:
                        Add(new ShortTile
                        {
                            Position = currentBeatmap.TilesInfo[i].Position,
                            Rotation = currentBeatmap.TilesInfo[i].Angle,
                            TileInfo = currentBeatmap.TilesInfo[i]
                        });

                        break;

                    case TileType.Circular:
                        Add(new CircularTile
                        {
                            Position = currentBeatmap.TilesInfo[i].Position,
                            Rotation = currentBeatmap.TilesInfo[i].Angle,
                            TileInfo = currentBeatmap.TilesInfo[i]
                        });

                        break;
                }
            }
        }

        public void AddTileTransforms(double gameStartTime, double countdownDuration)
        {
            float bpm = currentBeatmap.Settings.Bpm;
            var tilesOffset = CalculationExtensions.GetTileStartTime(currentBeatmap, gameStartTime, countdownDuration);
            var frontVisibilityCount = config.Get<int>(CircleSetting.TileFrontDistance);
            var backVisibilityCount = config.Get<int>(CircleSetting.TileBackDistance);

            for (int i = frontVisibilityCount; i < currentBeatmap.TilesInfo.Length; i++)
                Children[i].Alpha = 0;

            // Fade in
            for (int i = frontVisibilityCount; i < currentBeatmap.TilesInfo.Length; i++)
            {
                bpm = currentBeatmap.TilesInfo.GetNewBpm(bpm, i - 8);
                Children[i].LifetimeStart = tilesOffset[i - frontVisibilityCount];

                using (Children[i].BeginAbsoluteSequence(tilesOffset[i - frontVisibilityCount], false))
                    Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);
            }

            bpm = currentBeatmap.Settings.Bpm;

            // Fade out
            for (int i = 0; i < currentBeatmap.TilesInfo.Length; i++)
            {
                bpm = currentBeatmap.TilesInfo.GetNewBpm(bpm, i);

                if (i > backVisibilityCount - 1)
                {
                    Children[i - backVisibilityCount].LifetimeEnd = tilesOffset[i] + 60000 / bpm;
                    using (Children[i - backVisibilityCount].BeginAbsoluteSequence(tilesOffset[i], false))
                        Children[i - backVisibilityCount].FadeOut(60000 / bpm, Easing.Out).Then().Expire();
                }
            }
        }
    }
}
