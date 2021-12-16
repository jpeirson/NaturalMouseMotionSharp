namespace NaturalMouseMotionSharp.Tests.Util
{
    using FluentAssertions;
    using NaturalMouseMotionSharp.Util;
    using NUnit.Framework;

    public class MathUtilTest
    {
        [Test]
        public void roundTowards_lowValueLowerThanTarget()
        {
            var result = MathUtil.RoundTowards(0.3, 1);
            result.Should().Be(1);
        }

        [Test]
        public void roundTowards_lowValueHigherThanTarget()
        {
            var result = MathUtil.RoundTowards(0.3, 0);
            result.Should().Be(0);
        }

        [Test]
        public void roundTowards_highValueHigherThanTarget()
        {
            var result = MathUtil.RoundTowards(2.9, 2);
            result.Should().Be(2);
        }

        [Test]
        public void roundTowards_highValueLowerThanTarget()
        {
            var result = MathUtil.RoundTowards(2.9, 3);
            result.Should().Be(3);
        }

        [Test]
        public void roundTowards_valueEqualToTarget()
        {
            var result = MathUtil.RoundTowards(2.0, 2);
            result.Should().Be(2);
        }

        [Test]
        public void roundTowards_valueExactlyOneBiggerToLowerTarget()
        {
            var result = MathUtil.RoundTowards(3.0, 2);
            result.Should().Be(3);
        }

        [Test]
        public void roundTowards_valueExactlyOneSmallerToHigherTarget()
        {
            var result = MathUtil.RoundTowards(1.0, 2);
            result.Should().Be(1);
        }

        [Test]
        public void roundTowards_specialHighNumberToHigherTarget()
        {
            // 99.99999999999999
            var hundred_low = 111 / 1.11;
            var result = MathUtil.RoundTowards(hundred_low, 100);
            result.Should().Be(100);
        }

        [Test]
        public void roundTowards_specialHighNumberToLowerTarget()
        {
            // 99.99999999999999
            var hundred_low = 111 / 1.11;
            // It's very close to 101.
            var result = MathUtil.RoundTowards(hundred_low + 1, 100);
            result.Should().Be(100);
        }

        [Test]
        public void roundTowards_specialLowNumberToHigherTarget()
        {
            // 1.4210854715202004E-14
            var high_zero = 100 - (111 / 1.11);
            var result = MathUtil.RoundTowards(5 + high_zero, 6);
            result.Should().Be(6);
        }

        [Test]
        public void roundTowards_specialLowNumberToLowerTarget()
        {
            // 1.4210854715202004E-14
            var high_zero = 100 - (111 / 1.11);
            var result = MathUtil.RoundTowards(5 + high_zero, 5);
            result.Should().Be(5);
        }
    }
}
