using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaintApp.Core.Interfaces;
using PaintApp_Data.Entities;
using System;

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
            //LoadProfilesCommand.Execute(null);
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
        public async Task AddProfile(string name)
        {

            if(String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var profile = new UserProfile
            {
                UserName = name,
            };

            await _profileService.AddProfileAsync(profile);

            Profiles.Add(profile);
        }

    }
}
