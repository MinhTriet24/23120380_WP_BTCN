using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintApp.Core.Interfaces;
using PaintApp_Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PaintApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {

        private readonly IUserProfileService _profileService;

        [ObservableProperty]
        private ObservableCollection<UserProfile> profiles;

        [ObservableProperty]
        private bool isLoading;

        public HomeViewModel(IUserProfileService profileService)
        {
            _profileService = profileService;
            Profiles = new ObservableCollection<UserProfile>();
            LoadProfilesCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadProfiles()
        {
            IsLoading = true;
            Profiles.Clear();

            var list = await _profileService.GetAllProfilesAsync();
            foreach ( var profile in list )
            {
                Profiles.Add(profile);
            }
            IsLoading = false;
        }

        [RelayCommand]
        public async Task AddProfile(UserProfile profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.UserName)) return;

            await _profileService.AddProfileAsync(profile);

            if (profile.Id == 0)
            {
                await LoadProfiles();
            }
            else
            {
                var existingProfile = Profiles.FirstOrDefault(p => p.Id == profile.Id);
                if (existingProfile != null)
                {
                    // --- CẬP NHẬT TẤT CẢ CÁC THUỘC TÍNH ---
                    existingProfile.UserName = profile.UserName;
                    existingProfile.ThemePreference = profile.ThemePreference;
                    existingProfile.DefaultCanvasWidth = profile.DefaultCanvasWidth;
                    existingProfile.DefaultCanvasHeight = profile.DefaultCanvasHeight;
                    existingProfile.DefaultCanvasColor = profile.DefaultCanvasColor;
                    existingProfile.DefaultStrokeSize = profile.DefaultStrokeSize;
                    existingProfile.DefaultStrokeColor = profile.DefaultStrokeColor;
                    existingProfile.DefaultStrokeStyle = profile.DefaultStrokeStyle;
                    existingProfile.LastAccessed = DateTime.Now;
                }
                await LoadProfiles();
            }
        }

    }
}
