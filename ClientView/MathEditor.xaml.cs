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

        private double fontSize = 18;

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
                    var rowPoint = GetRowPoint();

                    Line fractionLine = new Line();
                    fractionLine.Stroke = Brushes.Black;
                    fractionLine.X1 = rowPoint.X;
                    fractionLine.Y1 = rowPoint.Y + fontSize + fontSize*0.5-1;
                    fractionLine.X2 = rowPoint.X + 10;
                    fractionLine.Y2 = fractionLine.Y1;
                    fractionLine.Uid = Guid.NewGuid().ToString();
                    fractionLine.StrokeThickness = 1;
                    fractionLine.SnapsToDevicePixels = true;
                    editorCanvas.Children.Add(fractionLine);

                    LineBlock fractionLineData = new LineBlock(rowPoint);
                    fractionLineData.Rect = new Rect(fractionLine.X1, fractionLine.Y1- currentInputBlockRow.RowRect.Top, fractionLine.Width, fractionLine.Height);
                    fractionLineData.RenderUid = fractionLine.Uid;

                    FractionBlockComponent fractionBlock = new FractionBlockComponent(rowPoint, fractionLineData) { FontSize = fontSize };

                    currentInputBlockRow.AddBlockToRow(fractionBlock, LayoutRowChildrenHorizontialCenter, RefreshBlockRow);

                    
                    break;
                case InputCommands.NextCommand:
                    InputNextPart();
                    break;
                case InputCommands.Exponential:
                    break;
                case InputCommands.Radical:
                    break;
                default:
                    break;
            }
        }

        private void InputNextPart()
        {
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var componentItem = currentInputBlockRow.InputBlockComponentStack.Peek();
                if (componentItem is FractionBlockComponent)
                {
                    var fractionComponent = componentItem as FractionBlockComponent;
                    var rowTop = currentInputBlockRow.RowRect.Top;
                    var nextPartLocation = fractionComponent.GetNextPartLocation(rowTop);
                    SetCaretLocation(nextPartLocation);
                    if (fractionComponent.CurrentInputPart==0)
                    {
                        fractionComponent.CurrentInputPart++;
                    }
                    else
                    {
                        currentInputBlockRow.InputBlockComponentStack.Pop();
                    }
                    
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
            SetCaretLocation(lineOffsetX, 0);
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

            CharBlock charBlock = new CharBlock(inputedTextBlock.Text, inputedTextBlock.FontStyle.ToString(), inputedTextBlock.FontFamily.ToString(), inputedTextBlock.Foreground.ToString(), inputedTextBlock.FontSize, inputedTextBlock.Uid);
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);
            //初始化块区域
            var charRect = charBlock.CreateRect(new Point(oldCaretLeft, oldCaretTop - currentInputBlockRow.RowRect.Top));

            currentInputBlockRow.AddBlockToRow(charBlock, LayoutRowChildrenHorizontialCenter, RefreshBlockRow);

            editorCanvas.Children.Add(inputedTextBlock);

            Canvas.SetLeft(inputedTextBlock, oldCaretLeft + lineOffsetX);
            Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
            caretTextBox.Text = string.Empty;

            return charBlock.WidthIncludingTrailingWhitespace;
        }

        public void RefreshBlockRow()
        {
            //todo:更新shape block大小
            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                var inputComponent = currentInputBlockRow.InputBlockComponentStack.Peek();
                inputComponent.UpdateShapeBlocks();
                RefreshComponentShapeBlock(inputComponent);
            }
        }

        public void LayoutRowChildrenHorizontialCenter()
        {
            /*
            先循环输入组合堆栈，由内向外更新大小，对齐
            1.找到对齐线最大的元素对齐线，2.对齐每个子块，3.更新行区域
            */

            if (currentInputBlockRow.InputBlockComponentStack.Count > 0)
            {
                LayoutComponentStackChildrenCenter(currentInputBlockRow.InputBlockComponentStack);
            }

            double rowYOffset = GetChildrenMaxCenterLine(currentInputBlockRow.Children);

            AlignChildrenToMaxCenter(rowYOffset + currentInputBlockRow.RowRect.Top, currentInputBlockRow.Children);
                       
            UpdateRowBlockRectByChildren();
        }



        private void RefreshComponentShapeBlock(BlockComponent inputComponent)
        {
            if (inputComponent is FractionBlockComponent)
            {
                var fractionComponent = inputComponent as FractionBlockComponent;
                var lineBlock = fractionComponent.Children[2][0] as LineBlock;
                var lineElement = GetElementByUid(lineBlock.RenderUid);
                var newLineElement = RefreshLineElement(lineBlock,lineElement as Line);
                editorCanvas.Children.Remove(lineElement);
                editorCanvas.Children.Add(newLineElement);
            }
        }

        private void UpdateRowBlockRectByChildren()
        {
            if (currentInputBlockRow != null && currentInputBlockRow.Children.Count > 0)
            {
                foreach (var item in currentInputBlockRow.Children)
                {
                    var childRect = item.GetRect();
                    //更新高度
                    if (childRect.Top + childRect.Height > currentInputBlockRow.RowRect.Height)
                    {
                        currentInputBlockRow.RowRect = new Rect(currentInputBlockRow.RowRect.Location, new Size(currentInputBlockRow.RowRect.Width, childRect.Top + childRect.Height));
                    }

                    //更新宽度
                    if (childRect.Left + childRect.Width > currentInputBlockRow.RowRect.Width)
                    {
                        currentInputBlockRow.RowRect = new Rect(currentInputBlockRow.RowRect.Location, new Size(childRect.Left + childRect.Width, currentInputBlockRow.RowRect.Height));
                    }
                }
            }
        }

        public void LayoutComponentStackChildrenCenter(Stack<BlockComponent> currentRowComponentStack)
        {
            Stack<BlockComponent> tempComponentStack = new Stack<BlockComponent>();

            for (int i = 0; i < currentRowComponentStack.Count; i++)
            {
                var topComponent= currentRowComponentStack.Pop();

                double rowYOffset = GetChildrenMaxCenterLine(topComponent.Children[topComponent.CurrentInputPart]);
                AlignChildrenToMaxCenter(rowYOffset + currentInputBlockRow.RowRect.Top, topComponent.Children[topComponent.CurrentInputPart]);
                UpdateBlockRectByChildren(topComponent, topComponent.Children[topComponent.CurrentInputPart]);

                tempComponentStack.Push(topComponent);
            }

            for (int i = 0; i < tempComponentStack.Count; i++)
            {
                var backComponent = tempComponentStack.Pop();
                currentRowComponentStack.Push(backComponent);
            }

        }

        private void UpdateBlockRectByChildren(BlockComponent blockComponent,List<IBlockComponent> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                {
                    var childRect = item.GetRect();
                   
                    var blockComponentItem = blockComponent as BlockComponent;

                    //更新高度
                    if (childRect.Top + childRect.Height > currentInputBlockRow.RowRect.Height)
                    {

                        var rect = new Rect(blockComponent.Rect.Location, new Size(blockComponent.Rect.Width, childRect.Top + childRect.Height));
                        blockComponentItem.SetRect(rect);
                    }

                    //更新宽度
                    if (childRect.Left + childRect.Width > currentInputBlockRow.RowRect.Width)
                    {
                        var rect = new Rect(blockComponent.Rect.Location, new Size(childRect.Left + childRect.Width, blockComponent.Rect.Height));
                        blockComponentItem.SetRect(rect);
                    }
                }
            }
        }

        private void AlignChildrenToMaxCenter(double maxcCenterLienYOffset,List<IBlockComponent> children)
        {
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                {
                    var itemAlignmentYOffset=item.GetHorizontialAlignmentYOffset()+currentInputBlockRow.RowRect.Top;

                    double adjustYOffset = maxcCenterLienYOffset - itemAlignmentYOffset;

                    AdjustChildLocation(item, 0, adjustYOffset);
                }
            }
        }

        private double GetChildrenMaxCenterLine(List<IBlockComponent> children)
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

        private void AdjustBlockChildLocation(MathData.Block block, double xOffset, double yOffset)
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
                }
            }
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

        private Line RefreshLineElement(LineBlock lineBlock,Line oldLine)
        {
            Line newLine = new Line();
            newLine.X1 = oldLine.X1;
            newLine.Y1 = oldLine.Y1;
            newLine.X2 = oldLine.X1+ lineBlock.Rect.Width;
            newLine.Y2 = oldLine.Y2;
            newLine.Stroke = oldLine.Stroke;
            newLine.StrokeThickness = oldLine.StrokeThickness;
            newLine.Uid = oldLine.Uid;

            return newLine;
        }

        private void AdjustChildLocation(IBlockComponent block, double xOffset, double yOffset)
        {
            if (block is MathData.Block)
            {
                var blockItem = block as MathData.Block;

                AdjustBlockChildLocation(blockItem, xOffset, yOffset);
            }
            else if (block is BlockComponent)
            {
                var componentBlock = block as BlockComponent;
                foreach (var componentChild in componentBlock.Children)
                {
                    if (componentChild.Count > 0)
                    {
                        foreach (var partChild in componentChild)
                        {
                            if (partChild is MathData.Block)
                            {
                                var blockChild = partChild as MathData.Block;
                                AdjustBlockChildLocation(blockChild, xOffset, yOffset);
                            }
                            else if (partChild is BlockComponent)
                            {
                                AdjustChildLocation(partChild, xOffset, yOffset);
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

                CharBlock charBlock = new CharBlock(inputedTextBlock.Text, inputedTextBlock.FontStyle.ToString(), inputedTextBlock.FontFamily.ToString(), inputedTextBlock.Foreground.ToString(), inputedTextBlock.FontSize, inputedTextBlock.Uid);
                var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                var oldCaretTop = Canvas.GetTop(caretTextBox);
                //初始化块区域
                var charRect = charBlock.CreateRect(new Point(oldCaretLeft, oldCaretTop - currentInputBlockRow.RowRect.Top));

                currentInputBlockRow.AddBlockToRow(charBlock, LayoutRowChildrenHorizontialCenter, RefreshBlockRow);

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

        private void SetCaretLocation(double x, double y)
        {
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            Canvas.SetLeft(caretTextBox, oldCaretLeft + x);
            Canvas.SetTop(caretTextBox, oldCaretTop + y);
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
    }
}
