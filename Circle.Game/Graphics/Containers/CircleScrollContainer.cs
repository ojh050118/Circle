using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace Circle.Game.Graphics.Containers
{
    public class CircleScrollContainer : CircleScrollContainer<Drawable>
    {
        public CircleScrollContainer()
        {
        }

        public CircleScrollContainer(Direction direction)
            : base(direction)
        {
        }
    }

    public class CircleScrollContainer<T> : ScrollContainer<T> where T : Drawable
    {
        public const float SCROLL_BAR_HEIGHT = 10;
        public const float SCROLL_BAR_PADDING = 3;

        /// <summary>
        /// Allows controlling the scroll bar from any position in the container using the right mouse button.
        /// Uses the value of <see cref="DistanceDecayOnRightMouseScrollbar"/> to smoothly scroll to the dragged location.
        /// </summary>
        public bool RightMouseScrollbar;

        /// <summary>
        /// Controls the rate with which the target position is approached when performing a relative drag. Default is 0.02.
        /// </summary>
        public double DistanceDecayOnRightMouseScrollbar = 0.02;

        private bool shouldPerformRightMouseScroll(MouseButtonEvent e) => RightMouseScrollbar && e.Button == MouseButton.Right;

        private void scrollFromMouseEvent(MouseEvent e) =>
            ScrollTo(Clamp(ToLocalSpace(e.ScreenSpaceMousePosition)[ScrollDim] / DrawSize[ScrollDim]) * Content.DrawSize[ScrollDim], true, DistanceDecayOnRightMouseScrollbar);

        private bool rightMouseDragging;

        protected override bool IsDragging => base.IsDragging || rightMouseDragging;

        public CircleScrollContainer(Direction scrollDirection = Direction.Vertical)
            : base(scrollDirection)
        {
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (shouldPerformRightMouseScroll(e))
            {
                scrollFromMouseEvent(e);
                return true;
            }

            return base.OnMouseDown(e);
        }

        protected override void OnDrag(DragEvent e)
        {
            if (rightMouseDragging)
            {
                scrollFromMouseEvent(e);
                return;
            }

            base.OnDrag(e);
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            if (shouldPerformRightMouseScroll(e))
            {
                rightMouseDragging = true;
                return true;
            }

            return base.OnDragStart(e);
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            if (rightMouseDragging)
            {
                rightMouseDragging = false;
                return;
            }

            base.OnDragEnd(e);
        }

        protected override ScrollbarContainer CreateScrollbar(Direction direction) => new CircleScrollbar(direction);

        protected class CircleScrollbar : ScrollbarContainer
        {
            private Color4 hoverColour;
            private Color4 defaultColour;
            private Color4 highlightColour;

            private readonly Box box;

            public CircleScrollbar(Direction scrollDir)
                : base(scrollDir)
            {
                Blending = BlendingParameters.Additive;

                CornerRadius = 5;

                // needs to be set initially for the ResizeTo to respect minimum size
                Size = new Vector2(SCROLL_BAR_HEIGHT);

                const float margin = 3;

                Margin = new MarginPadding
                {
                    Left = scrollDir == Direction.Vertical ? margin : 0,
                    Right = scrollDir == Direction.Vertical ? margin : 0,
                    Top = scrollDir == Direction.Horizontal ? margin : 0,
                    Bottom = scrollDir == Direction.Horizontal ? margin : 0,
                };

                Masking = true;
                Child = box = new Box { RelativeSizeAxes = Axes.Both };
            }

            [BackgroundDependencyLoader]
            private void load(CircleColour colours)
            {
                Colour = defaultColour = colours.TransparentGray;
                hoverColour = colours.TransparentWhite;
                highlightColour = colours.TransparentDeepSkyBlue.Opacity(0.8f);
            }

            public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
            {
                Vector2 size = new Vector2(SCROLL_BAR_HEIGHT)
                {
                    [(int)ScrollDirection] = val
                };
                this.ResizeTo(size, duration, easing);
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.FadeColour(hoverColour, 100);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                this.FadeColour(defaultColour, 200);
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                if (!base.OnMouseDown(e)) return false;

                // note that we are changing the colour of the box here as to not interfere with the hover effect.
                box.FadeColour(highlightColour, 100);
                return true;
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                if (e.Button != MouseButton.Left) return;

                box.FadeColour(Color4.White, 200);

                base.OnMouseUp(e);
            }
        }
    }
}
