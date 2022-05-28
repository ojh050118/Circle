using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Rulesets.Extensions
{
    public static class ElementTransformExtensions
    {
        public static void AddCameraTransforms(this Container cameraContainer, Settings settings, TileInfo[] tilesInfo, IList<CameraTransform> cameraTransforms)
        {
            var lastPosition = new Vector2(settings.Position[0], settings.Position[1]);
            var lastRotation = settings.Rotation;
            var lastRelativity = settings.RelativeTo;

            // 트랜스폼을 시작 시간순으로 추가해야 올바르게 추가됩니다.
            foreach (var cameraTransform in cameraTransforms.OrderBy(a => a.StartTime))
            {
                var action = cameraTransform.Action;
                var tileCameraPosition = tilesInfo[action.Floor].Position;

                if (cameraTransform.Position.HasValue)
                    tileCameraPosition += new Vector2(Tile.WIDTH * cameraTransform.Position.Value.X, -Tile.WIDTH * cameraTransform.Position.Value.Y);

                // 카메라 회전, 확대 효과를 적용합니다.
                using (cameraContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    if (cameraTransform.Zoom.HasValue)
                    {
                        float cameraZoom = 1 / ((float)cameraTransform.Zoom.Value / 100);

                        if (float.IsInfinity(cameraZoom))
                            cameraZoom = 0;

                        cameraContainer.ScaleTo(cameraZoom, cameraTransform.Duration, action.Ease);
                    }

                    if (cameraTransform.Rotation.HasValue)
                    {
                        if (cameraTransform.RelativeTo == Relativity.LastPosition)
                            lastRotation += cameraTransform.Rotation.Value;
                        else
                            lastRotation = cameraTransform.Rotation.Value;

                        cameraContainer.RotateTo(lastRotation, cameraTransform.Duration, action.Ease);
                    }
                }

                // 카메라의 위치를 바꿉니다.
                using (cameraContainer.Child.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    switch (cameraTransform.RelativeTo)
                    {
                        // 플레이어(행성)의 위치로 고정합니다.
                        // 카메라가 타일에 고정됩니다.
                        case Relativity.Player:
                        case Relativity.Tile:
                            lastRelativity = cameraTransform.RelativeTo.Value;
                            lastPosition = tileCameraPosition;
                            cameraContainer.Child.MoveTo(-tileCameraPosition, cameraTransform.Duration, action.Ease);
                            break;

                        // 첫 타일의 위치로 고정합니다.
                        case Relativity.Global:
                            lastRelativity = cameraTransform.RelativeTo.Value;
                            lastPosition = Vector2.Zero;

                            if (cameraTransform.Position.HasValue)
                                lastPosition = new Vector2(Tile.WIDTH * cameraTransform.Position.Value.X, -Tile.WIDTH * cameraTransform.Position.Value.Y);

                            cameraContainer.Child.MoveTo(-lastPosition, cameraTransform.Duration, action.Ease);
                            break;

                        // 마지막 카메라 위치로 고정합니다.
                        default:
                        case Relativity.LastPosition:
                            if (cameraTransform.Position.HasValue)
                                lastPosition = new Vector2(Tile.WIDTH * cameraTransform.Position.Value.X, -Tile.WIDTH * cameraTransform.Position.Value.Y);

                            cameraContainer.Child.MoveTo(-lastPosition, cameraTransform.Duration, action.Ease);
                            break;
                    }
                }
            }
        }

        public static IReadOnlyList<CameraTransform> GenerateCameraTransforms(Settings settings, IReadOnlyList<double> startTimes, TileInfo[] tilesInfo)
        {
            float bpm = settings.Bpm;
            var offset = startTimes;
            var cameraTransforms = new List<CameraTransform>();

            var lastRelativity = settings.RelativeTo;
            var lastPosition = new Vector2(settings.Position[0], settings.Position[1]);
            var lastRotation = settings.Rotation;
            var lastZoom = settings.Zoom;

            for (int floor = 0; floor < tilesInfo.Length; floor++)
            {
                bpm = CalculationExtensions.GetNewBpm(tilesInfo, bpm, floor);
                var prevAngle = tilesInfo[floor].Angle;
                var fixedRotation = CalculationExtensions.ComputeRotation(tilesInfo, floor, prevAngle);

                //기준 좌표가 플레이어일 때 자동으로 처리해야함.
                if (true)
                {
                    cameraTransforms.Add(new CameraTransform
                    {
                        Action = new Actions
                        {
                            Floor = floor,
                            BeatsPerMinute = bpm,
                            Ease = Easing.OutSine,
                            Rotation = lastRotation
                        },
                        StartTime = offset[floor],
                        Duration = 400 + 60 / bpm * 500,
                        RelativeTo = Relativity.Player,
                        Rotation = lastRotation,
                        Position = lastPosition,
                        Zoom = lastZoom
                    });
                }

                // 한 타일의 액션에 접근
                for (int actionIndex = 0; actionIndex < tilesInfo[floor].Action.Length; actionIndex++)
                {
                    var action = tilesInfo[floor].Action[actionIndex];

                    if (action.EventType == EventType.MoveCamera)
                    {
                        lastRelativity = action.RelativeTo ?? lastRelativity;
                        lastPosition = action.Position != null ? new Vector2(action.Position[0], action.Position[1]) : lastPosition;
                        lastZoom = action.Zoom ?? lastZoom;

                        if (action.Rotation.HasValue)
                        {
                            if (action.RelativeTo == null)
                                lastRotation = action.Rotation.Value;
                            else if (action.RelativeTo == Relativity.LastPosition)
                                lastRotation += action.Rotation.Value;
                        }
                    }

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            var angleOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);
                            cameraTransforms.Add(new CameraTransform
                            {
                                Action = action,
                                StartTime = offset[floor] + angleOffset,
                                Duration = CalculationExtensions.GetRelativeDuration(fixedRotation, tilesInfo[floor].TileType == TileType.Midspin ? floor + 1 : floor, bpm) * action.Duration,
                                Position = action.Position != null ? new Vector2(action.Position[0], action.Position[1]) : default,
                                Rotation = action.Rotation,
                                RelativeTo = action.RelativeTo,
                                Zoom = action.Zoom
                            });
                            break;

                        // 이벤트 반복은 원래 이벤트를 포함해 반복하지 않습니다. (ex: 반복횟수가 1이면 이벤트는 총 2번 실행됨)
                        case EventType.RepeatEvents:
                            var cameraEvents = Array.FindAll(tilesInfo[action.Floor].Action, a => a.EventType == EventType.MoveCamera);
                            var intervalBeat = 60000 / bpm * action.Interval;

                            var relativity = action.RelativeTo ?? lastRelativity;
                            var position = action.Position != null ? new Vector2(action.Position[0], action.Position[1]) : lastPosition;
                            var rotation = 0f;

                            if (action.Rotation.HasValue)
                            {
                                if (action.RelativeTo == null)
                                    rotation = action.Rotation.Value;
                                else if (action.RelativeTo == Relativity.LastPosition)
                                    rotation += action.Rotation.Value;
                            }

                            for (int i = 1; i <= action.Repetitions; i++)
                            {
                                var startTime = offset[floor] + intervalBeat * i;

                                foreach (var cameraEvent in cameraEvents)
                                {
                                    var angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + cameraEvent.AngleOffset, bpm);

                                    if (cameraEvent.EventTag == action.Tag)
                                    {
                                        cameraTransforms.Add(new CameraTransform
                                        {
                                            Action = cameraEvent,
                                            StartTime = startTime + angleTimeOffset,
                                            Duration = CalculationExtensions.GetRelativeDuration(fixedRotation, tilesInfo[floor].TileType == TileType.Midspin ? floor + 1 : floor, bpm) * cameraEvent.Duration,
                                            Position = cameraEvent.Position != null ? new Vector2(cameraEvent.Position[0], cameraEvent.Position[1]) : default,
                                            Rotation = cameraEvent.Rotation,
                                            RelativeTo = cameraEvent.RelativeTo,
                                            Zoom = cameraEvent.Zoom
                                        });
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            return cameraTransforms;
        }
    }
}
