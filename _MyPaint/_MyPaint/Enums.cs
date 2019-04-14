using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _MyPaint
{
    public enum CurrentShape
    {
        Line,
        Rectangle,
        Ellipse,
        Bezier,
        Polygon,
        NoDrawing
    }

    public enum ShapeMode
    {
        Fill,
        NoFill
    }
}
