using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class SuperscriptComponent : BlockComponent, IBlockComponent
    {
        private double fontHeight = 18;
        public double FontSize
        {
            get { return fontHeight; }
            set { fontHeight = value; }
        }
   

        public double SuperscriptFontSize
        {
            get { return 0.5* fontHeight; }           
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
            return this.Rect.Top + this.Rect.Height * 2 / 3;
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
                    return new Rect(this.Rect.Location, new Size(4, 0.5*fontHeight));
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
            if (CurrentInputPart==0)
            {
                CurrentInputPart++;
                isInputFinished = false;
            }
            else
            {
                isInputFinished = true;
            }

            return nextPartLocation;
        }

    }
}
