namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;

    public interface IRobot
    {
        void mouseMove(int x, int y);
        Size getScreenSize();
        Point getMouseLocation();
    }
}
