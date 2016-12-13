using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class SuperscriptComponent : BlockComponentBase, IBlockComponent
    {
        private double fontHeight = 18.4;
        public double FontSize
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }


        public double SuperscriptFontSize
        {
            get { return 0.5 * fontHeight; }
        }

        /// <summary>
        /// 初始化默认大小
        /// </summary>
        /// <param name="rowPoint"></param>
        public SuperscriptComponent(Point rowPoint)
        {
            Children = new List<List<IBlockComponent>>();
            Size defaultSize = new Size(14, fontHeight * 1.5);
            Point location = new Point(rowPoint.X, rowPoint.Y);
            this.Rect = new Rect(location, defaultSize);

            var child0 = new List<IBlockComponent>();
            var child1 = new List<IBlockComponent>();

            Children.Add(child0);
            Children.Add(child1);
        }

        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public void AddShapeChild()
        {
            throw new NotImplementedException();
        }

        public Rect CreateRect(Point rowPoint)
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

        public double GetHorizontialAlignmentYOffset()
        {
            if (Children[0].Count == 0)
            {
                return this.Rect.Top+this.Rect.Height * 2 / 3;
            }
            else
            {
                var baseRect = GetChildRect(Children[0]);
              
                var baseCenterYOffset = BlockComponentTools.GetChildrenMaxCenterLine(Children[0]);

                return baseCenterYOffset;
            }

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
                //指数部分
                if (child == Children[1])
                {
                    return new Rect(this.Rect.Location, new Size(4, 0.5 * fontHeight));
                }

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

            var baseChild = Children[0];
            var baseRect = GetChildRect(baseChild);
            if (CurrentInputPart == 0)
            {
                nextPartLocation = new Point(this.Rect.Left + baseRect.Width, rowTop + baseRect.Top - 0.5 * fontHeight);
                CurrentInputPart++;
                isInputFinished = false;
            }
            else
            {
                var topRect = GetChildRect(Children[1]);
                double centerYOffset = BlockComponentTools.GetChildrenMaxCenterLine(Children[0]);
                nextPartLocation = new Point(this.Rect.Left + baseRect.Width+topRect.Width, rowTop + centerYOffset-0.5*fontHeight);

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

        public override Vector GetRedirectCaretVector()
        {
            return new Vector(0, 0.5 * fontHeight);
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
