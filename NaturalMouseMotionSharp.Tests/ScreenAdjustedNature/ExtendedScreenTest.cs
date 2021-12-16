namespace NaturalMouseMotionSharp.Tests.ScreenAdjustedNature
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Api;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;
    using TestUtils;

    public class ExtendedScreenTest
    {
        private MouseMotionFactory factory;
        private MockMouse mouse;

        [SetUp]
        public void setup()
        {
            var robot = Substitute.For<IRobot>();
            robot.GetMouseLocation().ThrowsForAnyArgs<NotImplementedException>();
            robot.GetScreenSize().ThrowsForAnyArgs<NotImplementedException>();
            robot.WhenForAnyArgs(x => x.MouseMove(0, 0)).Do(_ => throw new NotImplementedException());

            this.factory = new MouseMotionFactory(robot)
            {
                Nature = new ScreenAdjustedNature(robot, new Size(1800, 1500), new Point(0, 0))
            };
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.mouse = new MockMouse(100, 100);
            this.factory.SystemCalls = new MockSystemCalls(this.mouse, 800, 500);
            this.factory.NoiseProvider = Substitute.For<INoiseProvider>();
            this.factory.DeviationProvider = Substitute.For<IDeviationProvider>();
            this.factory.SpeedManager = new MockSpeedManager();
            this.factory.Random = new MockRandom(new[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });
            this.factory.MouseInfo = this.mouse;
        }

        [Test]
        public void testScreenSizeIsExtended()
        {
            this.factory.Move(1800, 1500);

            var moves = this.mouse.getMouseMovements();
            moves[0].Should().Be(new Point(100, 100));
            moves.Last().Should().Be(new Point(1799, 1499));
        }
    }
}
