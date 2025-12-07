using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PaintApp.ViewModels;
using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaintApp.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {

        public HomeViewModel ViewModel { get; }

        public HomePage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetService<HomeViewModel>();
        }

        private async void OnAddProfileClicked(object sender, RoutedEventArgs e)
        {
            await ShowProfileDialog();
            ViewModel.LoadProfilesCommand.Execute(null);
        }

        private Color GetColorFromHex(string hex)
        {
            // Sao chép hàm GetColorFromHex từ DashboardPage.xaml.cs
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

        private void OnSelectProfileClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PaintApp_Data.Entities.UserProfile profile)
            {
                App.Current.CurrentProfile = profile; // Đặt profile hiện tại

                var mainWindow = App.Current.Window as MainWindow;

                if (mainWindow != null)
                {
                    // Chuyển hướng và cập nhật NavigationView
                    mainWindow.NavigateAndUpdateNavView(typeof(DrawingPage), profile);
                }
                else
                {
                    // Hoặc chuyển hướng đơn giản nếu không dùng MainWindow có NavigationView
                    Frame.Navigate(typeof(DrawingPage), profile);
                }
            }
        }
    }
}
