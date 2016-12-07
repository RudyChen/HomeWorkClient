using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathData
{
  public static class BlockComponentTools
    {
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
    }
}
