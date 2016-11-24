﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class BlockRow
    {
        private double height=20;
        private double width;
        private Rect rowRect;
        private List<IBlockComponent> children = new List<IBlockComponent>();


        private Stack<BlockComponent> inputBlockComponentStack = new Stack<BlockComponent>();

        /// <summary>
        /// 水平居中排列子元素
        /// </summary>
        public delegate void LayoutRowChildrenHorizontialCenterHandler();


        public BlockRow(Point rowPoint)
        {
            rowRect = new Rect(rowPoint, new Size(width, height));
        }

        public void AddBlockToRow(IBlockComponent block, LayoutRowChildrenHorizontialCenterHandler layoutRowChildrenHorizontialCenter)
        {
            if (block is CharBlock)
            {
                Children.Add(block);
                var blockRect = block.GetRect();
                //嵌套元素输入
                if (inputBlockComponentStack.Count > 0)
                {
                    //更新所有嵌套元素子块区域大小
                    UpdateAllNastedComponentBlockChildRect(new Size(blockRect.Width,0));
                }              

                rowRect.Width += blockRect.Width;


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
                    var containerChildRect = container.GetInputChildRect();
                    if (containerChildRect.Height< componentBlock.Rect.Height)
                    {
                        var offsetHeight = componentBlock.Rect.Height - containerChildRect.Height;
                        //更新所有嵌套元素子块区域大小
                        UpdateAllNastedComponentBlockChildRect(new Size(0,offsetHeight));

                        rowRect.Height += offsetHeight;
                    }

                    inputBlockComponentStack.Push(componentBlock);

                }
                else
                {
                    //在行内加入组合块  
                    inputBlockComponentStack.Push(componentBlock);

                    if (rowRect.Height<componentBlock.Rect.Height)
                    {
                        rowRect.Height += componentBlock.Rect.Height - rowRect.Height;
                        layoutRowChildrenHorizontialCenter();
                    }
                }
            }

        }

        private void UpdateAllNastedComponentBlockChildRect(Size offsetSize)
        {
            throw new NotImplementedException();
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
