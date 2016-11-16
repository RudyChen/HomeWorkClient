using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
   public abstract class Block
    {
        private Rect rowRect;

        private Guid containerId;

        private string renderUid;

        /// <summary>
        /// 显示的页面元素唯一ID
        /// </summary>
        public string RenderUid
        {
            get { return renderUid; }
            set { renderUid = value; }
        }

        /// <summary>
        /// 所属容器GUID，
        /// 如 row,page等
        /// </summary>
        public Guid ContainerId
        {
            get { return containerId; }
            set { containerId = value; }
        }

        /// <summary>
        /// 行内显示矩形区域
        /// </summary>
        public Rect RowRect
        {
            get { return rowRect; }
            set { rowRect = value; }
        }
    }
}
