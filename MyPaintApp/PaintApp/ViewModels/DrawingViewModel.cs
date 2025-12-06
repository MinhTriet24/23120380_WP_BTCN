using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using PaintApp.Core.Enums;

namespace PaintApp.ViewModels
{
    public partial class DrawingViewModel : ObservableObject
    {
        // Công cụ đang chọn
        [ObservableProperty]
        private ToolType currentTool = ToolType.Rectangle;

        // Độ dày nét vẽ (Mặc định 2)
        [ObservableProperty]
        private double strokeThickness = 2.0;

        // Màu nét vẽ (Mặc định Đen)
        [ObservableProperty]
        private SolidColorBrush strokeColor = new SolidColorBrush(Colors.Black);

        // Màu nền (Fill) - Tạm thời để Transparent
        [ObservableProperty]
        private SolidColorBrush fillColor = new SolidColorBrush(Colors.Transparent);
    }
}
