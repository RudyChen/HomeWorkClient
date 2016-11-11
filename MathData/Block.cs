using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
   public abstract class Block
    {
        private Rect rowRect;

        public Rect RowRect
        {
            get { return rowRect; }
            set { rowRect = value; }
        }


    }
}
