namespace NaturalMouseMotionSharp.Support
{
    using System;
    using Api;
    using Util;

    /// <summary>
    ///     Helper implementation of <see cref="ISpeedManager" /> that implements <see cref="ISpeedManager.GetFlowWithTime" />
    ///     as a configurable function.
    /// </summary>
    public class SpeedManager : ISpeedManager
    {
        private readonly Func<double, Pair<Flow, long>> getFlowWithTime;

        public SpeedManager(Func<double, Pair<Flow, long>> getFlowWithTime) => this.getFlowWithTime = getFlowWithTime;

        public Pair<Flow, long> GetFlowWithTime(double distance) => this.getFlowWithTime(distance);
    }
}
