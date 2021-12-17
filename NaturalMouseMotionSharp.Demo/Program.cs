namespace NaturalMouseMotionSharp.Demo
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Support;
    using Util;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var log = new SimpleConsoleLogger(LogLevel.Debug);

            var robot = new DefaultRobot();
            var templateName = args.ElementAtOrDefault(0);
            var factory = templateName?.ToLowerInvariant() switch
            {
                "granny" => FactoryTemplates.CreateGrannyMotionFactory(robot),
                "demorobot" => FactoryTemplates.CreateDemoRobotMotionFactory(robot, 100),
                "fastgamer" => FactoryTemplates.CreateFastGamerMotionFactory(robot),
                "averagecomputeruser" => FactoryTemplates.CreateAverageComputerUserMotionFactory(robot),
                null => FactoryTemplates.CreateGrannyMotionFactory(robot),
                _ => throw new ArgumentException($"Unknown factory template: {templateName}")
            };

            var iterations = 1;
            if (args.Length >= 2)
            {
                iterations = int.Parse(args[1]);
            }

            var screenSize = robot.GetScreenSize();
            log.LogDebug("ScreenSize: {size}", screenSize);
            log.LogDebug("MouseLocation: {mouse}", robot.GetMouseLocation());

            for (var i = 0; i < iterations; i++)
            {
                log.LogInformation("Iteration {i} of {iterations}", i + 1, iterations);
                factory.Move((int)(factory.Random.NextDouble() * screenSize.Width),
                    (int)(factory.Random.NextDouble() * screenSize.Height), log);
            }
        }
    }
}
