namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using Api;
    using Util;

    public class DefaultOvershootManager : IOvershootManager
    {
        public const double DefaultOvershootSpeedupDivider = 1.8;
        public const int DefaultMinOvershootMovementMs = 40;
        public const int DefaultOvershootRandomModifierDivider = 20;
        public const int DefaultMinDistanceForOvershoots = 10;
        public const int DefaultOvershootAmount = 3;
        private readonly Random random;

        public DefaultOvershootManager(Random random) => this.random = random;

        public long MinDistanceForOvershoots { get; set; } = DefaultMinDistanceForOvershoots;

        public long MinOvershootMovementMs { get; set; } = DefaultMinOvershootMovementMs;

        public double OvershootRandomModifierDivider { get; set; } = DefaultOvershootRandomModifierDivider;

        public int Overshoots { get; set; } = DefaultOvershootAmount;

        public double OvershootSpeedupDivider { get; set; } = DefaultOvershootSpeedupDivider;

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
