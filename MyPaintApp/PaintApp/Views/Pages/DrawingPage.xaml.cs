using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.Foundation; // Chứa struct Point
using PaintApp.ViewModels;
using PaintApp.Core.Enums;
using Microsoft.UI.Xaml.Media;
using System.Net.WebSockets;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaintApp.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrawingPage : Page
    {

        public DrawingViewModel ViewModel { get; }

        private bool _isDrawing = false;
        private Point _startPoint;
        private Shape _currentShape;
        public DrawingPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetService<DrawingViewModel>();
        }

        private void OnToolClicked(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton btn && btn.Tag is string toolName)
            {
                if (Enum.TryParse<ToolType>(toolName, out var tool))
                {
                    ViewModel.CurrentTool = tool;
                }
            }
        }

        private void OnCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.CurrentTool == ToolType.Cursor) return;

            _isDrawing = true;

            var pt = e.GetCurrentPoint(DrawingCanvas);
            _startPoint = pt.Position;

            _currentShape = CreateShape(ViewModel.CurrentTool);

            if (_currentShape != null)
            {
                _currentShape.Stroke = ViewModel.StrokeColor;
                _currentShape.StrokeThickness = ViewModel.StrokeThickness;
                _currentShape.Fill = ViewModel.FillColor;

                if (_currentShape is Line)
                {
                    Canvas.SetLeft(_currentShape, 0);
                    Canvas.SetTop(_currentShape, 0);
                }
                else
                {
                    Canvas.SetLeft(_currentShape, _startPoint.X);
                    Canvas.SetTop(_currentShape, _startPoint.Y);
                }

                DrawingCanvas.Children.Add(_currentShape);
                DrawingCanvas.CapturePointer(e.Pointer);
            }
        }

        private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDrawing || _currentShape == null) return;

            var pt = e.GetCurrentPoint(DrawingCanvas);
            var currentPoint = pt.Position;

            UpdateShape(_currentShape, currentPoint);
        }

        private void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                _currentShape = null;
                DrawingCanvas.ReleasePointerCapture(e.Pointer);
            }
        }

        // --- (HELPER FUNCTIONS) ---
        private Shape CreateShape(ToolType tool)
        {
            switch (tool)
            {
                case ToolType.Line: return new Line();
                case ToolType.Rectangle: return new Rectangle();
                case ToolType.Circle:
                case ToolType.Oval: 
                    return new Ellipse();
                case ToolType.Triangle:
                    return new Polygon();
                default: return null;
            }
        }

        private void UpdateShape(Shape shape, Point endPoint)
        {
            if (shape is Line line)
            {
                line.X1 = _startPoint.X;
                line.Y1 = _startPoint.Y;
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
                return;
            }

            var left = Math.Min(_startPoint.X, endPoint.X);
            var top = Math.Min(_startPoint.Y, endPoint.Y);

            var width = Math.Abs(_startPoint.X - endPoint.X);
            var height = Math.Abs(_startPoint.Y - endPoint.Y);

            Canvas.SetLeft(shape, left);
            Canvas.SetTop(shape, top);

            //Circle
            if(ViewModel.CurrentTool == ToolType.Circle)
            {
                var size = Math.Max(width, height);
                shape.Width = size;
                shape.Height = size;
            }
            else if(shape is Polygon polygon) //Triangle and Polygon
            {
                polygon.Points.Clear();

                if(ViewModel.CurrentTool == ToolType.Triangle)
                {
                    polygon.Points.Add(new Point(width/2, 0));
                    polygon.Points.Add(new Point(0, height));
                    polygon.Points.Add(new Point(width, height));
                }
            }
            else //Rectangle, Oval 
            {
                shape.Width = width;
                shape.Height = height;
            }
        }

    }
}
