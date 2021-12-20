namespace NaturalMouseMotionSharp.Api
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Abstracts ordinary static System calls away
    /// </summary>
    public interface ISystemCalls
    {
        /// <summary>
        ///     Gets the current time, in milliseconds since 1 Jan 1970 UTC.
        /// </summary>
        /// <returns>Current time in milliseconds.</returns>
        long CurrentTimeMillis();

        /// <summary>Blocks the current thread for the given amount of time.</summary>
        /// <param name="time">Time to wait, in milliseconds.</param>
        /// <exception cref="ThreadInterruptedException">Sleep is interrupted.</exception>
        void Sleep(long time);

        /// <summary>Async call, waits the given amount of time.</summary>
        /// <param name="time">Time to wait, in milliseconds.</param>
        /// <param name="cancellation">Supports cancellation</param>
        /// <returns>A task that completes when sleep is done.</returns>
        /// <exception cref="OperationCanceledException">Sleep is interrupted.</exception>
        Task SleepAsync(long time, CancellationToken cancellation = default);

        /// <summary>
        ///     Gets the primary screen's size, in pixels.
        /// </summary>
        /// <returns>Size in pixels.</returns>
        Size GetScreenSize();

        /// <summary>
        ///     Asynchronously gets the primary screen's size, in pixels.
        /// </summary>
        /// <param name="cancellation">Supports cancellation</param>
        /// <returns>A task that returns the screen size in pixels.</returns>
        /// <exception cref="OperationCanceledException">Task is interrupted</exception>
        Task<Size> GetScreenSizeAsync(CancellationToken cancellation = default);

        /// <summary>
        ///     Sets the current mouse position.
        /// </summary>
        /// <param name="x">X coordinate, in pixels</param>
        /// <param name="y">Y coordinate, in pixels</param>
        void SetMousePosition(int x, int y);

        /// <summary>
        ///     Asynchronously sets the current mouse position.
        /// </summary>
        /// <param name="x">X coordinate, in pixels</param>
        /// <param name="y">Y coordinate, in pixels</param>
        /// <param name="cancellation">Supports cancellation</param>
        /// <returns>A task that completes when the mouse has moved to the given position.</returns>
        /// <exception cref="OperationCanceledException">Task is interrupted</exception>
        Task SetMousePositionAsync(int x, int y, CancellationToken cancellation = default);
    }
}
