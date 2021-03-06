namespace NaturalMouseMotionSharp.Api
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Support;
    using Support.MouseMotion;
    using Util;

    /// <summary>
    ///     Contains instructions to move cursor smoothly to the destination coordinates from where ever the cursor
    ///     currently is. The class is reusable, meaning user can keep calling it and the cursor returns in a random,
    ///     but reliable way, described in this class, to the destination.
    /// </summary>
    public class MouseMotion
    {
        private const int SleepAfterAdjustmentMs = 2;
        private readonly IDeviationProvider deviationProvider;
        private readonly int effectFadeSteps;
        private readonly ILogger log;
        private readonly int minSteps;
        private readonly IMouseInfoAccessor mouseInfo;
        private readonly INoiseProvider noiseProvider;
        private readonly IOvershootManager overshootManager;
        private readonly Random random;
        private readonly int reactionTimeBaseMs;
        private readonly int reactionTimeVariationMs;
        private readonly Size screenSize;
        private readonly ISpeedManager speedManager;
        private readonly ISystemCalls systemCalls;
        private readonly double timeToStepsDivider;
        private readonly int xDest;
        private readonly int yDest;
        private Point mousePosition;

        /// <param name="nature"> the nature that defines how mouse is moved</param>
        /// <param name="xDest">  the x-coordinate of destination</param>
        /// <param name="yDest">  the y-coordinate of destination</param>
        /// <param name="random"> the random used for unpredictability</param>
        /// <param name="log"> Optional logger for tracing activity</param>
        public MouseMotion(MouseMotionNature nature, Random random, int xDest, int yDest, ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
            this.deviationProvider = nature.DeviationProvider;
            this.noiseProvider = nature.NoiseProvider;
            this.systemCalls = nature.SystemCalls;
            this.screenSize = this.systemCalls.GetScreenSize();
            this.xDest = this.LimitByScreenWidth(xDest);
            this.yDest = this.LimitByScreenHeight(yDest);
            this.random = random;
            this.mouseInfo = nature.MouseInfo;
            this.speedManager = nature.SpeedManager;
            this.timeToStepsDivider = nature.TimeToStepsDivider;
            this.minSteps = nature.MinSteps;
            this.effectFadeSteps = nature.EffectFadeSteps;
            this.reactionTimeBaseMs = nature.ReactionTimeBaseMs;
            this.reactionTimeVariationMs = nature.ReactionTimeVariationMs;
            this.overshootManager = nature.OvershootManager;
        }

        /// <summary>
        ///     Blocking call, starts to move the cursor to the specified location from where it currently is.
        /// </summary>
        /// <exception cref="ThreadInterruptedException"> when interrupted</exception>
        public void Move() =>
            this.Move((x, y) =>
            {
            });

        /// <summary>
        ///     Async call, starts to move the cursor to the specified location from where it currently is.
        /// </summary>
        /// <param name="cancellation">Supports cancellation</param>
        /// <returns>A task that completes when the mouse movement completes.</returns>
        /// <exception cref="OperationCanceledException"> when interrupted</exception>
        public Task MoveAsync(CancellationToken cancellation = default) =>
            this.MoveAsync((x, y) =>
            {
            }, cancellation);

        /// <summary>Blocking call, starts to move the cursor to the specified location from where it currently is.</summary>
        /// <param name="observer">Provide observer if you are interested receiving the location of mouse on every step</param>
        /// <exception cref="ThreadInterruptedException"> when interrupted</exception>
        public void Move(MouseMotionObserver observer) => this.Move(observer, CancellationToken.None, true)
            .ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>Async call, starts to move the cursor to the specified location from where it currently is.</summary>
        /// <param name="observer">Provide observer if you are interested receiving the location of mouse on every step</param>
        /// <param name="cancellation">Supports cancellation</param>
        /// <returns>A task that completes when the mouse movement completes.</returns>
        /// <exception cref="OperationCanceledException"> when interrupted</exception>
        public Task MoveAsync(MouseMotionObserver observer, CancellationToken cancellation = default) =>
            this.Move(observer, cancellation, false);

        /// <summary>
        ///     Implements both sync and async versions of <c>Move</c>. If <paramref name="sync" /> is true,
        ///     then this method is guaranteed to return an already-completed task.
        /// </summary>
        /// <remarks>
        ///     See
        ///     <a
        ///         href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/july/async-programming-brownfield-async-development#the-flag-argument-hack">
        ///         the
        ///         flag argument hack
        ///     </a>
        ///     .
        /// </remarks>
        private async Task Move(MouseMotionObserver observer, CancellationToken cancellation, bool sync)
        {
            cancellation.ThrowIfCancellationRequested();

            this.UpdateMouseInfo();
            this.log.LogInformation("Starting to move mouse to ({x1}, {y1}), current position: ({x0}, {y0})",
                this.xDest,
                this.yDest,
                this.mousePosition.X, this.mousePosition.Y);

            var movementFactory = new MovementFactory(this.xDest, this.yDest, this.speedManager, this.overshootManager,
                this.screenSize, this.log);
            var movements = movementFactory.CreateMovements(this.mousePosition);
            var overshoots = movements.Count - 1;
            while (this.mousePosition.X != this.xDest || this.mousePosition.Y != this.yDest)
            {
                cancellation.ThrowIfCancellationRequested();

                if (movements.Count == 0)
                {
                    // This shouldn't usually happen, but it's possible that somehow we won't end up on the target,
                    // Then just re-attempt from mouse new position. (There are known JDK bugs, that can cause sending the cursor
                    // to wrong pixel)
                    this.UpdateMouseInfo();
                    this.log.LogWarning("Re-populating movement array. Did not end up on target pixel.");
                    movements = movementFactory.CreateMovements(this.mousePosition);
                }

                var movement = movements.First.Value;
                movements.RemoveFirst();
                if (movements.Count > 0)
                {
                    this.log.LogDebug("Using overshoots ({i} out of {count}), aiming at ({x}, {y})",
                        overshoots - movements.Count + 1, overshoots, movement.DestX, movement.DestY);
                }

                var distance = movement.Distance;
                var mouseMovementMs = movement.Time;
                var flow = movement.Flow;
                double xDistance = movement.XDistance;
                double yDistance = movement.YDistance;
                this.log.LogDebug("Movement arc length computed to {d} and time predicted to {ms} ms", distance,
                    mouseMovementMs);

                // Number of steps is calculated from the movement time and limited by minimal amount of steps
                // (should have at least MIN_STEPS) and distance (shouldn't have more steps than pixels travelled)
                var steps = (int)Math.Ceiling(Math.Min(distance,
                    Math.Max(mouseMovementMs / this.timeToStepsDivider, this.minSteps)));

                var startTime = this.systemCalls.CurrentTimeMillis();
                var stepTime = (long)(mouseMovementMs / (double)steps);

                this.UpdateMouseInfo();
                double simulatedMouseX = this.mousePosition.X;
                double simulatedMouseY = this.mousePosition.Y;

                var deviationMultiplierX = (this.random.NextDouble() - 0.5) * 2;
                var deviationMultiplierY = (this.random.NextDouble() - 0.5) * 2;

                double completedXDistance = 0;
                double completedYDistance = 0;
                double noiseX = 0;
                double noiseY = 0;

                for (var i = 0; i < steps; i++)
                {
                    cancellation.ThrowIfCancellationRequested();

                    // All steps take equal amount of time. This is a value from 0...1 describing how far along the process is.
                    var timeCompletion = i / (double)steps;

                    double effectFadeStep = Math.Max(i - (steps - this.effectFadeSteps) + 1, 0);
                    // value from 0 to 1, when effectFadeSteps remaining steps, starts to decrease to 0 linearly
                    // This is here so noise and deviation wouldn't add offset to mouse final position, when we need accuracy.
                    var effectFadeMultiplier = (this.effectFadeSteps - effectFadeStep) / this.effectFadeSteps;

                    var xStepSize = flow.GetStepSize(xDistance, steps, timeCompletion);
                    var yStepSize = flow.GetStepSize(yDistance, steps, timeCompletion);

                    completedXDistance += xStepSize;
                    completedYDistance += yStepSize;
                    var completedDistance = MathUtil.Hypot(completedXDistance, completedYDistance);
                    var completion = Math.Min(1, completedDistance / distance);
                    this.log.LogTrace("Step: x: {x} y: {y} tc: {tc} c: {c}", xStepSize, yStepSize, timeCompletion,
                        completion);

                    var noise = this.noiseProvider.GetNoise(this.random, xStepSize, yStepSize);
                    var deviation = this.deviationProvider.GetDeviation(distance, completion);

                    noiseX += noise.X;
                    noiseY += noise.Y;
                    simulatedMouseX += xStepSize;
                    simulatedMouseY += yStepSize;

                    this.log.LogTrace("EffectFadeMultiplier: {m}", effectFadeMultiplier);
                    this.log.LogTrace("SimulatedMouse: [{x}, {y}]", simulatedMouseX, simulatedMouseY);

                    var endTime = startTime + (stepTime * (i + 1));
                    var mousePosX = MathUtil.RoundTowards(
                        simulatedMouseX +
                        (deviation.X * deviationMultiplierX * effectFadeMultiplier) +
                        (noiseX * effectFadeMultiplier),
                        movement.DestX
                    );

                    var mousePosY = MathUtil.RoundTowards(
                        simulatedMouseY +
                        (deviation.Y * deviationMultiplierY * effectFadeMultiplier) +
                        (noiseY * effectFadeMultiplier),
                        movement.DestY
                    );

                    mousePosX = this.LimitByScreenWidth(mousePosX);
                    mousePosY = this.LimitByScreenHeight(mousePosY);

                    if (sync)
                    {
                        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                        // intentional sync usage
                        this.systemCalls.SetMousePosition(
                            mousePosX,
                            mousePosY
                        );
                    }
                    else
                    {
                        await this.systemCalls.SetMousePositionAsync(
                            mousePosX,
                            mousePosY,
                            cancellation
                        ).ConfigureAwait(false);
                    }

                    // Allow other action to take place or just observe, we'll later compensate by sleeping less.
                    observer(mousePosX, mousePosY);

                    var timeLeft = endTime - this.systemCalls.CurrentTimeMillis();
                    await this.SleepAround(Math.Max(timeLeft, 0), 0, sync, cancellation).ConfigureAwait(false);
                }

                this.UpdateMouseInfo();

                if (this.mousePosition.X != movement.DestX || this.mousePosition.Y != movement.DestY)
                {
                    // It's possible that mouse is manually moved or for some other reason.
                    // Let's start next step from pre-calculated location to prevent errors from accumulating.
                    // But print warning as this is not expected behavior.
                    this.log.LogWarning("Mouse off from step endpoint (adjustment was done) " +
                                        "x: (" + this.mousePosition.X + " -> " + movement.DestX + ") " +
                                        "y: (" + this.mousePosition.Y + " -> " + movement.DestY + ") "
                    );

                    if (sync)
                    {
                        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                        // intentional sync usage
                        this.systemCalls.SetMousePosition(movement.DestX, movement.DestY);
                    }
                    else
                    {
                        await this.systemCalls.SetMousePositionAsync(movement.DestX, movement.DestY, cancellation)
                            .ConfigureAwait(false);
                    }

                    // Let's wait a bit before getting mouse info.
                    await this.SleepAround(SleepAfterAdjustmentMs, 0, sync, cancellation).ConfigureAwait(false);
                    this.UpdateMouseInfo();
                }

                if (this.mousePosition.X != this.xDest || this.mousePosition.Y != this.yDest)
                {
                    // We are dealing with overshoot, let's sleep a bit to simulate human reaction time.
                    await this.SleepAround(this.reactionTimeBaseMs, this.reactionTimeVariationMs, sync, cancellation)
                        .ConfigureAwait(false);
                }

                this.log.LogDebug("Steps completed, mouse at " + this.mousePosition.X + " " + this.mousePosition.Y);
            }

            this.log.LogInformation("Mouse movement to ({x}, {y}) completed", this.xDest, this.yDest);
        }

        private int LimitByScreenWidth(int value) => Math.Max(0, Math.Min(this.screenSize.Width - 1, value));

        private int LimitByScreenHeight(int value) => Math.Max(0, Math.Min(this.screenSize.Height - 1, value));

        private async Task SleepAround(long sleepMin, long randomPart, bool sync, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var sleepTime = sleepMin + (this.random.NextDouble() * randomPart);
            if (this.log.IsEnabled(LogLevel.Trace) && sleepTime > 0)
            {
                this.UpdateMouseInfo();
                this.log.LogTrace("Sleeping at ({x}, {y}) for {ms} ms", this.mousePosition.X, this.mousePosition.Y,
                    sleepTime);
            }

            if (sync)
            {
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                // intentional sync usage
                this.systemCalls.Sleep((long)sleepTime);
            }
            else
            {
                await this.systemCalls.SleepAsync((long)sleepTime, cancellation).ConfigureAwait(false);
            }
        }

        private void UpdateMouseInfo() => this.mousePosition = this.mouseInfo.GetMousePosition();
    }
}
