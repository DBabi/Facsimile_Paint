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

        public Bezier()
        {
            name = "Bezier";
        }

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

        public override bool IsClick(Point point)
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

        public override void Move(Point distance)
        {
            startPoint = new Point(startPoint.X + distance.X, startPoint.Y + distance.Y);
            endPoint = new Point(endPoint.X + distance.X, endPoint.Y + distance.Y);
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X + distance.X, Points[i].Y + distance.Y);
            }
        }

        public override object Clone()
        {
            Bezier bezier = new Bezier
            {
                startPoint = startPoint,
                endPoint = endPoint,
                isSelect = isSelect,
                name = name,
                myPen = myPen,
            };
            Points.ForEach(point => bezier.Points.Add(point));
            return bezier;
        }
    }
}
