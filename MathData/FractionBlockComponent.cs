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


       

        public double FractionSpace
        {
            get { return 0.5* fontHeight; }           
        }


        public FractionBlockComponent(Point rowPoint,LineBlock fractionLineData)
        {
            Children = new List<List<IBlockComponent>>();
            Size size = new Size(10, fontHeight * 2 + 2* FractionSpace);
            this.Rect = new Rect(rowPoint, size);
            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();
            var shapeChild = new List<IBlockComponent>();
            shapeChild.Add(fractionLineData);

            Children.Add(child0);
            Children.Add(child1);
            Children.Add(shapeChild);

          

        }

        public Point GetNextPartLocation(double rowTop)
        {
            Point nextPartLocation = this.Rect.Location;
            var topChild = Children[0];
            var topRect = GetChildRect(topChild);
            if (CurrentInputPart == 0)
            {
                nextPartLocation = new Point(this.Rect.Left, this.Rect.Top + topRect.Height + 2* FractionSpace);
            }
            else
            {
                var lineBlock = Children[2][0] as LineBlock;
                nextPartLocation = new Point(this.Rect.Left + this.Rect.Width,this.Rect.Top+ lineBlock.Rect.Top);
                UpdateRect();
            }

            return nextPartLocation;
        }

        public void UpdateRect()
        {
            var firstChild = GetChildRect(Children[0]);
            var secondChild = GetChildRect(Children[1]);
            var width = firstChild.Width > secondChild.Width ? firstChild.Width : secondChild.Width;
            var height =firstChild.Top+ secondChild.Height + firstChild.Height+2*FractionSpace;
            this.Rect = new Rect(firstChild.Location, new Size(width, height));
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
            var topRect = GetChildRect(Children[0]);       
            return  topRect.Top+topRect.Height+FractionSpace-1;
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

            Point lineStart = new Point(firstPartRect.Left, firstPartRect.Height + FractionSpace -1);
            separateLineBlock.Rect = new Rect(lineStart,new Size(width, 2));

            base.UpdateShapeBlocks();
        }

        public override Rect GetChildRect(List<IBlockComponent> child)
        {
            double width = 0;
            double heigh = 0;
            Point mostLeftPoint = new Point(0, 0);
            if (child.Count == 0)
            {
                return new Rect(this.Rect.Location, new Size(10, fontHeight));
            }
            else
            {
                foreach (var item in child)
                {
                    var childItemRect = item.GetRect();
                    width += childItemRect.Width;
                    heigh = childItemRect.Height > heigh ? childItemRect.Height : heigh;
                    if (mostLeftPoint.X > childItemRect.Left)
                    {
                        mostLeftPoint = new Point(childItemRect.Left, 0);
                    }
                }
            }

            return new Rect(mostLeftPoint, new Size(width, heigh));
        }
    }
}
