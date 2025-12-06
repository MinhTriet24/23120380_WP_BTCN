using System.Collections.Generic;

namespace PaintApp.Core.Models
{
    public class ShapeData
    {
        public string Type { get; set; } // "Rectangle", "Ellipse", "Line", "Polygon"
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Fill { get; set; }
        public string Stroke { get; set; }
        public double StrokeThickness { get; set; }

        // Lưu kiểu nét đứt (nếu có)
        public List<double> StrokeDashArray { get; set; }

        // Dành cho Line
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public List<string> Points { get; set; }
    }
}