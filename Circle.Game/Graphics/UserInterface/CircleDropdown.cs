using System.Linq;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleDropdown<T> : Dropdown<T>
    {
        private const float corner_radius = 5;

        private CircleDropdownMenu menu = null!;

        public float? MaxWidth
        {
            get => menu.MaxWidth;
            set
            {
                if (value != null)
                    menu.MaxWidth = value.Value;
            }
        }

        public float? MaxHeight
        {
            get => menu.MaxHeight;
            set
            {
                if (value != null)
                    menu.MaxHeight = value.Value;
            }
        }

        protected override DropdownHeader CreateHeader() => new CircleDropdownHeader();

        protected override DropdownMenu CreateMenu() => menu = new CircleDropdownMenu();

        #region CircleDropdownMenu

        protected partial class CircleDropdownMenu : DropdownMenu
        {
            private Color4 hoverColour;
            private Sample? sampleClose;

            private Sample? sampleOpen;

            private Color4 selectionColour;

            // todo: this shouldn't be required after https://github.com/ppy/osu-framework/issues/4519 is fixed.
            private bool wasOpened;

            // todo: this uses the same styling as CircleMenu. hopefully we can just use CircleMenu in the future with some refactoring
            public CircleDropdownMenu()
            {
                CornerRadius = corner_radius;

                MaskingContainer.CornerRadius = corner_radius;
                Alpha = 0;

                // todo: this uses the same styling as CircleMenu. hopefully we can just use CircleMenu in the future with some refactoring
                ItemsContainer.Padding = new MarginPadding(5);
            }

            public override bool HandleNonPositionalInput => State == MenuState.Open;

            public Color4 HoverColour
            {
                get => hoverColour;
                set
                {
                    hoverColour = value;
                    foreach (var c in Children.OfType<DrawableCircleDropdownMenuItem>())
                        c.BackgroundColourHover = value;
                }
            }

            public Color4 SelectionColour
            {
                get => selectionColour;
                set
                {
                    selectionColour = value;
                    foreach (var c in Children.OfType<DrawableCircleDropdownMenuItem>())
                        c.BackgroundColourSelected = value;
                }
            }

            [BackgroundDependencyLoader]
            private void load(AudioManager audio, CircleColour colours)
            {
                BackgroundColour = colours.TransparentBlack;
                HoverColour = Color4.DeepSkyBlue.Opacity(0.6f);
                SelectionColour = Color4.DeepSkyBlue.Opacity(0.8f);

                sampleOpen = audio.Samples.Get(@"dropdown-open");
                sampleClose = audio.Samples.Get(@"dropdown-close");
            }

            // todo: this uses the same styling as CircleMenu. hopefully we can just use CircleMenu in the future with some refactoring
            protected override void AnimateOpen()
            {
                wasOpened = true;
                this.FadeIn(300, Easing.OutQuint);
                sampleOpen?.Play();
            }

            protected override void AnimateClose()
            {
                if (wasOpened)
                {
                    this.FadeOut(300, Easing.OutQuint);
                    sampleClose?.Play();
                }
            }

            // todo: this uses the same styling as CircleMenu. hopefully we can just use CircleMenu in the future with some refactoring
            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
                }
            }

            protected override Menu CreateSubMenu() => new CircleMenu(Direction.Vertical);

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawableCircleDropdownMenuItem(item)
            {
                BackgroundColourHover = HoverColour,
                BackgroundColourSelected = SelectionColour
            };

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new CircleScrollContainer(direction);

            #region DrawableCircleDropdownMenuItem

            public partial class DrawableCircleDropdownMenuItem : DrawableDropdownMenuItem
            {
                public DrawableCircleDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    Foreground.Padding = new MarginPadding(2);

                    Masking = true;
                    CornerRadius = corner_radius;
                }

                // IsHovered is used
                public override bool HandlePositionalInput => true;

                public new Color4 BackgroundColourHover
                {
                    get => base.BackgroundColourHover;
                    set
                    {
                        base.BackgroundColourHover = value;
                        updateColours();
                    }
                }

                public new Color4 BackgroundColourSelected
                {
                    get => base.BackgroundColourSelected;
                    set
                    {
                        base.BackgroundColourSelected = value;
                        updateColours();
                    }
                }

                private void updateColours()
                {
                    BackgroundColour = BackgroundColourHover.Opacity(0);

                    UpdateBackgroundColour();
                    UpdateForegroundColour();
                }

                protected override void UpdateBackgroundColour()
                {
                    if (!IsPreSelected && !IsSelected)
                    {
                        Background.FadeOut(600, Easing.OutQuint);
                        return;
                    }

                    Background.FadeIn(100, Easing.OutQuint);
                    Background.FadeColour(IsPreSelected ? BackgroundColourHover : BackgroundColourSelected, 100, Easing.OutQuint);
                }

                protected override void UpdateForegroundColour()
                {
                    base.UpdateForegroundColour();

                    if (Foreground.Children.FirstOrDefault() is Content content)
                        content.Hovering = IsHovered;
                }

                protected override Drawable CreateContent() => new Content();

                protected new partial class Content : CompositeDrawable, IHasText
                {
                    private const float chevron_offset = -3;
                    public readonly SpriteIcon Chevron;

                    public readonly SpriteText Label;

                    private bool hovering;

                    public Content()
                    {
                        RelativeSizeAxes = Axes.X;
                        AutoSizeAxes = Axes.Y;

                        InternalChildren = new Drawable[]
                        {
                            Chevron = new SpriteIcon
                            {
                                Icon = FontAwesome.Solid.ChevronRight,
                                Size = new Vector2(8),
                                Alpha = 0,
                                X = chevron_offset,
                                Margin = new MarginPadding { Left = 3, Right = 3 },
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                            Label = new CircleSpriteText
                            {
                                X = 15,
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                        };
                    }

                    public bool Hovering
                    {
                        get => hovering;
                        set
                        {
                            if (value == hovering)
                                return;

                            hovering = value;

                            if (hovering)
                            {
                                Chevron.FadeIn(400, Easing.OutQuint);
                                Chevron.MoveToX(0, 400, Easing.OutQuint);
                            }
                            else
                            {
                                Chevron.FadeOut(200);
                                Chevron.MoveToX(chevron_offset, 200, Easing.In);
                            }
                        }
                    }

                    public LocalisableString Text
                    {
                        get => Label.Text;
                        set => Label.Text = value;
                    }
                }
            }

            #endregion
        }

        #endregion

        public partial class CircleDropdownHeader : DropdownHeader
        {
            protected readonly SpriteIcon Icon;
            protected readonly CircleSpriteText Text;

            protected override DropdownSearchBar CreateSearchBar() => new CircleDropdownSearchBar(this);

            public CircleDropdownHeader()
            {
                Foreground.Padding = new MarginPadding(10);

                AutoSizeAxes = Axes.None;
                Margin = new MarginPadding { Bottom = 4 };
                CornerRadius = corner_radius;
                Height = 40;

                Foreground.Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize),
                    },
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize),
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            Text = new CircleSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                                Font = CircleFont.Default.With(size: 22)
                            },
                            Icon = new SpriteIcon
                            {
                                Icon = FontAwesome.Solid.ChevronDown,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Size = new Vector2(16),
                            },
                        }
                    }
                };
            }

            protected override LocalisableString Label
            {
                get => Text.Text;
                set => Text.Text = value;
            }

            [BackgroundDependencyLoader]
            private void load(CircleColour colours)
            {
                BackgroundColour = colours.TransparentBlack;
                BackgroundColourHover = colours.TransparentGray;
            }

            public partial class CircleDropdownSearchBar : DropdownSearchBar
            {
                private readonly CircleDropdownHeader header;

                public CircleDropdownSearchBar(CircleDropdownHeader header)
                {
                    this.header = header;
                }

                protected override void PopIn()
                {
                    Logger.Log($"Header: {header.Count}");
                    header.Except(this).ForEach(d => d.Alpha = 0);
                    this.FadeIn();
                }

                protected override void PopOut()
                {
                    header.Except(this).ForEach(d => d.Alpha = 1);
                    this.FadeOut();
                }

                protected override TextBox CreateTextBox() => new CircleTextBox
                {
                    PlaceholderText = "type to search"
                };
            }
        }
    }
}
