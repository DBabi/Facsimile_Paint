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
    /// Abstract Class Model
    /// </summary>
    abstract class Shape
    {
        //Point of top left
        public Point startPoint { get; set; }
        //Point of bottom right
        public Point endPoint { get; set; }
        //Use to paint
        public Pen myPen { get; set; }
        //Shape is selected
        public bool isSelect { get; set; }
        //Shape is filled
        public bool isFilled { get; set; }
        //Name of shape
        public string name { get; protected set; }

        #region Method

        protected abstract GraphicsPath GraphicsPath { get; }

        //When user click on shape
        public abstract bool IsClick(Point point);

        //Draw or fill shape
        public abstract void Draw(Graphics graphics);

        //Move shape
        public abstract void Move(Point distance);

        //Clone object
        public abstract object Clone();

        public override string ToString()
        {
            return $"{name} [({startPoint.X}, {startPoint.Y}); ({endPoint.X}, {endPoint.Y})]";
        }
        #endregion
    }
}
