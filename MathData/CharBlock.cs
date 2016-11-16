using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MathData
{
    public class CharBlock : Block, IBlockComponent
    {
        private int fontSize;

        private string foreground;

        private string fontFamily;

        private string fontStyle;

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        public string ForeGround
        {
            get { return foreground; }
            set { foreground = value; }
        }

        public string FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }


        public string FontFamily
        {
            get { return fontFamily; }
            set { fontFamily = value; }
        }

        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        public Size GetSize()
        {
            Size blockSize = new Size();
            Color foregroundColor=(Color)ColorConverter.ConvertFromString(foreground);
            FormattedText formatted = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize, new SolidColorBrush(foregroundColor));
            blockSize.Width = formatted.Width;
            blockSize.Height = formatted.Height;
            return blockSize;
        }

        public void AddChild(IBlockComponent blockComponent, Point rowPoint)
        {
            throw new NotImplementedException();
        }

        public void AddShapeChild()
        {
            throw new NotImplementedException();
        }

        public void RemoveCharChild()
        {
            throw new NotImplementedException();
        }

        public void LayoutChildren()
        {
            throw new NotImplementedException();
        }
    }
}
