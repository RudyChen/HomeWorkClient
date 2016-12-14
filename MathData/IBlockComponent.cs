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
        Rect GetRect();

        double GetHorizontialAlignmentYOffset();

        void Move(Vector offsetVector);
    }
}
