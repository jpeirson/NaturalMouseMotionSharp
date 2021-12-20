namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Default <see cref="IRobot" /> implementation that works on Windows (non-headless) and Linux (non-headless).
    /// </summary>
    public class DefaultRobot : IRobot
    {
        private readonly IRobot platform;

        public DefaultRobot()
        {
            // Figure out which platform we're on, and delegate.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.platform = new DefaultWindowsRobot();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.platform = new DefaultLinuxRobot();
            }
            else
            {
                throw new NotSupportedException("No default implementation for this platform.");
            }
        }

        public void MouseMove(int x, int y) => this.platform.MouseMove(x, y);

        public Task MouseMoveAsync(int x, int y, CancellationToken cancellation = default) =>
            this.platform.MouseMoveAsync(x, y, cancellation);

        public Size GetScreenSize() => this.platform.GetScreenSize();

        public Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default) =>
            this.platform.GetScreenSizeAsync(cancellation);

        public Point GetMouseLocation() => this.platform.GetMouseLocation();

        public Task<Point> GetMouseLocationAsync(CancellationToken cancellation = default) =>
            this.platform.GetMouseLocationAsync(cancellation);
    }
}
