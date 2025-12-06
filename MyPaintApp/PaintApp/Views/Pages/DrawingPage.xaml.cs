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
        { }

        private void OnCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
        { }

        private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        { }

        private void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
        { }

    }
}
