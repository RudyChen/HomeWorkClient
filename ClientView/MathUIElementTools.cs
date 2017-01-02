using MathData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            Point point1 = new Point(radicalLocation.X, radicalLocation.Y + radicalHeight * RadicalComponent.SYMBOL_POINT1_RATIO);
            Point point2 = new Point(radicalLocation.X + firstPartWidth * RadicalComponent.SYMBOL_POINT2X_RATIO, radicalLocation.Y + radicalHeight * RadicalComponent.SYMBOL_POINT2Y_RATIO);
            Point point3 = new Point(radicalLocation.X + firstPartWidth * RadicalComponent.SYMBOL_POINT3X_RATIO, radicalLocation.Y + radicalHeight);
            Point point4 = new Point(radicalLocation.X + firstPartWidth * RadicalComponent.SYMBOL_POINT4X_RATIO, radicalLocation.Y);
            Point point5 = new Point(radicalLocation.X + firstPartWidth * RadicalComponent.SYMBOL_POINT5X_RATIO + secondPartWidth, radicalLocation.Y);

            radicalSymbol.Points.Add(point1);
            radicalSymbol.Points.Add(point2);
            radicalSymbol.Points.Add(point3);
            radicalSymbol.Points.Add(point4);
            radicalSymbol.Points.Add(point5);

            radicalSymbol.Uid = guid;

            return radicalSymbol;

        }

        public static Polyline CreateRadicalPolyline(PolylineBlock polylineBlock, double rowTop)
        {
            Polyline radicalSymbol = new Polyline();
            radicalSymbol.Points = new PointCollection();
            var color = (Color)ColorConverter.ConvertFromString(polylineBlock.Stroke);
            radicalSymbol.Stroke = new SolidColorBrush(color);
            radicalSymbol.StrokeThickness = polylineBlock.StrokeThickness;
            radicalSymbol.Uid = polylineBlock.RenderUid;

            for (int i = 0; i < polylineBlock.PolylinePoints.Count; i++)
            {
                Point polylinePoint = new Point(polylineBlock.PolylinePoints[i].X, polylineBlock.PolylinePoints[i].Y + rowTop);
                radicalSymbol.Points.Add(polylinePoint);
            }

            return radicalSymbol;
        }



        public static Line CreateLine(LineBlock lineBlock, double rowTop)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lineBlock.Stroke));
            line.StrokeThickness = lineBlock.StrokeThickness;
            line.X1 = lineBlock.StartPoint.X;
            line.X2 = lineBlock.EndPoint.X;
            line.Y1 = lineBlock.StartPoint.Y + rowTop;
            line.Y2 = lineBlock.EndPoint.Y + rowTop;
            line.Uid = lineBlock.RenderUid;
            return line;
        }


        public static PolylineBlock GetRadicalPolineBlocks(Polyline polyline, Point rowPoint)
        {
            PolylineBlock polylineBlock = new PolylineBlock(rowPoint);
            polylineBlock.Stroke = polyline.Stroke.ToString();
            polylineBlock.StrokeThickness = polyline.StrokeThickness;
            polylineBlock.RenderUid = polyline.Uid;
            foreach (var item in polyline.Points)
            {
                Point pointItem = new Point(item.X, item.Y - rowPoint.Y);
                polylineBlock.PolylinePoints.Add(pointItem);
            }

            polylineBlock.Rect = new Rect(rowPoint.X, rowPoint.Y, polyline.Width, polyline.Height);

            return polylineBlock;
        }



        public static void CreateCharBlockUIElement(Canvas containerCanvas, CharBlock block, Double rowTop)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = block.FontSize;
            textBlock.FontFamily = (FontFamily)(new FontFamilyConverter().ConvertFromString(block.FontFamily));
            textBlock.FontStyle = (FontStyle)(new FontStyleConverter().ConvertFromString(block.FontStyle));
            textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(block.ForeGround));
            textBlock.Text = block.Text;
            textBlock.Uid = block.RenderUid;

            containerCanvas.Children.Add(textBlock);

            Canvas.SetLeft(textBlock, block.Rect.Left);
            Canvas.SetTop(textBlock, block.Rect.Top + rowTop);
        }

        public static void CreateShapeBlockUIElement(Canvas containerCanvas, ShapeBlock shapeBlock, double rowTop)
        {
            if (shapeBlock is LineBlock)
            {
                LineBlock lineBlock = shapeBlock as LineBlock;
                Line line = new Line();
                line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lineBlock.Stroke)); ;
                line.StrokeThickness = lineBlock.StrokeThickness;
                line.X1 = lineBlock.Rect.Left;
                line.X2 = lineBlock.Rect.Left + lineBlock.Rect.Width;
                line.Y1 = lineBlock.Rect.Top + rowTop;
                line.Y2 = line.Y1;
                line.Uid = lineBlock.RenderUid;

                containerCanvas.Children.Add(line);
            }
            else if (shapeBlock is PolylineBlock)
            {
                PolylineBlock polylineBlock = shapeBlock as PolylineBlock;
                Polyline polyline = new Polyline();
                polyline.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(polylineBlock.Stroke));
                polyline.StrokeThickness = polylineBlock.StrokeThickness;
                polyline.Uid = polylineBlock.RenderUid;
                polyline.Points = new PointCollection();
                foreach (var item in polylineBlock.PolylinePoints)
                {
                    Point pointItem = new Point(item.X, item.Y + rowTop);
                    polyline.Points.Add(pointItem);
                }

                containerCanvas.Children.Add(polyline);
            }
        }

        public static void CreateComponentBaseUIElements(Canvas container, BlockComponentBase blockComponentBase, double rowTop)
        {
            if (blockComponentBase.Children.Count > 0)
            {
                foreach (var childItem in blockComponentBase.Children)
                {
                    if (childItem.Count > 0)
                    {
                        foreach (var blockItem in childItem)
                        {
                            if (blockItem is CharBlock)
                            {
                                var charBlock = blockItem as CharBlock;
                                CreateCharBlockUIElement(container, charBlock, rowTop);
                            }
                            else if (blockItem is ShapeBlock)
                            {
                                var shapeBlock = blockItem as ShapeBlock;
                                CreateShapeBlockUIElement(container, shapeBlock, rowTop);
                            }
                            else if (blockItem is BlockComponentBase)
                            {
                                var blockComponent = blockItem as BlockComponentBase;
                                CreateComponentBaseUIElements(container, blockComponent, rowTop);
                            }
                        }
                    }
                }
            }
        }

        public static void CreateBlockRowUIElements(Canvas container, BlockRow blockRow)
        {
            if (blockRow.Children.Count > 0)
            {
                foreach (var child in blockRow.Children)
                {
                    if (child is CharBlock)
                    {
                        var charBlock = child as CharBlock;
                        CreateCharBlockUIElement(container, charBlock, blockRow.RowRect.Top);
                    }
                    else if (child is ShapeBlock)
                    {
                        var shapeBlock = child as ShapeBlock;
                        CreateShapeBlockUIElement(container, shapeBlock, blockRow.RowRect.Top);
                    }
                    else if (child is BlockComponentBase)
                    {
                        var blockComponent = child as BlockComponentBase;
                        CreateComponentBaseUIElements(container, blockComponent, blockRow.RowRect.Top);
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标点击，返回重定位插字符位置
        /// </summary>        
        public static Point RelocateCaretByClick(Point rowPoint, BlockRow currentBlockRow, double fontSize)
        {
            Point relocatePoint=new Point();

            if (currentBlockRow.InputBlockComponentStack.Count > 0)
            {
                currentBlockRow.InputBlockComponentStack.Clear();
            }

            //1 找到包含点击的点的block元素。2 构造包含点击点的输入堆栈

            foreach (var item in currentBlockRow.Children)
            {
                CreateInputBlockStack(item, rowPoint, currentBlockRow.InputBlockComponentStack,ref relocatePoint);
            }

            return relocatePoint;
        }

        private static void CreateInputBlockStack(IBlockComponent component, Point rowPoint, Stack<BlockComponentBase> inputStack,ref Point relocateCaretPoint)
        {
            var childRect = component.GetRect();
            // 包含偏移一点点
            var containerRect = new Rect(childRect.X + 2, childRect.Y, childRect.Width, childRect.Height);
            if (containerRect.Contains(rowPoint))
            {
                if (component is BlockComponentBase)
                {
                    var blockBase = component as BlockComponentBase;
                    inputStack.Push(blockBase);

                    for (int i = 0; i < blockBase.Children.Count; i++)
                    {
                        if (i!=blockBase.ShapeChildIndex)
                        {
                            var chilRect = blockBase.GetChildRect(blockBase.Children[i]);
                            var childContainerRect = new Rect(chilRect.X + 2, chilRect.Y, chilRect.Width, chilRect.Height);
                            if (childContainerRect.Contains(rowPoint))
                            {
                                foreach (var child in blockBase.Children[i])
                                {
                                    CreateInputBlockStack(child, rowPoint, inputStack,ref relocateCaretPoint);
                                }
                            }
                        }                        
                    }                 
                }
                else
                {
                    relocateCaretPoint = new Point(childRect.X + childRect.Width, childRect.Y);
                }
            }
        }
    }
}
