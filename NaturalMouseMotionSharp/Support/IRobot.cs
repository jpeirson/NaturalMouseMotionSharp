namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;

    public interface IRobot
    {
        void MouseMove(int x, int y);
        Size GetScreenSize();
        Point GetMouseLocation();
    }
}
