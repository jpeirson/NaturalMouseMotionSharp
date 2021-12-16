namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Collections.Generic;
    using Api;
    using Util;

    public class DefaultSpeedManager : ISpeedManager
    {
        private const double SMALL_DELTA = 10e-6;
        private readonly List<Flow> flows = new List<Flow>();

        public DefaultSpeedManager(ICollection<Flow> flows) => this.flows.AddRange(flows);

        public DefaultSpeedManager() :
            this(new[]
                {
                    new Flow(FlowTemplates.constantSpeed()), new Flow(FlowTemplates.variatingFlow()),
                    new Flow(FlowTemplates.interruptedFlow()), new Flow(FlowTemplates.interruptedFlow2()),
                    new Flow(FlowTemplates.slowStartupFlow()), new Flow(FlowTemplates.slowStartup2Flow()),
                    new Flow(FlowTemplates.adjustingFlow()), new Flow(FlowTemplates.jaggedFlow()),
                    new Flow(FlowTemplates.stoppingFlow())
                }
            )
        {
        }

        public long MouseMovementBaseTimeMs { get; set; } = 500;

        public Pair<Flow, long> GetFlowWithTime(double distance)
        {
            var time = this.MouseMovementBaseTimeMs + (MathUtil.Random() * this.MouseMovementBaseTimeMs);
            var flow = this.flows[(int)(MathUtil.Random() * this.flows.Count)];

            // Let's ignore waiting time, e.g 0's in flow, by increasing the total time
            // by the amount of 0's there are in the flow multiplied by the time each bucket represents.
            var timePerBucket = time / flow.GetFlowCharacteristics().Length;
            foreach (var bucket in flow.GetFlowCharacteristics())
            {
                if (Math.Abs(bucket - 0) < SMALL_DELTA)
                {
                    time += timePerBucket;
                }
            }

            return new Pair<Flow, long>(flow, (long)time);
        }
    }
}
