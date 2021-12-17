namespace NaturalMouseMotionSharp.Support.MouseMotion
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Api;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Util;

    public class MovementFactory
    {
        private readonly ILogger log;
        private readonly IOvershootManager overshootManager;
        private readonly Size screenSize;
        private readonly ISpeedManager speedManager;
        private readonly int xDest;
        private readonly int yDest;

        public MovementFactory(int xDest, int yDest, ISpeedManager speedManager,
            IOvershootManager overshootManager, Size screenSize, ILogger log = null)
        {
            this.xDest = xDest;
            this.yDest = yDest;
            this.speedManager = speedManager;
            this.overshootManager = overshootManager;
            this.screenSize = screenSize;
            this.log = log ?? NullLogger.Instance;
        }

        public LinkedList<Movement> CreateMovements(Point currentMousePosition)
        {
            var movements = new LinkedList<Movement>();
            var lastMousePositionX = currentMousePosition.X;
            var lastMousePositionY = currentMousePosition.Y;
            var xDistance = this.xDest - lastMousePositionX;
            var yDistance = this.yDest - lastMousePositionY;

            var initialDistance = MathUtil.Hypot(xDistance, yDistance);
            var flowTime = this.speedManager.GetFlowWithTime(initialDistance);
            var flow = flowTime.X;
            var mouseMovementMs = flowTime.Y;
            var overshoots = this.overshootManager.GetOvershoots(flow, mouseMovementMs, initialDistance);

            if (overshoots == 0)
            {
                this.log.LogDebug("No overshoots for movement from ({x0}, {y0}) -> ({x1}, {y1})", currentMousePosition.X,
                    currentMousePosition.Y, this.xDest, this.yDest);
                movements.AddLast(new Movement(this.xDest, this.yDest, initialDistance, xDistance, yDistance,
                    mouseMovementMs, flow));
                return movements;
            }

            double distance;
            for (var i = overshoots; i > 0; i--)
            {
                var overshoot = this.overshootManager.GetOvershootAmount(this.xDest - lastMousePositionX,
                    this.yDest - lastMousePositionY, mouseMovementMs, i
                );
                var currentDestinationX = this.LimitByScreenWidth(this.xDest + overshoot.X);
                var currentDestinationY = this.LimitByScreenHeight(this.yDest + overshoot.Y);
                xDistance = currentDestinationX - lastMousePositionX;
                yDistance = currentDestinationY - lastMousePositionY;
                distance = MathUtil.Hypot(xDistance, yDistance);
                flow = this.speedManager.GetFlowWithTime(distance).X;
                movements.AddLast(
                    new Movement(currentDestinationX, currentDestinationY, distance, xDistance, yDistance,
                        mouseMovementMs, flow)
                );
                lastMousePositionX = currentDestinationX;
                lastMousePositionY = currentDestinationY;
                // Apply for the next overshoot if exists.
                mouseMovementMs = this.overshootManager.DeriveNextMouseMovementTimeMs(mouseMovementMs, i - 1);
            }

            var remove = true;
            // Remove overshoots from the end, which are matching the readonly destination, but keep those in middle of motion.
            var node = movements.Last;
            while (node != null && remove)
            {
                var next = node.Previous;
                var movement = node.Value;
                if (movement.DestX == this.xDest && movement.DestY == this.yDest)
                {
                    lastMousePositionX = movement.DestX - movement.XDistance;
                    lastMousePositionY = movement.DestY - movement.YDistance;
                    this.log.LogTrace("Pruning 0-overshoot movement (Movement to target) from the end. " + movement);
                    movements.Remove(node);
                }
                else
                {
                    remove = false;
                }

                node = next;
            }

            xDistance = this.xDest - lastMousePositionX;
            yDistance = this.yDest - lastMousePositionY;
            distance = MathUtil.Hypot(xDistance, yDistance);
            var movementToTargetFlowTime = this.speedManager.GetFlowWithTime(distance);
            var readonlyMovementTime =
                this.overshootManager.DeriveNextMouseMovementTimeMs(movementToTargetFlowTime.Y, 0);
            var readonlyMove = new Movement(this.xDest, this.yDest, distance, xDistance, yDistance,
                readonlyMovementTime, movementToTargetFlowTime.X
            );
            movements.AddLast(readonlyMove);

            this.log.LogDebug("{count} movements returned for move ({x0}, {y0}) -> ({x1}, {y1})", movements.Count,
                currentMousePosition.X,
                currentMousePosition.Y, this.xDest, this.yDest);
            this.log.LogTrace("Movements are: {m} ", movements);

            return movements;
        }

        private int LimitByScreenWidth(int value) => Math.Max(0, Math.Min(this.screenSize.Width - 1, value));

        private int LimitByScreenHeight(int value) => Math.Max(0, Math.Min(this.screenSize.Height - 1, value));
    }
}
