namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;

    /// <summary>
    ///     Helper implementation of <see cref="INoiseProvider" /> that implements
    ///     <see cref="INoiseProvider.GetNoise" /> as
    ///     a configurable function.
    /// </summary>
    public class NoiseProvider : INoiseProvider
    {
        private readonly Func<Random, double, double, DoublePoint> getNoise;

        public NoiseProvider(Func<Random, double, double, DoublePoint> getNoise) => this.getNoise = getNoise;

        public DoublePoint GetNoise(Random random, double xStepSize, double yStepSize) =>
            this.getNoise(random, xStepSize, yStepSize);
    }
}
