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

        private Point rowPoint;

        public Point RowPoint
        {
            get { return rowPoint; }
            set { rowPoint = value; }
        }


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


        public PolylineBlock(Point rowPoint)
        {
            this.rowPoint = rowPoint;
        }

        public double GetHorizontialAlignmentYOffset()
        {
            throw new NotImplementedException();
        }

        public Rect GetRect()
        {
            var shapeRect = new Rect(rowPoint, new Size(0,0));
            foreach (var item in polylinePoints)
            {
                shapeRect.Union(item);
            }
            this.Rect = shapeRect;
            return this.Rect;
        }

        public void Move(Vector offsetVector)
        {
            this.Rect = new Rect(this.Rect.X + offsetVector.X, this.Rect.Y + offsetVector.Y, this.Rect.Width, this.Rect.Height);
        }
    }
}
