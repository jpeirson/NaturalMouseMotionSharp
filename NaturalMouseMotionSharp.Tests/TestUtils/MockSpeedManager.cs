namespace NaturalMouseMotionSharp.Tests.TestUtils
{
    using Api;
    using NaturalMouseMotionSharp.Support;
    using NaturalMouseMotionSharp.Util;

    public class MockSpeedManager : ISpeedManager
    {
        public Pair<Flow, long> GetFlowWithTime(double distance)
        {
            double[] characteristics = { 100 };
            return new Pair<Flow, long>(new Flow(characteristics), 10L);
        }
    }
}
