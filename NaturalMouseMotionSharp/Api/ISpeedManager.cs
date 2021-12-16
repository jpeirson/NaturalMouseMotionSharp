namespace NaturalMouseMotionSharp.Api
{
    using Support;
    using Util;

    /// <summary>
    ///     SpeedManager controls how long does it take to complete a mouse movement and within that
    ///     time how slow or fast the cursor is moving at a particular moment, the flow.
    ///     Flow controls how jagged or smooth, accelerating or decelerating, the movement is.
    /// </summary>
    public interface ISpeedManager
    {
        /// <summary>Get the SpeedFlow object, which contains Flow and planned time for mouse movement in ms.</summary>
        /// <param name="distance">the distance from where the cursor is now to the destination point  </param>
        /// <returns>the SpeedFlow object, which details are a <see cref="ISpeedManager" /> implementation decision.</returns>
        Pair<Flow, long> GetFlowWithTime(double distance);
    }
}
