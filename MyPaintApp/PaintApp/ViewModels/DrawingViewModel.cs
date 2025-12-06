using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using PaintApp.Core.Enums;
using Windows.UI; // Chứa struct Color

namespace PaintApp.ViewModels
{
    public partial class DrawingViewModel : ObservableObject
    {
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
    }
}
