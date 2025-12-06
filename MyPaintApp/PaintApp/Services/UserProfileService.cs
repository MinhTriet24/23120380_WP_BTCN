using Microsoft.EntityFrameworkCore;
using PaintApp.Core.Interfaces;
using PaintApp_Data.Context;
using PaintApp_Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaintApp.Services
{
    public class UserProfileService : IUserProfileService
    {

        private readonly AppDbContext _context;

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserProfile>> GetAllProfilesAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

    }
}
