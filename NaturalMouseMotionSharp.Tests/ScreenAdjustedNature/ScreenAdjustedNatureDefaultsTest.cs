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

    /// <summary>
    ///     This should run same tests as ScreenAdjustedNatureTest with difference in setup.
    ///     These tests verify that the offsets and dimensions are properly set when user does not explicitly
    ///     set MouseInfoAccessor and SystemCalls, but rely on the default version in DefaultMouseMotionNature
    /// </summary>
    [Ignore("Don't know how to interpret the Java mockito code here")]
    public class ScreenAdjustedNatureDefaultsTest
    {
        private MouseMotionFactory factory;
        private MockMouse mouse;

        [SetUp]
        public void Setup()
        {
            var robot = Substitute.For<IRobot>();
            this.mouse = new MockMouse(60, 60);
            var mockSystemCalls = Substitute.For<MockSystemCalls>(this.mouse, 800, 500);

            // TODO: not sure how to port this yet
            /*
            Answer<?> mouseAnswer = invocationOnMock -> {
              Method method = mouse.getClass().getMethod(invocationOnMock.getMethod().getName(), invocationOnMock.getMethod().getParameterTypes());
              return method.invoke(mouse, invocationOnMock.getArguments());
            };
        
            Answer<?> systemCallAnswer = invocationOnMock -> {
              Method method = mockSystemCalls.getClass().getMethod(invocationOnMock.getMethod().getName(), invocationOnMock.getMethod().getParameterTypes());
              return method.invoke(mockSystemCalls, invocationOnMock.getArguments());
            };
        
            try (
                // Not sure how to proxy constructor mocks to returns mock objects directly, but this ugly workaround seems to work as well.
                MockedConstruction<DefaultMouseInfoAccessor> mouseConstruction = mockConstruction(DefaultMouseInfoAccessor.class, (mock, context) -> {
                  // Direct all calls to 'mouse'
                  when(mock.getMousePosition()).thenAnswer(mouseAnswer);
                });
                MockedConstruction<DefaultSystemCalls> mockedDefaultSystemCalls = mockConstruction(DefaultSystemCalls.class, (mock, context) -> {
                  // Direct all calls to 'mockSystemCalls'
                  when(mock.currentTimeMillis()).thenAnswer(systemCallAnswer);
                  when(mock.getScreenSize()).thenAnswer(systemCallAnswer);
                  Mockito.doAnswer(systemCallAnswer).when(mock).setMousePosition(anyInt(), anyInt());
                });
        
            ) {
         */

            this.factory = new MouseMotionFactory(robot);
            this.factory.Nature = new ScreenAdjustedNature(robot, new Size(100, 100), new Point(50, 50));
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.factory.NoiseProvider = Substitute.For<INoiseProvider>();
            this.factory.DeviationProvider = Substitute.For<IDeviationProvider>();
            var mockSpeedManager = Substitute.For<ISpeedManager>();
            mockSpeedManager.GetFlowWithTime(Arg.Any<double>())
                .Returns(_ => new Pair<Flow, long>(new Flow(new[] { 100.0 }), 10));
            this.factory.SpeedManager = mockSpeedManager;
            this.factory.Random = new MockRandom(new[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });
        }

        [Test]
        public void TestOffsetAppliesToMouseMovement()
        {
            this.factory.Move(50, 50);

            var moves = this.mouse.GetMouseMovements();
            moves.First().Should().Be(new Point(60, 60));
            moves.Last().Should().Be(new Point(100, 100));
            var lastPos = new Point(0, 0);
            foreach (var p in moves)
            {
                lastPos.X.Should().BeLessThan(p.X, lastPos.X + " vs " + p.X);
                lastPos.Y.Should().BeLessThan(p.Y, lastPos.Y + " vs " + p.Y);
                lastPos = p;
            }
        }

        [Test]
        public void testDimensionsLimitScreenOnLargeSide()
        {
            // Arbitrary large movement attempt: (60, 60) -> (1060, 1060)
            this.factory.Move(1000, 1000);

            var moves = this.mouse.GetMouseMovements();

            moves.First().Should().Be(new Point(60, 60));
            // Expect the screen size to be only 100x100px, so it gets capped on 150, 150.
            // But NaturalMouseMotion allows to move to screen length - 1, so it's [149, 149]
            moves.Last().Should().Be(new Point(149, 149));
        }

        [Test]
        public void testOffsetLimitScreenOnSmallSide()
        {
            // Try to move out of the specified screen
            this.factory.Move(-1, -1);

            var moves = this.mouse.GetMouseMovements();
            moves.First().Should().Be(new Point(60, 60));
            // Expect the offset to limit the mouse movement to 50, 50
            moves.Last().Should().Be(new Point(50, 50));
        }
    }
}
