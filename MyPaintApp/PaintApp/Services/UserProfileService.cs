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
            return await _context.UserProfiles.AsNoTracking().ToListAsync();
        }

        public async Task AddProfileAsync(UserProfile profile)
        {
            if (profile.Id == 0)
            {
                _context.UserProfiles.Add(profile);
            }
            else
            {
                _context.UserProfiles.Update(profile);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProfileAsync(int id)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }
    }
}