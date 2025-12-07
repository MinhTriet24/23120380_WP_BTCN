using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintApp.Core.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Windows.UI;

namespace PaintApp.Core.Helpers
{
    public static class ShapeSerializer
    {
        public static string Serialize(UIElementCollection shapes)
        {
            var dataList = new List<ShapeData>();

            foreach (var item in shapes)
            {
                if (item is Shape shape)
                {
                    dataList.Add(ConvertToData(shape));
                }
            }
            return JsonSerializer.Serialize(dataList);
        }

        public static List<Shape> Deserialize(string json)
        {
            var shapes = new List<Shape>();
            if (string.IsNullOrEmpty(json)) return shapes;

            List<ShapeData> dataList = null;

            try
            {
                dataList = JsonSerializer.Deserialize<List<ShapeData>>(json);
            }
            catch
            {
                try
                {
                    var singleData = JsonSerializer.Deserialize<ShapeData>(json);
                    if (singleData != null)
                    {
                        dataList = new List<ShapeData> { singleData };
                    }
                }
                catch { /* Dữ liệu hỏng hẳn thì bỏ qua */ }
            }

            if (dataList == null) return shapes;

            foreach (var data in dataList)
            {
                var shape = ConvertToShape(data);
                if (shape != null) shapes.Add(shape);
            }
            return shapes;
        }

        public static string SerializeSingle(Shape shape)
        {
            if (shape == null) return "[]";

            var data = ConvertToData(shape);
            var listWrapper = new List<ShapeData> { data }; // Gói vào list

            return JsonSerializer.Serialize(listWrapper);
        }
       
        // --- Helper: Hex String -> SolidColorBrush ---
        private static ShapeData ConvertToData(Shape shape)
        {
            var data = new ShapeData
            {
                Type = shape.GetType().Name,
                Top = Canvas.GetTop(shape),
                Left = Canvas.GetLeft(shape),
                Width = shape.Width,
                Height = shape.Height,
                StrokeThickness = shape.StrokeThickness,
                Fill = (shape.Fill as SolidColorBrush)?.Color.ToString() ?? "#00FFFFFF",
                Stroke = (shape.Stroke as SolidColorBrush)?.Color.ToString() ?? "#FF000000"
            };

            if (shape is Line line)
            {
                data.X1 = line.X1; data.Y1 = line.Y1;
                data.X2 = line.X2; data.Y2 = line.Y2;
            }

            if (shape is Polygon poly)
            {
                data.Points = new List<string>();
                foreach (var p in poly.Points)
                    data.Points.Add($"{p.X.ToString(CultureInfo.InvariantCulture)},{p.Y.ToString(CultureInfo.InvariantCulture)}");
            }

            if (shape.StrokeDashArray != null)
                data.StrokeDashArray = new List<double>(shape.StrokeDashArray);

            return data;
        }

        private static Shape ConvertToShape(ShapeData data)
        {
            Shape shape = null;
            switch (data.Type)
            {
                case "Rectangle": shape = new Rectangle(); break;
                case "Ellipse": shape = new Ellipse(); break;
                case "Line": shape = new Line { X1 = data.X1, Y1 = data.Y1, X2 = data.X2, Y2 = data.Y2 }; break;
                case "Polygon": shape = new Polygon(); break;
            }

            if (shape == null) return null;

            if (!(shape is Line))
            {
                shape.Width = data.Width;
                shape.Height = data.Height;
                Canvas.SetTop(shape, data.Top);
                Canvas.SetLeft(shape, data.Left);
            }

            if (shape is Polygon poly && data.Points != null)
            {
                foreach (var pointStr in data.Points)
                {
                    var parts = pointStr.Split(',');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                        double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                    {
                        poly.Points.Add(new Windows.Foundation.Point(x, y));
                    }
                }
            }

            if (data.StrokeDashArray != null)
            {
                var col = new DoubleCollection();
                foreach (var d in data.StrokeDashArray) col.Add(d);
                shape.StrokeDashArray = col;
            }

            shape.StrokeThickness = data.StrokeThickness;
            shape.Fill = GetBrushFromHex(data.Fill);
            shape.Stroke = GetBrushFromHex(data.Stroke);

            return shape;
        }

        private static SolidColorBrush GetBrushFromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return new SolidColorBrush(Colors.Transparent);
            try
            {
                hex = hex.Replace("#", "");
                byte a = 255, r = 0, g = 0, b = 0;
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }
                else if (hex.Length == 6)
                {
                    r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                }
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            catch { return new SolidColorBrush(Colors.Black); }
        }
    }
}