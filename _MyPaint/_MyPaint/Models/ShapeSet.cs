using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace _MyPaint.Models
{
    /// <summary>
    /// Class ShapeSet Model
    /// Include on other shapes
    /// </summary>
    class ShapeSet : Shape, IEnumerable
    {
        private List<Shape> Shapes = new List<Shape>();

        public ShapeSet()
        {
            name = "Group";
        }

        public Shape this[int index]
        {
            get => Shapes[index];
            set => Shapes[index] = value;
        }

        public void Add(Shape shape)
        {
            Shapes.Add(shape);
        }

        private GraphicsPath[] GraphicsPaths
        {
            get
            {
                GraphicsPath[] paths = new GraphicsPath[Shapes.Count];

                for (int i = 0; i < Shapes.Count; i++)
                {
                    GraphicsPath path = new GraphicsPath();
                    if (Shapes[i] is Line line)
                    {
                        path.AddLine(line.startPoint, line.endPoint);
                    }
                    else if (Shapes[i] is Rectangle rect)
                    {
                        path.AddRectangle(new RectangleF(rect.startPoint.X, rect.startPoint.Y, rect.endPoint.X - rect.startPoint.X, rect.endPoint.Y - rect.startPoint.Y));
                    }
                    else if (Shapes[i] is Ellipse ellipse)
                    {
                        path.AddEllipse(new System.Drawing.Rectangle(ellipse.startPoint.X, ellipse.startPoint.Y, ellipse.endPoint.X - ellipse.startPoint.X, ellipse.endPoint.Y - ellipse.startPoint.Y));
                    }
                    else if (Shapes[i] is Bezier bezier)
                    {
                        path.AddCurve(bezier.Points.ToArray());
                    }
                    else if (Shapes[i] is Polygon polygon)
                    {
                        path.AddPolygon(polygon.Points.ToArray());
                    }
                    else if (Shapes[i] is ShapeSet group)
                    {
                        for (int j = 0; j < group.GraphicsPaths.Length; j++)
                        {
                            path.AddPath(group.GraphicsPaths[j], false);
                        }
                    }
                    paths[i] = path;
                }

                return paths;
            }
        }

        public override void Draw(Graphics graphics)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if(Shapes[i] is Line || Shapes[i] is Bezier)
                    {
                        using (Pen pen = new Pen(Shapes[i].myPen.Color, Shapes[i].myPen.Width) { DashStyle = Shapes[i].myPen.DashStyle })
                        {
                            graphics.DrawPath(pen, path);
                        }
                    }
                    else if (Shapes[i] is ShapeSet group)
                    {
                        group.Draw(graphics);
                    }
                    else
                    {
                        if (Shapes[i].isFilled)
                        {
                            using (Brush brush = new SolidBrush(Shapes[i].myPen.Color))
                            {
                                graphics.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(Shapes[i].myPen.Color, Shapes[i].myPen.Width)
                                    { DashStyle = Shapes[i].myPen.DashStyle })
                            {
                                graphics.DrawPath(pen, path);
                            }
                        }
                    }

                }
            }
        }

        public override bool IsClick(Point point)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (Shapes[i] is ShapeSet group)
                    {
                        return group.IsClick(point);
                    }
                    else if (Shapes[i] is Line || Shapes[i] is Bezier)
                    {
                        using (Pen pen = new Pen(Shapes[i].myPen.Color, Shapes[i].myPen.Width + 3))
                        {
                            if (path.IsOutlineVisible(point, pen))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Shapes[i].isFilled)
                        {
                            using (Brush brush = new SolidBrush(Shapes[i].myPen.Color))
                            {
                                if (path.IsVisible(point))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(Shapes[i].myPen.Color, Shapes[i].myPen.Width + 3))
                            {
                                if (path.IsOutlineVisible(point, pen))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override void Move(Point distance)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (Shapes[i] is Bezier bezier)
                {
                    bezier.startPoint = new Point(bezier.startPoint.X + distance.X, bezier.startPoint.Y + distance.Y);
                    bezier.endPoint = new Point(bezier.endPoint.X + distance.X, bezier.endPoint.Y + distance.Y);

                    for (int j = 0; j < bezier.Points.Count; j++)
                    {
                        bezier.Points[j] = new Point(bezier.Points[j].X + distance.X, bezier.Points[j].Y + distance.Y);
                    }
                }
                else if (Shapes[i] is Polygon polygon)
                {
                    polygon.startPoint = new Point(polygon.startPoint.X + distance.X, polygon.startPoint.Y + distance.Y);
                    polygon.endPoint = new Point(polygon.endPoint.X + distance.X, polygon.endPoint.Y + distance.Y);

                    for (int j = 0; j < polygon.Points.Count; j++)
                    {
                        polygon.Points[j] = new Point(polygon.Points[j].X + distance.X, polygon.Points[j].Y + distance.Y);
                    }
                }
                else if (Shapes[i] is ShapeSet group)
                {
                    group.Move(distance);
                }
                else
                {
                    Shapes[i].startPoint = new Point(Shapes[i].startPoint.X + distance.X, Shapes[i].startPoint.Y + distance.Y);
                    Shapes[i].endPoint = new Point(Shapes[i].endPoint.X + distance.X, Shapes[i].endPoint.Y + distance.Y);
                }
            }
            startPoint = new Point(startPoint.X + distance.X, startPoint.Y + distance.Y);
            endPoint = new Point(endPoint.X + distance.X, endPoint.Y + distance.Y);
        }

        public override object Clone()
        {
            ShapeSet group = new ShapeSet
            {
                startPoint = startPoint,
                endPoint = endPoint,
                isSelect = isSelect,
                name = name,
                myPen = myPen,
            };
            for (int i = 0; i < Shapes.Count; i++)
            {
                group.Shapes.Add(Shapes[i].Clone() as Shape);
            }
            return group;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Shapes.GetEnumerator();
        }

        public int Count => Shapes.Count;

        protected override GraphicsPath GraphicsPath => null;
    }
}
