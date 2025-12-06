using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintApp.Core.Enums;
using PaintApp.ViewModels;
using System;
using System.Net.WebSockets;
using Windows.Foundation;

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
        private Shape _selectedShape;
        private Point _lastMovePoint;
        private Brush _originalStroke;
        private bool _isDraggingShape = false;
        public DrawingPage()
        {
            ViewModel = App.Current.Services.GetService<DrawingViewModel>();
            InitializeComponent();
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
            if (ViewModel.CurrentTool == ToolType.Cursor)
            {
                DeselectCurrentShape();
                return;
            }

            _isDrawing = true;

            var pt = e.GetCurrentPoint(DrawingCanvas);
            _startPoint = pt.Position;

            _currentShape = CreateShape(ViewModel.CurrentTool);

            if (_currentShape != null)
            {
                _currentShape.Stroke = new SolidColorBrush(ViewModel.StrokeColor);
                _currentShape.StrokeThickness = ViewModel.StrokeThickness;
                _currentShape.Fill = ViewModel.FillColor;
                _currentShape.StrokeDashArray = ViewModel.StrokeDashArray;

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
                if (_isDrawing)
                {
                    _isDrawing = false;

                    if (_currentShape != null)
                    {
                        AttachEventsToShape(_currentShape);

                        if (_currentShape.Fill == null)
                        {
                            _currentShape.Fill = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        }
                    }

                    _currentShape = null;
                    DrawingCanvas.ReleasePointerCapture(e.Pointer);
                }
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
                case ToolType.Polygon:
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
                else if(ViewModel.CurrentTool == ToolType.Polygon)
                {
                    polygon.Points.Add(new Point(width/2, 0));     
                    polygon.Points.Add(new Point(0, height*0.25));   
                    polygon.Points.Add(new Point(0, height*0.75)); 
                    polygon.Points.Add(new Point(width/2, height));
                    polygon.Points.Add(new Point(width, height*0.75));
                    polygon.Points.Add(new Point(width, height*0.25));
                }

                polygon.Stretch = Stretch.Fill;
                polygon.Width = width;
                polygon.Height = height;
            }
            else //Rectangle, Oval 
            {
                shape.Width = width;
                shape.Height = height;
            }
        }

        private void OnStrokeStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;

            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
            {
                if (item.Tag != null)
                {
                    var style = item.Tag.ToString();
                    ViewModel.SetStrokeStyle(style);
                }
            }
        }

        private void SelectShape(Shape shape)
        {
            if (_selectedShape == shape) return;

            DeselectCurrentShape();

            _selectedShape = shape;

            _originalStroke = _selectedShape.Stroke;

            _selectedShape.Stroke = new SolidColorBrush(Microsoft.UI.Colors.DodgerBlue);
        }

        private void DeselectCurrentShape()
        {
            if (_selectedShape != null)
            {
                _selectedShape.Stroke = _originalStroke ?? new SolidColorBrush(Microsoft.UI.Colors.Black);

                _selectedShape = null;
                _originalStroke = null;
            }
        }

        private void AttachEventsToShape(Shape shape)
        {
            shape.PointerPressed += Shape_PointerPressed;
            shape.PointerMoved += Shape_PointerMoved;
            shape.PointerReleased += Shape_PointerReleased;

            var menu = new MenuFlyout();

            // Tạo các mục menu
            var itemRect = CreateMenuItem("Convert to Rectangle", ToolType.Rectangle);
            var itemOval = CreateMenuItem("Convert to Oval", ToolType.Oval);
            var itemTri = CreateMenuItem("Convert to Triangle", ToolType.Triangle);
            var itemPoly = CreateMenuItem("Convert to Polygon", ToolType.Polygon);
            var itemCircle = CreateMenuItem("Convert to Circle", ToolType.Circle);
            var itemLine = CreateMenuItem("Convert to Line", ToolType.Line);

            menu.Items.Add(itemRect);
            menu.Items.Add(itemOval);
            menu.Items.Add(itemTri);
            menu.Items.Add(itemPoly);
            menu.Items.Add(itemCircle);
            menu.Items.Add(itemLine);

            // Gán menu vào hình
            shape.ContextFlyout = menu;
        }

        private void Shape_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.CurrentTool != ToolType.Cursor) return;

            var shape = sender as Shape;
            if (shape == null) return;

            SelectShape(shape);

            _isDraggingShape = true;

            var pt = e.GetCurrentPoint(DrawingCanvas);
            _lastMovePoint = pt.Position;

            shape.CapturePointer(e.Pointer);

            e.Handled = true;
        }

        private void Shape_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isDraggingShape && _selectedShape != null && ViewModel.CurrentTool == ToolType.Cursor)
            {
                var pt = e.GetCurrentPoint(DrawingCanvas);
                var currentPoint = pt.Position;

                double offsetX = currentPoint.X - _lastMovePoint.X;
                double offsetY = currentPoint.Y - _lastMovePoint.Y;

                double oldLeft = Canvas.GetLeft(_selectedShape);
                double oldTop = Canvas.GetTop(_selectedShape);

                Canvas.SetLeft(_selectedShape, oldLeft + offsetX);
                Canvas.SetTop(_selectedShape, oldTop + offsetY);

                _lastMovePoint = currentPoint;
            }
        }

        private void Shape_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isDraggingShape)
            {
                _isDraggingShape = false;
                var shape = sender as Shape;
                shape?.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        private void RecalculatePolygonPoints(Polygon polygon, double width, double height, ToolType type)
        {
            polygon.Points.Clear();

            if (type == ToolType.Triangle)
            {
                polygon.Points.Add(new Point(width / 2, 0));     
                polygon.Points.Add(new Point(0, height));       
                polygon.Points.Add(new Point(width, height));   
            }
            else if (type == ToolType.Polygon) // Lục giác
            {
                polygon.Points.Add(new Point(width/2, 0));
                polygon.Points.Add(new Point(0, height*0.25));
                polygon.Points.Add(new Point(0, height*0.75));
                polygon.Points.Add(new Point(width/2, height));
                polygon.Points.Add(new Point(width, height*0.75));
                polygon.Points.Add(new Point(width, height*0.25));
            }

            polygon.Stretch = Stretch.Fill;
        }

        private void ConvertSelectedShape(ToolType targetType)
        {
            if (_selectedShape == null) return;

            double left = Canvas.GetLeft(_selectedShape);
            double top = Canvas.GetTop(_selectedShape);

            double width = _selectedShape.Width;
            double height = _selectedShape.Height;

            var stroke = _selectedShape.Stroke;
            var fill = _selectedShape.Fill;
            var thickness = _selectedShape.StrokeThickness;
            var dashArray = _selectedShape.StrokeDashArray;

            Shape newShape = CreateShape(targetType);
            if (newShape == null) return;

            if (targetType == ToolType.Circle)
            {
                double size = Math.Max(width, height);
                newShape.Width = size;
                newShape.Height = size;
            }
            else if (newShape is Line line)
            {
                line.X1 = 0;
                line.Y1 = 0;
                line.X2 = width;
                line.Y2 = height;

                line.Width = width;
                line.Height = height;
            }
            else
            {
                newShape.Width = width;
                newShape.Height = height;
            }

            newShape.Stroke = _originalStroke;
            newShape.Fill = fill;
            newShape.StrokeThickness = thickness;

            if (dashArray != null)
            {
                var newDash = new DoubleCollection();
                foreach (var d in dashArray) newDash.Add(d);
                newShape.StrokeDashArray = newDash;
            }

            Canvas.SetLeft(newShape, left);
            Canvas.SetTop(newShape, top);

            if (newShape is Polygon poly)
            {
                RecalculatePolygonPoints(poly, width, height, targetType);
            }

            int index = DrawingCanvas.Children.IndexOf(_selectedShape);
            DrawingCanvas.Children.RemoveAt(index);
            DrawingCanvas.Children.Insert(index, newShape);

            AttachEventsToShape(newShape);
            SelectShape(newShape);
        }

        private MenuFlyoutItem CreateMenuItem(string text, ToolType type)
        {
            var item = new MenuFlyoutItem { Text = text, Tag = type };
            item.Click += OnConvertShapeClicked;
            return item;
        }

        private void OnConvertShapeClicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && item.Tag is ToolType targetType)
            {
                ConvertSelectedShape(targetType);
            }
        }

    }
}
