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
        private double height = 20;
        private double width;
        private Rect rowRect;
        private List<IBlockComponent> children = new List<IBlockComponent>();

        private double rowCenterYOffset;

        public double RowCenterYOffset
        {
            get { return rowCenterYOffset; }
            set { rowCenterYOffset = value; }
        }



        private Stack<BlockComponentBase> inputBlockComponentStack = new Stack<BlockComponentBase>();

        /// <summary>
        /// 水平居中排列子元素
        /// </summary>
        public delegate void LayoutRowChildrenHorizontialCenterHandler(Vector offsetVector);

        /// <summary>
        /// 更新行内图形块等
        /// </summary>
        public delegate void RefreshBlockRowHandler();




        public BlockRow(Point rowPoint)
        {
            rowRect = new Rect(rowPoint, new Size(width, height));
        }

        public void AddBlockToRow(IBlockComponent block, LayoutRowChildrenHorizontialCenterHandler layoutRowChildrenHorizontialCenter, RefreshBlockRowHandler refreshBlockRow)
        {
            if (block is CharBlock)
            {
                var blockRect = block.GetRect();
                if (InputBlockComponentStack.Count > 0)
                {
                    var container = InputBlockComponentStack.Peek();
                    container.Children[container.CurrentInputPart].Add(block);

                    //更新所有嵌套元素子块区域大小
                    UpdateAllNastedComponentBlockChildRect(new Size(blockRect.Width, 0));


                }
                else
                {
                    Children.Add(block);
                }

                rowRect.Width += blockRect.Width;

                refreshBlockRow();

            }
            else if (block is ShapeBlock)
            {

            }
            else
            {
                Vector offsetVector = new Vector(0, 0);
                var componentBlock = block as BlockComponentBase;
                //嵌套在组合块里面的
                if (InputBlockComponentStack.Count > 0)
                {
                    var container = InputBlockComponentStack.Peek();
                    var containerChildRect = container.GetInputChildRect();
                    if (containerChildRect.Height < componentBlock.Rect.Height)
                    {

                        offsetVector.Y = componentBlock.Rect.Height - containerChildRect.Height;
                        //更新所有嵌套元素子块区域大小
                        UpdateAllNastedComponentBlockChildRect(new Size(0, offsetVector.Y));
                    }

                    InputBlockComponentStack.Push(componentBlock);
                    container.Children[container.CurrentInputPart].Add(block);
                    //container.UpdateShapeBlocks();
                }
                else
                {
                    Children.Add(block);
                    //在行内加入组合块  
                    InputBlockComponentStack.Push(componentBlock);

                }
                layoutRowChildrenHorizontialCenter(offsetVector);
            }
        }

      

        private void UpdateAllNastedComponentBlockChildRect(Size offsetSize)
        {
            //更新所有在输入的嵌套块当前输入部分块宽度
            if (inputBlockComponentStack.Count > 0)
            {
                foreach (var item in inputBlockComponentStack)
                {
                    var inputRect = item.GetInputChildRect();
                    if (inputRect.Width + offsetSize.Width > item.Rect.Width)
                    {
                        item.Rect = new Rect(item.Rect.Location, new Size(item.Rect.Width + offsetSize.Width, item.Rect.Height));
                    }
                    item.Rect = new Rect(item.Rect.Location, new Size(item.Rect.Width, item.Rect.Height + offsetSize.Height));
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

        public Stack<BlockComponentBase> InputBlockComponentStack
        {
            get { return inputBlockComponentStack; }
            set { inputBlockComponentStack = value; }
        }
    }
}
