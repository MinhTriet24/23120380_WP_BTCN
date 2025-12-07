using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintApp.Core.Interfaces
{
    public interface IUserProfileService
    {

        Task<List<UserProfile>> GetAllProfilesAsync();
        Task AddProfileAsync(UserProfile profile);

        //Task Update

        Task DeleteProfileAsync(int id);

    }
}
