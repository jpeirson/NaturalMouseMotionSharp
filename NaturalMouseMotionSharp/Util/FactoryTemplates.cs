namespace NaturalMouseMotionSharp.Util
{
    using System.Collections.Generic;
    using Api;
    using Support;

    public static class FactoryTemplates
    {
        /// <summary>
        ///     <h1>Stereotypical granny using a computer with non-optical mouse from the 90s.</h1>
        ///     Low speed, variating flow, lots of noise in movement.
        /// </summary>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateGrannyMotionFactory(IRobot robot) =>
            CreateGrannyMotionFactory(new DefaultMouseMotionNature(robot));

        /// <summary>
        ///     <h1>Stereotypical granny using a computer with non-optical mouse from the 90s.</h1>
        ///     Low speed, variating flow, lots of noise in movement.
        /// </summary>
        /// <param name="nature">the nature for the template to be configured on</param>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateGrannyMotionFactory(MouseMotionNature nature)
        {
            var factory = new MouseMotionFactory(nature);
            var flows = new List<Flow>(new[]
                {
                    new Flow(FlowTemplates.JaggedFlow()), new Flow(FlowTemplates.Random()),
                    new Flow(FlowTemplates.InterruptedFlow()), new Flow(FlowTemplates.InterruptedFlow2()),
                    new Flow(FlowTemplates.AdjustingFlow()), new Flow(FlowTemplates.StoppingFlow())
                }
            );
            var manager = new DefaultSpeedManager(flows);
            factory.DeviationProvider = new SinusoidalDeviationProvider(9);
            factory.NoiseProvider = new DefaultNoiseProvider(1.6);
            factory.Nature.ReactionTimeBaseMs = 100;

            var overshootManager = (DefaultOvershootManager)factory.OvershootManager;
            overshootManager.Overshoots = 3;
            overshootManager.MinDistanceForOvershoots = 3;
            overshootManager.MinOvershootMovementMs = 400;
            overshootManager.OvershootSpeedupDivider =
                DefaultOvershootManager.DefaultOvershootRandomModifierDivider / 2f;
            overshootManager.OvershootSpeedupDivider = DefaultOvershootManager.DefaultOvershootSpeedupDivider * 2;

            factory.Nature.TimeToStepsDivider = DefaultMouseMotionNature.DefaultTimeToStepsDivider - 2;
            manager.MouseMovementBaseTimeMs = 1000;
            factory.SpeedManager = manager;
            return factory;
        }

        /// <summary>
        ///     <h1>Robotic fluent movement.</h1>
        ///     Custom speed, constant movement, no mistakes, no overshoots.
        /// </summary>
        /// <param name="robot">Automation object</param>
        /// <param name="motionTimeMsPer100Pixels">approximate time a movement takes per 100 pixels of travelling</param>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateDemoRobotMotionFactory(IRobot robot, long motionTimeMsPer100Pixels) =>
            CreateDemoRobotMotionFactory(new DefaultMouseMotionNature(robot), motionTimeMsPer100Pixels);

        /// <summary>
        ///     <h1>Robotic fluent movement.</h1>
        ///     Custom speed, constant movement, no mistakes, no overshoots.
        /// </summary>
        /// <param name="nature">the nature for the template to be configured on</param>
        /// <param name="motionTimeMsPer100Pixels">approximate time a movement takes per 100 pixels of travelling</param>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateDemoRobotMotionFactory(
            MouseMotionNature nature, long motionTimeMsPer100Pixels
        )
        {
            var factory = new MouseMotionFactory(nature);
            var flow = new Flow(FlowTemplates.ConstantSpeed());
            var timePerPixel = motionTimeMsPer100Pixels / 100d;
            var manager = new SpeedManager(distance => new Pair<Flow, long>(flow, (long)(timePerPixel * distance)));
            factory.DeviationProvider =
                new DeviationProvider((totalDistanceInPixels, completionFraction) => DoublePoint.Zero);
            factory.NoiseProvider = new NoiseProvider((random, xStepSize, yStepSize) => DoublePoint.Zero);

            var overshootManager = (DefaultOvershootManager)factory.OvershootManager;
            overshootManager.Overshoots = 0;

            factory.SpeedManager = manager;
            return factory;
        }

        /// <summary>
        ///     <h1>Gamer with fast reflexes and quick mouse movements.</h1>
        ///     Quick movement, low noise, some deviation, lots of overshoots.
        /// </summary>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateFastGamerMotionFactory(IRobot robot) =>
            CreateFastGamerMotionFactory(new DefaultMouseMotionNature(robot));

        /// <summary>
        ///     <h1>Gamer with fast reflexes and quick mouse movements.</h1>
        ///     Quick movement, low noise, some deviation, lots of overshoots.
        /// </summary>
        /// <param name="nature">the nature for the template to be configured on</param>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateFastGamerMotionFactory(MouseMotionNature nature)
        {
            var factory = new MouseMotionFactory(nature);
            var flows = new List<Flow>(new[]
            {
                new Flow(FlowTemplates.VariatingFlow()), new Flow(FlowTemplates.SlowStartupFlow()),
                new Flow(FlowTemplates.SlowStartup2Flow()), new Flow(FlowTemplates.AdjustingFlow()),
                new Flow(FlowTemplates.JaggedFlow())
            });
            var manager = new DefaultSpeedManager(flows);
            factory.DeviationProvider =
                new SinusoidalDeviationProvider(SinusoidalDeviationProvider.DefaultSlopeDivider);
            factory.NoiseProvider = new DefaultNoiseProvider(DefaultNoiseProvider.DefaultNoisinessDivider);
            factory.Nature.ReactionTimeVariationMs = 100;
            manager.MouseMovementBaseTimeMs = 250;

            var overshootManager = (DefaultOvershootManager)factory.OvershootManager;
            overshootManager.Overshoots = 4;

            factory.SpeedManager = manager;
            return factory;
        }

        /// <summary>
        ///     <h1>Standard computer user with average speed and movement mistakes</h1>
        ///     medium noise, medium speed, medium noise and deviation.
        /// </summary>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateAverageComputerUserMotionFactory(IRobot robot) =>
            CreateAverageComputerUserMotionFactory(new DefaultMouseMotionNature(robot));

        /// <summary>
        ///     <h1>Standard computer user with average speed and movement mistakes</h1>
        ///     medium noise, medium speed, medium noise and deviation.
        /// </summary>
        /// <param name="nature">the nature for the template to be configured on</param>
        /// <returns>the factory</returns>
        public static MouseMotionFactory CreateAverageComputerUserMotionFactory(MouseMotionNature nature)
        {
            var factory = new MouseMotionFactory(nature);
            var flows = new List<Flow>(new[]
            {
                new Flow(FlowTemplates.VariatingFlow()), new Flow(FlowTemplates.InterruptedFlow()),
                new Flow(FlowTemplates.InterruptedFlow2()), new Flow(FlowTemplates.SlowStartupFlow()),
                new Flow(FlowTemplates.SlowStartup2Flow()), new Flow(FlowTemplates.AdjustingFlow()),
                new Flow(FlowTemplates.JaggedFlow()), new Flow(FlowTemplates.StoppingFlow())
            });
            var manager = new DefaultSpeedManager(flows);
            factory.DeviationProvider =
                new SinusoidalDeviationProvider(SinusoidalDeviationProvider.DefaultSlopeDivider);
            factory.NoiseProvider = new DefaultNoiseProvider(DefaultNoiseProvider.DefaultNoisinessDivider);
            factory.Nature.ReactionTimeVariationMs = 110;
            manager.MouseMovementBaseTimeMs = 400;

            var overshootManager = (DefaultOvershootManager)factory.OvershootManager;
            overshootManager.Overshoots = 4;

            factory.SpeedManager = manager;
            return factory;
        }
    }
}
