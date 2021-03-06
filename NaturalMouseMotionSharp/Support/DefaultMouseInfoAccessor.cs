namespace NaturalMouseMotionSharp.Support
{
    using System.Drawing;
    using Api;

    public class DefaultMouseInfoAccessor : IMouseInfoAccessor
    {
        private readonly IRobot robot;

        public DefaultMouseInfoAccessor() : this(new DefaultRobot())
        {
        }

        public DefaultMouseInfoAccessor(IRobot robot) => this.robot = robot;

        public virtual Point GetMousePosition() => this.robot.GetMouseLocation();
    }
}
