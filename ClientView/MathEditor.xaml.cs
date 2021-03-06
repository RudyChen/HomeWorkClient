﻿using MathData;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ClientView
{
    /// <summary>
    /// MathEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MathEditor : UserControl
    {

        FontFamily fontFamily = new FontFamily("Times new roman");

        List<Rectangle> inputPartsTipRect = new List<Rectangle>();

        private double fontSize = 18.4;



        private double fractionSpace = 6;



        public double FractionSpace
        {
            get { return fractionSpace; }
            set { fractionSpace = value; }
        }


        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }



        BlockRow currentInputBlockRow;



        public MathEditor()
        {
            InitializeComponent();
            CreateNewRow();
            UpdateDefaultInputTipRect();
        }

        private void UpdateDefaultInputTipRect()
        {
            var caretPoint = GetCaretPoint();

            var tipRect = CreateRectangle(10, FontSize, Brushes.Blue, 1, new DoubleCollection() { 2, 1 });

            inputPartsTipRect.Add(tipRect);
            editorCanvas.Children.Add(tipRect);
            Canvas.SetLeft(tipRect, caretPoint.X);
            Canvas.SetTop(tipRect, caretPoint.Y);
        }

        /// <summary>
        /// 提示输入插字符的位置
        /// </summary>                
        private Rectangle CreateRectangle(double width, double height, Brush brush, double strokeThickness, DoubleCollection strokeDashArray)
        {
            Rectangle tipRect = new Rectangle();
            tipRect.Width = width;
            tipRect.Height = height;

            tipRect.Stroke = brush;
            tipRect.StrokeThickness = strokeThickness;
            tipRect.StrokeDashArray = strokeDashArray;

            return tipRect;
        }

        private Rectangle CreateRectangle(Rect childRect)
        {
            Rectangle tipRect = new Rectangle();
            tipRect.Width = childRect.Width;
            tipRect.Height = childRect.Height;

            tipRect.Stroke = Brushes.Blue;
            tipRect.StrokeThickness = 1;
            tipRect.StrokeDashArray = new DoubleCollection() { 2, 1 };


            return tipRect;
        }

        public void AcceptInputCommand(InputCommands command)
        {
            switch (command)
            {
                case InputCommands.FractionCommand:
                    AddDefaultFractionComponent();
                    break;
                case InputCommands.NextCommand:
                    InputNextPart();
                    break;
                case InputCommands.Exponential:
                    AddDefaultExponenttialComponent();
                    break;
                case InputCommands.Radical:
                    AddDefaultRadicalComponent();

                    break;
                case InputCommands.DeleteCommand:
                    DeleteElementBeforeCaret();
                    break;

                default:
                    break;
            }

            UpdateBlockInputTicRectangles();
        }

        private void AddDefaultRadicalComponent()
        {
            var location = GetCaretPoint();

            var rowPoint = GetRowPoint();
            var id = Guid.NewGuid().ToString();
            Polyline polyline = MathUIElementTools.CreateRadicalPolyline(new Point(location.X + 2, location.Y), 18.4, 10, 10, id);
            PolylineBlock raicalSymbol = MathUIElementTools.GetRadicalPolineBlocks(polyline, new Point(rowPoint.X + 2, rowPoint.Y));
            RadicalComponent radicalComponent = new RadicalComponent(new Point(rowPoint.X + 2, rowPoint.Y), raicalSymbol);

            currentInputBlockRow.AddBlockToRow(radicalComponent, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);

            editorCanvas.Children.Add(polyline);

            if (radicalComponent.IsNeedRootIndex)
            {
                fontSize = fontSize * 2 / 3;
                caretTextBox.FontSize = fontSize;
            }

        }

        private void AddDefaultExponenttialComponent()
        {
            var rowPoint = GetRowPoint();
            Point ExponentialLeftPoint = rowPoint;

            Rect containerRect;
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var blockComponent = currentInputBlockRow.InputBlockComponentStack.Peek();
                var inputChild = blockComponent.Children[blockComponent.CurrentInputPart];
                containerRect = blockComponent.GetChildRect(inputChild);
            }
            else
            {
                containerRect = new Rect();
            }

            var superscriptY = rowPoint.Y - 0.5 * fontSize;
            if (superscriptY >= containerRect.Top)
            {
                ExponentialLeftPoint = new Point(rowPoint.X, superscriptY);
            }
            else
            {
                ExponentialLeftPoint = new Point(rowPoint.X, containerRect.Top);
            }

            SuperscriptComponent superscriptComponent = new SuperscriptComponent(ExponentialLeftPoint);
            currentInputBlockRow.AddBlockToRow(superscriptComponent, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);
        }

        private void AddDefaultFractionComponent()
        {
            var rowPoint = GetRowPoint();

            LineBlock fractionLineData = new LineBlock(rowPoint);
            fractionLineData.StartPoint = new Point(rowPoint.X, rowPoint.Y + fontSize + fontSize * 0.3);
            fractionLineData.EndPoint = new Point(rowPoint.X + 10, fractionLineData.StartPoint.Y);
            fractionLineData.Rect = new Rect(fractionLineData.StartPoint.X, fractionLineData.StartPoint.Y - currentInputBlockRow.RowRect.Top, 10, 2);
            fractionLineData.RenderUid = Guid.NewGuid().ToString();

            FractionBlockComponent fractionBlock = new FractionBlockComponent(rowPoint, fractionLineData) { FontSize = fontSize };

            double alignCenterYOffset = 0;
            double centerOffset = 0;
            if (currentInputBlockRow.Children.Count > 0)
            {
                var lastChild = currentInputBlockRow.Children.Last();
                if (null != lastChild)
                {
                    if (currentInputBlockRow.InputBlockComponentStack.Count == 0)
                    {
                        if (currentInputBlockRow.RowRect.Height >= fractionBlock.Rect.Height)
                        {
                            alignCenterYOffset = lastChild.GetHorizontialAlignmentYOffset();

                            centerOffset = alignCenterYOffset - fractionLineData.Rect.Top;

                            fractionLineData.StartPoint = new Point(fractionLineData.StartPoint.X, fractionLineData.StartPoint.Y + centerOffset);
                            fractionLineData.EndPoint = new Point(fractionLineData.EndPoint.X, fractionLineData.StartPoint.Y);
                            fractionLineData.Rect = new Rect(fractionLineData.StartPoint.X, fractionLineData.StartPoint.Y, 10, 2);

                            SetCaretOffset(0, centerOffset);
                            rowPoint = GetRowPoint();
                        }
                    }
                    else
                    {
                        var componentBlock = currentInputBlockRow.InputBlockComponentStack.Peek();
                        var inputChildren = componentBlock.Children[componentBlock.CurrentInputPart];
                        var inputChildrenRect = componentBlock.GetChildRect(inputChildren);
                        if (inputChildrenRect.Height >= fractionBlock.Rect.Height)
                        {
                            alignCenterYOffset = BlockComponentTools.GetChildrenMaxCenterLine(inputChildren);

                            fractionLineData.StartPoint = new Point(fractionLineData.StartPoint.X, fractionLineData.StartPoint.Y + centerOffset);
                            fractionLineData.EndPoint = new Point(fractionLineData.EndPoint.X, fractionLineData.StartPoint.Y);
                            fractionLineData.Rect = new Rect(fractionLineData.StartPoint.X, fractionLineData.StartPoint.Y, 10, 2);

                            SetCaretOffset(0, centerOffset);
                            rowPoint = GetRowPoint();
                        }
                    }
                }
            }

            var lineElement = MathUIElementTools.CreateLine(fractionLineData, currentInputBlockRow.RowRect.Top);

            editorCanvas.Children.Add(lineElement);
            fractionBlock = new FractionBlockComponent(rowPoint, fractionLineData) { FontSize = fontSize };
            currentInputBlockRow.AddBlockToRow(fractionBlock, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);
        }

        private void InputNextPart()
        {
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var componentItem = currentInputBlockRow.InputBlockComponentStack.Peek();
                var rowTop = currentInputBlockRow.RowRect.Top;
                bool isFinished = false;
                var nextPartLocation = componentItem.GetNextPartLocation(rowTop, ref isFinished);
                SetCaretLocation(nextPartLocation);
                if (isFinished)
                {
                    currentInputBlockRow.InputBlockComponentStack.Pop();
                }
                UpdateBlockInputTicRectangles();
            }

        }

        private void SetCaretLocation(Point location)
        {
            Canvas.SetLeft(caretTextBox, location.X);
            Canvas.SetTop(caretTextBox, location.Y);
            var height = caretTextBox.Height;
            var obje = height;
        }

        private void editorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            caretTextBox.Focus();

            var absolutePoint = e.GetPosition(editorCanvas);
            //todo: find the row who contains this absolutePoint as currentInputBlockRow
            var rowPoint = new Point(absolutePoint.X, absolutePoint.Y - currentInputBlockRow.RowRect.Top);
            if (currentInputBlockRow.RowRect.Contains(rowPoint))
            {
                var caretRowPoint = MathUIElementTools.RelocateCaretByClick(rowPoint, currentInputBlockRow, FontSize);
                if (caretRowPoint != new Point())
                {
                    var newAbsoluteCaretPoint = new Point(caretRowPoint.X, caretRowPoint.Y + currentInputBlockRow.RowRect.Top);
                    SetCaretLocation(newAbsoluteCaretPoint);
                }
            }

        }

        private void editorCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void caretTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double lineOffsetX = 0;

            if (IsChinese(e.Text))
            {
                if (!string.IsNullOrEmpty(e.Text))
                {
                    lineOffsetX = AcceptChineseInputText(0, 0, e.Text);
                }
            }
            else
            {
                lineOffsetX = AcceptEnglishInputText(0, 0, e.Text, FontSize, caretTextBox.FontFamily, caretTextBox.FontStyle);
            }
            e.Handled = true;
            RefreshAfterInput(lineOffsetX, 0);
        }

        public void RefreshAfterInput(double xOffset, double yOffset)
        {
            SetCaretOffset(xOffset, yOffset);

            UpdateBlockInputTicRectangles();
        }

        public void UpdateBlockInputTicRectangles()
        {

            foreach (var item in inputPartsTipRect)
            {
                editorCanvas.Children.Remove(item);
            }
            inputPartsTipRect.Clear();

            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var blockComponentBase = currentInputBlockRow.InputBlockComponentStack.Peek();


                for (int i = 0; i < blockComponentBase.Children.Count; i++)
                {
                    if (i != blockComponentBase.ShapeChildIndex)
                    {
                        Rect childRect = blockComponentBase.GetChildRect(blockComponentBase.Children[i]);
                        Rectangle childRectangle = CreateRectangle(childRect);

                        inputPartsTipRect.Add(childRectangle);

                        editorCanvas.Children.Add(childRectangle);
                        Canvas.SetLeft(childRectangle, childRect.Left);
                        Canvas.SetTop(childRectangle, childRect.Top + currentInputBlockRow.RowRect.Top);

                    }
                }
            }
            else
            {
                UpdateDefaultInputTipRect();
            }
        }



        private void EquationTypeButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.FractionCommand);
        }


        public double AcceptEnglishInputText(double lineOffsetX, double lineOffsetY, string text, double fontSize, FontFamily family, FontStyle fontStyle)
        {
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = text;
            inputedTextBlock.FontSize = fontSize;// caretTextBox.FontSize;
            inputedTextBlock.FontFamily = family;// fontFamily;
            inputedTextBlock.FontStyle = fontStyle;// FontStyles.Italic;
            inputedTextBlock.Uid = Guid.NewGuid().ToString();

            var rowPoint = GetRowPoint();
            CharBlock charBlock = new CharBlock(inputedTextBlock.Text, inputedTextBlock.FontStyle.ToString(), inputedTextBlock.FontFamily.ToString(), inputedTextBlock.Foreground.ToString(), inputedTextBlock.FontSize, inputedTextBlock.Uid, rowPoint);
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);
            //初始化块区域


            currentInputBlockRow.AddBlockToRow(charBlock, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);

            editorCanvas.Children.Add(inputedTextBlock);

            Canvas.SetLeft(inputedTextBlock, oldCaretLeft + lineOffsetX);
            Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
            caretTextBox.Text = string.Empty;

            return charBlock.WidthIncludingTrailingWhitespace;
        }

        public void RefreshInputComponentShapeBlock()
        {
            //更新shape block大小
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                foreach (var item in currentInputBlockRow.InputBlockComponentStack)
                {
                    item.UpdateShapeBlocks();
                    RefreshComponentShapeBlock(item);
                }
            }
        }


        private void RefreshComponentShapeBlock(BlockComponentBase inputComponent)
        {
            if (inputComponent is FractionBlockComponent)
            {
                var fractionComponent = inputComponent as FractionBlockComponent;
                var lineBlock = fractionComponent.Children[fractionComponent.ShapeChildIndex][0] as LineBlock;
                var lineElement = GetElementByUid(lineBlock.RenderUid);
                var newLine = MathUIElementTools.CreateLine(lineBlock, currentInputBlockRow.RowRect.Top);

                editorCanvas.Children.Remove(lineElement);
                editorCanvas.Children.Add(newLine);
            }
            else if (inputComponent is RadicalComponent)
            {
                var radicalComponent = inputComponent as RadicalComponent;

                var polylineBlock = radicalComponent.Children[radicalComponent.ShapeChildIndex][0] as PolylineBlock;
                var oldRadicalSymbol = GetElementByUid(polylineBlock.RenderUid);
                if (null != oldRadicalSymbol)
                {
                    var polyline = MathUIElementTools.CreateRadicalPolyline(polylineBlock, currentInputBlockRow.RowRect.Top);
                    editorCanvas.Children.Remove(oldRadicalSymbol);
                    editorCanvas.Children.Add(polyline);
                }
            }


        }

        public void LayoutRowChildrenHorizontialCenter(Vector offsetVector)
        {
            /*
            先循环输入组合堆栈，由内向外更新大小，对齐
            1.找到对齐线最大的元素对齐线，2.对齐每个子块，3.更新行区域
            */

            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                LayoutComponentStackChildrenCenter(currentInputBlockRow.InputBlockComponentStack, offsetVector);
            }

            currentInputBlockRow.RowCenterYOffset = BlockComponentTools.GetChildrenMaxCenterLine(currentInputBlockRow.Children);

            AlignChildrenToMaxCenter(currentInputBlockRow.RowCenterYOffset, currentInputBlockRow.Children);

            UpdateRowBlockRectByChildren();

            var caretRowPoint = GetRowPoint();
            var topComponent = currentInputBlockRow.InputBlockComponentStack.Pop();
            BlockComponentBase containerComponent;
            Rect containerInputChildRect;
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                containerComponent = currentInputBlockRow.InputBlockComponentStack.Peek();

                var inputChild = containerComponent.Children[containerComponent.CurrentInputPart];
                containerInputChildRect = containerComponent.GetChildRect(inputChild);
            }
            else
            {
                containerInputChildRect = new Rect();
            }
            currentInputBlockRow.InputBlockComponentStack.Push(topComponent);

            if (topComponent is SuperscriptComponent)
            {
                if (caretRowPoint.Y - 0.5 * fontSize < containerInputChildRect.Top)
                {
                    SetCaretLocation(new Point(caretRowPoint.X, currentInputBlockRow.RowRect.Top + containerInputChildRect.Top + 0.5 * fontSize));
                }
            }
            else if (topComponent is RadicalComponent)
            {
                var radicalComponent = topComponent as RadicalComponent;
                if (radicalComponent.IsNeedRootIndex)
                {
                    SetCaretOffset(4, 0);
                }
                else
                {
                    SetCaretOffset(radicalComponent.FirstDefaultPartWidth, 0);
                }
            }


        }

        private void UpdateRowBlockRectByChildren()
        {
            if (currentInputBlockRow != null && currentInputBlockRow.Children.Count > 0)
            {
                Rect rowRect = new Rect(currentInputBlockRow.RowRect.Location, new Size(0, 0));
                foreach (var item in currentInputBlockRow.Children)
                {
                    var childRect = item.GetRect();

                    rowRect.Union(childRect);
                }

                currentInputBlockRow.RowRect = new Rect(currentInputBlockRow.RowRect.Location, rowRect.Size);
            }
        }

        public void LayoutComponentStackChildrenCenter(Stack<BlockComponentBase> currentRowComponentStack, Vector offsetVector)
        {
            Stack<BlockComponentBase> tempComponentStack = new Stack<BlockComponentBase>();

            while (currentRowComponentStack.Count > 0)
            {
                var topComponent = currentRowComponentStack.Pop();

                double rowYOffset = 0;
                var inputChild = topComponent.Children[topComponent.CurrentInputPart];

                rowYOffset = BlockComponentTools.GetChildrenMaxCenterLine(inputChild);

                AlignChildrenToMaxCenter(rowYOffset, inputChild);

                if (BlockComponentTools.CanUpdateOtherChildrenLocation(topComponent))
                {
                    /*输入部分大小改变(主要是高度变化)，更新另外组成部分位置
                     这是一种后台更新数据，与页面更新数据分离的方式，先更新后台数据，然后再更新页面
                     */
                    topComponent.UpdateOtherChildrenLocation(offsetVector);

                    UpdateOtherBlockChildrenLocation(topComponent, offsetVector);

                    /*此处更新图形块由上面方法代替，但是分数线长度更新并不能代替所以得保留
                    */
                    //topComponent.UpdateShapeBlocks();

                    //UpdateShapeBlocksLocation(topComponent);
                }

                topComponent.UpdateRect();

                tempComponentStack.Push(topComponent);

                //为了更新根号高度
                topComponent.UpdateShapeBlocks();
                RefreshComponentShapeBlock(topComponent);
            }

            while (tempComponentStack.Count > 0)
            {
                var backComponent = tempComponentStack.Pop();
                currentRowComponentStack.Push(backComponent);
            }
        }




        /// <summary>
        /// 用于更新不在输入堆栈中的子块位置
        /// </summary>
        /// <param name="componentBase"></param>
        /// <param name="offsetVector"></param>
        private void UpdateOtherBlockChildrenLocation(BlockComponentBase componentBase, Vector offsetVector)
        {
            for (int i = 0; i < componentBase.Children.Count; i++)
            {
                if (i != componentBase.CurrentInputPart)
                {
                    UpdateComponentChildrenLocation(componentBase.Children[i], offsetVector);
                }
            }
        }

        private void UpdateComponentChildrenLocation(List<IBlockComponent> childBlocks, Vector offsetVector)
        {
            if (childBlocks.Count > 0)
            {
                foreach (var item in childBlocks)
                {
                    UpdateComponentLocation(item, offsetVector);
                }
            }
        }

        private void UpdateComponentLocation(IBlockComponent component, Vector offsetVector)
        {
            if (component is BlockComponentBase)
            {
                var componentItem = component as BlockComponentBase;
                foreach (var itemChild in componentItem.Children)
                {
                    UpdateComponentChildrenLocation(itemChild, offsetVector);
                }
            }
            else
            {
                var blockItem = component as MathData.Block;
                UpdateBlockLocation(blockItem, offsetVector);
            }
        }

        private void UpdateBlockLocation(MathData.Block block, Vector offsetVector)
        {
            FrameworkElement element = GetElementByUid(block.RenderUid);
            if (block is CharBlock)
            {
                var charBlock = block as CharBlock;
                charBlock.Move(offsetVector);
                if (null != element)
                {
                    Canvas.SetLeft(element, Canvas.GetLeft(element) + offsetVector.X);
                    Canvas.SetTop(element, Canvas.GetTop(element) + offsetVector.Y);
                }
            }
            else if (block is LineBlock)
            {
                var LineBlock = block as LineBlock;
                LineBlock.Move(offsetVector);
                var line = MathUIElementTools.CreateLine(LineBlock, currentInputBlockRow.RowRect.Top);

                if (null != element)
                {
                    editorCanvas.Children.Remove(element);
                    editorCanvas.Children.Add(line);
                }
            }
            else if (block is PolylineBlock)
            {
                var polylineBlock = block as PolylineBlock;
                polylineBlock.Move(offsetVector);
                var newPolylineBlock = MathUIElementTools.CreateRadicalPolyline(polylineBlock, currentInputBlockRow.RowRect.Top);
                if (null != element)
                {
                    editorCanvas.Children.Remove(element);
                    editorCanvas.Children.Add(newPolylineBlock);
                }
            }
        }

        private void AlignChildrenToMaxCenter(double maxcCenterLienYOffset, List<IBlockComponent> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                {
                    var itemAlignmentYOffset = item.GetHorizontialAlignmentYOffset();

                    double adjustYOffset = maxcCenterLienYOffset - itemAlignmentYOffset;

                    UpdateComponentLocation(item, new Vector(0, adjustYOffset));
                    //AdjustComponentChildrenLocation(item, 0, adjustYOffset);
                }
            }
        }

        /* 已有更规范的调整位置的方法替换这个了
        private void AdjustComponentChildrenLocation(IBlockComponent block, double xOffset, double yOffset)
        {
            if (block is MathData.Block)
            {
                var blockItem = block as MathData.Block;

                AdjustBlockLocation(blockItem, xOffset, yOffset);
            }
            else if (block is BlockComponentBase)
            {
                var componentBlock = block as BlockComponentBase;
                foreach (var componentChild in componentBlock.Children)
                {
                    if (componentChild.Count > 0)
                    {
                        foreach (var partChild in componentChild)
                        {
                            if (partChild is MathData.Block)
                            {
                                var blockChild = partChild as MathData.Block;
                                AdjustBlockLocation(blockChild, xOffset, yOffset);
                            }
                            else if (partChild is BlockComponentBase)
                            {
                                var blockComponent = partChild as BlockComponentBase;
                                AdjustComponentChildrenLocation(partChild, xOffset, yOffset);
                            }
                        }
                    }
                }
            }
        }

        private void AdjustBlockLocation(MathData.Block block, double xOffset, double yOffset)
        {
            FrameworkElement matchedElement = GetElementByUid(block.RenderUid);

            if (null != matchedElement)
            {
                if (block is ShapeBlock)
                {
                    if (block is LineBlock)
                    {
                        var lineElement = matchedElement as Line;
                        if (null != lineElement)
                        {
                            Line newLine = MoveLineElement(lineElement, xOffset, yOffset);
                            editorCanvas.Children.Remove(matchedElement);
                            editorCanvas.Children.Add(newLine);
                        }
                    }
                    else if (block is PolylineBlock)
                    {
                        var oldPolyline = matchedElement as Polyline;
                        if (null != oldPolyline)
                        {
                            var polylineBlock = block as PolylineBlock;
                            polylineBlock.Move(new Vector(xOffset, yOffset));
                            var newPolyline = MathUIElementTools.CreateRadicalPolyline(polylineBlock, currentInputBlockRow.RowRect.Top);
                            editorCanvas.Children.Remove(matchedElement);
                            editorCanvas.Children.Add(newPolyline);
                        }
                    }
                }
                else
                {
                    Canvas.SetLeft(matchedElement, Canvas.GetLeft(matchedElement) + xOffset);
                    Canvas.SetTop(matchedElement, Canvas.GetTop(matchedElement) + yOffset);

                    UpdateCharBlockRect(block, matchedElement);
                }
            }
        }

        */
        private void UpdateCharBlockRect(MathData.Block block, FrameworkElement matchedElement)
        {
            Point blockLocation = new Point(Canvas.GetLeft(matchedElement), Canvas.GetTop(matchedElement) - currentInputBlockRow.RowRect.Top);
            block.Rect = new Rect(blockLocation, block.Rect.Size);
        }

        private Line MoveLineElement(Line lineElement, double xOffset, double yOffset)
        {
            Line newLine = new Line();
            newLine.X1 = lineElement.X1 + xOffset;
            newLine.Y1 = lineElement.Y1 + yOffset;
            newLine.X2 = lineElement.X2 + xOffset;
            newLine.Y2 = lineElement.Y2 + yOffset;
            newLine.Stroke = lineElement.Stroke;
            newLine.StrokeThickness = lineElement.StrokeThickness;
            newLine.Uid = lineElement.Uid;

            return newLine;
        }

        private FrameworkElement GetElementByUid(string uid)
        {
            var canvasChildren = LogicalTreeHelper.GetChildren(editorCanvas);
            foreach (FrameworkElement item in canvasChildren)
            {
                if (item.Uid == uid)
                {
                    return item;
                }
            }

            return null;
        }


        private double AcceptChineseInputText(double lineOffsetX, double lineOffsetY, string text)
        {
            double allWidth = 0;
            var rowPoint = GetRowPoint();
            Point blockPoint = new Point(rowPoint.X, rowPoint.Y);
            foreach (var item in text)
            {
                TextBlock inputedTextBlock = new TextBlock();
                inputedTextBlock.Text = item.ToString();
                inputedTextBlock.FontSize = caretTextBox.FontSize;
                inputedTextBlock.FontFamily = fontFamily;
                inputedTextBlock.FontStyle = FontStyles.Normal;
                inputedTextBlock.Uid = Guid.NewGuid().ToString();


                CharBlock charBlock = new CharBlock(inputedTextBlock.Text, inputedTextBlock.FontStyle.ToString(), inputedTextBlock.FontFamily.ToString(), inputedTextBlock.Foreground.ToString(), inputedTextBlock.FontSize, inputedTextBlock.Uid, blockPoint);
                var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                var oldCaretTop = Canvas.GetTop(caretTextBox);

                currentInputBlockRow.AddBlockToRow(charBlock, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);

                editorCanvas.Children.Add(inputedTextBlock);

                Canvas.SetLeft(inputedTextBlock, oldCaretLeft + allWidth + lineOffsetX);
                Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
                caretTextBox.Text = string.Empty;

                allWidth += charBlock.WidthIncludingTrailingWhitespace;
                blockPoint = new Point(blockPoint.X + charBlock.WidthIncludingTrailingWhitespace, blockPoint.Y);

                //FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

                //var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                //var oldCaretTop = Canvas.GetTop(caretTextBox);

                //editorCanvas.Children.Add(inputedTextBlock);

                //Canvas.SetLeft(inputedTextBlock, oldCaretLeft + allWidth + lineOffsetX);
                //Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);

                //allWidth += formatted.WidthIncludingTrailingWhitespace;
            }

            caretTextBox.Text = string.Empty;

            return allWidth;

        }

        private void CreateNewRow()
        {
            var rowPoint = GetCaretPoint();
            currentInputBlockRow = new BlockRow(rowPoint);
        }

        private void SetCaretOffset(double xOffset, double yOffset)
        {
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            Canvas.SetLeft(caretTextBox, oldCaretLeft + xOffset);
            Canvas.SetTop(caretTextBox, oldCaretTop + yOffset);
        }

        private bool IsChinese(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            text = text.Trim();

            foreach (char c in text)
            {
                if (c < 0x301E) return false;
            }

            return true;
        }

        private Point GetCaretPoint()
        {
            var caretLeft = Canvas.GetLeft(caretTextBox);
            var caretTop = Canvas.GetTop(caretTextBox);
            Point caretPoint = new Point(caretLeft, caretTop);

            return caretPoint;
        }

        private Point GetRowPoint()
        {
            var caretPoint = GetCaretPoint();
            Point rowPoint = new Point(caretPoint.X, caretPoint.Y - currentInputBlockRow.RowRect.Top);

            return rowPoint;
        }

        private void NextPartButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.NextCommand);
        }

        private void SuperscriptTypeButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.Exponential);
        }

        private void RadicalTypeButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.Radical);
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {

            //  FileStream readstream = new FileStream("D:\\StrObj.txt", FileMode.Open, FileAccess.Read,
            //FileShare.Read);
            //  BinaryFormatter formatter = new BinaryFormatter();
            //  var readdata = (BlockRow)formatter.Deserialize(readstream);
            //  readstream.Close();
            //  Console.WriteLine(readdata);
            //  Console.ReadLine();


            // FileStream stream = new FileStream("D:\\StrObj.txt", FileMode.Create, FileAccess.Write,
            //FileShare.None);
            // BinaryFormatter formatter = new BinaryFormatter();
            // formatter.Serialize(stream, currentInputBlockRow);
            // stream.Close();

        }

        private void BackSpaceButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.DeleteCommand);

        }

        private void DeleteElementBeforeCaret()
        {
            //1.找到插字符前面一个元素 2.移除元素对应页面元素 3.修改尾随元素位置，修改相应界面元素位置 4.移除这个元素
            var caretPoint = GetCaretPoint();
            var caretPointOffset = new Point(caretPoint.X - 2, caretPoint.Y);
            IBlockComponent block = null;

            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var topComponent = currentInputBlockRow.InputBlockComponentStack.Peek();
                for (int i = 0; i < topComponent.Children.Count; i++)
                {
                    if (i != topComponent.ShapeChildIndex)
                    {
                        var itemRect = topComponent.GetChildRect(topComponent.Children[i]);
                        if (itemRect.Contains(caretPointOffset))
                        {
                            foreach (var child in topComponent.Children[i])
                            {
                                var childRect = child.GetRect();
                                if (childRect.Contains(caretPointOffset))
                                {
                                    block = child;
                                    topComponent.CurrentInputPart = i;
                                    break;
                                }
                            }
                            if (null != block)
                            {
                                break;
                            }
                        }
                    }
                }

                var behindBrothers = topComponent.GetBlockBehindBrothers(block);
                var blockRect = block.GetRect();
                if (null != behindBrothers && behindBrothers.Count > 0)
                {
                    foreach (var item in behindBrothers)
                    {

                        var vector = new Vector(-blockRect.Width, 0);

                        var mathDataBlock = item as MathData.Block;
                        UpdateBlockLocation(mathDataBlock, vector);
                    }
                }

                RemoveBlockComponentUIElements(block);
                topComponent.Children[topComponent.CurrentInputPart].Remove(block);
                SetCaretOffset(-blockRect.Width, 0);
            }
            else
            {
                foreach (var item in currentInputBlockRow.Children)
                {
                    var childRect = item.GetRect();
                    if (childRect.Contains(caretPointOffset))
                    {
                        block = item;
                        break;
                    }
                }

                var behindBrothers = GetBlockBehindBrothers(block);
                var blockRect = block.GetRect();
                if (null != behindBrothers && behindBrothers.Count > 0)
                {
                    foreach (var item in behindBrothers)
                    {
                        var vector = new Vector(-blockRect.Width, 0);
                        var mathDataBlock = item as MathData.Block;
                        UpdateBlockLocation(mathDataBlock, vector);
                    }
                }

                RemoveBlockComponentUIElements(block);
                currentInputBlockRow.Children.Remove(block);
                SetCaretOffset(-blockRect.Width, 0);
            }

            //if (block is CharBlock)
            //{
            //    var charBlock = block as CharBlock;
            //    FrameworkElement element= GetElementByUid(charBlock.RenderUid);
            //    if (null!=element)
            //    {
            //        editorCanvas.Children.Remove(element);
            //    }



            //}
            UpdateBlockInputTicRectangles();

        }

        public List<IBlockComponent> GetBlockBehindBrothers(IBlockComponent blockComponent)
        {
            List<IBlockComponent> behindBrothers = null;
            var index = currentInputBlockRow.Children.IndexOf(blockComponent);
            if (index >= 0 && index < currentInputBlockRow.Children.Count)
            {
                var count = currentInputBlockRow.Children.Count - index - 1;
                behindBrothers = currentInputBlockRow.Children.GetRange(index + 1, count);
            }

            return behindBrothers;
        }

        public void RemoveBlockComponentUIElements(IBlockComponent blockComponent)
        {
            if (blockComponent is BlockComponentBase)
            {
                var componentBase = blockComponent as BlockComponentBase;
                foreach (List<IBlockComponent> item in componentBase.Children)
                {
                    foreach (var blockItem in item)
                    {
                        if (blockItem is MathData.Block)
                        {
                            var block = blockItem as MathData.Block;
                            RemoveUIElement(block);
                        }
                        else
                        {
                            RemoveBlockComponentUIElements(blockItem);
                        }
                    }
                }
            }
            else if (blockComponent is MathData.Block)
            {
                var block = blockComponent as MathData.Block;
                RemoveUIElement(block);
            }

        }

        public void RemoveUIElement(MathData.Block block)
        {
            var uiElement = GetElementByUid(block.RenderUid);
            if (null != uiElement)
            {
                editorCanvas.Children.Remove(uiElement);
            }
        }


        public void SaveMathEquationText()
        {
            string defaultFileName = "MathEquation";
            CommonSaveFileDialog saveFileDialog = new CommonSaveFileDialog(defaultFileName);
            saveFileDialog.DefaultExtension = "met";
            saveFileDialog.DefaultFileName = defaultFileName;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = saveFileDialog.FileName;
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;//这一行就是设置Json.NET能够序列化接口或继承类的关键，将TypeNameHandling设置为All后，Json.NET会在序列化后的json文本中附加一个属性说明json到底是从什么类序列化过来的，也可以设置TypeNameHandling为Auto，表示让Json.NET自动判断是否需要在序列化后的json中添加类型属性，如果序列化的对象类型和声明类型不一样的话Json.NET就会在json中添加类型属性，反之就不添加，但是我发现TypeNameHandling.Auto有时候不太好用。。。
                string textJson = JsonConvert.SerializeObject(currentInputBlockRow, jsonSerializerSettings);//将JsonSerializerSettings作为参数传入序列化函数，这样序列化后的Json就附带类型属性
                var result = JsonConvert.DeserializeObject<BlockRow>(textJson, jsonSerializerSettings);//将JsonSerializerSettings作为参数传入反序列化函数，这样Json.NET就会读取json文本中的类型属性，知道应该反序列化成什么类型
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, currentInputBlockRow);
                    stream.Close();
                }
            }
        }

        public string GetEditorTypeInString()
        {
            string typeInString = string.Empty;

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;//这一行就是设置Json.NET能够序列化接口或继承类的关键，将TypeNameHandling设置为All后，Json.NET会在序列化后的json文本中附加一个属性说明json到底是从什么类序列化过来的，也可以设置TypeNameHandling为Auto，表示让Json.NET自动判断是否需要在序列化后的json中添加类型属性，如果序列化的对象类型和声明类型不一样的话Json.NET就会在json中添加类型属性，反之就不添加，但是我发现TypeNameHandling.Auto有时候不太好用。。。
            typeInString = JsonConvert.SerializeObject(currentInputBlockRow, jsonSerializerSettings);//将JsonSerializerSettings作为参数传入序列化函数，这样序列化后的Json就附带类型属性
            //var result = JsonConvert.DeserializeObject<BlockRow>(textJson, jsonSerializerSettings);//将JsonSerializerSettings作为参数传入反序列化函数，这样Json.NET就会读取json文本中的类型属性，知道应该反序列化成什么类型


            return typeInString;
        }
         

        public void OpenMathEquationText()
        {
            ShellContainer selectedFolder = null;
            selectedFolder = KnownFolders.Computer as ShellContainer;
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.InitialDirectoryShellContainer = selectedFolder;
            openFileDialog.EnsurePathExists = true;
            openFileDialog.EnsureFileExists = true;
            openFileDialog.DefaultExtension = "met";
            openFileDialog.EnsureReadOnly = true;
            BlockRow blockRowRead = null;
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filePath = openFileDialog.FileName;
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream readstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    blockRowRead = (BlockRow)formatter.Deserialize(readstream);
                    readstream.Close();
                }
            }

            if (null != blockRowRead)
            {
                currentInputBlockRow = blockRowRead;
                MathUIElementTools.CreateBlockRowUIElements(editorCanvas, currentInputBlockRow);
            }

        }


    }
}
