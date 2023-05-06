using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace Circle.Game.Screens.Play
{
    public class CameraContainer : Container
    {
        private Container positionContainer, scalingContainer;

        public new Container Content;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = scalingContainer = new Container
            {
                Name = "Scaling Container",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = positionContainer = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Name = "Positioning Container",
                    AutoSizeAxes = Axes.Both,
                    Child = Content
                }
            };
        }

        public void InitializeSettings(Settings settings)
        {
            scalingContainer.Rotation = settings.Rotation;
            var zoom = Precision.AlmostEquals(settings.Zoom, 0) ? 100 : settings.Zoom;
            scalingContainer.Scale = new Vector2(1 / (zoom / 100));

            if (settings.Position != null)
                positionContainer.Position = new Vector2(settings.Position[0], settings.Position[1]) * Tile.WIDTH;
        }

        public void AddCameraTransforms(Beatmap beatmap, IReadOnlyList<double> tileStartTime)
        {
            float bpm = beatmap.Settings.Bpm;

            Relativity lastRelativity = beatmap.Settings.RelativeTo;
            Vector2 lastOffset = Vector2.Zero;
            Vector2 relativePosition;
            float relativeRotation;

            var tilesInfo = CalculationExtensions.GetTilesInfo(beatmap);
            var cameraTransforms = new List<CameraTransform>();

            if (beatmap.Settings.Position != null)
            {
                lastOffset = new Vector2(beatmap.Settings.Position[0], beatmap.Settings.Position[1]) * (Tile.WIDTH - Planet.PLANET_SIZE);
                relativePosition = lastOffset;
                relativeRotation = beatmap.Settings.Rotation;
            }

            #region Process Camera Transform

            for (int floor = 0; floor < tilesInfo.Count; floor++)
            {
                bpm = tilesInfo.GetNewBpm(bpm, floor);
                var prevAngle = tilesInfo[floor].Angle;
                var fixedRotation = tilesInfo.ComputeRotation(floor, prevAngle);

                foreach (var action in tilesInfo[floor].Action)
                {
                    var position = -tilesInfo[floor].Position;

                    if (action.Position != null)
                    {
                        var offset = -new Vector2(action.Position[0], -action.Position[1]) * (Tile.WIDTH - Planet.PLANET_SIZE);
                        position += offset;
                        lastOffset = offset;
                    }

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            var angleOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);

                            if (action.RelativeTo.HasValue)
                                lastRelativity = action.RelativeTo.Value;

                            // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                            cameraTransforms.Add(new CameraTransform
                            {
                                Action = action,
                                StartTime = tileStartTime[floor] + angleOffset,
                                Duration = tilesInfo.GetRelativeDuration(fixedRotation, floor, bpm) * action.Duration,
                                Position = position,
                                Easing = action.Ease
                            });
                            break;

                        // 이벤트 반복엔 타일에 종속되지 않는 이벤트(카메라 트랜스폼)가 실행됩니다.
                        case EventType.RepeatEvents:
                            var cameraEvents = Array.FindAll(tilesInfo[action.Floor].Action, a => a.EventType == EventType.MoveCamera);
                            var intervalBeat = 60000 / bpm * action.Interval;

                            // 이벤트를 일정한 주기로 반복 횟수만큼 추가합니다.
                            for (int i = 1; i <= action.Repetitions; i++)
                            {
                                var startTime = tileStartTime[floor] + intervalBeat * i;

                                foreach (var cameraEvent in cameraEvents)
                                {
                                    var angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + cameraEvent.AngleOffset, bpm);

                                    if (cameraEvent.EventTag == action.Tag)
                                    {
                                        // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                                        cameraTransforms.Add(new CameraTransform
                                        {
                                            Action = cameraEvent,
                                            StartTime = startTime + angleTimeOffset,
                                            Duration = tilesInfo.GetRelativeDuration(fixedRotation, floor, bpm) * cameraEvent.Duration,
                                            Position = position,
                                            Easing = action.Ease
                                        });
                                    }
                                }
                            }

                            break;
                    }
                }

                if (lastRelativity == Relativity.Player)
                {
                    cameraTransforms.Add(new CameraTransform
                    {
                        StartTime = tileStartTime[floor],
                        Duration = 400 + 60 / bpm * 500,
                        Position = -tilesInfo[floor].Position + lastOffset,
                        Easing = Easing.OutSine
                    });
                }
            }

            #endregion

            #region Register Camera Transform

            // 트랜스폼을 시작 시간순으로 추가해야 올바르게 추가됩니다.
            // 여기서 특정 시간에 발생하는 이벤트의 액션을 수행합니다.
            foreach (var cameraTransform in cameraTransforms.OrderBy(a => a.StartTime))
            {
                var action = cameraTransform.Action;

                using (scalingContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    // 확대 값이 있으면 확대 트랜스폼을 수행합니다.
                    if (action.Zoom.HasValue)
                    {
                        float cameraZoom = 1 / ((float)action.Zoom.Value / 100);

                        if (float.IsInfinity(cameraZoom))
                            cameraZoom = 0;

                        scalingContainer.ScaleTo(cameraZoom, cameraTransform.Duration, action.Ease);
                    }

                    // 회전 값이 있으면 회전 트랜스폼을 실행합니다.
                    if (action.Rotation.HasValue)
                        scalingContainer.RotateTo(action.Rotation.Value, cameraTransform.Duration, action.Ease);
                }

                using (positionContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    if (cameraTransform.Position.HasValue)
                        positionContainer.MoveTo(cameraTransform.Position.Value, cameraTransform.Duration, cameraTransform.Easing);
                }
            }

            #endregion
        }
    }
}
