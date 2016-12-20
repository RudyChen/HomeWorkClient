using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    [Serializable]
    public class LineBlock : ShapeBlock, IBlockComponent
    {
        public LineBlock()
        {
                
        }

        public LineBlock(Point rowPoint)
        {
            CreateRect(rowPoint);
        }

        private double width=10;

        private double height=1;

        private Point startPoint;

        private Point endPoint;

        private string stroke="Black";

        private double strokeThickness=1;

        public double StrokeThickness
        {
            get { return strokeThickness; }
            set { strokeThickness = value; }
        }


        public string Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }


        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }


        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }


        public double DefaultHeight
        {
            get { return height; }
            set { height = value; }
        }


        public double DefaultWidth
        {
            get { return width; }
            set { width = value; }
        }

        private Rect CreateRect(Point rowPoint)
        {
            this.Rect= new Rect(rowPoint, new Size(width, height));
            return Rect;
        }

        public double GetHorizontialAlignmentYOffset()
        {
            throw new NotImplementedException();
        }

        public Rect GetRect()
        {
            return this.Rect;
        }
     

        public void Move(Vector offsetVector)
        {
            this.Rect = new Rect(this.Rect.X + offsetVector.X, this.Rect.Y + offsetVector.Y, this.Rect.Width, this.Rect.Height);
        }
    }
}
