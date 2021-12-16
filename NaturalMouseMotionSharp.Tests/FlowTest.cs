namespace NaturalMouseMotionSharp.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NaturalMouseMotionSharp.Support;
    using NUnit.Framework;
    using Support;

    public class FlowTest
    {
        private static readonly double SmallDelta = 10e-6;

        [Test]
        public void ConstantCharacteristicsGetNormalizedTo100()
        {
            var characteristics = Enumerable.Repeat(500d, 100).ToArray();
            var flow = new Flow(characteristics);

            var result = flow.GetFlowCharacteristics();
            double sum = 0;
            for (var i = 0; i < result.Length; i++)
            {
                result[i].Should().BeApproximately(100, SmallDelta);
                sum += result[i];
            }

            sum.Should().BeApproximately(100 * characteristics.Length, SmallDelta);
        }

        [Test]
        public void ConstantCharacteristicsGetNormalizedTo100WithLargeArray()
        {
            var characteristics = Enumerable.Repeat(500d, 1000).ToArray();
            var flow = new Flow(characteristics);

            var result = flow.GetFlowCharacteristics();
            double sum = 0;
            for (var i = 0; i < result.Length; i++)
            {
                result[i].Should().BeApproximately(100, SmallDelta);
                sum += result[i];
            }

            sum.Should().BeApproximately(100 * characteristics.Length, SmallDelta);
        }

        [Test]
        public void ConstantCharacteristicsGetNormalizedTo100FromLowValues()
        {
            var characteristics = Enumerable.Repeat(5.0, 100).ToArray();
            var flow = new Flow(characteristics);

            var result = flow.GetFlowCharacteristics();
            double sum = 0;
            for (var i = 0; i < result.Length; i++)
            {
                result[i].Should().BeApproximately(100, SmallDelta);
                sum += result[i];
            }

            sum.Should().BeApproximately(100 * characteristics.Length, SmallDelta);
        }

        [Test]
        public void CharacteristicsGetNormalizedToAverage100()
        {
            double[] characteristics = { 1, 2, 3, 4, 5 };

            var flow = new Flow(characteristics);

            var result = flow.GetFlowCharacteristics();
            double sum = 0;
            for (var i = 0; i < result.Length; i++)
            {
                sum += result[i];
            }

            result[0].Should().BeApproximately(33.33333333d, SmallDelta);
            result[1].Should().BeApproximately(66.66666666d, SmallDelta);
            result[2].Should().BeApproximately(100.00000000d, SmallDelta);
            result[3].Should().BeApproximately(133.33333333d, SmallDelta);
            result[4].Should().BeApproximately(166.66666666d, SmallDelta);

            sum.Should().BeApproximately(100 * characteristics.Length, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_accelerating()
        {
            double[] characteristics = { 1, 2, 3, 4, 5 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 5, 0);
            var step2 = flow.GetStepSize(100, 5, 0.2);
            var step3 = flow.GetStepSize(100, 5, 0.4);
            var step4 = flow.GetStepSize(100, 5, 0.6);
            var step5 = flow.GetStepSize(100, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_decelerating()
        {
            double[] characteristics = { 5, 4, 3, 2, 1 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 5, 0);
            var step2 = flow.GetStepSize(100, 5, 0.2);
            var step3 = flow.GetStepSize(100, 5, 0.4);
            var step4 = flow.GetStepSize(100, 5, 0.6);
            var step5 = flow.GetStepSize(100, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_characteristics_not_dividable_by_steps_1()
        {
            double[] characteristics = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 5, 0);
            var step2 = flow.GetStepSize(100, 5, 0.2);
            var step3 = flow.GetStepSize(100, 5, 0.4);
            var step4 = flow.GetStepSize(100, 5, 0.6);
            var step5 = flow.GetStepSize(100, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_characteristics_not_dividable_by_steps_2()
        {
            double[] characteristics = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 5, 0);
            var step2 = flow.GetStepSize(100, 5, 0.2);
            var step3 = flow.GetStepSize(100, 5, 0.4);
            var step4 = flow.GetStepSize(100, 5, 0.6);
            var step5 = flow.GetStepSize(100, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_characteristics_not_dividable_by_steps_3()
        {
            double[] characteristics = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 3, 0);
            var step2 = flow.GetStepSize(100, 3, 1d / 3d);
            var step3 = flow.GetStepSize(100, 3, 1d / 3d * 2);
            var sum = step1 + step2 + step3;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_characteristics_array_smaller_than_steps_not_dividable()
        {
            double[] characteristics = { 1, 2, 3 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(100, 5, 0);
            var step2 = flow.GetStepSize(100, 5, 0.2);
            var step3 = flow.GetStepSize(100, 5, 0.4);
            var step4 = flow.GetStepSize(100, 5, 0.6);
            var step5 = flow.GetStepSize(100, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(100d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_constantFlow()
        {
            double[] characteristics = { 10, 10, 10, 10, 10 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(500, 5, 0);
            var step2 = flow.GetStepSize(500, 5, 0.2);
            var step3 = flow.GetStepSize(500, 5, 0.4);
            var step4 = flow.GetStepSize(500, 5, 0.6);
            var step5 = flow.GetStepSize(500, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(500d, SmallDelta);
        }

        [Test]
        public void StepsAddUpToDistance_constantFlow_characteristics_to_steps_not_dividable()
        {
            double[] characteristics = { 10, 10, 10, 10, 10, 10 };
            var flow = new Flow(characteristics);
            var step1 = flow.GetStepSize(500, 5, 0);
            var step2 = flow.GetStepSize(500, 5, 0.2);
            var step3 = flow.GetStepSize(500, 5, 0.4);
            var step4 = flow.GetStepSize(500, 5, 0.6);
            var step5 = flow.GetStepSize(500, 5, 0.8);
            var sum = step1 + step2 + step3 + step4 + step5;
            sum.Should().BeApproximately(500d, SmallDelta);
        }
    }
}
