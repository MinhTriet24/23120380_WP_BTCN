using Microsoft.UI.Composition.SystemBackdrops; // Cho Mica
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaintApp.Views.Pages;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaintApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);


            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // Xử lý khi chọn nút Settings (nếu cần)
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                string pageTag = selectedItem.Tag.ToString();

                switch (pageTag)
                {
                    case "Home":
                        ContentFrame.Navigate(typeof(HomePage));
                        break;
                    case "Draw":
                        ContentFrame.Navigate(typeof(DrawingPage));
                        break;
                    case "Dashboard":
                        ContentFrame.Navigate(typeof(DashboardPage));
                        break;
                }
            }
        }

    }
}
