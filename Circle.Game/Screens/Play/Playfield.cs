#nullable disable

using System;
using Circle.Game.Beatmaps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play
{
    public partial class Playfield : Container
    {
        private readonly Beatmap currentBeatmap;

        private CameraContainer cameraContainer;
        private PlanetContainer planetContainer;
        private ObjectContainer tileContainer;

        public Playfield(Beatmap beatmap)
        {
            currentBeatmap = beatmap;
        }

        public Action OnComplete { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = cameraContainer = new CameraContainer(currentBeatmap)
            {
                Content = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        tileContainer = new ObjectContainer(currentBeatmap),
                        planetContainer = new PlanetContainer(currentBeatmap)
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms();
            planetContainer.AddPlanetTransform();
            cameraContainer.AddCameraTransforms();

            base.LoadComplete();
        }
    }
}
