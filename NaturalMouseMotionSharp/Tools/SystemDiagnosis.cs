namespace NaturalMouseMotionSharp.Tools
{
    using System;
    using System.Threading;
    using Api;
    using Support;

    public static class SystemDiagnosis
    {
        /// <summary>
        ///     Runs a diagnosis with default configuration, by setting mouse all over your screen and expecting to receive
        ///     correct coordinates back.
        /// </summary>
        /// <exception cref="ApplicationException">If a <see cref="DefaultRobot" /> cannot be constructed</exception>
        /// <exception cref="InvalidOperationException">An error occurred moving the mouse</exception>
        public static void ValidateMouseMovement() => ValidateMouseMovement(new DefaultRobot());

        /// <summary>
        ///     Runs a diagnosis with default configuration, by setting mouse all over your screen and expecting to receive
        ///     correct coordinates back.
        /// </summary>
        /// <exception cref="InvalidOperationException">An error occurred moving the mouse</exception>
        public static void ValidateMouseMovement(IRobot robot)
        {
            try
            {
                ValidateMouseMovement(new DefaultSystemCalls(robot), new DefaultMouseInfoAccessor(robot));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message, e);
            }
        }

        /// <summary>
        ///     Runs a diagnosis, by setting mouse all over your screen and expecting to receive correct coordinates back.
        ///     If no issues are found, then this method completes without throwing an error, otherwise IllegalStateException is
        ///     thrown.
        /// </summary>
        /// <param name="system">a <see cref="ISystemCalls" /> class which is used for setting the mouse position</param>
        /// <param name="accessor">a <see cref="IMouseInfoAccessor" /> which is used for querying mouse position</param>
        /// <exception cref="InvalidOperationException">An error occurred moving the mouse</exception>
        public static void ValidateMouseMovement(ISystemCalls system, IMouseInfoAccessor accessor)
        {
            var dimension = system.GetScreenSize();
            for (var y = 0; y < dimension.Height; y += 50)
            {
                for (var x = 0; x < dimension.Width; x += 50)
                {
                    system.SetMousePosition(x, y);

                    try
                    {
                        Thread.Sleep(1);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }

                    var p = accessor.GetMousePosition();
                    if (x != p.X || y != p.Y)
                    {
                        throw new InvalidOperationException(
                            "Tried to move mouse to (" + x + ", " + y + "). Actually moved to (" + p.X + ", " + p.Y +
                            ")" +
                            "This means NaturalMouseMotion is not able to work optimally on this system as the cursor move " +
                            "calls may miss the target pixels on the screen."
                        );
                    }
                }
            }
        }
    }
}
