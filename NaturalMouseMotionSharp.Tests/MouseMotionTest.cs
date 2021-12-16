namespace NaturalMouseMotionSharp.Tests
{
    using System.Drawing;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NUnit.Framework;
    using Support;

    public class MouseMotionTest : MouseMotionTestBase
    {
        [Test]
        public void linearMotionNoOvershoots()
        {
            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.factory.Move(50, 50);
            this.assertMousePosition(50, 50);

            var points = this.mouse.getMouseMovements();
            // The chosen 5 is 'good enough value' for 0,0 -> 50,50 for this test. we don't expect it to
            // be any certain value, because it can be changed in the future how the implementation actually works,
            // but based on gut feeling anything below 5 is too low.
            points.Should().HaveCountGreaterThan(5);
            // We don't want to verify every pixel what the mouse visits
            // instead we make sure its path is linear, as this is what we can expect from this test.
            var lastPoint = new Point();
            foreach (var p in points)
            {
                p.Y.Should().Be(p.X);
                p.X.Should().BeGreaterOrEqualTo(lastPoint.X, "p.x  = " + p.X + " lastPoint.x = " + lastPoint.X);
                p.Y.Should().BeGreaterOrEqualTo(lastPoint.Y, "p.y  = " + p.Y + " lastPoint.y = " + lastPoint.Y);
                lastPoint = p;
            }
        }

        [Test]
        public void cantMoveOutOfScreenToNegative_noOverShoots()
        {
            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.factory.Move(-50, -50);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.X.Should().BeGreaterOrEqualTo(0);
                p.Y.Should().BeGreaterOrEqualTo(0);
                this.assertMousePosition(0, 0);
            }
        }

        [Test]
        public void cantMoveUpToScreenWidth_noOvershoots()
        {
            // This helps to make sure that the test detects if used height instead of width or vice versa in implementation
            SCREEN_HEIGHT.Should().NotBe(SCREEN_WIDTH);

            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.factory.Move(SCREEN_WIDTH + 100, SCREEN_HEIGHT - 100);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.X.Should().BeLessThan(SCREEN_WIDTH);
            }

            this.assertMousePosition(SCREEN_WIDTH - 1, SCREEN_HEIGHT - 100);
        }

        [Test]
        public void cantMoveUpToScreenWidth_withOvershoots()
        {
            // This helps to make sure that the test detects if used height instead of width or vice versa in implementation
            SCREEN_HEIGHT.Should().NotBe(SCREEN_WIDTH);

            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 100;
            this.factory.Move(SCREEN_WIDTH - 1, SCREEN_HEIGHT - 100);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.X.Should().BeLessThan(SCREEN_WIDTH);
                this.assertMousePosition(SCREEN_WIDTH - 1, SCREEN_HEIGHT - 100);
            }
        }

        [Test]
        public void cantMoveUpToScreenHeight_noOvershoots()
        {
            // This helps to make sure that the test detects if used height instead of width or vice versa in implementation
            SCREEN_HEIGHT.Should().NotBe(SCREEN_WIDTH);

            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 0;
            this.factory.Move(SCREEN_WIDTH - 100, SCREEN_HEIGHT + 100);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.Y.Should().BeLessThan(SCREEN_HEIGHT);
                this.assertMousePosition(SCREEN_WIDTH - 100, SCREEN_HEIGHT - 1);
            }
        }

        [Test]
        public void cantMoveUpToScreenHeight_withOvershoots()
        {
            // This helps to make sure that the test detects if used height instead of width or vice versa in implementation
            SCREEN_HEIGHT.Should().NotBe(SCREEN_WIDTH);

            this.assertMousePosition(0, 0);
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 100;
            this.factory.Move(SCREEN_WIDTH - 100, SCREEN_HEIGHT - 1);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.Y.Should().BeLessThan(SCREEN_HEIGHT);
                this.assertMousePosition(SCREEN_WIDTH - 100, SCREEN_HEIGHT - 1);
            }
        }

        [Test]
        public void cantMoveOutOfScreenToNegative_withOverShoots()
        {
            // setup mouse to 50,50
            this.mouse.mouseMove(50, 50);
            this.assertMousePosition(50, 50);

            // Moving mouse to 0,0 with large amount of overshoots, so it would be likely to hit negative if possible.
            ((DefaultOvershootManager)this.factory.OvershootManager).Overshoots = 100;
            this.factory.Move(0, 0);

            var points = this.mouse.getMouseMovements();
            foreach (var p in points)
            {
                p.X.Should().BeGreaterOrEqualTo(0);
                p.Y.Should().BeGreaterOrEqualTo(0);
            }

            this.assertMousePosition(0, 0);
        }
    }
}
