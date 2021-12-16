namespace NaturalMouseMotionSharp.Api
{
    using System;
    using Support;

    /// <summary>
    ///     Provides noise or mistakes in the mouse movement
    /// </summary>
    /// <remarks>
    ///     <see cref="INoiseProvider" /> implementation should be immutable.
    /// </remarks>
    public interface INoiseProvider
    {
        /// <summary>
        ///     Noise is offset from the original trajectory, simulating user and physical errors on mouse movement.
        /// </summary>
        /// <param name="random">random use this to generate randomness in the offset</param>
        /// <param name="xStepSize">the step size that is taken horizontally</param>
        /// <param name="yStepSize">the step size that is taken vertically</param>
        /// <returns>
        ///     a point which describes how much the mouse offset is increased or decreased this step.
        ///     This value must not include the parameters xStepSize and yStepSize. For no change in noise just return (0,0).
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Noise is accumulating, so on average it should create an equal chance of either positive or negative movement
        ///         on each axis, otherwise the mouse movement will always be slightly offset to single direction.
        ///     </para>
        ///     <para>
        ///         Deviation from DeviationProvider is different from the Noise
        ///         because it works like a mathematical function and is not accumulating.
        ///     </para>
        ///     <para>
        ///         Not every step needs to add noise, use randomness to only add noise sometimes, otherwise return Point(0, 0).
        ///     </para>
        ///     <para>
        ///         During the final steps of mouse movement, the effect of noise is gradually reduced, so the mouse
        ///         would finish on the intended pixel smoothly, thus the implementation of this class can safely ignore
        ///         and not know the beginning and end of the movement.
        ///     </para>
        /// </remarks>
        /// <seealso cref="IDeviationProvider" />
        DoublePoint GetNoise(Random random, double xStepSize, double yStepSize);
    }
}
