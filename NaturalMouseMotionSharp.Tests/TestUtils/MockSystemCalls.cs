namespace NaturalMouseMotionSharp.Tests.TestUtils
{
    using System.Drawing;
    using Api;

    public class MockSystemCalls : ISystemCalls
    {
        private readonly MockMouse mockMouse;
        private readonly int screenHeight;
        private readonly int screenWidth;

        public MockSystemCalls(MockMouse mockMouse, int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.mockMouse = mockMouse;
        }

        public long CurrentTimeMillis() => 0;

        public void Sleep(long time)
        {
            // Do nothing.
        }

        public Size GetScreenSize() => new Size(this.screenWidth, this.screenHeight);

        public void SetMousePosition(int x, int y) => this.mockMouse.mouseMove(x, y);
    }
}
