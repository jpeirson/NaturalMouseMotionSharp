namespace NaturalMouseMotionSharp.Api
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using Support;

    /// <summary>
    ///     This class should be used for creating new <see cref="MouseMotion" />s. The default instance
    ///     is available via <see cref="Default" />, but can create new instance via constructor.
    /// </summary>
    public class MouseMotionFactory
    {
        private static readonly Lazy<MouseMotionFactory> DefaultFactory =
            new Lazy<MouseMotionFactory>(() => new MouseMotionFactory(new DefaultRobot()),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly MouseMotionNature nature;
        private readonly Random random = new Random();

        public MouseMotionFactory(MouseMotionNature nature) => this.nature = nature;

        public MouseMotionFactory(IRobot robot) : this(new DefaultMouseMotionNature(robot)) { }

        /// Get the default factory implementation.
        /// <returns>the factory</returns>
        public static MouseMotionFactory Default => DefaultFactory.Value;

        /// <inheritdoc cref="MouseMotionNature.SystemCalls" />
        public ISystemCalls SystemCalls
        {
            get => this.nature.SystemCalls;
            set => this.nature.SystemCalls = value;
        }

        /// <inheritdoc cref="MouseMotionNature.DeviationProvider" />
        public IDeviationProvider DeviationProvider
        {
            get => this.nature.DeviationProvider;
            set => this.nature.DeviationProvider = value;
        }

        /// <inheritdoc cref="MouseMotionNature.NoiseProvider" />
        public INoiseProvider NoiseProvider
        {
            get => this.nature.NoiseProvider;
            set => this.nature.NoiseProvider = value;
        }

        /// <inheritdoc cref="MouseMotionNature.SpeedManager" />
        public ISpeedManager SpeedManager
        {
            get => this.nature.SpeedManager;
            set => this.nature.SpeedManager = value;
        }

        /// <summary>
        ///     Get or sets the random used whenever randomized behavior is needed in MouseMotion
        /// </summary>
        /// <returns>the random</returns>
        public Random Random { get; set; }

        /// <inheritdoc cref="MouseMotionNature.MouseInfo" />
        public IMouseInfoAccessor MouseInfo
        {
            get => this.nature.MouseInfo;
            set => this.nature.MouseInfo = value;
        }

        /// <summary>
        ///     Gets or sets the Nature of mousemotion covers all aspects how the mouse is moved.
        /// </summary>
        public MouseMotionNature Nature { get; set; }

        /// <inheritdoc cref="MouseMotionNature.OvershootManager" />
        public IOvershootManager OvershootManager
        {
            get => this.nature.OvershootManager;
            set => this.nature.OvershootManager = value;
        }

        /// <summary>Builds the <see cref="MouseMotion" />, which can be executed instantly or saved for later.</summary>
        /// <param name="xDest">the end position x-coordinate for the mouse</param>
        /// <param name="yDest">the end position y-coordinate for the mouse</param>
        /// <returns>the <see cref="MouseMotion" /> which can be executed instantly or saved for later.</returns>
        /// <remarks>
        ///     (Mouse will be moved from its current position, not from the position
        ///     where mouse was during building.)
        /// </remarks>
        public MouseMotion Build(int xDest, int yDest, ILogger log = null) =>
            new MouseMotion(this.nature, this.random, xDest, yDest, log);

        /// <summary>Start moving the mouse to specified location. Blocks until done.</summary>
        /// <param name="xDest">the end position x-coordinate for the mouse</param>
        /// <param name="yDest">the end position y-coordinate for the mouse</param>
        /// <exception cref="ThreadInterruptedException">if something interrupts the thread.</exception>
        public void Move(int xDest, int yDest, ILogger log = null) => this.Build(xDest, yDest, log).Move();
    }
}
