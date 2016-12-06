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

        private int currentInputPart;

        private Rect rect;

        public Rect Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public void SetRect(Rect rect)
        {
            this.Rect = rect;
        }

        public delegate void SizeChangedEventHandler(Size size);
        public event SizeChangedEventHandler SizeChangedEvent;

        public List<List<IBlockComponent>> Children
        {
            get { return children; }
            set { children = value; }
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

        protected virtual void OnSizeChanged(Size size)
        {
            if (this.SizeChangedEvent != null)
            {
                this.SizeChangedEvent(size);
            }
        }

        public virtual Rect GetChildRect(List<IBlockComponent> child)
        {
            return new Rect(0, 0, 0, 0);
        }

        public Rect GetInputChildRect()
        {
           var rect= GetChildRect(children[currentInputPart]);
            return rect;
        }

        /// <summary>
        /// 子类更新图形块
        /// </summary>
        public virtual void UpdateShapeBlocks()
        {

        }

        /// <summary>
        /// 更新组合块大小
        /// </summary>
        public virtual void UpdateRect()
        {

        }

        /// <summary>
        /// 获取组合块下一部分输入位置
        /// </summary>
        /// <param name="rowTop">行Y坐标</param>
        /// <param name="isInputFinished">输入完毕标识</param>
        /// <returns>下一部分输入位置</returns>
        public virtual Point GetNextPartLocation(double rowTop,ref bool isInputFinished)
        {
            return new Point(0, 0);
        }

    }
}
