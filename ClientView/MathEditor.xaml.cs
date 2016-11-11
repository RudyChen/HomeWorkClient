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


        public MathEditor()
        {
            InitializeComponent();
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
                lineOffsetX = AcceptChineseInputText(0, 0);
            }
            else
            {
                lineOffsetX = AcceptEnglishInputText(0, 0);
            }

            SetCaretLocation(lineOffsetX, 0);
        }

        private void EquationTypeButton_Clicked(object sender, RoutedEventArgs e)
        {

        }


        private double AcceptEnglishInputText(double lineOffsetX, double lineOffsetY)
        {
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = caretTextBox.Text;
            inputedTextBlock.FontSize = caretTextBox.FontSize;
            inputedTextBlock.FontFamily = fontFamily;
            inputedTextBlock.FontStyle =  FontStyles.Italic;
            FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            editorCanvas.Children.Add(inputedTextBlock);

            Canvas.SetLeft(inputedTextBlock, oldCaretLeft + lineOffsetX);
            Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
            caretTextBox.Text = string.Empty;

            return formatted.WidthIncludingTrailingWhitespace;

        }

        private double AcceptChineseInputText(double lineOffsetX, double lineOffsetY)
        {
            double allWidth = 0;
            foreach (var item in caretTextBox.Text)
            {
                TextBlock inputedTextBlock = new TextBlock();
                inputedTextBlock.Text = item.ToString();
                inputedTextBlock.FontSize = caretTextBox.FontSize;
                inputedTextBlock.FontFamily = fontFamily;
                inputedTextBlock.FontStyle =  FontStyles.Normal;
                FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

                var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                var oldCaretTop = Canvas.GetTop(caretTextBox);

                editorCanvas.Children.Add(inputedTextBlock);

                Canvas.SetLeft(inputedTextBlock, oldCaretLeft + allWidth + lineOffsetX);
                Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);

                allWidth += formatted.WidthIncludingTrailingWhitespace;
            }

            caretTextBox.Text = string.Empty;

            return allWidth;

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
    }
}
