using System;
using System.Collections.Generic;
using Circle.Game.Beatmap;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace Circle.Game.Screens.Play
{
    public class ObjectContainer : Container<Tile>
    {
        /// <summary>
        /// 다음 타일의 위치. (초기 값은 Vector2.Zero)
        /// </summary>
        private Vector2 tilePosition = Vector2.Zero;

        /// <summary>
        /// 행성이 정지하는 위치 입니다.
        /// 지금은 미드스핀 타일 위치까지 포함하고 있습니다.
        /// 나중에 제외해야 합니다.
        /// </summary>
        private List<Vector2> planetPositions;

        private int current;

        private BeatmapInfo beatmap;

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
            this.beatmap = convertAngles(beatmap.Value);
            createTiles(this.beatmap);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (current + 1 < planetPositions.Count)
            {
                current++;
                this.MoveTo(-planetPositions[current], 250, Easing.OutSine);
            }

            return base.OnKeyDown(e);
        }

        private void createTiles(BeatmapInfo info)
        {
            if (info.Angles.Count == 0)
                return;

            // 행성이 처음에 있어야 하는 위치는 (0, 0) 입니다.
            planetPositions = new List<Vector2> { Vector2.Zero };

            for (int i = 0; i < info.Angles.Count; i++)
            {
                if (info.Angles[i] == 999 || i + 1 > info.Angles.Count)
                    continue;

                if (i + 1 < info.Angles.Count)
                {
                    if (info.Angles[i + 1] == 999)
                    {
                        Add(new MidspinTile(info.Angles[i])
                        {
                            Position = tilePosition,
                            Rotation = info.Angles[i],
                        });
                        // 실제로 행성이 이 위치에 있으면 안됩니다.
                        planetPositions.Add(tilePosition);

                        continue;
                    }
                }

                if (i - 1 >= 0)
                {
                    if (Math.Abs(info.Angles[i] - info.Angles[i - 1]) == 180)
                    {
                        Add(new CircularTile(info.Angles[i])
                        {
                            Position = tilePosition,
                            Rotation = info.Angles[i],
                        });

                        var x = (float)Math.Cos(MathHelper.DegreesToRadians(info.Angles[i])) * 100;
                        var y = (float)Math.Sin(MathHelper.DegreesToRadians(info.Angles[i])) * 100;
                        tilePosition += new Vector2(x, y);
                        planetPositions.Add(tilePosition);

                        continue;
                    }
                }

                Add(new BasicTile(info.Angles[i])
                {
                    Position = tilePosition,
                    Rotation = info.Angles[i],
                });

                var nextX = (float)Math.Cos(MathHelper.DegreesToRadians(info.Angles[i])) * 100;
                var nextY = (float)Math.Sin(MathHelper.DegreesToRadians(info.Angles[i])) * 100;
                tilePosition += new Vector2(nextX, nextY);
                planetPositions.Add(tilePosition);
            }
        }

        private BeatmapInfo convertAngles(BeatmapInfo info)
        {
            for (int i = 0; i < info.Angles.Count; i++)
            {
                if (info.Angles[i] == 0 || info.Angles[i] == 999)
                    continue;

                info.Angles[i] = 360 - info.Angles[i];
            }

            return info;
        }
    }
}
