using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class ShapeBlock : Block, IBlockComponent
    {
        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public void AddShapeChild()
        {
            throw new NotImplementedException();
        }

        public Rect CreateRect(Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public Rect GetRect()
        {
            throw new NotImplementedException();
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
