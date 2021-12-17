namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;

    internal class DefaultWindowsRobot : IRobot
    {
        public void MouseMove(int x, int y) => throw new NotImplementedException();

        public Size GetScreenSize() => throw new NotImplementedException();

        public Point GetMouseLocation() => throw new NotImplementedException();
    }
}
