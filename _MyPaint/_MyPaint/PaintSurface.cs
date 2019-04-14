using System.Windows.Forms;

namespace _MyPaint
{
    public class PaintSurface : Panel
    {
        public PaintSurface()
        {
            DoubleBuffered = true;
            SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}