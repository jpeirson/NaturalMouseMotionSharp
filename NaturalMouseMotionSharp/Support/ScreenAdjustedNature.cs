namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using Api;

    /// <summary>
    ///     This nature translates mouse coordinates to specified offset and screen dimension.
    ///     Internally it wraps the SystemCalls and MouseInfoAccessor in proxies which handle the translations.
    /// </summary>
    public class ScreenAdjustedNature : DefaultMouseMotionNature
    {
        private readonly Point offset;
        private readonly Size screenSize;

        public ScreenAdjustedNature(int x, int y, int x2, int y2) :
            this(new Size(x2 - x, y2 - y), new Point(x, y))
        {
            if (y2 <= y || x2 <= x)
            {
                throw new ArgumentException("Invalid range " + x + " " + y + " " + x2 + " " + y2);
            }
        }

        public ScreenAdjustedNature(Size screenSize, Point mouseOffset)
        {
            this.screenSize = screenSize;
            this.offset = mouseOffset;
        }

        public override IMouseInfoAccessor MouseInfo
        {
            get => base.MouseInfo;
            set => base.MouseInfo = new ProxyMouseInfo(this, value);
        }

        public override ISystemCalls SystemCalls
        {
            get => base.SystemCalls;
            set => base.SystemCalls = new ProxySystemCalls(this, value);
        }

        private class ProxyMouseInfo : IMouseInfoAccessor
        {
            private readonly ScreenAdjustedNature owner;
            private readonly IMouseInfoAccessor underlying;

            // This implementation reuses the point.
            private Point p;

            public ProxyMouseInfo(ScreenAdjustedNature owner, IMouseInfoAccessor underlying)
            {
                this.owner = owner;
                this.underlying = underlying;
            }

            public Point GetMousePosition()
            {
                var realPointer = this.underlying.GetMousePosition();
                this.p.X = realPointer.X - this.owner.offset.X;
                this.p.Y = realPointer.Y - this.owner.offset.Y;
                return this.p;
            }
        }

        private class ProxySystemCalls : ISystemCalls
        {
            private readonly ScreenAdjustedNature owner;
            private readonly ISystemCalls underlying;

            public ProxySystemCalls(ScreenAdjustedNature owner, ISystemCalls underlying)
            {
                this.owner = owner;
                this.underlying = underlying;
            }

            public long CurrentTimeMillis() => this.underlying.CurrentTimeMillis();

            public void Sleep(long time) => this.underlying.Sleep(time);

            public Size GetScreenSize() => this.owner.screenSize;

            public void SetMousePosition(int x, int y) =>
                this.underlying.SetMousePosition(x + this.owner.offset.X, y + this.owner.offset.Y);
        }
    }
}
