using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathData
{
    public class PolylineBlock : ShapeBlock, IBlockComponent
    {

        private List<Point> polylinePoints=new List<Point>();

        private string stroke;

        private double strokeThickness;

        public double StrokeThickness
        {
            get { return strokeThickness; }
            set { strokeThickness = value; }
        }


        public string Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }


        public List<Point> PolylinePoints
        {
            get { return polylinePoints; }
            set { polylinePoints = value; }
        }


        public PolylineBlock()
        {

        }

        public double GetHorizontialAlignmentYOffset()
        {
            throw new NotImplementedException();
        }

        public Rect GetRect()
        {
            return this.Rect;
        }

        public void Move(Vector offsetVector)
        {
            this.Rect = new Rect(this.Rect.X + offsetVector.X, this.Rect.Y + offsetVector.Y, this.Rect.Width, this.Rect.Height);
        }
    }
}
