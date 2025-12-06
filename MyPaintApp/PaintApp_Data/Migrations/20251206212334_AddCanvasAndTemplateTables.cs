using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintApp_Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCanvasAndTemplateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    IconPreview = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeTemplates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 7, 4, 23, 30, 681, DateTimeKind.Local).AddTicks(472), new DateTime(2025, 12, 7, 4, 23, 30, 681, DateTimeKind.Local).AddTicks(483) });

            migrationBuilder.CreateIndex(
                name: "IX_DrawingCanvases_UserProfileId",
                table: "DrawingCanvases",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawingCanvases");

            migrationBuilder.DropTable(
                name: "ShapeTemplates");

            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed" },
                values: new object[] { new DateTime(2025, 12, 6, 22, 45, 3, 323, DateTimeKind.Local).AddTicks(1736), new DateTime(2025, 12, 6, 22, 45, 3, 323, DateTimeKind.Local).AddTicks(1761) });
        }
    }
}
