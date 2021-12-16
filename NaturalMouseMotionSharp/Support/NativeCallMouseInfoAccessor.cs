namespace NaturalMouseMotionSharp.Support
{
    using System;
    using System.Drawing;
    using Api;

    /// <summary>
    ///     This is faster version of <see cref="IMouseInfoAccessor" />. Should be around 2x faster than
    ///     <see cref="DefaultMouseInfoAccessor" />, because the latter also returns Device info
    ///     while we only care about position. This class also reuses the returned Point from
    ///     <see cref="IMouseInfoAccessor.GetMousePosition" /> which is filled with the mouse data, so it doesn't create
    ///     unnecessary temporary objects.
    /// </summary>
    /// <remarks>
    ///     Since this class uses internal API, it's experimental and
    ///     not guaranteed to work everywhere or all situations. Use with caution.
    ///     Generally <see cref="DefaultMouseInfoAccessor" /> should be preferred over this, unless faster version is required.
    /// </remarks>
    public class NativeCallMouseInfoAccessor : IMouseInfoAccessor
    {
        private readonly Point p = new Point(0, 0);
        private readonly MouseInfoPeer peer;

        public NativeCallMouseInfoAccessor()
        {
            Toolkit toolkit = Toolkit.getDefaultToolkit();
            MouseInfoPeer mp;
            try
            {
                Method method = SunToolkit.getDeclaredMethod("getMouseInfoPeer");
                method.setAccessible(true);
                mp = (MouseInfoPeer)method.invoke(toolkit);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message, e);
            }

            this.peer = mp;
        }

        public Point GetMousePosition()
        {
            this.peer.fillPointWithCoords(this.p);
            return this.p;
        }
    }
}
