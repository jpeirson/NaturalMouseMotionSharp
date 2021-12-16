namespace NaturalMouseMotionSharp.Tests.TestUtils
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Api;

    public class MockMouse : IMouseInfoAccessor
    {
        private readonly List<Point> mouseMovements = new List<Point>();

        public MockMouse() => this.mouseMovements.Add(Point.Empty);

        public MockMouse(int posX, int posY) => this.mouseMovements.Add(new Point(posX, posY));

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Point GetMousePosition() => this.mouseMovements.Last();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void mouseMove(int x, int y) => this.mouseMovements.Add(new Point(x, y));

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<Point> getMouseMovements() => new List<Point>(this.mouseMovements);
    }
}
