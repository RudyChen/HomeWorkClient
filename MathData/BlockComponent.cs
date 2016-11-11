using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathData
{
    public abstract class BlockComponent 
    {
        private List<List<IBlockComponent>> children=new List<List<IBlockComponent>>();

        public List<List<IBlockComponent>> Children
        {
            get { return children; }
            set { children = value; }
        }

    }
}
