namespace NaturalMouseMotionSharp.Util
{
    public class Pair<TX, TY>
    {
        public Pair(TX x, TY y)
        {
            this.X = x;
            this.Y = y;
        }

        public TX X { get; }
        public TY Y { get; }
    }
}
