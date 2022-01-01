using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using Circle.Game.Rulesets.Objects;
using osuTK.Graphics;
using osu.Framework.Bindables;
using Circle.Game.Beatmap;
using osu.Framework.Allocation;
using Circle.Game.Rulesets.Extensions;
using System;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        private readonly ObjectContainer tiles;
        private readonly Planet redPlanet;
        private readonly Planet bluePlanet;

        [Resolved]
        private Bindable<BeatmapInfo> beatmap { get; set; }

        /// <summary>
        /// 현재 회전하는 행성을 가리킵니다.
        /// </summary>
        private readonly Bindable<PlanetState> planetState;

        public Playfield()
        {
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                tiles = new ObjectContainer(),
                redPlanet = new Planet(Color4.Red),
                bluePlanet = new Planet(Color4.DeepSkyBlue),
            };

            redPlanet.Expansion = bluePlanet.Expansion = 0;
            planetState = new Bindable<PlanetState>(PlanetState.Ice);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bluePlanet.Rotation = tiles.FilteredAngles[tiles.Current];
            redPlanet.Rotation = tiles.FilteredAngles[tiles.Current];
            planetState.ValueChanged += _ => movePlanet();
        }

        public void StartPlaying()
        {
            tiles.Current++;
            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.BPM, Easing.Out);
            bluePlanet.RotateTo(tiles.FilteredAngles[tiles.Current], calculateDuration(tiles.FilteredAngles[tiles.Current]))
                      .Then()
                      .Schedule(() =>
                      {
                          bluePlanet.Expansion = 0;
                          bluePlanet.Rotation += 180;
                          tiles.Current++;
                          bluePlanet.Position = tiles.PlanetPositions[tiles.Current];
                          planetState.Value = PlanetState.Fire;
                      });
        }

        private void movePlanet()
        {
            if (tiles.Current >= tiles.Children.Count)
                return;

            float newRotation = planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation;

            if (tiles.Current < tiles.PlanetPositions.Count - 1)
            {
                if (tiles.FilteredAngles[tiles.Current] == tiles.FilteredAngles[tiles.Current + 1])
                    newRotation += 180;
                else
                    newRotation = tiles.FilteredAngles[tiles.Current];
            }


            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.Position = tiles.PlanetPositions[tiles.Current];
                redPlanet.RotateTo(newRotation, calculateDuration(newRotation))
                         .Then()
                         .Schedule(() =>
                         {
                             redPlanet.Expansion = 0;
                             tiles.Current++;
                             redPlanet.Rotation = getSafeAngle(redPlanet.Rotation);
                             redPlanet.Position = tiles.PlanetPositions[tiles.Current];
                             planetState.Value = PlanetState.Ice;
                         });
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.Position = tiles.PlanetPositions[tiles.Current];
                bluePlanet.RotateTo(newRotation, calculateDuration(newRotation))
                          .Then()
                          .Schedule(() =>
                          {
                              bluePlanet.Expansion = 0;
                              tiles.Current++;
                              bluePlanet.Rotation = getSafeAngle(bluePlanet.Rotation);
                              bluePlanet.Position = tiles.PlanetPositions[tiles.Current];
                              planetState.Value = PlanetState.Fire;
                          });
            }

            this.MoveTo(-tiles.CameraPositions[tiles.Current], 250, Easing.OutSine);
        }

        private float calculateDuration(float newRotation)
        {
            return 60000 / (float)beatmap.Value.Settings.BPM * Math.Abs(planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation - newRotation) / 180;                                        
        }

        private float getSafeAngle(float angle)
        {
            if (angle < 0)
            {
                while (angle < 0)
                    angle += 360;

                return angle;
            }

            if (angle <= 360)
                return angle;

            while (angle > 360)
                angle -= 360;

            return angle;
        }
    }
}
