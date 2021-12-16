namespace NaturalMouseMotionSharp.Util
{
    using System;
    using System.Linq;

    public static class FlowUtil
    {
        /// <summary>Stretch flow to longer length. Tries to fill the caps with averages.</summary>
        /// <param name="flow">the original flow</param>
        /// <param name="targetLength">the resulting flow length</param>
        /// <returns>the resulting flow</returns>
        /// <remarks>
        ///     This is an unintuitive method, because it turns out that, for example, array size of 3
        ///     scales better to array size of 5 than it does to array size of 6. [1, 2, 3] can be
        ///     easily scaled to [1, 1.5, 2, 2.5, 3], but it's not possible without recalculating middle number (2)
        ///     with array size of 6, simplistic solutions quickly would run to trouble like this  [1, 1.5, 2, 2.5, 3, (3)? ]
        ///     or maybe: [1, 1.5, 2, 2.5, ..., 3 ]. The correct solution would correctly scale the middle numbers
        /// </remarks>
        public static double[] StretchFlow(double[] flow, int targetLength) => StretchFlow(flow, targetLength, a => a);

        /// <summary>Stretch flow to longer length. Tries to fill the caps with averages.</summary>
        /// <param name="flow">the original flow</param>
        /// <param name="targetLength">the resulting flow length</param>
        /// <param name="modifier">
        ///     modifies the resulting values, you can use this to provide noise or amplify
        ///     the flow characteristics.
        /// </param>
        /// <returns>the resulting flow</returns>
        /// <remarks>
        ///     This is an unintuitive method, because it turns out that, for example, array size of 3
        ///     scales better to array size of 5 than it does to array size of 6. [1, 2, 3] can be
        ///     easily scaled to [1, 1.5, 2, 2.5, 3], but it's not possible without recalculating middle number (2)
        ///     with array size of 6, simplistic solutions quickly would run to trouble like this  [1, 1.5, 2, 2.5, 3, (3)? ]
        ///     or maybe: [1, 1.5, 2, 2.5, ..., 3 ]. The correct solution would correctly scale the middle numbers
        ///     over several indexes.
        /// </remarks>
        public static double[] StretchFlow(double[] flow, int targetLength, Func<double, double> modifier)
        {
            if (targetLength < flow.Length)
            {
                throw new InvalidOperationException("Target bucket length smaller than flow. " +
                                                    "" + targetLength + " vs " + flow.Length);
            }

            double[] result;
            var tempLength = targetLength;

            if (flow.Length != 1 && (tempLength - flow.Length) % (flow.Length - 1) != 0)
            {
                tempLength = ((flow.Length - 1) * (tempLength - flow.Length)) + 1;
            }

            result = new double[tempLength];
            var insider = flow.Length - 2;
            var stepLength = (int)((tempLength - 2) / (double)(insider + 1)) + 1;
            var countToNextStep = stepLength;
            var fillValueIndex = 0;
            for (var i = 0; i < tempLength; i++)
            {
                var fillValueBottom = flow[fillValueIndex];
                var fillValueTop = fillValueIndex + 1 < flow.Length ? flow[fillValueIndex + 1] : flow[fillValueIndex];

                var completion = (stepLength - countToNextStep) / (double)stepLength;

                result[i] = (fillValueBottom * (1 - completion)) + (fillValueTop * completion);

                countToNextStep--;

                if (countToNextStep == 0)
                {
                    countToNextStep = stepLength;
                    fillValueIndex++;
                }
            }

            if (tempLength != targetLength)
            {
                result = ReduceFlow(result, targetLength);
            }

            return result.Select(modifier).ToArray();
        }

        /// Reduction causes loss of information, so the resulting flow is always 'good enough', but is not guaranteed
        /// to be equivalent, just a shorter version of the original flow
        /// @param flow the original flow
        /// @param targetLength the resulting array length
        /// @return the resulting flow
        public static double[] ReduceFlow(double[] flow, int targetLength)
        {
            if (flow.Length <= targetLength)
            {
                throw new InvalidOperationException("Bad arguments [" + flow.Length + ", " + targetLength + "]");
            }

            var multiplier = targetLength / (double)flow.Length;
            var result = new double[targetLength];
            for (var i = 0; i < flow.Length; i++)
            {
                var index = i * multiplier;
                var untilIndex = (i + 1) * multiplier;
                var indexInt = (int)index;
                var untilIndexInt = (int)untilIndex;
                if (indexInt != untilIndexInt)
                {
                    var resultIndexPortion = 1 - (index - indexInt);
                    var nextResultIndexPortion = untilIndex - untilIndexInt;
                    result[indexInt] += flow[i] * resultIndexPortion;
                    if (untilIndexInt < result.Length)
                    {
                        result[untilIndexInt] += flow[i] * nextResultIndexPortion;
                    }
                }
                else
                {
                    result[indexInt] += flow[i] * (untilIndex - index);
                }
            }

            return result;
        }
    }
}
