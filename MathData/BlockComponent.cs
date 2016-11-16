using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public abstract class BlockComponent 
    {
        private List<List<IBlockComponent>> children=new List<List<IBlockComponent>>();

        private Rect rect;

        public Rect Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public delegate void SizeChangedEventHandler(Size size);
        public event SizeChangedEventHandler SizeChangedEvent;

        public List<List<IBlockComponent>> Children
        {
            get { return children; }
            set { children = value; }
        }

        protected virtual void OnSizeChanged(Size size)
        {
            if (this.SizeChangedEvent != null)
            {
                this.SizeChangedEvent(size);
            }
        }


    }
}
