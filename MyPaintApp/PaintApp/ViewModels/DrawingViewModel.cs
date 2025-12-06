using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using PaintApp.Core.Enums;

namespace PaintApp.ViewModels
{
    public partial class DrawingViewModel : ObservableObject
    {
        [ObservableProperty]
        private ToolType currentTool = ToolType.Line;

        [ObservableProperty]
        private double strokeThickness = 2.0;

        [ObservableProperty]
        private DoubleCollection strokeDashArray; //null - nét liền, value - nét đứt

        [ObservableProperty]
        private SolidColorBrush strokeColor = new SolidColorBrush(Colors.Black);

        [ObservableProperty]
        private SolidColorBrush fillColor = new SolidColorBrush(Colors.Transparent);

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
