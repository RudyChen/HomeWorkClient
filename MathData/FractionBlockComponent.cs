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

        private double fontHeight = 18;
        public double FontSize
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }


        private double fractionSpace=6;

        public double FractionSpace
        {
            get { return fractionSpace; }
            set { fractionSpace = value; }
        }


        public FractionBlockComponent(Point rowPoint,LineBlock fractionLineData)
        {
            Children = new List<List<IBlockComponent>>();
            Size size = new Size(10, fontHeight * 2 + 2*fractionSpace);
            this.Rect = new Rect(rowPoint, size);
            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();
            var shapeChild = new List<IBlockComponent>();
            shapeChild.Add(fractionLineData);

            Children.Add(child0);
            Children.Add(child1);
            Children.Add(shapeChild);

          

        }

        public Point GetNextPartLocation()
        {
            Point nextPartLocation = this.Rect.Location;
            if (CurrentInputPart == 0)
            {
                var topChild = Children[CurrentInputPart];
                var topRect = GetChildRect(topChild);
                //加上分数分割线高度
                nextPartLocation = new Point(this.Rect.Left, this.Rect.Top + topRect.Height + 2*fractionSpace+2);
            }
            else
            {
                var topChild = Children[CurrentInputPart];
                var topRect = GetChildRect(topChild);
                nextPartLocation = new Point(this.Rect.Left + this.Rect.Width, this.Rect.Top + topRect.Height + fontHeight * 0.1-fontHeight/2);
            }

            return nextPartLocation;
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
                inputCharBlock.Rect = new Rect(rowPoint, size);
                this.OnSizeChanged(size);
            }

            //加入child元素
            if (CurrentInputPart == 0)
            {
                Children[0].Add(blockComponent);
            }
            else if (CurrentInputPart == 1)
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

        public Rect CreateRect(Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public Rect GetRect()
        {
            return this.Rect;
        }

        public double GetHorizontialAlignmentYOffset()
        {
            var topChild = Children[0];
            var topRect = GetChildRect(topChild);
            return topRect.Top+ topRect.Height + fontHeight * 0.1;
        }

        /// <summary>
        /// 更新分数线长度
        /// </summary>
        public override void UpdateShapeBlocks()
        {
            LineBlock separateLineBlock = Children[2][0] as LineBlock;
            var firstPartRect = GetChildRect(Children[0]);

            Point lineStart = new Point(firstPartRect.Left, firstPartRect.Height + fractionSpace + 2);
            separateLineBlock.Rect = new Rect(lineStart,new Size(this.Rect.Width,2));

            base.UpdateShapeBlocks();
        }
    }
}
