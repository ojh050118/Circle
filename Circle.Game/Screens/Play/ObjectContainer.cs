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

        private float[] angleData => CalculationExtensions.ConvertAngles(currentBeatmap.AngleData);

        [BackgroundDependencyLoader]
        private void load()
        {
            createTiles();
        }

        private void createTiles()
        {
            var info = currentBeatmap.TilesInfo;

            for (int i = 0; i < angleData.Length; i++)
            {
                switch (info[i].TileType)
                {
                    case TileType.Normal:
                        Add(new BasicTile
                        {
                            Position = info[i].Position,
                            Rotation = info[i].Angle,
                            TileInfo = info[i]
                        });

                        break;

                    case TileType.Midspin:
                        Add(new MidspinTile
                        {
                            Position = info[i].Position,
                            Rotation = info[i].Angle,
                            TileInfo = info[i]
                        });

                        break;

                    case TileType.Short:
                        Add(new ShortTile
                        {
                            Position = info[i].Position,
                            Rotation = info[i].Angle,
                            TileInfo = info[i]
                        });

                        break;

                    case TileType.Circular:
                        Add(new CircularTile
                        {
                            Position = info[i].Position,
                            Rotation = info[i].Angle,
                            TileInfo = info[i]
                        });

                        break;
                }
            }
        }

        public void AddTileTransforms(double gameStartTime, double countdownDuration)
        {
            float bpm = currentBeatmap.Settings.Bpm;
            var tilesOffset = CalculationExtensions.GetTileStartTime(currentBeatmap, gameStartTime, countdownDuration);
            var tilesInfo = currentBeatmap.TilesInfo;
            var frontVisibilityCount = config.Get<int>(CircleSetting.TileFrontDistance);
            var backVisibilityCount = config.Get<int>(CircleSetting.TileBackDistance);

            for (int i = frontVisibilityCount; i < tilesInfo.Count; i++)
                Children[i].Alpha = 0;

            // Fade in
            for (int i = frontVisibilityCount; i < tilesInfo.Count; i++)
            {
                bpm = tilesInfo.GetNewBpm(bpm, i - 8);
                Children[i].LifetimeStart = tilesOffset[i - frontVisibilityCount];

                using (Children[i].BeginAbsoluteSequence(tilesOffset[i - frontVisibilityCount], false))
                    Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);
            }

            bpm = currentBeatmap.Settings.Bpm;

            // Fade out
            for (int i = 0; i < tilesInfo.Count; i++)
            {
                bpm = tilesInfo.GetNewBpm(bpm, i);

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
