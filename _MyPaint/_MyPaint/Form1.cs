using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using _MyPaint.Models;
namespace _MyPaint
{
    public partial class frmMain : Form
    {
        #region Property
        private List<Button> buttons;
        private List<Shape> shapes = new List<Shape>();
        private CurrentShape currentShape = CurrentShape.NoDrawing;
        private Point previousPoint;
        private ShapeMode mode = ShapeMode.NoFill;
        private Brush brush = new SolidBrush(Color.Blue);
        private Pen framePen = new Pen(Color.Blue, 1)
        {
            DashPattern = new float[] { 3, 3, 3, 3 },
            DashStyle = DashStyle.Custom
        };
        private float zoom;
        private Shape selectedShape;
        private System.Drawing.Rectangle selectedRegion;
        private bool isMouseDown;
        private bool isDrawPolygon;
        private bool isDrawBezier;
        private bool isMovingShape;
        private bool isControlKeyPress;
        private bool isMouseSelect;
        #endregion

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            buttons = new List<Button> { btnLine, btnRectangle, btnEllipse, btnBezier, btnPolygon, btnSelect };
            cbbDashStyle.SelectedIndex = 0;
            nmrSize.Value = 1;
            zoom = 1f;
        }

        #region Support Function
        /// <summary>
        /// To remove all focus on button
        /// </summary>
        private void UncheckButton()
        {
            buttons.ForEach(button => button.BackColor = Color.WhiteSmoke);
        }

        /// <summary>
        /// Make enable all button
        /// </summary>
        private void EnableButtons()
        {
            buttons.ForEach(button => button.Enabled = true);
        }

