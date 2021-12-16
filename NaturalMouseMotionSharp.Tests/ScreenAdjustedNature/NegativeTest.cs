namespace NaturalMouseMotionSharp.Tests.ScreenAdjustedNature
{
    using System.Drawing;
    using System.Linq;
    using Api;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NaturalMouseMotionSharp.Util;
    using NSubstitute;
    using NUnit.Framework;
    using TestUtils;

    public class NegativeTest
    {
        private MouseMotionFactory factory;
        private MockMouse mouse;

        [SetUp]
        public void Setup()
        {
            var robot = Substitute.For<IRobot>();
            this.factory = new MouseMotionFactory(robot);
            this.factory = new MouseMotionFactory(robot);

            this.factory.Nature = new ScreenAdjustedNature(robot, new Size(1800, 1500), new Point(-1000, -1000));
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.mouse = new MockMouse(100, 100);
            this.factory.SystemCalls = new MockSystemCalls(this.mouse, 800, 500);
            this.factory.NoiseProvider = Substitute.For<INoiseProvider>();
            this.factory.DeviationProvider = Substitute.For<IDeviationProvider>();
            var mockSpeedManager = Substitute.For<ISpeedManager>();
            mockSpeedManager.GetFlowWithTime(Arg.Any<double>())
                .Returns(_ => new Pair<Flow, long>(new Flow(new[] { 100.0 }), 10));
            this.factory.SpeedManager = mockSpeedManager;
            this.factory.Random = new MockRandom(new[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });
            this.factory.MouseInfo = this.mouse;
        }

        [Test]
        public void TestOffsetAppliesToMouseMovement()
        {
            this.factory.Move(500, 100);

            var moves = this.mouse.GetMouseMovements();
            moves[0].Should().Be(new Point(100, 100));
            moves.Last().Should().Be(new Point(-500, -900));
        }


        [Test]
        public void testOffsetLimitScreenOnSmallSide()
        {
            // Try to move out of the specified screen
            this.factory.Move(-1, -1);

            var moves = this.mouse.GetMouseMovements();
            moves[0].Should().Be(new Point(100, 100));
            // Expect the offset to limit the mouse movement to -1000, -1000
            moves.Last().Should().Be(new Point(-1000, -1000));
        }
    }
}
