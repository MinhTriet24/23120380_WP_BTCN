using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintApp.Core.Interfaces;
using PaintApp.Services;
using PaintApp_Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PaintApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUserProfileService _profileService;
        private readonly CanvasService _canvasService;

        // Danh sách hiển thị trên UI
        public ObservableCollection<UserProfile> Profiles { get; } = new();

        // Biến thống kê
        [ObservableProperty] private int _totalDrawings;
        [ObservableProperty] private int _totalTemplates;

        public DashboardViewModel(IUserProfileService profileService, CanvasService canvasService)
        {
            _profileService = profileService;
            _canvasService = canvasService;
        }

        [RelayCommand]
        public async Task LoadData()
        {
            var profiles = await _profileService.GetAllProfilesAsync();
            Profiles.Clear();
            foreach (var p in profiles) Profiles.Add(p);

            var drawings = await _canvasService.GetAllCanvasesAsync();
            var templates = await _canvasService.GetAllTemplatesAsync();

            TotalDrawings = drawings.Count;
            TotalTemplates = templates.Count;
        }

        [RelayCommand]
        public async Task AddProfile(UserProfile profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.UserName)) return;

            await _profileService.AddProfileAsync(profile);

            if (profile.Id == 0)
            {
                await LoadData();
            }
            else
            {
                var existingProfile = Profiles.FirstOrDefault(p => p.Id == profile.Id);
                if (existingProfile != null)
                {
                    existingProfile.UserName = profile.UserName;
                    existingProfile.ThemePreference = profile.ThemePreference;
                    existingProfile.DefaultCanvasWidth = profile.DefaultCanvasWidth;
                    existingProfile.DefaultCanvasHeight = profile.DefaultCanvasHeight;
                    existingProfile.LastAccessed = DateTime.Now;
                    // ... Cập nhật các thuộc tính cấu hình khác
                }
            }
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