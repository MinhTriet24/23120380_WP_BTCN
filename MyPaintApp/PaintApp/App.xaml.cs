using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PaintApp.Core.Interfaces;
using PaintApp.Services;
using PaintApp.ViewModels;
using PaintApp_Data.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaintApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }
        public App()
        {
            InitializeComponent();

            Services = ConfigureServices();

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>();

            // B. Đăng ký ViewModels
            services.AddTransient<HomeViewModel>();
            services.AddTransient<DrawingViewModel>();

            // C. Đăng ký Services (Logic nghiệp vụ)
            services.AddTransient<IUserProfileService, UserProfileService>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
