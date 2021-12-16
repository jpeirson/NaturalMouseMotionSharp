namespace NaturalMouseMotionSharp.Api
{
    using System.Drawing;

    /// <summary>Abstraction for getting mouse position.</summary>
    public interface IMouseInfoAccessor
    {
        /// <summary>Get the current mouse position.</summary>
        /// <returns>the current mouse position</returns>
        /// <remarks>
        ///     NB, for optimization reasons this method might return the same Point instance, but is not guaranteed to.
        ///     It is recommended not to save this Point anywhere as it may or may not change its coordinates.
        /// </remarks>
        Point GetMousePosition();
    }
}
