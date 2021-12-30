using System;
using System.Collections.Generic;
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
        /// 행성이 정지하는 위치 입니다.
        /// 지금은 미드스핀 타일 위치까지 포함하고 있습니다.
        /// 나중에 제외해야 합니다.
        /// </summary>
        public List<Vector2> PlanetPositions;

        public List<Vector2> CameraPositions;

        public int Current;

        private float[] angleData;

        // Todo: 행성이 있어야 할 위치(tilePositions) 수정과 각 타일의 각도를 담는 리스트가 있어야 함.
        // Todo: 새로운 방법 제안: 행성이 회전할 때 현재 회전정도에서 타일의 각도를 더해서 회전을 시키는 것.
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

            // 행성이 처음에 있어야 하는 위치는 (0, 0) 입니다.
            PlanetPositions = CameraPositions = new List<Vector2> { Vector2.Zero };

            for (int i = 0; i < angleData.Length; i++)
            {
                // 각도 값이 999일 때 스킵.
                if (angleData[i] == 999 || i + 1 > angleData.Length)
                    continue;

                if (i + 1 < angleData.Length)
                {
                    if (angleData[i + 1] == 999)
                    {
                        Add(new MidspinTile(angleData[i])
                        {
                            Position = tilePosition,
                            Rotation = angleData[i],
                        });
                        CameraPositions.Add(tilePosition);

                        continue;
                    }
                }

                var nextX = (float)Math.Cos(MathHelper.DegreesToRadians(angleData[i])) * 100;
                var nextY = (float)Math.Sin(MathHelper.DegreesToRadians(angleData[i])) * 100;
                if (i - 1 >= 0)
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
                        CameraPositions.Add(tilePosition);

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
                CameraPositions.Add(tilePosition);
            }
        }

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
    }
}
