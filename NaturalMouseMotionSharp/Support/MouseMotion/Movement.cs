namespace NaturalMouseMotionSharp.Support.MouseMotion
{
    public class Movement
    {
        public Movement(int destX, int destY, double distance, int xDistance, int yDistance, long time, Flow flow)
        {
            this.DestX = destX;
            this.DestY = destY;
            this.Distance = distance;
            this.XDistance = xDistance;
            this.YDistance = yDistance;
            this.Time = time;
            this.Flow = flow;
        }

        public int DestX { get; }

        public int DestY { get; }

        public double Distance { get; }

        public Flow Flow { get; }

        public long Time { get; }

        public int XDistance { get; }

        public int YDistance { get; }

        public override string ToString() =>
            "Movement{" +
            "destX=" + this.DestX +
            ", destY=" + this.DestY +
            ", xDistance=" + this.XDistance +
            ", yDistance=" + this.YDistance +
            ", time=" + this.Time +
            '}';
    }
}
