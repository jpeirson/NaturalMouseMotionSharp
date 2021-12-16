namespace NaturalMouseMotionSharp.Tests
{
    using System;
    using System.Drawing;
    using Api;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NaturalMouseMotionSharp.Util;
    using NSubstitute;
    using NUnit.Framework;
    using TestUtils;

    public class MouseMotionTestBase
    {
        protected static readonly double SMALL_DELTA = 0.000001;
        protected static readonly int SCREEN_WIDTH = 800;
        protected static readonly int SCREEN_HEIGHT = 500;
        protected IDeviationProvider deviationProvider;
        protected MouseMotionFactory factory;
        protected MockMouse mouse;
        protected INoiseProvider noiseProvider;
        protected Random random;
        protected ISpeedManager speedManager;
        protected ISystemCalls systemCalls;

        [SetUp]
        public void setup()
        {
            var mockRobot = Substitute.For<IRobot>();
            var mockMouse = new MockMouse();
            var mockSystemCalls = new MockSystemCalls(mockMouse, SCREEN_WIDTH, SCREEN_HEIGHT);
            var mockDeviationProvider = Substitute.For<IDeviationProvider>();
            var mockNoiseProvider = Substitute.For<INoiseProvider>();

            var mockSpeedManager = Substitute.For<ISpeedManager>();
            mockSpeedManager.GetFlowWithTime(Arg.Any<double>())
                .Returns(_ => new Pair<Flow, long>(new Flow(new[] { 100.0 }), 10));

            var mockRandom = new MockRandom(new[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });

            this.mouse = mockMouse;
            this.factory = new MouseMotionFactory(mockRobot);
            this.systemCalls = mockSystemCalls;
            this.deviationProvider = mockDeviationProvider;
            this.noiseProvider = mockNoiseProvider;
            this.speedManager = mockSpeedManager;
            this.random = mockRandom;

            this.factory.SystemCalls = this.systemCalls;
            this.factory.DeviationProvider = this.deviationProvider;
            this.factory.NoiseProvider = this.noiseProvider;
            this.factory.SpeedManager = this.speedManager;
            this.factory.Random = this.random;
            this.factory.MouseInfo = this.mouse;
        }

        protected void assertMousePosition(int x, int y)
        {
            var pos = this.mouse.GetMousePosition();
            pos.X.Should().Be(x);
            pos.Y.Should().Be(y);
        }
    }
}
