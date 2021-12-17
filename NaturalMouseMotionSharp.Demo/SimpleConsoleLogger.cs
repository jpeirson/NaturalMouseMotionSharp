namespace NaturalMouseMotionSharp.Demo
{
    using System;
    using Microsoft.Extensions.Logging;

    public class SimpleConsoleLogger : ILogger
    {
        private readonly LogLevel threshold;
        public SimpleConsoleLogger(LogLevel threshold) => this.threshold = threshold;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            Console.WriteLine($"[{logLevel.ToString().ToUpper()[..4]}] {state}");
            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= this.threshold;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
