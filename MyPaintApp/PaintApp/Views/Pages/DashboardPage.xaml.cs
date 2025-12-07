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
using Windows.UI;

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

        private Color GetColorFromHex(string hex)
        {
            try
            {
                hex = hex.Replace("#", "").Length == 6 ? $"FF{hex.Replace("#", "")}" : hex.Replace("#", "");
                if (hex.Length != 8) return Microsoft.UI.Colors.White;

                byte a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte r = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

                return Color.FromArgb(a, r, g, b);
            }
            catch { return Microsoft.UI.Colors.White; }
        }

        private async Task ShowProfileDialog(UserProfile profileToEdit = null)
        {
            bool isEditing = profileToEdit != null;
            UserProfile profile = profileToEdit ?? new PaintApp_Data.Entities.UserProfile();


            var nameInput = new TextBox { Header = "Tên Profile", Text = profile.UserName };

            var widthInput = new TextBox { Header = "Rộng (px)", Text = profile.DefaultCanvasWidth.ToString(), InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };
            var heightInput = new TextBox { Header = "Cao (px)", Text = profile.DefaultCanvasHeight.ToString(), InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };

            var themeCombo = new ComboBox { Header = "Theme Mặc định", SelectedItem = profile.ThemePreference };
            themeCombo.Items.Add("System");
            themeCombo.Items.Add("Light");
            themeCombo.Items.Add("Dark");

            var strokeSizeInput = new TextBox { Header = "Độ dày nét (1.0-10.0)", Text = profile.DefaultStrokeSize.ToString(), InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };

            var strokeStyleCombo = new ComboBox { Header = "Kiểu Nét vẽ", SelectedIndex = profile.DefaultStrokeStyle };
            strokeStyleCombo.Items.Add("Solid"); // 0
            strokeStyleCombo.Items.Add("Dashed"); // 1
            strokeStyleCombo.Items.Add("Dotted"); // 2


            var canvasColorPicker = new ColorPicker
            {
                Color = GetColorFromHex(profile.DefaultCanvasColor),
                IsMoreButtonVisible = false,
                IsColorSpectrumVisible = true,
                IsColorSliderVisible = true
            };
            var canvasColorStack = new StackPanel { Spacing = 5 };
            canvasColorStack.Children.Add(new TextBlock { Text = "Màu Nền Canvas Mặc định", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
            canvasColorStack.Children.Add(canvasColorPicker);

            var strokeColorPicker = new ColorPicker
            {
                Color = GetColorFromHex(profile.DefaultStrokeColor),
                IsMoreButtonVisible = false,
                IsColorSpectrumVisible = true,
                IsColorSliderVisible = true
            };
            var strokeColorStack = new StackPanel { Spacing = 5 };
            strokeColorStack.Children.Add(new TextBlock { Text = "Màu Nét vẽ Mặc định", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
            strokeColorStack.Children.Add(strokeColorPicker);



            var dialogStack = new StackPanel { Spacing = 12 };
            dialogStack.Children.Add(nameInput);
            dialogStack.Children.Add(themeCombo);

            var sizeStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { widthInput, heightInput } };
            dialogStack.Children.Add(sizeStack);

            var strokeStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { strokeSizeInput, strokeStyleCombo } };
            dialogStack.Children.Add(strokeStack);

            dialogStack.Children.Add(canvasColorStack);
            dialogStack.Children.Add(strokeColorStack);


            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = isEditing ? $"Chỉnh sửa Profile: {profile.UserName}" : "Tạo Profile Mới",
                Content = new ScrollViewer
                {
                    Content = dialogStack,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
                },
                PrimaryButtonText = isEditing ? "Lưu" : "Tạo",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                MaxWidth = 500
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(nameInput.Text))
                {
                    return;
                }

                profile.UserName = nameInput.Text.Trim();
                profile.ThemePreference = themeCombo.SelectedItem?.ToString() ?? "System";

                if (double.TryParse(widthInput.Text, out double width)) profile.DefaultCanvasWidth = width;
                if (double.TryParse(heightInput.Text, out double height)) profile.DefaultCanvasHeight = height;
                if (double.TryParse(strokeSizeInput.Text, out double strokeSize)) profile.DefaultStrokeSize = strokeSize;

                profile.DefaultStrokeStyle = strokeStyleCombo.SelectedIndex;

                profile.DefaultCanvasColor = $"#{canvasColorPicker.Color.A:X2}{canvasColorPicker.Color.R:X2}{canvasColorPicker.Color.G:X2}{canvasColorPicker.Color.B:X2}";
                profile.DefaultStrokeColor = $"#{strokeColorPicker.Color.A:X2}{strokeColorPicker.Color.R:X2}{strokeColorPicker.Color.G:X2}{strokeColorPicker.Color.B:X2}";

                profile.LastAccessed = DateTime.Now; // Cập nhật thời gian sửa

                await ViewModel.AddProfileCommand.ExecuteAsync(profile);
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
                App.Current.CurrentProfile = profile;

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