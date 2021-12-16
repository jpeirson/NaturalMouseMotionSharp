namespace NaturalMouseMotionSharp.Support
{
    public struct DoublePoint
    {
        public static readonly DoublePoint Zero = new DoublePoint(0, 0);

        public DoublePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; }

        public double Y { get; }
    }
}
