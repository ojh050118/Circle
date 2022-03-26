using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Screens.Play
{
    public class ObjectContainer : Container<Tile>
    {
        private float[] angleData => CalculationExtensions.ConvertAngles(currentBeatmap.AngleData);

        private readonly Beatmap currentBeatmap;

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
            var types = getTileType();
            var positions = getTilePositions();

            for (int i = 0; i < angleData.Length; i++)
            {
                switch (types[i])
                {
                    case TileType.Normal:
                        Add(new BasicTile
                        {
                            Position = positions[i],
                            Rotation = angleData[i],
                            Action = addActions(i)
                        });

                        break;

                    case TileType.Midspin:
                        Add(new MidspinTile
                        {
                            Position = positions[i],
                            Rotation = getAvailableAngle(i),
                            Action = addActions(i)
                        });

                        break;

                    case TileType.Short:
                        Add(new ShortTile
                        {
                            Position = positions[i],
                            Rotation = angleData[i],
                            Action = addActions(i)
                        });

                        break;

                    case TileType.Circular:
                        Add(new CircularTile
                        {
                            Position = positions[i],
                            Rotation = getAvailableAngle(i),
                            Action = addActions(i)
                        });

                        break;
                }
            }
        }

        private IReadOnlyList<TileType> getTileType() => CalculationExtensions.GetTileType(angleData);

        private IReadOnlyList<Vector2> getTilePositions() => CalculationExtensions.GetTilePositions(angleData);

        public IReadOnlyList<TileInfo> GetTilesInfo() => CalculationExtensions.GetTilesInfo(currentBeatmap);

        private float getAvailableAngle(int floor) => CalculationExtensions.GetAvailableAngle(angleData, floor);

        private Actions[] addActions(int floor)
        {
            List<Actions> actions = new List<Actions>();

            foreach (var action in currentBeatmap.Actions)
            {
                if (floor == action.Floor)
                    actions.Add(action);
            }

            return actions.ToArray();
        }
    }
}
