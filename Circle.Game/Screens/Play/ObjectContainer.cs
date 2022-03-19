﻿using System;
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
        private float[] angleData => convertAngles(currentBeatmap.AngleData);

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

        private IReadOnlyList<TileType> getTileType()
        {
            List<TileType> tileTypes = new List<TileType>();

            for (int floor = 0; floor < angleData.Length; floor++)
            {
                if (floor + 1 < angleData.Length)
                {
                    if (angleData[floor + 1] == 999 && angleData[floor] != 999)
                    {
                        tileTypes.Add(TileType.Short);
                        continue;
                    }
                    else if (Math.Abs(angleData[floor] - angleData[floor + 1]) == 180)
                    {
                        tileTypes.Add(TileType.Short);
                        continue;
                    }
                    else if (floor > 0)
                    {
                        if (Math.Abs(angleData[floor] - angleData[floor - 1]) == 180)
                        {
                            tileTypes.Add(TileType.Circular);
                            continue;
                        }
                    }
                }
                else
                {
                    // 마지막 타일을 의미합니다.
                    tileTypes.Add(TileType.Circular);
                    break;
                }

                if (angleData[floor] == 999)
                {
                    tileTypes.Add(TileType.Midspin);
                    continue;
                }

                tileTypes.Add(TileType.Normal);
            }

            return tileTypes;
        }

        private IReadOnlyList<Vector2> getTilePositions()
        {
            var types = getTileType();
            var positions = new List<Vector2> { Vector2.Zero };
            Vector2 offset = Vector2.Zero;

            for (int i = 0; i < angleData.Length; i++)
            {
                var newTilePosition = CalculationExtensions.GetComputedTilePosition(angleData[i]);

                switch (types[i])
                {
                    case TileType.Normal:
                        offset += newTilePosition;
                        break;

                    case TileType.Midspin:

                        offset -= CalculationExtensions.GetComputedTilePosition(getAvailableAngle(i));
                        break;

                    case TileType.Short:
                        offset += newTilePosition;
                        break;

                    case TileType.Circular:
                        if (i + 1 < angleData.Length)
                            offset += newTilePosition;

                        break;
                }

                positions.Add(offset);
            }

            return positions;
        }

        public IReadOnlyList<TileInfo> GetTilesInfo()
        {
            TileInfo[] infos = new TileInfo[angleData.Length];
            var types = getTileType();
            var tilePositions = getTilePositions();

            for (int floor = 0; floor < angleData.Length; floor++)
            {
                infos[floor] = new TileInfo
                {
                    TileType = types[floor],
                    Floor = floor,
                    Angle = types[floor] == TileType.Midspin ? getAvailableAngle(floor) : angleData[floor],
                    Position = tilePositions[floor],
                };
            }

            foreach (var action in currentBeatmap.Actions)
            {
                infos[action.Floor].EventType = action.EventType;

                switch (action.EventType)
                {
                    case EventType.Twirl:
                        infos[action.Floor].Twirl = true;
                        break;

                    case EventType.SetSpeed:
                        switch (action.SpeedType)
                        {
                            case SpeedType.Multiplier:
                                infos[action.Floor].SpeedType = SpeedType.Multiplier;
                                infos[action.Floor].BpmMultiplier = action.BpmMultiplier;
                                break;

                            case SpeedType.Bpm:
                                infos[action.Floor].SpeedType = SpeedType.Bpm;
                                infos[action.Floor].Bpm = action.BeatsPerMinute;
                                break;
                        }

                        break;

                    case EventType.SetPlanetRotation:
                        infos[action.Floor].Easing = action.Ease;
                        break;

                    case EventType.MoveCamera:
                        // Todo: 카메라 기능 추가
                        break;

                    case EventType.Other:
                        break;
                }
            }

            return infos;
        }

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

        private float getAvailableAngle(int index)
        {
            var availableAngle = 0f;
            for (int i = index - 1; i >= 0; i--)
            {
                if (angleData[i] != 999)
                {
                    availableAngle += angleData[i];
                    break;
                }
                else
                    availableAngle += 180;
            }

            return availableAngle;
        }

        /// <summary>
        /// 반시계 방향으로 되어있는 각도데이터를 시계방향으로 변환합니다.
        /// </summary>
        /// <param name="targetAngleData"></param>
        /// <returns>시계방향으로 변환된 각도데이터.</returns>
        private float[] convertAngles(float[] targetAngleData)
        {
            List<float> newAngleData = new List<float>();

            for (int i = 0; i < targetAngleData.Length; i++)
            {
                if (targetAngleData[i] == 999 || targetAngleData[i] <= 0)
                {
                    if (targetAngleData[i] >= 0)
                        newAngleData.Add(targetAngleData[i]);
                    else
                        newAngleData.Add(newAngleData[i - 1] - 180);
                    continue;
                }

                newAngleData.Add(360 - targetAngleData[i]);
            }

            newAngleData.Add(360 - targetAngleData.Last());

            return newAngleData.ToArray();
        }
    }
}
