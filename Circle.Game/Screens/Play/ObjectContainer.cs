using System;
using System.Collections.Generic;
using Circle.Game.Beatmap;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Screens.Play
{
    public class ObjectContainer : Container<Tile>
    {
        /// <summary>
        /// 다음 타일의 위치. (초기 값: Vector2.Zero)
        /// </summary>
        private Vector2 tilePosition = Vector2.Zero;

        private float[] angleData;

        private BeatmapInfo beatmapInfo;

        public ObjectContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<BeatmapInfo> beatmap)
        {
            beatmapInfo = beatmap.Value;
            angleData = convertAngles(beatmap.Value);
            createTiles(angleData);
            addActionsToTile();
        }

        private void createTiles(float[] angleData)
        {
            if (angleData.Length == 0)
                return;

            for (int i = 0; i < angleData.Length; i++)
            {
                if (i > angleData.Length - 1)
                    continue;

                var newTilePosition = CalculationExtensions.GetComputedTilePosition(angleData[i]);

                if (i < angleData.Length - 1)
                {
                    if (angleData[i + 1] == 999)
                    {
                        Add(new ShortTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });

                        i++;
                    }

                    if (Math.Abs(angleData[i] - angleData[i + 1]) == 180)
                    {
                        Add(new ShortTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });

                        tilePosition += newTilePosition;

                        continue;
                    }
                }

                if (angleData[i] == 999)
                {
                    Add(new MidspinTile(999)
                    {
                        Position = tilePosition,
                        Rotation = angleData[i - 1]
                    });

                    i++;
                    newTilePosition = CalculationExtensions.GetComputedTilePosition(angleData[i]);
                }

                if (i > 0)
                {
                    if (Math.Abs(angleData[i] - angleData[i - 1]) == 180)
                    {
                        Add(new CircularTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });

                        tilePosition += newTilePosition;

                        continue;
                    }
                }

                Add(new BasicTile(angleData[i])
                {
                    Position = tilePosition,
                    Rotation = angleData[i],
                });

                tilePosition += newTilePosition;
            }
        }

        private void addActionsToTile()
        {
            if (beatmapInfo.Actions is null)
                return;

            foreach (var action in beatmapInfo.Actions)
            {
                switch (action.EventType)
                {
                    case EventType.Twirl:
                        Children[action.Floor].Reverse.Value = true;
                        break;

                    case EventType.SetSpeed:
                        switch (action.SpeedType)
                        {
                            case SpeedType.Multiplier:
                                Children[action.Floor].SpeedType = SpeedType.Multiplier;
                                Children[action.Floor].BpmMultiplier.Value = action.BpmMultiplier;
                                break;

                            case SpeedType.Bpm:
                                Children[action.Floor].SpeedType = SpeedType.Bpm;
                                Children[action.Floor].Bpm.Value = action.BeatsPerMinute;
                                break;
                        }

                        break;

                    case EventType.MoveCamera:
                        // Todo: 카메라 기능 추가
                        break;
                }
            }
        }

        /// <summary>
        /// 반시계 방향으로 되어있는 각도데이터를 시계방향으로 변환합니다.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>시계방향으로 변환된 각도데이터.</returns>
        private float[] convertAngles(BeatmapInfo info)
        {
            List<float> newAngleData = new List<float>();

            for (int i = 0; i < info.AngleData.Length; i++)
            {
                if (info.AngleData[i] == 999 || info.AngleData[i] <= 0)
                {
                    newAngleData.Add(info.AngleData[i]);
                    continue;
                }

                newAngleData.Add(360 - info.AngleData[i]);
            }

            return newAngleData.ToArray();
        }
    }
}
