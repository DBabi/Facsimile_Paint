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
    /// Class Ellipse Model
    /// </summary>
    class Ellipse : Shape
    {
        public Ellipse()
        {
            name = "Eclipse";
        }

        protected override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(new System.Drawing.Rectangle(startPoint.X, startPoint.Y, endPoint.X - startPoint.X, endPoint.Y - startPoint.Y));
                return path;
            }
        }

        public override object Clone()
        {
            return new Ellipse
            {
                startPoint = startPoint,
                endPoint = endPoint,
                isSelect = isSelect,
                name = name,
                myPen = myPen,
            };
        }

        public override void Draw(Graphics graphics)
        {
            using (GraphicsPath path = GraphicsPath)
            {
                if (!isFilled)
                {
                    using (Pen pen = new Pen(myPen.Color, myPen.Width) { DashStyle = myPen.DashStyle })
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
                else
                {
                    using (Brush brush = new SolidBrush(myPen.Color))
                    {
                        graphics.FillPath(brush, path);
                    }
                }
            }
        }

        public override bool IsClick(Point point)
        {
            bool res = false;
            using (GraphicsPath path = GraphicsPath)
            {
                if (!isFilled)
                {
                    using (Pen pen = new Pen(myPen.Color, myPen.Width + 3))
                    {
                        res = path.IsOutlineVisible(point, pen);
                    }
                }
                else
                {
                    res = path.IsVisible(point);
                }
            }
            return res;
        }

        public override void Move(Point distance)
        {
            startPoint = new Point(startPoint.X + distance.X, startPoint.Y + distance.Y);
            endPoint = new Point(endPoint.X + distance.X, endPoint.Y + distance.Y);
        }
    }
}
