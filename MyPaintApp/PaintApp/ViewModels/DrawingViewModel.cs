using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintApp.Core.Enums;
using PaintApp.Core.Helpers;
using PaintApp.Services;
using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

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

        public ObservableCollection<DrawingCanvas> SavedCanvases { get; } = new();

        public event Action<List<Shape>, string> OnCanvasLoaded;

        [ObservableProperty]
        private int _currentCanvasId = -1;

        public DrawingViewModel(CanvasService canvasService)
        {
            _canvasService = canvasService;
            try
            {
                LoadTemplatesCommand.Execute(null);
            }
            catch
            {
            }
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
        public async Task AddToTemplate(Shape shape)
        {
            if (shape == null) return;

            string json = ShapeSerializer.SerializeSingle(shape);

            var newTemplate = new ShapeTemplate
            {
                Name = $"{shape.GetType().Name} - {DateTime.Now:HH:mm:ss}",
                ShapeJson = json,
            };

            await _canvasService.AddTemplateAsync(newTemplate);

            await LoadTemplates();
        }

        [RelayCommand]
        public async Task SaveDrawing(object collection)
        {
            var shapes = collection as UIElementCollection;

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

        [RelayCommand]
        public async Task LoadSavedCanvases()
        {
            var list = await _canvasService.GetAllCanvasesAsync();
            SavedCanvases.Clear();
            foreach (var item in list) SavedCanvases.Add(item);
        }

        [RelayCommand]
        public async Task OpenCanvas(int id)
        {
            var canvasData = await _canvasService.GetCanvasByIdAsync(id);
            if (canvasData != null && !string.IsNullOrEmpty(canvasData.DataJson))
            {
                var shapes = ShapeSerializer.Deserialize(canvasData.DataJson);
                CurrentCanvasId = canvasData.Id;

                OnCanvasLoaded?.Invoke(shapes, canvasData.BackgroundColor);

                CanvasWidth = canvasData.Width;
                CanvasHeight = canvasData.Height;
            }
        }

        [RelayCommand]
        public async Task DeleteCanvas(int id)
        {
            await _canvasService.DeleteCanvasAsync(id);

            var itemToRemove = SavedCanvases.FirstOrDefault(c => c.Id == id);
            if (itemToRemove != null)
            {
                SavedCanvases.Remove(itemToRemove);
            }

            if (id == CurrentCanvasId)
            {
                CurrentCanvasId = -1;
            }
        }

        public void CreateNewCanvas()
        {
            CurrentCanvasId = -1;
        }
    }
}
