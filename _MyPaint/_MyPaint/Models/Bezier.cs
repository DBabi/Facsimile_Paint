using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace _MyPaint.Models
{
    /// <summary>
    /// Class bezier model 
    /// </summary>
    class Bezier : Shape
    {
        public List<Point> Points { get; set; } = new List<Point>();
        private int indexPointControl;
        protected override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddCurve(Points.ToArray());
                return path;
            }
        }

        public override void Draw(Graphics graphics)
        {
            using (GraphicsPath path = GraphicsPath)
            {
                using (Pen pen = new Pen(myPen.Color, myPen.Width) { DashStyle = myPen.DashStyle })
                {
                    graphics.DrawPath(pen, path);
                }
            }
        }

        protected override bool GetOutLine(Point point)
        {
            bool res = false;
            using (GraphicsPath path = GraphicsPath)
            {
                using (Pen pen = new Pen(myPen.Color, myPen.Width + 3))
                {
                    res = path.IsOutlineVisible(point, pen);
                }
            }
            return res;
        }

        protected override bool IsPointControl(Point point)
        {
            for (indexPointControl = 0; indexPointControl < Points.Count; indexPointControl++)
                if (point.X >= Points[indexPointControl].X - myPen.Width / 2 - 3 && point.X <= Points[indexPointControl].X + myPen.Width / 2 + 3)
                    if (point.Y >= Points[indexPointControl].Y - myPen.Width / 2 - 3 && point.Y <= Points[indexPointControl].Y + myPen.Width / 2 + 3)
                    {
                        return true;
                    }

            return false;
        }

        public override void Resize(Point point)
        {
            Points[indexPointControl] = point;
        }

        public override void Move(Point distance)
        {
            startPoint = new Point(startPoint.X + distance.X, startPoint.Y + distance.Y);
            endPoint = new Point(endPoint.X + distance.X, endPoint.Y + distance.Y);
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X + distance.X, Points[i].Y + distance.Y);
            }
        }
    }
}
