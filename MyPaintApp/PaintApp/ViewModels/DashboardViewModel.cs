using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintApp.Services;
using PaintApp_Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PaintApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly UserProfileService _profileService;
        private readonly CanvasService _canvasService;

        // Danh sách hiển thị trên UI
        public ObservableCollection<UserProfile> Profiles { get; } = new();

        // Biến thống kê
        [ObservableProperty] private int _totalDrawings;
        [ObservableProperty] private int _totalTemplates;

        public DashboardViewModel(UserProfileService profileService, CanvasService canvasService)
        {
            _profileService = profileService;
            _canvasService = canvasService;
        }

        [RelayCommand]
        public async Task LoadData()
        {
            // 1. Load Profiles
            var profiles = await _profileService.GetAllProfilesAsync();
            Profiles.Clear();
            foreach (var p in profiles) Profiles.Add(p);

            // 2. Load Thống kê (Dùng hàm GetAll có sẵn của CanvasService để đếm)
            var drawings = await _canvasService.GetAllCanvasesAsync();
            var templates = await _canvasService.GetAllTemplatesAsync();

            TotalDrawings = drawings.Count;
            TotalTemplates = templates.Count;
        }

        [RelayCommand]
        public async Task AddProfile(string userName)
        {
            // Tạo cấu hình mặc định cho người dùng mới
            var newProfile = new UserProfile
            {
                UserName = userName,
                DefaultCanvasWidth = 1000,
                DefaultCanvasHeight = 800,
                ThemePreference = "System",
                CreatedAt = DateTime.Now,
                LastAccessed = DateTime.Now
            };

            await _profileService.AddProfileAsync(newProfile);
            await LoadData(); // Refresh lại danh sách
        }

        [RelayCommand]
        public async Task DeleteProfile(UserProfile profile)
        {
            if (profile == null) return;
            await _profileService.DeleteProfileAsync(profile.Id);
            Profiles.Remove(profile);
        }
    }
}