using Microsoft.UI.Composition.SystemBackdrops; // Cho Mica
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaintApp.Views.Pages;
using System;
using System.Linq;

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
                Type targetPageType = null;
                bool requiresProfile = false; // Flag kiểm tra trang cần profile

                switch (pageTag)
                {
                    case "Home":
                        targetPageType = typeof(HomePage);
                        break;
                    case "Draw":
                        targetPageType = typeof(DrawingPage);
                        requiresProfile = true; // Trang Vẽ Cần Profile
                        break;
                    case "Dashboard":
                        targetPageType = typeof(DashboardPage);
                        break;
                }

                if (targetPageType != null)
                {
                    // KIỂM TRA ĐIỀU KIỆN HẠN CHẾ TRUY CẬP
                    if (requiresProfile && App.Current.CurrentProfile == null)
                    {
                        // Nếu trang cần Profile VÀ Profile chưa được chọn
                        ShowMessage("Vui lòng chọn hoặc tạo Profile trước khi truy cập trang Vẽ!");

                        // Ngăn chặn chuyển trang và chuyển hướng về Dashboard/Home
                        sender.SelectedItem = sender.MenuItems.OfType<NavigationViewItem>()
                            .FirstOrDefault(item => item.Tag.ToString() == "Dashboard"); // Hoặc Home

                        ContentFrame.Navigate(typeof(DashboardPage)); // Đảm bảo người dùng quay lại Dashboard
                    }
                    else
                    {
                        // Cho phép điều hướng
                        ContentFrame.Navigate(targetPageType);
                    }
                }
            }
        }

        public void NavigateAndUpdateNavView(Type pageType, object parameter = null)
        {
            var navItem = NavView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item =>
                {
                    return item.Tag.ToString() == "Draw" && pageType == typeof(PaintApp.Views.Pages.DrawingPage);
                });

            if (navItem != null)
            {
                NavView.SelectedItem = navItem;
            }

            ContentFrame.Navigate(pageType, parameter);
        }

        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Cảnh báo",
                Content = message,
                CloseButtonText = "Đóng",
                XamlRoot = this.Content.XamlRoot // Sử dụng XamlRoot của Window
            };
            await dialog.ShowAsync();
        }

    }
}
