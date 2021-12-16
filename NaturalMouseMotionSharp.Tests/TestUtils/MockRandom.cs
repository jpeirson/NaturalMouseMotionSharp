namespace NaturalMouseMotionSharp.Tests.TestUtils
{
    using System;

    public class MockRandom : Random
    {
        private readonly double[] doubles;
        private int i;

        public MockRandom(double[] doubles) => this.doubles = doubles;

        protected override double Sample() => this.doubles[this.i++ % this.doubles.Length];
    }
}
