using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaintApp.ViewModels;
using System;
using System.Collections.Generic;

namespace PaintApp.Views.Pages
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage()
        {
            ViewModel = App.Current.Services.GetService<DashboardViewModel>();
            this.InitializeComponent();

            this.Loaded += (s, e) => ViewModel.LoadDataCommand.Execute(null);

            BreadcrumbNav.ItemsSource = new List<string> { "Trang chủ", "Dashboard & Quản lý" };
        }

        private async void OnAddProfileClicked(object sender, RoutedEventArgs e)
        {
            TextBox input = new TextBox { PlaceholderText = "Nhập tên người dùng..." };
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Tạo Profile Mới",
                Content = input,
                PrimaryButtonText = "Tạo",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (!string.IsNullOrWhiteSpace(input.Text))
                    await ViewModel.AddProfileCommand.ExecuteAsync(input.Text);
            }
        }

        private async void OnDeleteProfileClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PaintApp_Data.Entities.UserProfile profile)
            {
                ContentDialog confirm = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Xác nhận xóa",
                    Content = $"Bạn có chắc muốn xóa profile '{profile.UserName}'?.",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy"
                };

                if (await confirm.ShowAsync() == ContentDialogResult.Primary)
                {
                    await ViewModel.DeleteProfileCommand.ExecuteAsync(profile);
                }
            }
        }

        private void OnSelectProfileClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PaintApp_Data.Entities.UserProfile profile)
            {
                var mainWindow = App.Current.Window as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.NavigateAndUpdateNavView(typeof(DrawingPage), profile);
                }
                else
                {
                    Frame.Navigate(typeof(DrawingPage), profile);
                }
            }
        }
    }
}