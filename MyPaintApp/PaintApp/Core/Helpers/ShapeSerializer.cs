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

namespace WinUI_PaintApp.Core.Helpers
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

                    dataList.Add(data);
                }
            }
            return JsonSerializer.Serialize(dataList);
        }

        public static List<Shape> Deserialize(string json)
        {
            var shapes = new List<Shape>();
            if (string.IsNullOrEmpty(json)) return shapes;

            var dataList = JsonSerializer.Deserialize<List<ShapeData>>(json);

            foreach (var data in dataList)
            {
                Shape shape = null;
                switch (data.Type)
                {
                    case "Rectangle": shape = new Rectangle(); break;
                    case "Ellipse": shape = new Ellipse(); break;
                    case "Line":
                        shape = new Line { X1 = data.X1, Y1 = data.Y1, X2 = data.X2, Y2 = data.Y2 };
                        break;
                }

                if (shape != null)
                {
                    if (!(shape is Line))
                    {
                        shape.Width = data.Width;
                        shape.Height = data.Height;
                        Canvas.SetTop(shape, data.Top);
                        Canvas.SetLeft(shape, data.Left);
                    }

                    shape.StrokeThickness = data.StrokeThickness;
                    shape.Fill = GetBrushFromHex(data.Fill);
                    shape.Stroke = GetBrushFromHex(data.Stroke);

                    shapes.Add(shape);
                }
            }
            return shapes;
        }

        public static string SerializeSingle(Shape shape)
        {
            if (shape == null) return "";
            var data = ConvertToData(shape);
            return JsonSerializer.Serialize(data);
        }

        // --- Helper: Convert Shape -> ShapeData ---
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

            // Lưu Line
            if (shape is Line line)
            {
                data.X1 = line.X1; data.Y1 = line.Y1;
                data.X2 = line.X2; data.Y2 = line.Y2;
            }

            // Lưu Polygon Points
            if (shape is Polygon poly)
            {
                data.Points = new List<string>();
                foreach (var p in poly.Points)
                {
                    // Lưu dạng chuỗi "10.5,20.5" (dùng InvariantCulture để đảm bảo dấu chấm)
                    data.Points.Add($"{p.X.ToString(CultureInfo.InvariantCulture)},{p.Y.ToString(CultureInfo.InvariantCulture)}");
                }
            }

            // Lưu StrokeDashArray (Nét đứt)
            if (shape.StrokeDashArray != null)
            {
                data.StrokeDashArray = new List<double>(shape.StrokeDashArray);
            }

            return data;
        }

        // --- Helper: Hex String -> SolidColorBrush ---
        private static SolidColorBrush GetBrushFromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return new SolidColorBrush(Colors.Transparent);

            try
            {
                hex = hex.Replace("#", "");

                byte a = 255;
                byte r = 0;
                byte g = 0;
                byte b = 0;

                // Xử lý dạng 8 ký tự (ARGB) - Ví dụ: FF000000
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }
                // Xử lý dạng 6 ký tự (RGB) - Ví dụ: FF0000
                else if (hex.Length == 6)
                {
                    r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                }

                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            catch
            {
                return new SolidColorBrush(Colors.Black); // Fallback nếu lỗi
            }
        }
    }
}