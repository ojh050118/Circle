using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmap;
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

        /// <summary>
        /// 행성이 정지해야 하는 위치.
        /// </summary>
        public List<Vector2> PlanetPositions { get; private set; }

        /// <summary>
        /// 미드스핀 타일를 구성하는 각도를 제외한 각도들.
        /// </summary>
        public List<float> FilteredAngles { get; private set; }

        public int Current;

        private float[] angleData;

        public ObjectContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<BeatmapInfo> beatmap)
        {
            angleData = convertAngles(beatmap.Value);
            FilteredAngles = filterMidspinAngle(angleData);
            createTiles(angleData);
        }

        public void MoveCamera()
        {
            if (Current + 1 < PlanetPositions.Count)
            {
                Current++;
                this.MoveTo(-PlanetPositions[Current], 250, Easing.OutSine);
            }
        }

        private void createTiles(float[] angleData)
        {
            if (angleData.Length == 0)
                return;

            // 시작위치.
            PlanetPositions = new List<Vector2> { Vector2.Zero };

            for (int i = 0; i < angleData.Length; i++)
            {
                // 각도 값이 999일 때 스킵.
                if (angleData[i] == 999 || i > angleData.Length - 1)
                    continue;

                if (i < angleData.Length - 1)
                {
                    if (angleData[i + 1] == 999)
                    {
                        Add(new MidspinTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });

                        continue;
                    }
                }

                var nextX = (float)Math.Cos(MathHelper.DegreesToRadians(angleData[i])) * 100;
                var nextY = (float)Math.Sin(MathHelper.DegreesToRadians(angleData[i])) * 100;

                if (i >= 1)
                {
                    if (Math.Abs(angleData[i] - angleData[i - 1]) == 180)
                    {
                        Add(new CircularTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });

                        tilePosition += new Vector2(nextX, nextY);
                        PlanetPositions.Add(tilePosition);

                        continue;
                    }
                }

                Add(new BasicTile(angleData[i])
                {
                    Position = tilePosition,
                    Rotation = angleData[i],
                });

                tilePosition += new Vector2(nextX, nextY);
                PlanetPositions.Add(tilePosition);
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

            for (int i = 0; i < info.Angles.Length; i++)
            {
                if (info.Angles[i] == 0 || info.Angles[i] == 999)
                {
                    newAngleData.Add(info.Angles[i]);
                    continue;
                }

                newAngleData.Add(360 - info.Angles[i]);
            }

            return newAngleData.ToArray();
        }

        private List<float> filterMidspinAngle(float[] originAngleData)
        {
            Stack<float> filteredAngleData = new Stack<float>();

            foreach (var angle in originAngleData)
            {
                filteredAngleData.Push(angle);

                if (angle == 999)
                {
                    filteredAngleData.Pop();
                    filteredAngleData.Pop();
                }
            }

            var list = filteredAngleData.ToList();
            list.Reverse();

            return list;
        }
    }
}
