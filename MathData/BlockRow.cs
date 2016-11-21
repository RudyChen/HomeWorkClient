using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathData
{
   public class BlockRow
    {
        private double rowTop;

        private double rowBottom;

        private double inputSpaceTop;

        private double inputSpaceBottom;

        private List<IBlockComponent> rowBlockItems=new List<IBlockComponent>();


        private void AddBlockToRow(IBlockComponent block)
        {
            if (block is CharBlock)
            {
                RowBlockItems.Add(block);
               

            }

        }

        private Guid rowId;

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
        public List<IBlockComponent> RowBlockItems
        {
            get { return rowBlockItems; }
            set { rowBlockItems = value; }
        }


        public double InputSpaceBottom
        {
            get { return inputSpaceBottom; }
            set { inputSpaceBottom = value; }
        }


        public double InputSpaceTop
        {
            get { return inputSpaceTop; }
            set { inputSpaceTop = value; }
        }


        public double RowBottom
        {
            get { return rowBottom; }
            set { rowBottom = value; }
        }

        public double RowTop
        {
            get { return rowTop; }
            set { rowTop = value; }
        }



    }
}
