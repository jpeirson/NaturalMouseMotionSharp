namespace NaturalMouseMotionSharp.Demo
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Support;
    using Util;

    /// <summary>
    ///     Demo program that moves the cursor around on the main desktop screen.
    /// </summary>
    /// <remarks>
    ///     Arguments:
    ///     <ul>
    ///         <li>1st arg: factory template: Granny, DemoRobot, FastGamer, AverageComputerUser, or omit to use a default</li>
    ///         <li>2nd arg: iterations: number of times to create random mouse movements, or omit to use a default.</li>
    ///     </ul>
    /// </remarks>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var log = new SimpleConsoleLogger(LogLevel.Debug);

            var templateName = args.ElementAtOrDefault(0);
            var factory = templateName?.ToLowerInvariant() switch
            {
                "granny" => FactoryTemplates.CreateGrannyMotionFactory(),
                "demorobot" => FactoryTemplates.CreateDemoRobotMotionFactory(100),
                "fastgamer" => FactoryTemplates.CreateFastGamerMotionFactory(),
                "averagecomputeruser" => FactoryTemplates.CreateAverageComputerUserMotionFactory(),
                null => FactoryTemplates.CreateGrannyMotionFactory(),
                _ => throw new ArgumentException($"Unknown factory template: {templateName}")
            };

            var iterations = 1;
            if (args.Length >= 2)
            {
                iterations = int.Parse(args[1]);
            }

            var robot = new DefaultRobot();
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
