using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaintApp.Core.Enums;
using PaintApp.Services;
using PaintApp_Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using WinUI_PaintApp.Core.Helpers; // Chứa struct Color

namespace PaintApp.ViewModels
{
    public partial class DrawingViewModel : ObservableObject
    {
        private readonly CanvasService _canvasService;

        [ObservableProperty]
        private ToolType currentTool = ToolType.Line;

        [ObservableProperty]
        private double strokeThickness = 2.0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StrokeBrush))] // Khi màu đổi, tự báo cho Brush đổi theo
        private Color _strokeColor = Colors.Black;

        public SolidColorBrush StrokeBrush => new SolidColorBrush(StrokeColor);

        [ObservableProperty]
        private DoubleCollection strokeDashArray;


        [ObservableProperty]
        private Color _fillColor = Colors.White;

        [ObservableProperty] private double _canvasWidth = 800;
        [ObservableProperty] private double _canvasHeight = 600;
        [ObservableProperty] private SolidColorBrush _canvasBackground = new SolidColorBrush(Colors.White);

        public ObservableCollection<ShapeTemplate> Templates { get; } = new();

        public DrawingViewModel(CanvasService canvasService)
        {
            _canvasService = canvasService;
            //LoadTemplatesCommand.Execute(null);
        }

        public void SetStrokeStyle(string style)
        {
            if (style == "Solid")
            {
                StrokeDashArray = null;
            }
            else if (style == "Dashed")
            {
                // 4 là độ dài nét, 2 là khoảng hở
                StrokeDashArray = new DoubleCollection { 4, 2 };
            }
            else if (style == "Dotted")
            {
                StrokeDashArray = new DoubleCollection { 1, 2 };
            }
        }

        [RelayCommand]
        public async Task SaveDrawing(UIElementCollection shapes)
        {
            if (shapes == null) return;

            string json = ShapeSerializer.Serialize(shapes);

            var canvas = new DrawingCanvas
            {
                Name = "My Art " + DateTime.Now.ToString("HH:mm:ss"),
                Width = CanvasWidth,
                Height = CanvasHeight,
                BackgroundColor = CanvasBackground.Color.ToString(),
                DataJson = json,
                UserProfileId = 1
            };

            await _canvasService.SaveCanvasAsync(canvas);
        }

        [RelayCommand]
        public async Task LoadTemplates()
        {
            var list = await _canvasService.GetAllTemplatesAsync();
            Templates.Clear();
            foreach (var item in list) Templates.Add(item);
        }

        [RelayCommand]
        public async Task DeleteTemplate(int id)
        {
            await _canvasService.DeleteTemplateAsync(id);
            await LoadTemplates();
        }
    }
}
