using Microsoft.EntityFrameworkCore;
using PaintApp_Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PaintApp_Data.Context
{
    public class AppDbContext : DbContext
    {

        public DbSet<Entities.UserProfile> UserProfiles { get; set; }
        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = ApplicationData.Current.LocalFolder.Path;
            DbPath = Path.Combine(folder, "paint_app.db");
            System.Diagnostics.Debug.WriteLine($"DB PATH: {DbPath}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Data Seeding
            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    Id = 1,
                    UserName = "Admin",
                    ThemePreference = "Dark"
                }
            );
        }
    }
}
