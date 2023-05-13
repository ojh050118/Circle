using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Rulesets.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace Circle.Game.Screens.Play.HUD
{
    public class HUDOverlay : Container
    {
        private readonly Beatmap beatmap;

        private SpriteText complete;
        private GameplayProgress progress;
        private IReadOnlyList<double> hitTimes;
        private int floor;
        private float countdownInterval;

        public HUDOverlay(Beatmap beatmap)
        {
            this.beatmap = beatmap;
            countdownInterval = 60000 / beatmap.Settings.Bpm;
        }

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            var countdown = countdownInterval * beatmap.Settings.CountdownTicks;
            hitTimes = CalculationExtensions.GetTileStartTime(beatmap, beatmap.Settings.Offset, countdown);

            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Margin = new MarginPadding { Top = 30 },
                    Text = $"{beatmap.Settings.Artist} - {beatmap.Settings.Song}",
                    Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 34),
                    Shadow = true,
                    ShadowColour = colours.TransparentBlack
                },
                complete = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 64),
                    Shadow = true,
                    ShadowColour = colours.TransparentBlack
                },
                progress = new GameplayProgress(hitTimes.Count - 1)
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                },
            };
        }

        public void Start()
        {
            countdownInterval = 60000 / beatmap.TilesInfo[floor].Bpm;
            var tick = beatmap.Settings.CountdownTicks;

            Countdown(countdownInterval * tick);
        }

        public void Countdown(float startUntilTime)
        {
            var tick = beatmap.Settings.CountdownTicks;
            countdownInterval = startUntilTime / tick;

            for (int i = 0; i < tick; i++)
            {
                using (complete.BeginDelayedSequence(countdownInterval * i, false))
                {
                    var text = i + 1 == tick ? "Go!" : (tick - i - 1).ToString();
                    complete.TransformTo("Text", (LocalisableString)text);
                    complete.ScaleTo(1.3f).Delay(100).ScaleTo(1, countdownInterval, Easing.Out);
                    complete.FadeTo(1).Delay(100).FadeOut(countdownInterval, Easing.Out);
                }
            }
        }

        public void UpdateProgress(int floor)
        {
            this.floor = floor;
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
