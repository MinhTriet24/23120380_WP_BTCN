using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PaintApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
            var inputNameTextBox = new TextBox()
            {
                PlaceholderText = "Nhập vào tên của bạn",
                Height = 32
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Tạo hồ sơ mới",
                PrimaryButtonText = "Tạo",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                Content = inputNameTextBox
            };

            var result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                string name = inputNameTextBox.Text;
                await ViewModel.AddProfileCommand.ExecuteAsync(name);
            }
        }
    }
}
