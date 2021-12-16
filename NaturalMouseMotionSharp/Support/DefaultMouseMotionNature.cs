namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;

    public class DefaultMouseMotionNature : MouseMotionNature
    {
        public const int TIME_TO_STEPS_DIVIDER = 8;
        public const int MIN_STEPS = 10;
        public const int EFFECT_FADE_STEPS = 15;
        public const int REACTION_TIME_BASE_MS = 20;
        public const int REACTION_TIME_VARIATION_MS = 120;

        public DefaultMouseMotionNature(ISystemCalls systemCalls) :
            this(systemCalls, new DefaultMouseInfoAccessor())
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
            this.DeviationProvider = new SinusoidalDeviationProvider(SinusoidalDeviationProvider.DEFAULT_SLOPE_DIVIDER);
            this.NoiseProvider = new DefaultNoiseProvider(DefaultNoiseProvider.DEFAULT_NOISINESS_DIVIDER);
            this.SpeedManager = new DefaultSpeedManager();
            this.OvershootManager = new DefaultOvershootManager(new Random());
            this.EffectFadeSteps = EFFECT_FADE_STEPS;
            this.MinSteps = MIN_STEPS;
            this.MouseInfo = mouseInfoAccessor;
            this.ReactionTimeBaseMs = REACTION_TIME_BASE_MS;
            this.ReactionTimeVariationMs = REACTION_TIME_VARIATION_MS;
            this.TimeToStepsDivider = TIME_TO_STEPS_DIVIDER;
        }

        public DefaultMouseMotionNature(IRobot robot) :
            this(new DefaultSystemCalls(robot))
        {
        }
    }
}
