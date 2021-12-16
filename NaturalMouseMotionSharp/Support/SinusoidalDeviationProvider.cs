namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;

    public class SinusoidalDeviationProvider : IDeviationProvider
    {
        public const int DefaultSlopeDivider = 10;
        private readonly double slopeDivider;

        public SinusoidalDeviationProvider(double slopeDivider) => this.slopeDivider = slopeDivider;

        public DoublePoint GetDeviation(double totalDistanceInPixels, double completionFraction)
        {
            var deviationFunctionResult = (1 - Math.Cos(completionFraction * Math.PI * 2)) / 2;

            var deviationX = totalDistanceInPixels / this.slopeDivider;
            var deviationY = totalDistanceInPixels / this.slopeDivider;

            return new DoublePoint(deviationFunctionResult * deviationX, deviationFunctionResult * deviationY);
        }
    }
}
