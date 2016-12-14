using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    /// <summary>
    /// 根式组合块
    /// </summary>
    public class RadicalComponent : BlockComponentBase, IBlockComponent
    {
        private double fontHeight = 18.4;
        public double FontSize
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }

        public RadicalComponent(Point rowPoint)
        {

        }
             
        private Rect CreateRect(Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public double GetHorizontialAlignmentYOffset()
        {
            var maxCenterY = BlockComponentTools.GetChildrenMaxCenterLine(Children[0]);
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
                return new Rect(this.Rect.Location, new Size(10, fontHeight));
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
            //todo:下一部分位置

            return nextPartLocation;
        }

        public override void UpdateRect()
        {
            Rect componentRect = new Rect(Rect.Location, new Size(0, 0));

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
    }
}
