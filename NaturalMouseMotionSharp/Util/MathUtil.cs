namespace NaturalMouseMotionSharp.Util
{
    using System;

    public static class MathUtil
    {
        /// <summary>Rounds value towards target to exact integer value.</summary>
        /// <param name="value">the value to be rounded</param>
        /// <param name="target">the target to be rounded towards</param>
        /// <returns>the rounded value</returns>
        public static int RoundTowards(double value, int target)
        {
            if (target > value)
            {
                return (int)Math.Ceiling(value);
            }

            return (int)Math.Floor(value);
        }

        /// <summary>
        ///     Helper method to translate Java's <c>Math.hypot</c> to C#.
        /// </summary>
        internal static double Hypot(double x, double y) => Math.Sqrt((x * x) + (y * y));

        /// <summary>
        /// Helper method to translate Java's <c>Math.random</c> to C#.
        /// </summary>
        internal static double Random() => (rand ??= new Random()).NextDouble();

        [ThreadStatic]
        private static Random rand;
    }
}
