using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathData
{
    public interface IBlockComponent
    {
        void AddCharChild();
        void AddShapeChild();
        void RemoveCharChild();
        void LayoutChildren();
    }
}
