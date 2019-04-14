using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _MyPaint.Models
{
    /// <summary>
    /// Class Rectangle Models
    /// </summary>
    class Rectangle : Shape
    {
        public Rectangle()
        {
            name = "Rectangle";
        }

        protected override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y)
                {
                    path.AddRectangle(new System.Drawing.Rectangle(endPoint, new Size(startPoint.X - endPoint.X, startPoint.Y - endPoint.Y)));
                }
                else if (startPoint.X > endPoint.X && startPoint.Y < endPoint.Y)
                {
                    path.AddRectangle(new System.Drawing.Rectangle(new Point(endPoint.X, startPoint.Y), new Size(startPoint.X - endPoint.X, endPoint.Y - startPoint.Y)));
                }
                else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
                {
                    path.AddRectangle(new System.Drawing.Rectangle(new Point(startPoint.X, endPoint.Y), new Size(endPoint.X - startPoint.X, startPoint.Y - endPoint.Y)));
                }
                else
                {
                    path.AddRectangle(new System.Drawing.Rectangle(startPoint, new Size(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y)));
                }

                return path;
            }
        }

        public override object Clone()
        {
            return new Rectangle
            {
                startPoint = startPoint,
                endPoint = endPoint,
                myPen = myPen,
                isSelect = isSelect,
                name = name
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
