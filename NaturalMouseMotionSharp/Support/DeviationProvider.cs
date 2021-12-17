namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;

    /// <summary>
    ///     Helper implementation of <see cref="IDeviationProvider" /> that implements
    ///     <see cref="IDeviationProvider.GetDeviation" /> as
    ///     a configurable function.
    /// </summary>
    public class DeviationProvider : IDeviationProvider
    {
        private readonly Func<double, double, DoublePoint> getDeviation;

        public DeviationProvider(Func<double, double, DoublePoint> getDeviation) => this.getDeviation = getDeviation;

        public DoublePoint GetDeviation(double totalDistanceInPixels, double completionFraction) =>
            this.getDeviation(totalDistanceInPixels, completionFraction);
    }
}
