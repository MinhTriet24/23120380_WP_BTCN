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
        private SolidColorBrush strokeColor = new SolidColorBrush(Colors.Black);

        [ObservableProperty]
        private SolidColorBrush fillColor = new SolidColorBrush(Colors.Transparent);
    }
}
