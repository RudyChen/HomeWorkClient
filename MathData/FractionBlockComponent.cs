using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class FractionBlockComponent : BlockComponent, IBlockComponent
    {
        private int currentInputPart;
        private double fontHeight = 18;

        public FractionBlockComponent(Point rowPoint)
        {
            Children = new List<List<IBlockComponent>>();
            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();
            var shapeChild = new List<IBlockComponent>();
            Children.Add(child0);
            Children.Add(child1);
            Children.Add(shapeChild);
            Size size = new Size(10, fontHeight * 2 + fontHeight * 0.2);
            this.Rect = new Rect(rowPoint, size);
        }

        /// <summary>
        /// 当前输入部分
        /// 0代表分子，1代表分母
        /// </summary>
        public int CurrentInputPart
        {
            get { return currentInputPart; }
            set { currentInputPart = value; }
        }


        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
            //添加元素导致Block大小改变
            if (blockComponent is BlockComponent)
            {
                var baseBlockComponent = blockComponent as BlockComponent;
                baseBlockComponent.SizeChangedEvent += BlockBaseComponent_SizeChangedEvent;


               
            }
            else
            {
                var inputCharBlock = blockComponent as CharBlock;
                var size = inputCharBlock.GetSize();
                inputCharBlock.RowRect = new Rect(rowPoint, size);               
                this.OnSizeChanged(size);
            }

            //加入child元素
            if (currentInputPart == 0)
            {
                Children[0].Add(blockComponent);
            }
            else if (currentInputPart == 1)
            {
                Children[1].Add(blockComponent);
            }
        }

        private void BlockBaseComponent_SizeChangedEvent(Size size)
        {
            //this.OnSizeChangedEvent()
            throw new NotImplementedException();
        }

        public void AddShapeChild()
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

        public Rect GetRect(Point rowPoint)
        {
            throw new NotImplementedException();
        }
    }
}
