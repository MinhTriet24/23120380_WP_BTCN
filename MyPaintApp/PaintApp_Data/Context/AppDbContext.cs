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
        public DbSet<DrawingCanvas> DrawingCanvases { get; set; }
        public DbSet<ShapeTemplate> ShapeTemplates { get; set; }
        public string DbPath { get; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var folder = System.Environment.SpecialFolder.LocalApplicationData;
                var path = System.Environment.GetFolderPath(folder);
                var dbPath = System.IO.Path.Join(path, "paint_app_v3.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
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
