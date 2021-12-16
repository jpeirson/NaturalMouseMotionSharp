namespace NaturalMouseMotionSharp.Api
{
    using System.Drawing;
    using System.Threading;

    /// <summary>
    ///     Abstracts ordinary static System calls away
    /// </summary>
    public interface ISystemCalls
    {
        long CurrentTimeMillis();

        /// <exception cref="ThreadInterruptedException"></exception>
        void Sleep(long time);

        Size GetScreenSize();

        void SetMousePosition(int x, int y);
    }
}
