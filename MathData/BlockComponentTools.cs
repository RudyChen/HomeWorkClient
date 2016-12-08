using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
  public static class BlockComponentTools
    {
        /// <summary>
        /// 获取组合块排列位置
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        public static double GetChildrenMaxCenterLine(List<IBlockComponent> children)
        {
            //行内Y偏移量
            double maxCenterLineYOffset = 0;
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                {
                    var areaRect = item.GetRect();
                    double itemCenterLineOffset = item.GetHorizontialAlignmentYOffset();
                    if (itemCenterLineOffset > maxCenterLineYOffset)
                    {
                        maxCenterLineYOffset = itemCenterLineOffset;
                    }
                }
            }

            return maxCenterLineYOffset;
        }

        /// <summary>
        /// 移动组合块
        /// </summary>
        /// <param name="children"></param>
        /// <param name="vector"></param>
        public static void MoveBlockComponents(List<IBlockComponent> children,Vector vector)
        {
            if (children!=null&&children.Count>0)
            {
                foreach (var item in children)
                {
                    if (item is BlockComponentBase)
                    {
                        var component = item as BlockComponentBase;
                        foreach (var componentChild in component.Children)
                        {
                            MoveBlockComponents(componentChild, vector);
                        }
                    }
                    else
                    {
                        item.Move(vector);
                    }
                }
            }
        }
    }
}
