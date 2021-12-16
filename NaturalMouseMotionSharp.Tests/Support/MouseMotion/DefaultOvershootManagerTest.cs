namespace NaturalMouseMotionSharp.Tests.Support.MouseMotion
{
    using System;
    using System.Drawing;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NUnit.Framework;
    using TestUtils;

    public class DefaultOvershootManagerTest
    {
        [Test]
        public void ReturnsSetOvershootNumber()
        {
            Random random = new MockRandom(new[] { 0.1, 0.2, 0.3, 0.4, 0.5 });
            var manager = new DefaultOvershootManager(random);

            var overshoots = manager.GetOvershoots(new Flow(new double[] { 100 }), 200, 1000);
            overshoots.Should().Be(3);

            manager.Overshoots = 10;
            overshoots = manager.GetOvershoots(new Flow(new double[] { 100 }), 200, 1000);
            overshoots.Should().Be(10);
        }

        [Test]
        public void OvershootSizeDecreasesWithOvershootsRemaining()
        {
            Point overshoot1;
            Point overshoot2;
            Point overshoot3;

            {
                Random random = new MockRandom(new[] { 0.1 });
                var manager = new DefaultOvershootManager(random);
                overshoot1 = manager.GetOvershootAmount(1000, 500, 1000, 1);
            }

            {
                Random random = new MockRandom(new[] { 0.1 });
                var manager = new DefaultOvershootManager(random);
                overshoot2 = manager.GetOvershootAmount(1000, 500, 1000, 2);
            }

            {
                Random random = new MockRandom(new[] { 0.1 });
                var manager = new DefaultOvershootManager(random);
                overshoot3 = manager.GetOvershootAmount(1000, 500, 1000, 3);
            }

            (overshoot1.X * 3).Should().Be(overshoot3.X);
            (overshoot1.X * 2).Should().Be(overshoot2.X);
        }

        [Test]
        public void nextMouseMovementTimeIsBasedOnCurrentMouseMovementMs()
        {
            Random random = new MockRandom(new[] { 0.1, 0.2, 0.3, 0.4, 0.5 });
            var manager = new DefaultOvershootManager(random);

            {
                // DEFAULT VALUE
                var nextTime = manager.DeriveNextMouseMovementTimeMs(
                    (long)(DefaultOvershootManager.DefaultOvershootSpeedupDivider * 500), 3
                );
                nextTime.Should().Be(500);
            }

            {
                manager.OvershootSpeedupDivider = 2;
                var nextTime = manager.DeriveNextMouseMovementTimeMs(1000, 3);
                nextTime.Should().Be(500);
            }

            {
                manager.OvershootSpeedupDivider = 4;
                var nextTime = manager.DeriveNextMouseMovementTimeMs(1000, 3);
                nextTime.Should().Be(250);
            }
        }

        [Test]
        public void NextMouseMovementTimeHasMinValue()
        {
            Random random = new MockRandom(new[] { 0.1, 0.2, 0.3, 0.4, 0.5 });
            var manager = new DefaultOvershootManager(random);

            {
                manager.OvershootSpeedupDivider = 2;
                manager.MinOvershootMovementMs = 1500;
                var nextTime = manager.DeriveNextMouseMovementTimeMs(1000, 3);
                nextTime.Should().Be(1500);
            }
        }
    }
}