        /// <summary>
        /// Find the out size
        /// Cover the bezier
        /// </summary>
        /// <param name="bezier"></param>
        private void FindBezierRegion(Bezier bezier)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            bezier.Points.ForEach(p =>
            {
                if (minX > p.X)
                {
                    minX = p.X;
                }
                if (minY > p.Y)
                {
                    minY = p.Y;
                }
                if (maxX < p.X)
                {
                    maxX = p.X;
                }
                if (maxY < p.Y)
                {
                    maxY = p.Y;
                }
            });
            bezier.startPoint = new Point(minX, minY);
            bezier.endPoint = new Point(maxX, maxY);
        }

        /// <summary>
        /// Find the out size
        /// Cover the polygon
        /// </summary>
        /// <param name="polygon"></param>
        private void FindPolygonRegion(Polygon polygon)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            polygon.Points.ForEach(p =>
            {
                if (minX > p.X)
                {
                    minX = p.X;
                }
                if (minY > p.Y)
                {
                    minY = p.Y;
                }
                if (maxX < p.X)
                {
                    maxX = p.X;
                }
                if (maxY < p.Y)
                {
                    maxY = p.Y;
                }
            });
            polygon.startPoint = new Point(minX, minY);
            polygon.endPoint = new Point(maxX, maxY);
        }

        /// <summary>
        /// Find the out size
        /// Cover the group shapes
        /// </summary>
        /// <param name="group"></param>
        private void FindGroupRegion(ShapeSet group)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            for (int i = 0; i < group.Count; i++)
            {
                Shape shape = group[i];

                if (shape is Bezier curve)
                {
                    FindBezierRegion(curve);
                }
                else if (shape is Polygon polygon)
                {
                    FindPolygonRegion(polygon);
                }


                if (shape.startPoint.X < minX)
                {
                    minX = shape.startPoint.X;
                }
                if (shape.endPoint.X < minX)
                {
                    minX = shape.endPoint.X;
                }

                if (shape.startPoint.Y < minY)
                {
                    minY = shape.startPoint.Y;
                }
                if (shape.endPoint.Y < minY)
                {
                    minY = shape.endPoint.Y;
                }

                if (shape.startPoint.X > maxX)
                {
                    maxX = shape.startPoint.X;
                }
                if (shape.endPoint.X > maxX)
                {
                    maxX = shape.endPoint.X;
                }

                if (shape.startPoint.Y > maxY)
                {
                    maxY = shape.startPoint.Y;
                }
                if (shape.endPoint.Y > maxY)
                {
                    maxY = shape.endPoint.Y;
                }
            }
            group.startPoint = new Point(minX, minY);
            group.endPoint = new Point(maxX, maxY);
        }
        #endregion

        #region Panel_Paint Event
        private void pnlPaint_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.ScaleTransform(zoom, zoom);
            foreach(Shape shape in shapes)
            {
                if (shape.isSelect)
                {
                    shape.Draw(e.Graphics);
                    if (shape is Ellipse || shape is ShapeSet)
                    {
                        e.Graphics.DrawRectangle(framePen, new System.Drawing.Rectangle(shape.startPoint.X, shape.startPoint.Y, shape.endPoint.X - shape.startPoint.X, shape.endPoint.Y - shape.startPoint.Y));
                    }
                    else if (shape is Bezier curve)
                    {
                        for (int i = 0; i < curve.Points.Count; i++)
                        {
                            e.Graphics.FillEllipse(brush, new System.Drawing.Rectangle(curve.Points[i].X - 4, curve.Points[i].Y - 4, 8, 8));
                        }
                    }
                    else if (shape is Polygon polygon)
                    {
                        for (int i = 0; i < polygon.Points.Count; i++)
                        {
                            e.Graphics.FillEllipse(brush, new System.Drawing.Rectangle(polygon.Points[i].X - 4, polygon.Points[i].Y - 4, 8, 8));
                        }
                    }
                    else
                    {
                        e.Graphics.FillEllipse(brush, new System.Drawing.Rectangle(shape.startPoint.X - 4, shape.startPoint.Y - 4, 8, 8));
                        e.Graphics.FillEllipse(brush, new System.Drawing.Rectangle(shape.endPoint.X - 4, shape.endPoint.Y - 4, 8, 8));
                    }
                }
                else
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (isMouseSelect)
            {
                e.Graphics.DrawRectangle(framePen, selectedRegion);
            }
        }

        private void pnlPaint_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentShape == CurrentShape.NoDrawing)
            {
                if (isControlKeyPress)
                {
                    for (int i = 0; i < shapes.Count; i++)
                    {
                        if (shapes[i].IsClick(e.Location))
                        {
                            shapes[i].isSelect = !shapes[i].isSelect;
                            pnlPaint.Invalidate();
                            break;
                        }
                    }
                }
                else
                {
                    foreach (Shape s in shapes)
                        s.isSelect = false;
                    pnlPaint.Invalidate();
                    for (int i = 0; i < shapes.Count; i++)
                    {
                        if (shapes[i].IsClick(e.Location))
                        {
                            selectedShape = shapes[i];
                            shapes[i].isSelect = true;

                            if (!(shapes[i] is ShapeSet))
                            {
                                nmrSize.Value = int.Parse(shapes[i].myPen.Width.ToString());
                                btnColor.BackColor = shapes[i].myPen.Color;
                                ckbFill.Checked = shapes[i].isFilled;
                            }

                            pnlPaint.Invalidate();
                            break;
                        }
                    }

                    if (selectedShape != null)
                    {
                        isMovingShape = true;
                        previousPoint = e.Location;
                    }
                    else
                    {
                        isMouseSelect = true;
                        selectedRegion = new System.Drawing.Rectangle(e.Location, new Size(0, 0));
                    }
                }
            }
            else
            {
                isMouseDown = true;
                shapes.ForEach(shape => shape.isSelect = false);

                if (currentShape == CurrentShape.Line)
                {
                    Pen tPen = new Pen(btnColor.BackColor, int.Parse(nmrSize.Value.ToString()));
                    tPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                    Line line = new Line
                    {
                        startPoint = e.Location,
                        myPen = tPen
                    };
                    shapes.Add(line);
                }
                else if (currentShape == CurrentShape.Rectangle)
                {
                    Pen tPen = new Pen(btnColor.BackColor, int.Parse(nmrSize.Value.ToString()));
                    tPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                    Models.Rectangle rectangle = new Models.Rectangle
                    {
                        startPoint = e.Location,
                        myPen = tPen,
                    };

                    if (mode == ShapeMode.Fill)
                    {
                        rectangle.isFilled = true;
                    }

                    shapes.Add(rectangle);
                }
                else if (currentShape == CurrentShape.Ellipse)
                {
                    Pen tPen = new Pen(btnColor.BackColor, int.Parse(nmrSize.Value.ToString()));
                    tPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                    Ellipse ellipse = new Ellipse
                    {
                        startPoint = e.Location,
                        myPen = tPen,
                    };

                    if (mode == ShapeMode.Fill)
                    {
                        ellipse.isFilled = true;
                    }

                    shapes.Add(ellipse);
                }
                else if (currentShape == CurrentShape.Polygon)
                {

                    if (!isDrawPolygon)
                    {
                        Pen tPen = new Pen(btnColor.BackColor, int.Parse(nmrSize.Value.ToString()));
                        tPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                        Polygon polygon = new Polygon
                        {
                            myPen = tPen,
                        };

                        if (mode == ShapeMode.Fill)
                        {
                            polygon.isFilled = true;
                        }

                        polygon.Points.Add(e.Location);
                        polygon.Points.Add(e.Location);

                        shapes.Add(polygon);

                        isDrawPolygon = true;
                    }
                    else
                    {
                        Polygon polygon = shapes[shapes.Count - 1] as Polygon;

                        polygon.Points[polygon.Points.Count - 1] = e.Location;
                        polygon.Points.Add(e.Location);
                    }

                    isMouseDown = false;
                }
                else if (currentShape == CurrentShape.Bezier)
                {
                    if (!isDrawBezier)
                    {
                        Pen tPen = new Pen(btnColor.BackColor, int.Parse(nmrSize.Value.ToString()));
                        tPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                        Bezier bezier = new Bezier
                        {
                            myPen = tPen,
                        };

                        bezier.Points.Add(e.Location);
                        bezier.Points.Add(e.Location);

                        shapes.Add(bezier);

                        isDrawBezier = true;
                    }
                    else
                    {
                        Bezier bezier = shapes[shapes.Count - 1] as Bezier;
                        bezier.Points[bezier.Points.Count - 1] = e.Location;

                        bezier.Points.Add(e.Location);
                    }
                    isMouseDown = false;
                }
            }
        }

        private void pnlPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                shapes[shapes.Count - 1].endPoint = e.Location;
                pnlPaint.Invalidate();
            }
            else if (isMovingShape)
            {
                Point d = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                selectedShape.Move(d);
                previousPoint = e.Location;

                pnlPaint.Invalidate();
            }
            else if (currentShape == CurrentShape.NoDrawing)
            {
                if (isMouseSelect)
                {
                    selectedRegion.Width = e.Location.X - selectedRegion.X;
                    selectedRegion.Height = e.Location.Y - selectedRegion.Y;

                    pnlPaint.Invalidate();
                }
                else
                {
                    if (shapes.Exists(shape => shape.IsClick(e.Location)))
                    {
                        pnlPaint.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        pnlPaint.Cursor = Cursors.Default;
                    }
                }
            }

            if (isDrawPolygon)
            {
                Polygon polygon = shapes[shapes.Count - 1] as Polygon;
                polygon.Points[polygon.Points.Count - 1] = e.Location;

                pnlPaint.Invalidate();
            }
            else if (isDrawBezier)
            {
                Bezier bezier = shapes[shapes.Count - 1] as Bezier;
                bezier.Points[bezier.Points.Count - 1] = e.Location;

                pnlPaint.Invalidate();
            }
        }

        private void pnlPaint_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            if (isMovingShape)
            {
                isMovingShape = false;
                selectedShape = null;
            }
            else if (isMouseSelect)
            {
                isMouseSelect = false;
                for (int i = 0; i < shapes.Count; i++)
                {
                    shapes[i].isSelect = false;

                    if (shapes[i].startPoint.X >= selectedRegion.X && shapes[i].endPoint.X <= selectedRegion.X + selectedRegion.Width && shapes[i].startPoint.Y >= selectedRegion.Y && shapes[i].endPoint.Y <= selectedRegion.Y + selectedRegion.Height)
                    {
                        shapes[i].isSelect = true;
                    }
                }

                pnlPaint.Invalidate();
            }
        }

        //End draw bezier and polygon
        private void pnlPaint_DoubleClick(object sender, EventArgs e)
        {
            if (isDrawPolygon)
            {
                isDrawPolygon = false;

                Polygon polygon = shapes[shapes.Count - 1] as Polygon;
                polygon.Points.RemoveAt(polygon.Points.Count - 1);

                pnlPaint.Invalidate();

                FindPolygonRegion(polygon);
            }
            else if (isDrawBezier)
            {
                isDrawBezier = false;

                Bezier bezier = shapes[shapes.Count - 1] as Bezier;
                bezier.Points.RemoveAt(bezier.Points.Count - 1);
                bezier.Points.RemoveAt(bezier.Points.Count - 1);
                pnlPaint.Invalidate();
                FindBezierRegion(bezier);
            }

        }
        #endregion

        #region Controls Event
        //Event select shapes
        private void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (Shape s in shapes)
                s.isSelect = false;
            pnlPaint.Invalidate();

            currentShape = CurrentShape.NoDrawing;
            UncheckButton();
            btnSelect.BackColor = Color.SkyBlue;
            pnlPaint.Cursor = Cursors.Default;
        }

        //Event Ctrl down from keyboard
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            isControlKeyPress = e.Control;
        }

        //Event Ctrl up from keyboard
        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            isControlKeyPress = e.Control;
        }

        //Group shapes
        private void btnGroup_Click(object sender, EventArgs e)
        {
            UncheckButton();
            if (shapes.Count(shape => shape.isSelect) > 1)
            {
                ShapeSet group = new ShapeSet();

                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].isSelect)
                    {
                        group.Add(shapes[i]);
                        shapes.RemoveAt(i);
                        i--;
                    }
                }

                FindGroupRegion(group);
                shapes.Add(group);
                group.isSelect = true;
                pnlPaint.Invalidate();
            }
        }

        //Ungroup shapes
        private void btnUnGroup_Click(object sender, EventArgs e)
        {
            UncheckButton();
            if (shapes.Find(shape => shape.isSelect) is ShapeSet selectedGroup)
            {
                foreach (Shape shape in selectedGroup)
                {
                    shapes.Add(shape);
                }
                shapes.Remove(selectedGroup);
            }
            pnlPaint.Invalidate();
        }

        //Delete shapes
        private void btnDelete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].isSelect)
                {
                    shapes.RemoveAt(i);
                    i--;
                }
            }
            pnlPaint.Invalidate();
        }

        //Change draw mode (draw or fill)
        private void ckbFill_CheckedChanged(object sender, EventArgs e)
        {
            bool fill = false;
            UncheckButton();
            currentShape = CurrentShape.NoDrawing;
            if (!ckbFill.Checked)
            {
                mode = ShapeMode.NoFill;
                EnableButtons();
                cbbDashStyle.Enabled = true;
                nmrSize.Enabled = true;
                fill = false;
            }
            else
            {
                mode = ShapeMode.Fill;
                nmrSize.Enabled = false;
                btnLine.Enabled = btnBezier.Enabled = false;
                cbbDashStyle.Enabled = false;

                fill = true;
            }
            shapes.FindAll(shape => shape.isSelect).ForEach(shape =>
            {
                if (shape is ShapeSet group)
                {
                    foreach (Shape s in group)
                    {
                        s.isFilled = fill;
                    }
                }
                else
                {
                    shape.isFilled = fill;
                }
            });
            pnlPaint.Invalidate();
        }

        //Change Color
        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if (color.ShowDialog() == DialogResult.OK)
            {
                btnColor.BackColor = color.Color;
            }
            shapes.FindAll(shape => shape.isSelect).ForEach(shape =>
            {
                if (shape is ShapeSet group)
                {
                    foreach (Shape s in group)
                    {
                        s.myPen.Color = btnColor.BackColor;
                    }
                }
                else
                {
                    shape.myPen.Color = btnColor.BackColor;
                }
            });
            pnlPaint.Invalidate();
        }

        //Change style
        private void cbbDashStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            shapes.FindAll(shape => shape.isSelect).ForEach(shape =>
            {
                if (shape is ShapeSet group)
                {
                    foreach (Shape s in group)
                    {
                        s.myPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                    }
                }
                else
                {
                    shape.myPen.DashStyle = (DashStyle)cbbDashStyle.SelectedIndex;
                }
            });
            pnlPaint.Invalidate();
        }

        //Change size
        private void nmrSize_ValueChanged(object sender, EventArgs e)
        {
            shapes.FindAll(shape => shape.isSelect).ForEach(shape =>
            {
                if (shape is ShapeSet group)
                {
                    foreach (Shape s in group)
                    {
                        s.myPen.Width = int.Parse(nmrSize.Value.ToString());
                    }
                }
                else
                {
                    shape.myPen.Width = int.Parse(nmrSize.Value.ToString());
                }
            });
            pnlPaint.Invalidate();
        }

        //Event click of button choose shape
        private void btnShapes_Click(object sender, EventArgs e)
        {
            shapes.ForEach(shape => shape.isSelect = false);
            pnlPaint.Invalidate();

            Button btn = sender as Button;
            if (btn.BackColor == Color.SkyBlue)
            {
                UncheckButton();
                currentShape = CurrentShape.NoDrawing;
                pnlPaint.Cursor = Cursors.Default;
                btnSelect.BackColor = Color.SkyBlue;
            }
            else
            {
                UncheckButton();
                pnlPaint.Cursor = Cursors.Cross;
                btn.BackColor = Color.SkyBlue;
                switch(int.Parse(btn.Tag.ToString()))
                {
                    case 0: currentShape = CurrentShape.Line; break;
                    case 1: currentShape = CurrentShape.Rectangle; break;
                    case 2: currentShape = CurrentShape.Ellipse; break;
                    case 3: currentShape = CurrentShape.Bezier; break;
                    case 4: currentShape = CurrentShape.Polygon; break;
                    default: currentShape = CurrentShape.NoDrawing;
                        MessageBox.Show("Can't Choose This Shape!");
                        break;
                }
            }
        }

        //Event Zoom shapes
        private void Zoom_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag.ToString() == "+")
                zoom += 0.1f;
            else if (btn.Tag.ToString() == "-")
                zoom -= 0.1f;
            else
                zoom = 1;
            lblScale.Text = (int)((zoom - 1) * 100) + "%";
            pnlPaint.Invalidate();
        }
        #endregion
    }
}
