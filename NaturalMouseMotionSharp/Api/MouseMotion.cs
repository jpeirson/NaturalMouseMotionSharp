namespace NaturalMouseMotionSharp.Api
{
    using System;
    using System.Drawing;
    using System.Threading;
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
        private static readonly int SleepAfterAdjustmentMs = 2;
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

        ///<param name="nature"> the nature that defines how mouse is moved</param>
        ///<param name="xDest">  the x-coordinate of destination</param>
        ///<param name="yDest">  the y-coordinate of destination</param>
        ///<param name="random"> the random used for unpredictability</param>
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


        /// <summary>Blocking call, starts to move the cursor to the specified location from where it currently is.</summary>
        /// <param name="observer">Provide observer if you are interested receiving the location of mouse on every step</param>
        /// <exception cref="ThreadInterruptedException"> when interrupted</exception>
        public void Move(MouseMotionObserver observer)
        {
            this.UpdateMouseInfo();
            this.log.LogInformation("Starting to move mouse to ({}, {}), current position: ({}, {})", this.xDest,
                this.yDest,
                this.mousePosition.X, this.mousePosition.Y);

            var movementFactory = new MovementFactory(this.xDest, this.yDest, this.speedManager, this.overshootManager,
                this.screenSize, this.log);
            var movements = movementFactory.CreateMovements(this.mousePosition);
            var overshoots = movements.Count - 1;
            while (this.mousePosition.X != this.xDest || this.mousePosition.Y != this.yDest)
            {
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
                    this.log.LogDebug("Using overshoots ({} out of {}), aiming at ({}, {})",
                        overshoots - movements.Count + 1, overshoots, movement.DestX, movement.DestY);
                }

                var distance = movement.Distance;
                var mouseMovementMs = movement.Time;
                var flow = movement.Flow;
                double xDistance = movement.XDistance;
                double yDistance = movement.YDistance;
                this.log.LogDebug("Movement arc length computed to {} and time predicted to {} ms", distance,
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
                    this.log.LogTrace("Step: x: {} y: {} tc: {} c: {}", xStepSize, yStepSize, timeCompletion,
                        completion);

                    var noise = this.noiseProvider.GetNoise(this.random, xStepSize, yStepSize);
                    var deviation = this.deviationProvider.GetDeviation(distance, completion);

                    noiseX += noise.X;
                    noiseY += noise.Y;
                    simulatedMouseX += xStepSize;
                    simulatedMouseY += yStepSize;

                    this.log.LogTrace("EffectFadeMultiplier: {}", effectFadeMultiplier);
                    this.log.LogTrace("SimulatedMouse: [{}, {}]", simulatedMouseX, simulatedMouseY);

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

                    this.systemCalls.SetMousePosition(
                        mousePosX,
                        mousePosY
                    );

                    // Allow other action to take place or just observe, we'll later compensate by sleeping less.
                    observer(mousePosX, mousePosY);

                    var timeLeft = endTime - this.systemCalls.CurrentTimeMillis();
                    this.SleepAround(Math.Max(timeLeft, 0), 0);
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
                    this.systemCalls.SetMousePosition(movement.DestX, movement.DestY);
                    // Let's wait a bit before getting mouse info.
                    this.SleepAround(SleepAfterAdjustmentMs, 0);
                    this.UpdateMouseInfo();
                }

                if (this.mousePosition.X != this.xDest || this.mousePosition.Y != this.yDest)
                {
                    // We are dealing with overshoot, let's sleep a bit to simulate human reaction time.
                    this.SleepAround(this.reactionTimeBaseMs, this.reactionTimeVariationMs);
                }

                this.log.LogDebug("Steps completed, mouse at " + this.mousePosition.X + " " + this.mousePosition.Y);
            }

            this.log.LogInformation("Mouse movement to ({}, {}) completed", this.xDest, this.yDest);
        }


        private int LimitByScreenWidth(int value) => Math.Max(0, Math.Min(this.screenSize.Width - 1, value));

        private int LimitByScreenHeight(int value) => Math.Max(0, Math.Min(this.screenSize.Height - 1, value));

        private void SleepAround(long sleepMin, long randomPart)
        {
            var sleepTime = sleepMin + (this.random.NextDouble() * randomPart);
            if (this.log.IsEnabled(LogLevel.Trace) && sleepTime > 0)
            {
                this.UpdateMouseInfo();
                this.log.LogTrace("Sleeping at ({}, {}) for {} ms", this.mousePosition.X, this.mousePosition.Y,
                    sleepTime);
            }

            this.systemCalls.Sleep((long)sleepTime);
        }

        private void UpdateMouseInfo() => this.mousePosition = this.mouseInfo.GetMousePosition();
    }
}
