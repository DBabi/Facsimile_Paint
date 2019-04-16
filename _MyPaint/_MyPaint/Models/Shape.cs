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

        protected bool headIsControl;
        public Shape()
        {
            myPen = new Pen(Color.Black, 1f);
            startPoint = new Point(0, 0);
            endPoint = new Point(0, 0);
            isSelect = false;
            isFilled = false;
            headIsControl = false;
        }
        
        #region Method

        protected abstract GraphicsPath GraphicsPath { get; }

        /// <summary>
        /// When shapes be click
        /// </summary>
        /// <param name="point"></param>
        /// <returns>return 1 : click inside
        ///          return 0 : click border
        ///          return -1: click outside </returns>
        public virtual int IsClick(Point point)
        {
            if (IsPointControl(point))
                return 1;
            else if (GetOutLine(point))
                return 0;
            return -1;
        }

        //Checkout this point is point control to rezise
        protected virtual bool IsPointControl(Point point)
        {
            if (point.X >= endPoint.X - myPen.Width / 2 - 3 && point.X <= endPoint.X + myPen.Width / 2 + 3)
            {
                if (point.Y >= endPoint.Y - myPen.Width / 2 - 3 && point.Y <= endPoint.Y + myPen.Width / 2 + 3)
                {
                    headIsControl = false;
                    return true;
                }
            }
            else if (point.X >= startPoint.X - myPen.Width / 2 - 3 && point.X <= startPoint.X + myPen.Width / 2 + 3)
            {
                if (point.Y >= startPoint.Y - myPen.Width / 2 - 3 && point.Y <= startPoint.Y + myPen.Width / 2 + 3)
                {
                    headIsControl = true;
                    return true;
                }
            }
            return false;
        }

        public virtual void Resize(Point point)
        {
            if (headIsControl)
                startPoint = point;
            else
                endPoint = point;
        }

        //Get of out line object
        protected abstract bool GetOutLine(Point point);
        //Draw or fill shape
        public abstract void Draw(Graphics graphics);

        //Move shape
        public abstract void Move(Point distance);

        #endregion
    }
}
