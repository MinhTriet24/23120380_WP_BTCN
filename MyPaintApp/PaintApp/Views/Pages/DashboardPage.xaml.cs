using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PaintApp.ViewModels;
using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        private async Task ShowProfileDialog(UserProfile profile = null)
        {
            // 1. Khởi tạo giá trị ban đầu (nếu là chỉnh sửa)
            bool isEditing = profile != null;
            profile ??= new PaintApp_Data.Entities.UserProfile(); // Nếu null thì tạo mới

            // 2. Thiết lập các controls nhập liệu
            var nameInput = new TextBox { Header = "Tên Profile", Text = profile.UserName };
            var widthInput = new TextBox { Header = "Rộng (px)", Text = profile.DefaultCanvasWidth.ToString(), InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };
            var heightInput = new TextBox { Header = "Cao (px)", Text = profile.DefaultCanvasHeight.ToString(), InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };

            var themeCombo = new ComboBox { Header = "Theme Mặc định", SelectedItem = profile.ThemePreference };
            themeCombo.Items.Add("System");
            themeCombo.Items.Add("Light");
            themeCombo.Items.Add("Dark");

            var dialogStack = new StackPanel { Spacing = 12 };
            dialogStack.Children.Add(nameInput);
            dialogStack.Children.Add(new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { widthInput, heightInput } });
            dialogStack.Children.Add(themeCombo);
            // Bạn cần thêm các controls cho StrokeSize, StrokeColor, StrokeStyle ở đây

            // 3. Tạo ContentDialog
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = isEditing ? $"Chỉnh sửa Profile: {profile.UserName}" : "Tạo Profile Mới",
                Content = dialogStack,
                PrimaryButtonText = isEditing ? "Lưu" : "Tạo",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(nameInput.Text))
                {
                    // Có thể thêm thông báo lỗi
                    return;
                }

                // 4. Cập nhật dữ liệu từ Dialog
                profile.UserName = nameInput.Text.Trim();
                profile.ThemePreference = themeCombo.SelectedItem?.ToString() ?? "System";

                // Cần xử lý lỗi chuyển đổi từ string sang double/int
                if (double.TryParse(widthInput.Text, out double width)) profile.DefaultCanvasWidth = width;
                if (double.TryParse(heightInput.Text, out double height)) profile.DefaultCanvasHeight = height;

                // 5. Gọi Service/ViewModel
                await ViewModel.AddProfileCommand.ExecuteAsync(profile);
                // Lệnh AddProfileCommand đã được thiết kế để xử lý cả Add và Update dựa trên profile.Id
            }
        }

        private async void OnAddProfileClicked(object sender, RoutedEventArgs e)
        {
            await ShowProfileDialog();
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

        private async void OnEditProfileClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PaintApp_Data.Entities.UserProfile profile)
            {
                await ShowProfileDialog(profile); // Gọi hàm chỉnh sửa với dữ liệu profile hiện tại
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