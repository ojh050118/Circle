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
            bluePlanet.Rotation = -180;
            planetState = new Bindable<PlanetState>(PlanetState.Ice);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            planetState.ValueChanged += _ => movePlanet();
        }

        public void StartPlaying()
        {
            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.BPM, Easing.Out);
            bluePlanet.RotateTo(tiles.Children[tiles.Current].Angle, calculateDuration(tiles.Children[tiles.Current].Angle))
                      .Then()
                      .Schedule(() =>
                      {
                          bluePlanet.Expansion = 0;
                          planetState.Value = PlanetState.Fire;
                          tiles.Current++;
                      });
        }

        private void movePlanet()
        {
            if (tiles.Current >= tiles.Children.Count)
                return;

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.RotateTo(redPlanet.Rotation + tiles.Children[tiles.Current].Angle, calculateDuration(redPlanet.Rotation + tiles.Children[tiles.Current].Angle))
                         .Then()
                         .Schedule(() =>
                         {
                             redPlanet.Expansion = 0;
                             redPlanet.Position = tiles.Children[tiles.Current].Position;
                             tiles.Current++;
                             planetState.Value = PlanetState.Ice;
                         });
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.RotateTo(bluePlanet.Rotation + tiles.Children[tiles.Current].Angle, calculateDuration(bluePlanet.Rotation + tiles.Children[tiles.Current].Angle))
                          .Then()
                          .Schedule(() =>
                          {
                              bluePlanet.Expansion = 0;
                              bluePlanet.Position = tiles.Children[tiles.Current].Position;
                              tiles.Current++;
                              planetState.Value = PlanetState.Fire;
                          });
            }

            this.MoveTo(-tiles.CameraPositions[tiles.Current], 250, Easing.OutSine);
        }

        private float calculateDuration(float newRotation)
        {
            return 60000 / (float)beatmap.Value.Settings.BPM * Math.Abs(planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation - newRotation) / 180;                                        
        }
    }
}
