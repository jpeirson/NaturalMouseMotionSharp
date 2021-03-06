namespace NaturalMouseMotionSharp.Tests.Support.MouseMotion
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Api;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NaturalMouseMotionSharp.Support.MouseMotion;
    using NaturalMouseMotionSharp.Util;
    using NSubstitute;
    using NUnit.Framework;

    public class MovementFactoryTest
    {
        private const double SmallDelta = 0.00000000001;

        [Test]
        public void TestSingleMovement()
        {
            var speedManager = this.CreateConstantSpeedManager(100);
            var overshootManager = this.CreateNoOvershootManager();
            var factory = new MovementFactory(50, 51, speedManager, overshootManager, new Size(500, 500));

            var movements = factory.CreateMovements(new Point(100, 100));
            movements.Count.Should().Be(1);
            movements.First.Value.DestX.Should().Be(50);
            movements.First.Value.DestY.Should().Be(51);
            movements.First.Value.Time.Should().Be(100);
            movements.First.Value.XDistance.Should().Be(-50);
            movements.First.Value.YDistance.Should().Be(-49);
            this.AssertArrayEquals(new double[] { 100 }, movements.First.Value.Flow.GetFlowCharacteristics());
        }

        [Test]
        public void TestMultipleMovement()
        {
            var speedManager = this.CreateConstantSpeedManager(100);
            var overshootManager = this.CreateMultiOvershootManager();
            var factory = new MovementFactory(50, 150, speedManager, overshootManager, new Size(500, 500));

            var movements = factory.CreateMovements(new Point(100, 100));
            movements.Count.Should().Be(3);

            var first = movements.First.Value;
            movements.RemoveFirst();
            first.DestX.Should().Be(55);
            first.DestY.Should().Be(155);
            first.Time.Should().Be(100);
            first.XDistance.Should().Be(-45);
            first.YDistance.Should().Be(55);
            first.Distance.Should().BeApproximately(Hypot(first.XDistance, first.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, first.Flow.GetFlowCharacteristics());

            var second = movements.First.Value;
            movements.RemoveFirst();
            second.DestX.Should().Be(45);
            second.DestY.Should().Be(145);
            second.Time.Should().Be(50);
            second.XDistance.Should().Be(-10);
            second.YDistance.Should().Be(-10);
            second.Distance.Should().BeApproximately(Hypot(second.XDistance, second.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, second.Flow.GetFlowCharacteristics());


            var third = movements.First.Value;
            movements.RemoveFirst();
            third.DestX.Should().Be(50);
            third.DestY.Should().Be(150);
            third.Time.Should().Be(50);
            third.XDistance.Should().Be(5);
            third.YDistance.Should().Be(5);
            third.Distance.Should().BeApproximately(Hypot(third.XDistance, third.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, third.Flow.GetFlowCharacteristics());
        }

        [Test]
        public void TestZeroOffsetOvershootsRemovedFromEnd()
        {
            var speedManager = this.CreateConstantSpeedManager(64);
            var overshootManager = this.CreateOvershootManagerWithZeroOffsets();
            var factory = new MovementFactory(50, 150, speedManager, overshootManager, new Size(500, 500));

            var movements = factory.CreateMovements(new Point(100, 100));
            movements.Count.Should().Be(4); // 3 overshoots and 1 final approach to destination

            var first = movements.First.Value;
            movements.RemoveFirst();
            first.DestX.Should().Be(55);
            first.DestY.Should().Be(155);
            first.Time.Should().Be(64);
            first.XDistance.Should().Be(-45);
            first.YDistance.Should().Be(55);
            first.Distance.Should().BeApproximately(Hypot(first.XDistance, first.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, first.Flow.GetFlowCharacteristics());

            // 0-offset in the middle is not removed, this one actually hits destination.
            var second = movements.First.Value;
            movements.RemoveFirst();
            second.DestX.Should().Be(50);
            second.DestY.Should().Be(150);
            second.Time.Should().Be(32);
            second.XDistance.Should().Be(-5);
            second.YDistance.Should().Be(-5);
            second.Distance.Should().BeApproximately(Hypot(second.XDistance, second.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, second.Flow.GetFlowCharacteristics());

            var third = movements.First.Value;
            movements.RemoveFirst();
            third.DestX.Should().Be(51);
            third.DestY.Should().Be(151);
            third.Time.Should().Be(16);
            third.XDistance.Should().Be(1);
            third.YDistance.Should().Be(1);
            third.Distance.Should().BeApproximately(Hypot(third.XDistance, third.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, third.Flow.GetFlowCharacteristics());

            var fourth = movements.First.Value;
            movements.RemoveFirst();
            fourth.DestX.Should().Be(50);
            fourth.DestY.Should().Be(150);
            fourth.Time.Should().Be(32);
            fourth.XDistance.Should().Be(-1);
            fourth.YDistance.Should().Be(-1);
            fourth.Distance.Should().BeApproximately(Hypot(fourth.XDistance, fourth.YDistance), SmallDelta);
            this.AssertArrayEquals(new double[] { 100 }, fourth.Flow.GetFlowCharacteristics());
        }

        [Test]
        public void TestZeroOffsetOvershootsRemovedFromEndIfAllZero()
        {
            var speedManager = this.CreateConstantSpeedManager(100);
            var overshootManager = this.CreateOvershootManagerWithAllZeroOffsets();
            var factory = new MovementFactory(50, 150, speedManager, overshootManager, new Size(500, 500));

            var movements = factory.CreateMovements(new Point(100, 100));
            movements.Count.Should().Be(1);

            movements.First.Value.DestX.Should().Be(50);
            movements.First.Value.DestY.Should().Be(150);
            movements.First.Value.Time.Should().Be(50);
            movements.First.Value.XDistance.Should().Be(-50);
            movements.First.Value.YDistance.Should().Be(50);
            this.AssertArrayEquals(new double[] { 100 }, movements.First.Value.Flow.GetFlowCharacteristics());
        }

        private void AssertArrayEquals(double[] expected, double[] actual) =>
            actual.Should().Equal(expected, ApproxEqual);

        private static bool ApproxEqual(double x, double y) => Math.Abs(x - y) <= SmallDelta;

        protected ISpeedManager CreateConstantSpeedManager(long time)
        {
            var m = Substitute.For<ISpeedManager>();
            m.GetFlowWithTime(Arg.Any<double>())
                .Returns(call => new Pair<Flow, long>(new Flow(new double[] { 100 }), time));
            return m;
        }

        private IOvershootManager CreateNoOvershootManager() => Substitute.For<IOvershootManager>();

        private IOvershootManager CreateMultiOvershootManager()
        {
            Point[] points = { new Point(5, 5), new Point(-5, -5) };
            return CreateOvershootManager(points);
        }

        private IOvershootManager CreateOvershootManagerWithZeroOffsets()
        {
            Point[] points = { new Point(5, 5), new Point(0, 0), new Point(1, 1), new Point(0, 0), new Point(0, 0) };
            return CreateOvershootManager(points);
        }

        private IOvershootManager CreateOvershootManagerWithAllZeroOffsets()
        {
            Point[] points = { new Point(0, 0), new Point(0, 0), new Point(0, 0) };
            return CreateOvershootManager(points);
        }

        private static IOvershootManager CreateOvershootManager(Point[] points)
        {
            var deque = new LinkedList<Point>(points);

            var m = Substitute.For<IOvershootManager>();
            m.GetOvershoots(Arg.Any<Flow>(), 0, 0).ReturnsForAnyArgs(points.Length);
            m.GetOvershootAmount(0, 0, 0, 0).ReturnsForAnyArgs(_ =>
            {
                var p = deque.First.Value;
                deque.RemoveFirst();
                return p;
            });
            m.DeriveNextMouseMovementTimeMs(0, 0).ReturnsForAnyArgs(call => call.ArgAt<long>(0) / 2);
            return m;
        }


        /// <summary>
        ///     Helper method to translate Java's <c>Math.hypot</c> to C#.
        /// </summary>
        private static double Hypot(double x, double y) => Math.Sqrt((x * x) + (y * y));
    }
}
