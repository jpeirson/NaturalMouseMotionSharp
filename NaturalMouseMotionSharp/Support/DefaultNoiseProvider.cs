namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;
    using Util;

    public class DefaultNoiseProvider : INoiseProvider
    {
        public const double DefaultNoisinessDivider = 2;
        private const double SmallDelta = 10e-6;
        private readonly double noisinessDivider;

        /// <param name="noisinessDivider">bigger value means less noise.</param>
        public DefaultNoiseProvider(double noisinessDivider) => this.noisinessDivider = noisinessDivider;

        public DoublePoint GetNoise(Random random, double xStepSize, double yStepSize)
        {
            if (Math.Abs(xStepSize - 0) < SmallDelta && Math.Abs(yStepSize - 0) < SmallDelta)
            {
                return DoublePoint.Zero;
            }

            double noiseX = 0;
            double noiseY = 0;
            var stepSize = MathUtil.Hypot(xStepSize, yStepSize);
            var noisiness = Math.Max(0, 8 - stepSize) / 50;
            if (random.NextDouble() < noisiness)
            {
                noiseX = (random.NextDouble() - 0.5) * Math.Max(0, 8 - stepSize) / this.noisinessDivider;
                noiseY = (random.NextDouble() - 0.5) * Math.Max(0, 8 - stepSize) / this.noisinessDivider;
            }

            return new DoublePoint(noiseX, noiseY);
        }
    }
}
