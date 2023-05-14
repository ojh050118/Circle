#nullable disable

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
    public partial class CameraContainer : Container
    {
        public new Container Content;
        private Container offsetContainer, positionContainer, scalingContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = scalingContainer = new Container
            {
                Name = "Rotate/Scaling Container",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = offsetContainer = new Container
                {
                    Name = "Offset Container",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Child = positionContainer = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Name = "Positioning Container",
                        AutoSizeAxes = Axes.Both,
                        Child = Content
                    }
                }
            };
        }

        public void InitializeSettings(Settings settings)
        {
            scalingContainer.Rotation = settings.Rotation;
            var zoom = Precision.AlmostEquals(settings.Zoom, 0) ? 100 : settings.Zoom;
            scalingContainer.Scale = new Vector2(1 / (zoom / 100));
            offsetContainer.Position = settings.Position.ToVector2() * (Tile.WIDTH - Planet.PLANET_SIZE);
        }

        public void AddCameraTransforms(Beatmap beatmap, IReadOnlyList<double> tileStartTime)
        {
            #region Initialize Camera Setting

            float bpm = beatmap.Settings.Bpm;

            // 카메라 이동을 위한 전역 설정입니다.
            Relativity cameraRelativity = beatmap.Settings.RelativeTo;
            Vector2 cameraOffset = beatmap.Settings.Position.ToVector2() * (Tile.WIDTH - Planet.PLANET_SIZE);
            Vector2 cameraPosition = cameraOffset;
            float cameraRotation = beatmap.Settings.Rotation;

            var tilesInfo = beatmap.TilesInfo;
            var cameraTransforms = new List<CameraTransform>();

            // 카메라 기준좌표에 마지막위치로 설정하면 마지막에 설정한 기준좌표를 알 수 없습니다.
            if (beatmap.Settings.RelativeTo == Relativity.LastPosition)
                cameraRelativity = Relativity.Player;

            #endregion

            #region Process Camera Transform

            for (int floor = 0; floor < tilesInfo.Length; floor++)
            {
                var prevAngle = tilesInfo[floor].Angle;
                var fixedRotation = tilesInfo.ComputeRotation(floor, prevAngle);
                var position = -tilesInfo[floor].Position;
                bpm = tilesInfo.GetNewBpm(bpm, floor);

                foreach (var action in tilesInfo[floor].Action)
                {
                    void setCameraProperty(Actions cameraAction)
                    {
                        var offset = cameraAction.Position.ToVector2() * (Tile.WIDTH - Planet.PLANET_SIZE);

                        if (cameraAction.RelativeTo != Relativity.LastPosition)
                        {
                            cameraOffset = offset;

                            if (cameraAction.Rotation.HasValue)
                                cameraRotation = cameraAction.Rotation.Value;
                        }

                        switch (cameraAction.RelativeTo)
                        {
                            case Relativity.Global:
                                cameraPosition = Vector2.Zero;
                                break;

                            case Relativity.Player:
                            case Relativity.Tile:
                                cameraPosition = position;
                                break;

                            case Relativity.LastPosition:
                                if (cameraRelativity == Relativity.Player)
                                    cameraPosition = position;

                                if (cameraAction.Rotation.HasValue)
                                    cameraRotation += cameraAction.Rotation.Value;

                                cameraOffset += offset;
                                break;

                            case null:
                                break;
                        }

                        if (cameraAction.RelativeTo != null && cameraAction.RelativeTo != Relativity.LastPosition)
                            cameraRelativity = cameraAction.RelativeTo.Value;
                    }

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            setCameraProperty(action);
                            var angleOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);
                            Vector2? specificPosition0 = cameraRelativity == Relativity.Player ? null : (Vector2?)cameraPosition;

                            // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                            cameraTransforms.Add(new CameraTransform
                            {
                                StartTime = tileStartTime[floor] + angleOffset,
                                Duration = tilesInfo.GetRelativeDuration(fixedRotation, floor, bpm) * action.Duration,
                                Position = specificPosition0,
                                Offset = cameraOffset,
                                Rotation = cameraRotation,
                                Zoom = action.Zoom,
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
                                    setCameraProperty(cameraEvent);
                                    var angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + cameraEvent.AngleOffset, bpm);
                                    Vector2? specificPosition = cameraRelativity == Relativity.Player ? null : (Vector2?)cameraPosition;

                                    if (cameraEvent.EventTag == action.Tag)
                                    {
                                        // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                                        cameraTransforms.Add(new CameraTransform
                                        {
                                            StartTime = startTime + angleTimeOffset,
                                            Duration = tilesInfo.GetRelativeDuration(fixedRotation, floor, bpm) * cameraEvent.Duration,
                                            Position = specificPosition,
                                            Offset = cameraOffset,
                                            Rotation = cameraRotation,
                                            Zoom = cameraEvent.Zoom,
                                            Easing = action.Ease
                                        });
                                    }
                                }
                            }

                            break;
                    }
                }

                if (cameraRelativity == Relativity.Player)
                {
                    cameraTransforms.Add(new CameraTransform
                    {
                        StartTime = tileStartTime[floor],
                        Duration = 400 + 60 / bpm * 500,
                        Position = position,
                        Offset = null,
                        Rotation = null,
                        Zoom = null,
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
                using (scalingContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    // 확대 값이 있으면 확대 트랜스폼을 수행합니다.
                    if (cameraTransform.Zoom.HasValue)
                    {
                        float cameraZoom = 1 / (cameraTransform.Zoom.Value / 100);

                        if (float.IsInfinity(cameraZoom))
                            cameraZoom = 0;

                        scalingContainer.ScaleTo(cameraZoom, cameraTransform.Duration, cameraTransform.Easing);
                    }

                    // 회전 값이 있으면 회전 트랜스폼을 실행합니다.
                    if (cameraTransform.Rotation.HasValue)
                        scalingContainer.RotateTo(cameraTransform.Rotation.Value, cameraTransform.Duration, cameraTransform.Easing);
                }

                if (cameraTransform.Offset.HasValue)
                {
                    using (offsetContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                        offsetContainer.MoveTo(cameraTransform.Offset.Value, cameraTransform.Duration, cameraTransform.Easing);
                }

                if (cameraTransform.Position.HasValue)
                {
                    using (positionContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                        positionContainer.MoveTo(cameraTransform.Position.Value, cameraTransform.Duration, cameraTransform.Easing);
                }
            }

            #endregion
        }
    }
}
