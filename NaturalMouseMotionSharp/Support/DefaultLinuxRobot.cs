namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;

    internal class DefaultLinuxRobot : IRobot
    {
        public void MouseMove(int x, int y) => throw new System.NotImplementedException();

        public Size GetScreenSize() => throw new System.NotImplementedException();

        public Point GetMouseLocation() => throw new System.NotImplementedException();
    }
}
