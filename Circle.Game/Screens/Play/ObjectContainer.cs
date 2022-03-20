using System;
using System.Collections.Generic;
using System.Linq;
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
                            Floor = i,
                            Position = positions[i],
                            Rotation = angleData[i],
                        });

                        break;

                    case TileType.Midspin:
                        Add(new MidspinTile
                        {
                            Floor = i,
                            Position = positions[i],
                            Rotation = getAvailableAngle(i),
                        });

                        break;

                    case TileType.Short:
                        Add(new ShortTile
                        {
                            Floor = i,
                            Position = positions[i],
                            Rotation = angleData[i],
                        });

                        break;

                    case TileType.Circular:
                        Add(new CircularTile
                        {
                            Floor = i,
                            Position = positions[i],
                            Rotation = getAvailableAngle(i),
                        });

                        break;
                }
            }

            addActionsToTile();
        }

        private IReadOnlyList<TileType> getTileType() => CalculationExtensions.GetTileType(angleData);

        private IReadOnlyList<Vector2> getTilePositions() => CalculationExtensions.GetTilePositions(angleData);

        public IReadOnlyList<TileInfo> GetTilesInfo() => CalculationExtensions.GetTilesInfo(currentBeatmap);

        private float getAvailableAngle(int floor) => CalculationExtensions.GetAvailableAngle(angleData, floor);

        private void addActionsToTile()
        {
            if (currentBeatmap.Actions is null)
                return;

            foreach (var action in currentBeatmap.Actions)
            {
                switch (action.EventType)
                {
                    case EventType.Twirl:
                        Children[action.Floor].Twirl = true;
                        break;

                    case EventType.SetSpeed:
                        switch (action.SpeedType)
                        {
                            case SpeedType.Multiplier:
                                if (action.Floor < Children.Count)
                                {
                                    Children[action.Floor].SpeedType = SpeedType.Multiplier;
                                    Children[action.Floor].BpmMultiplier = action.BpmMultiplier;
                                }

                                break;

                            case SpeedType.Bpm:
                                if (action.Floor < Children.Count)
                                {
                                    Children[action.Floor].SpeedType = SpeedType.Bpm;
                                    Children[action.Floor].Bpm = action.BeatsPerMinute;
                                }

                                break;
                        }

                        break;

                    case EventType.SetPlanetRotation:
                        Children[action.Floor].Easing = action.Ease;
                        break;

                    case EventType.MoveCamera:
                        // Todo: 카메라 기능 추가
                        break;
                }
            }
        }
    }
}
