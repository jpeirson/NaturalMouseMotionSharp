namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using Api;
    using Util;

    public class DefaultOvershootManager : IOvershootManager
    {
        public const double OVERSHOOT_SPEEDUP_DIVIDER = 1.8;
        public const int MIN_OVERSHOOT_MOVEMENT_MS = 40;
        public const int OVERSHOOT_RANDOM_MODIFIER_DIVIDER = 20;
        public const int MIN_DISTANCE_FOR_OVERSHOOTS = 10;
        public const int DEFAULT_OVERSHOOT_AMOUNT = 3;
        private readonly Random random;

        public DefaultOvershootManager(Random random) => this.random = random;

        public long MinDistanceForOvershoots { get; set; } = MIN_DISTANCE_FOR_OVERSHOOTS;

        public long MinOvershootMovementMs { get; set; } = MIN_OVERSHOOT_MOVEMENT_MS;

        public double OvershootRandomModifierDivider { get; set; } = OVERSHOOT_RANDOM_MODIFIER_DIVIDER;

        public int Overshoots { get; set; } = DEFAULT_OVERSHOOT_AMOUNT;

        public double OvershootSpeedupDivider { get; set; } = OVERSHOOT_SPEEDUP_DIVIDER;

        public int GetOvershoots(Flow flow, long mouseMovementMs, double distance)
        {
            if (distance < this.MinDistanceForOvershoots)
            {
                return 0;
            }

            return this.Overshoots;
        }

        public Point GetOvershootAmount(double distanceToRealTargetX, double distanceToRealTargetY,
            long mouseMovementMs,
            int overshootsRemaining)
        {
            var distanceToRealTarget = MathUtil.Hypot(distanceToRealTargetX, distanceToRealTargetY);

            var randomModifier = distanceToRealTarget / this.OvershootRandomModifierDivider;
            //double speedPixelsPerSecond = distanceToRealTarget / mouseMovementMs * 1000; // TODO utilize speed
            var x = (int)((this.random.NextDouble() * randomModifier) - (randomModifier / 2d)) * overshootsRemaining;
            var y = (int)((this.random.NextDouble() * randomModifier) - (randomModifier / 2d)) * overshootsRemaining;
            return new Point(x, y);
        }

        public long DeriveNextMouseMovementTimeMs(long mouseMovementMs, int overshootsRemaining) =>
            Math.Max((long)(mouseMovementMs / this.OvershootSpeedupDivider), this.MinOvershootMovementMs);
    }
}
