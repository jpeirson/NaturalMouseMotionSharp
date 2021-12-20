namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;

    public class DefaultMouseMotionNature : MouseMotionNature
    {
        public const int DefaultTimeToStepsDivider = 8;
        public const int DefaultMinSteps = 10;
        public const int DefaultEffectFadeSteps = 15;
        public const int DefaultReactionTimeBaseMs = 20;
        public const int DefaultReactionTimeVariationMs = 120;

        public DefaultMouseMotionNature(ISystemCalls systemCalls) : this(new DefaultRobot(), systemCalls)
        {
        }

        public DefaultMouseMotionNature(IRobot robot, ISystemCalls systemCalls) :
            this(systemCalls, new DefaultMouseInfoAccessor(robot))
        {
        }

        /// <summary>
        ///     Create a default mouse motion nature with custom system calls and mouse info.
        ///     Use this when running somewhere where java Robot does not work.
        /// </summary>
        /// <param name="systemCalls">custom system calls to be used in the nature</param>
        /// <param name="mouseInfoAccessor">custom mouse info accessor to be used in the nature</param>
        public DefaultMouseMotionNature(ISystemCalls systemCalls, IMouseInfoAccessor mouseInfoAccessor)
        {
            this.SystemCalls = systemCalls;
            this.DeviationProvider = new SinusoidalDeviationProvider(SinusoidalDeviationProvider.DefaultSlopeDivider);
            this.NoiseProvider = new DefaultNoiseProvider(DefaultNoiseProvider.DefaultNoisinessDivider);
            this.SpeedManager = new DefaultSpeedManager();
            this.OvershootManager = new DefaultOvershootManager(new Random());
            this.EffectFadeSteps = DefaultEffectFadeSteps;
            this.MinSteps = DefaultMinSteps;
            this.MouseInfo = mouseInfoAccessor;
            this.ReactionTimeBaseMs = DefaultReactionTimeBaseMs;
            this.ReactionTimeVariationMs = DefaultReactionTimeVariationMs;
            this.TimeToStepsDivider = DefaultTimeToStepsDivider;
        }

        public DefaultMouseMotionNature() : this(new DefaultRobot())
        {
        }

        public DefaultMouseMotionNature(IRobot robot) :
            this(robot, new DefaultSystemCalls(robot))
        {
        }
    }
}
