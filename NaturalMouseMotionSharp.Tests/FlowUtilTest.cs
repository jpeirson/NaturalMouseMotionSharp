namespace NaturalMouseMotionSharp.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Util;

    public class FlowUtilTest
    {
        private static readonly double SMALL_DELTA = 10e-6;

        [Test]
        public void testStretchFlow_3to9()
        {
            double[] flow = { 1, 2, 3 };
            var result = FlowUtil.StretchFlow(flow, 9);
            result.Should().Equal(
                new[] { 1.0, 1.25, 1.5, 1.75, 2.0, 2.25, 2.5, 2.75, 3.0 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 9, result);
        }

        [Test]
        public void testStretchFlow_1to9()
        {
            double[] flow = { 1 };
            var result = FlowUtil.StretchFlow(flow, 9);
            result.Should().Equal(
                new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 9, result);
        }

        [Test]
        public void testStretchFlow_3to5()
        {
            double[] flow = { 1, 2, 3 };
            var result = FlowUtil.StretchFlow(flow, 5);
            result.Should().Equal(
                new[] { 1.0, 1.5, 2.0, 2.5, 3 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 5, result);
        }

        [Test]
        public void testStretchFlow_3to5_withModifier()
        {
            double[] flow = { 1, 2, 3 };
            Func<double, double> modifier = value => value * 2;
            var result = FlowUtil.StretchFlow(flow, 5, modifier);
            result.Should().Equal(
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 2 * 5, result);
        }

        [Test]
        public void testStretchFlow_3to6_withModifier()
        {
            double[] flow = { 1, 2, 3 };
            Func<double, double> modifier = Math.Floor;
            var result = FlowUtil.StretchFlow(flow, 6, modifier);
            result.Should().Equal(
                new double[] { 1, 1, 1, 2, 2, 2 }, ApproxEqual
            );
        }

        [Test]
        public void testStretchFlow_2to9()
        {
            double[] flow = { 1, 2 };
            var result = FlowUtil.StretchFlow(flow, 9);
            result.Should().Equal(
                new[] { 1.0, 1.125, 1.25, 1.375, 1.5, 1.625, 1.75, 1.875, 2.0 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 9, result);
        }

        [Test]
        public void testStretchFlow_2to8()
        {
            double[] flow = { 1, 2 };
            var result = FlowUtil.StretchFlow(flow, 8);

            result.Should().Equal(
                new[] { 1.0, 1.142857, 1.285714, 1.428571, 1.571428, 1.714285, 1.857142, 2.0 }, ApproxEqual);

            this.assertArraySum(this.average(flow) * 8, result);
        }

        [Test]
        public void testStretchFlow_3to6()
        {
            double[] flow = { 1, 2, 3 };
            var result = FlowUtil.StretchFlow(flow, 6);
            result.Should().Equal(
                new[] { 1.047619, 1.428571, 1.809523, 2.190476, 2.571428, 2.952380 }, ApproxEqual);

            this.assertArraySum(this.average(flow) * 6, result);
        }


        [Test]
        public void testStretchFlow_3to18()
        {
            double[] flow = { 1.1, 1.2, 1.3 };
            var result = FlowUtil.StretchFlow(flow, 18);
            result.Should().Equal(
                new[]
                {
                    1.102795, 1.113978, 1.125161, 1.136774, 1.148602, 1.159784, 1.170967, 1.183010, 1.194408,
                    1.205591, 1.216989, 1.229032, 1.240215, 1.251397, 1.263225, 1.274838, 1.286021, 1.297204
                }, ApproxEqual);

            this.assertArraySum(this.average(flow) * 18, result);
        }


        [Test]
        public void testReduceFlow_5to3()
        {
            double[] flow = { 1, 1.5, 2, 2.5, 3 };
            var result = FlowUtil.ReduceFlow(flow, 3);
            result.Should().Equal(
                new[] { 1.2, 2, 2.8 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 3, result);
        }

        [Test]
        public void testReduceFlow_10to3()
        {
            double[] flow = { 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
            var result = FlowUtil.ReduceFlow(flow, 3);
            result.Should().Equal(
                new[] { 4.6, 3.0, 1.4 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 3, result);
        }

        [Test]
        public void testReduceFlow_10to1()
        {
            double[] flow = { 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
            var result = FlowUtil.ReduceFlow(flow, 1);
            result.Should().Equal(
                new[] { 3.0 }, ApproxEqual
            );

            this.assertArraySum(this.average(flow) * 1, result);
        }

        private void assertArraySum(double expected, double[] actual) =>
            actual.Sum().Should().BeApproximately(expected, SMALL_DELTA);

        private double average(double[] array) => array.Average();

        private static bool ApproxEqual(double x, double y) => Math.Abs(x - y) <= SMALL_DELTA;
    }
}
