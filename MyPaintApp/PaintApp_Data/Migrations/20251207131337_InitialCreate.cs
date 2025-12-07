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

            migrationBuilder.CreateTable(
                name: "DrawingCanvases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Width = table.Column<double>(type: "REAL", nullable: false),
                    Height = table.Column<double>(type: "REAL", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    DataJson = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailImage = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserProfileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingCanvases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingCanvases_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShapeTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    ShapeJson = table.Column<string>(type: "TEXT", nullable: true),
                    IconPreview = table.Column<string>(type: "TEXT", nullable: true),
                    UserProfileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShapeTemplates_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "AvatarIcon", "CreatedAt", "DefaultCanvasColor", "DefaultCanvasHeight", "DefaultCanvasWidth", "DefaultStrokeColor", "DefaultStrokeSize", "DefaultStrokeStyle", "LastAccessed", "ThemePreference", "UserName" },
                values: new object[] { 1, "Assets/DefaultAvatar.jpg", new DateTime(2025, 12, 7, 20, 13, 32, 618, DateTimeKind.Local).AddTicks(4281), "#FFFFFFFF", 600.0, 800.0, "#FF000000", 2.0, 0, new DateTime(2025, 12, 7, 20, 13, 32, 618, DateTimeKind.Local).AddTicks(4300), "Dark", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_DrawingCanvases_UserProfileId",
                table: "DrawingCanvases",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ShapeTemplates_UserProfileId",
                table: "ShapeTemplates",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawingCanvases");

            migrationBuilder.DropTable(
                name: "ShapeTemplates");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
