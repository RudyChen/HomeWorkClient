using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class LineBlock : ShapeBlock, IBlockComponent
    {
        public LineBlock(Point rowPoint)
        {
            CreateRect(rowPoint);
        }

        private double width=10;

        private double height=1;

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


        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
           
        }

        public void AddShapeChild()
        {
           
        }

        public Rect CreateRect(Point rowPoint)
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

        public void LayoutChildren()
        {
            throw new NotImplementedException();
        }

        public void RemoveCharChild()
        {
            throw new NotImplementedException();
        }
    }
}
