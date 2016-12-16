using MathData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

namespace ClientView
{
    /// <summary>
    /// MathEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MathEditor : UserControl
    {

        FontFamily fontFamily = new FontFamily("Times new roman");

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
                default:
                    break;
            }
        }

        private void AddDefaultRadicalComponent()
        {
            var location = GetCaretPoint();
            
            var rowPoint = GetRowPoint();
            var id = Guid.NewGuid().ToString();
            Polyline polyline = MathUIElementTools.CreateRadicalPolyline(new Point(location.X+2, location.Y), 18.4, 10, 10, id);
            PolylineBlock raicalSymbol = MathUIElementTools.GetRadicalPolineBlocks(polyline, new Point(rowPoint.X+2,rowPoint.Y));
            raicalSymbol.RenderUid = id;
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

            Line fractionLine = new Line();
            fractionLine.Stroke = Brushes.Black;
            fractionLine.X1 = rowPoint.X;
            fractionLine.Y1 = currentInputBlockRow.RowRect.Top + rowPoint.Y + fontSize + fontSize * 0.3;
            fractionLine.X2 = rowPoint.X + 10;
            fractionLine.Y2 = fractionLine.Y1;
            fractionLine.Uid = Guid.NewGuid().ToString();
            fractionLine.StrokeThickness = 1;
            fractionLine.SnapsToDevicePixels = true;


            LineBlock fractionLineData = new LineBlock(rowPoint);
            fractionLineData.Rect = new Rect(fractionLine.X1, fractionLine.Y1 - currentInputBlockRow.RowRect.Top, 10, 2);
            fractionLineData.RenderUid = fractionLine.Uid;

            FractionBlockComponent fractionBlock = new FractionBlockComponent(rowPoint, fractionLineData) { FontSize = fontSize };

            double alignCenterYOffset = 0;
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

                            var centerOffset = alignCenterYOffset - fractionLineData.Rect.Top;
                            fractionLine.Y1 = fractionLine.Y1 + centerOffset;
                            fractionLine.Y2 = fractionLine.Y1;
                            fractionLineData.Rect = new Rect(fractionLine.X1, fractionLine.Y1 - currentInputBlockRow.RowRect.Top, 10, 2);

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

                            var centerOffset = alignCenterYOffset - fractionLineData.Rect.Top;
                            fractionLine.Y1 = fractionLine.Y1 + centerOffset;
                            fractionLine.Y2 = fractionLine.Y1;
                            fractionLineData.Rect = new Rect(fractionLine.X1, fractionLine.Y1 - currentInputBlockRow.RowRect.Top, 10, 2);

                            SetCaretOffset(0, centerOffset);
                            rowPoint = GetRowPoint();
                        }
                    }
                }
            }


            editorCanvas.Children.Add(fractionLine);
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
                lineOffsetX = AcceptEnglishInputText(0, 0, e.Text);
            }
            e.Handled = true;
            SetCaretOffset(lineOffsetX, 0);
        }

        private void EquationTypeButton_Clicked(object sender, RoutedEventArgs e)
        {
            AcceptInputCommand(InputCommands.FractionCommand);
        }


        private double AcceptEnglishInputText(double lineOffsetX, double lineOffsetY, string text)
        {
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = text;
            inputedTextBlock.FontSize = caretTextBox.FontSize;
            inputedTextBlock.FontFamily = fontFamily;
            inputedTextBlock.FontStyle = FontStyles.Italic;
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
            //todo:更新shape block大小
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var inputComponent = currentInputBlockRow.InputBlockComponentStack.Peek();
                inputComponent.UpdateShapeBlocks();
                RefreshComponentShapeBlock(inputComponent);
            }
        }
        private void RefreshComponentShapeBlock(BlockComponentBase inputComponent)
        {
            if (inputComponent is FractionBlockComponent)
            {
                var fractionComponent = inputComponent as FractionBlockComponent;
                var lineBlock = fractionComponent.Children[2][0] as LineBlock;
                var lineElement = GetElementByUid(lineBlock.RenderUid);
                var newLineElement = UpdateFractionLineElementLength(lineBlock, lineElement as Line);
                editorCanvas.Children.Remove(lineElement);
                editorCanvas.Children.Add(newLineElement);
            }
            else if (inputComponent is RadicalComponent)
            {
                var radicalComponent = inputComponent as RadicalComponent;

                var polylineBlock = radicalComponent.Children[radicalComponent.ShapeChildIndex][0] as PolylineBlock;
                var oldRadicalSymbol = GetElementByUid(polylineBlock.RenderUid);
                var polyline = MathUIElementTools.CreateRadicalPolyline(polylineBlock,currentInputBlockRow.RowRect.Top);
                editorCanvas.Children.Remove(oldRadicalSymbol);
                editorCanvas.Children.Add(polyline);
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

            // var newInputComponent = currentRowComponentStack.Pop();

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
            }

            while (tempComponentStack.Count > 0)
            {
                var backComponent = tempComponentStack.Pop();
                currentRowComponentStack.Push(backComponent);
            }

            //  currentRowComponentStack.Push(newInputComponent);
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
                    if (item is BlockComponentBase)
                    {
                        var componentItem = item as BlockComponentBase;
                        foreach (var itemChild in componentItem.Children)
                        {
                            UpdateComponentChildrenLocation(itemChild, offsetVector);
                        }
                    }
                    else
                    {
                        var blockItem = item as MathData.Block;
                        UpdateBlockLocation(blockItem, offsetVector);
                    }
                }
            }
        }

        private void UpdateBlockLocation(MathData.Block block, Vector offsetVector)
        {
            FrameworkElement element = GetElementByUid(block.RenderUid);
            if (block is CharBlock)
            {
                Canvas.SetLeft(element, Canvas.GetLeft(element) + offsetVector.X);
                Canvas.SetTop(element, Canvas.GetTop(element) + offsetVector.Y);
            }
            else if (block is LineBlock)
            {
                var lineElement = element as Line;
                editorCanvas.Children.Remove(lineElement);
                lineElement.X1 += offsetVector.X;
                lineElement.X2 += offsetVector.X;
                lineElement.Y1 += offsetVector.Y;
                lineElement.Y2 += offsetVector.Y;
                editorCanvas.Children.Add(lineElement);
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

                    AdjustComponentChildrenLocation(item, 0, adjustYOffset);
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
                }
                else
                {
                    Canvas.SetLeft(matchedElement, Canvas.GetLeft(matchedElement) + xOffset);
                    Canvas.SetTop(matchedElement, Canvas.GetTop(matchedElement) + yOffset);

                    UpdateCharBlockRect(block, matchedElement);
                }
            }
        }

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

        private Line UpdateFractionLineElementLength(LineBlock lineBlock, Line oldLine)
        {
            Line newLine = new Line();
            newLine.X1 = oldLine.X1;
            newLine.Y1 = oldLine.Y1;
            newLine.X2 = oldLine.X1 + lineBlock.Rect.Width;
            newLine.Y2 = oldLine.Y2;
            newLine.Stroke = oldLine.Stroke;
            newLine.StrokeThickness = oldLine.StrokeThickness;
            newLine.Uid = oldLine.Uid;

            return newLine;
        }

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
            foreach (var item in text)
            {
                TextBlock inputedTextBlock = new TextBlock();
                inputedTextBlock.Text = item.ToString();
                inputedTextBlock.FontSize = caretTextBox.FontSize;
                inputedTextBlock.FontFamily = fontFamily;
                inputedTextBlock.FontStyle = FontStyles.Normal;
                inputedTextBlock.Uid = Guid.NewGuid().ToString();

                var rowPoint = GetRowPoint();
                CharBlock charBlock = new CharBlock(inputedTextBlock.Text, inputedTextBlock.FontStyle.ToString(), inputedTextBlock.FontFamily.ToString(), inputedTextBlock.Foreground.ToString(), inputedTextBlock.FontSize, inputedTextBlock.Uid, rowPoint);
                var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                var oldCaretTop = Canvas.GetTop(caretTextBox);

                currentInputBlockRow.AddBlockToRow(charBlock, LayoutRowChildrenHorizontialCenter, RefreshInputComponentShapeBlock);

                editorCanvas.Children.Add(inputedTextBlock);

                Canvas.SetLeft(inputedTextBlock, oldCaretLeft + allWidth + lineOffsetX);
                Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
                caretTextBox.Text = string.Empty;

                allWidth += charBlock.WidthIncludingTrailingWhitespace;

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
    }
}
