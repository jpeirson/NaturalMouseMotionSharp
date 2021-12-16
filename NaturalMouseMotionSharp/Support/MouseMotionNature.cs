namespace NaturalMouseMotionSharp.Support
{
    using Api;

    public class MouseMotionNature
    {
        /// <summary>
        ///     Gets or sets time to steps is how NaturalMouseMotion calculates how many locations need to be visited between
        ///     start and end point. More steps means more smooth movement. Thus increasing this divider means less
        ///     steps and decreasing means more steps.
        /// </summary>
        /// <returns> divider which is used to get amount of steps from the planned movement time</returns>
        public virtual double TimeToStepsDivider { get; set; }

        /// <summary>
        ///     Gets or sets minimum amount of steps that is taken to reach the target, this is used when calculation otherwise
        ///     would
        ///     lead to too few steps for smooth mouse movement, which can happen for very fast movements.
        /// </summary>
        /// <returns> minimal amount of steps used.</returns>
        public virtual int MinSteps { get; set; }

        /// <summary>
        ///     Gets or sets effect fade decreases the noise and deviation effects linearly to 0 at the end of the mouse movement,
        ///     so mouse would end up in the intended target pixel even when noise or deviation would otherwise
        ///     add offset to mouse position.
        /// </summary>
        /// <returns>the number of steps before last the effect starts to fade</returns>
        public virtual int EffectFadeSteps { get; set; }

        /// <summary>
        ///     Gets or sets the minimal sleep time when overshoot or some other feature has caused mouse to miss the original
        ///     target
        ///     to prepare for next attempt to move the mouse to target.
        /// </summary>
        /// <returns>the sleep time</returns>
        public virtual int ReactionTimeBaseMs { get; set; }

        /// <summary>
        ///     Gets or sets the random sleep time when overshoot or some other feature has caused mouse to miss the original
        ///     target
        ///     to prepare for next attempt to move the mouse to target. Random part of this is added to the
        ///     <see cref="ReactionTimeBaseMs" />.
        /// </summary>
        /// <returns> the sleep time</returns>
        public virtual int ReactionTimeVariationMs { get; set; }

        /// <summary>Get or sets the provider which is used to define how the MouseMotion trajectory is being deviated or arced</summary>
        /// <returns>the provider</returns>
        public virtual IDeviationProvider DeviationProvider { get; set; }

        /// <summary>
        ///     Gets or sets the provider which is used to make random mistakes in the trajectory of the moving mouse
        /// </summary>
        /// <returns>the provider</returns>
        public virtual INoiseProvider NoiseProvider { get; set; }

        /// <summary>Get or sets the accessor object, which MouseMotion uses to detect the position of mouse on screen.</summary>
        /// <returns>the accessor</returns>
        public virtual IMouseInfoAccessor MouseInfo { get; set; }

        /// <summary>Get or sets a system call interface, which MouseMotion uses internally</summary>
        /// <returns>the interface</returns>
        public virtual ISystemCalls SystemCalls { get; set; }

        /// <summary>
        ///     Get or sets the speed manager. SpeedManager controls how long does it take to complete a movement and within that
        ///     time how slow or fast the cursor is moving at a particular moment, the flow of movement.
        /// </summary>
        /// <returns>the SpeedManager</returns>
        public virtual ISpeedManager SpeedManager { get; set; }

        /// <summary>
        ///     Get or sets the manager that deals with overshoot properties.
        ///     Overshoots provide a realistic way to simulate user trying to reach the destination with mouse, but miss.
        /// </summary>
        /// <returns>the manager</returns>
        public virtual IOvershootManager OvershootManager { get; set; }
    }
}
