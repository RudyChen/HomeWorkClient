using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class FractionBlockComponent : BlockComponentBase, IBlockComponent
    {

        private double fontHeight = 18;
        public double FontSize
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }

        public double FractionSpace
        {
            get { return 0.2* fontHeight; }           
        }

        public FractionBlockComponent(Point rowPoint,LineBlock fractionLineData)
        {
            Children = new List<List<IBlockComponent>>();
            Size size = new Size(10, fontHeight * 2.5);
            Point location = new Point(rowPoint.X, rowPoint.Y);
            this.Rect = new Rect(location, size);
            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();
            var shapeChild = new List<IBlockComponent>();
            shapeChild.Add(fractionLineData);

            Children.Add(child0);
            Children.Add(child1);
            Children.Add(shapeChild);
        }

        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
            //添加元素导致Block大小改变
            if (blockComponent is BlockComponentBase)
            {
                var baseBlockComponent = blockComponent as BlockComponentBase;
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
            var topRect = GetChildRect(Children[0]);       
            return  topRect.Top+topRect.Height+0.3*fontHeight;
        }

        /// <summary>
        /// 更新分数线长度
        /// </summary>
        public override void UpdateShapeBlocks()
        {
            LineBlock separateLineBlock = Children[2][0] as LineBlock;
            var firstPartRect = GetChildRect(Children[0]);
            var secondPartRect = GetChildRect(Children[1]);
            double width = secondPartRect.Width;
            if (firstPartRect.Width>=secondPartRect.Width)
            {
                width = firstPartRect.Width;
            }

            ///为了上下对称，分数分子分母间隔空隙不能用成平均的值
            Point lineStart = new Point(firstPartRect.Left,this.Rect.Top+ firstPartRect.Height + 0.3*fontHeight);
            separateLineBlock.Rect = new Rect(lineStart,new Size(width, 2));

            base.UpdateShapeBlocks();
        }

        public override Rect GetChildRect(List<IBlockComponent> child)
        {
            Point mostLeftPoint = new Point(0, 0);
            if (child.Count == 0)
            {
                return new Rect(this.Rect.Location, new Size(10, fontHeight));
            }
            else
            {
                Rect totalRect = new Rect(this.Rect.Location,new Size(0,0));
                bool isFirst = true;
                foreach (var item in child)
                {
                   
                    var childItemRect = item.GetRect();
                    if (isFirst)
                    {
                        totalRect = childItemRect;
                        isFirst = false;
                    }
                    else
                    {
                        totalRect.Union(childItemRect);
                    }                   
                }

                return totalRect;
            }            
        }

        public override Point GetNextPartLocation(double rowTop, ref bool isInputFinished)
        {
            Point nextPartLocation = this.Rect.Location;
            var topChild = Children[0];
            var topRect = GetChildRect(topChild);
            if (CurrentInputPart == 0)
            {
                nextPartLocation = new Point(this.Rect.Left, rowTop + this.Rect.Top + topRect.Height + 0.4 * fontHeight);
                CurrentInputPart++;
                isInputFinished = false;
            }
            else
            {
                var centerYOffset = GetHorizontialAlignmentYOffset();

                nextPartLocation = new Point(this.Rect.Left + this.Rect.Width, rowTop + centerYOffset - 0.5 * fontHeight);
                UpdateRect();
                isInputFinished = true;
            }

            return nextPartLocation;
        }

        public override void UpdateRect()
        {
            Rect componentRect = new Rect(Rect.Location, new Size(0, 0));
            if (IsEditComponent())
            {
                return;
            }

            foreach (var item in Children)
            {
                var childRect = GetChildRect(item);
                componentRect.Union(childRect);
            }

            this.Rect = componentRect;
        }

        private bool IsEditComponent()
        {
            foreach (var item in Children)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
