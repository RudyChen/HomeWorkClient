using MathData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClientView
{
    public static class MathUIElementTools
    {
        /// <summary>
        /// 创建根式符号
        /// </summary>
        /// <param name="radicalLocation">根式位置</param>
        /// <param name="radicalHeight">根式高度</param>
        /// <param name="firstPartWidth">几次根式宽度</param>
        /// <param name="secondPartWidth">被开根式宽度</param>
        /// <param name="guid">根式页面唯一ID</param>
        /// <returns></returns>
        public static Polyline CreateRadicalPolyline(Point radicalLocation, double radicalHeight, double firstPartWidth, double secondPartWidth, string guid)
        {
            Polyline radicalSymbol = new Polyline();
            radicalSymbol.Points = new PointCollection();
            radicalSymbol.Stroke = Brushes.Black;
            radicalSymbol.StrokeThickness = 1;

            Point point1 = new Point(radicalLocation.X, radicalLocation.Y + radicalHeight* 7/ 8);
            Point point2 = new Point(radicalLocation.X + firstPartWidth / 3, radicalLocation.Y + radicalHeight *2/ 3);
            Point point3 = new Point(radicalLocation.X + firstPartWidth, radicalLocation.Y + radicalHeight);
            Point point4 = new Point(radicalLocation.X + firstPartWidth * 4 / 3, radicalLocation.Y);
            Point point5 = new Point(radicalLocation.X + firstPartWidth * 4 / 3 + secondPartWidth, radicalLocation.Y);

            radicalSymbol.Points.Add(point1);
            radicalSymbol.Points.Add(point2);
            radicalSymbol.Points.Add(point3);
            radicalSymbol.Points.Add(point4);
            radicalSymbol.Points.Add(point5);

            radicalSymbol.Uid = guid;

            return radicalSymbol;

        }

        public static PolylineBlock  GetRadicalPolineBlocks(Polyline polyline,Point rowPoint)
        {
            PolylineBlock polylineBlock = new PolylineBlock();
            polylineBlock.Stroke = polyline.Stroke.ToString();
            polylineBlock.StrokeThickness = polyline.StrokeThickness;
            foreach (var item in polyline.Points)
            {
                Point pointItem = new Point(item.X, item.Y - rowPoint.Y);
                polylineBlock.PolylinePoints.Add(pointItem);
            }

            polylineBlock.Rect = new Rect(rowPoint.X,rowPoint.Y, polyline.Width, polyline.Height);

            return polylineBlock;
        }
    }
}
