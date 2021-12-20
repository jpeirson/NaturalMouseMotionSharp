namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Native;

    internal class DefaultWindowsRobot : IRobot
    {
        public void MouseMove(int x, int y)
        {
            if (!NativeWinMethods.SetCursorPos(x, y))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public Task MouseMoveAsync(int x, int y, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            this.MouseMove(x, y);
            return Task.CompletedTask;
        }

        public Size GetScreenSize()
        {
            NativeWinMethods.DEVMODE devMode = default;
            devMode.dmSize = (short)Marshal.SizeOf(devMode);
            if (!NativeWinMethods.EnumDisplaySettings(null, NativeWinMethods.ENUM_CURRENT_SETTINGS, ref devMode))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            return new Size(devMode.dmPelsWidth, devMode.dmPelsHeight);
        }

        public Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            return Task.FromResult(this.GetScreenSize());
        }

        public Point GetMouseLocation()
        {
            if (!NativeWinMethods.GetCursorPos(out var lpPoint))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            return new Point(lpPoint.X, lpPoint.Y);
        }

        public Task<Point> GetMouseLocationAsync(CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            return Task.FromResult(this.GetMouseLocation());
        }
    }
}
