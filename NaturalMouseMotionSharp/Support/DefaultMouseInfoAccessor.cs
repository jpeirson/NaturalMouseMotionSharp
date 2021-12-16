namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;
    using Api;

    public class DefaultMouseInfoAccessor : IMouseInfoAccessor
    {
        private readonly IRobot robot;

        public DefaultMouseInfoAccessor(IRobot robot) => this.robot = robot;

        public Point GetMousePosition() => this.robot.getMouseLocation();
    }
}
