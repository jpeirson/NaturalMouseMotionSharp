namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Api;

    /// <summary>
    ///     Default implementation of <see cref="ISystemCalls" /> that uses a <see cref="IRobot" />.
    /// </summary>
    public class DefaultSystemCalls : ISystemCalls
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IRobot robot;

        /// <summary>
        ///     Creates a new instance wrapped around a default robot.
        /// </summary>
        public DefaultSystemCalls() : this(new DefaultRobot())
        {
        }

        /// <summary>
        ///     Creates a new instance wrapped around the given robot.
        /// </summary>
        /// <param name="robot">A <see cref="IRobot" /></param>
        public DefaultSystemCalls(IRobot robot) => this.robot = robot;

        /// <inheritdoc />
        public long CurrentTimeMillis() => (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;

        /// <inheritdoc />
        public void Sleep(long time) => Thread.Sleep(TimeSpan.FromMilliseconds(time));

        /// <inheritdoc />
        public Task SleepAsync(long time, CancellationToken cancellation = default) =>
            Task.Delay(TimeSpan.FromMilliseconds(time), cancellation);

        /// <inheritdoc />
        public Size GetScreenSize() => this.robot.GetScreenSize();

        /// <inheritdoc />
        public Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default) =>
            this.robot.GetScreenSizeAsync(cancellation);

        /// <summary>Moves the mouse to specified pixel using the provided Robot.</summary>
        /// <remarks>
        ///     It seems there is a certain delay, measurable in less than milliseconds,
        ///     before the mouse actually ends up on the requested pixel when using a Robot class.
        ///     this usually isn't a problem, but when we ask the mouse position right after this call,
        ///     there's extremely low but real chance we get wrong information back. I didn't add sleep
        ///     here as it would cause overhead to sleep always, even when we don't instantly use
        ///     the mouse position, but just acknowledged the issue with this warning.
        ///     (Use fast unrestricted loop of Robot movement and checking the position after every move to invoke the issue.)
        /// </remarks>
        /// <param name="x">the x-coordinate</param>
        /// <param name="y">the y-coordinate</param>
        public void SetMousePosition(int x, int y) => this.robot.MouseMove(x, y);

        /// <inheritdoc />
        public Task SetMousePositionAsync(int x, int y, CancellationToken cancellation = default) =>
            this.robot.MouseMoveAsync(x, y, cancellation);
    }
}
