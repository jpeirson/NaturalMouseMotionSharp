namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DefaultLinuxRobot : IRobot
    {
        public void MouseMove(int x, int y) => throw new NotImplementedException();
        public Task MouseMoveAsync(int x, int y, CancellationToken cancellation = default) => throw new NotImplementedException();

        public Size GetScreenSize() => throw new NotImplementedException();
        public Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default) => throw new NotImplementedException();

        public Point GetMouseLocation() => throw new NotImplementedException();
        public Task<Point> GetMouseLocationAsync(CancellationToken cancellation = default) => throw new NotImplementedException();
    }
}
