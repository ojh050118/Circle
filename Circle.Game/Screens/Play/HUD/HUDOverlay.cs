#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace Circle.Game.Screens.Play.HUD
{
    public partial class HUDOverlay : Container
    {
        private readonly Beatmap beatmap;

        private GlowingSpriteText complete;
        private IEnumerable<Tile> tiles;
        private GameplayProgress progress;

        public HUDOverlay(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            tiles = beatmap.Tiles;

            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new GlowingSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Margin = new MarginPadding { Top = 30 },
                    Text = string.IsNullOrEmpty(beatmap.Metadata.Artist) && string.IsNullOrEmpty(beatmap.Metadata.Song) ? string.Empty : $"{beatmap.Metadata.Artist} - {beatmap.Metadata.Song}",
                    Font = CircleFont.Default.With(weight: FontWeight.Bold, size: 34),
                    GlowColour = colours.TransparentBlack
                },
                complete = new GlowingSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    Font = CircleFont.Default.With(weight: FontWeight.Bold, size: 64),
                    GlowColour = colours.TransparentBlack
                },
                progress = new GameplayProgress(tiles.Count() - 1)
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                },
            };
        }

        public void Start()
        {
            float countdownInterval = 60000 / beatmap.Metadata.Bpm;
            int tick = beatmap.Metadata.CountdownTicks;

            Countdown(countdownInterval * tick);
        }

        public void Countdown(double startUntilTime)
        {
            int tick = beatmap.Metadata.CountdownTicks;
            startUntilTime /= tick;

            for (int i = 0; i < tick; i++)
            {
                using (complete.BeginDelayedSequence(startUntilTime * i, false))
                {
                    string text = i + 1 == tick ? "Go!" : (tick - i - 1).ToString();
                    complete.TransformTo("Text", (LocalisableString)text);
                    complete.ScaleTo(1.3f).Delay(100).ScaleTo(1, startUntilTime, Easing.Out);
                    complete.FadeTo(1).Delay(100).FadeOut(startUntilTime, Easing.Out);
                }
            }
        }

        public void UpdateProgress(int floor)
        {
            progress.ProgressTo(floor);
        }

        public void Complete()
        {
            complete.Text = "Congratulations!";
            complete.Alpha = 1;
            progress.Increase();
        }
    }
}
