namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     This class is used to generate native system input events for the purposes of test automation, self-running demos,
    ///     and other applications where control of the mouse and keyboard is needed.
    /// </summary>
    /// <remarks>
    ///     This is an abstraction of Java's <c>java.awt.Robot</c> used by the original Java-based NaturalMouseMotion of which
    ///     this is a C# port.
    /// </remarks>
    public interface IRobot
    {
        void MouseMove(int x, int y);
        Task MouseMoveAsync(int x, int y, CancellationToken cancellation = default);
        Size GetScreenSize();
        Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default);
        Point GetMouseLocation();
        Task<Point> GetMouseLocationAsync(CancellationToken cancellation = default);
    }
}
