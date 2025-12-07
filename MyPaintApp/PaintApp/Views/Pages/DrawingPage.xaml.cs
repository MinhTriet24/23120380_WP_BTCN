using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using PaintApp.Core.Enums;
using PaintApp.ViewModels;
using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

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
        private bool _isResizing = false;
        private string _currentResizeHandleTag = "";
        private UserProfile _pendingProfile;

        public DrawingPage()
        {
            ViewModel = App.Current.Services.GetService<DrawingViewModel>();
            InitializeComponent();
            ViewModel.OnCanvasLoaded += ViewModel_OnCanvasLoaded;
            this.Loaded += DrawingPage_Loaded;
        }

        private void OnToolClicked(object sender, RoutedEventArgs e)
        {
            DeselectCurrentShape();

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
                if (ViewModel.StrokeDashArray != null)
    {
        var newDash = new DoubleCollection();
        foreach (var d in ViewModel.StrokeDashArray)
        {
            newDash.Add(d);
        }
        _currentShape.StrokeDashArray = newDash;
    }
                _currentShape.Fill = new SolidColorBrush(ViewModel.FillColor);

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
            var pt = e.GetCurrentPoint(DrawingCanvas);
            var currentX = pt.Position.X;
            var currentY = pt.Position.Y;

            // 1. TRƯỜNG HỢP: ĐANG VẼ HÌNH MỚI
            if (_isDrawing && _currentShape != null)
            {
                UpdateShape(_currentShape, pt.Position);
                return;
            }

            // 2. TRƯỜNG HỢP: ĐANG RESIZE (Kéo 8 điểm neo)
            if (_isResizing && _selectedShape != null)
            {
                double l = Canvas.GetLeft(_selectedShape);
                double t = Canvas.GetTop(_selectedShape);
                double w = _selectedShape.Width;
                double h = _selectedShape.Height;

                if (double.IsNaN(w)) w = 0;
                if (double.IsNaN(h)) h = 0;

                switch (_currentResizeHandleTag)
                {
                    case "Right":
                        double newW_R = currentX - l;
                        if (newW_R > 10) _selectedShape.Width = newW_R;
                        break;

                    case "Bottom":
                        double newH_B = currentY - t;
                        if (newH_B > 10) _selectedShape.Height = newH_B;
                        break;

                    case "Left":
                        double newL = currentX;
                        double newW_L = (l + w) - currentX;
                        if (newW_L > 10)
                        {
                            Canvas.SetLeft(_selectedShape, newL);
                            _selectedShape.Width = newW_L;
                        }
                        break;

                    case "Top":
                        double newT = currentY;
                        double newH_T = (t + h) - currentY;
                        if (newH_T > 10)
                        {
                            Canvas.SetTop(_selectedShape, newT);
                            _selectedShape.Height = newH_T;
                        }
                        break;

                    case "BottomRight":
                        double newW_BR = currentX - l;
                        double newH_BR = currentY - t;
                        if (newW_BR > 10) _selectedShape.Width = newW_BR;
                        if (newH_BR > 10) _selectedShape.Height = newH_BR;
                        break;

                    case "TopLeft":
                        double newL_TL = currentX;
                        double newT_TL = currentY;
                        double newW_TL = (l + w) - currentX;
                        double newH_TL = (t + h) - currentY;
                        if (newW_TL > 10 && newH_TL > 10)
                        {
                            Canvas.SetLeft(_selectedShape, newL_TL);
                            Canvas.SetTop(_selectedShape, newT_TL);
                            _selectedShape.Width = newW_TL;
                            _selectedShape.Height = newH_TL;
                        }
                        break;

                    case "TopRight":
                        double newT_TR = currentY;
                        double newW_TR = currentX - l;
                        double newH_TR = (t + h) - currentY;
                        if (newW_TR > 10 && newH_TR > 10)
                        {
                            Canvas.SetTop(_selectedShape, newT_TR);
                            _selectedShape.Width = newW_TR;
                            _selectedShape.Height = newH_TR;
                        }
                        break;

                    case "BottomLeft":
                        double newL_BL = currentX;
                        double newW_BL = (l + w) - currentX;
                        double newH_BL = currentY - t;
                        if (newW_BL > 10 && newH_BL > 10)
                        {
                            Canvas.SetLeft(_selectedShape, newL_BL);
                            _selectedShape.Width = newW_BL;
                            _selectedShape.Height = newH_BL;
                        }
                        break;
                }

                if (_selectedShape is Line line)
                {
                    line.X2 = _selectedShape.Width;
                    line.Y2 = _selectedShape.Height;
                }
                else if (_selectedShape is Polygon poly)
                {
                    ToolType type = poly.Points.Count == 3 ? ToolType.Triangle : ToolType.Polygon;
                    RecalculatePolygonPoints(poly, _selectedShape.Width, _selectedShape.Height, type);
                }

                UpdateAdornerPosition();
                return;
            }

            // 3. TRƯỜNG HỢP: ĐANG DI CHUYỂN HÌNH (Dragging)
            if (_isDraggingShape && _selectedShape != null && ViewModel.CurrentTool == ToolType.Cursor)
            {
                double offsetX = currentX - _lastMovePoint.X;
                double offsetY = currentY - _lastMovePoint.Y;

                double oldLeft = Canvas.GetLeft(_selectedShape);
                double oldTop = Canvas.GetTop(_selectedShape);

                Canvas.SetLeft(_selectedShape, oldLeft + offsetX);
                Canvas.SetTop(_selectedShape, oldTop + offsetY);

                _lastMovePoint = pt.Position;

                UpdateAdornerPosition();
            }
        }

        private void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
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

            if (_isResizing)
            {
                _isResizing = false;
                _currentResizeHandleTag = "";

                HandleTopLeft.ReleasePointerCapture(e.Pointer);
                HandleTopRight.ReleasePointerCapture(e.Pointer);
                HandleBottomLeft.ReleasePointerCapture(e.Pointer);
                HandleBottomRight.ReleasePointerCapture(e.Pointer);

                HandleTop.ReleasePointerCapture(e.Pointer);
                HandleBottom.ReleasePointerCapture(e.Pointer);
                HandleLeft.ReleasePointerCapture(e.Pointer);
                HandleRight.ReleasePointerCapture(e.Pointer);
            }

            if (_isDraggingShape)
            {
                _isDraggingShape = false;
                var shape = sender as Shape;
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

            UpdateAdornerPosition();

            if (_selectedShape.Fill is SolidColorBrush fillBrush)
            {
                ViewModel.FillColor = fillBrush.Color;
            }
            else
            {
                ViewModel.FillColor = Microsoft.UI.Colors.Transparent;
            }
        }

        private void DeselectCurrentShape()
        {
            if (_selectedShape != null)
            {
                _selectedShape.Stroke = _originalStroke ?? new SolidColorBrush(Microsoft.UI.Colors.Black);

                _selectedShape = null;
                _originalStroke = null;
                ResizeAdorner.Visibility = Visibility.Collapsed;
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

            var itemSaveTemplate = new MenuFlyoutItem
            {
                Text = "Save as Template",
                Icon = new SymbolIcon(Symbol.Save)
            };

            var itemDelete = new MenuFlyoutItem { Text = "Delete", Icon = new SymbolIcon(Symbol.Delete) };
            itemDelete.Click += (s, e) => DeleteSelectedShape();

            menu.Items.Add(itemRect);
            menu.Items.Add(itemOval);
            menu.Items.Add(itemTri);
            menu.Items.Add(itemPoly);
            menu.Items.Add(itemCircle);
            menu.Items.Add(itemLine);

            menu.Items.Add(itemDelete);
            menu.Items.Add(itemSaveTemplate);
            itemSaveTemplate.Click += async (s, e) =>
            {
                if (_selectedShape == shape)
                {
                    DeselectCurrentShape();
                }
                TextBox nameInput = new TextBox
                {
                    PlaceholderText = "Nhập tên mẫu hình...",
                    Text = $"{shape.GetType().Name} - {DateTime.Now:dd/MM HH:mm}"
                };

                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Lưu Mẫu Hình (Template)",
                    Content = nameInput,
                    PrimaryButtonText = "Lưu",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    string nameToSave = nameInput.Text.Trim();
                    if (string.IsNullOrEmpty(nameToSave))
                    {
                        ShowNotification("Tên mẫu hình không được để trống!");
                        return;
                    }

                    if (ViewModel.AddToTemplateCommand.CanExecute(null))
                    {
                        ViewModel.AddToTemplateCommand.Execute(new object[] { nameToSave, shape });

                        ShowNotification($"Đã lưu mẫu hình '{nameToSave}' thành công!");
                    }
                }
            };
            menu.Items.Add(new MenuFlyoutSeparator());


            shape.ContextFlyout = menu;
        }

        private async void OnDeleteTemplateClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                ContentDialog confirm = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Xác nhận xóa",
                    Content = "Bạn có chắc muốn xóa vĩnh viễn mẫu hình này?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy"
                };

                if (await confirm.ShowAsync() == ContentDialogResult.Primary)
                {
                    await ViewModel.DeleteTemplateCommand.ExecuteAsync(id);
                }
            }
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

                UpdateAdornerPosition();
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
                RecalculatePolygonPoints(poly, newShape.Width, newShape.Height, targetType);
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

        private void OnResizeHandleHover(object sender, PointerRoutedEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null) return;

            InputSystemCursorShape cursorShape = InputSystemCursorShape.Arrow;
            string tag = rect.Tag.ToString();

            // Góc chéo
            if (tag == "TopLeft" || tag == "BottomRight") cursorShape = InputSystemCursorShape.SizeNorthwestSoutheast;
            else if (tag == "TopRight" || tag == "BottomLeft") cursorShape = InputSystemCursorShape.SizeNortheastSouthwest;

            // Cạnh thẳng (Mới)
            else if (tag == "Top" || tag == "Bottom") cursorShape = InputSystemCursorShape.SizeNorthSouth; // Mũi tên dọc
            else if (tag == "Left" || tag == "Right") cursorShape = InputSystemCursorShape.SizeWestEast;   // Mũi tên ngang

            this.ProtectedCursor = InputSystemCursor.Create(cursorShape);
        }

        private void OnResizeHandleExit(object sender, PointerRoutedEventArgs e)
        {
            this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        }

        private void OnResizeHandlePressed(object sender, PointerRoutedEventArgs e)
        {
            var rect = sender as Rectangle;
            if (rect == null) return;

            _isResizing = true;
            _currentResizeHandleTag = rect.Tag.ToString();

            // Quan trọng: Capture chuột vào ô vuông để khi kéo nhanh ra ngoài vẫn nhận sự kiện
            rect.CapturePointer(e.Pointer);

            // Ngăn sự kiện lan xuống Canvas (để không bị hiểu nhầm là click chọn hình khác)
            e.Handled = true;
        }

        private void UpdateAdornerPosition()
        {
            if (_selectedShape == null)
            {
                ResizeAdorner.Visibility = Visibility.Collapsed;
                return;
            }

            ResizeAdorner.Visibility = Visibility.Visible;

            double left = Canvas.GetLeft(_selectedShape);
            double top = Canvas.GetTop(_selectedShape);
            double w = _selectedShape.Width;
            double h = _selectedShape.Height;

            if (double.IsNaN(w)) w = 0;
            if (double.IsNaN(h)) h = 0;

            Canvas.SetLeft(HandleTopLeft, left - 5); 
            Canvas.SetTop(HandleTopLeft, top - 5);

            Canvas.SetLeft(HandleTopRight, left + w - 5); 
            Canvas.SetTop(HandleTopRight, top - 5);

            Canvas.SetLeft(HandleBottomLeft, left - 5); 
            Canvas.SetTop(HandleBottomLeft, top + h - 5);

            Canvas.SetLeft(HandleBottomRight, left + w - 5); 
            Canvas.SetTop(HandleBottomRight, top + h - 5);

            Canvas.SetLeft(HandleTop, left + (w / 2) - 5);
            Canvas.SetTop(HandleTop, top - 5);

            Canvas.SetLeft(HandleBottom, left + (w / 2) - 5);
            Canvas.SetTop(HandleBottom, top + h - 5);

            Canvas.SetLeft(HandleLeft, left - 5);
            Canvas.SetTop(HandleLeft, top + (h / 2) - 5);

            Canvas.SetLeft(HandleRight, left + w - 5);
            Canvas.SetTop(HandleRight, top + (h / 2) - 5);
        }

        private void OnFillColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_selectedShape != null)
            {
                var newColor = args.NewColor;

                newColor.A = 255;

                _selectedShape.Fill = new SolidColorBrush(newColor);

                if (ViewModel.FillColor != newColor)
                {
                    ViewModel.FillColor = newColor;
                }
            }
        }

        private void OnStrokeColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_selectedShape != null)
            {
                var newColor = args.NewColor; 

                newColor.A = 255;

                _originalStroke = new SolidColorBrush(newColor);

                _selectedShape.Stroke = new SolidColorBrush(newColor);

                if (ViewModel.StrokeColor != newColor)
                {
                    ViewModel.StrokeColor = newColor;
                }
            }
        }

        private void DeleteSelectedShape()
        {
           if (_selectedShape != null)
            {
                DrawingCanvas.Children.Remove(_selectedShape);

                DeselectCurrentShape();
            }
        }

        private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                if (_selectedShape != null)
                {
                    DeleteSelectedShape();
                    e.Handled = true;
                }
            }
        }

        private void OnTemplateClicked(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var template = e.AddedItems[0] as PaintApp_Data.Entities.ShapeTemplate;
            if (template == null) return;

            try
            {
                var shapes = PaintApp.Core.Helpers.ShapeSerializer.Deserialize(template.ShapeJson);

                if (shapes != null && shapes.Count > 0)
                {
                    var newShape = shapes[0];

                    Canvas.SetLeft(newShape, 100);
                    Canvas.SetTop(newShape, 100);

                    AttachEventsToShape(newShape);

                    DrawingCanvas.Children.Add(newShape);

                    SelectShape(newShape);
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Không thể mở mẫu hình này (Dữ liệu cũ hoặc bị lỗi).");

                System.Diagnostics.Debug.WriteLine($"Template Error: {ex.Message}");
            }

            (sender as ListView).SelectedIndex = -1;
        }

        private void ViewModel_OnCanvasLoaded(List<Shape> shapes, string backgroundColorHex)
        {
            DrawingCanvas.Children.Clear();

            DrawingCanvas.Children.Add(ResizeAdorner);

            if (!string.IsNullOrEmpty(backgroundColorHex))
            {
            }

            foreach (var shape in shapes)
            {
                AttachEventsToShape(shape);
                DrawingCanvas.Children.Add(shape);
            }
        }

        private void OnOpenFlyoutOpened(object sender, object e)
        {
            ViewModel.LoadSavedCanvasesCommand.Execute(null);
        }

        private void OnSavedCanvasClicked(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as PaintApp_Data.Entities.DrawingCanvas;
                if (item != null)
                {
                    ViewModel.OpenCanvasCommand.Execute(item.Id);

                    if (sender is ListView lv) lv.SelectedIndex = -1;
                }
            }
        }

        private async void OnDeleteCanvasClicked(object sender, RoutedEventArgs e)
        {
            e.OriginalSource.ToString();

            if (sender is Button btn && btn.Tag is int idToDelete)
            {
                bool isDeletingCurrent = (idToDelete == ViewModel.CurrentCanvasId);

                if (isDeletingCurrent)
                {
                    await DeleteCurrentCanvasWithConfirmation();
                }
                else
                {
                    await ViewModel.DeleteCanvasCommand.ExecuteAsync(idToDelete);
                }
            }
        }

        private async void ShowNotification(string message)
        {
            var root = this.XamlRoot ?? App.Current.Window.Content.XamlRoot;

            if (this.XamlRoot == null) return;

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot, 
                Title = "Thông báo",
                Content = message,
                CloseButtonText = "Đóng"
            };
            await dialog.ShowAsync();
        }

        private async void OnDeleteCurrentBoardClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentCanvasId == -1)
            {
                ContentDialog clearDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Làm mới bảng vẽ",
                    Content = "Bạn có muốn xóa toàn bộ hình trên màn hình để vẽ lại không?",
                    PrimaryButtonText = "Làm mới",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary
                };

                if (await clearDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    ResetCanvasUI();
                }
            }
            else
            {
                await DeleteCurrentCanvasWithConfirmation();
            }
        }

        private async System.Threading.Tasks.Task DeleteCurrentCanvasWithConfirmation()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Xóa bảng vẽ vĩnh viễn",
                Content = "CẢNH BÁO: Bảng vẽ này đang được lưu trong cơ sở dữ liệu.\n\nHành động này sẽ xóa hoàn toàn bảng vẽ này khỏi hệ thống và không thể khôi phục.\nBạn có chắc chắn không?",
                PrimaryButtonText = "Xóa ngay",
                CloseButtonText = "Hủy bỏ",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteCanvasCommand.ExecuteAsync(ViewModel.CurrentCanvasId);

                ResetCanvasUI();
                ViewModel.CreateNewCanvas();

                ShowNotification("Đã xóa bảng vẽ thành công.");
            }
        }

        private void ResetCanvasUI()
        {
            DrawingCanvas.Children.Clear();
            DrawingCanvas.Children.Add(ResizeAdorner);

            DrawingCanvas.Background = new SolidColorBrush(Colors.White); 
        }

        private async void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            DeselectCurrentShape();
            TextBox nameInput = new TextBox
            {
                PlaceholderText = "Nhập tên bài vẽ...",
                Text = ViewModel.CurrentCanvasId == -1 ? $"My Art {DateTime.Now:dd/MM HH:mm}" : ViewModel.CanvasName
            };

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Lưu bài vẽ",
                Content = nameInput,
                PrimaryButtonText = "Lưu",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string nameToSave = nameInput.Text.Trim();
                if (string.IsNullOrEmpty(nameToSave))
                {
                    ShowNotification("Tên bài vẽ không được để trống!");
                    return;
                }

                await ViewModel.SaveDrawing(nameToSave, DrawingCanvas.Children);

                ShowNotification("Đã lưu bài vẽ thành công!");
            }
        }

        private async void OnNewCanvasClicked(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas.Children.Count > 1)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Tạo bảng vẽ mới",
                    Content = "Bạn có chắc chắn muốn tạo bảng vẽ mới?\n\nMọi hình vẽ chưa lưu trên màn hình hiện tại sẽ bị mất.",
                    PrimaryButtonText = "Tạo mới",
                    CloseButtonText = "Hủy bỏ",
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await confirmDialog.ShowAsync();

                if (result != ContentDialogResult.Primary) return;
            }


            ViewModel.CreateNewCanvas();

            ResetCanvasUI();

            ShowNotification("Đã tạo bảng vẽ mới.");
        }

        private void OnNoFillColorClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.FillColor = Colors.Transparent;

            if (sender is FrameworkElement element && element.Parent is StackPanel panel && panel.Parent is FlyoutPresenter flyoutPresenter)
            {
                if (flyoutPresenter.Parent is Flyout flyout)
                {
                    flyout.Hide();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var profile = App.Current.CurrentProfile;

            if (profile != null)
            {
                _pendingProfile = profile;
                ViewModel.CanvasWidth = profile.DefaultCanvasWidth;
                ViewModel.CanvasHeight = profile.DefaultCanvasHeight;

                if (!string.IsNullOrEmpty(profile.DefaultCanvasColor))
                {
                    Color bgColor = ParseColorHex(profile.DefaultCanvasColor);
                    ViewModel.CanvasBackground = new SolidColorBrush(bgColor);
                }

                ViewModel.StrokeThickness = profile.DefaultStrokeSize;

                if (!string.IsNullOrEmpty(profile.DefaultStrokeColor))
                {
                    ViewModel.StrokeColor = ParseColorHex(profile.DefaultStrokeColor);
                }

                string styleName = "Solid";
                switch (profile.DefaultStrokeStyle)
                {
                    case 1: styleName = "Dashed"; break;
                    case 2: styleName = "Dotted"; break;
                    default: styleName = "Solid"; break;
                }
                ViewModel.SetStrokeStyle(styleName);

                if (!string.IsNullOrEmpty(profile.ThemePreference))
                {
                    if (profile.ThemePreference == "Light") this.RequestedTheme = ElementTheme.Light;
                    else if (profile.ThemePreference == "Dark") this.RequestedTheme = ElementTheme.Dark;
                    else this.RequestedTheme = ElementTheme.Default; // System
                }

                ViewModel.CreateNewCanvas();

                ResetCanvasUI();

                DrawingCanvas.Background = ViewModel.CanvasBackground;

            }
        }

        private Windows.UI.Color ParseColorHex(string hex)
        {
            try
            {
                hex = hex.Replace("#", "");
                byte a = 255;
                byte r = 0;
                byte g = 0;
                byte b = 0;

                // Dạng 8 ký tự: AARRGGBB
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    r = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                }
                // Dạng 6 ký tự: RRGGBB
                else if (hex.Length == 6)
                {
                    r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                }

                return Windows.UI.Color.FromArgb(a, r, g, b);
            }
            catch
            {
                return Microsoft.UI.Colors.Black; 
            }
        }

        private void DrawingPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_pendingProfile != null)
            {
                ShowNotification($"Xin chào {_pendingProfile.UserName}, đã tải cấu hình cá nhân thành công!");
                _pendingProfile = null;
                this.Loaded -= DrawingPage_Loaded;
            }
        }

    }
}
