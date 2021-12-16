namespace NaturalMouseMotionSharp.Tests
{
    using System;
    using Api;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
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

            this.mouse = new MockMouse();
            this.factory = new MouseMotionFactory(mockRobot);
            this.systemCalls = new MockSystemCalls(this.mouse, SCREEN_WIDTH, SCREEN_HEIGHT);
            this.deviationProvider = Substitute.For<IDeviationProvider>();
            this.noiseProvider = Substitute.For<INoiseProvider>();
            this.speedManager = new MockSpeedManager();
            this.random = new MockRandom(new[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });

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
