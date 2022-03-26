using Circle.Game.Graphics;
using Circle.Game.Graphics.Containers;
using Circle.Game.Input;
using Circle.Game.Overlays.Volume;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays
{
    public class VolumeOverlay : VisibilityContainer
    {
        private VolumeMeter volumeMeterMaster;
        private VolumeMeter volumeMeterEffect;
        private VolumeMeter volumeMeterMusic;

        private SelectionCycleFillFlowContainer<VolumeMeter> volumeMeters;

        private ScheduledDelegate popOutDelegate;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, CircleColour colours)
        {
            Anchor = Anchor.TopRight;
            Origin = Anchor.TopRight;
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            Children = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientHorizontal(Color4.Transparent, colours.TransparentGray.Opacity(0.2f)),
                },
                volumeMeters = new SelectionCycleFillFlowContainer<VolumeMeter>
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Margin = new MarginPadding { Bottom = 20, Right = 20 },
                    Spacing = new Vector2(10),
                    Children = new[]
                    {
                        volumeMeterMusic = new VolumeMeter("Music", 100, Color4.DeepSkyBlue)
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight
                        },
                        volumeMeterEffect = new VolumeMeter("Effect", 100, Color4.DeepSkyBlue)
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight
                        },
                        volumeMeterMaster = new VolumeMeter("Master", 150, Color4.DeepSkyBlue)
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight
                        }
                    }
                }
            };

            volumeMeterMaster.Current.BindTo(audio.Volume);
            volumeMeterEffect.Current.BindTo(audio.VolumeSample);
            volumeMeterMusic.Current.BindTo(audio.VolumeTrack);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            foreach (var meter in volumeMeters)
                meter.Current.ValueChanged += _ => Show();
        }

        public bool Adjust(InputAction action, float amount = 1)
        {
            if (!IsLoaded)
                return false;

            switch (action)
            {
                case InputAction.IncreaseVolume:
                    if (State.Value == Visibility.Hidden)
                        Show();
                    else
                        volumeMeters.Selected?.Increase(amount * 5);

                    return true;

                case InputAction.DecreaseVolume:
                    if (State.Value == Visibility.Hidden)
                        Show();
                    else
                        volumeMeters.Selected?.Decrease(amount * 5);

                    return true;

                case InputAction.NextVolumeMeter:
                    if (State.Value == Visibility.Visible)
                        volumeMeters.SelectNext();

                    Show();
                    return true;

                case InputAction.PreviousVolumeMeter:
                    if (State.Value == Visibility.Visible)
                        volumeMeters.SelectPrevious();

                    Show();
                    return true;
            }

            return false;
        }

        public override void Show()
        {
            if (State.Value == Visibility.Hidden)
                volumeMeters.Select(volumeMeterMaster);

            if (State.Value == Visibility.Visible)
                schedulePopOut();

            base.Show();
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            schedulePopOut();

            return base.OnMouseMove(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            schedulePopOut();

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            schedulePopOut();

            base.OnHoverLost(e);
        }

        protected override void PopIn()
        {
            ClearTransforms();
            this.FadeIn(100);

            schedulePopOut();
        }

        protected override void PopOut()
        {
            this.FadeOut(250);
        }

        private void schedulePopOut()
        {
            popOutDelegate?.Cancel();
            this.Delay(1250).Schedule(() =>
            {
                if (!IsHovered)
                    Hide();
            }, out popOutDelegate);
        }
    }
}
