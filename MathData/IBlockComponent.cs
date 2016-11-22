using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public interface IBlockComponent
    {
        void AddChild(IBlockComponent blockComponent, Point rowPoint);
        void AddShapeChild();
        void RemoveCharChild();
        void LayoutChildren();

        Rect CreateRect(Point rowPoint);

        Rect GetRect();
    }
}
