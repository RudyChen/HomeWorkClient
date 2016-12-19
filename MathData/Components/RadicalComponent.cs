using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    [Serializable]
    /// <summary>
    /// 根式组合块
    /// </summary>
    public class RadicalComponent : BlockComponentBase, IBlockComponent
    {
        /// <summary>
        /// 根号符号第一点比例
        /// </summary>
        public static readonly double SYMBOL_POINT1_RATIO = 0.875;
        /// <summary>
        /// 根号符号第2点比例
        /// </summary>
        public static readonly double SYMBOL_POINT2X_RATIO = 0.33333;
        /// <summary>
        /// 根号符号第2点比例
        /// </summary>
        public static readonly double SYMBOL_POINT2Y_RATIO = 0.66666;
        /// <summary>
        /// 根号符号第3点比例
        /// </summary>
        public static readonly double SYMBOL_POINT3X_RATIO = 0.66666;
        /// <summary>
        /// 根号符号第4点比例
        /// </summary>
        public static readonly double SYMBOL_POINT4X_RATIO = 1;
        /// <summary>
        /// 根号符号第5点比例
        /// </summary>
        public static readonly double SYMBOL_POINT5X_RATIO = 1;

        private double fontHeight = 18.4;

        private double firstDefaultPartWidth = 10;

        private double secondDefaultPartWidth = 10;

        private bool isNeedRootIndex = false;

        public bool IsNeedRootIndex
        {
            get { return isNeedRootIndex; }
            set { isNeedRootIndex = value; }
        }


        public double SecondDefaultPartWidth
        {
            get { return secondDefaultPartWidth; }
            set { secondDefaultPartWidth = value; }
        }


        public double FirstDefaultPartWidth
        {
            get { return firstDefaultPartWidth; }
            set { firstDefaultPartWidth = value; }
        }

        public double FontHeight
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }

        public RadicalComponent()
        {

        }

        public RadicalComponent(Point rowPoint, PolylineBlock radicalSymbol)
        {
            Children = new List<List<IBlockComponent>>();
            this.Rect = new Rect(radicalSymbol.Rect.Location, new Size(firstDefaultPartWidth + secondDefaultPartWidth, fontHeight));
            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();
            var shapeChild = new List<IBlockComponent>();

            if (!isNeedRootIndex)
            {
                CurrentInputPart = 1;                
            }

            shapeChild.Add(radicalSymbol);
            ShapeChildIndex = 2;

            Children.Add(child0);
            Children.Add(child1);
            Children.Add(shapeChild);
        }

        public double GetHorizontialAlignmentYOffset()
        {
            double maxCenterY = 0;
            if (Children[1].Count>0)
            {
                maxCenterY= BlockComponentTools.GetChildrenMaxCenterLine(Children[1]);
            }
            else
            {
               // maxCenterY = BlockComponentTools.GetChildrenMaxCenterLine(Children[0]);
            }
            
            return maxCenterY;
        }

        public Rect GetRect()
        {
            return this.Rect;
        }

        public override Rect GetChildRect(List<IBlockComponent> child)
        {
            Point mostLeftPoint = new Point(0, 0);
            if (child.Count == 0)
            {
                return new Rect(this.Rect.Location, new Size(firstDefaultPartWidth, fontHeight));
            }
            else
            {
                Rect totalRect = new Rect(this.Rect.Location, new Size(0, 0));
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
            var firstRect = GetChildRect(Children[0]);
            if (CurrentInputPart == 0)
            {
                nextPartLocation = new Point(this.Rect.Left + firstRect.Width + 2, rowTop + this.Rect.Top);
                CurrentInputPart++;
                isInputFinished = false;
            }
            else if (CurrentInputPart == 1)
            {
                var centerY = BlockComponentTools.GetChildrenMaxCenterLine(Children[1]);
                nextPartLocation = new Point(this.Rect.Left + this.Rect.Width + 2, centerY);
              
                isInputFinished = true;
            }

            return nextPartLocation;
        }

        public override void UpdateRect()
        {
            Rect componentRect = new Rect(Rect.Location, new Size(this.Rect.Width, this.Rect.Height));

            if (Children[1].Count == 0)
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

        public void Move(Vector offsetVector)
        {
            foreach (var item in Children)
            {
                BlockComponentTools.MoveBlockComponents(item, offsetVector);
            }
        }

        public override void UpdateOtherChildrenLocation(Vector offsetVector)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (i != CurrentInputPart)
                {
                    BlockComponentTools.MoveBlockComponents(Children[i], offsetVector);
                }
            }
        }

        public override void UpdateShapeBlocks()
        {
            if (Children[0].Count > 0)
            {
                var firstRect = GetChildRect(Children[0]);
                firstDefaultPartWidth = firstRect.Width;
            }
            var secondRect = Children[1].Count == 0 ? new Rect(this.Rect.Location, new Size(secondDefaultPartWidth, fontHeight)) : GetChildRect(Children[1]);


            Point point1 = new Point(Rect.X, Rect.Y + Rect.Height * SYMBOL_POINT1_RATIO);
            Point point2 = new Point(Rect.X + firstDefaultPartWidth * SYMBOL_POINT2X_RATIO, Rect.Y + Rect.Height * SYMBOL_POINT2Y_RATIO);
            Point point3 = new Point(Rect.X + firstDefaultPartWidth* SYMBOL_POINT3X_RATIO, Rect.Y + Rect.Height);
            Point point4 = new Point(Rect.X + firstDefaultPartWidth * SYMBOL_POINT4X_RATIO, Rect.Y);
            Point point5 = new Point(Rect.X + firstDefaultPartWidth * SYMBOL_POINT5X_RATIO + secondRect.Width, Rect.Y);

            PolylineBlock newPolylineBlock = new PolylineBlock(this.Rect.Location);
            var symbolBlock = Children[ShapeChildIndex][0] as PolylineBlock;
            symbolBlock.PolylinePoints.Clear();
            symbolBlock.PolylinePoints.Add(point1);
            symbolBlock.PolylinePoints.Add(point2);
            symbolBlock.PolylinePoints.Add(point3);
            symbolBlock.PolylinePoints.Add(point4);
            symbolBlock.PolylinePoints.Add(point5);
        }
    }
}
