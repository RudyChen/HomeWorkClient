using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class BlockRow
    {
        private double height;
        private double width;
        private Rect rowRect;
        private List<IBlockComponent> children = new List<IBlockComponent>();


        private Stack<BlockComponent> inputBlockComponentStack = new Stack<BlockComponent>();

        /// <summary>
        /// 水平居中排列子元素
        /// </summary>
        public delegate void LayoutChildrenHorizontialCenterHandler();


        public BlockRow(Point rowPoint)
        {
            rowRect = new Rect(rowPoint, new Size(width, height));
        }

        private void AddBlockToRow(IBlockComponent block, LayoutChildrenHorizontialCenterHandler layoutChildrenHorizontialCenter)
        {
            if (block is CharBlock)
            {
                Children.Add(block);

                if (rowRect.Contains(block.GetRect()))
                {

                }


            }
            else if (block is ShapeBlock)
            {

            }
            else
            {
                var componentBlock = block as BlockComponent;
                //嵌套在组合块里面的
                if (inputBlockComponentStack.Count > 0)
                {
                    var container = inputBlockComponentStack.Peek();
                    var containerRect = container.GetInputChildRect();


                }
                else
                {
                    //在行内加入组合块  
                    inputBlockComponentStack.Push(componentBlock);

                    if (rowRect.Height<componentBlock.Rect.Height)
                    {
                        rowRect.Height += componentBlock.Rect.Height - rowRect.Height;
                        layoutChildrenHorizontialCenter();
                    }
                }
            }

        }      

        private Guid rowId;

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// 行唯一标示
        /// 联系行内元素
        /// </summary>
        public Guid RowId
        {
            get { return rowId; }
            set { rowId = value; }
        }

        /// <summary>
        /// 行内元素集合
        /// </summary>
        public List<IBlockComponent> Children
        {
            get { return children; }
            set { children = value; }
        }

        public Rect RowRect
        {
            get { return rowRect; }
            set { rowRect = value; }
        }

        public Stack<BlockComponent> InputBlockComponentStack
        {
            get { return inputBlockComponentStack; }
            set { inputBlockComponentStack = value; }
        }
    }
}
