using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintApp_Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AvatarIcon = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultCanvasWidth = table.Column<double>(type: "REAL", nullable: false),
                    DefaultCanvasHeight = table.Column<double>(type: "REAL", nullable: false),
                    ThemePreference = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultCanvasColor = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultStrokeSize = table.Column<double>(type: "REAL", nullable: false),
                    DefaultStrokeColor = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultStrokeStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "AvatarIcon", "CreatedAt", "DefaultCanvasColor", "DefaultCanvasHeight", "DefaultCanvasWidth", "DefaultStrokeColor", "DefaultStrokeSize", "DefaultStrokeStyle", "LastAccessed", "ThemePreference", "UserName" },
                values: new object[] { 1, "Assets/DefaultAvatar.jpg", new DateTime(2025, 12, 6, 21, 16, 10, 345, DateTimeKind.Local).AddTicks(4410), "#FFFFFFFF", 600.0, 800.0, "#FF000000", 2.0, 0, new DateTime(2025, 12, 6, 21, 16, 10, 345, DateTimeKind.Local).AddTicks(4421), "Dark", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
