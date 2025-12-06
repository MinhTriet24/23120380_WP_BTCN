using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaintApp_Data.Context;
using System;
using System.IO;

namespace WinUI_PaintApp.Data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // 1. Tạo đường dẫn DB tạm thời để chạy lệnh
            // LƯU Ý: Tuyệt đối dùng System.Environment, KHÔNG dùng Windows.Storage (sẽ gây lỗi)
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "paint_app_v2.db");

            // 2. Cấu hình SQLite
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            // 3. Trả về DbContext với options vừa tạo
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}