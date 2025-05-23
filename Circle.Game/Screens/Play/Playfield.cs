#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Rulesets.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;

namespace Circle.Game.Screens.Play
{
    public partial class Playfield : PostProcessingContainer
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
        private void load(BeatmapManager beatmapManager)
        {
            Container backgrounds;

            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                backgrounds = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Background(TextureSource.External, beatmapInfo: currentBeatmap.BeatmapInfo),
                    }
                },
                cameraContainer = new CameraContainer(currentBeatmap)
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
                }
            };

            var stream = beatmapManager.GetWorkingBeatmap(currentBeatmap.BeatmapInfo).GetVideo();

            if (stream != null)
            {
                backgrounds.Add(new Video(stream, false)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    RelativeSizeAxes = Axes.Both,
                    Loop = false,
                    PlaybackPosition = currentBeatmap.Metadata.VidOffset - currentBeatmap.Metadata.Offset - 60000 / currentBeatmap.Metadata.Bpm * currentBeatmap.Metadata.CountdownTicks,
                });
            }
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
