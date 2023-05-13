using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        /// <summary>
        /// 게임 시작 전 카운트다운 지속시간.
        /// </summary>
        private readonly double countdownDuration;

        private readonly Beatmap currentBeatmap;

        /// <summary>
        /// 게임이 시작되는 시간.
        /// </summary>
        private readonly double gameplayStartTime;

        private CameraContainer cameraContainer;
        private PlanetContainer planetContainer;
        private ObjectContainer tileContainer;

        private TileInfo[] tilesInfo;

        public Playfield(Beatmap beatmap, double gameplayStartTime, double countdownDuration)
        {
            currentBeatmap = beatmap;
            this.gameplayStartTime = gameplayStartTime;
            this.countdownDuration = countdownDuration;
        }

        public Action OnComplete { get; set; }

        private IReadOnlyList<double> startTimes => CalculationExtensions.GetTileStartTime(currentBeatmap, gameplayStartTime, countdownDuration);

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager config)
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = cameraContainer = new CameraContainer
            {
                Content = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        tileContainer = new ObjectContainer(currentBeatmap),
                        planetContainer = new PlanetContainer()
                    }
                }
            };

            tilesInfo = tileContainer.GetTilesInfo().ToArray();

            planetContainer.RedPlanet.Expansion = planetContainer.BluePlanet.Expansion = 0;
            planetContainer.BluePlanet.Rotation = tilesInfo[0].Angle - CalculationExtensions.GetTimebaseRotation(gameplayStartTime, startTimes[0], currentBeatmap.Settings.Bpm);
            cameraContainer.InitializeSettings(currentBeatmap.Settings);
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms(gameplayStartTime, countdownDuration);
            planetContainer.AddPlanetTransform(currentBeatmap, gameplayStartTime);
            cameraContainer.AddCameraTransforms(currentBeatmap, startTimes);

            base.LoadComplete();
        }
    }
}
